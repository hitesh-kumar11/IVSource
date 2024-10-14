using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Windows;
/*
 * --------#1
    Edit by Nushrat on 01-Sep-2022 to write/read terminal id after login for unique user authentication and lock one use at one time 
 */
namespace IVSource.Controllers
{
    public class LoginController : Controller
    {
        private readonly IVSourceContext _context;
        private IHttpContextAccessor Accessor;

        // Added Akash tyagi//
        const string SessionId = "_Id";
        const string TerminalId = "_TerminalId";

        public LoginController(IVSourceContext context, IHttpContextAccessor _accessor)
        {
            _context = context;
            this.Accessor = _accessor;
            
        }
        [HttpPost]
        public IActionResult TID(Login login)
        {
            //-----------------------------------------<--#1
            //Login login = new Login();
            var T_ID = login.TerminalId;
            if (T_ID == "" || T_ID == null)
            {
                //----------------------Read/Fetch the Cookie value using its Key.--------------------------------------------------------------------------<
                //var strSessionID = HttpContext.Session.Id;
                string stRTID = this.Accessor.HttpContext.Request.Cookies["TerminalID"];
                TempData["TerminalID"] = stRTID != null ? stRTID : "undefined";
                string TID = TempData["TerminalID"].ToString();
                //------------------------------------------------------------------------------------------------------------------------------------>
                 login.TerminalId = TID.ToString();
                //login.TerminalId.GetType = "Hidden";
            }
            //-------------------------------------------#1>           
            return View(login);

        }

        [HttpGet]
        public IActionResult Members()
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages;
            ViewBag.LinksOurContent = listLinks;
            ViewBag.QuickContactInfo = quickcontactinfo;
            Login oLogin = new Login();
            return View(oLogin);            
        }

