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
        [IRequired]
        [Display(Name ="Mã nhân viên")]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Họ và tên nhân viên
        /// </summary>
        [Display(Name ="Tên nhân viên")]
        public string EmployeeName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        [Display(Name = "Giới tính")]
        public int? Gender { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        [IRequired]
        [Display(Name = "Mã phòng ban")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Tee phòng ban
        /// </summary>
        [Display(Name = "Tên phòng ban")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Số chứng minh thư/căn cước công dân
        /// </summary>
        [Display(Name = "Số CMND")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// Ngày cấp CMND
        /// </summary>
        [Display(Name = "Ngày cấp")]
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Nơi cấp CMND
        /// </summary>
        [Display(Name = "Nơi cấp")]
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
        [Display(Name = "Số TK ngân hàng")]
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
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [Display(Name = "Điện thoại cố định")]
        public string TelephoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        [IEmailFormat]
        public string Email { get; set; }
        #endregion
    }
}
