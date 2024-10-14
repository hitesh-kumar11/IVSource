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
//using System.Data.Linq.SqlClient;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaSaarcDetailsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaSaarcDetailsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaSaarcDetails
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "SerialNum")
        {
            var allList = _context.IvsVisaSaarcDetails.Where(x => x.CountryIso == TempData.Peek("CountryIso").ToString());

            if (!string.IsNullOrWhiteSpace(filter))
            {
                int? isEnableParam = null;
                if (filter.ToLower() == "yes")
                    isEnableParam = 1;
                else if (filter.ToLower() == "no")
                    isEnableParam = 0;

                allList = allList.Where(p => p.IsVisaRequired.Contains(filter) || p.VisaApplyWhere.Contains(filter) || (p.IsEnable == isEnableParam)
                || (p.VisaOfficeName.Contains(filter)) || p.VisaOfficeCountry.Contains(filter)).OrderByDescending(y => y.SerialNum);
            }
            else
                allList = allList.OrderByDescending(y => y.SerialNum);

            int count = allList.Count();
            ViewBag.Total = count;

            var model = await PagingList<IvsVisaSaarcDetails>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaSaarcDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaSaarcDetails = await _context.IvsVisaSaarcDetails.FindAsync(id);
            if (ivsVisaSaarcDetails == null)
            {
                return NotFound();
            }
            return View(ivsVisaSaarcDetails);
        }

        // POST: IvsVisaSaarcDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,CountryIso,CountryId,IsVisaRequired,VisaApplyWhere,VisaOfficeId,VisaOfficeName,VisaOfficeAddress,VisaOfficeCity,VisaOfficePincode,VisaOfficeCountry,VisaOfficeTelephone,VisaOfficeFax,VisaOfficeEmail,VisaOfficeWebsite,VisaOfficeUrl,VisaOfficeNotes,IsEnable,CreatedDate,ModifiedDate")] IvsVisaSaarcDetails ivsVisaSaarcDetails)
        {
            if (id != ivsVisaSaarcDetails.VisaOfficeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaSaarcDetails.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaSaarcDetails);
                    _context.Entry(ivsVisaSaarcDetails).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaSaarcDetails).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaSaarcDetails).Property(x => x.VisaOfficeId).IsModified = false;
                    _context.Entry(ivsVisaSaarcDetails).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).FirstOrDefault();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaSaarcDetailsExists(ivsVisaSaarcDetails.VisaOfficeId))
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
            return View(ivsVisaSaarcDetails);
        }

        private bool IvsVisaSaarcDetailsExists(string id)
        {
            return _context.IvsVisaSaarcDetails.Any(e => e.VisaOfficeId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaSaarcDetails obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);
            var vid = ran;

            var countryName = obj.CountryId;
            var countryId = (from cId in _context.IvsVisaCountries where cId.CountryName.Contains(countryName) select cId.CountryIso).FirstOrDefault();

            obj.CountryId = countryId;
            obj.CountryIso = countryIso.CountryIso;
            obj.VisaOfficeId = "SA" + vid;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;

            _context.IvsVisaSaarcDetails.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaSaarcDetails/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
