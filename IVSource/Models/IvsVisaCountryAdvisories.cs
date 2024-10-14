using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCountryAdvisories:MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string AdvisoryId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [Required]
        [XmlElement("Description")]
        [JsonProperty("Description")]
    [System.Xml.Serialization.XmlText()]
        public string Advisory { get; set; }
        [Display(Name = "Advisory Type")]
    [System.Xml.Serialization.XmlText()]
    [XmlIgnore]
        [JsonIgnore]
        public string AdvisoryType { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }
    }
}
