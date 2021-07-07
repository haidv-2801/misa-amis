using Microsoft.AspNetCore.Http;
using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public interface IBaseService<TEntity>
    {

        /// <summary>
        /// Lấy danh sách bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        IEnumerable<TEntity> GetEntities();

        /// <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        TEntity GetEntityById(Guid entityId);

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        ServiceResult Insert(TEntity entity);

        /// <summary>
        /// Cập nhập thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        ServiceResult Update(Guid entityId, TEntity entity);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Số dòng bị xóa</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        ServiceResult Delete(Guid entityId);

        /// <summary>
        /// Đọc file excel
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (07/07/2021)
        //Task<ServiceResult> readExcelFile(IFormFile formFile, CancellationToken cancellationToken);

        /// <summary>
        /// Thêm nhiều bản ghi
        /// </summary>
        /// <param name="ieEntities"></param>
        /// <returns></returns>\
        /// CREATED BY: DVHAI (07/07/2021)
        //ServiceResult MultiInsert(IEnumerable<TEntity> ieEntities);
    }
}
