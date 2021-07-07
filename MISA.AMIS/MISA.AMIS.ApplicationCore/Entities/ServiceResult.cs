using MISA.AMIS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Entities
{
    public class ServiceResult
    {
        public object Data { get; set; }
        public string Messasge { get; set; }
        public MISACode MISACode { get; set; }
    }
}
