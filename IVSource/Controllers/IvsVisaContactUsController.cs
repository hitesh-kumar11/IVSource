using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    public class IvsVisaContactUsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaContactUsController(IVSourceContext context)
        {
            _context = context;
        }
        public IActionResult Edit()
        {
            var ivsVisaContactUs = _context.IvsVisaPages.FirstOrDefault();

            if (ivsVisaContactUs == null)
            {
                return NotFound();
            }

            return View(ivsVisaContactUs);
        }

        [HttpPost]
        public IActionResult Edit(IvsVisaPages obj)
        {
            if(ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.ContactPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaContactUs/Edit");
        }
    }
}