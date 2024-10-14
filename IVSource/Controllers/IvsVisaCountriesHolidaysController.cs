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
using System.Net;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountriesHolidaysController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountriesHolidaysController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCountriesHolidays
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaCountriesHolidays.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString()); 
            var countryName = _context.IvsVisaCountries.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());
            ViewBag.countryName = countryName;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.HolidayName.Contains(filter) || p.Year.Contains(filter) || p.Month.Contains(filter) || p.Date.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaCountriesHolidays>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);

        }
 
        // GET: IvsVisaCountriesHolidays/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountriesHolidays = await _context.IvsVisaCountriesHolidays.FindAsync(id);
            if (ivsVisaCountriesHolidays == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountriesHolidays);
        }

        // POST: IvsVisaCountriesHolidays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,CountryIso,HolidayId,Year,Month,Date,HolidayName,IsEnable,CreatedDate,ModifiedDate")] IvsVisaCountriesHolidays ivsVisaCountriesHolidays)
        {
            if (id != ivsVisaCountriesHolidays.HolidayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCountriesHolidays.HolidayName = WebUtility.HtmlEncode(ivsVisaCountriesHolidays.HolidayName);
                    ivsVisaCountriesHolidays.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountriesHolidays);
                    _context.Entry(ivsVisaCountriesHolidays).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCountriesHolidays).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCountriesHolidays).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    var upCountry11 = _context.IvsVisaCountriesHolidays.Where(x => x.HolidayId == id).FirstOrDefault();
                    
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountriesHolidaysExists(ivsVisaCountriesHolidays.HolidayId))
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
            return View(ivsVisaCountriesHolidays);
        }

        private bool IvsVisaCountriesHolidaysExists(string id)
        {
            return _context.IvsVisaCountriesHolidays.Any(e => e.HolidayId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCountriesHolidays obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);

            var hid = countryIso.CountryIso + ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.HolidayId = hid;
            obj.CreatedDate = DateTime.Now;
            obj.HolidayName = WebUtility.HtmlEncode(obj.HolidayName);

            _context.IvsVisaCountriesHolidays.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountriesHolidays/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
