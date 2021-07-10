using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Lấy mã mới nhất của nhân viên cộng với 1
        /// </summary>
        /// <returns>Mã nhân viên</returns>
        ///CREATED BY: HAIDV 09/07/2021
        public string GetNewEmployeeCode()
        {
            var employeeCode = _employeeRepository.GetNewEmployeeCode();
            try
            {
                employeeCode = NextEmployeeCode(employeeCode);
            }
            catch (Exception) { }

            return employeeCode;
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

        /// <summary>
        /// Tính toán mã nhân viên mới nhất
        /// </summary>
        /// <param name="employeeCode">Mã cũ</param>
        /// <returns></returns>
        private string NextEmployeeCode(string employeeCode)
        {
            int firstDigitIndex = employeeCode.IndexOfAny(new char[]
                { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            string prefix = employeeCode.Substring(0, firstDigitIndex);
            string nextNum = (int.Parse(employeeCode.Substring(firstDigitIndex)) + 1).ToString().PadLeft(5, '0');
            return string.Concat(prefix, nextNum);
        }

        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="limit">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public ServiceResult GetEmployeesFilterPaging(string filterValue, int limit, int offset)
        {
            var employeeFilterPaging = _employeeRepository.GetEmployeesFilterPaging(filterValue, limit, offset).ToList();
            int totalRecord =_employeeRepository.GetTotalEmployeeFilter(filterValue);

            _serviceResult.Data = new
            {
                totalRecord = totalRecord,
                totalPage = totalRecord % limit == 0 ? (totalRecord / limit) : (totalRecord / limit) + 1,
                pageSize = limit,
                pageNumber = offset,
                data = employeeFilterPaging
            };

            return _serviceResult;
        }
        #endregion
    }
}
