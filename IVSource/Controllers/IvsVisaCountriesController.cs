using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountriesController : Controller
    {
        private readonly IVSourceContext _db;

        public IvsVisaCountriesController(IVSourceContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexOld(IvsVisaCountries obj)
        {
            //------validation added by Nushrat to avoid duplicate entry ---------
            obj.CreatedDate = DateTime.Now;
            obj.Country = obj.CountryName;
            //if (ModelState.IsValid)
            var country1 = (from x in _db.IvsVisaCountries
                            where x.CountryName == obj.CountryName
                            select x.Country).FirstOrDefault();
            
            if (country1 == null || country1 == "")
            {
                if (obj.Country != null)
                {
                    _db.IvsVisaCountries.Add(obj);
                    _db.SaveChanges();
                    TempData["result"] = "Record Saved Successfully !";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Please enter any country name.".ToString();
                    TempData["Message1"] = "Please enter any country name";
                    ModelState.Clear();
                }
            }
            else if (obj.Country != null)
            {

                TempData["Message1"] = "Country name exists ! try with unique name";
                ViewBag.Message = "Country name exists ! try with unique name.".ToString();
                Thread.Sleep(5000);
                obj.Country = country1;
                ModelState.Clear();
            }

            return RedirectToAction("../Admin/ManageCountry");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IvsVisaCountries obj)
        {
            //------validation added by Nushrat to avoid duplicate entry ---------
            if (ModelState.IsValid)
            {
                obj.CreatedDate = DateTime.Now;
                obj.Country = obj.CountryName;
                TempData["Message1"] = null;
                TempData["result"] = null;
                var country1 = (from x in _db.IvsVisaCountries
                                where x.CountryName == obj.CountryName || x.CountryIso == obj.CountryIso
                                select x.Country).FirstOrDefault();

                if (country1 == null || country1 == "")
                {
                    if (obj.Country != null)
                    {
                        obj.CountryIso = obj.CountryIso.ToUpper();
                        string Iso = obj.CountryIso;
                        _db.IvsVisaCountries.Add(obj);
                        _db.SaveChanges();
                        bool chk = AddAdvisories(Iso);
                        if (chk)
                        {
                            TempData["result"] = "Record Saved Successfully !";
                        }
                        else
                        {
                            TempData["result"] = " Record not saved !";
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        //ViewBag.Message = "Please enter any country name.";
                        TempData["Message1"] = "Please enter any country name";
                        ModelState.Clear();
                    }
                }
                else if (obj.Country != null)
                {
                    TempData["Message1"] = "Country name or Country Iso is already exists ! please try with unique name";
                    //ViewBag.Message = "Country name exists ! try with unique name.";
                    //Thread.Sleep(5000);
                    obj.Country = country1;
                    ModelState.Clear();
                }
            }
            return View();
            //return RedirectToAction("../Admin/ManageCountry");
        }


        // Funtion Added by Sandeep Sharma on 04-05-2023
        public bool AddAdvisories(string str)
        {

            bool chk_Flag = true;
            var ran = RandomString(8);
            var aid = ran;
            IvsVisaCountryAdvisories obj;
            try
            {
                List<string> add_Type = new List<string>(3);
                add_Type.Add("INTAD");
                add_Type.Add("IVSAD");
                add_Type.Add("RECVIS");

                foreach (string atype in add_Type)
                {
                    obj = new IvsVisaCountryAdvisories();
                    var advType = atype.Substring(0, 3);
                    obj.CountryIso = str;
                    obj.AdvisoryType = atype;
                    obj.AdvisoryId = advType + aid;
                    obj.Advisory = " ";
                    obj.CreatedDate = DateTime.Now;
                    obj.ModifiedDate = DateTime.Now;
                    _db.IvsVisaCountryAdvisories.AddRange(obj);
                }
                _db.SaveChanges();
            }
            catch (Exception )
            {
                //TempData["Message"] = "Oops Record Not Inserted due to" + " " + e.Message.ToString();
                chk_Flag = false;
            }
            //TempData["Message"] = "Inserted Successfully!";           
            return chk_Flag;
        }


        //Funtion Added by Sandeep Sharma on 04-05-2023
        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }



        public IActionResult Edit(int? SerialNum)
        {
            if (SerialNum == null || SerialNum == 0)
            {
                return NotFound();
            }

            var obj = _db.IvsVisaCountries.Find(SerialNum);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IvsVisaCountries obj)
        {
            obj.ModifiedDate = DateTime.Now;
            var country = (from x in _db.IvsVisaCountries
                          where x.CountryName == obj.CountryName
                          select x.Country).FirstOrDefault();

            obj.Country = country;

            if (ModelState.IsValid)
            {
                _db.IvsVisaCountries.Update(obj);
                _db.SaveChanges();
                TempData["result"] = "Record updated successfully !";
                ModelState.Clear();
                return RedirectToAction("../ManageCountry/Index");
            }
            return View();
        }
        public async Task<IActionResult> Details(string CountryName)
        {
            CountryName = CountryName.Replace(System.Environment.NewLine, string.Empty);

            if (CountryName == null)
            {
                TempData["Message1"] = "Please select country name";
                return RedirectToAction("Search");
            }

            //var ivsVisaCountries = await _db.IvsVisaCountries.FirstOrDefaultAsync(m => m.CountryName == CountryName.ToUpper());

            var ivsVisaCountries = await _db.IvsVisaCountries.FirstAsync(m => m.CountryName==CountryName || m.Country==CountryName);

            if (ivsVisaCountries == null)
            {
                TempData["Message1"] = "Please select country name";
                return RedirectToAction("Search");
            }

            TempData["CountryIso"] = ivsVisaCountries.CountryIso;
            TempData["CountryName"] = ivsVisaCountries.CountryName;
            return View(ivsVisaCountries);
        }

        //[AllowAnonymous]
        //public IActionResult Search(string pageType)
        //{
        //    if (pageType == "admin")
        //        return RedirectToAction(nameof(SearchAdmin));
        //    else
        //        return RedirectToAction(nameof(SearchHome));
        //}

        //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
        //public IActionResult SearchHome(string pageType)
        //{
        //    MemberPageBase obj = new IvsVisaCountries();
        //    ViewBag.listCities = _db.IvsVisaCountries.OrderBy(x => x.CountryName).ToList();
        //    return View("SearchCountry", obj);
        //}

        //[Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult SearchAdmin(string pageType)
        {
            //ViewBag.listCities = _db.IvsVisaCountries.OrderBy(x => x.CountryName).ToList(); //------------Added by Nushrat
            ViewBag.listCities = _db.IvsVisaCountries.OrderBy(x => x.Country).ToList();
            return View("Search");
        }
    }

    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCountries_UserController : Controller
    {
        private readonly IVSourceContext _db;

        public IvsVisaCountries_UserController(IVSourceContext db)
        {
            _db = db;
        }

        public IActionResult SearchHome(string pageType, string username)
        {
            MemberPageBase obj = new IvsVisaCountries();
            ViewBag.listCities = from country in _db.IvsVisaCountries
                                 where country.IsEnable == 1
                                 orderby country.Country
                                 select new IvsVisaCountries
                                 {
                                     CountryName = country.CountryName,
                                     CountryIso = country.CountryIso,
                                     Country = country.Country,
                                 };

            var logo = from info in _db.IvsVisaPages
                       where info.Type == HomePageType.LogoPage
                       select new IvsVisaPages
                       {
                          Image = info.Image
                       };

            obj.bLogo = logo.FirstOrDefault();
           

            var quickcontactinfo = _db.IvsVisaQuickContactInfo.ToList();
            var listPages = _db.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _db.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            ViewBag.QuickContactInfo = quickcontactinfo;
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.UserName = username;

            //HomePage obj = new HomePage();
            //obj.IvsVisaCountries = listCities.ToList();
            //ViewBag.listCities = listCities;
            //ViewBag.Id = "CountryName";

            //List<HomePage> allData = new List<HomePage>();
            //allData.Add(obj);
                       
            return View("SearchCountry", obj);
        }
    }
}