using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IVSource.Models;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsUserDetailsController : Controller
    {
        private readonly IVSourceContext _db;

        public object random { get; private set; }

        public IvsUserDetailsController(IVSourceContext db)
        {
            _db = db;
        }
        //function for add new user
        public IActionResult AddNewUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(IvsUserDetails obj)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string charsTerminalID = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                obj.UserId = "IVA" + (new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray())).ToUpper();
                obj.IsEnable = 1;

                IvsUserTerminalId terminalIds;

                //----------------------------Changes done by Nushrat on 27-01-2023--------
                //----------------------------for validationg null-----------------starts--
                var terminalIdNo = obj.TerminalIdNo;
                DateTime dt1 = new DateTime(2000, 01, 01);               

                obj.UserId = obj.UserId ?? "";
                obj.UserName	= obj.UserName ?? "";
                obj.Password	= obj.Password ?? "";
                //Added by Hitesh behalf of Sandeep on 21072023_1121
                obj.UserType	= obj.UserType ?? "USER";
                obj.CorporateId	= obj.CorporateId ?? "";
                obj.Name	= obj.Name ?? "";
                obj.Designation	= obj.Designation ?? "";
                obj.Company	= obj.Company ?? "";
                obj.Address	= obj.Address ?? "";
                obj.City	= obj.City ?? "";
                obj.Country	= obj.Country ?? "";
                obj.Phone	= obj.Phone ?? "";
                obj.Fax	= obj.Fax ?? "";
                obj.Email	= obj.Email ?? "";
                obj.AdditionalEmail	= obj.AdditionalEmail ?? "";
                obj.TerminalIdNo	= obj.TerminalIdNo ?? "";
                obj.ValidTo = obj.ValidTo ?? dt1;
                obj.ValidFrom	= obj.ValidFrom ?? dt1;
                obj.IsEnable	= obj.IsEnable ?? 0;
                //obj.CreatedDate	= obj.CreatedDate ?? obj.ValidFrom;
                obj.CreatedDate = DateTime.Now;
                obj.ModifiedDate	= obj.ModifiedDate ?? obj.ValidFrom;
                obj.ResetPasswordOtp = obj.ResetPasswordOtp ?? "";
                obj.ResetPasswordOtpexpireOn = dt1;
                //---------------------------------------------------------------------End--

                List<IvsUserTerminalId> list = new List<IvsUserTerminalId>();
                for (var i = 0; i < Convert.ToInt32(terminalIdNo); i++)
                {               
                    terminalIds = new IvsUserTerminalId();
                    terminalIds.UserId = obj.UserId;
                    var terminalId = "IVA" + (new string(Enumerable.Repeat(charsTerminalID, 16).Select(s => s[random.Next(s.Length)]).ToArray()));
                    terminalIds.TerminalId = terminalId;
                    list.Add(terminalIds);
                }

                _db.IvsUserDetails.Add(obj);
                _db.IvsUserTerminalId.AddRange(list);
                _db.SaveChanges();
                TempData["result"] = "Record saved successfully !";
                ModelState.Clear();
                return RedirectToAction("../IvsUserDetails/Index");
            }
            return View("AddNewUser", obj);
        }

        public IActionResult Display()
        {
            var list = _db.IvsUserDetails.ToList();
            return View("../Admin/ManageUsers", list);
        }



        [HttpGet]
        public IActionResult Edit(int? SerialNum)
        {
            if (SerialNum == null)
            {
                return NotFound();
            }

            IvsUserTerminal obj = new IvsUserTerminal();
            obj.userDetail = _db.IvsUserDetails.Find(SerialNum);
            obj.userDetail.TerminalIdNo = "0";
            obj.userDetailTerminalLists = _db.IvsUserTerminalId.Where(x => x.UserId == obj.userDetail.UserId).ToList();
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(IvsUserTerminal obj)
        {
            obj.userDetail.ModifiedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                Random random = new Random();
                string charsTerminalID = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                IvsUserTerminalId terminalIds;

                var terminalIdNo = obj.userDetail.TerminalIdNo;

                List<IvsUserTerminalId> list = new List<IvsUserTerminalId>();

                for (var i = 0; i < Convert.ToInt32(terminalIdNo); i++)
                {
                    terminalIds = new IvsUserTerminalId();
                    terminalIds.UserId = obj.userDetail.UserId;
                    var terminalId = "IVA" + (new string(Enumerable.Repeat(charsTerminalID, 16).Select(s => s[random.Next(s.Length)]).ToArray()));
                    terminalIds.TerminalId = terminalId;
                    list.Add(terminalIds);
                }
                //----------------------------Changes done by Nushrat on 27-01-2023--------
                //----------------------------for validationg null-----------------starts--               
                DateTime dt1 = new DateTime(2000, 01, 01);

                obj.userDetail.UserId = obj.userDetail.UserId ?? "";
                obj.userDetail.UserName = obj.userDetail.UserName ?? "";
                obj.userDetail.Password = obj.userDetail.Password ?? "";
                obj.userDetail.UserType = obj.userDetail.UserType ?? "";
                obj.userDetail.CorporateId = obj.userDetail.CorporateId ?? "";
                obj.userDetail.Name = obj.userDetail.Name ?? "";
                obj.userDetail.Designation = obj.userDetail.Designation ?? "";
                obj.userDetail.Company = obj.userDetail.Company ?? "";
                obj.userDetail.Address = obj.userDetail.Address ?? "";
                obj.userDetail.City = obj.userDetail.City ?? "";
                obj.userDetail.Country = obj.userDetail.Country ?? "";
                obj.userDetail.Phone = obj.userDetail.Phone ?? "";
                obj.userDetail.Fax = obj.userDetail.Fax ?? "";
                obj.userDetail.Email = obj.userDetail.Email ?? "";
                obj.userDetail.AdditionalEmail = obj.userDetail.AdditionalEmail ?? "";
                obj.userDetail.TerminalIdNo = obj.userDetail.TerminalIdNo ?? "";
                obj.userDetail.ValidTo = obj.userDetail.ValidTo ?? dt1;
                obj.userDetail.ValidFrom = obj.userDetail.ValidFrom ?? dt1;
                obj.userDetail.IsEnable = obj.userDetail.IsEnable ?? 0;
                obj.userDetail.CreatedDate = obj.userDetail.CreatedDate ?? obj.userDetail.ValidFrom;
                obj.userDetail.ModifiedDate = obj.userDetail.ModifiedDate ?? obj.userDetail.ValidFrom;
                obj.userDetail.ResetPasswordOtp = obj.userDetail.ResetPasswordOtp ?? "";
                obj.userDetail.ResetPasswordOtpexpireOn = dt1;
                //---------------------------------------------------------------------End--
                _db.IvsUserDetails.Update(obj.userDetail);
                _db.IvsUserTerminalId.AddRange(list);
                _db.SaveChanges();
                TempData["result"] = "Record updated successfully !";
                ModelState.Clear();
                return RedirectToAction("../IvsUserDetails/Index");
            }
            return RedirectToAction("Index");
        }
       public async Task<IActionResult> Index(int page = 1, string searchString = "", string rowsToShow = "10", string sortExpression = "CreatedDate")
       {
            DateTime? today = DateTime.Today;
            // var query = _db.IvsUserDetails.AsNoTracking().OrderBy(s => s.SerialNum);
            var query = (from details in _db.IvsUserDetails
                         select new IvsUserDetails
                         {
                             SerialNum = details.SerialNum,
                             UserName = details.UserName,
                             Name = details.Name,
                             Company = details.Company,
                             ValidTo = details.ValidTo,
                             UserType = (details.ValidFrom <= today && details.ValidTo >= today ? "Active" : "Inactive"),
                             CreatedDate=details.CreatedDate
                         }).OrderByDescending(x => x.CreatedDate);
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.UserName.Contains(searchString) || s.Name.Contains(searchString) || s.Company.Contains(searchString) || s.UserType.Contains(searchString)).OrderBy(s => s.SerialNum);

                //  query = query.Where(s => s.UserName.Contains(searchString) || s.Name.Contains(searchString) || s.Company.Contains(searchString) || s.City.Contains(searchString) || s.Country.Contains(searchString)).OrderBy(s => s.SerialNum);
            }
            //var model = await PagingList<IvsUserDetails>.CreateAsync(query, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            //model.RouteValue = new RouteValueDictionary { { "rowsToShow", rowsToShow } };

            //return View(model);
            // Edit by Nushrat----------------
            //var model = await PagingList<IvsUserDetails>.CreateAsync(query, Convert.ToInt16(rowsToShow), page, sortExpression, "UserType");
            int count = query.Count();
            ViewBag.Total = count;
            var model = await PagingList.CreateAsync(query, Convert.ToInt16(rowsToShow), page, sortExpression, "CreatedDate");
            model.RouteValue = new RouteValueDictionary { { "rowsToShow", rowsToShow } };
            return View(model);
       }
    }
}