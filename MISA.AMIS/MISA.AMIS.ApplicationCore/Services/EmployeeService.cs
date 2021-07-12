using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.ApplicationCore.Interfaces;
using MISA.AMIS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
        /// <param name="pageSize">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public ServiceResult GetEmployeesFilterPaging(string filterValue, int pageSize, int pageNumber)
        {
            var employeeFilterPaging = _employeeRepository.GetEmployeesFilterPaging(filterValue, pageSize, pageNumber).ToList();
            int totalRecord = _employeeRepository.GetTotalEmployeeFilter(filterValue);

            _serviceResult.Data = new
            {
                totalRecord = totalRecord,
                totalPage = totalRecord % pageSize == 0 ? (totalRecord / pageSize) : (totalRecord / pageSize) + 1,
                pageSize = pageSize,
                pageNumber = pageNumber,
                data = employeeFilterPaging
            };

            return _serviceResult;
        }

        /// <summary>
        /// Validate tùy chỉn theo màn hình nhân viên
        /// </summary>
        /// <param name="entity">Thực thể nhân viên</param>
        /// <returns>(true-false)</returns>
        protected override bool ValidateCustom(Employee employee)
        {
            bool isValid = true;

            //1. Đọc các property
            var properties = employee.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (isValid && property.IsDefined(typeof(IDuplicate), false))
                {
                    //1.1 Check trùng
                    isValid = ValidateDuplicate(employee, property);
                }

                if (isValid && property.IsDefined(typeof(IEmailFormat), false))
                {
                    //1.2 Kiểm tra định dạng email
                    isValid = ValidateEmail(employee, property);
                }
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
        private bool ValidateDuplicate(Employee employee, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Tên hiển thị
            var propertyDisplayName = GetAttributeDisplayName(propertyName);

            //3. Tùy chỉnh nguồn dữ liệu để validate, trạng thái thêm hoắc sửa
            var entityDuplicate = _employeeRepository.GetEntityByProperty(employee, propertyInfo);

            if (entityDuplicate != null)
            {
                isValid = false;

                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_Duplicate, propertyDisplayName);
            }

            return isValid;
        }

        private bool ValidateEmail(Employee employee, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Tên hiển thị
            var propertyDisplayName = GetAttributeDisplayName(propertyName);

            //3. Gía trị
            var value = propertyInfo.GetValue(employee);

            //Không validate required
            if (string.IsNullOrEmpty(value.ToString()))
                return isValid;


            isValid = new EmailAddressAttribute().IsValid(value.ToString());

            if (!isValid)
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_NotFormat, propertyDisplayName);
            }

            return isValid;
        }
        #endregion
    }
}
