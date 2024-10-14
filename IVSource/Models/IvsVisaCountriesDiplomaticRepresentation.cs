using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCountriesDiplomaticRepresentation  : MemberPageBase
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
        [Display(Name = "Collection Timings")]
        [XmlElement("CollectionTimings")]
        [JsonProperty("CollectionTimings")]

        public string OfficeCollectionTimings { get; set; }
        [Required]
        [Display(Name = "Country")]
        [XmlElement("Country")]
        [JsonProperty("Country")]

        public string OfficeCountry { get; set; }
        [Required]
        [Display(Name = "Email")]
        [XmlElement("Email")]
        [JsonProperty("Email")]
        public string OfficeEmail { get; set; }
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
        [Display(Name = "Pin Code")]
        [XmlElement("PinCode")]
        [JsonProperty("PinCode")]
        public string OfficePincode { get; set; }
        [Display(Name = "Public Timings")]
        [XmlElement("PublicTimings")]
        [JsonProperty("PublicTimings")]
        public string OfficePublicTimings { get; set; }

        [Display(Name = "Telephone Visa")]
        [XmlElement("Telephone")]
        [JsonProperty("Telephone")]
        public string OfficeTelephoneVisa { get; set; }

        [Required]
        [Display(Name = "Timings")]
        [XmlElement("Timings")]
        [JsonProperty("Timings")]
        public string OfficeTimings { get; set; }
        [Required]
        [Display(Name = "Visa Timings")]
        [XmlElement("VisaTimings")]
        [JsonProperty("VisaTimings")]
        public string OfficeVisaTimings { get; set; }
        [Display(Name = "Website")]
        [Url]
        [XmlElement("Website")]
        [JsonProperty("Website")]
        public string OfficeWebsite { get; set; }

        [Display(Name = "Territory Jurisdiction")]
        [XmlIgnore]
        [JsonIgnore]
        public string TerritoryJurisdiction { get; set; }
        [Required]
        [Display(Name = "Enable")]
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string Priority { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
    }
}
