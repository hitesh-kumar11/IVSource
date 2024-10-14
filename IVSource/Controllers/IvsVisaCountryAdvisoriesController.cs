using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using HtmlAgilityPack;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountryAdvisoriesController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountryAdvisoriesController(IVSourceContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            //var allList = _context.IvsVisaCountryAdvisories.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());

            var allList = from country in _context.IvsVisaCountryAdvisories
                          where country.CountryIso == TempData.Peek("CountryIso").ToString()
                          select new IvsVisaCountryAdvisories
                          {
                              Advisory = a(country.Advisory),
                              AdvisoryId = country.AdvisoryId,
                              AdvisoryType = country.AdvisoryType,
                          };


            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => (p.Advisory.Contains(filter) || p.AdvisoryType.Contains(filter))).OrderByDescending(y => y.SerialNum);

                string advisoryType = null;
                if (("reciprocal visa").Contains(filter.ToLower()))
                    advisoryType = "RECVIS";
                else if ("international advisory".Contains(filter.ToLower()))
                    advisoryType = "INTAD";
                else if ("IVSource advisory".Contains(filter.ToLower()))
                    advisoryType = "IVSAD";

                allList = allList.Where(p => p.Advisory.Contains(filter) || (p.AdvisoryType == advisoryType)).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;
          // Edit by Nushrat
          // var model = await PagingList<IvsVisaCountryAdvisories>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCountryAdvisories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountryAdvisories = await _context.IvsVisaCountryAdvisories.FindAsync(id);
            if (ivsVisaCountryAdvisories == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountryAdvisories);
        }

        // POST: IvsVisaCountryAdvisories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,AdvisoryId,CountryIso,Advisory,AdvisoryType,CreatedDate,ModifiedDate")] IvsVisaCountryAdvisories ivsVisaCountryAdvisories)
        {
            if (id != ivsVisaCountryAdvisories.AdvisoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCountryAdvisories.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountryAdvisories);
                    _context.Entry(ivsVisaCountryAdvisories).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCountryAdvisories).Property(x => x.AdvisoryId).IsModified = false;
                    _context.Entry(ivsVisaCountryAdvisories).Property(x => x.CountryIso).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountryAdvisoriesExists(ivsVisaCountryAdvisories.AdvisoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(ivsVisaCountryAdvisories);
        }

        private bool IvsVisaCountryAdvisoriesExists(string id)
        {
            return _context.IvsVisaCountryAdvisories.Any(e => e.AdvisoryId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCountryAdvisories obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);
            var aid = ran;

            var advType = obj.AdvisoryType.Substring(0, 3);

            obj.CountryIso = countryIso.CountryIso;
            obj.AdvisoryId = advType + aid;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;

            _context.IvsVisaCountryAdvisories.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountryAdvisories/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string a(string subPages)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(subPages.ToString());
            string result = htmlDoc.DocumentNode.InnerText.Replace(" ", " ");
            return result;
        }
    }

    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCountryAdvisories_UserController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountryAdvisories_UserController(IVSourceContext context)
        {
            _context = context;
        }

        //
        [CustemFilter]
        public IActionResult ReciprocalVisa(string countryName, string countryFlag)
        {
            var countryIso = (from coun in _context.IvsVisaCountries where coun.CountryName.Contains(countryName) select coun.CountryIso).FirstOrDefault();

            var checkReciprocalVisaData = (from reci in _context.IvsVisaCountryAdvisories select reci.CountryIso).Contains(countryIso);

            var checkReciVisaData = (from recivisa in _context.IvsVisaCountryAdvisories
                                     where recivisa.CountryIso == countryIso
                                     select recivisa.AdvisoryType).Contains("RECVIS");
            //var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate); //------Added by Nushrat
            //var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate); //------Added by Nushrat

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkReciprocalVisaData == true && checkReciVisaData == true)
            {
                var countryDetail = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                    where country.CountryName == countryName && us.AdvisoryType == "RECVIS"
                                    select new IvsVisaCountryAdvisories
                                    {
                                        Advisory = HttpUtility.HtmlDecode(us.Advisory),
                                        bCountryFlag = countryFlag,
                                        bCountryName = countryName
                                    };

                HomePage model = new HomePage();
                model.reciprocalvisa = countryDetail.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);

                //var model = countryDetail.ToList();
                return View("ReciprocalVisa", allData);
            }
            else
            {
                IvsVisaCountryAdvisories obj = new IvsVisaCountryAdvisories();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                return View("RreciprocalVisa", obj);
            }
        }

        
        public IActionResult ReciprocalVisar(string countryName, string countryFlag)
        {
            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var countryDetail = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                    where country.CountryName == countryName && us.AdvisoryType == "RECVIS"
                                    select new IvsVisaCountryAdvisories
                                    {
                                        Advisory = HttpUtility.HtmlDecode(us.Advisory),
                                        bCountryFlag = countryFlag,
                                        bCountryName = countryName
                                    };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage model = new HomePage();
                model.reciprocalvisa = countryDetail.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                ViewBag.PagesOurContent = listPages;
                ViewBag.LinksOurContent = listLinks;
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);

                //var model = countryDetail.ToList();
                return View("ReciprocalVisar", allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to register / login here.";
                return RedirectToAction("Members", "Login");
            }
        }
        [CustemFilter]
        public IActionResult ThreeAdvisory(string countryName, string countryFlag)
        {
            var ids = _context.IvsVisaCountries.Where(x => x.CountryName == countryName).Select(x => x.CountryIso);
            string ISOdata = ids.FirstOrDefault().ToString();

            var checkThreeAdvisoryData = (from recivisa in _context.IvsVisaCountryAdvisories
                                          where recivisa.AdvisoryType == "IVSAD" || recivisa.AdvisoryType == "INTAD"
                                          select recivisa.CountryIso).Contains(ISOdata);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkThreeAdvisoryData == true)
            {
                var advisoryData = from country in _context.IvsVisaCountries
                                   join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                   where country.CountryName == countryName
                                   select new IvsVisaCountryAdvisories
                                   {
                                       Advisory = HttpUtility.HtmlDecode(us.Advisory),
                                       AdvisoryType = us.AdvisoryType,
                                       bCountryName = countryName,
                                       bCountryFlag = countryFlag,
                                   };
                var saarcDetails = from country in _context.IvsVisaCountries
                                   join us in _context.IvsVisaSaarcDetails on country.CountryIso equals us.CountryId
                                   where us.CountryIso == ISOdata //&& us.IsEnable == 1
                                   select new IvsVisaSaarcDetails
                                   {
                                       CountryIso = us.CountryId,
                                       CountryName = country.CountryName,
                                       IsVisaRequired = us.IsVisaRequired,
                                       VisaApplyWhere = us.VisaApplyWhere,
                                       VisaOfficeName = us.VisaOfficeName,
                                       VisaOfficeAddress = us.VisaOfficeAddress,
                                       VisaOfficeCity = us.VisaOfficeCity,
                                       VisaOfficeCountry = us.VisaOfficeCountry,
                                       VisaOfficeTelephone = us.VisaOfficeTelephone,
                                       VisaOfficeFax = us.VisaOfficeFax,
                                       bCountryName = countryName,
                                       bCountryFlag = countryFlag,
                                       IsEnable = us.IsEnable
                                   };

                HomePage obj = new HomePage();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                obj.threeadvisories = advisoryData.ToList();
                obj.saarcDetails = saarcDetails.ToList();
                obj.PagesOurContentPagesList = listPages.ToList();
                obj.PagesOurContentLinksList = listLinks.ToList();

                List<HomePage> allData = new List<HomePage>();
                allData.Add(obj);
                return View("ThreeAdvisory", allData);
            }
            else
            {
                IvsVisaCountryAdvisories model = new IvsVisaCountryAdvisories();
                model.bCountryFlag = countryFlag;
                model.bCountryName = countryName;
                return View("Advisories", model);
            }
        }
        
        public IActionResult ThreeAdvisoryr(string countryName, string countryFlag)
        {
            var ids = _context.IvsVisaCountries.Where(x => x.CountryName.Contains(countryName)).Select(x => x.CountryIso);
            string ISOdata = ids.FirstOrDefault().ToString();

            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var advisoryData = from country in _context.IvsVisaCountries
                                   join us in _context.IvsVisaCountryAdvisories on country.CountryIso equals us.CountryIso
                                   where country.CountryName == countryName
                                   select new IvsVisaCountryAdvisories
                                   {
                                       Advisory = HttpUtility.HtmlDecode(us.Advisory),
                                       AdvisoryType = us.AdvisoryType,
                                       bCountryName = countryName,
                                       bCountryFlag = countryFlag,
                                   };
                var saarcDetails = from country in _context.IvsVisaCountries
                                   join us in _context.IvsVisaSaarcDetails on country.CountryIso equals us.CountryId
                                   where us.CountryIso == ISOdata //&& us.IsEnable == 1
                                   select new IvsVisaSaarcDetails
                                   {
                                       CountryIso = us.CountryId,
                                       CountryName = country.CountryName,
                                       IsVisaRequired = us.IsVisaRequired,
                                       VisaApplyWhere = us.VisaApplyWhere,
                                       VisaOfficeName = us.VisaOfficeName,
                                       VisaOfficeAddress = us.VisaOfficeAddress,
                                       VisaOfficeCity = us.VisaOfficeCity,
                                       VisaOfficeCountry = us.VisaOfficeCountry,
                                       VisaOfficeTelephone = us.VisaOfficeTelephone,
                                       VisaOfficeFax = us.VisaOfficeFax,
                                       bCountryName = countryName,
                                       bCountryFlag = countryFlag,
                                       IsEnable = us.IsEnable
                                   };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage obj = new HomePage();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                obj.threeadvisories = advisoryData.ToList();
                obj.saarcDetails = saarcDetails.ToList();
                obj.PagesOurContentPagesList = listPages.ToList();
                obj.PagesOurContentLinksList = listLinks.ToList();
                ViewBag.PagesOurContent = listPages.ToList();
                ViewBag.LinksOurContent = listLinks.ToList();
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(obj);
                return View("ThreeAdvisoryr", allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to reister / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }
    }
}
