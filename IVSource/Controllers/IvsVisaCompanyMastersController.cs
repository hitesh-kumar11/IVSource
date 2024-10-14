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
    public class IvsVisaCompanyMastersController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCompanyMastersController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCompanyMasters
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Id")
        {
            var allList = _context.IvsVisaCompanyMaster.Where(x=>!string.IsNullOrWhiteSpace(x.CompanyName)); //.ToListAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable) : (filter.ToLower() == "no" ? (!p.IsEnable) :
                        (p.CompanyName.Contains(filter) || p.CompanyDescription.Contains(filter)))).OrderByDescending(y => y.Id);
            }
            else
                allList = allList.OrderByDescending(y => y.Id);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaCompanyMaster>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            // var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            var model= await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page,sortExpression, "Id");
            //await PagingList<BookListViewModel>.CreateAsync(query, 10, page);
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow}, { "Id", sortExpression } };
                     
            return View(model);
        }

        // GET: IvsVisaCompanyMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IvsVisaCompanyMasters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,CompanyDescription,IsEnable")] IvsVisaCompanyMaster ivsVisaCompanyMaster)
        {
            if (ModelState.IsValid)
            {
                ivsVisaCompanyMaster.CreatedDate = DateTime.Now;
                _context.Add(ivsVisaCompanyMaster);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Added Successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(ivsVisaCompanyMaster);
        }

        // GET: IvsVisaCompanyMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCompanyMaster = await _context.IvsVisaCompanyMaster.FindAsync(id);
            if (ivsVisaCompanyMaster == null)
            {
                return NotFound();
            }
            return View(ivsVisaCompanyMaster);
        }

        // POST: IvsVisaCompanyMasters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,CompanyDescription,IsEnable")] IvsVisaCompanyMaster ivsVisaCompanyMaster)
        {
            if (id != ivsVisaCompanyMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCompanyMaster.UpdatedDate = DateTime.Now;
                    _context.Update(ivsVisaCompanyMaster);
                    _context.Entry(ivsVisaCompanyMaster).Property(x => x.CreatedDate).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCompanyMasterExists(ivsVisaCompanyMaster.Id))
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
            return View(ivsVisaCompanyMaster);
        }

        private bool IvsVisaCompanyMasterExists(int id)
        {
            return _context.IvsVisaCompanyMaster.Any(e => e.Id == id);
        }
    }
}
