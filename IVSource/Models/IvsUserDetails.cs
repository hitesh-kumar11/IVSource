using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class IvsUserDetails
    {
        public int SerialNum { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please Enter User Name")]
        public string UserName { get; set; }
        //[MinLength(8)]
        [MaxLength(15)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$", ErrorMessage = "Password should contain atleast one lower and upper case alphabet, one special character and one number")]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        [Display(Name = "User Type")]
        public string UserType { get; set; }
        [Display(Name = "Corporate ID")]
        public string CorporateId { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        public string Designation { get; set; }
        [Required(ErrorMessage = "Please Enter Company Name")]
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [Display(Name = "Phone Number")]
        //[RegularExpression("([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string Phone { get; set; }
        public string Fax { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        //[RegularExpression("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string AdditionalEmail { get; set; }
        [Display(Name = "No.of Terminals")]
        public string TerminalIdNo { get; set; }
        [Required(ErrorMessage = "Please select start date")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? ValidFrom { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please select end date")]

        public DateTime? ValidTo { get; set; }
        [Display(Name = "Type")]
        public int? IsEnable { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ResetPasswordOtp { get; set; }
        public DateTime? ResetPasswordOtpexpireOn { get; set; }
    }
}
