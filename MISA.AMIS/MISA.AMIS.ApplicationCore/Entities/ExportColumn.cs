using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.AMIS.ApplicationCore.Entities
{
    public class ExportColumn
    {
        public int ColumnId { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public int Width { get; set; }
    }
}
