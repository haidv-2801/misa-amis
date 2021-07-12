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
    /// <summary>
    /// Service dùng chung
    /// </summary>
    /// <typeparam name="TEntity">Loại thực thể</typeparam>
    /// CREATED BY: DVHAI (11/07/2021)
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        #region Declare
        IBaseRepository<TEntity> _baseRepository;
        protected ServiceResult _serviceResult = null;
        #endregion

        #region Constructer
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            _serviceResult = new ServiceResult()
            {
                MISACode = MISACode.Success
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        /// CREATED BY: DVHAI 11/07/2021
        public IEnumerable<TEntity> GetEntities()
        {
            var entities = _baseRepository.GetEntities();
            return entities;
        }

        /// <summary>
        /// Lấy bản ghi theo Id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi duy nhất</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public TEntity GetEntityById(Guid entityId)
        {
            var entity = _baseRepository.GetEntityById(entityId);
            return entity;
        }

        /// <summary>
        /// Thêm một thực thể
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public virtual ServiceResult Insert(TEntity entity)
        {
            entity.EntityState = EntityState.Add;

            //1. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);

            //2. Sử lí lỗi tương ứng
            if (isValid)
            {
                _serviceResult.Data = _baseRepository.Insert(entity);
                _serviceResult.MISACode = MISACode.Valid;
                _serviceResult.Messasge = Properties.Resources.Msg_Success;
            }
            else
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
            }

            //3. Trả về kế quả
            return _serviceResult;
        }

        /// <summary>
        /// Cập nhập thông tin bản ghi 
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public ServiceResult Update(Guid entityId, TEntity entity)
        {
            //1. Trạng thái
            entity.EntityState = EntityState.Update;

            //2. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);
            if (isValid)
            {
                _serviceResult.Data = _baseRepository.Update(entityId, entity);
                _serviceResult.MISACode = MISACode.Valid;
                _serviceResult.Messasge = Properties.Resources.Msg_Success;
            }
            else
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
            }
            //3. Trả về kế quả
            return _serviceResult;
        }

        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
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
                    isValid = ValidateRequired(entity, property);
                }
            }

            //2. Validate tùy chỉnh từng màn hình
            if (isValid)
            {
                isValid = ValidateCustom(entity);
            }

            return isValid;
        }

        /// <summary>
        /// Validate từng màn hình
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// CREATED BY: DVHAI (07/07/2021)
        protected virtual bool ValidateCustom(TEntity entity)
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
        private bool ValidateRequired(TEntity entity, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Giấ trị
            var propertyValue = propertyInfo.GetValue(entity);

            //3. Tên hiển thị
            var propertyDisplayName = GetAttributeDisplayName(propertyName);

            if (string.IsNullOrEmpty(propertyValue.ToString()))
            {
                isValid = false;

                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_Required, propertyDisplayName);
            }

            return isValid;
        }

        /// <summary>
        /// Lấy tên hiển thị của trường trong entity
        /// </summary>
        /// <param name="attributeName">Tên thuộc tính</param>
        /// <returns>Tên hiển thị</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        protected String GetAttributeDisplayName(string attributeName)
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

        /// <summary>
        /// Trả về dữ liệu với kiểu dữ liệu truyền vào
        /// </summary>
        /// <param name="type">Kiểu dữ liệu</param>
        /// <param name="value">Giá trị kiểu string</param>
        /// <returns>Kiểu dữ liệu động</returns>
        protected dynamic convertValue(Type type, string value)
        {
            dynamic res = null;
            
            if (string.IsNullOrEmpty(value))
                return res;

            //Lấy ra kiểu dữ liệu chuẩn
            if (type.Name == "Nullable`1")
            {
                type = Nullable.GetUnderlyingType(type);
                if (type.Name == "DateTime")
                    value = String.Join("-", value.Split('/').Reverse());
            }
            res = Convert.ChangeType(value, type);

            return res;
        }
        #endregion
    }
}
