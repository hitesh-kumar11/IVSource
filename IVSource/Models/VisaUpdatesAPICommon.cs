using IVSource.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

/*namespace IVSource.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Xml1
    {
        [JsonProperty("@version")]
        [XmlAttribute("version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        [XmlAttribute("encoding")]
        public string Encoding { get; set; }
    }

    public class Source1
    {
        [JsonProperty("@Id")]
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [JsonProperty("@UserName")]
        [XmlAttribute("UserName")]
        public string UserName { get; set; }

        [JsonProperty("@Password")]
        [XmlAttribute("Password")]
        public string Password { get; set; }
    }

    public class POS1
    {
        public Source1 Source1 { get; set; }
    }

    public class Country1
    {
        //[JsonProperty("@Code")]
        //[XmlAttribute("Code")]
        //public string Code { get; set; }
        [JsonProperty("@FromDate")]
        [XmlAttribute("FromDate")]
        public DateTime FromDate { get; set; }
        [JsonProperty("@ToDate")]
        [XmlAttribute("ToDate")]
        public DateTime ToDate { get; set; }
    }

    public class CountryDetails2
    {
        public Country1 Country1 { get; set; }
    }

    public class VisaCountryRequest
    {
        [JsonProperty("CountryDetails2")]
        [XmlElement("CountryDetails2")]
        public CountryDetails2 CountryDetails2 { get; set; }
    }

    public class IVA_VisaCountryRequest
    {
        public POS1 POS1
 { get; set; }
        [JsonProperty("VisaCountryRequest")]
        [XmlElement("VisaCountryRequest")]
        public VisaCountryRequest VisaCountryRequest { get; set; }
    }

    public class IVSource2
    {
        [JsonProperty("@xmlns")]
        [XmlAttribute("xmlns")]
        public string Xmlns { get; set; }
        public IVA_VisaCountryRequest IVA_VisaCountryRequest { get; set; }
    }

    public class SOAPBody1
    {
        [JsonProperty("IVSource")]
        [XmlElement("IVSource")]
        public IVSource2 IVSource2 { get; set; }
    }

    public class SOAPEnvelope1
    {
        [JsonProperty("@xmlns:SOAP")]
        [XmlAttribute("SOAP", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string XmlnsSOAP { get; set; }

        [JsonProperty("@SOAP:encodingStyle")]
        [XmlAttribute("encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/encoding/")]
        public string SOAPEncodingStyle { get; set; }

        [JsonProperty("SOAPBody")]
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAPBody1 SOAPBody1 { get; set; }
    }
    //[XmlIgnore]
    public class Root1
    {
        [JsonProperty("?xml")]
        [XmlElement("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("SOAPEnvelope")]
        [XmlElement("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAPEnvelope1 SOAPEnvelope1 { get; set; }
    }
}*/
//namespace change 
namespace IVSource.RequestModels
{
    public class Xml
    {
        [JsonProperty("@version")]
        [XmlAttribute("version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        [XmlAttribute("encoding")]
        public string Encoding { get; set; }
    }

    public class Source
    {
        [JsonProperty("@Id")]
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [JsonProperty("@UserName")]
        [XmlAttribute("UserName")]
        public string UserName { get; set; }

        [JsonProperty("@Password")]
        [XmlAttribute("Password")]
        public string Password { get; set; }
    }

    public class POS
    {
        public Source Source { get; set; }
    }

    public class Country
    {       
        [JsonProperty("@FromDate")]
        [XmlAttribute("FromDate")]
        public DateTime FromDate { get; set; }
        [JsonProperty("@ToDate")]
        [XmlAttribute("ToDate")]
        public DateTime ToDate { get; set; }
    }

    public class CountryDetails
    {
        public Country Country { get; set; }
    }

    public class VisaCountryRequest
    {
        [JsonProperty("CountryDetails")]
        [XmlElement("CountryDetails")]
        public CountryDetails CountryDetails { get; set; }
    }

    public class IVA_VisaCountryRequest
    {
        public POS POS  { get; set; }
        [JsonProperty("VisaCountryRequest")]
        [XmlElement("VisaCountryRequest")]
        public VisaCountryRequest VisaCountryRequest { get; set; }
    }

    public class IVSource1
    {
        [JsonProperty("@xmlns")]
        [XmlAttribute("xmlns")]
        public string Xmlns { get; set; }
        public IVA_VisaCountryRequest IVA_VisaCountryRequest { get; set; }
    }

    public class SOAPBody
    {
        [JsonProperty("IVSource")]
        [XmlElement("IVSource")]
        public IVSource1 IVSource1 { get; set; }
    }

    public class SOAPEnvelope
    {
        [JsonProperty("@xmlns:SOAP")]
        [XmlAttribute("SOAP", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string XmlnsSOAP { get; set; }

        [JsonProperty("@SOAP:encodingStyle")]
        [XmlAttribute("encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/encoding/")]
        public string SOAPEncodingStyle { get; set; }

        [JsonProperty("SOAPBody")]
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAPBody SOAPBody { get; set; }
    }
    public class Root
    {
        [JsonProperty("?xml")]
        [XmlElement("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("SOAPEnvelope")]
        [XmlElement("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAPEnvelope SOAPEnvelope { get; set; }
    }
}
