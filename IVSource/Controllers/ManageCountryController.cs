using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReflectionIT.Mvc.Paging;
using System.Text.RegularExpressions;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class ManageCountryController : Controller
    {
        private readonly IVSourceContext _db;

        public ManageCountryController(IVSourceContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, string rowsToShow = "10", string searchString = "", string sortExpression = "ModifiedDate")
        {
            var query = _db.IvsVisaCountries.AsNoTracking().OrderBy(s => s.CountryIso);
            int count = query.Count();
            ViewBag.Total = count;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.CountryName.Contains(searchString) || s.CountryIso.Contains(searchString)).OrderByDescending(s => s.ModifiedDate);
            }
            // change by Nushrat
            // var model = await PagingList<IvsVisaCountries>.CreateAsync(query, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(query, Convert.ToInt16(rowsToShow), page, sortExpression, "ModifiedDate");
            model.RouteValue = new RouteValueDictionary { { "rowsToShow", rowsToShow } };

            return View(model);
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

            // Set Validation by Hitesh on 03-05-2023
            int checkduplicatecountryNameCount = _db.IvsVisaCountries.Where(x => x.CountryName == obj.CountryName && x.SerialNum!=obj.SerialNum).Count();
            int checkduplicatecountryISOCount = _db.IvsVisaCountries.Where(x => x.CountryIso == obj.CountryIso && x.SerialNum != obj.SerialNum).Count();
            if (checkduplicatecountryNameCount > 0)
            {
                TempData["Message"] = "Please enter any country name";
                ViewBag.Message = "<script>alert('Country name is already exists ! please try with unique name'); </script>";
                ModelState.Clear();
                return View(obj);
            }
            else if (checkduplicatecountryISOCount > 0)
            {
                TempData["Message"] = "Please enter any country name";
                ViewBag.Message = "<script>alert('Country iso is already exists ! please try with unique iso'); </script>";
                ModelState.Clear();
                return View(obj);
            }
            else
            {
                var country = (from x in _db.IvsVisaCountries
                               where x.CountryName == obj.CountryName
                               select x.Country).FirstOrDefault();

                obj.Country = country;
                if (ModelState.IsValid)
                {
                    _db.IvsVisaCountries.Update(obj);
                    _db.SaveChanges();
                    //TempData["result"] = "Record updated successfully !";
                    //ViewBag.Message = "Record updated successfully !";
                    ViewBag.Message = "<script>alert('Record updated successfully !');  window.location.href = '/ManageCountry/Index';</script>";
                    //ModelState.Clear();
                    //ViewBag.Message = "Record updated successfully !";
                    //  return View(obj);
                }
            }


            return View(obj);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IvsVisaCountries ivsVisaCountries = new IvsVisaCountries();
            return View(ivsVisaCountries);
        }
        // Funtion Added by Hitesh on 03-05-2023
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IvsVisaCountries obj)
        {
            //------validation added by Nushrat to avoid duplicate entry ---------
            obj.CreatedDate = DateTime.Now;
            obj.Country = obj.CountryName;
            TempData["Message"] = null;
            TempData["result"] = null;
            string msg = string.Empty;
            string Iso = obj.CountryIso;

            var country1 = (from x in _db.IvsVisaCountries
                            where x.CountryName == obj.CountryName || x.CountryIso == obj.CountryIso
                            select x.Country).FirstOrDefault();

            if (country1 == null || country1 == "")
            {
                if (obj.Country != null)
                {                   
                    //------validation added by Sandeep Sharma  12/05/2023 to avoid duplicate entry  ---------
                    bool saveFlg = true;
                    if (IsCountry(obj.CountryName))
                    {
                        obj.CountryName = obj.CountryName;
                        if (IsCountryIso(obj.CountryIso))
                        {
                            obj.CountryIso = obj.CountryIso.ToUpper();
                        }
                        else
                        {
                            ModelState.Clear();
                            saveFlg = false;
                            msg = "<script>alert('The field Country Iso must be a string or array type with a maximum length of 2. !');</script>";                          
                        }
                    }
                    else
                    {
                        ModelState.Clear();
                        saveFlg = false;
                        msg = "<script>alert('Country Name should starts with capital letter and No numeric or special character are allowed. !');</script>";                     
                    }                  
                  
                    if (saveFlg)
                        {
                            ModelState.Clear();
                            _db.IvsVisaCountries.Add(obj);
                            _db.SaveChanges();
                            bool chk = AddAdvisories(Iso);
                            bool chk1 = AddVisaInformation(Iso);
                            if (chk == true && chk1 == true)
                            {                               
                                msg = "<script>alert('Record Saved Successfully !'); window.location.href = '/ManageCountry/Index';</script>";                               
                            }
                            else
                            {                               
                                msg = "<script>alert('Record not saved !'); </script>";                               
                            }
                        }                   
                }               
            }
            else if (obj.Country != null)
            {
                ModelState.Clear();
                msg = "<script>alert('Country name or Country Iso is already exists ! please try with unique name'); </script>";               
                obj.Country = country1;                            
            }
            TempData["Message"] = msg;
            ViewBag.Message = msg;
            return View(obj);
        }
        public static bool IsCountry(string number)
        {
            return Regex.Match(number, @"^[A-Z]+[a-zA-Z\s]*$").Success;
        }
        public static bool IsCountryIso(string number)
        {
            int count = number.Count();
            if (count == 2)
            {
                return Regex.Match(number, @"^[A-Za-z\s]*$").Success;
            }
            else
            {
                return false;
            }
        }


        public async Task<IActionResult> Details(string CountryName)
        {
            CountryName = CountryName.Replace(System.Environment.NewLine, string.Empty);

            if (CountryName == null)
            {
                TempData["Message"] = "Please select country name";
                return RedirectToAction("Search");
            }

            //var ivsVisaCountries = await _db.IvsVisaCountries.FirstOrDefaultAsync(m => m.CountryName == CountryName.ToUpper());

            var ivsVisaCountries = await _db.IvsVisaCountries.FirstAsync(m => m.CountryName.Contains(CountryName));

            if (ivsVisaCountries == null)
            {
                TempData["Message"] = "Please select country name";
                return RedirectToAction("Search");
            }

            TempData["CountryIso"] = ivsVisaCountries.CountryIso;
            TempData["CountryName"] = ivsVisaCountries.CountryName;
            return View(ivsVisaCountries);
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
            catch (Exception)
            {
                //TempData["Message"] = "Oops Record Not Inserted due to" + " " + e.Message.ToString();
                chk_Flag = false;
            }
            //TempData["Message"] = "Inserted Successfully!";           
            return chk_Flag;
        }
        // Funtion Added by Hitesh Kumar on 16-05-2023
        public bool AddVisaInformation(string str)
        {
            bool status = true;
            var ran = RandomString(8);
            var aid = ran;
            IvsVisaInformation obj;
            try
            {
                var oList = _db.IvsVisaCountryTerritoryCities.Where(x => x.IsEnable == 1).OrderBy(x => x.SerialNum).ToList();
                int num = 0;
                foreach (var item in oList)
                {
                    num = num + 1;
                    obj = new IvsVisaInformation();

                    obj.CountryIso = str;
                    obj.CityId = item.CityId;
                    obj.VisaInformation = "NA";
                    obj.VisaGeneralInformation = "NA";
                    obj.IsEnable = 1;
                    obj.Priority = Convert.ToString(num);
                    obj.CreatedDate = DateTime.Now;
                    obj.ModifiedDate = DateTime.Now;
                    _db.IvsVisaInformation.AddRange(obj);
                }
                _db.SaveChanges();
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Funtion Added by Sandeep Sharma on 04-05-2023
        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}