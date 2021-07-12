using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="limit">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        ServiceResult GetEmployeesFilterPaging(string filterValue, int limit, int offset);

        /// <summary>
        /// Lấy mã nhân viên mới
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        string GetNewEmployeeCode();

        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <param name="employeeCode">Mã của nhân viên</param>
        /// <returns></returns>
        /// CREATED BY: DVHAI 09/07/2021
        public Employee GetEmployeeByCode(string employeeCode);


        /// <summary>
        /// Xuất excel
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public MemoryStream Export(CancellationToken cancellationToken, string filterValue);
    }
}
