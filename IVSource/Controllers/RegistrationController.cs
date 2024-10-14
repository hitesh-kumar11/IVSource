using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using System.Net.Mail;
using System.Text;
using IVSource.Controllers.Email;

namespace IVSource.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IVSourceContext _context;

        public RegistrationController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: Registration/Create
        public IActionResult Create()
        {
            var listPages = _context.IvsVisaPages.Where(x => x.Type == HomePageType.Pages && x.IsEnable == true).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.Type == HomePageType.Links && x.IsEnable == true).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            HomePage obj = new HomePage();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo.ToList();
            return View();
        }

        // POST: Registration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserName,Password,Designation,Company,Address,City,Country,Phone,Email,AdditionalEmail,ValidFrom,ValidTo")] IvsUserDetails ivsUserDetails)
        {
            ModelState.Remove("ValidFrom");
            ModelState.Remove("ValidTo");
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            HomePage obj = new HomePage();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo.ToList();
        
            if (ModelState.IsValid)
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                
                ivsUserDetails.CreatedDate = DateTime.Now;
                ivsUserDetails.UserType = "USER";
                ivsUserDetails.IsEnable = 1;
                ivsUserDetails.TerminalIdNo = "1";
                ivsUserDetails.UserId = "IVA"+ (new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray())).ToUpper();
                ivsUserDetails.ValidFrom = DateTime.Now;
               

                //----------------------------Changes done by Nushrat on 27-01-2023--------
                //----------------------------for validationg null-----------------starts--
                DateTime dt1 = new DateTime(2000, 01, 01);

                ivsUserDetails.UserId = ivsUserDetails.UserId ?? "";
                ivsUserDetails.UserName = ivsUserDetails.UserName ?? "";
                ivsUserDetails.Password = ivsUserDetails.Password ?? "";
                ivsUserDetails.UserType = ivsUserDetails.UserType ?? "";
                ivsUserDetails.CorporateId = ivsUserDetails.CorporateId ?? "";
                ivsUserDetails.Name = ivsUserDetails.Name ?? "";
                ivsUserDetails.Designation = ivsUserDetails.Designation ?? "";
                ivsUserDetails.Company = ivsUserDetails.Company ?? "";
                ivsUserDetails.Address = ivsUserDetails.Address ?? "";
                ivsUserDetails.City = ivsUserDetails.City ?? "";
                ivsUserDetails.Country = ivsUserDetails.Country ?? "";
                ivsUserDetails.Phone = ivsUserDetails.Phone ?? "";
                ivsUserDetails.Fax = ivsUserDetails.Fax ?? "";
                ivsUserDetails.Email = ivsUserDetails.Email ?? "";
                ivsUserDetails.AdditionalEmail = ivsUserDetails.AdditionalEmail ?? "";
                ivsUserDetails.TerminalIdNo = ivsUserDetails.TerminalIdNo ?? "";
                ivsUserDetails.ValidTo = ivsUserDetails.ValidTo ?? dt1;
                ivsUserDetails.ValidFrom = ivsUserDetails.ValidFrom ?? dt1;
                ivsUserDetails.IsEnable = ivsUserDetails.IsEnable ?? 0;
                ivsUserDetails.CreatedDate = ivsUserDetails.CreatedDate ?? ivsUserDetails.ValidFrom;
                ivsUserDetails.ModifiedDate = ivsUserDetails.ModifiedDate ?? ivsUserDetails.ValidFrom;
                ivsUserDetails.ResetPasswordOtp = ivsUserDetails.ResetPasswordOtp ?? "";
                ivsUserDetails.ResetPasswordOtpexpireOn = dt1;
                //---------------------------------------------------------------------End--

                IvsUserTerminalId UTI = new IvsUserTerminalId()
                {
                    UserId = ivsUserDetails.UserId,
                    TerminalId= Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("-", "").Substring(0, 20),
                    //IsUsed=1,
                    CreatedDate=DateTime.Now
                };
                _context.Add(ivsUserDetails);
                _context.Add(UTI);
                await _context.SaveChangesAsync();

                //LoginController lc = new LoginController();
                //await lc.SignInMemberUser(login.UserName, false);

                //Login login = new Login()
                //{
                //    UserName = ivsUserDetails.UserName,
                //    Password = ivsUserDetails.Password,
                //    TerminalId = UTI.TerminalId
                //};

                StringBuilder sb = new StringBuilder();
                sb.Append("Full Name : " + ivsUserDetails.Name+ "<br/>");
                sb.Append("Designation : " + ivsUserDetails.Designation+ "<br/>");
                sb.Append("Company : " + ivsUserDetails.Company+ "<br/>");
                sb.Append("Full Address : " + ivsUserDetails.Address+ "<br/>");
                sb.Append("Phone Number : " + ivsUserDetails.Phone+ "<br/>");
                sb.Append("Email ID : " + ivsUserDetails.Email+ "<br/><br/>");
                sb.Append("Thanks & Regards, <br>IVSource Team");

                EMail em = new EMail();
                em.SendEmail(sb.ToString(), "Registration Application | IV Source", null,null, true);

                TempData["Message"] = "Your account has been created.";  //Please login with your Terminal Id : "+ UTI.TerminalId;
                return RedirectToAction("Members", "Login");
            }
            return View(obj);
        }
      
        private bool IvsUserDetailsExists(int id)
        {
            return _context.IvsUserDetails.Any(e => e.SerialNum == id);
        }
    }
}
