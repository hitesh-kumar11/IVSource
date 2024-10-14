using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IVSource.Models
{
    public abstract class MemberPageBase
    {
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public string bCountryName { get; set; } = "";
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public string bCountryFlag { get; set; } = "";
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public string bCountryIso { get; set; } = "";

        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public IvsVisaPages bLogo { get; set; }
    }
}