/*
 //-------------#1----------- Added by Nushrat on 04-Jan-2023
 //-------------------------- As could not add a NULL value in DB
 */

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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace IVSource.Controllers
{
    public class IvsVisaSubPagesController : Controller
    {
        private readonly IVSourceContext _context;
        private readonly IHostingEnvironment _HostEnvironment;
        public HomePage homepage { get; set; }
        public IConfiguration _Configuration { get; }

        public IvsVisaSubPagesController(IVSourceContext context, IHostingEnvironment HostEnvironment, IConfiguration iconfiguration)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
            _Configuration = iconfiguration;
        }
        public async Task<IActionResult> News(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "CreatedDate")
        {
            //var allList = _context.IvsVisaSubPages.Where(x => x.PageId == 11).OrderByDescending(y => y.CreatedDate);

            //------Code Created By Sandeep Sharma on 21-06-2023--------//
            var allList = (from m in _context.IvsVisaSubPages
                           where m.PageId == 11
                           orderby m.CreatedDate
                           select new IvsVisaSubPages
                           {
                               Title = m.Title,
                               Description = m.Description,
                               IsEnable = m.IsEnable,
                               CreatedDate = m.CreatedDate,
                               Id = m.Id
                           });


            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == true) : (filter.ToLower() == "no" ? (p.IsEnable == false) :
                p.Title.Contains(filter) || p.Description.Contains(filter))).OrderByDescending(y => y.CreatedDate);
            }

            int count = allList.Count();
            ViewBag.Total = count;
            // Edit by Nushrat
            //var models = await PagingList<IvsVisaSubPages>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            var models = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "CreatedDate");
            models.Action = "News";
            models.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(models);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaSubPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;
                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedImage = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedImage.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\News";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), dirPath,
                                    fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedImage.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;

                obj.PageId = 11;
                obj.IsEnable = true;
                obj.CreatedDate = DateTime.Now;
                obj.Description = "";           //----------As could not add a NULL value in DB ---#1
                obj.Country_ISO = "";           //----------As could not add a NULL value in DB ---#1             

                _context.IvsVisaSubPages.Add(obj);
                _context.SaveChanges();
                TempData["Message"] = "Inserted Successfully !";
                return RedirectToAction("../IvsVisaSubPages/News");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            var ivsVisaNews = _context.IvsVisaSubPages.Where(u => u.Id == id).FirstOrDefault();

            if (ivsVisaNews == null)
            {
                return NotFound();
            }

            return View(ivsVisaNews);
        }

        [HttpPost]
        public IActionResult Edit(IvsVisaSubPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;

                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedFile = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\News";
                        if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                   Directory.GetCurrentDirectory(), dirPath,
                                   fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;
                if (obj.Image == null)
                {
                    obj.Image = "";
                }
                if (obj.Country_ISO == null)
                {
                    obj.Country_ISO = "";
                }
                if (obj.Description == null)
                {
                    obj.Description = "";
                }
                if (obj.Title == null)
                {
                    obj.Title = "";
                }

                obj.PageId = 11;

                _context.Update(obj);

                _context.SaveChanges();

                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("News", "IvsVisaSubPages");
            }
            return View(obj);
        }

        public IActionResult Delete()
        {
            var ivsVisaNews = _context.IvsVisaSubPages.FirstOrDefault();

            if(ivsVisaNews == null)
            {
                return NotFound();
            }

            return View(ivsVisaNews);
        }

        [HttpPost]
        public IActionResult Delete(IvsVisaSubPages obj)
        {
            _context.Remove(obj);
            _context.SaveChanges();
            var subPages = _context.IvsVisaSubPages.Where(x => x.PageId == 11);
            int count = subPages.Count();
            return Json(count);
        }

        public async Task<IActionResult> VisitorsArea(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Id")
        {
            var allList = _context.IvsVisaSubPages.Where(u => u.PageId == 12).OrderBy(p => p.Title);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == true) : (filter.ToLower() == "no" ? (p.IsEnable == false) :
                p.Title.Contains(filter) || p.Description.Contains(filter))).OrderByDescending(y => y.PageId);
            }

            int count = allList.Count();
            ViewBag.Total = count;

            //var models = await PagingList<IvsVisaSubPages>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            var models = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            models.Action = "VisitorsArea";
            models.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(models);
        }

        public IActionResult VisitorsAreaAdd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VisitorsAreaAdd(IvsVisaSubPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;
                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedImage = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedImage.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\VisitorsArea";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), dirPath,
                                    fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedImage.CopyTo(stream);
                        }
                    }
                }
                if (obj.Image == null)
                {
                    obj.Image = "";
                }
                if (obj.Country_ISO == null)
                {
                    obj.Country_ISO = "";
                }
                if (obj.Description == null)
                {
                    obj.Description = "";
                }
                if (obj.Title == null)
                {
                    obj.Title = "";
                }
                if (fileNameImage != null)
                    obj.Image = fileNameImage;

                obj.PageId = 12;
                obj.IsEnable = true;
                obj.CreatedDate = DateTime.Now;
                //var title = obj.Title;

                //var countr = (from country in _context.IvsVisaCountries
                //             where country.CountryName.Contains(title)
                //             select new IvsVisaSubPages
                //             {
                //                 Title = country.CountryName
                //             }).FirstOrDefault();

                //obj.Title = countr.Title;

                _context.IvsVisaSubPages.Add(obj);
                _context.SaveChanges();
                TempData["Message"] = "Inserted Successfully !";
                return RedirectToAction("../IvsVisaSubPages/VisitorsArea");
            }
            return View();
        }

        public IActionResult VisitorsAreaEdit(int Id)
        {
            var ivsVisaNews = _context.IvsVisaSubPages.Where(u => u.Id == Id).FirstOrDefault();

            if (ivsVisaNews == null)
            {
                return NotFound();
            }

            return View(ivsVisaNews);
        }

        [HttpPost]
        public IActionResult VisitorsAreaEdit(IvsVisaSubPages obj)
        {
            //var oIvsVisaSubPages =_context.IvsVisaSubPages.Where(u => u.Id == obj.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                string fileNameImage = null;

                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedFile = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\VisitorsArea";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                   Directory.GetCurrentDirectory(), dirPath,
                                   fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;
                //if (obj.Image == null)
                //{
                //    obj.Image = "";
                //}
                if (obj.Country_ISO == null)
                {
                    obj.Country_ISO = "";
                }
                if (obj.Description == null)
                {
                    obj.Description = "";
                }
                if (obj.Title == null)
                {
                    obj.Title = "";
                }
                //if (obj.Image == null)
                //{
                //    obj.Image = _context.IvsVisaSubPages.Where(u => u.Id ==obj.Id).FirstOrDefault().Image;
                //}
                obj.PageId = 12;

                _context.Update(obj);

                _context.SaveChanges();

                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("VisitorsArea", "IvsVisaSubPages");
            }
            return View(obj);
        }

        public IActionResult VisitorsAreaDelete(int Id)
        {
            var ivsVisaNews = _context.IvsVisaSubPages.Where(u => u.Id == Id).FirstOrDefault();

            if (ivsVisaNews == null)
            {
                return NotFound();
            }

            return View(ivsVisaNews);
        }

        [HttpPost]
        public IActionResult VisitorsAreaDelete(IvsVisaSubPages obj)
        {
            _context.Remove(obj);
            _context.SaveChanges();
            var subPages = _context.IvsVisaSubPages.Where(x => x.PageId == 12);
            int count = subPages.Count();
            return Json(count);
        }

        public IActionResult LatestNews(int Id)
        {
            //t => t.Videos.Count).Take(10);
            var obj = from info in _context.IvsVisaSubPages
                      where info.Id == Id
                      select new IvsVisaSubPages 
                      {
                          Id = info.Id,
                          Title = info.Title,
                          Description = info.Description,
                          Image = info.Image,
                          IsEnable = info.IsEnable,
                          CreatedDate = info.CreatedDate
                      };
            ///var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);//----Added by Nushrat
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            //var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate); //----Added by Nushrat
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
      
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            List<HomePage> allData = new List<HomePage>();
            HomePage model = new HomePage();
            model.SubPagesNews = obj.ToList();
            ViewBag.PagesOurContent = listPages.ToList(); 
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;
            //allData.Add(model);
            return View(model);
        }

        public IActionResult Blog(int? pageNumber, string rowsToShow)
        {
            HomePage model = new HomePage();

            int pageSize = 10;

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (rowsToShow == null)
            {
                var subPagess = from info in _context.IvsVisaSubPages
                                where info.PageId == 11 && info.IsEnable==true
                                orderby info.Id descending
                                select new IvsVisaSubPages
                                {
                                    Id = info.Id,
                                    Title = info.Title,
                                    Description = a(info.Description),
                                    Image = info.Image,
                                    IsEnable = info.IsEnable
                                };                             

                model.NewsList = subPagess.ToList();
                //List<HomePage> allData = new List<HomePage>();
                //model.PagesOurContentPagesList = listPages.ToList();
                //model.PagesOurContentLinksList = listLinks.ToList();
                //allData.Add(model);
                //countNewsList = model.NewsList.Count();

                return View(PaginatedList<IvsVisaSubPages>.Create(subPagess.AsNoTracking(), pageNumber ?? 1, pageSize));
            }

            string[] stringArray = rowsToShow.Split('-');

            int[] intArray = Array.ConvertAll(stringArray, s => int.Parse(s));

            var subPages = from info in _context.IvsVisaSubPages
                           where info.PageId == 11 && info.CreatedDate.Year == intArray.First() && info.CreatedDate.Month == intArray.Last()
                           orderby info.CreatedDate descending
                           select new IvsVisaSubPages
                           {
                               Id = info.Id,
                               Title = info.Title,
                               Description = a(info.Description),
                               Image = info.Image,
                               IsEnable = info.IsEnable
                           };

            var logo = from info in _context.IvsVisaPages
                       where info.Type == HomePageType.LogoPage
                       select new IvsVisaPages
                       {
                           Image = info.Image
                       };
            model.bLogo = new IvsVisaPages();
            model.bLogo.Image = _Configuration["Logo"].ToString() + logo.First().Image;

            model.NewsList = subPages.ToList();
            //model.PagesOurContentPagesList = listPages.ToList();
            //model.PagesOurContentLinksList = listLinks.ToList();
            //List<HomePage> allDatar = new List<HomePage>();
            //allDatar.Add(model);
            //countNewsList = model.NewsList.Count();

            return View(PaginatedList<IvsVisaSubPages>.Create(subPages.AsNoTracking(), pageNumber ?? 1, pageSize));        
        }

        public string a(string subPages)
        {

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(subPages.ToString());
            string result = htmlDoc.DocumentNode.InnerText.Replace("&nbsp;", " ");
            return result;
        }

        public IActionResult CountryFactFinderr(string page, string countryName)
        {
            if (countryName.LastIndexOf(' ') != -1)
                countryName = countryName.Substring(0, countryName.LastIndexOf(' '));
            //Add logic by Hitesh on 07062023
            var ivsvisasubpagescountries = (from country in _context.IvsVisaSubPages.Where(x=>x.PageId==12 && x.IsEnable==true)
                                            select country.Title).Contains(countryName);

            if (ivsvisasubpagescountries == true)
            {
                var countryDetails = from country in _context.IvsVisaCountries
                                     join us in _context.IvsVisaCountriesDetails on country.CountryIso equals us.CountryIso
                                     join holidays in _context.IvsVisaCountriesHolidays on country.CountryIso equals holidays.CountryIso
                                     join airlines in _context.IvsVisaCountriesAirlines on country.CountryIso equals airlines.CountryIso
                                     where country.CountryName.Contains(countryName) && holidays.IsEnable == 1
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
                                         CountryFlag = us.CountryFlag,
                                         bCountryName = countryName,
                                         CountrySmallMap = us.CountrySmallMap,
                                         CountryLargeMap = us.CountryLargeMap
                                     };

                var airportName = from info in _context.IvsVisaCountries
                                  join airports in _context.IvsVisaCountriesAirports on info.CountryIso equals airports.CountryIso
                                  where info.CountryName.Contains(countryName) && airports.IsEnable == 1
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

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();

                HomePage model = new HomePage();
                model.countryfactfinderr = countryDetails.FirstOrDefault();
                model.AirportName1 = airportName.ToList();
                model.AirportNameCode = AirportNameCode;
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
                TempData["Message"] = "To find the other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }

        public async Task<IActionResult> Partners(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Id")
        {
            var allList = _context.IvsVisaSubPages.Where(u => u.PageId == 14).OrderBy(p => p.Title);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == true) : (filter.ToLower() == "no" ? (p.IsEnable == false) :
                p.Title.Contains(filter) || p.Description.Contains(filter))).OrderByDescending(y => y.PageId);
            }

            int count = allList.Count();
            ViewBag.Total = count;

            var models = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            models.Action = "Partners";
            models.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(models);
        }

        public IActionResult PartnersAdd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PartnersAdd(IvsVisaSubPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;
                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedImage = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedImage.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\Partners";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), dirPath,
                                    fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedImage.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;
                if (obj.Image == null)
                {
                    obj.Image = "";
                }
                if (obj.Country_ISO == null)
                {
                    obj.Country_ISO = "";
                }
                if (obj.Description == null)
                {
                    obj.Description = "";
                }
                if (obj.Title == null)
                {
                    obj.Title = "";
                }
                obj.PageId = 14;
                obj.IsEnable = true;
                obj.CreatedDate = DateTime.Now;

                _context.IvsVisaSubPages.Add(obj);
                _context.SaveChanges();
                TempData["Message"] = "Inserted Successfully !";
                return RedirectToAction("../IvsVisaSubPages/Partners");
            }
            return View();
        }

        public IActionResult PartnersEdit(int Id)
        {
            var ivsVisaPartners = _context.IvsVisaSubPages.Where(u => u.Id == Id).FirstOrDefault();

            if (ivsVisaPartners == null)
            {
                return NotFound();
            }

            return View(ivsVisaPartners);
        }
        [HttpPost]
        public IActionResult PartnersEdit(IvsVisaSubPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;

                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedFile = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images\\Partners";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                   Directory.GetCurrentDirectory(), dirPath,
                                   fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;
                if (obj.Image == null)
                {
                    obj.Image = "";
                }
                if (obj.Country_ISO == null)
                {
                    obj.Country_ISO = "";
                }
                if (obj.Description == null)
                {
                    obj.Description = "";
                }
                if (obj.Title == null)
                {
                    obj.Title = "";
                }
                obj.PageId = 14;

                _context.Update(obj);

                _context.SaveChanges();

                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("Partners", "IvsVisaSubPages");
            }
            return View(obj);
        }

    }
}