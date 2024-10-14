using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllPageLogout.Filter;
using IVSource.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCovidInformation_UserController : Controller
    {
        private readonly IVSourceContext _context;
        public IActionResult Index()
        {
            return View();
        }

        public IvsVisaCovidInformation_UserController(IVSourceContext context)
        {
            _context = context;
        }
        [CustemFilter]
        public IActionResult CovidInformation(string countryName, string countryFlag)
        {          

            var countryIso = from n in _context.IvsVisaCountries
                             where n.CountryName == countryName
                             select n.CountryIso;

            //var checkcovidinformation = (from info in _context.IvsVisaApi
            //                             where info.Username == "ITQCovid19"
            //                             select info.Url).Contains("ITQCovid19");

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;
            ViewBag.CountryIso = countryIso.FirstOrDefault();

            //if (checkcovidinformation == true)
            //{
            //    var apiData = from n in _context.IvsVisaApi
            //                  where n.Name == "COVID" && n.IsEnabled
            //                  select new IvsVisaApi
            //                  {
            //                      bCountryIso = countryIso.First(),
            //                      bCountryFlag = countryFlag,
            //                      bCountryName = countryName,
            //                      Url = n.Url,
            //                      Type = n.Type,
            //                      Username = n.Username,
            //                      Password = n.Password
            //                  };


            HomePage model = new HomePage();
            //model.PagesOurContentPagesList = listPages.ToList();
            //model.PagesOurContentLinksList = listLinks.ToList();
            model.bCountryName = countryName;
            model.bCountryFlag = countryFlag;
            //model.QuickContactInfo = quickcontactinfo;
            model.bCountryIso = countryIso.FirstOrDefault();
            //model.covidinformation = apiData.ToList();

            //List<HomePage> allData = new List<HomePage>();
            //allData.Add(model);

                return View(model);
            //}
            //else
            //{
            //    IvsVisaApi obj = new IvsVisaApi();
            //    obj.bCountryName = countryName;
            //    obj.bCountryFlag = countryFlag;
            //    return View("CovidInformationrr", obj);
            //}
        }

       
        public IActionResult CovidInformationr(string countryName, string countryFlag)
        {
            var countryIso = from n in _context.IvsVisaCountries
                             where n.CountryName == countryName
                             select n.CountryIso;

            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                //var apiData = from n in _context.IvsVisaApi
                //              where n.Name == "COVID" && n.IsEnabled
                //              select new IvsVisaApi
                //              {
                //                  bCountryIso = countryIso.First(),
                //                  bCountryFlag = countryFlag,
                //                  bCountryName = countryName,
                //                  Url = n.Url,
                //                  Type = n.Type,
                //                  Username = n.Username,
                //                  Password = n.Password
                //              };

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();

                HomePage model = new HomePage();
                model.PagesOurContentPagesList = listPages.ToList();
                model.PagesOurContentLinksList = listLinks.ToList();
                model.bCountryName = countryName;
                model.bCountryFlag = countryFlag;
                model.bCountryIso = countryIso.FirstOrDefault();
                //model.covidinformation = apiData.ToList();
                ViewBag.PagesOurContent = listPages;
                ViewBag.LinksOurContent = listLinks;
                ViewBag.QuickContactInfo = quickcontactinfo;
                ViewBag.CountryIso = countryIso.FirstOrDefault();
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