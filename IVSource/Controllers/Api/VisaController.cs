using IVSource.Models;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IVSource.Controllers.Api
{
    //[Route("api/[controller]")]
    [Route("services/[controller]")]
    [ApiController]
    public class VisaController : ControllerBase
    {
        private readonly IVSourceContext _context;
        private readonly IConfiguration _configuration;


        public VisaController(IVSourceContext context, IConfiguration iConfig)
        {
            _context = context;
            _configuration = iConfig;
        }

        [HttpPost]
        public ActionResult getdetails()
        {
            Root data = new Root();
            string Countryiso = "";
            string Id = "";
            string UserName = "";
            string Password = "";

            XmlSerializer xsSubmit1 = new XmlSerializer(typeof(Root));
            var subReq1 = data;
            var xmll = "";
            APILogs objAppLog = new APILogs();

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit1.Serialize(writer, subReq1);
                    xmll = sww.ToString(); // Your XML
                }
            }

            string Accept = ((Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders)Request.Headers).HeaderAccept;
            string ContentType = ((Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders)Request.Headers).HeaderContentType;
            Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders responseHeaders = new Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestHeaders();

            if (Request.Body.CanSeek)
            {
                Request.Body.Position = 0;
            }
            var input = new StreamReader(Request.Body).ReadToEnd();
            string type = string.Empty;
            Root obj = new Root();
            bool isValid = false;
            bool isValidCountry = false;

            var DemoId = _configuration["VisaApiDemo:Id"];
            var DemoUserName = _configuration["VisaApiDemo:UserName"];
            var DemoPassword = _configuration["VisaApiDemo:Password"];
            var DemoUserType = _configuration["VisaApiDemo:UserType"];
            var FilePath = _configuration["VisaApiFilePath:FilePath"];

            if (Request.ContentType == "application/xml" || Request.ContentType == "text/xml;charset=UTF-8")
            {
                type = "xml";

                var xmldoc = new XmlDocument();
                string xml = Convert.ToString(input).Replace(Environment.NewLine, "");
                xmldoc.LoadXml(xml.Replace("SOAP:", "SOAP"));
                var fromXml = JsonConvert.SerializeXmlNode(xmldoc);

                data = JsonConvert.DeserializeObject<Root>(fromXml);

                Countryiso = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.VisaSearchRequest1.CountryDetails1.Country.Code;
                Id = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Id;
                UserName = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.UserName;
                Password = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Password;
                objAppLog.ErrLog("Countryiso : " + Countryiso + "Id ; " + Id + "UserName ; " + UserName + " Password : " + Password + "IP : " + Request.HttpContext.Connection.RemoteIpAddress.ToString());

                //var model = from m in _context.IvsVisaClientIpRegistration
                //            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                //            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString()
                //            && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable
                //            select m;

                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable == true && n.UserType.ToUpper() != DemoUserType.ToUpper()
                            select new
                            {
                                UserType = n.UserType,
                                UserName = n.UserName,
                                Password = n.Password,
                                IPAddress = m.IpAddress,
                                SerialNum = m.SerialNum
                            };

                if (model.Count() > 0)
                {
                    isValid = true;
                }
                else
                {
                    if (UserName == DemoUserName && Password == DemoPassword && Id == DemoId)
                    {
                        var model22 = from m in _context.IvsVisaClientIpRegistration
                                      join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                                      where n.UserType.ToUpper() == DemoUserType && n.UserName == UserName && n.Password == Password && n.CorporateId == Id && n.IsEnable == 1 && m.IsEnable == true
                                      select new
                                      {
                                          UserType = n.UserType
                                      };

                        if (model22.FirstOrDefault().UserType == DemoUserType)
                        {

                            //Add logic by Hitesh on 03072023
                            //var countryname = _context.IvsVisaCountries.Where(x => x.CountryIso == Countryiso && x.IsEnable == 1).FirstOrDefault().CountryName;

                            var ivsvisasubpagescountries = (from country in _context.IvsVisaSubPages.Where(x => x.PageId == 12 && x.IsEnable == true)
                                                            select country.Country_ISO).Contains(Countryiso);

                            if (ivsvisasubpagescountries == true)
                            {
                                isValid = true;
                            }
                            else
                            {
                                if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
                                {
                                    string AuthorizationError = "{\"Authorization\":\"Not accessible product with this country using demo credential\"}";
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

                                    var xmlll = new XDocument(
                                        new XDeclaration("1.0", "utf-8", "yes"),
                                            new XElement(soap + "Envelop",
                                                new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                            new XElement(soap + "Body",
                                            new XElement(a + "IVSource",
                                            new XElement(new XElement("Authorization", "Not accessible product with this country using demo credential"))))));

                                    foreach (var node in xmlll.Root.Descendants())
                                    {
                                        if (node.Name.NamespaceName == "")
                                        {
                                            node.Attributes("xmlns").Remove();
                                            node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                                        }
                                    }

                                    sr = xmlll.ToString();
                                    return Content(sr, "text/xml");
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
                        {
                            string AuthorizationError = "{\"Authorization\":\"User authorization failure.\"}";
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

                            var xmlll = new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement(soap + "Envelop",
                                        new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                    new XElement(soap + "Body",
                                    new XElement(a + "IVSource",
                                    new XElement(new XElement("Authorization", "User authorization failure."))))));

                            foreach (var node in xmlll.Root.Descendants())
                            {
                                if (node.Name.NamespaceName == "")
                                {
                                    node.Attributes("xmlns").Remove();
                                    node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                                }
                            }

                            sr = xmlll.ToString();
                            return Content(sr, "text/xml");
                        }
                    }
                }
            }
            else if (Request.ContentType == "application/json")
            {
                //type = "json";
                //data = JsonConvert.DeserializeObject<Root>(input);
                //---------------------
                //var xmldoc = new XmlDocument();
                //string xml = Convert.ToString(input).Replace(Environment.NewLine, "");
                //xmldoc.LoadXml(xml.Replace("SOAP:", "SOAP"));
                //var fromXml = JsonConvert.SerializeXmlNode(xmldoc);

                //data = JsonConvert.DeserializeObject<Root>(fromXml);
                VisaSearchRequestJson oVisaSearchRequestJson = JsonConvert.DeserializeObject<VisaSearchRequestJson>(input);
                if (oVisaSearchRequestJson != null && oVisaSearchRequestJson.Id != "0" && oVisaSearchRequestJson.Countryiso != null)
                {
                    Countryiso = oVisaSearchRequestJson.Countryiso;
                    Id = oVisaSearchRequestJson.Id;
                    UserName = oVisaSearchRequestJson.UserName;
                    Password = oVisaSearchRequestJson.Password;
                }
                //---------------------

                objAppLog.ErrLog("Countryiso : " + Countryiso + " Id ; " + Id + " UserName ; " + UserName + " Password : " + Password + "IP : " + Request.HttpContext.Connection.RemoteIpAddress.ToString());
                //var model = from m in _context.IvsVisaClientIpRegistration
                //            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                //            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString()
                //            && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable
                //            select m;

                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable == true && n.UserType.ToUpper() != DemoUserType.ToUpper()
                            select new
                            {
                                UserType = n.UserType,
                                UserName = n.UserName,
                                Password = n.Password,
                                IPAddress = m.IpAddress,
                                SerialNum = m.SerialNum
                            };

                if (model.Count() > 0)
                {
                    isValid = true;
                }
                else
                {
                    if (UserName == DemoUserName && Password == DemoPassword && Id == DemoId)
                    {
                        var model22 = from m in _context.IvsVisaClientIpRegistration
                                      join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                                      where n.UserType.ToUpper() == DemoUserType && n.UserName == UserName && n.Password == Password && n.CorporateId == Id && n.IsEnable == 1 && m.IsEnable == true
                                      select new
                                      {
                                          UserType = n.UserType
                                      };

                        if (model22.FirstOrDefault().UserType == DemoUserType)
                        {

                            //Add logic by Hitesh on 03072023
                            //var countryname = _context.IvsVisaCountries.Where(x => x.CountryIso == Countryiso && x.IsEnable == 1).FirstOrDefault().CountryName;

                            var ivsvisasubpagescountries = (from country in _context.IvsVisaSubPages.Where(x => x.PageId == 12 && x.IsEnable == true)
                                                            select country.Country_ISO).Contains(Countryiso);

                            if (ivsvisasubpagescountries == true)
                            {
                                isValid = true;
                            }
                            else
                            {
                                if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
                                {
                                    string AuthorizationError = "{\"Authorization\":\"Not an accessible product with test credential\"}";
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

                                    var xmlll = new XDocument(
                                        new XDeclaration("1.0", "utf-8", "yes"),
                                            new XElement(soap + "Envelop",
                                                new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                                new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                            new XElement(soap + "Body",
                                            new XElement(a + "IVSource",
                                            new XElement(new XElement("Authorization", "Not an accessible product with test credential"))))));

                                    foreach (var node in xmlll.Root.Descendants())
                                    {
                                        if (node.Name.NamespaceName == "")
                                        {
                                            node.Attributes("xmlns").Remove();
                                            node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                                        }
                                    }

                                    sr = xmlll.ToString();
                                    return Content(sr, "text/xml");
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
                        {
                            string AuthorizationError = "{\"Authorization\":\"User authorization failure.\"}";
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

                            var xmlll = new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement(soap + "Envelop",
                                        new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                                        new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                                    new XElement(soap + "Body",
                                    new XElement(a + "IVSource",
                                    new XElement(new XElement("Authorization", "User authorization failure."))))));

                            foreach (var node in xmlll.Root.Descendants())
                            {
                                if (node.Name.NamespaceName == "")
                                {
                                    node.Attributes("xmlns").Remove();
                                    node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                                }
                            }

                            sr = xmlll.ToString();
                            return Content(sr, "text/xml");
                        }
                    }
                }

                objAppLog.ErrLog("Authentication Result : " + isValid);


            }


            if (isValid)
            {
                List<CountryName> name = (from country in _context.IvsVisaCountries
                                          where country.CountryIso == Countryiso
                                          select new CountryName
                                          {
                                              Name = country.CountryName,
                                          }).ToList();

                List<IvsVisaCountriesDetails> CountryList = (from country in _context.IvsVisaCountries
                                                             join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                                             where country.CountryIso == Countryiso
                                                             select new IvsVisaCountriesDetails
                                                             {
                                                                 CountryIso = us.CountryIso,
                                                                 CountryArea = us.CountryArea,
                                                                 CountryCapital = us.CountryCapital,
                                                                 CountryClimate = us.CountryClimate,
                                                                 CountryCurrency = us.CountryCurrency,
                                                                 CountryFlag = us.CountryFlag.ToString(),
                                                                 CountryLanguages = us.CountryLanguages,
                                                                 CountryLargeMap = us.CountryLargeMap.ToString(),
                                                                 CountryLocation = us.CountryLocation,
                                                                 CountryNationalDay = us.CountryNationalDay,
                                                                 CountryPopulation = us.CountryPopulation,
                                                                 CountrySmallMap = us.CountrySmallMap.ToString(),
                                                                 CountryTime = us.CountryTime,
                                                                 CountryWorldFactBook = us.CountryWorldFactBook
                                                             }).ToList();
                //select new IvsVisaCountriesDetails
                //{
                //  CountryIso = us.CountryIso,
                // // CountryName = new CountryName {Name=country.CountryName },
                //  CountryArea = us.CountryArea,
                //  CountryCapital = us.CountryCapital,
                //  CountryClimate = us.CountryClimate,
                //  CountryCurrency = us.CountryCurrency,
                //  CountryFlag = "http://app.ivsource.com/app/img/flag/" + us.CountryFlag.ToString(),
                //  CountryLanguages = us.CountryLanguages,
                //  CountryLargeMap = "http://app.ivsource.com/app/img/map/large/" + us.CountryLargeMap.ToString(),
                //  CountryLocation = us.CountryLocation,
                //  CountryNationalDay = us.CountryNationalDay,
                //  CountryPopulation = us.CountryPopulation,
                //  CountrySmallMap = "http://app.ivsource.com/app/img/map/small/" + us.CountrySmallMap.ToString(),
                //  CountryTime = us.CountryTime,
                //  CountryWorldFactBook = us.CountryWorldFactBook
                //}).ToList();
                //// Holidays 
                List<IvsVisaCountriesHolidays> HolidaysList = (from country in _context.IvsVisaCountries
                                                               join us in _context.IvsVisaCountriesHolidays on country.CountryIso equals us.CountryIso
                                                               where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1
                                                               select new IvsVisaCountriesHolidays
                                                               {
                                                                   CountryIso = us.CountryIso,
                                                                   Date = us.Date.Replace(@"", " "),
                                                                   HolidayName = WebUtility.HtmlDecode(GetFormattedValue(us.HolidayName)),
                                                                   Month = us.Month,
                                                                   Year = us.Year
                                                               }).ToList();
                //Air Lines
                List<IvsVisaCountriesAirlines> AirLines = (from country in _context.IvsVisaCountries
                                                           join us in _context.IvsVisaCountriesAirlines on country.CountryIso equals us.CountryIso
                                                           where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1
                                                           select new IvsVisaCountriesAirlines
                                                           {
                                                               CountryIso = us.CountryIso,
                                                               AirlineCode = us.AirlineCode,
                                                               AirlineName = us.AirlineName
                                                           }).ToList();

                // Airports
                List<IvsVisaCountriesAirports> AirPorts = (from country in _context.IvsVisaCountries
                                                           join us in _context.IvsVisaCountriesAirports on country.CountryIso equals us.CountryIso
                                                           where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1
                                                           select new IvsVisaCountriesAirports
                                                           {
                                                               CountryIso = us.CountryIso,
                                                               AirportCode = us.AirportCode,
                                                               AirportName = us.AirportName,
                                                               AirportType = us.AirportType
                                                           }).ToList();
                //DiplomaticRepresentation Offices  list
                List<IvsVisaCountriesDiplomaticRepresentation> DiplomaticRepresentation = (from country in _context.IvsVisaCountries
                                                                                           join us in _context.IvsVisaCountriesDiplomaticRepresentation on country.CountryIso equals us.CountryIso
                                                                                           where country.CountryIso == Countryiso && us.IsEnable == 1 && country.IsEnable == 1 && us.IsEnable == 1
                                                                                           select new IvsVisaCountriesDiplomaticRepresentation
                                                                                           {
                                                                                               CountryIso = us.CountryIso,
                                                                                               OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                                                               OfficeCity = WebUtility.HtmlDecode(us.OfficeCity),
                                                                                               OfficeCollectionTimings = us.OfficeCollectionTimings,
                                                                                               OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                                                               OfficeEmail = us.OfficeEmail,
                                                                                               OfficeFax = us.OfficeFax,
                                                                                               OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                                                               OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes),
                                                                                               OfficePhone = us.OfficePhone,
                                                                                               OfficePincode = us.OfficePincode,
                                                                                               OfficePublicTimings = us.OfficePublicTimings,
                                                                                               OfficeTelephoneVisa = us.OfficeTelephoneVisa,
                                                                                               OfficeVisaTimings = us.OfficeVisaTimings,
                                                                                               OfficeTimings = us.OfficeTimings,
                                                                                               OfficeWebsite = us.OfficeWebsite
                                                                                           }).ToList();
                //IndianEmbassy
                List<IvsVisaHelpAddress> IndianEmbassy = (from country in _context.IvsVisaCountries
                                                          join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                                          where country.CountryIso == Countryiso && us.AddressType == "IMO" && country.IsEnable == 1 && us.IsEnable == 1
                                                          select new IvsVisaHelpAddress
                                                          {
                                                              CountryIso = us.CountryIso,
                                                              OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                              OfficeCity = WebUtility.HtmlDecode(us.OfficeCity.Replace(@"\n", String.Empty)),
                                                              OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                              OfficeEmail = us.OfficeEmail,
                                                              OfficeFax = us.OfficeFax,
                                                              OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                              OfficePincode = us.OfficePincode,
                                                              OfficeUrl = us.OfficeUrl,
                                                              OfficeWebsite = us.OfficeWebsite,
                                                              OfficePhone = us.OfficePhone,
                                                              OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes)
                                                          }).ToList();

                //IndialHelpAddress
                List<IvsVisaHelpAddress> IndialHelpAddress = (from country in _context.IvsVisaCountries
                                                              join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                                              where country.CountryIso == Countryiso && us.AddressType == "IHA" && us.IsEnable == 1 && us.IsEnable == 1
                                                              select new IvsVisaHelpAddress
                                                              {
                                                                  CountryIso = us.CountryIso,
                                                                  OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                                  OfficeCity = WebUtility.HtmlDecode(us.OfficeCity),
                                                                  OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                                  OfficeEmail = us.OfficeEmail,
                                                                  OfficeFax = us.OfficeFax,
                                                                  OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                                  OfficePincode = us.OfficePincode,
                                                                  OfficeUrl = us.OfficeUrl,
                                                                  OfficeWebsite = us.OfficeWebsite,
                                                                  OfficePhone = us.OfficePhone,
                                                                  OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes)
                                                              }).ToList();
                //Advisory
                var Advisory = from country in _context.IvsVisaCountries
                               join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                               where country.CountryIso == Countryiso && us.AdvisoryType == "IVSAD" && country.IsEnable == 1
                               //select new IvsVisaCountryAdvisories
                               //{
                               //  CountryIso = us.CountryIso,
                               //  Advisory = us.Advisory
                               //}).ToList();
                               select WebUtility.HtmlDecode(us.Advisory);

                //ReciprocalVisaInfo
                //List<IvsVisaCountryAdvisories> 
                var ReciprocalVisaInfo = from country in _context.IvsVisaCountries
                                         join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                         where country.CountryIso == Countryiso && us.AdvisoryType == "RECVIS" && country.IsEnable == 1
                                         //select new IvsVisaCountryAdvisories
                                         //{
                                         //  CountryIso = us.CountryIso,
                                         //  Advisory = HttpUtility.HtmlDecode(us.Advisory)
                                         //}).ToList();
                                         select WebUtility.HtmlDecode(us.Advisory);
                //InternationalAdvisory
                //List<IvsVisaCountryAdvisories> 
                var InternationalAdvisory = from country in _context.IvsVisaCountries
                                            join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                            where country.CountryIso == Countryiso && us.AdvisoryType == "INTAD" && country.IsEnable == 1
                                            //  select new IvsVisaCountryAdvisories
                                            //  {
                                            //    CountryIso = us.CountryIso,
                                            //    Advisory = WebUtility.HtmlDecode(us.Advisory)
                                            //  }).ToList();
                                            select WebUtility.HtmlDecode(us.Advisory);
                // SAARCInfo
                List<IvsVisaSaarcDetails> SAARCInfo = (from country in _context.IvsVisaCountries
                                                       join us in _context.IvsVisaSaarcDetails on country.CountryIso equals us.CountryIso
                                                       where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1
                                                       select new IvsVisaSaarcDetails
                                                       {
                                                           CountryIso = us.CountryIso,
                                                           IsVisaRequired = WebUtility.HtmlDecode(us.IsVisaRequired),
                                                           VisaApplyWhere = WebUtility.HtmlDecode(us.VisaApplyWhere),
                                                           VisaOfficeAddress = WebUtility.HtmlDecode(us.VisaOfficeAddress),
                                                           VisaOfficeCity = WebUtility.HtmlDecode(us.VisaOfficeCity),
                                                           VisaOfficeCountry = WebUtility.HtmlDecode(us.VisaOfficeCountry),
                                                           CountryId = us.CountryId,
                                                           VisaOfficeEmail = us.VisaOfficeEmail,
                                                           VisaOfficeFax = us.VisaOfficeFax,
                                                           VisaOfficeName = WebUtility.HtmlDecode(us.VisaOfficeName),
                                                           VisaOfficePincode = us.VisaOfficePincode,
                                                           VisaOfficeTelephone = us.VisaOfficeTelephone,
                                                           VisaOfficeWebsite = us.VisaOfficeWebsite,
                                                           VisaOfficeNotes = WebUtility.HtmlDecode(us.VisaOfficeNotes)
                                                       }).ToList();

                //Category
                // List<IvsVisaCategories> Category = new List<IvsVisaCategories>();
                List<IvsVisaCategories> Category1 = (from country in _context.IvsVisaCategories
                                                     join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                     //  join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode 
                                                     //join ss in _context.IvsVisaQuickContactInfo on country.VisaCategoryId equals ss.VisaCategoryCode //commented by Hitesh on 05062023
                                                     where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1
                                                     orderby country.VisaCategory ascending
                                                     select new IvsVisaCategories
                                                     {
                                                         //CountryIso = ss.CountryIso,Regex.Replace(stw, @"[$,]", "");
                                                         VisaCategoryRequirements = WebUtility.HtmlDecode(country.VisaCategoryRequirements),
                                                         VisaCategoryInformation = WebUtility.HtmlDecode(country.VisaCategoryInformation),
                                                         VisaCategoryNotes = WebUtility.HtmlDecode(country.VisaCategoryNotes),
                                                         VisaCategoryId = country.VisaCategoryId,// get visacatid for fees
                                                         VisaCategory = WebUtility.HtmlDecode(country.VisaCategory),// show
                                                         CityId = country.CityId,//get city for category
                                                                                 //   VisaCategoryCode = ss.VisaCategoryCode //      commented for multiple data 11oc2021                        
                                                     }).Distinct().ToList();
                // CategoryFees
                List<IvsVisaCategoriesOptions> CategoryFees = (from country in _context.IvsVisaCategories
                                                               join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                               join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode
                                                               where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1 && ss.IsEnable == 1
                                                               select new IvsVisaCategoriesOptions
                                                               {
                                                                   CountryIso = ss.CountryIso,
                                                                   VisaCategory = ss.VisaCategory,
                                                                   VisaCategoryCode = WebUtility.HtmlDecode(ss.VisaCategoryCode),
                                                                   VisaCategoryOption = WebUtility.HtmlDecode(ss.VisaCategoryOption),
                                                                   VisaCategoryOptionAmountInr = WebUtility.HtmlDecode(ss.VisaCategoryOptionAmountInr),
                                                                   VisaCategoryOptionAmountOther = ss.VisaCategoryOptionAmountOther
                                                               }).ToList();
                //// CategoryFees
                //List<IvsVisaCategoriesForms> CategoryForms = (from country in _context.IvsVisaCategoriesForms
                //                                              join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                //                                              join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryCode equals ss.VisaCategoryCode
                //                                              where country.CountryIso == Countryiso && country.IsEnable == 1 && ss.IsEnable==1
                //                                              select new IvsVisaCategoriesForms
                //                                              {
                //                                                  CountryIso = ss.CountryIso,
                //                                                  VisaCategoryCode = WebUtility.HtmlDecode(ss.VisaCategoryCode),
                //                                                  Form = WebUtility.HtmlDecode(country.Form),
                //                                                  FormPath = "http://app.ivsource.com/app/img/forms/" + country.FormPath

                //                                              }).ToList();

                // CategoryFees
                List<IvsVisaCategoriesForms> CategoryForms = (from country in _context.IvsVisaCategoriesForms
                                                              join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                              //join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryCode equals ss.VisaCategoryCode
                                                              where country.CountryIso == Countryiso && country.IsEnable == 1 && us.IsEnable == 1 //&& ss.IsEnable == 1
                                                              select new IvsVisaCategoriesForms
                                                              {
                                                                  CountryIso = us.CountryIso,
                                                                  VisaCategoryCode = WebUtility.HtmlDecode(country.VisaCategoryCode),
                                                                  Form = WebUtility.HtmlDecode(country.Form),
                                                                  FormPath = FilePath + country.FormPath

                                                              }).ToList();

                //VisaInformation
                List<IvsVisaInformation> VisaInformation = (from country in _context.IvsVisaCountries
                                                            join ss in _context.IvsVisaInformation on country.CountryIso equals ss.CountryIso
                                                            join us in _context.IvsVisaCountryTerritoryCities on ss.CityId equals us.CityId
                                                            where country.CountryIso == Countryiso && country.IsEnable == 1 && ss.IsEnable == 1 && us.IsEnable == 1
                                                            orderby us.CityId ascending
                                                            select new IvsVisaInformation
                                                            {
                                                                CountryIso = country.CountryIso,
                                                                VisaInformation = WebUtility.HtmlDecode(ss.VisaInformation),
                                                                VisaGeneralInformation = WebUtility.HtmlDecode(ss.VisaGeneralInformation),
                                                                CityId = ss.CityId,
                                                                CityName = WebUtility.HtmlDecode(us.CityName),
                                                                VisaGeneralInformation1 = new IvsVisaCategoriesForApi1
                                                                {
                                                                    VisaCategoryRequirements = WebUtility.HtmlDecode(ss.VisaGeneralInformation),
                                                                    VisaInformation = WebUtility.HtmlDecode(ss.VisaInformation)
                                                                },
                                                                Categories = new IvsVisaInformationCategories
                                                                {
                                                                    Category = Category1.Where(x => x.CityId == ss.CityId).Distinct().OrderBy(x => x.VisaCategoryCode).ToList()
                                                                    //.Select(y=> new IvsVisaCategories() {
                                                                    //ivsVisaCategoriesFeeList =new IvsVisaCategoriesFees
                                                                    //{
                                                                    //    CategoryFee= CategoryFees.Where(z=>z.VisaCategory==y.VisaCategory).ToList()
                                                                    //},
                                                                    ////ivsVisaCategoriesFeeList = CategoryFees.Where(x => x.VisaCategoryCode == y.VisaCategoryCode).Select(z=> new IvsVisaCategoriesFees() {CategoryFee= new List<IvsVisaCategoriesOptions>  z.catefe}).ToList(),
                                                                    //}).ToList(),                                                                   

                                                                },


                                                                //CategoryFees=new IvsVisaCategoriesFees 
                                                                //{
                                                                //    CategoryFee=CategoryFees.Where(x=>x.VisaCategory==x.VisaCategory).ToList() 
                                                                //} //Comment by Hitesh on 05062023
                                                            }).ToList();

                //foreach (var info in VisaInformation)
                //{
                //    var oTempList = new IvsVisaCategoriesFees();
                //    oTempList.CategoryFees = new List<IvsVisaCategoriesFee>();
                //    foreach (var category in info.Categories.Category.Where(x => x.CityId == info.CityId).Distinct())
                //    {
                //        IvsVisaCategoriesFee CategoryFee = new IvsVisaCategoriesFee();
                //        //List<IvsVisaCategoriesOptions> FeeList = new List<IvsVisaCategoriesOptions>();
                //        CategoryFee.CategoryFee = CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).OrderBy(x => x.VisaCategoryCode).ToList();
                //        oTempList.CategoryFees.Add(CategoryFee);
                //    }
                //    info.CategoryFees = oTempList;

                //    //info.CategoryFees = new IvsVisaCategoriesFees();
                //    //var asCatIDS = info.Categories.Category.Select(x => x.VisaCategoryId).ToList();
                //    //info.CategoryFees.CategoryFee = CategoryFees.Where(x => asCatIDS.Contains(x.VisaCategoryCode)).OrderBy(x => x.VisaCategoryCode).ToList();

                //}

                foreach (var info in VisaInformation)
                {
                    var oTempList = new List<IvsVisaCategoriesFees>();
                    //List<IvsVisaCategoriesFees> ss = new List<IvsVisaCategoriesFees>();
                    foreach (var category in info.Categories.Category.Where(x => x.CityId == info.CityId).Distinct())
                    {
                        IvsVisaCategoriesFees CategoryFee = new IvsVisaCategoriesFees();
                        //List<IvsVisaCategoriesOptions> FeeList = new List<IvsVisaCategoriesOptions>();
                        CategoryFee.CategoryFee = CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).OrderBy(x => x.VisaCategoryCode).ToList();
                        oTempList.Add(CategoryFee);
                        //oTempList.CategoryFees = ss;
                    }
                    info.CategoryFees = oTempList;

                    //info.CategoryFees = new IvsVisaCategoriesFees();
                    //var asCatIDS = info.Categories.Category.Select(x => x.VisaCategoryId).ToList();
                    //info.CategoryFees.CategoryFee = CategoryFees.Where(x => asCatIDS.Contains(x.VisaCategoryCode)).OrderBy(x => x.VisaCategoryCode).ToList();

                }

                foreach (var info1 in VisaInformation)
                {
                    var oTempList = new List<CategoryForms>();
                    //oTempList.CategoryFormList = new List<VisaCategoryForms>();
                    foreach (var category1 in info1.Categories.Category.Distinct())
                    {
                        CategoryForms categoryForm = new CategoryForms();
                        categoryForm.CategoryForm = CategoryForms.Where(x => x.VisaCategoryCode == category1.VisaCategoryId).OrderBy(x => x.VisaCategoryCode).ToList();
                        oTempList.Add(categoryForm);
                    }
                    info1.IvsVisaCategoryForms = oTempList;
                }


                //foreach (var info1 in VisaInformation)
                //{
                //    info1.CategoryForms = new CategoryForms();
                //    var asCatIDSS = info1.Categories.Category.Select(x => x.VisaCategoryId).ToList();
                //    info1.CategoryForms.CategoryForm = CategoryForms.Where(x => asCatIDSS.Contains(x.VisaCategoryCode)).OrderBy(x => x.VisaCategoryCode).ToList();
                //}

                VisaDetails VisaDetail = new VisaDetails();
                VisaDetail.CountryDetails = new CountryDetail();
                VisaDetail.CountryDetails.cNames = name;
                VisaDetail.CountryDetails.GeneralInfo = CountryList;
                VisaDetail.CountryCode = Countryiso;
                // VisaDetail.cNames = name;
                VisaDetail.CountryDetails.Holidays = HolidaysList;
                VisaDetail.CountryDetails.Airlines = AirLines;
                VisaDetail.CountryDetails.Airports = AirPorts;
                VisaDetail.DiplomaticRepresentation = new DiplomaticRepresentation();
                VisaDetail.DiplomaticRepresentation.Office = new Office();
                VisaDetail.DiplomaticRepresentation.Office.Offices = DiplomaticRepresentation;
                //VisaDetail.DiplomaticRepresentation = DiplomaticRepresentation;
                VisaDetail.IndianEmbassy = IndianEmbassy;
                //VisaDetail.IntlHelpAddress = IndialHelpAddress;
                VisaDetail.IntlHelpAddress = new IntlHelpAddress();
                VisaDetail.IntlHelpAddress.HelpAddress = IndialHelpAddress;
                VisaDetail.IVSAdvisory = new Descriptions();
                VisaDetail.IVSAdvisory.Description = Advisory.ToList();
                //  VisaDetail.ReciprocalVisaInfo = ReciprocalVisaInfo;
                VisaDetail.ReciprocalVisaInfo = new Descriptions();
                VisaDetail.ReciprocalVisaInfo.Description = ReciprocalVisaInfo.ToList();
                // VisaDetail.InternationalAdvisory = InternationalAdvisory;
                VisaDetail.InternationalAdvisory = new Descriptions();
                VisaDetail.InternationalAdvisory.Description = InternationalAdvisory.ToList();
                //VisaDetail.SAARCInfo = SAARCInfo;
                VisaDetail.SAARCInfo = new SAARCInfo();
                VisaDetail.SAARCInfo.CountryOffice = new CountryOffice();
                VisaDetail.SAARCInfo.CountryOffice.CountryOffices = SAARCInfo;
                VisaDetail.Visainf = VisaInformation;
                VisaDetail.AdditionalInfo = "";
                //VisaDetail.Categories = Category1.Distinct().ToList();
                //VisaDetail.CategoryFees = CategoryFees.Distinct().ToList();
                //VisaDetail.CategoriesForm = CategoryForms.Distinct().ToList();
                VisaSearchResponse visaSearchResponse = new VisaSearchResponse { VisaDetails = VisaDetail };
                IVA_VisaSearchResponse objXml = new IVA_VisaSearchResponse { VisaSearchResponse = visaSearchResponse };
                VisaSearchRequest objJson = new VisaSearchRequest { IVA_VisaSearchResponse = objXml };

                XmlSerializer xsSubmit = new XmlSerializer(typeof(IVA_VisaSearchResponse));
                dynamic subReq = objXml;

                var xml1 = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, subReq);
                        xml1 = @"<soap:Envelope soap:encodingStyle='http://schemas.xmlsoap.org/soap/encoding/' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soapenc='http://schemas.xmlsoap.org/soap/encoding/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>" +
                          "<soap:Body><IVSource xmlns='http://www.IVSource.com/IVACONT/Global'>" + //HttpUtility.HtmlDecode(
                            sww.ToString() //.Replace(@"\n", String.Empty).Replace("![CDATA[", "").Replace("]]", "").Replace("&nbsp;", "")
                                           //)
                            ; // Your XML
                    }
                }
                string strResponse = string.Empty;
                if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
                {
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                    strResponse = JsonConvert.SerializeObject(objJson);
                    strResponse = rx.Replace(strResponse, "");
                    dynamic json = JsonConvert.DeserializeObject(strResponse);
                    return new JsonResult(json);
                }
                else
                {
                    xml1 = //HttpUtility.HtmlDecode(
                      xml1
                      //)

                      + "</IVSource> </soap:Body> "
                        + "</soap:Envelope>";
                    //.Replace("&lt;", "<").Replace("&amp;", "&")
                    //                               .Replace("&gt;", ">")
                    //                               .Replace("&quot;", "\"")
                    //                               .Replace("&apos;", "'").Replace("![CDATA[", "").Replace("]]", "").Replace("\t\t   \t\t", "");

                    if (!string.IsNullOrEmpty(xml1))
                    {
                        // replace entities with literal values
                        xml1 = xml1.Replace("&amp;quot;", "\"");
                        xml1 = xml1.Replace("&amp;gt;", ">");
                        xml1 = xml1.Replace("&amp;lt;", "<");
                        //xml1 = xml1.Replace("&amp;lt;", "&");

                        xml1 = xml1.Replace("&amp;", "&");
                        xml1 = xml1.Replace("&apos;", "'");
                        xml1 = xml1.Replace("&quot;", "\"");
                        xml1 = xml1.Replace("&gt;", ">");
                        xml1 = xml1.Replace("&lt;", "<");
                        xml1 = xml1.Replace("&nbsp;", " ");
                        xml1 = xml1.Replace("&amp;nbsp;", " ");
                        xml1 = xml1.Replace("\\", "");
                        xml1 = xml1.Replace(@"\", "");
                        xml1 = xml1.Replace(@"&", "and");
                    }
                    byte[] data1 = System.Text.Encoding.Default.GetBytes(xml1);

                    xml1 = //HttpUtility.HtmlDecode
                           // xml1;
                           // Uri.EscapeUriString(xml1);
                    xml1.Replace("/[^(\x20-\x7F)\x0A\x0D]*/", "").Replace("&acirc;", "â").Replace("&rsquo;", "’").Replace("&aacute;", "á").Replace("&oacute;", "ó").Replace("&atilde;", "").Replace("&ndash;", "–").Replace("&Atilde;", "Ã").Replace("&iexcl;", "¡").Replace("&ordf;", "ª").Replace("&ccedil;", "ç").Replace("&cent;", "¢").Replace("&not;", "¬").Replace("&iuml;", "").Replace("&euro;", "€").Replace("&lsaquo;", "‹").Replace("&iacute;", "í").Replace("&lsquo;", "‘").Replace("&trade;", "™").Replace("&plusmn;", "±").Replace("&uuml;", "ü").Replace("&copy;", "©").Replace("&shy;", "­").Replace("&pound;", "£");

                    //XDocument doc = XDocument.Parse(xml1);
                    //return new XmlActionResult(doc);
                    //return Content(doc.ToString(), "text/xml");

                    return Content(xml1.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("IvsVisaCountriesDetails", "GeneralInfo").Replace("IvsVisaCountriesHolidays", "Holiday")
                      .Replace("IvsVisaCountriesAirlines", "Airline").Replace("IvsVisaCountriesAirports", "Airport").Replace("IvsVisaCountriesDiplomaticRepresentation", "Offices")
                      .Replace("IvsVisaHelpAddress", "Office").Replace("<Visainf>", "").Replace("</Visainf>", "").Replace("IvsVisaSaarcDetails", "CountryOffices").Replace("<cNames>", "").Replace("</cNames>", "") //.Replace("![CDATA[", "").Replace("]]", "")
                      .Replace("IvsVisaInformation", "VisaInformation").Replace("IvsVisaCategories", "Category").Replace("IvsVisaCategoriesOptions", "CategoryFee").Replace("CategoryOptions", "CategoryFee")
                      , "text/xml");
                }
            }
            else
            {
                //if (Accept != null && Accept.ToLower() == "application/json")
                if ((Accept != null && Accept.ToLower() == "application/json") || (ContentType != null && ContentType.ToLower() == "application/json"))
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


        [HttpGet]
        public ActionResult getdetails222()
        {
            Root data = new Root();
            string Countryiso = "";
            string Id = "";
            string UserName = "";
            string Password = "";

            XmlSerializer xsSubmit1 = new XmlSerializer(typeof(Root));
            var subReq1 = data;
            var xmll = "";
            APILogs objAppLog = new APILogs();

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
            Root obj = new Root();
            bool isValid = true;
            if (Request.ContentType == "application/xml" || Request.ContentType == "text/xml;charset=UTF-8")
            {
                type = "xml";

                var xmldoc = new XmlDocument();
                string xml = Convert.ToString(input).Replace(Environment.NewLine, "");
                xmldoc.LoadXml(xml.Replace("SOAP:", "SOAP"));
                var fromXml = JsonConvert.SerializeXmlNode(xmldoc);

                data = JsonConvert.DeserializeObject<Root>(fromXml);

                Countryiso = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.VisaSearchRequest1.CountryDetails1.Country.Code;
                Id = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Id;
                UserName = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.UserName;
                Password = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Password;
                objAppLog.ErrLog("Countryiso : " + Countryiso + "Id ; " + Id + "UserName ; " + UserName + " Password : " + Password + "IP : " + Request.HttpContext.Connection.RemoteIpAddress.ToString());

                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString()
                            && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable
                            select m;
                if (model.Count() > 0)
                    isValid = true;
                objAppLog.ErrLog("Authentication Result : " + isValid);
            }
            else if (Request.ContentType == "application/json")
            {
                //type = "json";
                //data = JsonConvert.DeserializeObject<Root>(input);
                //---------------------
                var xmldoc = new XmlDocument();
                string xml = Convert.ToString(input).Replace(Environment.NewLine, "");
                xmldoc.LoadXml(xml.Replace("SOAP:", "SOAP"));
                var fromXml = JsonConvert.SerializeXmlNode(xmldoc);

                data = JsonConvert.DeserializeObject<Root>(fromXml);

                //---------------------
                Countryiso = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.VisaSearchRequest1.CountryDetails1.Country.Code;
                Id = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Id;
                UserName = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.UserName;
                Password = data.SOAPEnvelope.SOAPBody.IVSource1.IVA_VisaSearchRequest.POS.Source.Password;
                objAppLog.ErrLog("Countryiso : " + Countryiso + " Id ; " + Id + " UserName ; " + UserName + " Password : " + Password + "IP : " + Request.HttpContext.Connection.RemoteIpAddress.ToString());
                var model = from m in _context.IvsVisaClientIpRegistration
                            join n in _context.IvsUserDetails on m.SerialNum equals n.SerialNum
                            where n.CorporateId == Id && m.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString()
                            && n.UserName == UserName && n.Password == Password && n.IsEnable == 1 && m.IsEnable

                            select m;
                if (model.Count() > 0)
                    isValid = true;
                objAppLog.ErrLog("Authentication Result : " + isValid);
            }
            if (isValid)
            {
                List<CountryName> name = (from country in _context.IvsVisaCountries
                                          where country.CountryIso == Countryiso
                                          select new CountryName
                                          {
                                              Name = country.CountryName,
                                          }).ToList();

                List<IvsVisaCountriesDetails> CountryList = (from country in _context.IvsVisaCountries
                                                             join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                                             where country.CountryIso == Countryiso
                                                             select new IvsVisaCountriesDetails
                                                             {
                                                                 CountryIso = us.CountryIso,
                                                                 CountryArea = us.CountryArea,
                                                                 CountryCapital = us.CountryCapital,
                                                                 CountryClimate = us.CountryClimate,
                                                                 CountryCurrency = us.CountryCurrency,
                                                                 CountryFlag = us.CountryFlag.ToString(),
                                                                 CountryLanguages = us.CountryLanguages,
                                                                 CountryLargeMap = us.CountryLargeMap.ToString(),
                                                                 CountryLocation = us.CountryLocation,
                                                                 CountryNationalDay = us.CountryNationalDay,
                                                                 CountryPopulation = us.CountryPopulation,
                                                                 CountrySmallMap = us.CountrySmallMap.ToString(),
                                                                 CountryTime = us.CountryTime,
                                                                 CountryWorldFactBook = us.CountryWorldFactBook
                                                             }).ToList();
                //select new IvsVisaCountriesDetails
                //{
                //  CountryIso = us.CountryIso,
                // // CountryName = new CountryName {Name=country.CountryName },
                //  CountryArea = us.CountryArea,
                //  CountryCapital = us.CountryCapital,
                //  CountryClimate = us.CountryClimate,
                //  CountryCurrency = us.CountryCurrency,
                //  CountryFlag = "http://app.ivsource.com/app/img/flag/" + us.CountryFlag.ToString(),
                //  CountryLanguages = us.CountryLanguages,
                //  CountryLargeMap = "http://app.ivsource.com/app/img/map/large/" + us.CountryLargeMap.ToString(),
                //  CountryLocation = us.CountryLocation,
                //  CountryNationalDay = us.CountryNationalDay,
                //  CountryPopulation = us.CountryPopulation,
                //  CountrySmallMap = "http://app.ivsource.com/app/img/map/small/" + us.CountrySmallMap.ToString(),
                //  CountryTime = us.CountryTime,
                //  CountryWorldFactBook = us.CountryWorldFactBook
                //}).ToList();
                //// Holidays 
                List<IvsVisaCountriesHolidays> HolidaysList = (from country in _context.IvsVisaCountries
                                                               join us in _context.IvsVisaCountriesHolidays on country.CountryIso equals us.CountryIso
                                                               where country.CountryIso == Countryiso && us.IsEnable == 1
                                                               select new IvsVisaCountriesHolidays
                                                               {
                                                                   CountryIso = us.CountryIso,
                                                                   Date = us.Date.Replace(@"", " "),
                                                                   HolidayName = WebUtility.HtmlDecode(GetFormattedValue(us.HolidayName)),
                                                                   Month = us.Month,
                                                                   Year = us.Year
                                                               }).ToList();
                //Air Lines
                List<IvsVisaCountriesAirlines> AirLines = (from country in _context.IvsVisaCountries
                                                           join us in _context.IvsVisaCountriesAirlines on country.CountryIso equals us.CountryIso
                                                           where country.CountryIso == Countryiso
                                                           select new IvsVisaCountriesAirlines
                                                           {
                                                               CountryIso = us.CountryIso,
                                                               AirlineCode = us.AirlineCode,
                                                               AirlineName = us.AirlineName
                                                           }).ToList();

                // Airports
                List<IvsVisaCountriesAirports> AirPorts = (from country in _context.IvsVisaCountries
                                                           join us in _context.IvsVisaCountriesAirports on country.CountryIso equals us.CountryIso
                                                           where country.CountryIso == Countryiso
                                                           select new IvsVisaCountriesAirports
                                                           {
                                                               CountryIso = us.CountryIso,
                                                               AirportCode = us.AirportCode,
                                                               AirportName = us.AirportName,
                                                               AirportType = us.AirportType
                                                           }).ToList();
                //DiplomaticRepresentation Offices  list
                List<IvsVisaCountriesDiplomaticRepresentation> DiplomaticRepresentation = (from country in _context.IvsVisaCountries
                                                                                           join us in _context.IvsVisaCountriesDiplomaticRepresentation on country.CountryIso equals us.CountryIso
                                                                                           where country.CountryIso == Countryiso && us.IsEnable == 1
                                                                                           select new IvsVisaCountriesDiplomaticRepresentation
                                                                                           {
                                                                                               CountryIso = us.CountryIso,
                                                                                               OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                                                               OfficeCity = WebUtility.HtmlDecode(us.OfficeCity),
                                                                                               OfficeCollectionTimings = us.OfficeCollectionTimings,
                                                                                               OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                                                               OfficeEmail = us.OfficeEmail,
                                                                                               OfficeFax = us.OfficeFax,
                                                                                               OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                                                               OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes),
                                                                                               OfficePhone = us.OfficePhone,
                                                                                               OfficePincode = us.OfficePincode,
                                                                                               OfficePublicTimings = us.OfficePublicTimings,
                                                                                               OfficeTelephoneVisa = us.OfficeTelephoneVisa,
                                                                                               OfficeVisaTimings = us.OfficeVisaTimings,
                                                                                               OfficeTimings = us.OfficeTimings,
                                                                                               OfficeWebsite = us.OfficeWebsite
                                                                                           }).ToList();
                //IndianEmbassy
                List<IvsVisaHelpAddress> IndianEmbassy = (from country in _context.IvsVisaCountries
                                                          join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                                          where country.CountryIso == Countryiso && us.AddressType == "IMO"
                                                          select new IvsVisaHelpAddress
                                                          {
                                                              CountryIso = us.CountryIso,
                                                              OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                              OfficeCity = WebUtility.HtmlDecode(us.OfficeCity.Replace(@"\n", String.Empty)),
                                                              OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                              OfficeEmail = us.OfficeEmail,
                                                              OfficeFax = us.OfficeFax,
                                                              OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                              OfficePincode = us.OfficePincode,
                                                              OfficeUrl = us.OfficeUrl,
                                                              OfficeWebsite = us.OfficeWebsite,
                                                              OfficePhone = us.OfficePhone,
                                                              OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes)
                                                          }).ToList();

                //IndialHelpAddress
                List<IvsVisaHelpAddress> IndialHelpAddress = (from country in _context.IvsVisaCountries
                                                              join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                                              where country.CountryIso == Countryiso && us.AddressType == "IHA" && us.IsEnable == 1
                                                              select new IvsVisaHelpAddress
                                                              {
                                                                  CountryIso = us.CountryIso,
                                                                  OfficeAddress = WebUtility.HtmlDecode(us.OfficeAddress),
                                                                  OfficeCity = WebUtility.HtmlDecode(us.OfficeCity),
                                                                  OfficeCountry = WebUtility.HtmlDecode(us.OfficeCountry),
                                                                  OfficeEmail = us.OfficeEmail,
                                                                  OfficeFax = us.OfficeFax,
                                                                  OfficeName = WebUtility.HtmlDecode(us.OfficeName),
                                                                  OfficePincode = us.OfficePincode,
                                                                  OfficeUrl = us.OfficeUrl,
                                                                  OfficeWebsite = us.OfficeWebsite,
                                                                  OfficePhone = us.OfficePhone,
                                                                  OfficeNotes = WebUtility.HtmlDecode(us.OfficeNotes)
                                                              }).ToList();
                //Advisory
                var Advisory = from country in _context.IvsVisaCountries
                               join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                               where country.CountryIso == Countryiso && us.AdvisoryType == "IVSAD"
                               //select new IvsVisaCountryAdvisories
                               //{
                               //  CountryIso = us.CountryIso,
                               //  Advisory = us.Advisory
                               //}).ToList();
                               select WebUtility.HtmlDecode(us.Advisory);

                //ReciprocalVisaInfo
                //List<IvsVisaCountryAdvisories> 
                var ReciprocalVisaInfo = from country in _context.IvsVisaCountries
                                         join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                         where country.CountryIso == Countryiso && us.AdvisoryType == "RECVIS"
                                         //select new IvsVisaCountryAdvisories
                                         //{
                                         //  CountryIso = us.CountryIso,
                                         //  Advisory = HttpUtility.HtmlDecode(us.Advisory)
                                         //}).ToList();
                                         select WebUtility.HtmlDecode(us.Advisory);
                //InternationalAdvisory
                //List<IvsVisaCountryAdvisories> 
                var InternationalAdvisory = from country in _context.IvsVisaCountries
                                            join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                            where country.CountryIso == Countryiso && us.AdvisoryType == "INTAD"
                                            //  select new IvsVisaCountryAdvisories
                                            //  {
                                            //    CountryIso = us.CountryIso,
                                            //    Advisory = WebUtility.HtmlDecode(us.Advisory)
                                            //  }).ToList();
                                            select WebUtility.HtmlDecode(us.Advisory);
                // SAARCInfo
                List<IvsVisaSaarcDetails> SAARCInfo = (from country in _context.IvsVisaCountries
                                                       join us in _context.IvsVisaSaarcDetails on country.CountryIso equals us.CountryIso
                                                       where country.CountryIso == Countryiso
                                                       select new IvsVisaSaarcDetails
                                                       {
                                                           CountryIso = us.CountryIso,
                                                           IsVisaRequired = WebUtility.HtmlDecode(us.IsVisaRequired),
                                                           VisaApplyWhere = WebUtility.HtmlDecode(us.VisaApplyWhere),
                                                           VisaOfficeAddress = WebUtility.HtmlDecode(us.VisaOfficeAddress),
                                                           VisaOfficeCity = WebUtility.HtmlDecode(us.VisaOfficeCity),
                                                           VisaOfficeCountry = WebUtility.HtmlDecode(us.VisaOfficeCountry),
                                                           CountryId = us.CountryId,
                                                           VisaOfficeEmail = us.VisaOfficeEmail,
                                                           VisaOfficeFax = us.VisaOfficeFax,
                                                           VisaOfficeName = WebUtility.HtmlDecode(us.VisaOfficeName),
                                                           VisaOfficePincode = us.VisaOfficePincode,
                                                           VisaOfficeTelephone = us.VisaOfficeTelephone,
                                                           VisaOfficeWebsite = us.VisaOfficeWebsite,
                                                           VisaOfficeNotes = WebUtility.HtmlDecode(us.VisaOfficeNotes)
                                                       }).ToList();

                //Category
                // List<IvsVisaCategories> Category = new List<IvsVisaCategories>();
                List<IvsVisaCategories> Category1 = (from country in _context.IvsVisaCategories
                                                     join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                     //  join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode 
                                                     //join ss in _context.IvsVisaQuickContactInfo on country.VisaCategoryId equals ss.VisaCategoryCode //commented by Hitesh on 05062023
                                                     where country.CountryIso == Countryiso
                                                     orderby country.VisaCategory ascending
                                                     select new IvsVisaCategories
                                                     {
                                                         //CountryIso = ss.CountryIso,
                                                         VisaCategoryRequirements = (WebUtility.HtmlDecode(country.VisaCategoryRequirements)),
                                                         VisaCategoryInformation = (WebUtility.HtmlDecode(country.VisaCategoryInformation)),
                                                         VisaCategoryNotes = (WebUtility.HtmlDecode(country.VisaCategoryNotes)),
                                                         VisaCategoryId = country.VisaCategoryId,// get visacatid for fees
                                                         VisaCategory = WebUtility.HtmlDecode(country.VisaCategory),// show
                                                         CityId = country.CityId,//get city for category
                                                                                 //   VisaCategoryCode = ss.VisaCategoryCode //      commented for multiple data 11oc2021                        
                                                     }).Distinct().ToList();
                // CategoryFees
                List<IvsVisaCategoriesOptions> CategoryFees = (from country in _context.IvsVisaCategories
                                                               join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                               join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode
                                                               where country.CountryIso == Countryiso
                                                               select new IvsVisaCategoriesOptions
                                                               {
                                                                   CountryIso = ss.CountryIso,
                                                                   VisaCategoryCode = WebUtility.HtmlDecode(ss.VisaCategoryCode),
                                                                   VisaCategoryOption = WebUtility.HtmlDecode(ss.VisaCategoryOption),
                                                                   VisaCategoryOptionAmountInr = WebUtility.HtmlDecode(ss.VisaCategoryOptionAmountInr),
                                                                   VisaCategoryOptionAmountOther = ss.VisaCategoryOptionAmountOther
                                                               }).ToList();
                // CategoryFees
                List<IvsVisaCategoriesForms> CategoryForms = (from country in _context.IvsVisaCategoriesForms
                                                              join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                                              join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryCode equals ss.VisaCategoryCode
                                                              where country.CountryIso == Countryiso && country.IsEnable == 1
                                                              select new IvsVisaCategoriesForms
                                                              {
                                                                  CountryIso = ss.CountryIso,
                                                                  VisaCategoryCode = WebUtility.HtmlDecode(ss.VisaCategoryCode),
                                                                  Form = WebUtility.HtmlDecode(country.Form),
                                                                  FormPath = "http://app.ivsource.com/app/img/forms/" + country.FormPath

                                                              }).ToList();

                //VisaInformation
                List<IvsVisaInformation> VisaInformation = (from country in _context.IvsVisaCountries
                                                            join ss in _context.IvsVisaInformation on country.CountryIso equals ss.CountryIso
                                                            join us in _context.IvsVisaCountryTerritoryCities on ss.CityId equals us.CityId
                                                            where country.CountryIso == Countryiso
                                                            orderby us.CityId ascending
                                                            select new IvsVisaInformation
                                                            {
                                                                CountryIso = country.CountryIso,
                                                                VisaInformation = WebUtility.HtmlDecode(ss.VisaInformation),
                                                                VisaGeneralInformation = WebUtility.HtmlDecode(ss.VisaGeneralInformation),
                                                                CityId = ss.CityId,
                                                                CityName = WebUtility.HtmlDecode(us.CityName),
                                                                VisaGeneralInformation1 = new IvsVisaCategoriesForApi1 { VisaCategoryRequirements = WebUtility.HtmlDecode(ss.VisaGeneralInformation), VisaInformation = WebUtility.HtmlDecode(ss.VisaInformation) },
                                                                Categories = new IvsVisaInformationCategories { Category = Category1.Where(x => x.CityId == ss.CityId).Distinct().ToList() },
                                                                //CategoryFees=new IvsVisaCategoriesFees {CategoryFee=CategoryFees.Where(x=>x.VisaCategory==null).ToList() } //Comment by Hitesh on 05062023
                                                            }).ToList();

                //foreach (var info in VisaInformation)
                //{
                //    foreach (var category in info.Categories.Category.Distinct())
                //    {
                //        info.CategoryFees = new IvsVisaCategoriesFees();
                //        info.CategoryFees.CategoryFee =  CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).ToList();
                //    }
                //}
                foreach (var info in VisaInformation)
                {
                    var oTempList = new List<IvsVisaCategoriesFees>();
                    //oTempList.CategoryFees = new List<IvsVisaCategoriesFees>();
                    //foreach (var category in info.Categories.Category.Where(x => x.CityId == info.CityId).Distinct())
                    //{
                    //    IvsVisaCategoriesFees CategoryFee = new IvsVisaCategoriesFees();
                    //    CategoryFee.CategoryFee = CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).ToList();
                    //    category.CategoryFeesDetails = new CategoryFeeDetails();
                    //    category.CategoryFees = CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).ToList();
                    //    category.CategoryFeesDetails.CategoryFee = CategoryFees.Where(x => x.VisaCategoryCode == category.VisaCategoryId).ToList();
                    //    oTempList.CategoryFee.Add(CategoryFee);
                    //}
                    info.CategoryFees = oTempList;

                    //info.CategoryFees = new IvsVisaCategoriesFees();
                    //var asCatIDS = info.Categories.Category.Select(x => x.VisaCategoryId).ToList();
                    //info.CategoryFees.CategoryFee = CategoryFees.Where(x => asCatIDS.Contains(x.VisaCategoryCode)).OrderBy(x => x.VisaCategoryCode).ToList();

                }
                foreach (var info1 in VisaInformation)
                {
                    var oTempList = new List<CategoryForms>();
                    //oTempList.CategoryFormList = new List<VisaCategoryForms>();
                    foreach (var category1 in info1.Categories.Category.Distinct())
                    {
                        CategoryForms categoryForm = new CategoryForms();
                        categoryForm.CategoryForm = CategoryForms.Where(x => x.VisaCategoryCode == category1.VisaCategoryId).OrderBy(x => x.VisaCategoryCode).ToList();
                        oTempList.Add(categoryForm);
                    }
                    info1.IvsVisaCategoryForms = oTempList;
                }

                VisaDetails VisaDetail = new VisaDetails();
                VisaDetail.CountryDetails = new CountryDetail();
                VisaDetail.CountryDetails.cNames = name;
                VisaDetail.CountryDetails.GeneralInfo = CountryList;
                VisaDetail.CountryCode = Countryiso;
                // VisaDetail.cNames = name;
                VisaDetail.CountryDetails.Holidays = HolidaysList;
                VisaDetail.CountryDetails.Airlines = AirLines;
                VisaDetail.CountryDetails.Airports = AirPorts;
                VisaDetail.DiplomaticRepresentation = new DiplomaticRepresentation();
                VisaDetail.DiplomaticRepresentation.Office = new Office();
                VisaDetail.DiplomaticRepresentation.Office.Offices = DiplomaticRepresentation;
                //VisaDetail.DiplomaticRepresentation = DiplomaticRepresentation;
                VisaDetail.IndianEmbassy = IndianEmbassy;
                //VisaDetail.IntlHelpAddress = IndialHelpAddress;
                VisaDetail.IntlHelpAddress = new IntlHelpAddress();
                VisaDetail.IntlHelpAddress.HelpAddress = IndialHelpAddress;
                VisaDetail.IVSAdvisory = new Descriptions();
                VisaDetail.IVSAdvisory.Description = Advisory.ToList();
                //  VisaDetail.ReciprocalVisaInfo = ReciprocalVisaInfo;
                VisaDetail.ReciprocalVisaInfo = new Descriptions();
                VisaDetail.ReciprocalVisaInfo.Description = ReciprocalVisaInfo.ToList();
                // VisaDetail.InternationalAdvisory = InternationalAdvisory;
                VisaDetail.InternationalAdvisory = new Descriptions();
                VisaDetail.InternationalAdvisory.Description = InternationalAdvisory.ToList();
                //VisaDetail.SAARCInfo = SAARCInfo;
                VisaDetail.SAARCInfo = new SAARCInfo();
                VisaDetail.SAARCInfo.CountryOffice = new CountryOffice();
                VisaDetail.SAARCInfo.CountryOffice.CountryOffices = SAARCInfo;
                VisaDetail.Visainf = VisaInformation;
                VisaDetail.AdditionalInfo = "";
                //VisaDetail.Categories = Category1.Distinct().ToList();
                // VisaDetail.CategoryFees = CategoryFees;
                // VisaDetail.CategoriesForm = CategoryForms;
                VisaSearchResponse visaSearchResponse = new VisaSearchResponse { VisaDetails = VisaDetail };
                IVA_VisaSearchResponse objXml = new IVA_VisaSearchResponse { VisaSearchResponse = visaSearchResponse };
                VisaSearchRequest objJson = new VisaSearchRequest { IVA_VisaSearchResponse = objXml };

                XmlSerializer xsSubmit = new XmlSerializer(typeof(IVA_VisaSearchResponse));
                dynamic subReq = objXml;

                var xml1 = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, subReq);
                        xml1 = @"<soap:Envelope soap:encodingStyle='http://schemas.xmlsoap.org/soap/encoding/' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soapenc='http://schemas.xmlsoap.org/soap/encoding/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>" +
                          "<soap:Body><IVSource xmlns='http://www.IVSource.com/IVACONT/Global'>" + //HttpUtility.HtmlDecode(
                            sww.ToString() //.Replace(@"\n", String.Empty).Replace("![CDATA[", "").Replace("]]", "").Replace("&nbsp;", "")
                                           //)
                            ; // Your XML
                    }
                }
                string strResponse = string.Empty;
                if (Accept != null && Accept.ToLower() == "application/json")
                {
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                    strResponse = JsonConvert.SerializeObject(objJson);
                    strResponse = rx.Replace(strResponse, "");
                    dynamic json = JsonConvert.DeserializeObject(strResponse);
                    return new JsonResult(json);
                }
                else
                {
                    xml1 = //HttpUtility.HtmlDecode(
                      xml1
                      //)

                      + "</IVSource> </soap:Body> "
                        + "</soap:Envelope>";
                    //.Replace("&lt;", "<").Replace("&amp;", "&")
                    //                               .Replace("&gt;", ">")
                    //                               .Replace("&quot;", "\"")
                    //                               .Replace("&apos;", "'").Replace("![CDATA[", "").Replace("]]", "").Replace("\t\t   \t\t", "");

                    if (!string.IsNullOrEmpty(xml1))
                    {
                        // replace entities with literal values
                        xml1 = xml1.Replace("&amp;quot;", "\"");
                        xml1 = xml1.Replace("&amp;gt;", ">");
                        xml1 = xml1.Replace("&amp;lt;", "<");
                        //xml1 = xml1.Replace("&amp;lt;", "&");

                        xml1 = xml1.Replace("&amp;", "&");
                        xml1 = xml1.Replace("&apos;", "'");
                        xml1 = xml1.Replace("&quot;", "\"");
                        xml1 = xml1.Replace("&gt;", ">");
                        xml1 = xml1.Replace("&lt;", "<");
                        xml1 = xml1.Replace("&nbsp;", " ");
                        xml1 = xml1.Replace("&amp;nbsp;", " ");
                        xml1 = xml1.Replace("\\", "");
                        xml1 = xml1.Replace(@"\", "");
                        xml1 = xml1.Replace(@"&", "and");
                    }
                    byte[] data1 = System.Text.Encoding.Default.GetBytes(xml1);

                    xml1 = //HttpUtility.HtmlDecode
                           // xml1;
                           // Uri.EscapeUriString(xml1);
                    xml1.Replace("/[^(\x20-\x7F)\x0A\x0D]*/", "").Replace("&acirc;", "â").Replace("&rsquo;", "’").Replace("&aacute;", "á").Replace("&oacute;", "ó").Replace("&atilde;", "").Replace("&ndash;", "–").Replace("&Atilde;", "Ã").Replace("&iexcl;", "¡").Replace("&ordf;", "ª").Replace("&ccedil;", "ç").Replace("&cent;", "¢").Replace("&not;", "¬").Replace("&iuml;", "").Replace("&euro;", "€").Replace("&lsaquo;", "‹").Replace("&iacute;", "í").Replace("&lsquo;", "‘").Replace("&trade;", "™").Replace("&plusmn;", "±").Replace("&uuml;", "ü").Replace("&copy;", "©").Replace("&shy;", "­").Replace("&pound;", "£");

                    //XDocument doc = XDocument.Parse(xml1);
                    //return new XmlActionResult(doc);
                    //return Content(doc.ToString(), "text/xml");

                    return Content(xml1.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("IvsVisaCountriesDetails", "GeneralInfo").Replace("IvsVisaCountriesHolidays", "Holiday")
                      .Replace("IvsVisaCountriesAirlines", "Airline").Replace("IvsVisaCountriesAirports", "Airport").Replace("IvsVisaCountriesDiplomaticRepresentation", "Offices")
                      .Replace("IvsVisaHelpAddress", "Office").Replace("<Visainf>", "").Replace("</Visainf>", "").Replace("IvsVisaSaarcDetails", "CountryOffices").Replace("<cNames>", "").Replace("</cNames>", "") //.Replace("![CDATA[", "").Replace("]]", "")
                      .Replace("IvsVisaInformation", "VisaInformation").Replace("IvsVisaCategories", "Category").Replace("IvsVisaCategoriesOptions", "CategoryFee").Replace("CategoryOptions", "CategoryFee")
                      , "text/xml");
                }
            }
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




        public string GetFormattedValue(string xml1)
        {
            if (!string.IsNullOrEmpty(xml1))
            {
                // replace entities with literal values
                xml1 = xml1.Replace("&amp;quot;", "\"");
                xml1 = xml1.Replace("&amp;gt;", ">");
                xml1 = xml1.Replace("&amp;lt;", "<");
                //xml1 = xml1.Replace("&amp;lt;", "&");

                xml1 = xml1.Replace(@"\", "");
                xml1 = xml1.Replace("&amp;", "&");
                xml1 = xml1.Replace("&apos;", "'");
                xml1 = xml1.Replace("&quot;", "\"");
                xml1 = xml1.Replace("&gt;", ">");
                xml1 = xml1.Replace("&lt;", "<");
                xml1 = xml1.Replace("&nbsp;", " ");
                xml1 = xml1.Replace("&amp;nbsp;", " ");
                xml1 = xml1.Replace("\\", "");

            }
            return xml1;

        }
        /*  public IActionResult getdetails(string Countryiso)
            // public IActionResult getdetails(string Countryiso ,[FromHeader]string Id, [FromHeader]string UserName, [FromHeader]string Password)
            {
                //var CheckCountryIso = from country in _context.IvsVisaCountries
                //                 select country.CountryIso;
                bool CountryExists = _context.IvsVisaCountries.Any(x => x.CountryIso == Countryiso);

                //var ID = _configuration["VisaApi:Id"];
                //var UserN = _configuration["VisaApi:UserName"];
                //var Pass = _configuration["VisaApi:Password"];

                //if (ID==Id && UserN==UserName && Pass==Password)
                //{
                if (Countryiso != null && CountryExists == true) { 

                //country details general information
                var countryDetail = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                    where country.CountryIso == Countryiso
                                    select new IvsVisaCountriesDetails
                                    {
                                        CountryIso = us.CountryIso,
                                        bCountryName = country.CountryName,
                                        CountryArea = us.CountryArea,
                                        CountryCapital = us.CountryCapital,
                                        CountryCurrency = us.CountryCurrency,
                                        CountryClimate=HttpUtility.UrlDecode(us.CountryClimate),
                                        CountryFlag = us.CountryFlag,
                                        CountryLanguages = us.CountryLanguages,
                                        CountryLargeMap = us.CountryLargeMap,
                                        CountryLocation = us.CountryLocation,
                                        CountryNationalDay = us.CountryNationalDay,
                                        CountryPopulation = us.CountryPopulation,
                                        CountrySmallMap = us.CountrySmallMap,
                                        CountryTime = us.CountryTime,
                                        CountryWorldFactBook = us.CountryWorldFactBook
                                    };
                var model1 = countryDetail.ToList();

                // holydays name
                var HolydaysName = from country in _context.IvsVisaCountries
                                   join us in _context.IvsVisaCountriesHolidays on country.CountryIso equals us.CountryIso
                                   where country.CountryIso == Countryiso && us.IsEnable == 1
                                   select new IvsVisaCountriesHolidays
                                   {
                                       CountryIso = us.CountryIso,
                                       Date = us.Date,
                                       //HolidayName = us.HolidayName.Replace("&lt;", "<").Replace("&amp;", "&")
                                       //                .Replace("&gt;", ">")
                                       //                .Replace("&quot;", "\"")
                                       //                .Replace("&apos;", "'"),
                                       HolidayName=HttpUtility.UrlDecode(us.HolidayName),
                                       Month = us.Month,
                                       Year = us.Year
                                   };
                var model2 = HolydaysName.ToList();

                //Air Lines
                var AirLines = from country in _context.IvsVisaCountries
                               join us in _context.IvsVisaCountriesAirlines on country.CountryIso equals us.CountryIso
                               where country.CountryIso == Countryiso
                               select new IvsVisaCountriesAirlines
                               {
                                   CountryIso = us.CountryIso,
                                   AirlineCode = us.AirlineCode,
                                   AirlineName = us.AirlineName
                               };
                var model3 = AirLines.ToList();

                // Airports
                var AirPorts = from country in _context.IvsVisaCountries
                               join us in _context.IvsVisaCountriesAirports on country.CountryIso equals us.CountryIso
                               where country.CountryIso == Countryiso
                               select new IvsVisaCountriesAirports
                               {
                                   CountryIso = us.CountryIso,
                                   AirportCode = us.AirportCode,
                                   AirportName = us.AirportName,
                                   AirportType = us.AirportType
                               };
                var model4 = AirPorts.ToList();

                //DiplomaticRepresentation Offices  list
                var DiplomaticRepresentation = from country in _context.IvsVisaCountries
                                               join us in _context.IvsVisaCountriesDiplomaticRepresentation on country.CountryIso equals us.CountryIso
                                               where country.CountryIso == Countryiso
                                               select new IvsVisaCountriesDiplomaticRepresentation
                                               {
                                                   CountryIso = us.CountryIso,
                                                   OfficeAddress = us.OfficeAddress,
                                                   OfficeCity = us.OfficeCity,
                                                   OfficeCollectionTimings = us.OfficeCollectionTimings,
                                                   OfficeEmail = us.OfficeEmail,
                                                   OfficeFax = us.OfficeFax,
                                                   OfficeName = us.OfficeName,
                                                   OfficeNotes = us.OfficeNotes,
                                                   OfficePhone = us.OfficePhone,
                                                   OfficePincode = us.OfficePincode,
                                                   OfficePublicTimings = us.OfficePublicTimings,
                                                   OfficeTelephoneVisa = us.OfficeTelephoneVisa,
                                                   OfficeVisaTimings = us.OfficeVisaTimings,
                                                   OfficeTimings = us.OfficeTimings,
                                                   OfficeWebsite = us.OfficeWebsite
                                               };
                var model5 = DiplomaticRepresentation.ToList();

                //IndianEmbassy
                var IndianEmbassy = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                    where country.CountryIso == Countryiso && us.AddressType == "IMO"
                                    select new IvsVisaHelpAddress
                                    {
                                        CountryIso = us.CountryIso,
                                        OfficeAddress = us.OfficeAddress,
                                        OfficeCity = us.OfficeCity.Replace(@"\n", String.Empty),
                                        OfficeCountry = us.OfficeCountry,
                                        OfficeEmail = us.OfficeEmail,
                                        OfficeFax = us.OfficeFax,
                                        OfficeName = us.OfficeName,
                                        OfficePincode = us.OfficePincode,
                                        OfficeUrl = us.OfficeUrl,
                                        OfficeWebsite = us.OfficeWebsite,
                                        OfficePhone = us.OfficePhone,
                                        OfficeNotes = us.OfficeNotes
                                    };
                var model6 = IndianEmbassy.ToList();

                //IndialHelpAddress
                var IndialHelpAddress = from country in _context.IvsVisaCountries
                                        join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                        where country.CountryIso == Countryiso && us.AddressType == "IHA"
                                        select new IvsVisaHelpAddress
                                        {
                                            CountryIso = us.CountryIso,
                                            OfficeAddress = us.OfficeAddress,
                                            OfficeCity = us.OfficeCity,
                                            OfficeCountry = us.OfficeCountry,
                                            OfficeEmail = us.OfficeEmail,
                                            OfficeFax = us.OfficeFax,
                                            OfficeName = us.OfficeName,
                                            OfficePincode = us.OfficePincode,
                                            OfficeUrl = us.OfficeUrl,
                                            OfficeWebsite = us.OfficeWebsite,
                                            OfficeNotes = us.OfficeNotes
                                        };
                var model7 = IndialHelpAddress.ToList();

                //Advisory
                var Advisory = from country in _context.IvsVisaCountries
                               join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                               where country.CountryIso == Countryiso && us.AdvisoryType == "IVSAD"
                               select new IvsVisaCountryAdvisories
                               {
                                   CountryIso = us.CountryIso,
                                   Advisory = HttpUtility.HtmlDecode(us.Advisory)
                               };
                var model8 = Advisory.ToList();

                //ReciprocalVisaInfo
                var ReciprocalVisaInfo = from country in _context.IvsVisaCountries
                                         join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                         where country.CountryIso == Countryiso && us.AdvisoryType == "RECVIS"
                                         select new IvsVisaCountryAdvisories
                                         {
                                             CountryIso = us.CountryIso,
                                             Advisory = us.Advisory
                                         };
                var model9 = ReciprocalVisaInfo.ToList();
                ////Advisorydetails
                var Advisorydetails = from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                      where country.CountryIso == Countryiso && us.AdvisoryType == "INTAD"
                                      select new IvsVisaCountryAdvisories
                                      {
                                          CountryIso = us.CountryIso,
                                          Advisory = us.Advisory
                                      };
                var model10 = Advisorydetails.ToList();
                // SAARCInfo
                var SAARCInfo = from country in _context.IvsVisaCountries
                                join us in _context.IvsVisaSaarcDetails on country.CountryIso equals us.CountryIso
                                where country.CountryIso == Countryiso
                                select new IvsVisaSaarcDetails
                                {
                                    CountryIso = us.CountryIso,
                                    IsVisaRequired = us.IsVisaRequired,
                                    VisaApplyWhere = us.VisaApplyWhere,
                                    VisaOfficeAddress = us.VisaOfficeAddress,
                                    VisaOfficeCity = us.VisaOfficeCity,
                                    VisaOfficeCountry = us.VisaOfficeCountry,
                                    CountryId = us.CountryId,
                                    VisaOfficeFax = us.VisaOfficeFax,
                                    VisaOfficeName = us.VisaOfficeName,
                                    VisaOfficePincode = us.VisaOfficePincode,
                                    VisaOfficeTelephone = us.VisaOfficeTelephone,
                                    VisaOfficeWebsite = us.VisaOfficeWebsite
                                };
                var model11 = SAARCInfo.ToList();

                // VisaInformation

                var VisaInformation = from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaCountryTerritoryCities on country.IsEnable equals us.IsEnable
                                      join ss in _context.IvsVisaInformation on country.CountryIso equals ss.CountryIso
                                      where country.CountryIso == Countryiso //&&   ss.CountryIso == Countryiso
                                      select new IvsVisaCountryTerritoryCitiesWithVisaInfoApi
                                      {
                                          CountryIso = us.CountryIso,
                                          CityName = us.CityName,
                                          CityId = us.CityId,
                                          VisaInformation = HttpUtility.HtmlDecode(ss.VisaInformation),
                                          VisaGeneralInformation = HttpUtility.HtmlDecode(ss.VisaGeneralInformation),
                                      };
                var model13 = VisaInformation.ToList();
                // Info

                var Information = from country in _context.IvsVisaCountries
                                  join ss in _context.IvsVisaInformation on country.CountryIso equals ss.CountryIso
                                  join us in _context.IvsVisaCountryTerritoryCities on ss.CityId equals us.CityId
                                  where country.CountryIso == Countryiso
                                  select new IvsVisaInformation
                                  {
                                      CountryIso = country.CountryIso,
                                      VisaInformation = HttpUtility.HtmlDecode(ss.VisaInformation),
                                      VisaGeneralInformation = HttpUtility.HtmlDecode(ss.VisaGeneralInformation),
                                      CityId = ss.CityId,
                                      CityName = us.CityName
                                  };
                var model14 = Information.ToList();

                // CategoryNotes

                var CategoryNotes = from country in _context.IvsVisaCategories
                                    join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                    join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode
                                    where country.CountryIso == Countryiso

                                    select new IvsVisaCategories
                                    {
                                        CountryIso = ss.CountryIso,
                                        VisaCategoryRequirements = HttpUtility.HtmlDecode(country.VisaCategoryRequirements.Replace(@"\n", String.Empty)),
                                        VisaCategoryInformation = HttpUtility.HtmlDecode(country.VisaCategoryInformation.Replace(@"\n", String.Empty)),
                                        VisaCategoryNotes = HttpUtility.HtmlDecode(country.VisaCategoryNotes.Replace(@"\n", String.Empty)),
                                        VisaCategoryId = country.VisaCategoryId,//
                                        VisaCategory = country.VisaCategory,//
                                        CityId = country.CityId,
                                    };
                var model15 = CategoryNotes.Distinct().ToList();
                // CategoryOption

                var CategoryOpt = from country in _context.IvsVisaCategories
                                  join us in _context.IvsVisaCountries on country.CountryIso equals us.CountryIso
                                  join ss in _context.IvsVisaCategoriesOptions on country.VisaCategoryId equals ss.VisaCategoryCode
                                  where country.CountryIso == Countryiso
                                  select new IvsVisaCategoriesOptions
                                  {
                                      CountryIso = ss.CountryIso,
                                      VisaCategoryCode = ss.VisaCategoryCode,
                                      VisaCategoryOption = ss.VisaCategoryOption,
                                      VisaCategoryOptionAmountInr = ss.VisaCategoryOptionAmountInr,
                                      VisaCategoryOptionAmountOther = ss.VisaCategoryOptionAmountOther

                                  };
                var model16 = CategoryOpt.Distinct().ToList();
                //VisaSearchRequest visaSearchRequest = new VisaSearchRequest();
                //visaSearchRequest.IvsVisaCategories = model15.ToList();
                //visaSearchRequest.IvsVisaCategoriesOptions = model16.ToList();
                //
                // CategoryOption

                var CategoryOption = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCategoriesOptions on country.CountryIso equals us.CountryIso
                                     where country.CountryIso == Countryiso && us.VisaCategoryCode == ""
                                     select new IvsVisaCategoriesOptions
                                     {
                                         CountryIso = us.CountryIso,
                                         VisaCategoryCode = us.VisaCategoryCode,
                                         VisaCategoryOption = us.VisaCategoryOption,
                                         VisaCategory = us.VisaCategory,
                                         VisaCategoryOptionAmountInr = us.VisaCategoryOptionAmountInr,
                                         VisaCategoryOptionAmountOther = us.VisaCategoryOptionAmountOther
                                     };
                var model12 = CategoryOption.ToList();


                var CountryIso = from cou in _context.IvsVisaCountries
                                 select cou.CountryIso;
                XNamespace encodingStyle = "http://schemas.xmlsoap.org/soap/encoding/";
                XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                XNamespace soapenc = "http://schemas.xmlsoap.org/soap/encoding/";
                XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                var a = XNamespace.Get("http://www.IVSource.com/IVACONT/Global");


                var visaInfo = new XElement("VisaInformation");
                foreach (var item in model14)
                {
                    var id = item.CityId;
                    var terr = new XElement("TerritoryCity", item.CityName);
                    var vI = new XElement("VisaInfo", new XElement("VisaGeneralInformation", item.VisaGeneralInformation),
                        new XElement("VisaInformation", item.VisaInformation));
                    visaInfo.Add(terr);
                    visaInfo.Add(vI);
                    foreach (var items in model15.Where(n => n.CityId == id))
                    {
                        var CategoryCode = items.VisaCategoryId;
                        var cI = new XElement("Categories", new XElement("Category", new XElement("Category", items.VisaCategory),
                            new XElement("CategoryCode", items.VisaCategoryId),
                            new XElement("CategoryInfo", items.VisaCategoryInformation),
                            new XElement("CategoryNotes", items.VisaCategoryNotes),
                            new XElement("CategoryRequirements", items.VisaCategoryRequirements)
                            ));
                        visaInfo.Add(cI);
                        foreach (var catfess in model16.Where(n => n.VisaCategoryCode == CategoryCode))
                        {
                            var fees = new XElement("CategoryFees", new XElement("CategoryFee",
                                new XElement("CategoryCode", catfess.VisaCategoryCode), new XElement("Category", catfess.VisaCategoryOption),
                                new XElement("CategoryFeeAmountINR", catfess.VisaCategoryOptionAmountInr),
                                new XElement("CategoryFeeAmountOther", catfess.VisaCategoryOptionAmountOther)));
                            visaInfo.Add(fees);

                        }
                    }
                }

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
    new XElement("IVA_VisaSearchResponse",
    new XElement("VisaSearchResponse",
     new XElement("VisaDetails",
     new XAttribute("CountryCode", Countryiso),
    new XElement("CountryDetails",
    from country in model1
    select new XElement("CountryName",
    new XElement("Name", country.bCountryName)),
    from coun in model1
    select new XElement("GeneralInfo",
    new XElement("Area", coun.CountryArea),
    new XElement("Capital", coun.CountryCapital),
    new XElement("Climate", coun.CountryClimate),
    new XElement("Currency", coun.CountryCurrency),
    new XElement("Flag", coun.CountryFlag),
    new XElement("Code", coun.CountryIso),
    new XElement("Language", coun.CountryLanguages),
    new XElement("LargeMap", coun.CountryLargeMap),
    new XElement("Loaction", coun.CountryLocation),
    new XElement("NationalDay", coun.CountryNationalDay),
    new XElement("Papulation", coun.CountryPopulation),
    new XElement("SmallMap", coun.CountrySmallMap),
    new XElement("Time", coun.CountryTime),
    new XElement("WorldFactBook", coun.CountryWorldFactBook)),
    new XElement("Holidays",
                    from cou in model2
                    select new XElement("Holiday",
                    new XElement("Date",
                    new XElement("HolidayName", cou.HolidayName),
                    new XElement("Month", cou.Month),
                    new XElement("Year", cou.Year)))),
    new XElement("Airlines",
            from cou in model3
            select new XElement("Airline",
            new XElement("Code", cou.AirlineCode),
            new XElement("Name", cou.AirlineName))),
            new XElement("Airports",
            from cou in model4
            select new XElement("Airport",
            new XElement("Code", cou.AirportCode),
            new XElement("Name", cou.AirportName),
            new XElement("Type", cou.AirportType)))),
            new XElement("DiplomaticRepresentation",
    new XElement("Offices",
    from cou in model5
    select new XElement("Office",
    new XElement("Address", cou.OfficeAddress),
    new XElement("City", cou.OfficeCity),
    new XElement("CollectionTimings", cou.OfficeCollectionTimings),
    new XElement("Country", cou.OfficeCountry),
    new XElement("Email", cou.OfficeEmail),
    new XElement("Fax", cou.OfficeFax),
    new XElement("Name", cou.OfficeName),
    new XElement("Notes", cou.OfficeNotes),
    new XElement("Phone", cou.OfficePhone),
    new XElement("Pincode", cou.OfficePincode),
    new XElement("PublicTimings", cou.OfficePublicTimings),
    new XElement("Telephone", cou.OfficeTelephoneVisa),
    new XElement("Timing", cou.OfficeTimings),
    new XElement("VisaTimings", cou.OfficeVisaTimings),
    new XElement("Website", cou.OfficeWebsite)))),
            new XElement("IndianEmbassy",
    from cou in model6
    select new XElement("Office",
    new XElement("Address", cou.OfficeAddress),
    new XElement("City", cou.OfficeCity.Replace(@"\n", String.Empty)),
    new XElement("Country", cou.OfficeCountry),
    new XElement("Email", cou.OfficeEmail),
    new XElement("Fax", cou.OfficeFax),
    new XElement("Name", cou.OfficeName),
    new XElement("Notes", cou.OfficeNotes),
    new XElement("Phone", cou.OfficePhone),
    new XElement("Pincode", cou.OfficePincode),
    new XElement("Url", cou.OfficeUrl),
    new XElement("Website", cou.OfficeWebsite))),
            new XElement("IntlHelpAddress",
    from cou in model7
    select new XElement("HelpAddress",
    new XElement("Address", cou.OfficeAddress),
    new XElement("City", cou.OfficeCity),
    new XElement("Country", cou.OfficeCountry),
    new XElement("Email", cou.OfficeEmail),
    new XElement("Fax", cou.OfficeFax),
    new XElement("Name", cou.OfficeName),
    new XElement("Notes", cou.OfficeNotes),
    new XElement("Phone", cou.OfficePhone),
    new XElement("Pincode", cou.OfficePincode),
    new XElement("Url", cou.OfficeUrl),
    new XElement("Website", cou.OfficeWebsite))),
            new XElement("IVSAdvisory",
            //    new XElement("Description",
            //    new XElement("Heading",
            //    new XElement("Description",
            //new XElement("p",
            //new XElement("Heading",
            //new XElement("Description",
            // new XElement("p",
            from cou in model8
            select new XElement("Description",cou.Advisory)),
            new XElement("ReciprocalVisaInfo",
            //new XElement("Description",
            //new XElement("ReciprocalVisaInfo",
            // new XElement("p",
            from cou in model9
            select new XElement("Description", cou.Advisory)),
             new XElement("InternationalAdvisory",
            //new XElement("Description",
            //new XElement("InternationalAdvisory",
            // new XElement("P",
            from cou in model10
            select new XElement("Description",cou.Advisory)),
             new XElement("SAARCInfo",
                new XElement("CountryOffices",
            from coun in model11
            select new XElement("CountryOffice",
            new XElement("CountryID", coun.CountryIso),
            new XElement("VisaRequired", coun.IsVisaRequired),
            new XElement("WhereToApply", coun.VisaApplyWhere),
            new XElement("Address", coun.VisaOfficeAddress),
            new XElement("City", coun.VisaOfficeCity),
            new XElement("Country", coun.VisaOfficeCountry),
            new XElement("Fax", coun.VisaOfficeFax),
            new XElement("Name", coun.VisaOfficeName),
            new XElement("Pincode", coun.VisaOfficePincode),
            new XElement("Telephone", coun.VisaOfficeTelephone),
            new XElement("Website", coun.VisaOfficeWebsite)))),
            // new XElement("Visainformation",         
            visaInfo
    )))))));
                //
                XElement xmll;
                while (true)
                {
                    var empties = xml.Descendants().Where(x => x.IsEmpty && !x.HasAttributes).ToList();
                    if (empties.Count == 0)
                        break;

                    empties.ForEach(e => e.Remove());
                }
                //
                foreach (var node in xml.Root.Descendants())
                {
                    if (node.Name.NamespaceName == "")
                    {
                        node.Attributes("xmlns").Remove();
                        node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                    }
                }
                var s = xml.ToString();
                    //


                    //
                    return Content(s.Replace(@"&lt;", "<").Replace("&amp;", "&")
                                                       .Replace("&gt;", ">")
                                                       .Replace("&quot;", "\"")
                                                       .Replace("&apos;", "'"), "text/xml");
                //
                // start blank data msg
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
                            new XElement("IVA_VisaSearchResponse",
                            new XElement("VisaSearchResponse",
                            new XElement("VisaDetails",
                            new XAttribute("CountryCode", Countryiso),
                            new XElement("Visa",
                            new XElement(new XElement("Error", "No Record Found"))))))))));

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
                // end blank data msg
                //}
                //    else
                //    {
                //        var sr = "";
                //        XNamespace encodingStyle = "http://schemas.xmlsoap.org/soap/encoding/";
                //        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                //        XNamespace soapenc = "http://schemas.xmlsoap.org/soap/encoding/";
                //        XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                //        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                //        var a = XNamespace.Get("http://www.IVSource.com/IVACONT/Global");

                //        var xml = new XDocument(
                //            new XDeclaration("1.0", "utf-8", "yes"),
                //                new XElement(soap + "Envelop",
                //                    new XAttribute(soap + "encodingStyle", encodingStyle.NamespaceName),
                //                    new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                //                    new XAttribute(XNamespace.Xmlns + "soapenc", soapenc.NamespaceName),
                //                    new XAttribute(XNamespace.Xmlns + "xsd", xsd.NamespaceName),
                //                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                //                new XElement(soap + "Body",
                //                new XElement(a + "IVSource",
                //                new XElement(new XElement("Authorization", "User authorization failure"))))));

                //        foreach (var node in xml.Root.Descendants())
                //        {
                //            if (node.Name.NamespaceName == "")
                //            {
                //                node.Attributes("xmlns").Remove();
                //                node.Name = node.Parent.Name.Namespace + node.Name.LocalName;

                //            }
                //        }


                //        sr = xml.ToString();
                //        return Content(sr, "text/xml");
                //    }
            }*/





    }
}
public class VisaSearchRequestJson
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Countryiso { get; set; }
}