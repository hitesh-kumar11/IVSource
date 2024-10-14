using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class Login
    {
        [Key]
        public int SerialNum { get; set; }
        //public string UserId { get; set; }
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Please Enter User Name")]
        public string UserName { get; set; } = null;
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Terminal Id")]
        public string TerminalId { get; set; }
        public string EMail { get; set; }
        public string OTP { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
