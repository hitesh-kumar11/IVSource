using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IVSource.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;
using IVSource.Controllers.Email;
using HtmlAgilityPack;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
//using AllPageLogout.Filter;

//------#1  By Nushrat to Apply filter isEnable addedd  'u.IsEnable==true &&' with existing code

namespace IVSource.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVSourceContext _context;
        private IHttpContextAccessor Accessor;
        private IConfiguration _iconfig;

        //public HomeC(IHttpContextAccessor _accessor)
        //{
        //    this.Accessor = _accessor;
        //}

        public HomeController(IVSourceContext context, IHttpContextAccessor _accessor, IConfiguration iconfig)
        {
            _context = context;
            this.Accessor = _accessor;
            _iconfig = iconfig;

        }
        public IActionResult Index()
        {
            HomePage model = new HomePage();
            model.Subscribe = new IvsVisaPagesForSubscribe();
            model.ContactUS = contactUs();
            model.Subscribe.SubscribeToUs = subscribeToUs();
            model.Subscribe.SubscribeToUsForm = new IvsVisaSubscribeToUs();
            model.News = new IvsVisaPagesForNews();
            model.News.pages = subPages();
            model.NewsList = subPagesList1();
            model.VisitorsArea = new IvsVisaPagesForVisitorsArea();
            model.VisitorsArea.visitorsArea = visitorsArea();
            model.VisitorsList = subPagesList2();
            model.Services = services();
            model.Partners = new IvsVisaPagesForPartners();
            model.Partners.partners = partners();
            model.PartnersList = subPagesList3();
            model.Logo = logo();
            model.AboutUs = aboutus();
            //model.MEAUsefulLinks = mea_useful_links();
            //model.EVisaIntoIndia = e_visa_into_india();
            //model.WhoHealthRulesWorldWide = who_health_rules_world_wide();
            //model.FlightsFromCom = flights_from_com();
            //model.FlightRadar = flight_radar();
            //model.PlanYourTravel = plan_your_travel();
            //model.ClickForPresentation = click_for_presentation();
            model.Links = links();
            model.Pages = pages();
            model.QuickContactInfo = quickcontactinfo();

            return View(model);
        }

        [HttpGet]
        public IActionResult Members()
        {
            HomePage obj = new HomePage();
            if (obj.VisitorsList != null)
            {
                return RedirectToAction("DiplomaticRepresentation", "IvsVisaCountriesDiplomaticRepr_User");
            }

            return RedirectToAction("Members", "Login");
            //return RedirectToAction("Members", "Home", oLogin);
        }
        public async Task<IActionResult> MemberLogout()
        {
            try
            {
                await HttpContext.SignOutAsync(MemberType.MemberUserAuth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Members");
        }
        public IActionResult CountryFactFinder()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Title.Contains("Country")).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult DiplomaticRepresentation()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Title.Contains("Diplomatic")).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult VisaNoteAndFees()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.VisaNoteFeesPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult VisaForm()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.VisaFormPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult Advisories()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.AdvisoriesPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult ReciprocalVisa()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.ReciprocalVisaPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult IndianMissionsOverseas()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.IndianMissionsOverseasPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult InternationalHelpAddresses()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.InternationalHelpAddressesPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        public IActionResult Holidays()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.HolidaysPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        //[CustemFilter]
        public IActionResult WorldFactBook()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.WorldFactBookPage).FirstOrDefault();
            if (obj.IsEnable == false)
            {
                obj.Title = null;
                obj.Description = null;
            }
            return View(obj);
        }
        //public IActionResult IndustryOpines()
        //{
        //    IvsVisaPages obj = new IvsVisaPages();
        //    obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.IndustryOpinesPage).FirstOrDefault();
        //    if (obj.IsEnable == false)
        //    {
        //        obj.Title = null;
        //        obj.Description = null;
        //    }
        //    return View(obj);
        //}
        //public IActionResult VThePeople()
        //{
        //    IvsVisaPages obj = new IvsVisaPages();
        //    obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.VThePeoplePage).FirstOrDefault();
        //    if (obj.IsEnable == false)
        //    {
        //        obj.Title = null;
        //        obj.Description = null;
        //    }
        //    return View(obj);
        //}
        //public IActionResult Music()
        //{
        //    IvsVisaPages obj = new IvsVisaPages();
        //    obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.MusicPage).FirstOrDefault();
        //    if (obj.IsEnable == false)
        //    {
        //        obj.Title = null;
        //        obj.Description = null;
        //    }
        //    return View(obj);
        //}


        [Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
        //[CustemFilter]
        public IActionResult SearchCountry()
        {
            var username = TempData["UserName"].ToString();
            return RedirectToAction("SearchHome", "IvsVisaCountries_User", new { pageType = "home", userName = username });
        }

        //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
        public IActionResult MemberPage(string page, string countryName, string countryFlag)
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;



            var country = (from count in _context.IvsVisaCountries
                           where count.CountryName.Contains(countryName) || count.Country.Contains(countryName)
                           select new IvsVisaCountries
                           {
                               CountryName = count.CountryName
                           }).FirstOrDefault();


            var coun = country.CountryName;


            if (!string.IsNullOrWhiteSpace(countryName))
            {
                switch (page)
                {
                    case "country_fact_find":
                        return RedirectToAction("CountryFactFinder", "IvsVisaCountriesDetails_User", new { countryName = countryName, countryFlag = countryFlag });
                    case "diplomatic_repr":
                        return RedirectToAction("DiplomaticRepresentation", "IvsVisaCountriesDiplomaticRepr_User", new { countryName = coun, countryFlag = countryFlag });
                    case "visa_note_and_fees":
                        return RedirectToAction("VisaNoteFees", "IvsVisaCategories_User", new { countryName = coun, countryFlag = countryFlag });
                    case "indian_mission_overseas":
                        return RedirectToAction("Indian_Mission_Overseas", "IvsVisaHelpAddresses_User", new { countryName = coun, countryFlag = countryFlag });
                    case "international_help":
                        return RedirectToAction("InternationalHelpAddress", "IvsVisaHelpAddresses_User", new { countryName = coun, countryFlag = countryFlag });
                    case "download_visa":
                        return RedirectToAction("DownloadVisaForm", "IvsVisaCategoriesForms_User", new { countryName = coun, countryFlag = countryFlag });
                    case "world_fact":
                        return RedirectToAction("WorldFactBook", "IvsVisaCountriesDetails_User", new { countryName = coun });
                    case "covid_information":
                        return RedirectToAction("CovidInformation", "IvsVisaCovidInformation_User", new { countryName = coun, countryFlag = countryFlag });
                    case "reciprocal_visa":
                        return RedirectToAction("ReciprocalVisa", "IvsVisaCountryAdvisories_User", new { countryName = coun, countryFlag = countryFlag });
                    case "3_advisories":
                        return RedirectToAction("ThreeAdvisory", "IvsVisaCountryAdvisories_User", new { countryName = coun, countryFlag = countryFlag });
                        //Added by Hitesh on 29092023
                    case "check_visa_requirements":
                        return RedirectToAction("CheckVisaRequirements", "IvsVisaCategoriesForms_User", new { countryName = coun, countryFlag = countryFlag });
                }
            }
            //else
            //{
            //    switch (page)
            //    {
            //        case "country_fact_find":
            //            return View("~/Views/IvsVisaCountriesDetails_User/CountryDetails.cshtml");
            //        case "diplomatic_repr":
            //            return View("~/Views/IvsVisaCountriesDiplomaticRepr_User/DiplomaticRepresentations.cshtml");
            //        case "visa_note_and_fees":
            //            return View("~/Views/IvsVisaCategories_User/VisaNoteAndFees.cshtml");
            //        case "indian_mission_overseas":
            //            return View("~/Views/IvsVisaHelpAddresses_User/IndianMissionOverseas.cshtml");
            //        case "international_help":
            //            return View("~/Views/IvsVisaHelpAddresses_User/InternationalHelpAddress.cshtml");
            //        case "download_visa":
            //            return View("~/Views/IvsVisaCategoriesForms/DownloadVisaFormrr.cshtml");
            //        case "covid_information":
            //            return View("~/Views/IvsVisaCovidInformation_User/CovidInformationrr.cshtml");
            //        case "reciprocal_visa":
            //            return View("~/Views/IvsVisaCountryAdvisories_User/RreciprocalVisa.cshtml");
            //        case "3_advisories":
            //            return View("~/Views/IvsVisaCountryAdvisories_User/Advisories.cshtml");

            //    }
            //}


            return RedirectToAction("SearchHome", "IvsVisaCountries_User", new { pageType = "home" });
        }

        public IActionResult MemberPageCountries(string page, string countryName, string countryFlag)
        {
            var country = (from count in _context.IvsVisaCountries
                           where count.CountryName.Contains(countryName)
                           select new IvsVisaCountries
                           {
                               CountryName = count.CountryName
                           }).FirstOrDefault();

            var coun = country.CountryName;

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                switch (page)
                {
                    case "country_fact_find":
                        return RedirectToAction("CountryFactFinderr", "IvsVisaCountriesDetails_User", new { countryName = countryName, countryFlag = countryFlag });
                    case "diplomatic_repr":
                        return RedirectToAction("DiplomaticRepresentationr", "IvsVisaCountriesDiplomaticRepr_User", new { countryName = coun, countryFlag = countryFlag });
                    case "visa_note_and_fees":
                        return RedirectToAction("VisaNoteFeesr", "IvsVisaCategories_User", new { countryName = coun, countryFlag = countryFlag });
                    case "indian_mission_overseas":
                        return RedirectToAction("Indian_Mission_Overseasr", "IvsVisaHelpAddresses_User", new { countryName = coun, countryFlag = countryFlag });
                    case "international_help":
                        return RedirectToAction("InternationalHelpAddressr", "IvsVisaHelpAddresses_User", new { countryName = coun, countryFlag = countryFlag });
                    case "download_visa":
                        return RedirectToAction("DownloadVisaFormr", "IvsVisaCategoriesForms_User", new { countryName = coun, countryFlag = countryFlag });
                    case "world_fact":
                        return RedirectToAction("WorldFactBookr", "IvsVisaCountriesDetails_User", new { countryName = coun });
                    case "covid_information":
                        return RedirectToAction("CovidInformationr", "IvsVisaCovidInformation_User", new { countryName = coun, countryFlag = countryFlag });
                    case "reciprocal_visa":
                        return RedirectToAction("ReciprocalVisar", "IvsVisaCountryAdvisories_User", new { countryName = coun, countryFlag = countryFlag });
                    case "3_advisories":
                        return RedirectToAction("ThreeAdvisoryr", "IvsVisaCountryAdvisories_User", new { countryName = coun, countryFlag = countryFlag });

                }
            }
            return RedirectToAction("SearchHome", "IvsVisaCountries_User", new { pageType = "home" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [HttpPost]
        public string SendEmailSubscribeToUs222(string ccEmail, string subject, string Years, string CaptchaCode, IvsVisaSubscribeToUs obj)
        {
            string retVal = string.Empty;

            try
            {
                retVal = "PLEASE FILL THE REQUIRED FIELD.";

                if (ModelState.IsValid)
                {
                    if (!Captcha.ValidateCaptchaCode(CaptchaCode, HttpContext))
                    {
                        retVal = "Entered code is incorrect";
                        return retVal;
                    }
                    else
                    {
                        if (obj.D_D_Number == null || obj.Date == null || obj.Amount == null || obj.Remarks == null)
                        {
                            obj.D_D_Number = "";
                            obj.Date = Convert.ToDateTime("");
                            obj.Amount = Convert.ToInt32("");
                            obj.Remarks = "";
                        }

                        DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("", typeof(string)),
                        new DataColumn("", typeof(string))
                        });

                        dt.Rows.Add("Name                             :", obj.Name);
                        dt.Rows.Add("Designation                      :", obj.Designation);
                        dt.Rows.Add("Company                          :", obj.Company);
                        dt.Rows.Add("Full Address                     :", obj.Full_Address);
                        dt.Rows.Add("Phone Number                     :", obj.Phone_Number);
                        dt.Rows.Add("Email Id                         :", obj.Email_Address);
                        dt.Rows.Add("DD/ Online Transaction Number    :", obj.D_D_Number);
                        dt.Rows.Add("DD Date                          :", obj.Date.Value.ToShortDateString());
                        dt.Rows.Add("DD Amount                        :", obj.Amount);
                        dt.Rows.Add("Feedback/Query                   :", obj.Remarks);

                        StringBuilder sb = new StringBuilder();

                        sb.Append(Years);

                        sb.Append("<br><br><table cellpadding='5' cellspacing='0' style='font-size: 8pt; font-family: Arial'>");

                        sb.Append("<tr>");

                        sb.Append("</tr>");

                        foreach (DataRow row in dt.Rows)
                        {
                            sb.Append("<tr>");

                            foreach (DataColumn column in dt.Columns)
                            {
                                sb.Append("<td style='width: 300px;'>" + row[column.ColumnName].ToString() + "</td>");
                            }

                            sb.Append("</tr>");
                        }

                        sb.Append("</table>");

                        sb.Append("<br/><br/>Thanks & Regards, <br>IVSource Team");

                        EMail em = new EMail();

                        em.SendEmail(sb.ToString(), subject, "nushrat.ali@itq.in", ccEmail, false);

                        retVal = "Mail Sent successfully";

                        return retVal;
                    }
                }
                else
                {
                    return retVal;
                }
            }
            catch
            {
                return retVal;
            }
        }


        [HttpPost]
        public string SendEmailSubscribeToUs(string ccEmail, string subject, string Years, string CaptchaCode, IvsVisaSubscribeToUs obj)
        {
            //Added validation by Hitesh on 06062023 
            string retVal = string.Empty;
            string emailTo = _iconfig.GetValue<string>("EmailParam:CCEmail");
            bool emailenabled = _iconfig.GetValue<bool>("EmailParam:EmailEnabled");
            bool resValidation = false;
            try
            {
                retVal = "PLEASE FILL THE REQUIRED FIELD.";

                if (retVal != "")
                {
                    if (!Captcha.ValidateCaptchaCode(CaptchaCode, HttpContext))
                    {
                        retVal = "Entered code is incorrect";
                        return retVal;
                    }
                    else
                    {
                        if (obj.D_D_Number == null)
                        {
                            resValidation = true;
                            obj.D_D_Number = "";
                        }
                        if (obj.Date == null)
                        {
                            resValidation = true;
                            string datetime = DateTime.Now.ToString();
                            obj.Date = Convert.ToDateTime(datetime);
                        }
                        if (obj.Amount == null)
                        {
                            resValidation = true;
                            int Amount = 0;
                            obj.Amount = Convert.ToInt32(Amount);
                        }
                        if (obj.Remarks == null)
                        {
                            obj.Remarks = "";
                        }
                        bool res = UserRegist(ccEmail, subject, Years, CaptchaCode, obj);
                        if (res)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("", typeof(string)),
                        new DataColumn("", typeof(string))
                        });

                            if (resValidation)
                            {
                                dt.Rows.Add("Name                             :", obj.Name);
                                dt.Rows.Add("Designation                      :", obj.Designation);
                                dt.Rows.Add("Company                          :", obj.Company);
                                dt.Rows.Add("Full Address                     :", obj.Full_Address);
                                dt.Rows.Add("Phone Number                     :", obj.Phone_Number);
                                dt.Rows.Add("Email Id                         :", obj.Email_Address);
                                //dt.Rows.Add("DD/ Online Transaction Number    :", obj.D_D_Number);
                                //dt.Rows.Add("DD Date                          :", obj.Date.Value.ToShortDateString());
                                //dt.Rows.Add("DD Amount                        :", obj.Amount);
                                dt.Rows.Add("Feedback/Query                   :", obj.Remarks);
                            }
                            else
                            {
                                dt.Rows.Add("Name                             :", obj.Name);
                                dt.Rows.Add("Designation                      :", obj.Designation);
                                dt.Rows.Add("Company                          :", obj.Company);
                                dt.Rows.Add("Full Address                     :", obj.Full_Address);
                                dt.Rows.Add("Phone Number                     :", obj.Phone_Number);
                                dt.Rows.Add("Email Id                         :", obj.Email_Address);
                                dt.Rows.Add("DD/ Online Transaction Number    :", obj.D_D_Number);
                                dt.Rows.Add("DD Date                          :", obj.Date.Value.ToShortDateString());
                                dt.Rows.Add("DD Amount                        :", obj.Amount);
                                dt.Rows.Add("Feedback/Query                   :", obj.Remarks);
                            }

                            StringBuilder sb = new StringBuilder();

                            sb.Append(Years);

                            sb.Append("<br><br><table cellpadding='5' cellspacing='0' style='font-size: 8pt; font-family: Arial'>");

                            sb.Append("<tr>");

                            sb.Append("</tr>");

                            foreach (DataRow row in dt.Rows)
                            {
                                sb.Append("<tr>");

                                foreach (DataColumn column in dt.Columns)
                                {
                                    sb.Append("<td style='width: 300px;'>" + row[column.ColumnName].ToString() + "</td>");
                                }

                                sb.Append("</tr>");
                            }

                            sb.Append("</table>");

                            sb.Append("<br/><br/>Thanks & Regards, <br>IVSource Team");

                            EMail em = new EMail();
                            ccEmail = "";// obj.Email_Address;
                            if (emailenabled)
                            {
                                if (!string.IsNullOrEmpty(emailTo))
                                {
                                    em.SendEmail(sb.ToString(), subject, emailTo, ccEmail, false);
                                }
                                else
                                {
                                    em.SendEmail(sb.ToString(), subject, "hitesh.kumar@itq.in", ccEmail, false);
                                }
                            }                            

                            retVal = "Mail Sent successfully";

                            return retVal;
                        }
                        else
                        {
                            retVal = "MailBox Unavailable. Please enter valid email.";
                            return retVal;
                        }
                    }
                }
                else
                {
                    return retVal;
                }
            }
            catch
            {
                return retVal;
            }
        }
        public bool UserRegist(string ccEmail, string subject, string Years, string CaptchaCode, IvsVisaSubscribeToUs obj)
        {
            bool res = false;
            var oUserinfo = new IvsUserDetails();
            string num = Convert.ToString(GenerateRandomNo());
            if (obj != null)
            {
                oUserinfo.UserId = obj.Name.Trim() + "@" + num;
                oUserinfo.UserName = obj.Name.Trim() + "@" + num;
                oUserinfo.Password = num;
                oUserinfo.UserType = "User";
                //oUserinfo.CorporateId = "dasdas";
                oUserinfo.Name = obj.Name;
                oUserinfo.Designation = obj.Designation;
                oUserinfo.Company = obj.Company;
                //oUserinfo.City = "city";
                oUserinfo.Country = obj.bCountryIso;
                oUserinfo.Address = obj.Full_Address;
                oUserinfo.Phone = Convert.ToString(obj.Phone_Number);
                //oUserinfo.Fax = "Fax";
                oUserinfo.Email = obj.Email_Address;
                oUserinfo.AdditionalEmail = obj.Email_Address;
                oUserinfo.TerminalIdNo = "0";
                oUserinfo.CorporateId = "NewRegistration";
                oUserinfo.IsEnable = 0;
                oUserinfo.CreatedDate = DateTime.Now;
                oUserinfo.ModifiedDate = null;
                oUserinfo.ResetPasswordOtp = "NA";
                oUserinfo.ResetPasswordOtpexpireOn = DateTime.Now;
                //oUserinfo.GST
                //oUserinfo.DD
                //oUserinfo.DDDate = obj.bCountryIso;
                //oUserinfo.DD Amount
                //oUserinfo.remark
                //oUserinfo.City = obj.b;

                _context.IvsUserDetails.Add(oUserinfo);
                _context.SaveChanges();
                //customResponse.errorMsg = "User registraton process is completed and concern person contact you soon.";
                //customResponse.status = "success";
                res = true;
            }
            return res;
        }


        public IvsVisaPages contactUs()
        {
            IvsVisaPages obj = new IvsVisaPages();
            //obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.ContactPage && ).FirstOrDefault(); //----------Added by Nushrat
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.ContactPage).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                    obj.Description = null;
                }
            }
            return obj;
        }
        public IvsVisaPages subscribeToUs()
        {
            IvsVisaPages obj = new IvsVisaPages();
            //obj =  _context.IvsVisaPages.Where(u => u.Type == HomePageType.SubscribePage).FirstOrDefault();//----------Added by Nushrat
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.SubscribePage).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                    obj.Description = null;
                }
            }
            return obj;
        }

        public IvsVisaPages subPages()
        {
            IvsVisaPages obj = new IvsVisaPages();
            //obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.NewsPage).FirstOrDefault();//----------Added by Nushrat
            obj = _context.IvsVisaPages.Where(u => (u.Type == HomePageType.NewsPage) && (u.IsEnable == true)).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                    obj.Description = null;
                }
            }
            return obj;
        }

        public List<IvsVisaSubPages> subPagesList1()
        {
            IvsVisaPagesForNews obj = new IvsVisaPagesForNews();

            var subPages = (from info in _context.IvsVisaSubPages
                            where info.PageId == 11 && info.IsEnable == true
                            orderby info.Id descending
                            select new IvsVisaSubPages
                            {
                                Id = info.Id,
                                Title = info.Title,
                                Description = a(info.Description),
                                Image = info.Image,
                                IsEnable = info.IsEnable
                            }).ToList().Take(3);


            return subPages.ToList();
        }

        public string a(string subPages)
        {

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(subPages.ToString());
            string result = htmlDoc.DocumentNode.InnerText.Replace("&nbsp;", " ");
            return result;
        }

        public IvsVisaPages visitorsArea()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.VisitorAreaPage).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                    obj.Description = null;
                }
            }
            return obj;
        }

        public List<IvsVisaSubPages> subPagesList2()
        {
            IvsVisaPagesForVisitorsArea obj = new IvsVisaPagesForVisitorsArea();

            var subPages = from info in _context.IvsVisaSubPages
                           where info.PageId == 12
                           && info.IsEnable == true      
                           orderby info.CreatedDate descending
                           select new IvsVisaSubPages
                           {
                               Id = info.Id,
                               Title = info.Title,
                               Image = info.Image,
                               IsEnable = info.IsEnable
                           };

            return subPages.ToList();
        }

        public IvsVisaPages services()
        {
            //var services = _context.IvsVisaPages.Where(u => u.Type == HomePageType.ServicePage).FirstOrDefault(); //------Added by Nushrat
            var services = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.IsEnable == true && u.Type == HomePageType.ServicePage).FirstOrDefault();
            if (services == null) // Added by Nushrat to show no content if any page get disabled.
            {
                services = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (services.IsEnable == false)
                {
                    services.Title = null;
                    services.Description = null;
                }
            }
            return services;
        }

        //public IvsVisaPages mea_useful_links()
        //{
        //    var mea_useful_links = _context.IvsVisaPages.Where(u => u.Type == HomePageType.MEAUsefulLinksPage).FirstOrDefault();
        //    if (mea_useful_links.IsEnable == false)
        //    {
        //        mea_useful_links.Title = null;
        //        mea_useful_links.Description = null;
        //    }
        //    return mea_useful_links;
        //}

        //public IvsVisaPages e_visa_into_india()
        //{
        //    var e_visa_into_india = _context.IvsVisaPages.Where(u => u.Type == HomePageType.EVisaIntoIndiaPage).FirstOrDefault();
        //    if (e_visa_into_india.IsEnable == false)
        //    {
        //        e_visa_into_india.Title = null;
        //        e_visa_into_india.Description = null;
        //    }
        //    return e_visa_into_india;
        //}

        //public IvsVisaPages who_health_rules_world_wide()
        //{
        //    var who_health_rules_world_wide = _context.IvsVisaPages.Where(u => u.Type == HomePageType.WhoHealthRulesWorldWidePage).FirstOrDefault();
        //    if (who_health_rules_world_wide.IsEnable == false)
        //    {
        //        who_health_rules_world_wide.Title = null;
        //        who_health_rules_world_wide.Description = null;
        //    }
        //    return who_health_rules_world_wide;
        //}

        //public IvsVisaPages flights_from_com()
        //{
        //    var flights_from_com = _context.IvsVisaPages.Where(u => u.Type == HomePageType.FlightsFromComPage).FirstOrDefault();
        //    if (flights_from_com.IsEnable == false)
        //    {
        //        flights_from_com.Title = null;
        //        flights_from_com.Description = null;
        //    }
        //    return flights_from_com;
        //}

        //public IvsVisaPages flight_radar()
        //{
        //    var flight_radar = _context.IvsVisaPages.Where(u => u.Type == HomePageType.FlightRadarPage).FirstOrDefault();
        //    if (flight_radar.IsEnable == false)
        //    {
        //        flight_radar.Title = null;
        //        flight_radar.Description = null;
        //    }
        //    return flight_radar;
        //}

        //public IvsVisaPages plan_your_travel()
        //{
        //    var plan_your_travel = _context.IvsVisaPages.Where(u => u.Type == HomePageType.PlanYourTravelPage).FirstOrDefault();
        //    if (plan_your_travel.IsEnable == false)
        //    {
        //        plan_your_travel.Title = null;
        //        plan_your_travel.Description = null;
        //    }
        //    return plan_your_travel;
        //}

        //public IvsVisaPages click_for_presentation()
        //{
        //    var click_for_presentation = _context.IvsVisaPages.Where(u => u.Type == HomePageType.ClickForPresentationPage).FirstOrDefault();
        //    if (click_for_presentation.IsEnable == false)
        //    {
        //        click_for_presentation.Title = null;
        //        click_for_presentation.Description = null;
        //    }
        //    return click_for_presentation;
        //}

        public IvsVisaPagesLinks links()
        {
            var subPages = from info in _context.IvsVisaPages
                           where info.Type == "LINKS"
                           && info.IsEnable == true       //------Added by Nushrat
                           orderby info.CreatedDate descending
                           select new IvsVisaPages
                           {
                               Id = info.Id,
                               Title = info.Title,
                               Description = info.Description,
                               Image = info.Image,
                               IsEnable = info.IsEnable
                           };

            IvsVisaPagesLinks model = new IvsVisaPagesLinks();
            model.Linkr = subPages.ToList();
            return model;
        }

        public IvsVisaPagesOurContent pages()
        {
            var pages = from info in _context.IvsVisaPages
                        where info.Type == "PAGES"
                        && info.IsEnable == true       //------Added by Nushrat
                        orderby info.CreatedDate descending
                        select new IvsVisaPages
                        {
                            Id = info.Id,
                            Title = info.Title,
                            Description = info.Description,
                            Image = info.Image,
                            IsEnable = info.IsEnable
                        };
            IvsVisaPagesOurContent model = new IvsVisaPagesOurContent();
            model.Pager = pages.ToList();
            return model;
        }

        public List<IvsVisaQuickContactInfo> quickcontactinfo()
        {
            List<IvsVisaQuickContactInfo> obj = new List<IvsVisaQuickContactInfo>();
            obj = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.QuickContactInfo = obj;
            return obj;
        }

        public IActionResult PagesOurContent(string title)
        {
            var obj = from info in _context.IvsVisaPages
                      where info.Type == HomePageType.Pages && info.Title == title
                      && info.IsEnable == true       //------Added by Nushrat
                      select new IvsVisaPages
                      {
                          Id = info.Id,
                          Title = info.Title,
                          Description = info.Description,
                          Image = info.Image,
                          IsEnable = info.IsEnable
                      };

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            List<HomePage> allList = new List<HomePage>();

            HomePage model = new HomePage();
            model.PagesOurContent = obj.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;
            allList.Add(model);
            return View(model);
        }

        public IvsVisaPages partners()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.PartnersPage).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                    obj.Description = null;
                }
            }
            return obj;
        }

        public List<IvsVisaSubPages> subPagesList3()
        {
            IvsVisaPagesForPartners obj = new IvsVisaPagesForPartners();

            var subPages = from info in _context.IvsVisaSubPages
                           where info.PageId == 14
                           //&& info.IsEnable == true       //------Added by Nushrat
                           orderby info.CreatedDate descending
                           select new IvsVisaSubPages
                           {
                               Id = info.Id,
                               Title = info.Title,
                               Image = info.Image,
                               IsEnable = info.IsEnable
                           };
            return subPages.ToList();
        }

        public IvsVisaPages logo()
        {
            IvsVisaPages obj = new IvsVisaPages();
            obj = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.LogoPage).FirstOrDefault();
            if (obj == null) // Added by Nushrat to show no content if any page get disabled.
            {
                obj = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (obj.IsEnable == false)
                {
                    obj.Title = null;
                }
            }
            return obj;
        }

        public IvsVisaPages aboutus()
        {
            var aboutus = _context.IvsVisaPages.Where(u => u.IsEnable == true && u.Type == HomePageType.AboutUsPage).FirstOrDefault();
            if (aboutus == null) // Added by Nushrat to show no content if any page get disabled.
            {
                aboutus = _context.IvsVisaPages.Where(u => u.Type == HomePageType.Default).FirstOrDefault();
            }
            else
            {
                if (aboutus.IsEnable == false)
                {
                    aboutus.Title = "";
                    aboutus.Description = "";
                }
            }
            return aboutus;
        }

        public async Task<IActionResult> Logout()
        {
            try
            {

                //----------------------Fetch the Cookie value using its Key.----------------------#1----------------------------------------------------<
                string stRTID = this.Accessor.HttpContext.Request.Cookies["TerminalID"];
                TempData["TerminalID"] = stRTID != null ? stRTID : "undefined";
                string TID = TempData["TerminalID"].ToString();
                //------------------------------------------------------------------------------------#1-------------------------------------------------->

                //var upIvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.TerminalId == login.TerminalId).FirstOrDefault();
                var upIvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.TerminalId == TID).FirstOrDefault();
                upIvsVisaTerminalId.IsUsed = 0;
                _context.IvsUserTerminalId.Update(upIvsVisaTerminalId);
                await _context.SaveChangesAsync();
                return RedirectToAction("Members", "Login");
                //await HttpContext.SignOutAsync(MemberType.MemberUserAuth);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return RedirectToAction("Index");
        }

        public IActionResult WriteCookie(string name)
        {
            //Set the Expiry date of the Cookie.
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);

            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("TerminalID", name, option);

            return RedirectToAction("Index");
        }
        public IActionResult ReadCookie()
        {
            //Fetch the Cookie value using its Key.
            string name = this.Accessor.HttpContext.Request.Cookies["TerminalID"];

            TempData["TerminalID"] = name != null ? name : "undefined";

            return RedirectToAction("Index");
        }

        //Generate RandomNo
        public int GenerateRandomNo()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
    }
}
