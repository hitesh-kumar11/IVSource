using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCountriesAirlines
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
        public string AirlineId { get; set; }
        [Required]
        [MaxLength(3)]
        [RegularExpression("[A-Z]+$", ErrorMessage = "The field Airline Code must be in capitals.")]
        [Display(Name = "Airline Code")]
        [XmlElement("Code")]
        [JsonProperty("Code")]
        public string AirlineCode { get; set; }
        [Required]
        [Display(Name = "Airline Name")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string AirlineName { get; set; }

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
