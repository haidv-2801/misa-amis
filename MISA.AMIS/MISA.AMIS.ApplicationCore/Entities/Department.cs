using MISA.AMIS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MISA.AMIS.ApplicationCoore.Entities
{
    /// <summary>
    /// Thực thể phòng ban
    /// </summary>
    public class Department : BaseEntity
    {
        #region Property

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        [Key]
        [Display(Name ="Mã phòng ban")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Display(Name = "Tên phòng ban")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        #endregion
    }
}
