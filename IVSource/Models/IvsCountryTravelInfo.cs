using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class IvsCountryTravelInfo
    {
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        public string CountryIso { get; set; }
        public string TravelInfoId { get; set; }
        [Required]
        [Display(Name = "Travel Category")]
        public string TravelCategory { get; set; }
        [Required]
        [Display(Name = "Travel Type")]
        public string TravelType { get; set; }
        [Required]
        [Display(Name = "Travel Description")]
        public string TravelDescription { get; set; }
        [Display(Name = "Enable")]
        public int? IsEnable { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
