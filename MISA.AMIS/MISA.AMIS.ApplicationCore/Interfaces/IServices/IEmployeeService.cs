using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public interface IEmployeeService : IBaseService<Employee>
    {

        /// <summary>
        /// Danh sách nhân viên phân trang
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (28/06/2021)
        public IEnumerable<Employee> GetEmployeesPaging(int limit, int offset);

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
    }
}
