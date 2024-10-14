using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class IvsVisaCompanyMaster
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }
        [Required]
        [Display(Name = "Enable")]
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
