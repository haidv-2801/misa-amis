using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    /// <summary>
    /// Interface danh mục nhân viên
    /// </summary>
    /// CREATED BY: DVHAI (07/07/2021)
    public interface IEmployeeRepository:IBaseRepository<Employee>
    {
        /// <summary>
        /// Lấy thông tin nhân viên theo mã 
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>Bản ghi nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        Employee GetEmployeeByCode(string employeeCode);

        /// <summary>
        /// Lấy mã nhân viên mới
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        string GetNewEmployeeCode();

        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="limit">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        IEnumerable<Employee> GetEmployeesFilterPaging(string filterValue, int limit, int offset);

        /// <summary>
        /// Lấy tổng số bản ghi sau khi lọc
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <returns>Tổng bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        int GetTotalEmployeeFilter(string filterValue);
    }
}
