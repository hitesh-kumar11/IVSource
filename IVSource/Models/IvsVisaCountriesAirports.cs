using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCountriesAirports
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
        public string AirportId { get; set; }
        [Required]
        [MaxLength(3)]
        [RegularExpression("[A-Z]+$", ErrorMessage = "The field Airport Code must be in capitals.")]
        [Display(Name = "Airport Code")]
        [XmlElement("Code")]
        [JsonProperty("Code")]
        public string AirportCode { get; set; }
        [Required]
        [Display(Name = "Airport Name")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string AirportName { get; set; }
        [Required]
        [Display(Name = "Airport Type")]
        [XmlElement("Type")]
        [JsonProperty("Type")]
        public string AirportType { get; set; }
        [Required]
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
    }
}
