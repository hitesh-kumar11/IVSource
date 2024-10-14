using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaPagesController : Controller
    {
        private readonly IVSourceContext _context;
        private readonly IHostingEnvironment _HostEnvironment;

        public IvsVisaPagesController(IVSourceContext context, IHostingEnvironment HostEnvironment)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
        }

        public IActionResult SubscribeUsEdit()
        {
            var ivsVisaSubscribeUs = _context.IvsVisaPages.Where(u => u.Type == HomePageType.SubscribePage).FirstOrDefault();

            if (ivsVisaSubscribeUs == null)
            {
                return NotFound();
            }

            return View(ivsVisaSubscribeUs);
        }

        [HttpPost]
        public IActionResult SubscribeUsEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.SubscribePage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/SubscribeUsEdit");
        }

        public IActionResult ContactUsEdit()
        {
            var ivsVisaContactUs = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ContactPage).FirstOrDefault();

            if (ivsVisaContactUs == null)
            {
                return NotFound();
            }

            return View(ivsVisaContactUs);
        }

        [HttpPost]
        public IActionResult ContactUsEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ContactPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/ContactUsEdit");
        }

        public IActionResult NewsEdit()
        {
            var ivsVisaNews = _context.IvsVisaPages.Where(x => x.Type == HomePageType.NewsPage).FirstOrDefault();

            if (ivsVisaNews == null)
            {
                return NotFound();
            }

            return View(ivsVisaNews);
        }
        [HttpPost]
        public IActionResult NewsEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.NewsPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/NewsEdit");
        }

        public IActionResult VisitorsAreaEdit()
        {
            var ivsVisaVisitorsArea = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisitorAreaPage).FirstOrDefault();

            if (ivsVisaVisitorsArea == null)
            {
                return NotFound();
            }

            return View(ivsVisaVisitorsArea);
        }

        [HttpPost]
        public IActionResult VisitorsAreaEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisitorAreaPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/VisitorsAreaEdit");
        }

        public IActionResult ServicesEdit()
        {
            var ivsVisaServices = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ServicePage).FirstOrDefault();

            if (ivsVisaServices == null)
            {
                return NotFound();
            }

            return View(ivsVisaServices);
        }

        [HttpPost]
        public IActionResult ServicesEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ServicePage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/Services" +
                "Edit");
        }

        public IActionResult PartnersEdit()
        {
            var ivsVisaPartners = _context.IvsVisaPages.Where(x => x.Type == HomePageType.PartnersPage).FirstOrDefault();

            if (ivsVisaPartners == null)
            {
                return NotFound();
            }

            return View(ivsVisaPartners);
        }

        [HttpPost]
        public IActionResult PartnersEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.PartnersPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/PartnersEdit");
        }

        public IActionResult LogoEdit()
        {
            var ivsVisaLogo = _context.IvsVisaPages.Where(u => u.Type == HomePageType.LogoPage).FirstOrDefault();

            if (ivsVisaLogo == null)
            {
                return NotFound();
            }

            return View(ivsVisaLogo);
        }

        [HttpPost]
        public IActionResult LogoEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                string fileNameImage = null;

                if (HttpContext.Request.Form.Files.Count() != 0)
                {
                    if (HttpContext.Request.Form.Files["Image"] != null)
                    {
                        var postedFile = HttpContext.Request.Form.Files["Image"];
                        fileNameImage = "Image" + DateTime.Now.Ticks.ToString() + Path.GetExtension(postedFile.FileName);

                        string dirPath = _HostEnvironment.WebRootPath + "\\images";
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        var path = Path.Combine(
                                   Directory.GetCurrentDirectory(), dirPath,
                                   fileNameImage);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                        }
                    }
                }

                if (fileNameImage != null)
                    obj.Image = fileNameImage;

                obj.Type = "LOGO";

                _context.Update(obj);

                _context.SaveChanges();

                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("LogoEdit", "IvsVisaPages");
            }
            return View(obj);
        }

        public IActionResult AboutUsEdit()
        {
            var ivsVisaAboutUs = _context.IvsVisaPages.Where(x => x.Type == HomePageType.AboutUsPage).FirstOrDefault();
            //var ivsVisaAboutUs = _context.IvsVisaPages.Where(x => x.Type == HomePageType.AboutUsPage).FirstOrDefault();

            if (ivsVisaAboutUs == null)
            {
                return NotFound();
            }

            return View(ivsVisaAboutUs);
        }

        [HttpPost]
        public IActionResult AboutUsEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x =>  x.Type == HomePageType.AboutUsPage).FirstOrDefault();
               // var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.AboutUsPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/AboutUsEdit");
        }

        public IActionResult CountryFactFinderEdit()
        {
            var ivsVisaCountryFactFinder = _context.IvsVisaPages.Where(x => x.Type == HomePageType.CountryFactFinderPage).FirstOrDefault();

            if (ivsVisaCountryFactFinder == null)
            {
                return NotFound();
            }

            return View(ivsVisaCountryFactFinder);
        }

        [HttpPost]
        public IActionResult CountryFactFinderEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.CountryFactFinderPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/CountryFactFinderEdit");
        }

        public IActionResult DiplomaticRepresentationEdit()
        {
            var ivsVisaDiplomaticRepresentation = _context.IvsVisaPages.Where(x => x.Type == HomePageType.DiplomaticRepresentationPage).FirstOrDefault();

            if (ivsVisaDiplomaticRepresentation == null)
            {
                return NotFound();
            }

            return View(ivsVisaDiplomaticRepresentation);
        }

        [HttpPost]
        public IActionResult DiplomaticRepresentationEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.DiplomaticRepresentationPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/DiplomaticRepresentationEdit");
        }

        public IActionResult VisaNoteFeesEdit()
        {
            var ivsVisaNoteFees = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisaNoteFeesPage).FirstOrDefault();

            if (ivsVisaNoteFees == null)
            {
                return NotFound();
            }

            return View(ivsVisaNoteFees);
        }

        [HttpPost]
        public IActionResult VisaNoteFeesEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisaNoteFeesPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/VisaNoteFeesEdit");
        }

        public IActionResult VisaFormEdit()
        {
            var ivsVisaForm = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisaFormPage).FirstOrDefault();

            if (ivsVisaForm == null)
            {
                return NotFound();
            }

            return View(ivsVisaForm);
        }

        [HttpPost]
        public IActionResult VisaFormEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.VisaFormPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/VisaFormEdit");
        }

        public IActionResult AdvisoriesEdit()
        {
            var ivsVisaAdvisories = _context.IvsVisaPages.Where(x => x.Type == HomePageType.AdvisoriesPage).FirstOrDefault();

            if (ivsVisaAdvisories == null)
            {
                return NotFound();
            }

            return View(ivsVisaAdvisories);
        }

        [HttpPost]
        public IActionResult AdvisoriesEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.AdvisoriesPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/AdvisoriesEdit");
        }

        public IActionResult ReciprocalVisaEdit()
        {
            var ivsVisaReciprocalVisa = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ReciprocalVisaPage).FirstOrDefault();

            if (ivsVisaReciprocalVisa == null)
            {
                return NotFound();
            }

            return View(ivsVisaReciprocalVisa);
        }

        [HttpPost]
        public IActionResult ReciprocalVisaEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.Type == HomePageType.ReciprocalVisaPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/ReciprocalVisaEdit");
        }

        public IActionResult IndianMissionsOverseasEdit()
        {
            var ivsVisaIndianMissionsOverseasEdit = _context.IvsVisaPages.Where(x => x.Type == HomePageType.IndianMissionsOverseasPage).FirstOrDefault();

            if (ivsVisaIndianMissionsOverseasEdit == null)
            {
                return NotFound();
            }

            return View(ivsVisaIndianMissionsOverseasEdit);
        }

        [HttpPost]
        public IActionResult IndianMissionsOverseasEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.IndianMissionsOverseasPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/IndianMissionsOverseasEdit");
        }

        public IActionResult InternationalHelpAddressesEdit()
        {
            var ivsVisaInternationalHelpAddressesEdit = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.InternationalHelpAddressesPage).FirstOrDefault();

            if (ivsVisaInternationalHelpAddressesEdit == null)
            {
                return NotFound();
            }

            return View(ivsVisaInternationalHelpAddressesEdit);
        }

        [HttpPost]
        public IActionResult InternationalHelpAddressesEdit(IvsVisaPages obj)
        {
            if (ModelState.IsValid)
            {
                var sr = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.InternationalHelpAddressesPage).FirstOrDefault();
                sr.Title = obj.Title;
                sr.Description = obj.Description;
                sr.IsEnable = obj.IsEnable;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/InternationalHelpAddressesEdit");
        }

        //public IActionResult HolidaysEdit()
        //{
        //    var ivsVisaHolidaysEdit = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.HolidaysPage).FirstOrDefault();

        //    if (ivsVisaHolidaysEdit == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ivsVisaHolidaysEdit);
        //}

        //[HttpPost]
        //public IActionResult HolidaysEdit(IvsVisaPages obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var sr = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.HolidaysPage).FirstOrDefault();
        //        sr.Title = obj.Title;
        //        sr.Description = obj.Description;
        //        sr.IsEnable = obj.IsEnable;

        //        _context.Update(sr);
        //        _context.SaveChanges();
        //    }
        //    TempData["Message"] = "Updated Successfully!";
        //    return RedirectToAction("../IvsVisaPages/HolidaysEdit");
        //}

        public IActionResult QuickContactInfoEdit()
        {
            var ivsVisaQuickContactInfoEdit = _context.IvsVisaQuickContactInfo.FirstOrDefault();

            if (ivsVisaQuickContactInfoEdit == null)
            {
                return NotFound();
            }

            return View(ivsVisaQuickContactInfoEdit);
        }

        [HttpPost]
        public IActionResult QuickContactInfoEdit(IvsVisaQuickContactInfo obj)
        {
            if(ModelState.IsValid)
            {
                var sr = _context.IvsVisaQuickContactInfo.FirstOrDefault();
                sr.CompanyName = obj.CompanyName;
                sr.Address = obj.Address;
                sr.Email = obj.Email;
                sr.Phone = obj.Phone;

                _context.Update(sr);
                _context.SaveChanges();
            }
            TempData["Message"] = "Updated Successfully!";
            return RedirectToAction("../IvsVisaPages/QuickContactInfoEdit");
        }
    }
}
