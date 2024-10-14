using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using IVSource.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using IVSource.Controllers.Api;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace IVSource.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisaUpdatesController : ControllerBase
    {
        private readonly IVSourceContext _context;
        private readonly IConfiguration _config;

        public VisaUpdatesController(IVSourceContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost]
        public ActionResult Auth()
        {
            RequestModels.Root data = new RequestModels.Root();
            DateTime FromDate;
            DateTime ToDate;
            string Id = "";
            string UserName = "";
            string Password = "";
            XmlSerializer xsSubmit1 = new XmlSerializer(typeof(RequestModels.Root));
            var subReq1 = data;
            var xmll = "";
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit1.Serialize(writer, subReq1);
                    xmll = sww.ToString(); // Your XML
                }
            }
            string Accept = ((Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders)Request.Headers).HeaderAccept;
            Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders responseHeaders = new Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders();
            if (Request.Body.CanSeek)
            {
                Request.Body.Position = 0;
            }
            var input = new StreamReader(Request.Body).ReadToEnd();
            string type = string.Empty;
            RequestModels.Root obj = new RequestModels.Root();// add with root
            bool isValid = false;
            if (Request.ContentType == "application/xml")
            {
                type = "xml";
                var xmldoc = new XmlDocument();
                string xml = Convert.ToString(input).Replace(Environment.NewLine, "");
                xmldoc.LoadXml(xml.Replace("SOAP:", "SOAP"));
                var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
                data = JsonConvert.DeserializeObject<RequestModels.Root>(fromXml);
                Id = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.Id;
                UserName = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.UserName;
                Password = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.Password;
                FromDate = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.VisaCountryRequest.CountryDetails.Country.FromDate;
                ToDate = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.VisaCountryRequest.CountryDetails.Country.ToDate;
                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString()
                            && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable
                            select m;
                if (model.Count() > 0)
                    isValid = true;
            }
            else if (Request.ContentType == "application/json")
            {
                type = "json";
                data = JsonConvert.DeserializeObject<RequestModels.Root>(input);
                Id = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.Id;
                UserName = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.UserName;
                Password = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.POS.Source.Password;
                FromDate = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.VisaCountryRequest.CountryDetails.Country.FromDate;
                ToDate = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaCountryRequest.VisaCountryRequest.CountryDetails.Country.ToDate;

                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString() &&
                            n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable
                            select m;
                if (model.Count() > 0)
                    isValid = true;
            }
            else
            {
                FromDate = new DateTime();
                ToDate = new DateTime();
            }
            var data1 = _context.IvsUserDetails.Where(x => x.CorporateId == Id.ToString() && x.UserName == UserName && x.Password == Password && x.IsEnable == 1);
            if (isValid && data1. Count() == 1) //added by rekha
            {       
                //IMP condition commented by rekha 04/oct/2021
                //var data1 = _context.IvsUserDetails.Where(x => x.CorporateId == Id.ToString() && x.UserName == UserName && x.Password == Password && x.IsEnable == 1);
                //if (data1.Count() == 1)
                //{  //end rekha
                    List<IvsVisaCountries> CountryList = (from country in _context.IvsVisaCountries
                                                          where country.CreatedDate >= FromDate && country.CreatedDate <= ToDate && country.IsUpdated == 1
                                                          orderby country.ModifiedDate descending
                                                          select new IvsVisaCountries
                                                          {
                                                              CountryName = country.CountryName,
                                                              CountryIso = country.CountryIso,
                                                              ModifiedDate = country.ModifiedDate
                                                          }).ToList();
                    CountryDetails CountryDetails = new CountryDetails();
                    CountryDetails.CountryList = CountryList;
                    CountryDetails.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
                    CountryDetails.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
                    VisaCountryResponse VisaCountryResponse = new VisaCountryResponse { CountryDetails = CountryDetails };
                    IVA_VisaCountryResponse objXml = new IVA_VisaCountryResponse { VisaCountryResponse = VisaCountryResponse };
                    VisaUpdatesAPI objJson = new VisaUpdatesAPI { IVA_VisaCountryResponse = objXml };
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(IVA_VisaCountryResponse));
                    dynamic subReq = objXml;
                    var xml1 = "";
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xsSubmit.Serialize(writer, subReq);
                            xml1 = @"<soap:Envelope soap:encodingStyle='http://schemas.xmlsoap.org/soap/encoding/' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soapenc='http://schemas.xmlsoap.org/soap/encoding/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><soap:Body><IVSource xmlns='http://www.IVSource.com/IVACONT/Global'>" + sww.ToString(); // Your XML
                        }
                    }
                    if (Accept != null && Accept.ToLower() == "application/json")
                        return new JsonResult(objXml);
                    else
                    {
                    xml1 = xml1 + "</IVSource> </soap:Body> </soap:Envelope>".Replace("&lt;", "<").Replace("&amp;", "&")
                                         .Replace("&gt;", ">")
                                         .Replace("&quot;", "\"")
                                         .Replace("&apos;", "'").Replace("![CDATA[", "").Replace("]]", "");
                    return Content(xml1.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "")
                      .Replace("![CDATA[", "").Replace("]]", "")                      
                      , "text/xml");
                }
            }
           // }
            else
            {
                if (Accept != null && Accept.ToLower() == "application/json")
                {
                    string AuthorizationError = "{\"Authorization\":\"User authorization failure\"}";
                    return new JsonResult(JObject.Parse(AuthorizationError));
                }
                else
                {
                    var sr = "";

                    XNamespace encodingStyle = "http://schemas.xmlsoap.org/soap/encoding/";
                    XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace soapenc = "http://schemas.xmlsoap.org/soap/encoding/";
                    XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                    var a = XNamespace.Get("http://www.IVSource.com/IVACONT/Global");

                    var xml = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                            new XElement(soap + "Envelop",
                                new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                            new XElement(soap + "Body",
                            new XElement(a + "IVSource",
                            new XElement(new XElement("Authorization", "User authorization failure"))))));

                    foreach (var node in xml.Root.Descendants())
                    {
                        if (node.Name.NamespaceName == "")
                        {
                            node.Attributes("xmlns").Remove();
                            node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                        }
                    }

                    sr = xml.ToString();
                    return Content(sr, "text/xml");
                }
            }
        }
        /* [HttpGet]
         public ActionResult Auth([FromHeader]int Id, [FromHeader]string UserName, [FromHeader]string Password, [FromHeader]string Accept, DateTime? FromDate, DateTime? ToDate)
         {
             if (Id != 0 && UserName != null && Password != null)
             {
                 var data = _context.IvsUserDetails.Where(x => x.CorporateId == Id.ToString() && x.UserName == UserName && x.Password == Password && x.IsEnable == 1);

                 if (data.Count() == 1)
                 {
                     List<IvsVisaCountries> CountryList = (from country in _context.IvsVisaCountries
                                                           where country.CreatedDate >= FromDate && country.CreatedDate <= ToDate && country.IsUpdated == 1
                                                           orderby country.ModifiedDate descending
                                                           select new IvsVisaCountries
                                                           {
                                                               CountryName = country.CountryName,
                                                               CountryIso = country.CountryIso,
                                                               ModifiedDate = country.ModifiedDate
                                                           }).ToList();

                     CountryDetails CountryDetails = new CountryDetails();
                     CountryDetails.CountryList = CountryList;
                     CountryDetails.FromDate = Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd");
                     CountryDetails.ToDate = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");

                     VisaCountryResponse VisaCountryResponse = new VisaCountryResponse { CountryDetails = CountryDetails };
                     IVA_VisaCountryResponse objXml = new IVA_VisaCountryResponse { VisaCountryResponse = VisaCountryResponse };
                     VisaUpdatesAPI objJson = new VisaUpdatesAPI { IVA_VisaCountryResponse = objXml };

                     XmlSerializer xsSubmit = new XmlSerializer(typeof(IVA_VisaCountryResponse));
                     dynamic subReq = objXml;

                     var xml1 = "";

                     using (var sww = new StringWriter())
                     {
                         using (XmlWriter writer = XmlWriter.Create(sww))
                         {
                             xsSubmit.Serialize(writer, subReq);
                             xml1 = @"<soap:Envelope soap:encodingStyle='http://schemas.xmlsoap.org/soap/encoding/' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soapenc='http://schemas.xmlsoap.org/soap/encoding/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><soap:Body><IVSource xmlns='http://www.IVSource.com/IVACONT/Global'>" + sww.ToString(); // Your XML
                         }
                     }
                     if (Accept.ToLower() == "application/xml")
                     {
                         xml1 = xml1 + "</IVSource> </soap:Body> </soap:Envelope>";
                         return Content(xml1.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", ""), "text/xml");
                     }
                     else if (Accept.ToLower() == "application/json")
                         return new JsonResult(objXml);
                     else
                         return StatusCode(406);
                 }
             }
             else
             {
                 if (Accept.ToLower() == "application/xml")
                 {
                     var sr = "";

                     XNamespace encodingStyle = "http://schemas.xmlsoap.org/soap/encoding/";
                     XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                     XNamespace soapenc = "http://schemas.xmlsoap.org/soap/encoding/";
                     XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                     XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                     var a = XNamespace.Get("http://www.IVSource.com/IVACONT/Global");

                     var xml = new XDocument(
                         new XDeclaration("1.0", "utf-8", "yes"),
                             new XElement(soap + "Envelop",
                                 new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                 new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                 new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                 new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                 new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                             new XElement(soap + "Body",
                             new XElement(a + "IVSource",
                             new XElement(new XElement("Authorization", "User authorization failure"))))));

                     foreach (var node in xml.Root.Descendants())
                     {
                         if (node.Name.NamespaceName == "")
                         {
                             node.Attributes("xmlns").Remove();
                             node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                         }
                     }

                     sr = xml.ToString();
                     return Content(sr, "text/xml");
                 }
                 else if (Accept.ToLower() == "application/json")
                 {
                     string AuthorizationError = "{\"Authorization\":\"User authorization failure\"}";
                     return new JsonResult(JObject.Parse(AuthorizationError));
                 }
             }
             return StatusCode(406);
         }*/
    }
}