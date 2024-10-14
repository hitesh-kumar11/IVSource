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

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCountriesAirportsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountriesAirportsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCountriesAirports
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaCountriesAirports.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString()); 

            if (!string.IsNullOrWhiteSpace(filter))
            {
                int? isEnableParam = null;
                if (filter.ToLower() == "yes")
                    isEnableParam = 1;
                else if (filter.ToLower() == "no")
                    isEnableParam = 0;

                string airportType = null;
                if (("international").Contains(filter.ToLower()))
                    airportType = "I";
                else if ("domestic".Contains(filter.ToLower()))
                    airportType = "D";

                allList = allList.Where(p => p.AirportName.Contains(filter) || p.AirportCode.Contains(filter) || (p.IsEnable == isEnableParam)
                || (p.AirportType == airportType)).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaCountriesAirports>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCountriesAirports/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountriesAirports = await _context.IvsVisaCountriesAirports.FindAsync(id);
            if (ivsVisaCountriesAirports == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountriesAirports);
        }

        // POST: IvsVisaCountriesAirports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,CountryIso,AirportId,AirportName,AirportCode,AirportType,IsEnable,CreatedDate,ModifiedDate")] IvsVisaCountriesAirports ivsVisaCountriesAirports)
        {
            if (id != ivsVisaCountriesAirports.AirportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCountriesAirports.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountriesAirports);
                    _context.Entry(ivsVisaCountriesAirports).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirports).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirports).Property(x => x.AirportId).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirports).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountriesAirportsExists(ivsVisaCountriesAirports.AirportId))
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
            return View(ivsVisaCountriesAirports);
        }

        private bool IvsVisaCountriesAirportsExists(string id)
        {
            return _context.IvsVisaCountriesAirports.Any(e => e.AirportId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCountriesAirports obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);

            var aid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.AirportId = countryIso.CountryIso + aid;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaCountriesAirports.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountriesAirports/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
