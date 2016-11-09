using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using WebService.CommonLibrary;
using System.Text;
using System.IO;
using System.Net;

namespace WebService.CommonLibrary
{
    public class HygeiaComUtility
    {

        #region "< Public Method >"

        /// <summary>
        /// Writes the system log.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="logSource">The log source.</param>
        /// <param name="logMessage">The log message.</param>

        public static void WriteClientLog(HygeiaEnum.LogType logType, string logSource, string logMessage)
        {
            StringBuilder sbLog = new StringBuilder();

            if (TextLog.LogFileDirectory.Length == 0)
            {
                //修改Log文件保存路径 20141202 ZAM 
                string physicalPath = System.Web.HttpContext.Current.Request.PhysicalPath;
                int index = physicalPath.LastIndexOf('\\');
                string logdir = physicalPath.Substring(0, index);
                //string dir = Path.Combine(HygeiaConst.CLIENT_LOG_DIR, GetTerminalID());
                string dir = Path.Combine(logdir + "\\Log", GetTerminalID());
                TextLog.Initialize(dir, HygeiaConst.CLIENT_LOG_BASENAME, HygeiaConst.CLIENT_LOG_RETRY, HygeiaConst.CLIENT_LOG_SAVEDAYS);
                TextLog.LogSeparator = HygeiaConst.CLIENT_LOG_SEPARATOR;
            }

            string strLogType = string.Empty;
            switch (logType)
            {
                case HygeiaEnum.LogType.TraceLog:
                    strLogType = "TRC";
                    break;
                case HygeiaEnum.LogType.InformationLog:
                    strLogType = "INF";
                    break;
                case HygeiaEnum.LogType.ErrorLog:
                    strLogType = "ERR";
                    break;
                default:
                    strLogType = "TRC";
                    break;
            }

            //Output
            //ZAM visitor tracking 2014-12-31 update log visitor name 2015-1-14
            string visitorIP = "";
            string machineName = "";
            visitorIP = HttpContext.Current.Request.UserHostAddress;
            System.Net.IPHostEntry host = new System.Net.IPHostEntry();
            host = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.UserHostAddress);
            machineName = host.HostName;

            sbLog.Append(SetSpace(machineName, 10) + HygeiaConst.CLIENT_LOG_SEPARATOR);
            sbLog.Append(SetSpace(visitorIP, 16) + HygeiaConst.CLIENT_LOG_SEPARATOR);
            sbLog.Append(SetSpace(strLogType, 3) + HygeiaConst.CLIENT_LOG_SEPARATOR);
            // string logSource display completed ZAM 2014-12-25 (iMax 30 to 60)
            sbLog.Append(SetSpace(logSource, 60) + HygeiaConst.CLIENT_LOG_SEPARATOR);
            //sbLog.Append(logMessage);
            sbLog.Append(logMessage.Trim());
            TextLog.WriteLog(sbLog.ToString());

        }

        //public static void WriteClientLog(HygeiaEnum.LogType logType, string logSource, string logMessage, int SortNo)
        //{
        //    StringBuilder sbLog = new StringBuilder();

        //    if (TextLog.LogFileDirectory.Length == 0)
        //    {
        //        //修改Log文件保存路径 20141202 ZAM 
        //        string physicalPath = System.Web.HttpContext.Current.Request.PhysicalPath;
        //        int index = physicalPath.LastIndexOf('\\');
        //        string logdir = physicalPath.Substring(0, index);
        //        //string dir = Path.Combine(HygeiaConst.CLIENT_LOG_DIR, GetTerminalID());
        //        string dir = Path.Combine(logdir + "\\Log", GetTerminalID());
        //        TextLog.Initialize(dir, HygeiaConst.CLIENT_LOG_BASENAME+SortNo.ToString(), HygeiaConst.CLIENT_LOG_RETRY, HygeiaConst.CLIENT_LOG_SAVEDAYS);
        //        TextLog.LogSeparator = HygeiaConst.CLIENT_LOG_SEPARATOR;
        //    }

