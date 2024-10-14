using System;
using System.Collections.Generic;

namespace IVSource.Models
{
    public partial class IvsExceptionMaster
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
