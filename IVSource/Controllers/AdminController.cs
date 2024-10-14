using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("AdminPanel", "Login");
        }

        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult DashboardTypeSelect()
        {
            return View();
        }
        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult DashboardWebsite()
        {
            return View();
        }
        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult ManageUsers()
        {
            return RedirectToAction("Index", "IvsUserDetails");
        }
        public IActionResult IvsUsrs()
        {
            return RedirectToAction("ManageUsers", "ManageUsers");
        }
        public IActionResult IvsUserDetails()
        {
             return RedirectToAction("IvsUserDetails", "IvsUserDetails");
            
        }
        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult ManageCountry()
        {
            return View();
        }
        [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
        public IActionResult VisaDetails()
        {
            return RedirectToAction("SearchAdmin", "IvsVisaCountries", new { pageType = "admin" });
        }
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(MemberType.MemberAdminAuth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index");
        }
    }
}