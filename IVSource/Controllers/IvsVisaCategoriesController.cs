using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IVSource.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Text;
using IVSource.Controllers.Email;
using Microsoft.AspNetCore.Hosting;
using AllPageLogout.Filter;

namespace IVSource.Controllers
{
    [Authorize(Roles = MemberType.MemberAdmin, AuthenticationSchemes = MemberType.MemberAdminAuth)]
    public class IvsVisaCategoriesController : Controller
    {
        private readonly IVSourceContext _context;

        public IvsVisaCategoriesController(IVSourceContext context)
        {
            _context = context;
        }

        // GET: IvsVisaCategories
        public async Task<IActionResult> Index(string filter, string rowsToShow = "10", int page = 1, string sortExpression = "Priority")
        {
            var allList = _context.IvsVisaCategories
                .Join(_context.IvsVisaCountryTerritoryCities,
                m => m.CityId,
                n => n.CityId,
                (m, n) => new IvsVisaCategories1
                {
                    VisaCategory = m.VisaCategory,
                    IsEnable = m.IsEnable,
                    SerialNum = m.SerialNum,
                    CountryIso = m.CountryIso,
                    VisaCategoryId = m.VisaCategoryId,
                    CityName = n.CityName,
                    Priority = string.IsNullOrEmpty(m.Priority)? 0 : Convert.ToInt32(m.Priority)
                }).Where(o => o.CountryIso == TempData.Peek("CountryIso").ToString());

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allList = allList.Where(p => filter.ToLower() == "yes" ? (p.IsEnable == 1) : (filter.ToLower() == "no" ? (p.IsEnable == 0) :
                        (p.CityName.Contains(filter) || p.VisaCategory.Contains(filter)))).OrderBy(y => y.Priority);
            }
            else
                allList = allList.OrderBy(y => y.Priority);

            int count = allList.Count();
            ViewBag.Total = count;

            //var model = await PagingList<IvsVisaCategories>.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "SerialNum");
            var model = await PagingList.CreateAsync(allList, Convert.ToInt16(rowsToShow), page, sortExpression, "Priority");
            model.RouteValue = new RouteValueDictionary { { "filter", filter }, { "rowsToShow", rowsToShow } };
            return View(model);
        }

        // GET: IvsVisaCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ivsVisaCategories = await _context.IvsVisaCategories.FindAsync(id);

