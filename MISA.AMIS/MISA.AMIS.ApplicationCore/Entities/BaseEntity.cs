using MISA.AMIS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IRequired : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IDuplicate : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IPrimaryKey : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IEmailFormat : Attribute
    {

    }

    public class BaseEntity
    {
        /// <summary>
        /// Trạng thái của Entity
        /// </summary>
        public EntityState EntityState { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Trạng thái insert của entity
        /// </summary>
        public List<String> Status { get; set; }
    }
}
