using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Text;

namespace IVSource.Controllers.Api
{
    public class APILogs : Controller
    {
        //Email objMail = new EMail();
        public void ErrLog(String ex)
        {
            try
            {
                string lstrErrFile;
                string lstrErrDir;
                FileStream tmpFile = null;
                //lstrErrDir = ConfigurableInfo.logPath.ToString() + "\\Log";// @Application.StartupPath.ToString() + "\\Log";
                //lstrErrDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //lstrErrDir = "C:\\Logs\\";//"//172.18.80.23/publish/Logs\";
                lstrErrDir = "\\\\172.18.80.23\\publish\\Logs\\";
                lstrErrFile = lstrErrDir + "\\" + System.DateTime.Now.ToString("dd-MMM-yyy") + "_ErrLog.log";
                if (!(Directory.Exists(lstrErrDir)))
                {
                    Directory.CreateDirectory(lstrErrDir);
                }
                //File.Exists(lstrErrFile)
                if (System.IO.File.Exists(lstrErrFile))
                {
                    tmpFile = new FileStream(lstrErrFile, FileMode.Append);
                }
                else
                {
                    tmpFile = new FileStream(lstrErrFile, FileMode.Create);
                }

                //string strErrorMsg = ex.Message.ToString();
                //string StackTrace = Convert.ToString(ex.StackTrace);
                //string Source = Convert.ToString(ex.Source);
                //string InnerException = Convert.ToString(ex.InnerException);
                //string pstrErrMsg = strErrorMsg + "\nStackTrace" + StackTrace + "\nSource  " + Source + "\nInnerException " + InnerException;

                string pstrErrMsg = ex;

                StreamWriter sw = new StreamWriter(tmpFile);
                sw.WriteLine(" DT:" + System.DateTime.Now.ToString("dd-MMM-yyy hh:mm:ss tt ") + " Err/Msg: " + pstrErrMsg);
                sw.Close();
                tmpFile.Close();
                sw.Dispose();
                tmpFile.Dispose();

            }
            catch (Exception ex1)
            {
                //objMail.SendMail(ex1);
                ErrLog(ex1.ToString());
            }

        }
        public void ResponseLog(string ex)
        {
            try
            {
                string lstrErrFile;
                string lstrErrDir;
                FileStream tmpFile = null;
                //lstrErrDir = ConfigurableInfo.logPath + "\\Log";// @Application.StartupPath.ToString() + "\\Log";
                lstrErrDir = "\\\\172.18.80.23\\publish\\Logs\\";
                lstrErrFile = lstrErrDir + "\\" + System.DateTime.Now.ToString("dd-MMM-yyy") + "_ResponseLog.log";
                if (!(Directory.Exists(lstrErrDir)))
                {
                    Directory.CreateDirectory(lstrErrDir);
                }

                //if (File.Exists(lstrErrFile))
                if(System.IO.File.Exists(lstrErrFile))
                {
                    tmpFile = new FileStream(lstrErrFile, FileMode.Append);
                }
                else
                {
                    tmpFile = new FileStream(lstrErrFile, FileMode.Create);
                }

                string strErrorMsg = ex;
                string StackTrace = "";
                string Source = "";
                string InnerException = "";


                string pstrErrMsg = strErrorMsg + "\nStackTrace" + StackTrace + "\nSource  " + Source + "\nInnerException " + InnerException;

                StreamWriter sw = new StreamWriter(tmpFile);
                sw.WriteLine(" DT:" + System.DateTime.Now.ToString("dd-MMM-yyy hh:mm:ss tt ") + " Err/Msg: " + pstrErrMsg);
                sw.Close();
                tmpFile.Close();
                sw.Dispose();
                tmpFile.Dispose();
            }
            catch (Exception ex1)
            {
                //objMail.SendMail(ex1);
                ErrLog(ex1.ToString());
            }
        }
    }
}
