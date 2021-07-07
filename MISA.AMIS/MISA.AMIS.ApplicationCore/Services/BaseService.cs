using Microsoft.AspNetCore.Http;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.ApplicationCore.Interfaces;
using MISA.AMIS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.AMIS.ApplicationCore
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        #region Declare
        IBaseRepository<TEntity> _baseRepository;
        ServiceResult _serviceResult = null;
        IEnumerable<TEntity> _entityDbList = null;
        #endregion

        #region Constructer
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            _serviceResult = new ServiceResult()
            {
                MISACode = MISACode.Success
            };
            _entityDbList = new List<TEntity>();
        }
        #endregion

        #region Methods
        public IEnumerable<TEntity> GetEntities()
        {
            var entities = _baseRepository.GetEntities();
            return entities;
        }

        public TEntity GetEntityById(Guid entityId)
        {
            var entity = _baseRepository.GetEntityById(entityId);
            return entity;
        }

        public virtual ServiceResult Insert(TEntity entity)
        {
            entity.EntityState = EntityState.Add;

            //1. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);

            if (isValid)
            {
                _serviceResult.Data = _baseRepository.Insert(entity);
                _serviceResult.MISACode = MISACode.Valid;
                _serviceResult.Messasge = "Thêm thành công";
            }
            else
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
            }

            return _serviceResult;
        }

        public ServiceResult Update(Guid entityId, TEntity entity)
        {
            entity.EntityState = EntityState.Update;

            //1. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);
            if (isValid)
            {
                _serviceResult.Data = _baseRepository.Update(entityId, entity);
                _serviceResult.Messasge = "Sửa thành công";
            }
            else
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
            }
            return _serviceResult;
        }

        public ServiceResult Delete(Guid entityId)
        {
            _serviceResult.Data = _baseRepository.Delete(entityId);
            return _serviceResult;
        }

        /// <summary>
        /// Validate tất cả
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool Validate(TEntity entity)
        {
            var isValid = true;

            //1. Đọc các property
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                //1.1 Kiểm tra xem  có attribute cần phải validate không
                if (isValid && property.IsDefined(typeof(IRequired), false))
                {
                    //1.1.1 Check bắt buộc nhập
                    isValid = validateRequired(entity, property);
                }

                if (isValid && property.IsDefined(typeof(IDuplicate), false))
                {
                    //1.1.2 Check trùng
                    isValid = validateDuplicate(entity, property);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validate từng màn hình
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// CREATED BY: DVHAI (07/07/2021)
        protected virtual bool validateCustom(TEntity entity)
        {
            return true;
        }

        /// <summary>
        /// Validate bắt buộc nhập
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool validateRequired(TEntity entity, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            var propertyName = propertyInfo.Name;
            var propertyValue = propertyInfo.GetValue(entity);
            var propertyDisplayName = getAttributeDisplayName(propertyName);

            if (propertyValue == null)
            {
                isValid = false;

                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = "Có lỗi xảy ra, vui lòng kiểm tra lại.";
                _serviceResult.Data = new { devMsg = $"{propertyDisplayName} không được trống", userMsg = $"{propertyDisplayName} không được trống" };
            }

            return isValid;
        }

        /// <summary>
        /// Validate trùng
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool validateDuplicate(TEntity entity, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            var propertyName = propertyInfo.Name;
            var propertyDisplayName = getAttributeDisplayName(propertyName);

            //Tùy chỉnh nguồn dữ liệu để validate, trạng thái thêm hoắc sửa
            var entityDuplicate = _baseRepository.GetEntityByProperty(entity, propertyInfo);

            if (entityDuplicate != null)
            {
                isValid = false;

                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = "Có lỗi xảy ra, vui lòng kiểm tra lại.";
                _serviceResult.Data = new { devMsg = $"{propertyDisplayName} bị trùng", userMsg = $"{propertyDisplayName} bị trùng" };
            }

            return isValid;
        }



        /// <summary>
        /// Lấy tên hiển thị của trường trong entity
        /// </summary>
        /// <param name="attributeName">Tên thuộc tính</param>
        /// <returns>Tên hiển thị</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private String getAttributeDisplayName(string attributeName)
        {
            //Gán mặc định bằng tên thuộc tính
            var res = attributeName;
            try
            {
                res = typeof(TEntity).GetProperty(attributeName).GetCustomAttributes(typeof(DisplayAttribute),
                                               false).Cast<DisplayAttribute>().Single().Name;
            }
            catch { }
            return res;
        }
        #endregion
    }
}
