using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IVSource.Controllers.Email;
using IVSource.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IVSource.Controllers
{
    public class SubscribeToUsController : Controller
    {
        private readonly IVSourceContext _context;

        public SubscribeToUsController(IVSourceContext context)
        {
            _context = context;
        }

        //[Route("get-captcha-image")]
        //public IActionResult GetCaptchaImage()
        //{
        //    int width = 100;
        //    int height = 36;
        //    var captchaCode = Captcha.GenerateCaptchaCode();
        //    var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
        //    HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
        //    Stream s = new MemoryStream(result.CaptchaByteData);
        //    return new FileStreamResult(s, "image/png");
        //}

        [HttpPost]
        //public string SendEmailSubscribeToUs(string ccEmail, string subject, string Years, string CaptchaCode, IvsVisaSubscribeToUs obj)
        public string SendEmailSubscribeToUs(string ccEmail, string subject, string Years, string CaptchaCode, IvsVisaSubscribeToUs obj)
        {
            string retVal = string.Empty;

            try
            {
                retVal = "PLEASE FILL THE REQUIRED FIELD.";

                if (ModelState.IsValid)
                {
                    if (!Captcha.ValidateCaptchaCode(CaptchaCode, HttpContext))
                    {
                        retVal = "Entered code is incorrect";
                        return retVal;
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("", typeof(string)),
                        new DataColumn("", typeof(string))
                        });

                        dt.Rows.Add("Name                             :", obj.Name);
                        dt.Rows.Add("Designation                      :", obj.Designation);
                        dt.Rows.Add("Company                          :", obj.Company);
                        dt.Rows.Add("Full Address                     :", obj.Full_Address);
                        dt.Rows.Add("Phone Number                     :", obj.Phone_Number);
                        dt.Rows.Add("Email Id                         :", obj.Email_Address);
                        dt.Rows.Add("DD/ Online Transaction Number    :", obj.D_D_Number);
                        dt.Rows.Add("DD Date                          :", obj.Date);
                        dt.Rows.Add("DD Amount                        :", obj.Amount);
                        dt.Rows.Add("Feedback/Query                   :", obj.Remarks);

                        StringBuilder sb = new StringBuilder();

                        //sb.Append("Subject : Registration Application | IV Source");

                        sb.Append(Years);

                        sb.Append("<br><br><table cellpadding='5' cellspacing='0' style='font-size: 8pt; font-family: Arial'>");

                        sb.Append("<tr>");

                        //foreach (DataColumn column in dt.Columns)
                        //{
                        //    sb.Append("<th style='width: 50px; background-color: #3366ff; border: 1px solid #000'>" + column.ColumnName + "</th>");
                        //}

                        sb.Append("</tr>");

                        foreach (DataRow row in dt.Rows)
                        {
                            sb.Append("<tr>");

                            foreach (DataColumn column in dt.Columns)
                            {
                                sb.Append("<td style='width: 300px;'>" + row[column.ColumnName].ToString() + "</td>");
                            }

                            sb.Append("</tr>");
                        }

                        sb.Append("</table>");

                        sb.Append("<br/><br/>Thanks & Regards, <br>IVSource Team");

                        EMail em = new EMail();

                        if (em.SendEmail(sb.ToString(), subject, obj.Email_Address, ccEmail, false))
                        {
                            retVal = "Mail Sent successfully";
                        }
                        else
                            retVal = "MailBox Unavailable. Please enter valid email.";

                        return retVal;
                    }
                }
                else
                {
                    return retVal;
                }                   
            }
            catch
            {
                return retVal;
            }
        }
    }
}