            if (ivsVisaCategories == null)
            {
                return NotFound();
            }
            var model = from m in new List<IvsVisaCategories> { ivsVisaCategories }
                        join n in _context.IvsVisaCountryTerritoryCities on m.CityId equals n.CityId
                        where m.CountryIso == TempData.Peek("CountryIso").ToString()
                        select new IvsVisaCategories
                        {
                            SerialNum = m.SerialNum,
                            VisaCategoryCode = m.VisaCategoryId,
                            VisaCategoryId = m.VisaCategoryId,
                            CityId = m.CityId,
                            VisaCategoryInformation = m.VisaCategoryInformation,
                            VisaCategoryInformationVisaProcedure = m.VisaCategoryInformationVisaProcedure,
                            VisaCategoryInformationDocumentsRequired = m.VisaCategoryInformationDocumentsRequired,
                            VisaCategoryInformationProcessingTime = m.VisaCategoryInformationProcessingTime,
                            VisaCategoryNotes = m.VisaCategoryNotes,
                            VisaCategoryRequirements = m.VisaCategoryRequirements,
                            IsEnable = m.IsEnable,
                            Priority = m.Priority,
                            VisaCategory = m.VisaCategory,
                            CityName = n.CityName
                        };
            return View(model.First());
        }

        // POST: IvsVisaCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string CountryName, [Bind("SerialNum,CountryIso,VisaCategoryId,CityId,VisaCategory,VisaCategoryCode,VisaCategoryInformation,VisaCategoryInformationVisaProcedure,VisaCategoryInformationDocumentsRequired,VisaCategoryInformationProcessingTime,VisaCategoryNotes,VisaCategoryRequirements,IsEnable,Priority,CreatedDate,ModifiedDate")] IvsVisaCategories ivsVisaCategories)
        {
            if (id != ivsVisaCategories.VisaCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ivsVisaCategories.ModifiedDate = DateTime.Now;
                    _context.Update(ivsVisaCategories);
                    _context.Entry(ivsVisaCategories).Property(x => x.SerialNum).IsModified = false;
                    _context.Entry(ivsVisaCategories).Property(x => x.CountryIso).IsModified = false;
                    _context.Entry(ivsVisaCategories).Property(x => x.VisaCategoryId).IsModified = false;
                    _context.Entry(ivsVisaCategories).Property(x => x.CityId).IsModified = false;
                    _context.Entry(ivsVisaCategories).Property(x => x.VisaCategoryCode).IsModified = false;
                    _context.Entry(ivsVisaCategories).Property(x => x.CreatedDate).IsModified = false;

                    var upCountry = _context.IvsVisaCountries.Where(y => y.CountryName == CountryName).First();
                    upCountry.IsUpdated = 1;
                    upCountry.ModifiedDate = DateTime.Now;
                    _context.Update(upCountry);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IvsVisaCategoriesExists(ivsVisaCategories.VisaCategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(ivsVisaCategories);
        }

        private bool IvsVisaCategoriesExists(string id)
        {
            return _context.IvsVisaCategories.Any(e => e.VisaCategoryId == id);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(IvsVisaCategories obj)
        {
            string country = TempData.Peek("CountryName").ToString();
            var countryIso = _context.IvsVisaCountries.FirstOrDefault(x => x.CountryName == country);

            var ran = RandomString(8);
            var vid = ran;

            obj.CountryIso = countryIso.CountryIso;
            obj.VisaCategoryId = countryIso.CountryIso + vid;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;

            _context.IvsVisaCategories.Add(obj);
            _context.SaveChanges();
            TempData["Message"] = "Inserted Successfully!";
            return RedirectToAction("../IvsVisaCategories/Index");
        }

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
    //[Authorize(Roles = MemberType.MemberUser, AuthenticationSchemes = MemberType.MemberUserAuth)]
    public class IvsVisaCategories_UserController : Controller
    {
        private readonly IVSourceContext _context;
        public IConfiguration _Configuration { get; }
        private readonly IHostingEnvironment _HostEnvironment;

        public IvsVisaCategories_UserController(IVSourceContext context, IHostingEnvironment HostEnvironment, IConfiguration iconfiguration)
        {
            _context = context;
            _HostEnvironment = HostEnvironment;
            _Configuration = iconfiguration;
        }
        [CustemFilter]
        public IActionResult VisaNoteFees(string countryName, string countryFlag)
        {
            string domain = Request.Scheme + Uri.SchemeDelimiter + Request.Host.Value;

            var countryIso = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.CountryIso).First();

            var checkVisaInformationData = (from visa in _context.IvsVisaInformation select visa.CountryIso).Contains(countryIso);

            var checkVisaCategoryData = (from cate in _context.IvsVisaCategories select cate.CountryIso).Contains(countryIso);

            var checkVisaCategoryOptionsData = (from opti in _context.IvsVisaCategoriesOptions select opti.CountryIso).Contains(countryIso);

            var checkVisaCategoryFormsData = (from form in _context.IvsVisaCategoriesForms select form.CountryIso).Contains(countryIso);

            
            //Added by Hitesh behalf of Sandeep on 18072023_1511
            var listPages = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
            var listLinks = _context.IvsVisaPages.Where(x => x.IsEnable == true && x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();
            ViewBag.PagesOurContent = listPages.ToList();
            ViewBag.LinksOurContent = listLinks.ToList();
            ViewBag.QuickContactInfo = quickcontactinfo;

            List<HomePage> allData = new List<HomePage>();

            if (checkVisaInformationData == true)
            {
                //---------Code Changed by Sandeep Sharma on 08-06-2023---------------//
                var sr = from country in _context.IvsVisaCountries
                         join us in _context.IvsVisaInformation on country.CountryIso equals us.CountryIso
                         join territ in _context.IvsVisaCountryTerritoryCities on us.CityId equals territ.CityId
                         where country.CountryName.Contains(countryName)
                         orderby string.IsNullOrWhiteSpace(us.Priority) ? 0 : Int16.Parse(us.Priority) ascending
                         select new IvsVisaInformation
                         {
                             CityId = territ.CityId,
                             CityName = territ.CityName
                         };

                foreach (var item in sr.ToList())
                {
                    bool chkIfEmpty = false;
                    var cityDetails = (from country in _context.IvsVisaCountries
                                       join us in _context.IvsVisaInformation on country.CountryIso equals us.CountryIso
                                       join category in _context.IvsVisaCategories on us.CountryIso equals category.CountryIso
                                       where country.CountryName == countryName
                                       && category.IsEnable == 1
                                       && us.CityId == item.CityId
                                       && category.CityId == item.CityId
                                       && us.IsEnable == 1
                                       orderby category.Priority ascending
                                       select new IvsVisaNoteFees
                                       {
                                           VisaInformation = us.VisaInformation,
                                           VisaGeneralInformation = us.VisaGeneralInformation,
                                           VisaCategory = category.VisaCategory,
                                           VisaCategoryInformation = category.VisaCategoryInformation,
                                           VisaCategoryInformationVisaProcedure = category.VisaCategoryInformationVisaProcedure,
                                           VisaCategoryInformationDocumentsRequired = category.VisaCategoryInformationDocumentsRequired,
                                           VisaCategoryInformationProcessingTime = category.VisaCategoryInformationProcessingTime,
                                           VisaCategoryNotes = category.VisaCategoryNotes,
                                           VisaCategoryRequirements = category.VisaCategoryRequirements,
                                           VisaCategoryId = category.VisaCategoryId,
                                           //VisaCategoryCode = category.VisaCategoryCode,
                                           bCountryFlag = countryFlag,
                                           bCountryName = countryName,
                                           Priority = string.IsNullOrWhiteSpace(category.Priority) ? 0 : Int16.Parse(category.Priority),
                                           CityId = category.CityId,
                                           IsEnable = category.IsEnable
                                       }).Distinct();
                     

                    if (cityDetails.Count()==0)
                    { 
                        var CheckPrior = (from info in _context.IvsVisaInformation where info.CityId.Contains(item.CityId) && info.CountryIso.Contains(countryIso) select info.Priority).First();
                        var ChkSerialNum = (from info in _context.IvsVisaInformation where info.CityId.Contains(item.CityId) && info.CountryIso.Contains(countryIso) select info.SerialNum).First();
                        chkIfEmpty = true;

                        cityDetails = (from country in _context.IvsVisaCountries
                                       join us in _context.IvsVisaInformation on country.CountryIso equals us.CountryIso                                      
                                       where country.CountryName == countryName                                     
                                       && us.CityId == item.CityId                                      
                                       && us.IsEnable == 1                                     
                                       select new IvsVisaNoteFees
                                       {
                                           VisaInformation = us.VisaInformation,
                                           VisaGeneralInformation = us.VisaGeneralInformation,
                                           VisaCategory = "",
                                           VisaCategoryInformation = "",
                                           VisaCategoryInformationVisaProcedure = "",
                                           VisaCategoryInformationDocumentsRequired = "",
                                           VisaCategoryInformationProcessingTime = "",
                                           VisaCategoryNotes = "",
                                           VisaCategoryRequirements = "",
                                           VisaCategoryId = "",
                                           VisaCategoryCode = "",
                                           bCountryFlag = countryFlag,
                                           bCountryName = countryName,
                                           Priority = string.IsNullOrWhiteSpace(CheckPrior) ? 0 : Int16.Parse(CheckPrior),                                          
                                           CityId = item.CityId,
                                           IsEnable = 1,
                                           VisaCategoryOption ="",
                                           VisaCategoryOptionAmountInr = "",
                                           VisaCategoryModifiedDate = DateTime.Now,                                         
                                           VisaCategoryOptionAmountOther = "",
                                           VisaCategoryForm="",
                                           VisaCategoryFormPath="",
                                           CountryIso= countryIso,
                                           SerialNum=Convert.ToInt32(ChkSerialNum)
                                       }).Distinct();

                    }

                    var prior = cityDetails.FirstOrDefault(x => x.CityId == item.CityId)?.Priority;                  



                    var cityDetailsInformation = (from info in cityDetails
                                                  select new IvsVisaNoteFees
                                                  {
                                                      VisaInformation = info.VisaInformation,
                                                      VisaGeneralInformation = info.VisaGeneralInformation,
                                                  }).Distinct();

                    var cityCategoryDetails = (from info in cityDetails
                                               where !string.IsNullOrWhiteSpace(info.VisaCategory)
                                               orderby info.Priority ascending
                                               select new IvsVisaNoteFees
                                               {
                                                   VisaCategory = info.VisaCategory,
                                                   VisaCategoryInformation = info.VisaCategoryInformation,
                                                   VisaCategoryInformationVisaProcedure = info.VisaCategoryInformationVisaProcedure,
                                                   VisaCategoryInformationDocumentsRequired = info.VisaCategoryInformationDocumentsRequired,
                                                   VisaCategoryInformationProcessingTime = info.VisaCategoryInformationProcessingTime,
                                                   VisaCategoryNotes = info.VisaCategoryNotes,
                                                   VisaCategoryRequirements = info.VisaCategoryRequirements,
                                                   VisaCategoryCode = info.VisaCategoryCode,
                                                   VisaCategoryId = info.VisaCategoryId
                                               }).Distinct();


                    if (chkIfEmpty)
                    {
                        cityCategoryDetails = (from info in cityDetails 
                                               select new IvsVisaNoteFees
                                               {
                                                   VisaCategory = info.VisaCategory,
                                                   VisaCategoryInformation = info.VisaCategoryInformation,
                                                   VisaCategoryInformationVisaProcedure = info.VisaCategoryInformationVisaProcedure,
                                                   VisaCategoryInformationDocumentsRequired = info.VisaCategoryInformationDocumentsRequired,
                                                   VisaCategoryInformationProcessingTime = info.VisaCategoryInformationProcessingTime,
                                                   VisaCategoryNotes = info.VisaCategoryNotes,
                                                   VisaCategoryRequirements = info.VisaCategoryRequirements,
                                                   VisaCategoryCode = info.VisaCategoryCode,
                                                   VisaCategoryId = info.VisaCategoryId
                                               }).Distinct();
                    } 

                    var cityCategoryFees = from info in cityDetails
                                           join categoryOptions in _context.IvsVisaCategoriesOptions on info.VisaCategoryId equals categoryOptions.VisaCategoryCode
                                           where !string.IsNullOrWhiteSpace(categoryOptions.VisaCategoryOption) && categoryOptions.IsEnable == 1
                                           select new IvsVisaNoteFees
                                           {
                                               VisaCategoryOption = categoryOptions.VisaCategoryOption,
                                               VisaCategoryOptionAmountInr = categoryOptions.VisaCategoryOptionAmountInr,
                                               VisaCategoryModifiedDate = categoryOptions.ModifiedDate,
                                               SerialNum = categoryOptions.SerialNum,
                                               VisaCategoryOptionAmountOther = categoryOptions.VisaCategoryOptionAmountOther,
                                               VisaCategoryCode = categoryOptions.VisaCategoryCode,
                                               VisaCategoryId = info.VisaCategoryId,
                                               IsEnable = categoryOptions.IsEnable
                                           };                   

                        var cityFormDetails = (from info in cityDetails
                                           join form in _context.IvsVisaCategoriesForms on info.VisaCategoryId equals form.VisaCategoryCode
                                           where form.CityId == info.CityId
                                           orderby info.Priority ascending
                                           select new IvsVisaNoteFees
                                           {
                                               VisaCategoryForm = form.Form,
                                               VisaCategoryFormPath = domain + "/" + _Configuration["FileName"].ToString() + form.FormPath,
                                               VisaCategoryCode = form.VisaCategoryCode,
                                               VisaCategoryId = info.VisaCategoryId,
                                               IsEnable = form.IsEnable
                                           });                    

                    HomePage model = new HomePage();
                    model.CityDetails = cityDetails.ToList();
                    model.CityDetailsInformation = cityDetailsInformation.ToList();
                    model.CityCategoryDetails = cityCategoryDetails.ToList();
                    if (!chkIfEmpty) {
                        model.CityCategoryFees = cityCategoryFees.ToList();
                        model.CityFormDetails = cityFormDetails.ToList();
                    }                                                 
                    model.bCountryName = countryName;
                    model.bCountryFlag = countryFlag;
                    model.CityName = item.CityName;
                    model.Priority = prior != null ? Convert.ToInt32(prior) : 0;                   
                    allData.Add(model);
                   
                }                           
                return View(allData);
            }
            else
            {
                IvsVisaNoteFees obj = new IvsVisaNoteFees();
                obj.bCountryFlag = countryFlag;
                obj.bCountryName = countryName;
                return View("VisaNoteAndFees", obj);
            }
        }

        
        public IActionResult VisaNoteFeesr(string countryName, string countryFlag)
        {
            var countryr = (from info in _context.IvsVisaCountries where info.CountryName.Contains(countryName) select info.Country).First();

            countryr = countryr.Replace(System.Environment.NewLine, string.Empty);

            string domain = Request.Scheme + Uri.SchemeDelimiter + Request.Host.Value;

            var fourCountries = (from info in _context.IvsVisaSubPages where info.PageId == 12 && info.IsEnable == true select info.Title).Contains(countryr);

            var quickcontactinfo = _context.IvsVisaQuickContactInfo.ToList();

            List<HomePage> allData = new List<HomePage>();

            if (fourCountries == true)
            {
                var sr = from country in _context.IvsVisaCountries
                         join us in _context.IvsVisaInformation on country.CountryIso equals us.CountryIso
                         join territ in _context.IvsVisaCountryTerritoryCities on us.CityId equals territ.CityId
                         where country.CountryName.Contains(countryName)
                         orderby us.Priority
                         select new IvsVisaInformation
                         {
                             CityId = territ.CityId,
                             CityName = territ.CityName
                         };

                foreach (var item in sr.ToList())
                {
                    var cityDetails = (from country in _context.IvsVisaCountries
                                       join us in _context.IvsVisaInformation on country.CountryIso equals us.CountryIso
                                       join category in _context.IvsVisaCategories on country.CountryIso equals category.CountryIso
                                       where country.CountryName == countryName
                                       && category.IsEnable == 1
                                       && us.CityId == item.CityId
                                       && category.CityId == item.CityId
                                       && us.IsEnable == 1
                                       orderby category.Priority ascending
                                       select new IvsVisaNoteFees
                                       {
                                           VisaInformation = us.VisaInformation,
                                           VisaGeneralInformation = us.VisaGeneralInformation,
                                           VisaCategory = category.VisaCategory,
                                           VisaCategoryInformation = category.VisaCategoryInformation,
                                           VisaCategoryInformationVisaProcedure = category.VisaCategoryInformationVisaProcedure,
                                           VisaCategoryInformationDocumentsRequired = category.VisaCategoryInformationDocumentsRequired,
                                           VisaCategoryInformationProcessingTime = category.VisaCategoryInformationProcessingTime,
                                           VisaCategoryNotes = category.VisaCategoryNotes,
                                           VisaCategoryRequirements = category.VisaCategoryRequirements,
                                           VisaCategoryId = category.VisaCategoryId,
                                           //VisaCategoryCode = category.VisaCategoryCode,
                                           bCountryFlag = countryFlag,
                                           bCountryName = countryName,
                                           Priority = string.IsNullOrWhiteSpace(category.Priority) ? 0 : Int16.Parse(category.Priority),
                                           CityId = category.CityId,
                                           IsEnable = category.IsEnable
                                       }).Distinct();

                    var cityDetailsInformation = (from info in cityDetails
                                                  select new IvsVisaNoteFees
                                                  {
                                                      VisaInformation = info.VisaInformation,
                                                      VisaGeneralInformation = info.VisaGeneralInformation,
                                                  }).Distinct();

                    var cityCategoryDetails = (from info in cityDetails
                                               where !string.IsNullOrWhiteSpace(info.VisaCategory)
                                               orderby info.Priority ascending
                                               select new IvsVisaNoteFees
                                               {
                                                   VisaCategory = info.VisaCategory,
                                                   VisaCategoryInformation = info.VisaCategoryInformation,
                                                   VisaCategoryInformationVisaProcedure = info.VisaCategoryInformationVisaProcedure,
                                                   VisaCategoryInformationDocumentsRequired = info.VisaCategoryInformationDocumentsRequired,
                                                   VisaCategoryInformationProcessingTime = info.VisaCategoryInformationProcessingTime,
                                                   VisaCategoryNotes = info.VisaCategoryNotes,
                                                   VisaCategoryRequirements = info.VisaCategoryRequirements,
                                                   VisaCategoryCode = info.VisaCategoryCode,
                                                   VisaCategoryId = info.VisaCategoryId
                                               }).Distinct();

                    var cityCategoryFees = from info in cityDetails
                                           join categoryOptions in _context.IvsVisaCategoriesOptions on info.VisaCategoryId equals categoryOptions.VisaCategoryCode
                                           where !string.IsNullOrWhiteSpace(categoryOptions.VisaCategoryOption) && categoryOptions.IsEnable == 1
                                           select new IvsVisaNoteFees
                                           {
                                               VisaCategoryOption = categoryOptions.VisaCategoryOption,
                                               VisaCategoryOptionAmountInr = categoryOptions.VisaCategoryOptionAmountInr,
                                               VisaCategoryModifiedDate = categoryOptions.ModifiedDate,
                                               SerialNum = categoryOptions.SerialNum,
                                               VisaCategoryOptionAmountOther = categoryOptions.VisaCategoryOptionAmountOther,
                                               VisaCategoryCode = categoryOptions.VisaCategoryCode,
                                               VisaCategoryId = info.VisaCategoryId,
                                               IsEnable = categoryOptions.IsEnable
                                           };

                    var cityFormDetails = (from info in cityDetails
                                           join form in _context.IvsVisaCategoriesForms on info.VisaCategoryId equals form.VisaCategoryCode
                                           where form.CityId == info.CityId
                                           orderby info.Priority ascending
                                           select new IvsVisaNoteFees
                                           {
                                               VisaCategoryForm = form.Form,
                                               VisaCategoryFormPath = domain + "/" + _Configuration["FileName"].ToString() + form.FormPath,
                                               VisaCategoryCode = form.VisaCategoryCode,
                                               VisaCategoryId = info.VisaCategoryId,
                                               IsEnable = form.IsEnable
                                           });

                    var listPages = _context.IvsVisaPages.Where(x => x.Type == HomePageType.Pages).OrderByDescending(y => y.CreatedDate);
                    var listLinks = _context.IvsVisaPages.Where(x => x.Type == HomePageType.Links).OrderByDescending(y => y.CreatedDate);
                    HomePage model = new HomePage();
                    model.CityDetails = cityDetails.ToList();
                    model.CityDetailsInformation = cityDetailsInformation.ToList();
                    model.CityCategoryDetails = cityCategoryDetails.ToList();
                    model.CityCategoryFees = cityCategoryFees.ToList();
                    model.CityFormDetails = cityFormDetails.ToList();
                    model.bCountryName = countryName;
                    model.bCountryFlag = countryFlag;
                    model.CityName = item.CityName;
                    ViewBag.PagesOurContent = listPages.ToList();
                    ViewBag.LinksOurContent = listLinks.ToList();
                    ViewBag.QuickContactInfo = quickcontactinfo;

                    allData.Add(model);
                }
                return View(allData);
            }
            else
            {
                TempData["Message"] = "To find the other country details you need to register / login yourself here.";
                return RedirectToAction("Members", "Login");
            }
        }

        [HttpPost]
        public bool SendEmailVisaInfo(string senderEmail, string toEmail, string ccEmail, string subject, string remarks, string catData)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                sb.Append(catData);
                if(!string.IsNullOrWhiteSpace(remarks))
                    sb.Append("<br/><b>Remarks</b> : " + remarks);
                sb.Append("<br/><br/>Thanks & Regards, <br>IVSource Team");

                EMailr em = new EMailr(senderEmail);
                bool retVal = em.SendEmail(sb.ToString(), subject, toEmail, ccEmail, false);
                return retVal;
            }
            catch
            {
                return false;
            }
        }
    }
}
