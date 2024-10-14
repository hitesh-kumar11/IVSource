using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public class VisaUpdatesAPI
    {
        [JsonProperty("IVA_VisaCountryResponse")]
        public IVA_VisaCountryResponse IVA_VisaCountryResponse;
    }
    public class IVA_VisaCountryResponse
    {
        [JsonProperty("VisaCountryResponse")]
        public VisaCountryResponse VisaCountryResponse;
    }
    public class VisaCountryResponse
    {
        [JsonProperty("CountryDetails")]
        public CountryDetails CountryDetails;
    }
    public class CountryDetails
    {
        [JsonIgnore]
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FromDate { get; set; }
        [JsonIgnore]
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ToDate { get; set; }

        [JsonProperty("CountryList")]
        public List<IvsVisaCountries> CountryList;
    }
}
