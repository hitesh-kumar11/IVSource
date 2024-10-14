using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountriesDetailsController : Controller
    {
        private readonly IVSourceContext _context;
        private readonly IHostingEnvironment _HostEnvironment;
        public IConfiguration _Configuration { get; }

        public IvsVisaCountriesDetailsController(IVSourceContext context, IHostingEnvironment HostEnvironment, IConfiguration iconfiguration)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
            _Configuration = iconfiguration;
        }

        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaCountriesDetails.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());
            var countryName = _context.IvsVisaCountries.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());
            ViewBag.countryName = countryName;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.CountryCapital.Contains(filter) || p.CountryIso.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaCountriesDetails>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCountriesDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountriesDetails = await _context.IvsVisaCountriesDetails.FindAsync(id);
            if (ivsVisaCountriesDetails == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountriesDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryLocation,CountryTime,CountryCapital,CountryLanguages,CountryArea,CountryPopulation,CountryNationalDay,CountryCurrency,CountryClimate,CountryWorldFactBook,CountryFlag,CountrySmallMap,CountryLargeMap,IsEnable")] IvsVisaCountriesDetails ivsVisaCountriesDetails)
        {
            if (ivsVisaCountriesDetails.SerialNum == 0)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    string fileNameCountryFlag = null;
                    string fileNameCountrySmallMap = null;
                    string fileNameCountryLargeMap = null;
                    if (HttpContext.Request.Form.Files.Count() != 0)
                    {
                        if (HttpContext.Request.Form.Files["CountryFlag"] != null)
                        {
                            var postedFile = HttpContext.Request.Form.Files["CountryFlag"];// HttpContext.Request.Form.Files[0];
                            //fileNameCountryFlag = "CountryFlag_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);
                            fileNameCountryFlag = postedFile.FileName;

                            string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountryFlag";
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), dirPath,
                                        fileNameCountryFlag);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                        if (HttpContext.Request.Form.Files["CountrySmallMap"] != null)
                        {
                            var postedFile = HttpContext.Request.Form.Files["CountrySmallMap"];
                            fileNameCountrySmallMap = "CountrySmallMap_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                            string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountrySmallMap";
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), dirPath,
                                        fileNameCountrySmallMap);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                        if (HttpContext.Request.Form.Files["CountryLargeMap"] != null)
                        {
                            var postedFile = HttpContext.Request.Form.Files["CountryLargeMap"];
                            fileNameCountryLargeMap = "CountryLargeMap_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                            string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountryLargeMap";
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), dirPath,
                                        fileNameCountryLargeMap);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                    }
                    ivsVisaCountriesDetails.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountriesDetails);

                    if (fileNameCountryFlag != null)
                        ivsVisaCountriesDetails.CountryFlag = fileNameCountryFlag;
                    else
                        _context.Entry(ivsVisaCountriesDetails).Property(x => x.CountryFlag).IsModified = false;
                    if (fileNameCountrySmallMap != null)
                        ivsVisaCountriesDetails.CountrySmallMap = fileNameCountrySmallMap;
                    else
                        _context.Entry(ivsVisaCountriesDetails).Property(x => x.CountrySmallMap).IsModified = false;
                    if (fileNameCountryLargeMap != null)
                        ivsVisaCountriesDetails.CountryLargeMap = fileNameCountryLargeMap;
                    else
                        _context.Entry(ivsVisaCountriesDetails).Property(x => x.CountryLargeMap).IsModified = false;

                    _context.Entry(ivsVisaCountriesDetails).Property(x => x.CountryIso).IsModified = false;
                    //_context.Entry(ivsVisaCountriesDetails).Property(x => x.IsEnable).IsModified = false;
                    _context.Entry(ivsVisaCountriesDetails).Property(x => x.CreatedDate).IsModified = false;

                //Addedby added by Hitesh on 18072023_1530
                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName || y.Country==CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountriesDetailsExists(ivsVisaCountriesDetails.SerialNum))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("Index", "IvsVisaCountriesDetails", new { CountryName = CountryName });
            //}
            //return View(ivsVisaCountriesDetails);
        }

        private bool IvsVisaCountriesDetailsExists(int id)
        {
            return _context.IvsVisaCountriesDetails.Any(e => e.SerialNum == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IvsVisaCountriesDetails obj)
        {
            string fileNameCountryFlag = null;
            string fileNameCountrySmallMap = null;
            string fileNameCountryLargeMap = null;

            if (HttpContext.Request.Form.Files.Count() != 0)
            {
                if (HttpContext.Request.Form.Files["CountryFlag"] != null)
                {
                    var postedFile = HttpContext.Request.Form.Files["CountryFlag"];// HttpContext.Request.Form.Files[0];
                    fileNameCountryFlag = "CountryFlag_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                    string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountryFlag";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), dirPath,
                                fileNameCountryFlag);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                }
                if (HttpContext.Request.Form.Files["CountrySmallMap"] != null)
                {
                    var postedFile = HttpContext.Request.Form.Files["CountrySmallMap"];
                    fileNameCountrySmallMap = "CountrySmallMap_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                    string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountrySmallMap";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), dirPath,
                                fileNameCountrySmallMap);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                }
                if (HttpContext.Request.Form.Files["CountryLargeMap"] != null)
                {
                    var postedFile = HttpContext.Request.Form.Files["CountryLargeMap"];
                    fileNameCountryLargeMap = "CountryLargeMap_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                    string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\CountryLargeMap";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), dirPath,
                                fileNameCountryLargeMap);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                }
            }

            if (fileNameCountryFlag != null)
            {
                obj.CountryFlag = fileNameCountryFlag;
            }
            else
            {
                TempData["Message"] = "Please choose country flag";
                return View("Add", obj);
            }

            obj.CountrySmallMap = fileNameCountrySmallMap;
            obj.CountryLargeMap = fileNameCountryLargeMap;

            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country || x.Country == country);//Added by Hitesh on18072023_1530 behalf of Sandeep

            obj.CountryIso = countryIso.CountryIso;
            obj.CreatedDate = DateTime.Now;
            obj.IsEnable = 1;

            _context.IvsVisaCountriesDetails.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountriesDetails/Index");
        }

    }

    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCountriesDetails_UserController : Controller
    {
        private readonly IVSourceContext _context;
        public IConfiguration _Configuration { get; }

        private readonly IHttpContextAccessor _httpcontext;

        public IvsVisaCountriesDetails_UserController(IVSourceContext context, IConfiguration iconfiguration, IHttpContextAccessor httpcontext)
        {
            _context = context;
            _Configuration = iconfiguration;
            _httpcontext = httpcontext;
        }

        [CustemFilter]
        public IActionResult CountryFactFinder(string page, string countryName, string countryFlag, string username)
        {
            countryName = countryName.Replace(System.Environment.NewLine, string.Empty);

            //var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();
            //Added by Hitesh on18072023_1530 behalf of Sandeep
            var countryIso = (from iso in _context.IvsVisaCountries where iso.CountryName ==countryName || iso.Country==countryName select iso.CountryIso).FirstOrDefault();

            var checkcountrydetailsData = (from coun in _context.IvsVisaCountriesDetails where coun.IsEnable == 1 select coun.CountryIso)
                .Contains(countryIso);

            var checkholidaysData = (from holi in _context.IvsVisaCountriesHolidays select holi.CountryIso).Contains(countryIso);

            var checkairlinesData = (from airl in _context.IvsVisaCountriesAirlines select airl.CountryIso).Contains(countryIso);

            var checkairportsData = (from airp in _context.IvsVisaCountriesAirports select airp.CountryIso).Contains(countryIso);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkcountrydetailsData == true && checkholidaysData == true && checkairlinesData == true && checkairportsData == true)
            {
                bool isAutenticated = _httpcontext.HttpContext.User.Identity.IsAuthenticated;
                var user = _httpcontext.HttpContext.User.Identity.Name;

                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                     join holidays in _context.IvsVisaCountriesHolidays on country.CountryIso equals holidays.CountryIso
                                     join airlines in _context.IvsVisaCountriesAirlines on country.CountryIso equals airlines.CountryIso

                                     where country.CountryName== countryName || country.Country == countryName && us.IsEnable == 1
                                     && country.IsEnable == 1 //-----Added by Nushrat to Avoid duplicacy by country
                                     select new IvsVisaCountry
                                     {
                                         CountryName = country.CountryName,
                                         CountryLocation = us.CountryLocation,
                                         CountryTime = us.CountryTime,
                                         CountryCapital = us.CountryCapital,
                                         CountryLanguages = us.CountryLanguages,
                                         CountryArea = us.CountryArea,
                                         CountryPopulation = us.CountryPopulation,
                                         CountryNationalDay = us.CountryNationalDay,
                                         CountryCurrency = us.CountryCurrency,
                                         CountryClimate = us.CountryClimate,
                                         HolidayName = holidays.HolidayName,
                                         AirlineName = airlines.AirlineName,
                                         AirlineCode = airlines.AirlineCode,
                                         //AirportName = airports.AirportName,
                                         CountryFlag = us.CountryFlag,
                                         //bCountryFlag = us.CountryFlag,
                                         bCountryName = country.CountryName,
                                         CountrySmallMap = us.CountrySmallMap,
                                         CountryLargeMap = us.CountryLargeMap
                                     };

                var airportName = from info in _context.IvsVisaCountries
                                  join airports in _context.IvsVisaCountriesAirports on info.CountryIso equals airports.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName==countryName || info.Country==countryName && airports.IsEnable == 1
                                  && info.IsEnable == 1 //-----Added by Nushrat to Avoid duplicacy by country
                                  select new IvsVisaCountry
                                  {
                                      AirportName = airports.AirportName,
                                      AirportCode = airports.AirportCode
                                  };

                string AirportNameCode = string.Empty;

                foreach (var x in airportName)
                {
                    AirportNameCode += " " + x.AirportName + "(" + x.AirportCode + ")" + ",";
                }
                AirportNameCode = AirportNameCode.TrimEnd(',');

                var holidayName = from info in _context.IvsVisaCountries
                                  join holidays in _context.IvsVisaCountriesHolidays on info.CountryIso equals holidays.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName==countryName || info.Country==countryName && holidays.IsEnable == 1
                                  && info.IsEnable == 1 //-----Added by Nushrat to Avoid duplicacy by country
                                  select new IvsVisaCountry
                                  {
                                      HolidayName = holidays.HolidayName
                                  };

                var airlineName = from info in _context.IvsVisaCountries
                                  join airlines in _context.IvsVisaCountriesAirlines on info.CountryIso equals airlines.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName==countryName || info.Country==countryName && airlines.IsEnable == 1
                                  && info.IsEnable == 1 //-----Added by Nushrat to Avoid duplicacy by country
                                  select new IvsVisaCountry
                                  {
                                      AirlineName = airlines.AirlineName,
                                      AirlineCode = airlines.AirlineCode
                                  };

                var logo = from info in _context.IvsVisaPages
                           where info.Type == "LOGO"

                           select new IvsVisaPages
                           {
                               Image = info.Image
                           };

                HomePage model = new HomePage();
                model.countryfactfinderr = countryDetails.FirstOrDefault();
                model.AirportName1 = airportName.ToList();
                model.HolidayName1 = holidayName.ToList();
                model.AirlineName1 = airlineName.ToList();
                model.AirportNameCode = AirportNameCode;
                model.bLogo = new IvsVisaPages();
                model.bLogo.Image = _Configuration["Logo"].ToString() + logo.First().Image;
                model.bCountryName = model.countryfactfinderr.CountryName;
                model.bCountryFlag = _Configuration["FileCountryFlag"].ToString() + model.countryfactfinderr.CountryFlag;
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(model);
                return View(allData);
            }
            else
            {
                IvsVisaCountry obj = new IvsVisaCountry();
                obj.bCountryName = countryName;
                obj.bCountryFlag = countryFlag;
                return View("CountryDetails", obj);
            }
        }

        public IActionResult CountryFactFinderr(string page, string countryName, string countryFlag, string username)
        {
            //Added by Hitesh on18072023_1530 behalf of Sandeep
            var countryr =  (from info in _context.IvsVisaCountries where info.CountryName==countryName || info.Country==countryName select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                     join holidays in _context.IvsVisaCountriesHolidays on country.CountryIso equals holidays.CountryIso
                                     join airlines in _context.IvsVisaCountriesAirlines on country.CountryIso equals airlines.CountryIso
                                     where country.CountryName==countryName || country.Country==countryName && us.IsEnable == 1
                                     select new IvsVisaCountry
                                     {
                                         CountryName = country.CountryName,
                                         CountryLocation = us.CountryLocation,
                                         CountryTime = us.CountryTime,
                                         CountryCapital = us.CountryCapital,
                                         CountryLanguages = us.CountryLanguages,
                                         CountryArea = us.CountryArea,
                                         CountryPopulation = us.CountryPopulation,
                                         CountryNationalDay = us.CountryNationalDay,
                                         CountryCurrency = us.CountryCurrency,
                                         CountryClimate = us.CountryClimate,
                                         HolidayName = holidays.HolidayName,
                                         AirlineName = airlines.AirlineName,
                                         AirlineCode = airlines.AirlineCode,
                                         //AirportName = airports.AirportName,
                                         CountryFlag = us.CountryFlag,
                                         //bCountryFlag = us.CountryFlag,
                                         bCountryName = country.CountryName,
                                         CountrySmallMap = us.CountrySmallMap,
                                         CountryLargeMap = us.CountryLargeMap
                                     };

                var airportName = from info in _context.IvsVisaCountries
                                  join airports in _context.IvsVisaCountriesAirports on info.CountryIso equals airports.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName==countryName || info.Country==countryName && airports.IsEnable == 1
                                  select new IvsVisaCountry
                                  {
                                      AirportName = airports.AirportName,
                                      AirportCode = airports.AirportCode
                                  };

                var holidayName = from info in _context.IvsVisaCountries
                                  join holidays in _context.IvsVisaCountriesHolidays on info.CountryIso equals holidays.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName==countryName || info.Country==countryName && holidays.IsEnable == 1
                                  select new IvsVisaCountry
                                  {
                                      HolidayName = holidays.HolidayName
                                  };

                var airlineName = from info in _context.IvsVisaCountries
                                  join airlines in _context.IvsVisaCountriesAirlines on info.CountryIso equals airlines.CountryIso
                                  //Added by Hitesh on18072023_1530 behalf of Sandeep
                                  where info.CountryName == countryName || info.Country == countryName && airlines.IsEnable == 1
                                  select new IvsVisaCountry
                                  {
                                      AirlineName = airlines.AirlineName,
                                      AirlineCode = airlines.AirlineCode
                                  };

                string AirportNameCode = string.Empty;

                foreach (var x in airportName)
                {
                    AirportNameCode += " " + x.AirportName + "(" + x.AirportCode + ")" + ",";
                }
                AirportNameCode = AirportNameCode.TrimEnd(',');


                var logo = from info in _context.IvsVisaPages
                           where info.Type == "LOGO"
                           select new IvsVisaPages
                           {
                               Image = info.Image
                           };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();

                HomePage model = new HomePage();
                model.countryfactfinderr = countryDetails.FirstOrDefault();
                model.AirportName1 = airportName.ToList();
                model.HolidayName1 = holidayName.ToList();
                model.AirlineName1 = airlineName.ToList();
                model.AirportNameCode = AirportNameCode;
                model.bLogo = new IvsVisaPages();
                model.bLogo.Image = _Configuration["Logo"].ToString() + logo.First().Image;
                model.bCountryName = model.countryfactfinderr.CountryName;
                model.bCountryFlag = _Configuration["FileCountryFlag"].ToString() + model.countryfactfinderr.CountryFlag;
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
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
                TempData["Message"] = "To find The other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }

        [CustemFilter]
        //World fact book redirect here
        public IActionResult WorldFactBook(IvsVisaCountriesDetails ivsVisaCountriesDetails, string countryName)
        {
            //Added by Hitesh on18072023_1530 behalf of Sandeep
            var countryiso = (from iso in _context.IvsVisaCountries where iso.CountryName==countryName || iso.Country==countryName select iso.CountryIso).FirstOrDefault();

            var checkworldfactbook = (from info in _context.IvsVisaCountriesDetails
                                      where info.CountryIso == countryiso
                                      select info.CountryIso).Contains(countryiso);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkworldfactbook == true)
            {
                var countryDetail = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                    where country.CountryName == countryName || country.Country== countryName
                                    select new IvsVisaCountriesDetails
                                    {
                                        CountryWorldFactBook = us.CountryWorldFactBook,
                                    };
                var model = countryDetail.FirstOrDefault();
                return Redirect(model.CountryWorldFactBook);
            }
            else
            {
                IvsVisaCountriesDetails obj = new IvsVisaCountriesDetails();
                obj.bCountryName = countryName;
                return View("WorldFactBook", obj);
            }
        }

       
        //Added by Hitesh on 07062023
        public IActionResult WorldFactBookr(IvsVisaCountriesDetails ivsVisaCountriesDetails, string countryName)
        {
            //Added by Hitesh on18072023_1530 behalf of Sandeep
            var countryiso = (from iso in _context.IvsVisaCountries where iso.CountryName==countryName || iso.Country==countryName select iso.CountryIso).FirstOrDefault();

            var checkworldfactbook = (from info in _context.IvsVisaCountriesDetails
                                      where info.CountryIso == countryiso
                                      select info.CountryIso).Contains(countryiso);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkworldfactbook == true)
            {
                var countryDetail = from country in _context.IvsVisaCountries
                                    join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                    //Added by Hitesh on18072023_1530 behalf of Sandeep
                                    where country.CountryName == countryName || country.Country==countryName
                                    select new IvsVisaCountriesDetails
                                    {
                                        CountryWorldFactBook = us.CountryWorldFactBook,
                                    };
                var model = countryDetail.FirstOrDefault();
                return Redirect(model.CountryWorldFactBook);
            }
            else
            {
                IvsVisaCountriesDetails obj = new IvsVisaCountriesDetails();
                obj.bCountryName = countryName;
                return View("WorldFactBook", obj);
            }
        }
    }
}
