using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    //[Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountriesDiplomaticRepresentationsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountriesDiplomaticRepresentationsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCountriesDiplomaticRepresentations
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaCountriesDiplomaticRepresentation.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.OfficeName.Contains(filter) || p.OfficeCity.Contains(filter) || p.OfficeCountry.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaCountriesDiplomaticRepresentation>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCountriesDiplomaticRepresentations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountriesDiplomaticRepresentation = await _context.IvsVisaCountriesDiplomaticRepresentation.FindAsync(id);
            if (ivsVisaCountriesDiplomaticRepresentation == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountriesDiplomaticRepresentation);
        }

        // POST: IvsVisaCountriesDiplomaticRepresentations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryIso,OfficeId,OfficeName,OfficeAddress,OfficeCity,OfficePincode,OfficeCountry,OfficePhone,OfficeTelephoneVisa,OfficeFax,OfficeTimings,OfficeVisaTimings,OfficePublicTimings,OfficeCollectionTimings,OfficeEmail,OfficeWebsite,OfficeNotes,TerritoryJurisdiction,IsEnable,Priority,CreatedDate,ModifiedDate")] IvsVisaCountriesDiplomaticRepresentation ivsVisaCountriesDiplomaticRepresentation)
        {
            if (id != ivsVisaCountriesDiplomaticRepresentation.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCountriesDiplomaticRepresentation.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountriesDiplomaticRepresentation);
                    _context.Entry(ivsVisaCountriesDiplomaticRepresentation).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCountriesDiplomaticRepresentation).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCountriesDiplomaticRepresentation).Property(x => x.OfficeId).IsModified = false;
                    _context.Entry(ivsVisaCountriesDiplomaticRepresentation).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;

                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountriesDiplomaticRepresentationExists(ivsVisaCountriesDiplomaticRepresentation.SerialNum))
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
            return View(ivsVisaCountriesDiplomaticRepresentation);
        }

        private bool IvsVisaCountriesDiplomaticRepresentationExists(int id)
        {
            return _context.IvsVisaCountriesDiplomaticRepresentation.Any(e => e.SerialNum == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCountriesDiplomaticRepresentation obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);

            var oid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.OfficeId = countryIso.CountryIso + oid;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaCountriesDiplomaticRepresentation.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountriesDiplomaticRepresentations/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCountriesDiplomaticRepr_UserController : Controller
    {
        private readonly IVSourceContext _context;
        public IConfiguration _Configuration { get; }

        public IvsVisaCountriesDiplomaticRepr_UserController(IVSourceContext context, IConfiguration iconfiguration)
        {
            _context = context;
            _Configuration = iconfiguration;
        }
        [CustemFilter]
        public IActionResult DiplomaticRepresentation(string countryName, string countryFlag)
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();

            var checkDiplomaticRepresentationData = (from dipl in _context.IvsVisaCountriesDiplomaticRepresentation select dipl.CountryIso).Contains(countryIso);

            if (checkDiplomaticRepresentationData == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCountriesDiplomaticRepresentation on country.CountryIso equals us.CountryIso
                                     where country.CountryName.Contains(countryName) //&& us.IsEnable == 1
                                     orderby us.Priority
                                     select new IvsVisaCountriesDiplomaticRepresentation
                                     {
                                         OfficeName = us.OfficeName,
                                         OfficeAddress = us.OfficeAddress,
                                         OfficeCity = us.OfficeCity,
                                         OfficeCountry = us.OfficeCountry,
                                         OfficePhone = us.OfficePhone,
                                         OfficeFax = us.OfficeFax,
                                         OfficeTimings = us.OfficeTimings,
                                         OfficeVisaTimings = us.OfficeVisaTimings,
                                         OfficeCollectionTimings = us.OfficeCollectionTimings,
                                         OfficeEmail = us.OfficeEmail,
                                         OfficeNotes = us.OfficeNotes,
                                         TerritoryJurisdiction = us.TerritoryJurisdiction,
                                         bCountryFlag = countryFlag,
                                         bCountryName = countryName,
                                         IsEnable = us.IsEnable
                                     };

                HomePage model = new HomePage();
                model.diplomaticrepresentation = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                model.QuickContactInfo = quickcontactinfo;

                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);

                return View(allData);
            }
            else
            {
                IvsVisaCountriesDiplomaticRepresentation obj = new IvsVisaCountriesDiplomaticRepresentation();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                return View("DiplomaticRepresentations", obj);
            }
        }
        
        public IActionResult DiplomaticRepresentationr(string countryName, string countryFlag)
        {
            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);
            
            if (fourCountries == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCountriesDiplomaticRepresentation on country.CountryIso equals us.CountryIso
                                     where country.CountryName.Contains(countryName) //&& us.IsEnable == 1
                                     orderby us.Priority
                                     select new IvsVisaCountriesDiplomaticRepresentation
                                     {
                                         OfficeName = us.OfficeName,
                                         OfficeAddress = us.OfficeAddress,
                                         OfficeCity = us.OfficeCity,
                                         OfficeCountry = us.OfficeCountry,
                                         OfficePhone = us.OfficePhone,
                                         OfficeFax = us.OfficeFax,
                                         OfficeTimings = us.OfficeTimings,
                                         OfficeVisaTimings = us.OfficeVisaTimings,
                                         OfficeCollectionTimings = us.OfficeCollectionTimings,
                                         OfficeEmail = us.OfficeEmail,
                                         OfficeNotes = us.OfficeNotes,
                                         TerritoryJurisdiction = us.TerritoryJurisdiction,
                                         bCountryFlag = countryFlag,
                                         bCountryName = countryName,
                                         IsEnable = us.IsEnable
                                     };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage model = new HomePage();
                model.diplomaticrepresentation = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                model.QuickContactInfo = quickcontactinfo;
                ViewBag.PagesOurContent = listPages.ToList();
                ViewBag.LinksOurContent = listLinks.ToList();
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);

                return View(allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }
    }
}
