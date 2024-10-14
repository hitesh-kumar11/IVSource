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
    public class IvsCountryTravelInfoesController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsCountryTravelInfoesController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsCountryTravelInfoes
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsCountryTravelInfo.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString()); 

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.TravelCategory.Contains(filter) || p.TravelType.Contains(filter)))).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsCountryTravelInfo>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsCountryTravelInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsCountryTravelInfo = await _context.IvsCountryTravelInfo.FindAsync(id);
            if (ivsCountryTravelInfo == null)
            {
                return NotFound();
            }
            return View(ivsCountryTravelInfo);
        }

        // POST: IvsCountryTravelInfoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryIso,TravelInfoId,TravelCategory,TravelType,TravelDescription,IsEnable,CreatedDate,ModifiedDate")] IvsCountryTravelInfo ivsCountryTravelInfo)
        {
            if (id != ivsCountryTravelInfo.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsCountryTravelInfo.ModifiedDate = DateTime.Now;
                    _context.Update(ivsCountryTravelInfo);
                    _context.Entry(ivsCountryTravelInfo).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsCountryTravelInfo).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsCountryTravelInfo).Property(x => x.TravelInfoId).IsModified = false;
                    _context.Entry(ivsCountryTravelInfo).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsCountryTravelInfoExists(ivsCountryTravelInfo.SerialNum))
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
            return View(ivsCountryTravelInfo);
        }

        private bool IvsCountryTravelInfoExists(int id)
        {
            return _context.IvsCountryTravelInfo.Any(e => e.SerialNum == id);
        }
    }
}
