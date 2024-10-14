

using IVSource.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AllPageLogout.Filter
{
    public class CustemFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string SessionId = filterContext.HttpContext.Session.GetString("_Id");
            string _TerminalId = filterContext.HttpContext.Session.GetString("_TerminalId");


            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            var connectionString = configuration.GetConnectionString("localDB");


            string dbSessionId = string.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();


                SqlCommand objSqlCommand = new SqlCommand("select Session_ID from [ivsource].[ivs_user_terminal_id] where terminal_id='" + _TerminalId + "'", con);
                try
                {
                    dbSessionId = Convert.ToString(objSqlCommand.ExecuteScalar());
                }
                catch (Exception)
                {
                    con.Close();
                }
            }

            if (dbSessionId != SessionId)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{{ "controller", "Login" },
                                          { "action", "Members" }  });
            }


            base.OnActionExecuting(filterContext);
        }
    }





}
