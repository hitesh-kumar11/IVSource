using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaSaarcDetails:MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        [XmlElement("CountryID")]
        [JsonProperty("CountryID")]
        public string CountryIso { get; set; }
        [Required]
        [Display(Name = "Country Id")]
        [XmlIgnore]
        [JsonIgnore]
        public string CountryId { get; set; }
        [Required]
        [Display(Name = "Visa Required")]
        [XmlElement("VisaRequired")]
        [JsonProperty("VisaRequired")]
        public string IsVisaRequired { get; set; }
        [Display(Name = "Visa Apply Where")]
        [XmlElement("WhereToApply")]
        [JsonProperty("WhereToApply")]
        public string VisaApplyWhere { get; set; }
        [Display(Name = "Office Address")]
        [XmlElement("Address")]
        [JsonProperty("Address")]
        public string VisaOfficeAddress { get; set; }
        [Display(Name = "Office City")]
        [XmlElement("City")]
        [JsonProperty("City")]
        public string VisaOfficeCity { get; set; }
        [Display(Name = "Country")]
        [XmlElement("Country")]
        [JsonProperty("Country")]
        public string VisaOfficeCountry { get; set; }
        [Display(Name = "Email")]
        [XmlElement("Email")]
        [JsonProperty("Email")]
        public string VisaOfficeEmail { get; set; }
        [Display(Name = "Fax")]
        [XmlElement("Fax")]
        [JsonProperty("Fax")]
        public string VisaOfficeFax { get; set; }
        [Display(Name = "Office Name")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string VisaOfficeName { get; set; }
        [XmlElement("Notes")]
        [JsonProperty("Notes")]
        public string VisaOfficeNotes { get; set; }
        [Display(Name = "Pincode")]
        [XmlElement("PinCode")]
        [JsonProperty("PinCode")]
        public string VisaOfficePincode { get; set; }
        [XmlElement("Telephone")]
        [JsonProperty("Telephone")]
        public string VisaOfficeTelephone { get; set; }
        [XmlElement("Website")]
        [JsonProperty("Website")]
        public string VisaOfficeWebsite { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string VisaOfficeId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string VisaOfficeUrl { get; set; }
        [Display(Name = "Enable")]
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public string CountryName { get; set; }
    }
}
