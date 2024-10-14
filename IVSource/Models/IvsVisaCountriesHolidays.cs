using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCountriesHolidays
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
        public string HolidayId { get; set; }

        [XmlElement("Date")]
        [JsonProperty("Date")]
        public string Date { get; set; }

        [Display(Name = "Name")]
        [XmlElement("HolidayName")]
        [JsonProperty("HolidayName")]
        public string HolidayName { get; set; }
        [XmlElement("Month")]
        [JsonProperty("Month")]
        public string Month { get; set; }
        //[Required]
        [XmlElement("Year")]
        [JsonProperty("Year")]
        public string Year { get; set; }
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
