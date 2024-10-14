using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    public class IvsVisaSubscribeUsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaSubscribeUsController(IVSourceContext context)
        {
            _context = context;
        }
        public IActionResult Edit()
        {
            var ivsVisaSubscribeUs = _context.IvsVisaPages.Where(u => u.Type == HomePageType.SubscribePage).First();

            if (ivsVisaSubscribeUs == null)
            {
                return NotFound();
            }

            return View(ivsVisaSubscribeUs);
        }

        [HttpPost]
        public IActionResult Edit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.SubscribePage).First();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaSubscribeUs/Edit");
        }
    }
}