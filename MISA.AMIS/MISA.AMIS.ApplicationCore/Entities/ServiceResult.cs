using MISA.AMIS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Entities
{
    public class ServiceResult
    {
        //Lưu dữ liệu được trả về, bao gồm cả câu thông báo
        public object Data { get; set; }

        //Lưu câu thông báo
        public string Messasge { get; set; }

        //Lưu mã MISA
        public MISACode MISACode { get; set; }
    }
}
