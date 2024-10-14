using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaSubscribeToUs : MemberPageBase
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Designation { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Full_Address { get; set; }
        //[Required]
        public string GSTIN_Number { get; set; }

        [Required]
        [RegularExpression("([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone Number")]
        public string Phone_Number { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        public string Email_Address { get; set; }
    
        [Required]
        [StringLength(4)]
        public string CaptchaCode { get; set; }

        //public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
        public string D_D_Number { get; set; }

        [DataType(DataType.Date)]
        //[Required]
        public DateTime? Date { get; set; }
        public int? Amount { get; set; }
        public string Remarks { get; set; }

        public byte[] CaptchaByteData { get; set; }
        public string PlanType { get; set; }
    }
}
