using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaClientIpRegistrationsController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaClientIpRegistrationsController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaClientIpRegistrations
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "UpdatedDate")
        {
            var allList = from cr in _context.IvsVisaClientIpRegistration
                          join ud in _context.IvsUserDetails on cr.SerialNum equals ud.SerialNum
                          where ud.IsEnable==1
                          group new IvsVisaClientIpRegistration {
                              SerialNum = cr.SerialNum,
                              Company = ud.Company,
                              IsEnable = cr.IsEnable,
                              IpAddress = "",
                              CreatedDate = Convert.ToDateTime(cr.CreatedDate),
                              UpdatedDate = Convert.ToDateTime(cr.UpdatedDate)
                          } by cr.SerialNum into fData                          
                          select fData.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == true) : (filter.ToLower() == "no" ? (p.IsEnable == false) :
                        (p.IpAddress.ToLower().Contains(filter.ToLower()) || p.Company.ToLower().Contains(filter.ToLower()))
                        )).OrderByDescending(y => y.UpdatedDate);
            }
            else
                allList = allList.OrderByDescending(y => y.UpdatedDate);

            int count = allList.Count();
            ViewBag.Total = count;          
            var model = await PagingList.CreateAsync(allList, Convert.ToInt32(rowsToShow),page,sortExpression, "UpdatedDate");          
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            //model.OrderByDescending(x => x.UpdatedDate);
            return View(model);
        }

        // GET: IvsVisaClientIpRegistrations/Create
        public IActionResult Create()
        {
            //_________________Code Added By sandeep Sharma on 23-05-2023______________________________//
            DateTime? today = DateTime.Today;
            var IsEnable = _context.IvsUserDetails.Where(x => x.ValidTo >= today && x.IsEnable == 0);          
            if (IsEnable.Count() > 0)
            {
                foreach (var ud in IsEnable)
                {                   
                    var newdetail = _context.IvsUserDetails.Where(f => f.SerialNum == ud.SerialNum).FirstOrDefault();
                    if (newdetail == null) throw new Exception("");
                    newdetail.IsEnable = 1;
                    newdetail.ModifiedDate = DateTime.Now;
                }                
                _context.SaveChanges();
            }

            //_________________Code Added By sandeep Sharma on 25 - 05 - 2023______________________________//
            var IsDisable = _context.IvsUserDetails.Where(x => x.ValidTo < today && x.IsEnable == 1);
            if (IsDisable.Count() > 0)
            {
                foreach (var ud in IsDisable)
                {
                    var newdetail = _context.IvsUserDetails.Where(f => f.SerialNum == ud.SerialNum).FirstOrDefault();
                    if (newdetail == null) throw new Exception("");
                    newdetail.IsEnable = 0;
                    newdetail.ModifiedDate = DateTime.Now;
                }
                _context.SaveChanges();
            }

            //-----------------------old code-----------------------//
            //IvsVisaClientIpRegistration model = new IvsVisaClientIpRegistration();
            //model.CompanyList = (from data in _context.IvsVisaCompanyMaster
            //                     where data.IsEnable == true && !string.IsNullOrWhiteSpace(data.CompanyName)
            //                     select new IvsVisaCompanyMaster
            //                     {
            //                         Id = data.Id,
            //                         CompanyName = data.CompanyName
            //                     }).ToList();

            //IvsVisaClientIpRegistration model = new IvsVisaClientIpRegistration();
            //model.CompanyList = (from data in _context.IvsVisaCompanyMaster
            //                     join ud in _context.IvsUserDetails on data.CompanyName equals ud.Company
            //                     where data.IsEnable == true && !string.IsNullOrWhiteSpace(data.CompanyName) && ud.IsEnable==1                                
            //                     select new IvsVisaCompanyMaster
            //                     {
            //                         Id = data.Id,
            //                         CompanyName = data.CompanyName
            //                     }).ToList();

            IvsVisaClientIpRegistration model = new IvsVisaClientIpRegistration();
            model.UserDetail = (from data in _context.IvsUserDetails 
                                 where data.IsEnable == 1
                                 select new IvsUserDetails
                                 {
                                     SerialNum = data.SerialNum,
                                     Company = data.Company
                                 }).ToList();


            return View(model);           
        }

        // POST: IvsVisaClientIpRegistrations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IvsVisaClientIpRegistration ivsVisaClientIpRegistration)
        {
            if (ModelState.IsValid)
            {
                var presentData = _context.IvsVisaClientIpRegistration.Where(x => x.SerialNum == ivsVisaClientIpRegistration.SerialNum);
                if (presentData.Count() > 0)
                {
                    if (UpdateInformation(ivsVisaClientIpRegistration))
                    {
                        TempData["Message"] = "Updated  Successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    var res = _context.IvsUserDetails.Where(x => x.Company == ivsVisaClientIpRegistration.Company && x.IsEnable == 1);
                    if (res.Count() == 1)
                    {
                        bool isIpExist = false;
                        var IPArr = ivsVisaClientIpRegistration.IpAddress.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(string ip in IPArr)
                        {
                            isIpExist = CheckIpExist(ip);
                        }
                        if(isIpExist!=true)
                        {
                            List<IvsVisaClientIpRegistration> list = new List<IvsVisaClientIpRegistration>();
                            foreach (string ip in IPArr)
                            {
                                IvsVisaClientIpRegistration obj = new IvsVisaClientIpRegistration();
                                obj.Company = ivsVisaClientIpRegistration.Company;
                                obj.IsEnable = true;
                                obj.IpAddress = ip;
                                obj.SerialNum = res.First().SerialNum;
                                obj.CreatedDate = DateTime.Now; 
                                obj.UpdatedDate= new DateTime(1900, 01, 01);
                                list.Add(obj);
                            }
                            _context.IvsVisaClientIpRegistration.AddRange(list);
                            _context.SaveChanges();

                            TempData["Message"] = "Added Successfully!";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["Message"] = "IP Address is already Exist!";
                            return RedirectToAction(nameof(Index));
                        }                       
                    }
                    else
                    {
                        TempData["Message"] = "Please activate company/user then try.";
                        return RedirectToAction(nameof(Index));
                    }                   
                }
                TempData["Message"] = "IP Address is already Exist!";
                return RedirectToAction(nameof(Index));
            }
            return View(ivsVisaClientIpRegistration);
        }

        //_________________Code Added By sandeep Sharma on 24-05-2023______________________________//
        bool CheckIpExistEdit(string ip,int SerialNum)
        {
            bool isFlag = false;
            var IpExist = _context.IvsVisaClientIpRegistration.Where(x => x.IpAddress == ip && x.SerialNum!= SerialNum);
            if (IpExist.Count() > 0)
            {
                isFlag = true;
            }
            return isFlag;
        }

        //_________________Code Added By sandeep Sharma on 24-05-2023______________________________//
        bool CheckIpExist(string ip)
        {
            bool isFlag = false;
            var IpExist = _context.IvsVisaClientIpRegistration.Where(x => x.IpAddress == ip);
            if (IpExist.Count() > 0)
            {
                isFlag = true;
            }
            return isFlag;
        }

        // GET: IvsVisaClientIpRegistrations/Edit/5
        public async Task<IActionResult> Edit(int SerialNum, int Id)
        {
            if (SerialNum == 0)
            {
                return NotFound();
            }
            var ivsVisaClientIpRegistrationList = from clientIPAddr in _context.IvsVisaClientIpRegistration
                                                  join ud in _context.IvsUserDetails on clientIPAddr.SerialNum equals ud.SerialNum
                                                  where clientIPAddr.SerialNum == SerialNum
                                                  select new IvsVisaClientIpRegistration
                                                  {
                                                      Company = ud.Company,
                                                      IpAddress = clientIPAddr.IpAddress,
                                                      SerialNum = clientIPAddr.SerialNum,
                                                      Id = clientIPAddr.Id,
                                                      IsEnable = clientIPAddr.IsEnable
                                                  };
        
            var ivsVisaClientIpRegistration = await _context.IvsVisaClientIpRegistration.FindAsync(ivsVisaClientIpRegistrationList.First().Id);
            ivsVisaClientIpRegistration.IpAddress = string.Join("#", ivsVisaClientIpRegistrationList.Select(y => y.IpAddress).ToArray());
            ivsVisaClientIpRegistration.Company = ivsVisaClientIpRegistrationList.First().Company;
            ivsVisaClientIpRegistration.CompanyList = (from data in _context.IvsVisaCompanyMaster
                                 where data.IsEnable && !string.IsNullOrWhiteSpace(data.CompanyName)
                                 select new IvsVisaCompanyMaster
                                 {
                                     Id = data.Id,
                                     CompanyName = data.CompanyName
                                 }).ToList();

            if (ivsVisaClientIpRegistration == null)
            {
                return NotFound();
            }
            return View(ivsVisaClientIpRegistration);
        }

        // POST: IvsVisaClientIpRegistrations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SerialNum,[Bind("Id,SerialNum,IpAddress,IsEnable,Company")] IvsVisaClientIpRegistration ivsVisaClientIpRegistration)
        {

            //Get all hidden information          
            var ivc = _context.IvsVisaClientIpRegistration.Where(x => x.SerialNum == ivsVisaClientIpRegistration.SerialNum).FirstOrDefault();
            ivsVisaClientIpRegistration.UpdatedDate = ivc.UpdatedDate;
            ivsVisaClientIpRegistration.CreatedDate = ivc.CreatedDate;
            ivsVisaClientIpRegistration.IsEnable = ivc.IsEnable;

            if (SerialNum != ivsVisaClientIpRegistration.SerialNum)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (UpdateInformation(ivsVisaClientIpRegistration))
                {
                    TempData["Message"] = "Updated  Successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Message"] = "Not Updated !";
                    TempData["Message"] = "Record Not Updated or IP Address is already Exist!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(ivsVisaClientIpRegistration);
        }

        private bool IvsVisaClientIpRegistrationExists(int id)
        {
            return _context.IvsVisaClientIpRegistration.Any(e => e.Id == id);
        }
        private bool UpdateInformation(IvsVisaClientIpRegistration ivsVisaClientIpRegistration)
        {
            bool saved = false;
            try
            {
                var res = _context.IvsUserDetails.Where(x => x.Company == ivsVisaClientIpRegistration.Company && x.IsEnable == 1 && x.SerialNum == ivsVisaClientIpRegistration.SerialNum);
                if (res.Count() == 1)
                {
                    var IPArr = ivsVisaClientIpRegistration.IpAddress.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    bool isIpExist = false;
                    List<string> UpIPArr = new List<string> { };
                    UpIPArr = IPArr.ToList();
                    foreach (string ip in IPArr)
                    {                                              
                        isIpExist = CheckIpExistEdit(ip, ivsVisaClientIpRegistration.SerialNum); 
                        if (isIpExist == true)
                        {                          
                            saved = false;
                            UpIPArr.Remove(ip);
                        }
                        else
                        {                           
                            saved = true;
                        }                       
                    }
                    List<IvsVisaClientIpRegistration> list = new List<IvsVisaClientIpRegistration>();                   
                    foreach (string ips in UpIPArr)
                    {                        
                        IvsVisaClientIpRegistration obj = new IvsVisaClientIpRegistration();
                        obj.Company = ivsVisaClientIpRegistration.Company;
                        obj.IsEnable = true;
                        obj.IpAddress = ips;
                        obj.SerialNum = res.First().SerialNum;
                        obj.CreatedDate = ivsVisaClientIpRegistration.CreatedDate;
                        obj.UpdatedDate = DateTime.Now;
                        list.Add(obj);
                    }
                    var toDeleteList = _context.IvsVisaClientIpRegistration.Where(x => x.SerialNum == ivsVisaClientIpRegistration.SerialNum);
                    foreach (var CIP in toDeleteList)
                    {
                        _context.Entry(CIP).State = EntityState.Deleted;
                    }
                    _context.IvsVisaClientIpRegistration.AddRange(list);
                    _context.SaveChanges();
                }
                else
                {
                    saved = false;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
              return saved;
        }

        
        [HttpPost]
        public JsonResult CheckAndGetDataForCompany(string Company)
        {
            var allList = from cr in _context.IvsVisaClientIpRegistration
                          join ud in _context.IvsUserDetails on cr.SerialNum equals ud.SerialNum 
                          where ud.Company == Company
                          select new IvsVisaClientIpRegistration
                          {
                              SerialNum = cr.SerialNum,
                              Company = ud.Company,
                              IpAddress = cr.IpAddress
                          };
            
            return Json(allList);
        }
    }
}
