using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using IVSource.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IVSource.Controllers
{
    public class LogErrorController : Controller
    {
        public static string conStr;

        public IActionResult LogInDB(IExceptionHandlerPathFeature error)
        {
            return View();
        }
        public void Add(IExceptionHandlerPathFeature error)
        {
        }
    }
}