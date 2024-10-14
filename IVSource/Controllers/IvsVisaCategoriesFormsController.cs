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
using Newtonsoft.Json;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCategoriesFormsController : Controller
    {
        private readonly IVSourceContext _context;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IConfiguration _configuration;


        public IvsVisaCategoriesFormsController(IVSourceContext context, IHostingEnvironment HostEnvironment, IConfiguration iConfig)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
            _configuration = iConfig;

        }

        // GET: IvsVisaCategoriesForms
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = from m in _context.IvsVisaCategoriesForms
                          join n in _context.IvsVisaCountryTerritoryCities on m.CityId equals n.CityId
                          join o in _context.IvsVisaCategories on m.VisaCategoryCode equals o.VisaCategoryId
                          where m.CountryIso == TempData.Peek("CountryIso").ToString()
                          select new IvsVisaCategoriesForms
                          {
                              SerialNum = m.SerialNum,
                              CountryIso = m.CountryIso,
                              Form = m.Form,
                              IsEnable = m.IsEnable,
                              VisaCategory = o.VisaCategory,
                              CityName = n.CityName
                          };

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        //(p.CityName.Contains(filter) || p.VisaCategory.Contains(filter) || p.Form.Contains(filter)))).OrderByDescending(y => y.SerialNum);
                        (p.CityName.Contains(filter) || p.Form.Contains(filter) ))).OrderByDescending(y => y.SerialNum);            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaCategoriesForms>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCategoriesForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCategoriesForms = await _context.IvsVisaCategoriesForms.FindAsync(id);
            if (ivsVisaCategoriesForms == null)
            {
                return NotFound();
            }

            var model = from m in new List<IvsVisaCategoriesForms> { ivsVisaCategoriesForms }
                        join n in _context.IvsVisaCountryTerritoryCities on m.CityId equals n.CityId
                        join o in _context.IvsVisaCategories on m.VisaCategoryCode equals o.VisaCategoryId
                        where m.CountryIso == TempData.Peek("CountryIso").ToString()
                        select new IvsVisaCategoriesForms
                        {
                            SerialNum = m.SerialNum,
                            VisaCategoryCode = m.VisaCategoryCode,
                            Form = m.Form,
                            FormPath = m.FormPath,
                            IsEnable = m.IsEnable,
                            VisaCategory = o.VisaCategory,
                            CityName = n.CityName
                        };
            return View(model.First());
        }

        // POST: IvsVisaCategoriesForms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryIso,CityId,FormId,VisaCategoryCode,Form,FormPath,IsEnable,CreatedDate,ModifiedDate")] IvsVisaCategoriesForms ivsVisaCategoriesForms)
        {
            if (id != ivsVisaCategoriesForms.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string fileNameFormPath = null;
                    if (HttpContext.Request.Form.Files.Count() != 0)
                    {
                        if (HttpContext.Request.Form.Files["FormPath"] != null)
                        {
                            var postedFile = HttpContext.Request.Form.Files["FormPath"];
                            fileNameFormPath = "FormPath_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                            string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\FormPath";
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), dirPath,
                                        fileNameFormPath);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                    }

                    ivsVisaCategoriesForms.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCategoriesForms);

                    if (fileNameFormPath != null)
                        ivsVisaCategoriesForms.FormPath = fileNameFormPath;
                    else
                        _context.Entry(ivsVisaCategoriesForms).Property(x => x.FormPath).IsModified = false;

                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.CityId).IsModified = false;
                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.FormId).IsModified = false;
                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.VisaCategoryCode).IsModified = false;
                    _context.Entry(ivsVisaCategoriesForms).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCategoriesFormsExists(ivsVisaCategoriesForms.SerialNum))
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
            return View(ivsVisaCategoriesForms);
        }
        private bool IvsVisaCategoriesFormsExists(int id)
        {
            return _context.IvsVisaCategoriesForms.Any(e => e.SerialNum == id);
        }

       
        //----------Added by Nushrat------------<
        public IActionResult AddCategory2(IvsVisaCategoriesFormsObj obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);
            var CatID = obj.categoriesForms.CityId;
            var categories = from info in _context.IvsVisaCategories
                             where info.CityId == CatID && info.CountryIso == countryIso.CountryIso && info.VisaCategory != "" && info.VisaCategory != "NA"
                             select new IvsVisaCategories
                             {
                                 VisaCategoryId = info.VisaCategoryId,
                                 VisaCategory = info.VisaCategory
                             };

            //IvsVisaCategoriesFormsObj obj = new IvsVisaCategoriesFormsObj();
            obj.Categories = categories.ToList();
            ViewBag.category = categories.ToList();
            return View(obj);
        }

        // Funtion Added by Hitesh on 08-05-2023
        public JsonResult AddCategory(string cityId)
        {
            IvsVisaCategoriesFormsObj obj = new IvsVisaCategoriesFormsObj();
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);
            //var CatID = obj.categoriesForms.CityId;
            var categories = from info in _context.IvsVisaCategories
                                 //where info.CityId == cityId && info.CountryIso == countryIso.CountryIso && info.VisaCategory != "" && info.VisaCategory != "NA"
                             where info.CityId == cityId && info.CountryIso == countryIso.CountryIso
                             select new IvsVisaCategories
                             {
                                 VisaCategoryId = info.VisaCategoryId,
                                 VisaCategory = info.VisaCategory
                             };

            obj.Categories = categories.ToList();
            //var oList = JsonConvert.SerializeObject(obj);


            return Json(categories);
            }
        //-------------------------------------->




        //public ActionResult Add(string selectedValue)
        //{
        //    return View();
        //}





        public IActionResult Add()
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);
            //var categories = from info in _context.IvsVisaCategories
            //                 where info.CountryIso == countryIso.CountryIso && info.VisaCategory != "" && info.VisaCategory != "NA"
            //                 select new IvsVisaCategories
            //                 {
            //                     VisaCategoryId = info.VisaCategoryId,
            //                     VisaCategory = info.VisaCategory + "-" + info.VisaCategoryId
            //                 };

            //IvsVisaCategoriesFormsObj obj = new IvsVisaCategoriesFormsObj();
            //obj.Categories = categories.ToList();
            //ViewBag.category = categories.ToList();
            ////AddCategory(obj);
            //return View(obj);

            var categories = from info in _context.IvsVisaCategories
                             where info.CountryIso == countryIso.CountryIso && info.VisaCategory != "" && info.VisaCategory != "NA"
                             select new IvsVisaCategories
                             {
                                 CityId = info.CityId,
                                 VisaCategory = info.VisaCategory + "-" + info.CityId
                             };


                IvsVisaCategoriesForms obj = new IvsVisaCategoriesForms();
             //IvsVisaCategoriesFormsObj obj = new IvsVisaCategoriesFormsObj();
            //obj.Categories = categories.ToList();
            ViewBag.category = categories.ToList();
            //AddCategory(obj);
            return View(obj);
        }


        // Change the model parameter and also change view's properties by Hitesh on 15-05-2023
        [HttpPost]
        public IActionResult Add(IvsVisaCategoriesForms obj)
        {
            string fileNameFormPath = null;
            if (HttpContext.Request.Form.Files.Count() != 0)
            {
                if (HttpContext.Request.Form.Files["FormPath"] != null)
                {
                    var postedFile = HttpContext.Request.Form.Files["FormPath"];
                    fileNameFormPath = "FormPath_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                    string dirPath = _HostEnvironment.WebRootPath + "\\Uploads\\FormPath";
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), dirPath,
                                fileNameFormPath);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                }
            }

            if (fileNameFormPath != null)
                obj.FormPath = fileNameFormPath;

            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);
            var fid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.FormId = countryIso.CountryIso + fid;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaCategoriesForms.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCategoriesForms/Index");
        }

       

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCategoriesForms_UserController : Controller
    {
        private readonly IVSourceContext _context;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IConfiguration _configuration;

        public IvsVisaCategoriesForms_UserController(IVSourceContext context, IHostingEnvironment HostEnvironment, IConfiguration iConfig)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
            _configuration = iConfig;
        }

        [CustemFilter]
        public IActionResult DownloadVisaForm(string countryName, string countryFlag)
        {
            var countryiso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();

            var checkdownloadvisaform = (from infor in _context.IvsVisaCategoriesForms
                                         where infor.CountryIso == countryiso
                                         select infor.CountryIso).Contains(countryiso);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkdownloadvisaform == true)
            {
                var countryDetails = (from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaCategoriesForms on country.CountryIso equals us.CountryIso
                                      where country.CountryName == countryName && us.IsEnable == 1
                                      select new IvsVisaCategoriesForms
                                      {
                                          Form = us.Form,
                                          FormPath = us.FormPath,
                                          bCountryFlag = countryFlag,
                                          bCountryName = countryName
                                      });


                HomePage r = new HomePage();
                r.downloadvisaform = countryDetails.Distinct().ToList();
                r.PagesOurContentPagesList = listPages.ToList();
                r.PagesOurContentLinksList = listLinks.ToList();
                r.bCountryName = countryName;
                r.bCountryFlag = countryFlag;

                List<HomePage> allData = new List<HomePage>();
                allData.Add(r);
                return View("DownloadVisaForm", allData);
            }
            else
            {
                IvsVisaCategoriesForms obj = new IvsVisaCategoriesForms();
                obj.bCountryName = countryName;
                obj.bCountryFlag = countryFlag;
                return View("DownloadVisaFormrr", obj);
            }
        }

        
        public IActionResult DownloadVisaFormr(string countryName, string countryFlag)
        {
            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).FirstOrDefault();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            if (fourCountries == true)
            {
                var countryDetails = (from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaCategoriesForms on country.CountryIso equals us.CountryIso
                                      where country.CountryName == countryName && us.IsEnable == 1
                                      select new IvsVisaCategoriesForms
                                      {
                                          Form = us.Form,
                                          FormPath = us.FormPath,
                                          bCountryFlag = countryFlag,
                                          bCountryName = countryName
                                      });

                var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
                HomePage r = new HomePage();
                r.downloadvisaform = countryDetails.Distinct().ToList();
                r.PagesOurContentPagesList = listPages.ToList();
                r.PagesOurContentLinksList = listLinks.ToList();
                r.bCountryName = countryName;
                r.bCountryFlag = countryFlag;
                ViewBag.PagesOurContent = listPages;
                ViewBag.LinksOurContent = listLinks;
                ViewBag.QuickContactInfo = quickcontactinfo;
                List<HomePage> allData = new List<HomePage>();
                allData.Add(r);
                return View("DownloadVisaFormr", allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats  officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public IActionResult DownloadFile(string FormPath)
        {
            var Filenamee = _configuration["FileName"];
            if (FormPath == null)
                return Content("filename not present");
            var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", Filenamee) + FormPath;
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }


        //Added by Hitesh on 28092023
        [CustemFilter]
        public IActionResult CheckVisaRequirements(string countryName, string countryFlag)
        {
            var countryiso = (from iso in _context.IvsVisaCountries where iso.CountryName.Contains(countryName) select iso.CountryIso).FirstOrDefault();
           
            var checkdownloadvisaform = (from infor in _context.IvsVisaCategoriesForms
                                         where infor.CountryIso == countryiso
                                         select infor.CountryIso).Contains(countryiso);

            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (checkdownloadvisaform == true)
            {
                var countryDetails = (from country in _context.IvsVisaCountries
                                      join us in _context.IvsVisaCategoriesForms on country.CountryIso equals us.CountryIso
                                      where country.CountryName == countryName && us.IsEnable == 1
                                      select new IvsVisaCategoriesForms
                                      {
                                          Form = us.Form,
                                          FormPath = us.FormPath,
                                          bCountryFlag = countryFlag,
                                          bCountryName = countryName
                                      });


                HomePage r = new HomePage();
                r.downloadvisaform = countryDetails.Distinct().ToList();
                r.PagesOurContentPagesList = listPages.ToList();
                r.PagesOurContentLinksList = listLinks.ToList();
                r.bCountryName = countryName;
                r.bCountryFlag = countryFlag;

                List<HomePage> allData = new List<HomePage>();
                allData.Add(r);
                return View("CheckVisaRequirements", allData);
            }
            else
            {
                IvsVisaCategoriesForms obj = new IvsVisaCategoriesForms();
                obj.bCountryName = countryName;
                obj.bCountryFlag = countryFlag;
                //return View("DownloadVisaFormrr", obj);
                //Added by Hitesh on 12102023
                return View("CheckVisaRequirements", obj);
            }

            //return View("CheckVisaRequirements");
        }
    }
}
