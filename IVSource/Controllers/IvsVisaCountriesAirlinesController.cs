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
    public class IvsVisaCountriesAirlinesController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCountriesAirlinesController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCountriesAirlines
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaCountriesAirlines.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString()); //.ToListAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.AirlineName.Contains(filter) || p.AirlineCode.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaCountriesAirlines>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);

        }

        // GET: IvsVisaCountriesAirlines/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCountriesAirlines = await _context.IvsVisaCountriesAirlines.FindAsync(id);
            if (ivsVisaCountriesAirlines == null)
            {
                return NotFound();
            }
            return View(ivsVisaCountriesAirlines);
        }

        // POST: IvsVisaCountriesAirlines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,CountryIso,AirlineId,AirlineName,AirlineCode,IsEnable,CreatedDate,ModifiedDate")] IvsVisaCountriesAirlines ivsVisaCountriesAirlines)
        {
            if (id != ivsVisaCountriesAirlines.AirlineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCountriesAirlines.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCountriesAirlines);
                    _context.Entry(ivsVisaCountriesAirlines).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirlines).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirlines).Property(x => x.AirlineId).IsModified = false;
                    _context.Entry(ivsVisaCountriesAirlines).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCountriesAirlinesExists(ivsVisaCountriesAirlines.AirlineId))
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
            return View(ivsVisaCountriesAirlines);
        }
        private bool IvsVisaCountriesAirlinesExists(string id)
        {
            return _context.IvsVisaCountriesAirlines.Any(e => e.AirlineId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCountriesAirlines obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);

            var aid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.AirlineId = aid;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaCountriesAirlines.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCountriesAirlines/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
