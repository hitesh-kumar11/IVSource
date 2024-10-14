using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVSource.Models
{
    public partial class IvsVisaClientIpRegistration
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SerialNum { get; set; }
        [Display(Name = "Ip Address")]
        [Required(ErrorMessage ="Please enter valid IP Address.")]
        //[RegularExpression(@"", ErrorMessage ="Please enter valid IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Is Enabled")]
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        [Required]
        public string Company { get; set; }
        [NotMapped]
        public List<IvsVisaCompanyMaster> CompanyList { get; set; }
        [NotMapped]
        public List<IvsUserDetails> UserDetail { get; set; }
    }
}
