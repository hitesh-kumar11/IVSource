using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;
/*
 #1 :   Edit by Nushrat as method was outdated.
 */

namespace IVSource.Controllers
{
    public class IvsVisaLinksController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaLinksController(IVSourceContext context)
        {
            _context = context;
        }        
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Id")
        {
            //var allList = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links); //----Edit by Nushrat
            var allList = _context.IvsVisaPages.Where(x => x.Type == HomePageType.Links);

            if (!string.IsNullOrWhiteSpace(filter))
            {   //----Edit by Nushrat-----------------
                //allList = allList.Where(x => filter.ToLower() == "yes" ? (x.IsEnable == true) : (filter.ToLower() == "no" ? (x.IsEnable == false) :
                allList = allList.Where(x => filter.ToLower() == "yes" ? (x.IsEnable == true) : (filter.ToLower() == "no" ? (x.IsEnable == false) :
                          (x.Title.Contains(filter) || x.Description.Contains(filter)))).OrderByDescending(y => y.Id);
            }
            else
                allList = allList.OrderByDescending(y => y.Id);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaPages>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "ID"); //#1
            //var model = await PagingList<IvsVisaPages>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            //model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaPages obj)
        {
            obj.Type = HomePageType.Links;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaPages.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaLinks/Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var ivsVisaLinks = await _context.IvsVisaPages.FindAsync(id);
            if(ivsVisaLinks == null)
            {
                return NotFound();
            }
            return View(ivsVisaLinks);
        }

        [HttpPost]
        public IActionResult Edit(IvsVisaPages obj)
        {
            if(ModelState.IsValid)
            {
                var link = _context.IvsVisaPages.Where(x => x.Id == obj.Id).FirstOrDefault();
                link.Title = obj.Title;
                link.Description = obj.Description;
                link.IsEnable = obj.IsEnable;
                link.Type = HomePageType.Links;

                _context.Update(link);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaLinks/Index");
        }

        [HttpPost]
        public IActionResult Delete(IvsVisaPages obj)
        {
            _context.Remove(obj);
            _context.SaveChanges();
            var pages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links);
            int count = pages.Count();
            return Json(count);
        }
    }
}