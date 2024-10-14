using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaHelpAddress : MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string OfficeId { get; set; }
        [Required]
        [Display(Name = "Address")]
        [XmlElement("Address")]
        [JsonProperty("Address")]
        public string OfficeAddress { get; set; }
        [Required]
        [Display(Name = "City")]
        [XmlElement("City")]
        [JsonProperty("City")]
        public string OfficeCity { get; set; }
        [Required]
        [Display(Name = "Country")]
        [XmlElement("Country")]
        [JsonProperty("Country")]
        public string OfficeCountry { get; set; }
        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]
        [XmlElement("Email")]
        [JsonProperty("Email")]
        public string OfficeEmail { get; set; }
        [Required]
        [Display(Name = "Fax")]
        [XmlElement("Fax")]
        [JsonProperty("Fax")]
        public string OfficeFax { get; set; }
        [Required]
        [Display(Name = "Name")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string OfficeName { get; set; }
        [Display(Name = "Notes")]
        [XmlElement("Notes")]
        [JsonProperty("Notes")]
        public string OfficeNotes { get; set; }
        [Required]
        //[RegularExpression("([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone")]
        [XmlElement("Phone")]
        [JsonProperty("Phone")]
        public string OfficePhone { get; set; }
        [Required]
        [Display(Name = "Pincode")]
        [XmlElement("PinCode")]
        [JsonProperty("PinCode")]
        public string OfficePincode { get; set; }
        [Display(Name = "URL")]
        [Url]
        [XmlElement("URL")]
        [JsonProperty("URL")]
        public string OfficeUrl { get; set; }
        [Display(Name = "Website")]
        [XmlElement("Website")]
        [JsonProperty("Website")]
        public string OfficeWebsite { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Address Type")]
        public string AddressType { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Enable")]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
    }
}
