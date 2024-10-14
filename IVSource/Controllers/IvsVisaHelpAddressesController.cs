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
using System.Text.RegularExpressions;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaHelpAddressesController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaHelpAddressesController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaHelpAddresses
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaHelpAddress.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString()); //.ToListAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.OfficeName.Contains(filter) || p.OfficeCountry.Contains(filter) || p.AddressType.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaHelpAddress>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaHelpAddresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaHelpAddress = await _context.IvsVisaHelpAddress.FindAsync(id);
            if (ivsVisaHelpAddress == null)
            {
                return NotFound();
            }
            return View(ivsVisaHelpAddress);
        }

        // POST: IvsVisaHelpAddresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryIso,OfficeId,OfficeName,OfficeAddress,OfficeCity,OfficePincode,OfficeCountry,OfficePhone,OfficeFax,OfficeEmail,OfficeWebsite,OfficeUrl,OfficeNotes,AddressType,IsEnable,CreatedDate,ModifiedDate")] IvsVisaHelpAddress ivsVisaHelpAddress)
        {
            if (id != ivsVisaHelpAddress.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaHelpAddress.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaHelpAddress);
                    _context.Entry(ivsVisaHelpAddress).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaHelpAddress).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaHelpAddress).Property(x => x.OfficeId).IsModified = false;
                    _context.Entry(ivsVisaHelpAddress).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaHelpAddressExists(ivsVisaHelpAddress.SerialNum))
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
            return View(ivsVisaHelpAddress);
        }

        private bool IvsVisaHelpAddressExists(int id)
        {
            return _context.IvsVisaHelpAddress.Any(e => e.SerialNum == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaHelpAddress obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);
            var oid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.OfficeId = "HA" + oid;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;

            _context.IvsVisaHelpAddress.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaHelpAddresses/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaHelpAddresses_UserController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaHelpAddresses_UserController(IVSourceContext context)
        {
            _context = context;
        }
        [CustemFilter]
        public IActionResult Indian_Mission_Overseas(string page, string countryName, string countryFlag)
        {
            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();           

            var checkIndianMissionOverseasData = (from indi in _context.IvsVisaHelpAddress
                                                  where indi.CountryIso == countryIso
                                                  select indi.AddressType).Contains("IMO");

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkIndianMissionOverseasData == true)
            {
                var countryDetails =  from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                      where country.CountryName.Contains(countryName) && us.AddressType == "IMO" //&& us.IsEnable == 1
                                      select new IvsVisaHelpAddress
                                      {
                                          OfficeName = us.OfficeName,
                                          OfficeAddress = us.OfficeAddress,
                                          OfficeCity = us.OfficeCity.Replace("\\n", string.Empty),
                                          OfficeCountry = us.OfficeCountry,
                                          OfficePhone = us.OfficePhone,
                                          OfficeFax = us.OfficeFax,
                                          OfficeEmail = us.OfficeEmail,
                                          OfficeWebsite = us.OfficeWebsite,
                                          bCountryFlag = countryFlag,
                                          bCountryName = countryName,
                                          IsEnable = us.IsEnable
                                      };


                HomePage model = new HomePage();
                model.helpaddress = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);
                return View("Indian_Mission_Overseas", allData);
            }
            else
            {
                IvsVisaHelpAddress obj = new IvsVisaHelpAddress();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                ViewBag.Page = page;
                return View("IndianMissionOverseas", obj);
            }
        }

        
        public IActionResult Indian_Mission_Overseasr(string countryName, string countryFlag)
        {
            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();

            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                     where country.CountryName.Contains(countryName) && us.AddressType == "IMO" //&& us.IsEnable == 1
                                     select new IvsVisaHelpAddress
                                     {
                                         OfficeName = us.OfficeName,
                                         OfficeAddress = us.OfficeAddress,
                                         OfficeCity = us.OfficeCity.Replace("\\n", string.Empty),
                                         OfficeCountry = us.OfficeCountry,
                                         OfficePhone = us.OfficePhone,
                                         OfficeFax = us.OfficeFax,
                                         OfficeEmail = us.OfficeEmail,
                                         OfficeWebsite = us.OfficeWebsite,
                                         bCountryFlag = countryFlag,
                                         bCountryName = countryName,
                                         IsEnable = us.IsEnable
                                     };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage model = new HomePage();
                model.helpaddress = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                ViewBag.PagesOurContent = listPages;
                ViewBag.LinksOurContent = listLinks;
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);
                return View("Indian_Mission_Overseasr", allData);
            }
            else
            {
                TempData["Message"] = "To find the oter country details you need to register / login here.";
                return RedirectToAction("Members", "Login");
            }
        }
        [CustemFilter]
        public IActionResult InternationalHelpAddress(string countryName, string countryFlag)
        {
            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();

            var checkInternationalHelpAddressData = (from inte in _context.IvsVisaHelpAddress
                                                     where inte.CountryIso == countryIso
                                                     select inte.AddressType).Contains("IHA");

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkInternationalHelpAddressData == true)
            {
                var countryDetails =  from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                      where country.CountryName == countryName && us.AddressType == "IHA" //&& us.IsEnable == 1
                                      select new IvsVisaHelpAddress
                                      {
                                          OfficeName = us.OfficeName,
                                          OfficeAddress = us.OfficeAddress,
                                          OfficeCity = us.OfficeCity,
                                          OfficeCountry = us.OfficeCountry,
                                          OfficePhone = us.OfficePhone,
                                          OfficeFax = us.OfficeFax,
                                          OfficeEmail = us.OfficeEmail,
                                          OfficeWebsite = us.OfficeWebsite,
                                          bCountryFlag = countryFlag,
                                          bCountryName = countryName,
                                          IsEnable = us.IsEnable
                                      };

                HomePage model = new HomePage();
                model.helpaddress = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);
                return View("International_Help_Address", allData);
            }
            else
            {
                IvsVisaHelpAddress obj = new IvsVisaHelpAddress();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                return View("InternationalHelpAddress", obj);
            }

        }

        
        public IActionResult InternationalHelpAddressr(string countryName, string countryFlag)
        {
            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();

            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaHelpAddress on country.CountryIso equals us.CountryIso
                                     where country.CountryName == countryName && us.AddressType == "IHA" //&& us.IsEnable == 1
                                     select new IvsVisaHelpAddress
                                     {
                                         OfficeName = us.OfficeName,
                                         OfficeAddress = us.OfficeAddress,
                                         OfficeCity = us.OfficeCity,
                                         OfficeCountry = us.OfficeCountry,
                                         OfficePhone = us.OfficePhone,
                                         OfficeFax = us.OfficeFax,
                                         OfficeEmail = us.OfficeEmail,
                                         OfficeWebsite = us.OfficeWebsite,
                                         bCountryFlag = countryFlag,
                                         bCountryName = countryName,
                                         IsEnable = us.IsEnable
                                     };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage model = new HomePage();
                model.helpaddress = countryDetails.ToList();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                ViewBag.PagesOurContent = listPages;
                ViewBag.LinksOurContent = listLinks;
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);
                return View("International_Help_Addressr", allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }

        }
    }
}
