using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVSource.Models
{
  // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
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
    [JsonProperty("@Code")]
    [XmlAttribute("Code")]
    public string Code { get; set; }
  }

  public class CountryDetails1
  {
    public Country Country { get; set; }
  }

  public class VisaSearchRequest1
  {
    [JsonProperty("CountryDetails")]
    [XmlElement("CountryDetails")]
    public CountryDetails1 CountryDetails1 { get; set; }
  }

  public class IVAVisaSearchRequest
  {
    public POS POS { get; set; }
    [JsonProperty("VisaSearchRequest")]
    [XmlElement("VisaSearchRequest")]
    public VisaSearchRequest1 VisaSearchRequest1 { get; set; }
  }

  public class IVSource1
  {
    [JsonProperty("@xmlns")]
    [XmlAttribute("xmlns")]
    public string Xmlns { get; set; }
    public IVAVisaSearchRequest IVA_VisaSearchRequest { get; set; }
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
  //[XmlIgnore]
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