        //    string strLogType = string.Empty;
        //    switch (logType)
        //    {
        //        case HygeiaEnum.LogType.TraceLog:
        //            strLogType = "TRC";
        //            break;
        //        case HygeiaEnum.LogType.InformationLog:
        //            strLogType = "INF";
        //            break;
        //        case HygeiaEnum.LogType.ErrorLog:
        //            strLogType = "ERR";
        //            break;
        //        default:
        //            strLogType = "TRC";
        //            break;
        //    }

        //    //Output
        //    //ZAM visitor tracking 2014-12-31 update log visitor name 2015-1-14
        //    string visitorIP = "";
        //    string machineName = "";
        //    visitorIP = HttpContext.Current.Request.UserHostAddress;
        //    System.Net.IPHostEntry host = new System.Net.IPHostEntry();
        //    host = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.UserHostAddress);
        //    machineName = host.HostName;

        //    sbLog.Append(SetSpace(machineName, 10) + HygeiaConst.CLIENT_LOG_SEPARATOR);
        //    sbLog.Append(SetSpace(visitorIP, 16) + HygeiaConst.CLIENT_LOG_SEPARATOR);
        //    sbLog.Append(SetSpace(strLogType, 3) + HygeiaConst.CLIENT_LOG_SEPARATOR);
        //    // string logSource display completed ZAM 2014-12-25 (iMax 30 to 60)
        //    sbLog.Append(SetSpace(logSource, 60) + HygeiaConst.CLIENT_LOG_SEPARATOR);
        //    //sbLog.Append(logMessage);
        //    sbLog.Append(logMessage.Trim());
        //    TextLog.WriteLog(sbLog.ToString());

        //}


        /// <summary>
        /// Gets the terminal ID.
        /// </summary>
        /// <returns></returns>
        public static string GetTerminalID()
        {
            return Environment.MachineName.ToUpper();
        }

        /// <summary>
        /// Gets the terminal IP.
        /// </summary>
        /// <returns></returns>
        public static string GetTerminalIP()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            return ipHostInfo.AddressList[0].ToString();
        }

        /// <summary>
        /// Changes the date format.
        /// </summary>
        /// <returns></returns>
        public static string ChgDateFormat(string DateString)
        {
            string strDate = DateString;
            string strDateDelimiter = "/";
            try
            {
                switch (DateString.Length)
                {
                    case 8:
                        //YYYYMMDD → e.g)YYYY/MM/DD
                        strDate = DateString.Substring(0, 4) + strDateDelimiter + DateString.Substring(4, 2) + strDateDelimiter + DateString.Substring(6, 2);

                        break;
                    case 10:
                        //e.g)YYYY/MM/DD → YYYYMMDD
                        strDate = DateString.Replace(strDateDelimiter, string.Empty);

                        break;
                    default:

                        break;
                }

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "", "ChgDateFormat！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;

            }
            return strDate;
        }

        /// <summary>
        /// Changes the time format.
        /// </summary>
        /// <returns></returns>
        public static string ChgTimeFormat(string TimeString)
        {
            string strTime = TimeString;
            string strTimeDelimiter = ":";
            try
            {
                if (TimeString.Split(Convert.ToChar(strTimeDelimiter)).Length > 1)
                {
                    //hh:mm → hhmm
                    strTime = TimeString.Replace(strTimeDelimiter, string.Empty);
                }
                else
                {
                    //hhmm → hh:mm
                    // 20141202 ZAM
                    string tempstr = ("0000" + TimeString);
                    strTime = tempstr.Substring(tempstr.Length - 4, 4);
                    strTime = strTime.Substring(0, 2) + strTimeDelimiter + strTime.Substring(2, 2);
                }

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "", "ChgTimeFormat！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
            return strTime;
        }

        /// <summary>
        /// Gets the startup path.
        /// </summary>
        /// <returns></returns>
        public static string GetStartupPath()
        {
            return System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        }
        #endregion

        #region "< Private Method >"

        /// <summary>
        /// Sets the space.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="iMax">The i max.</param>
        /// <returns></returns>
        private static string SetSpace(string str, int iMax)
        {

            byte[] b = Encoding.Default.GetBytes(str.PadRight(iMax));
            return Encoding.Default.GetString(b, 0, iMax);

        }

        #endregion

    }
}