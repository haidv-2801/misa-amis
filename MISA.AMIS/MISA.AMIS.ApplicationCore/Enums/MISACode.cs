using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.Entities
{
    public enum MISACode
    {
        /// <summary>
        /// Hợp lệ
        /// </summary>
        Valid = 100,

        /// <summary>
        /// Không hợp lệ
        /// </summary>
        InValid = 200,

        /// <summary>
        /// Thành công  
        /// </summary>
        Success = 900,

        /// <summary>
        /// Lỗi exception
        /// </summary>
        Exception = 500
    }

    public enum EntityState
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }
}
