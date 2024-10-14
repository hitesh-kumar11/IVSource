using System;
using System.Collections.Generic;

namespace IVSource.Models
{
    public partial class IvsUserTerminalId
    {
        public int SerialNum { get; set; }
        public string UserId { get; set; }
        public string TerminalId { get; set; }
        public int? IsUsed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string SessionID { get; set; }
    }
}
