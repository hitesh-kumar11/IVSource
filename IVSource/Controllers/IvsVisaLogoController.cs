
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    public class IvsVisaLogoController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaLogoController(IVSourceContext context)
        {
            _context = context;
        }
        public JsonResult Index()
        {
            var logo = _context.IvsVisaPages.Where(u => u.Type == HomePageType.LogoPage).FirstOrDefault();

            var newsObj = (from info in _context.IvsVisaSubPages
                           orderby info.CreatedDate descending
                           select new
                           {
                               CreatedDateText = info.CreatedDate.ToLongDateString().Substring(3),
                               CreatedDateValue = info.CreatedDate.Year + "-" + info.CreatedDate.Month
                           }).Distinct().Take(12);

            var obj = new { image = logo.Image, title = logo.Title, CreatedDate = newsObj };

            return Json(obj);
        }
    }
}