        [HttpPost]
        public async Task<IActionResult> Members(Login login)
        {
            //-----Create and save session in DB------------<
            //var strSessionID = HttpContext.Session.Id;
            //await TID(login);


            // Added Akash tyagi//

            string guid = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(SessionId, guid);
            HttpContext.Session.SetString(TerminalId, login.TerminalId);


            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;
            if (ModelState.IsValid)
            {
                bool isValidUser = false;
                var currDate = DateTime.Now;
                var list = from us in _context.IvsUserDetails
                           join ut in _context.IvsUserTerminalId on us.UserId equals ut.UserId
                           where us.UserName == login.UserName && us.Password == login.Password && us.IsEnable == 1
                           && us.UserType == "USER" && ut.TerminalId == login.TerminalId //&& (ut.SessionId==login || ut.SessionId == null)
                           && us.ValidFrom <= currDate && us.ValidTo >= currDate
                           select new { us.UserName };

                if (list.ToList().Count == 1)
                    isValidUser = true;

                if (isValidUser)
                {                 

                    await SignInMemberUser(login.UserName, MemberType.MemberUser, MemberType.MemberUserAuth, false);

                    var upIvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.TerminalId == login.TerminalId).FirstOrDefault();

                    var userdetails = _context.IvsUserDetails.Where(x => x.UserId == upIvsVisaTerminalId.UserId).FirstOrDefault();

                    var username = userdetails.Name;
                    //--------------Logout if login again with same credentials/read existing TID----#1----------------------<
                    string stRTID = this.Accessor.HttpContext.Request.Cookies["TerminalID"];
                    TempData["TerminalID"] = stRTID != null ? stRTID : "undefined";
                    string TID = TempData["TerminalID"].ToString();
                   
                    var IvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.TerminalId == TID).FirstOrDefault();
                    if (upIvsVisaTerminalId.IsUsed == 1)
                    {
                        ViewBag.Message = "You have already login with this Terminal ID.".ToString();
                        TempData["Message"]= "Credentials being used by other user !".ToString();

                        ////-----Reset for login----------------------------------<
                        ViewBag.Message = string.Format("Logging off, click on Login button again to login");
                        
                        upIvsVisaTerminalId.IsUsed = 0;
                        //Added Akash tyagi//
                        upIvsVisaTerminalId.SessionID = Guid.NewGuid().ToString();
                        //_context.IvsUserTerminalId.Update(IvsVisaTerminalId);
                        _context.IvsUserTerminalId.Update(upIvsVisaTerminalId);  
                        await _context.SaveChangesAsync();
                        ////TempData["Message"] = "You have already login with this Terminal ID.".ToString();
                        //------------------------------------------------------>
                        return View(login);
                    }
                 
                    //-----------------------------------------------------------#1--<
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(30);
                    //string stWValue = "IVAjcX2lp5TT8htK2".ToString();
                    string stWValue = login.TerminalId.ToString();
                    //Create a Cookie with a suitable Key and add the Cookie to Browser.
                    if (stWValue == null)
                    {
                        stWValue = "";
                    }
                    Response.Cookies.Append("TerminalID", stWValue, option);
                    ////-----Create and save session in DB------------<
                    //var strSessionID = HttpContext.Session.Id;
                    //IvsVisaTerminalId.SessionID = "";
                    //IvsVisaTerminalId.SessionID = strSessionID.ToString(); 
                    //_context.IvsUserTerminalId.Update(IvsVisaTerminalId);
                    //await _context.SaveChangesAsync();
                    ////---------------------------------------------->
                    //-------------------------------------------------------------#1->

                    if (upIvsVisaTerminalId.IsUsed == 1)
                    {
                        ViewBag.Message = "The Terminal ID is already in use.";
                        return RedirectToAction("Members", "Login");                        
                    }
                    if (userdetails.ValidTo <= currDate)
                    {
                        TempData["Message"] = "Entered credentials were valid till " + userdetails.ValidTo;
                        return RedirectToAction("Members", "Login");
                    }
                    else
                    {
                        
                        upIvsVisaTerminalId.IsUsed = 1;
                        //Added Akash tyagi//
                        upIvsVisaTerminalId.SessionID = guid;
                        _context.IvsUserTerminalId.Update(upIvsVisaTerminalId);
                        await _context.SaveChangesAsync();
                        //TempData["Message"] = "Already login with this Terminal ID.".ToString();
                        TempData["UserName"] = userdetails.Name;
                        return RedirectToAction("SearchCountry", "home");
                    }
                }
                else
                {
                    TempData["Message"] = "Invalid credentials.";
                    return View(login);
                }
            }
            else
          
            return View(login);
            
        }
        private async Task SignInMemberUser(string username, string userRole, string authentication, bool isPersistent)
        {
            try
            {                
                var identity = new ClaimsIdentity(authentication);
                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(authentication, principal);
                //// Setting  
                //claims.Add(new Claim(ClaimTypes.Name, username));
                //claims.Add(new Claim(ClaimTypes.Role, "User"));
                //var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //var claimPrincipal = new ClaimsPrincipal(claimIdenties);

                //// Sign In.  
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult ForgotPassword()
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            HomePage obj = new HomePage();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;
            return View(obj);
        }
        [HttpPost]
        public IActionResult ForgotPassword(Login login)
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            HomePage obj = new HomePage();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            if (!string.IsNullOrWhiteSpace(login.EMail))
            {
                var userList = _context.IvsUserDetails.Where(x => x.UserName == login.UserName && x.Email != null && x.Email.Contains(login.EMail) && x.IsEnable == 1);
                var phone = "";
                if (userList.Count() > 0)
                {
                    phone = userList.First().Phone;
                }
                if (userList != null && userList.ToList().Count > 0)
                {
                    Random generator = new Random();
                    string randomNum = generator.Next(0, 1000000).ToString("D6");

                    var user = userList.FirstOrDefault();
                    user.ResetPasswordOtp = randomNum;
                    user.ResetPasswordOtpexpireOn = DateTime.Now.AddMinutes(30);
                    _context.Update(user);
                    _context.SaveChanges();

                    SmtpClient client = new SmtpClient("172.18.80.5");
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("Donotreply@itq.in");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Dear Sir,<br/>");
                    sb.Append("<br/>");
                    sb.Append("Please contact below user to reset password. <br/>");
                    sb.Append("<br/>");
                    sb.Append("Name       :      <b>" + login.UserName + "</b> <br/>");
                    sb.Append("Email      :      <b>" + login.EMail + "</b> <br/>");
                    sb.Append("Phone      :      <b>" + phone + "</b> <br/><br/>");
                    sb.Append("<br/>");
                    //sb.Append("<a href='" + HttpContext.Request.Scheme + "://" + HttpContext.Request.Host /*+ HttpContext.Request.PathBase*/
                    //            + "/login/OTPAuthentication?user="+login.UserName+"&email=" + login.EMail + "'>Click here </a> to reset your password. <br><br>");
                    sb.Append("Thanks & Regards, <br>IVSource Team");

                    mailMessage.To.Add("vinay.sharma@itq.in");
                    mailMessage.Body = sb.ToString();
                    mailMessage.Subject = "IVSource - Reset password";
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                    
                    TempData["Message"] = "Admin will contact you soon to reset your password.";
                    ModelState.Clear();
                }
                else
                    TempData["Message"] = "User does not exists";
            }
            else
                TempData["Message"] = "Please fill below details";
            return View(obj);
        }


        public IActionResult OTPAuthentication(string user, string email)
        {
            Login loginr = new Login();
            HomePage obj = new HomePage();
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            obj.PagesOurContentPagesList = listPages.ToList();
            obj.PagesOurContentLinksList = listLinks.ToList();
            obj.QuickContactInfo = quickcontactinfo;
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(user))
            {                
                loginr.EMail = email;
                loginr.UserName = user;
            }

            ViewBag.Email = email;
            ViewBag.UserName = user;
            return View(obj);
        }
        [HttpPost]
        public IActionResult OTPAuthentication(Login login)
        {
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable==true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            HomePage obj = new HomePage();
            obj.PagesOurContentPagesList = listPages.ToList();
            obj.PagesOurContentLinksList = listLinks.ToList();
            obj.QuickContactInfo = quickcontactinfo;
            if (!string.IsNullOrWhiteSpace(login.OTP))
            {
                var userList = _context.IvsUserDetails.ToList().Where(x => x.UserName == login.UserName && x.Email != null && x.Email.Contains(login.EMail) && x.IsEnable == 1);
                var isExits = _context.IvsUserDetails.ToList().Exists(x => x.UserName == login.UserName && x.Email != null && x.Email.Contains(login.EMail) && x.IsEnable == 1);
                if (userList != null && userList.ToList().Count > 0)
                {
                    var user = userList.FirstOrDefault();

                    if (user.ResetPasswordOtpexpireOn >= DateTime.Now)
                    {
                        if (user.ResetPasswordOtp == login.OTP)
                            return RedirectToAction("ResetPassword", login);
                        else
                            TempData["Message"] = "Your OTP is incorrect.";
                    }
                    else
                        TempData["Message"] = "Your reset password link was only valid for 30 minutes.";
                }
                else
                    TempData["Message"] = "User does not exists";
            }
            else
                TempData["Message"] = "Please fill below detail";
            return View(obj);
        }

        public IActionResult ResetPassword(Login login)
        {
            return View(login);
        }
        [HttpPost]
        public IActionResult ResetPasswordP(Login login)
        {
            if (!string.IsNullOrWhiteSpace(login.Password) && !string.IsNullOrWhiteSpace(login.ConfirmPassword))
            {
                if (login.Password != login.ConfirmPassword)
                {
                    TempData["Message"] = "Passwords do not match.";
                }
                else
                {
                    var userList = _context.IvsUserDetails.Where(x => x.UserName == login.UserName && x.Email.Contains(login.EMail) && x.IsEnable == 1);
                    if (userList != null && userList.ToList().Count > 0)
                    {
                        var user = userList.FirstOrDefault();
                        user.Password = login.Password;
                        _context.Update(user);
                        _context.SaveChanges();
                        TempData["Message"] = "Your password has been changed. Please login with your new password.";
                        //   login.EMail = "";
                        login = new Login();

                        SmtpClient client = new SmtpClient("172.18.80.5");
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress("Donotreply@galileo.co.in");
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Dear Sir,<br/>");
                        sb.Append("<br/>");
                        sb.Append("This is a confirmation that the password for your IVSource account "+login.UserName+ " has just been changed. <br/><br/>");
                        sb.Append("Thanks & Regards, <br>IVSource Team");

                        foreach (string emailId in user.Email.Split(","))
                            mailMessage.To.Add(emailId.Trim());

                        mailMessage.Body = sb.ToString();
                        mailMessage.Subject = "IVSource - Password changed";
                        mailMessage.IsBodyHtml = true;
                        client.Send(mailMessage);
                    }
                    else
                        TempData["Message"] = "User does not exists.";
                }
            }
            else
                TempData["Message"] = "Please fill below details";
            return View("ResetPassword", login);
        }

        // GET: Login
        [Route("adminpanel")]
        public ActionResult AdminPanel()
        {
            return View();
        }

        [Route("adminpanel")]
        [HttpPost]
        public async Task<IActionResult> AdminPanel(Login login)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = false;
                isValidUser = _context.IvsUserDetails.ToList().Exists(x => x.UserName == login.UserName && x.Password == login.Password && !string.IsNullOrWhiteSpace(x.UserType) && x.UserType == MemberType.MemberAdmin.ToUpper());
                if (isValidUser)
                {
                    await SignInMemberUser(login.UserName, MemberType.MemberAdmin, MemberType.MemberAdminAuth, false);
                    return RedirectToAction("DashboardTypeSelect", "Admin");
                }
                else
                {
                    ViewBag.Message = "The username or password is incorrect.";
                    return View();
                }
            }
            else
                return View(login);
        }
        private async Task SignInAdminUser(string username, bool isPersistent)
        {
            // Initialization.  
            var claims = new List<Claim>();

            try
            {
      
                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                //var authenticationManager = HttpContext;

                // Sign In.  
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });

            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
        }

        //**
        private async Task SignInUser1(string username, bool isPersistent)
        {
            // Initialization.  
            var claims = new List<Claim>();

            try
            {
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                //    new Claim("CMS", "True")
                //};
                //            var identity = new ClaimsIdentity(claims);
                //            var principal = new ClaimsPrincipal(identity);
                //            await HttpContext.Authentication.SignInAsync("TmiginScheme", principal);


                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                var authenticationManager = HttpContext;

                // Sign In.  
                await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
                //await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
        }
        //[Route("adminpanel")]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // Post: Login
        public ActionResult Logout1()
        {          
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            //var upIvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.TerminalId == login.TerminalId).FirstOrDefault();
            var upIvsVisaTerminalId = _context.IvsUserTerminalId.Where(x => x.IsUsed == 1).FirstOrDefault();
            upIvsVisaTerminalId.IsUsed = 0;
            _context.IvsUserTerminalId.Update(upIvsVisaTerminalId);
            await _context.SaveChangesAsync();
            return RedirectToAction("Members", "Login");
            //return View();
        }

        /// <summary>
        /// ////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
