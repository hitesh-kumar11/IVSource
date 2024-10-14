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
using Microsoft.AspNetCore.Authorization;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCategoriesOptionsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCategoriesOptionsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCategoriesOptions
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "CreatedDate")
        {
             var allList = from m in _context.IvsVisaCategoriesOptions
                          join o in _context.IvsVisaCategories on m.VisaCategoryCode equals o.VisaCategoryId
                          join n in _context.IvsVisaCountryTerritoryCities on o.CityId equals n.CityId
                          where m.CountryIso == TempData.Peek("CountryIso").ToString()
                          select new IvsVisaCategoriesOptions
                          {
                              SerialNum = m.SerialNum,
                              CountryIso = m.CountryIso,
                              VisaCategoryOption = m.VisaCategoryOption,
                              IsEnable = m.IsEnable,
                              VisaCategory = o.VisaCategory,
                              CityName = n.CityName,
                              CreatedDate =n.CreatedDate
                          };


            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.CityName.Contains(filter) || p.VisaCategory.Contains(filter) || p.VisaCategoryOption.Contains(filter)))).OrderByDescending(y => y.CreatedDate);
            }
            else
                allList = allList.OrderByDescending(y => y.CreatedDate);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaCategoriesOptions>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "CreatedDate");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCategoriesOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCategoriesOptions = await _context.IvsVisaCategoriesOptions.FindAsync(id);
            if (ivsVisaCategoriesOptions == null)
            {
                return NotFound();
            }
            var model = from m in new List<IvsVisaCategoriesOptions> { ivsVisaCategoriesOptions }
                        join o in _context.IvsVisaCategories on m.VisaCategoryCode equals o.VisaCategoryId
                        join n in _context.IvsVisaCountryTerritoryCities on o.CityId equals n.CityId
                        where m.CountryIso == TempData.Peek("CountryIso").ToString()
                        select new IvsVisaCategoriesOptions
                        {
                            SerialNum = m.SerialNum,
                            VisaCategoryCode = m.VisaCategoryCode,
                            VisaCategoryOption = m.VisaCategoryOption,
                            VisaCategoryOptionAmountInr = m.VisaCategoryOptionAmountInr,
                            VisaCategoryOptionAmountOther = m.VisaCategoryOptionAmountOther,
                            IsEnable = m.IsEnable,
                            VisaCategory = o.VisaCategory,
                            CityName = n.CityName
                        };
            return View(model.First());
        }


        // Funtion Added by Sandeep Sharma on 15-06-2023
        public JsonResult AddCategory(string cityId)
        {
            IvsVisaCategoriesFormsObj obj = new IvsVisaCategoriesFormsObj();
            string country = TempData.Peek("CountryName").ToString();
            TempData["hdnCity"] = cityId;
            TempData.Keep("hdnCity");            
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);           
            var categories = from info in _context.IvsVisaCategories                                
                             where info.CityId == cityId && info.CountryIso == countryIso.CountryIso
                             select new IvsVisaCategories
                             {
                                 VisaCategoryId = info.VisaCategoryId,
                                 VisaCategory = info.VisaCategory
                             };

            obj.Categories = categories.ToList();          
            return Json(categories);
        }
        //-------------------------------------->

        public string FindCategory(string categoryId)
        {
            TempData["hdnCategoryId"] = categoryId;
            //TempData.Keep("hdnCategoryId");
            return string.Empty;
        }

        // POST: IvsVisaCategoriesOptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, IvsVisaCategoriesOptions ivsVisaCategoriesOptions)
        {
            if (id != ivsVisaCategoriesOptions.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCategoriesOptions.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCategoriesOptions);
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.VisaCategoryOptionId).IsModified = false;
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.VisaCategoryCode).IsModified = false;
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.VisaCategoryOptionCode).IsModified = false;
                    _context.Entry(ivsVisaCategoriesOptions).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    //var visaCategoryCode = ivsVisaCategoriesOptions.VisaCategoryCode;

                    //var upCategory = _context.IvsVisaCategories.Where(x => x.VisaCategoryId == visaCategoryCode).FirstOrDefault();
                    //upCategory.IsEnable = ivsVisaCategoriesOptions.IsEnable;
                    //_context.Update(upCategory);
                    //_context.Entry(upCategory).Property(x => x.SerialNum).IsModified = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCategoriesOptionsExists(ivsVisaCategoriesOptions.SerialNum))
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
            return View(ivsVisaCategoriesOptions);
        }

        // GET: IvsVisaCategoriesOptions/Edit/5
        public ActionResult EditAll()
        {
            var allList = (from op in _context.IvsVisaCategoriesOptions
                          join cat in _context.IvsVisaCategories on op.VisaCategoryCode equals cat.VisaCategoryId
                          where op.CountryIso == TempData.Peek("CountryIso").ToString() && !string.IsNullOrWhiteSpace(cat.VisaCategory)
                          select new IvsVisaCategories
                          {
                              VisaCategory = cat.VisaCategory,
                              VisaCategoryId = cat.VisaCategoryId,
                          }).Distinct();

            IvsVisaCategoriesOptions model = new IvsVisaCategoriesOptions();
            model.VisaCategoryList = allList.ToList();

            return View(model);
        }

        // POST: IvsVisaCategoriesOptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(string CountryName, [Bind("VisaCategoryOptionId,CountryIso,VisaCategoryCode,VisaCategoryOption,VisaCategoryOptionCode,VisaCategoryOptionAmountInr,VisaCategoryOptionAmountOther,IsEnable")] IvsVisaCategoriesOptions ivsVisaCategoriesOptions)
        {
            if (ModelState.IsValid)
            {
                try
                {   //------Commented and edit as wrong parameter is used for editall feature---Starts---------------------------------
                    //var allList = from op in _context.IvsVisaCategoriesOptions
                    //               join cat in _context.IvsVisaCategories on op.VisaCategoryCode equals cat.VisaCategoryId
                    //               where op.CountryIso == TempData.Peek("CountryIso").ToString() && cat.VisaCategoryId == ivsVisaCategoriesOptions.VisaCategoryCode
                    //               select op;
                    var allList = from op in _context.IvsVisaCategoriesOptions
                                  join cat in _context.IvsVisaCategories on op.VisaCategoryCode equals cat.VisaCategoryId
                                  where op.CountryIso == TempData.Peek("CountryIso").ToString() && op.VisaCategoryOption== ivsVisaCategoriesOptions.VisaCategoryOption //op.VisaCategoryOption== ivsVisaCategoriesOptions.VisaCategoryOption
                                  select op;
                    //--------------------------------------------------------------------------End---------------------------------
                    foreach (IvsVisaCategoriesOptions item in allList)
                    {
                        IvsVisaCategoriesOptions obj = item;
                        obj.ModifiedDate = DateTime.Now;
                        obj.VisaCategoryOption = ivsVisaCategoriesOptions.VisaCategoryOption;
                        obj.VisaCategoryOptionAmountInr = ivsVisaCategoriesOptions.VisaCategoryOptionAmountInr;
                        obj.VisaCategoryOptionAmountOther = ivsVisaCategoriesOptions.VisaCategoryOptionAmountOther;
                        obj.IsEnable = ivsVisaCategoriesOptions.IsEnable;
                        _context.Update(obj);
                    }

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCategoriesOptionsExists(ivsVisaCategoriesOptions.SerialNum))
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
            return View(ivsVisaCategoriesOptions);
        }

        private bool IvsVisaCategoriesOptionsExists(int id)
        {
            return _context.IvsVisaCategoriesOptions.Any(e => e.SerialNum == id);
        }

        public IActionResult Add()
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var categories = (from info in _context.IvsVisaCategories
                              where info.CountryIso == countryIso.CountryIso && info.VisaCategory != ""
                              select new IvsVisaCategories
                              {
                                  VisaCategoryId = info.VisaCategoryId,
                                  VisaCategory = info.VisaCategory
                              }).Distinct();

            IvsVisaCategoriesOptionsObj obj = new IvsVisaCategoriesOptionsObj();

            obj.Categories = categories.ToList();
            return View(obj);

        }

        [HttpPost]
        public IActionResult Add(IvsVisaCategoriesOptionsObj obj)
        {
            string cityCode = TempData.Peek("hdnCity").ToString();
            var cityName = getCityName(cityCode);
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);
            var visacatId = TempData.Peek("hdnCategoryId");

            var ran = RandomString(8);
            var vcid = countryIso.CountryIso + ran;

            obj.categoriesOptions.CountryIso = countryIso.CountryIso;
            obj.categoriesOptions.VisaCategoryOptionId = vcid;
            obj.categoriesOptions.CreatedDate = DateTime.Now;
            obj.categoriesOptions.CityName = cityName.ToString();
            if (visacatId.ToString() == "null")
            {
                TempData["Message"] = "Not Inserted blank Category!";
            }
            else
            {                
                obj.categoriesOptions.VisaCategoryCode = visacatId.ToString();
                _context.IvsVisaCategoriesOptions.Add(obj.categoriesOptions);
                _context.SaveChanges();
                TempData["Message"] = "Inserted Successfully!";
            }
             return RedirectToAction("../IvsVisaCategoriesOptions/Index");
        }

        public string getCityName(string ctyCode)
        {
            string cityName = string.Empty;
            if(ctyCode== "BLR8DL9D9H8")
            {
                cityName = "Bangalore";
            }
            else if(ctyCode == "CCUX67W7Z3M")
            {
                cityName = "Kolkata";
            }
            else if(ctyCode == "DELY79S2B5V")
            {
                cityName = "Delhi";
            }
            else if (ctyCode == "HYD4FY5Q8B6")
            {
                cityName = "Hyderabad";
            }
            else if (ctyCode == "IXAT12A9Q8F")
            {
                cityName = "Agartala";
            }
            else if (ctyCode == "MAAZ8W9T98V")
            {
                cityName = "Chennai";
            }
            else if (ctyCode == "MUMG7U9M52S")
            {
                cityName = "Mumbai";
            }
            else if (ctyCode == "PNY4LC7R46E")
            {
                cityName = "Pondicherry";
            }
            else
            {
                cityName = "Thiruvananthapuram";
            }
            return cityName;
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
