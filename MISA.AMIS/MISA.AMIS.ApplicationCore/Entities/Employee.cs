using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MISA.AMIS.ApplicationCoore.Entities
{
    /// <summary>
    /// Thực thể nhân viên
    /// </summary>
    public class Employee : BaseEntity
    {
        #region Property
        /// <summary>
        /// Id nhân viên
        /// </summary>
        [Key]
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [IDuplicate]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Họ và tên nhân viên
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Tee phòng ban
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Số chứng minh thư/căn cước công dân
        /// </summary>
        public string IdentityNumber { get; set; }

        /// <summary>
        /// Ngày cấp CMND
        /// </summary>
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Nơi cấp CMND
        /// </summary>
        public string IdentityPlace { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string EmployeePosition { get; set; }

        /// <summary>
        /// Địa chỉ nhân viên
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Số tk ngân hàng
        /// </summary>
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public string BankBranchName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BankProvinceName { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [IDuplicate]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [IDuplicate]
        public string TelephoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [IDuplicate]
        public string Email { get; set; }
        #endregion
    }
}
