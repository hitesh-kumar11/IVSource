using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaPageMenuType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public string NavigationLabel { get; set; }
        [Display(Name = "Enable")]
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
