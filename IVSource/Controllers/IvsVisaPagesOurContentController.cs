using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;

namespace IVSource.Controllers
{
    public class IvsVisaPagesOurContentController : Controller
    {
        private readonly IVSourceContext _context;
        public IvsVisaPagesOurContentController(IVSourceContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Id")
        {
            //var allList = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages);
            var allList = from pages in _context.IvsVisaPages
                          where pages.Type == HomePageType.Pages
                          //&& pages.IsEnable == true //--------------------Added by Nushrat
                          select new IvsVisaPages
                          {
                              Id = pages.Id,
                              Title = pages.Title,
                              Description = a(pages.Description),
                              IsEnable = pages.IsEnable
                          };

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(x => filter.ToLower() == "yes" ? (x.IsEnable == true) : (filter.ToLower() == "no" ? (x.IsEnable == false) :
                          (x.Title.Contains(filter) || x.Description.Contains(filter)))).OrderByDescending(y => y.Id);
            }
            else
                allList = allList.OrderByDescending(y => y.Id);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaPages>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Id");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        public string a(string subPages)
        {
            subPages = subPages ?? ""; //-----------added By Nushrat------------------
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(subPages.ToString());
            string result = htmlDoc.DocumentNode.InnerText.Replace(" ", " ");
            return result;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaPages obj)
        {
            obj.Type = HomePageType.Pages;
            obj.CreatedDate = DateTime.Now;

            _context.IvsVisaPages.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaPagesOurContent/Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var ivsVisaPagesOurContent = await _context.IvsVisaPages.FindAsync(id);
            if (ivsVisaPagesOurContent == null)
            {
                return NotFound();
            }
            return View(ivsVisaPagesOurContent);
        }

        [HttpPost]
        public IActionResult Edit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var link = _context.IvsVisaPages.Where(x => x.Id == obj.Id).FirstOrDefault();
                link.Title = obj.Title;
                link.Description = obj.Description;
                link.IsEnable = obj.IsEnable;
                link.Type = HomePageType.Pages;

                _context.Update(link);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPagesOurContent/Index");
        }

        [HttpPost]
        public IActionResult Delete(IvsVisaPages obj)
        {
            _context.Remove(obj);
            _context.SaveChanges();
            var pages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages);
            int count = pages.Count();
            return Json(count);
        }
    }
}