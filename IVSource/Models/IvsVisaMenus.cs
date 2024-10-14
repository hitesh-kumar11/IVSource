using System;
using System.Collections.Generic;

namespace IVSource.Models
{
    public partial class IvsVisaMenus
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int Priority { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
