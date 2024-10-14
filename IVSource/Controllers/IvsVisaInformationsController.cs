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
    public class IvsVisaInformationsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaInformationsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaInformations
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Priority")
        {
            var allList = _context.IvsVisaInformation
                .Join(_context.IvsVisaCountryTerritoryCities,
                m => m.CityId,
                n => n.CityId,
                (m, n) => new IvsVisaInformation1
                {
                    CityName = n.CityName,
                    CountryIso = m.CountryIso,
                    IsEnable = m.IsEnable,
                    SerialNum = m.SerialNum,                   
                    Priority = string.IsNullOrEmpty(m.Priority) ? 0 : Convert.ToInt32(m.Priority)
                }).Where(o=> o.CountryIso == TempData.Peek("CountryIso").ToString());

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.CityName.Contains(filter)))).OrderBy(y => y.Priority);
            }
            else
                allList = allList.OrderBy(y => y.Priority);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaInformation1>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Priority");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaInformations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaInformation = await _context.IvsVisaInformation.FindAsync(id);
            ivsVisaInformation.VisaInformation = System.Net.WebUtility.HtmlDecode(ivsVisaInformation.VisaInformation);
            ivsVisaInformation.VisaGeneralInformation = System.Net.WebUtility.HtmlDecode(ivsVisaInformation.VisaGeneralInformation);
            if (ivsVisaInformation == null)
            {
                return NotFound();
            }
            return View(ivsVisaInformation);
        }

        // POST: IvsVisaInformations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CountryName, [Bind("SerialNum,CountryIso,CityId,VisaInformation,VisaGeneralInformation,IsEnable,Priority,CreatedDate,ModifiedDate")] IvsVisaInformation ivsVisaInformation)
        {
            if (id != ivsVisaInformation.SerialNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaInformation.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaInformation);
                    _context.Entry(ivsVisaInformation).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaInformation).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaInformation).Property(x => x.CityId).IsModified = false;
                    _context.Entry(ivsVisaInformation).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaInformationExists(ivsVisaInformation.SerialNum))
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
            return View(ivsVisaInformation);
        }

        private bool IvsVisaInformationExists(int id)
        {
            return _context.IvsVisaInformation.Any(e => e.SerialNum == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaInformation obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            obj.CountryIso = countryIso.CountryIso;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;

            _context.IvsVisaInformation.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaInformations/Index");
        }
    }
}
