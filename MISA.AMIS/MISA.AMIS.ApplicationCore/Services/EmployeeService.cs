using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        #region Declare
        IEmployeeRepository _employeeRepository;
        #endregion

        #region Constructer
        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IEnumerable<Employee> GetEmployeesPaging(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lấy mã mới nhất của nhân viên cộng với 1
        /// </summary>
        /// <returns>Mã nhân viên</returns>
        ///CREATED BY: HAIDV 09/07/2021
        public string GetNewEmployeeCode()
        {
            return _employeeRepository.GetNewEmployeeCode();
        }

        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <returns>Nhân viên</returns>
        /// CREATED BY: HAIDV 09/07/2021
        public Employee GetEmployeeByCode(string employeeCode)
        {
            return _employeeRepository.GetEmployeeByCode(employeeCode);
        }
        #endregion
    }
}
