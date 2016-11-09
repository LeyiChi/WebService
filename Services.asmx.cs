using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using InterSystems.Data.CacheClient;
using System.Xml;
using System.Text;
using WebService.DataMethod;
using WebService.CommonLibrary;
using System.Data;
using InterSystems.Data.CacheTypes;
using WebService.DataClass;
using System.IO;
using System.Net;
using RongYunClassLib;
using ServiceStack.Redis;
using System.Security.Cryptography;

namespace WebService
{
    /// <summary>
    /// Summary description for Services
    /// </summary>
    [WebService(Namespace = "http://bme319.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Services : System.Web.Services.WebService
    {

        #region<Serivce Field>
        public static int serviceCount = 0; //Count for opening service
        static int serviceIDTrack = 0;
        public int serviceID;
        DataConnection _cnCache;           // Cache数据库连接对象
        #endregion

        #region <"Constructor">
        public Services()
        {
            serviceCount++;
            this.serviceID = ++serviceIDTrack;
            this._cnCache = new DataConnection(this.serviceID);
        }
        #endregion

        #region <" Cache数据库连接测试 ">

        [WebMethod(Description = "数据库连接测试  Author:ZAM  2014-11-25")]
        //数据库连接测试
        public bool TestGetCacheConnection()
        {
            try
            {
                if (_cnCache.Connect())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                _cnCache.DisConnect();
            }
        }
        #endregion

        #region <" common ">
        private string ConvertWeek(string weekstr)
        {
            switch (weekstr)
            {
                case "Monday": weekstr = "1"; break;
                case "Tuesday": weekstr = "2"; break;
                case "Wednesday": weekstr = "3"; break;
                case "Thursday": weekstr = "4"; break;
                case "Friday": weekstr = "5"; break;
                case "Saturday": weekstr = "6"; break;
                case "Sunday": weekstr = "7"; break;
                default: break;
            }
            return weekstr;
        }

        private string ConvertWeek_C(string weekstr)
        {
            switch (weekstr)
            {
                case "Monday": weekstr = "周一"; break;
                case "Tuesday": weekstr = "周二"; break;
                case "Wednesday": weekstr = "周三"; break;
                case "Thursday": weekstr = "周四"; break;
                case "Friday": weekstr = "周五"; break;
                case "Saturday": weekstr = "周六"; break;
                case "Sunday": weekstr = "周日"; break;
                default: break;
            }
            return weekstr;
        }

        public static bool IsToday(DateTime someDate)
        {
            DateTime dt = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            TimeSpan ts = someDate - dt;
            if (ts.Days == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [WebMethod(Description = "获取某个分类的类别 Table:Cm.MstType  Author:ZAM 2014-11-25")]
        // GetTypeList 获取某个分类的类别 ZAM 2014-11-25
        public DataSet GetTypeList(string Category)
        {
            try
            {
                DataTable DT_MstType = new DataTable();
                DataSet DS_MstType = new DataSet();
                DT_MstType = CmMstType.GetTypeList(_cnCache, Category);
                DS_MstType.Tables.Add(DT_MstType);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstType.GetTypeList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取MstType某个类型名称 Table：Cm.MstType Author:ZAM  2014-12-24")]
        //获取MstType某个类型名称       
        public string GetMstTypeName(string Category, int Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstType.GetName(_cnCache, Category, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMstTypeName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除用户所有旧信息(Date为8位int型) Table：Cm.MstUser Author:ZAM 2015-1-20")]
        public int DeleteUser(string UserId, int Date)
        {
            try
            {
                int ret = 0;
                ret = CmMstUser.DeleteAll(_cnCache, UserId, Date);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteUser", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetServerDateTime Table：Cm.CommonLibrary Author:ZC  2015-01-20")]
        public string GetServerTime()
        {
            string serverTime = string.Empty;
            try
            {
                if (!_cnCache.Connect())
                {
                    return serverTime;
                }
                serverTime = Cm.CommonLibrary.GetServerDateTime(_cnCache.CacheConnectionObject);    //2014/08/22 15:33:35
                serverTime = serverTime.Replace("/", "-");
                return serverTime;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetServerTime", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return serverTime;
                throw (ex);
            }
            finally
            {
                _cnCache.DisConnect();
            }
        }

        [WebMethod(Description = "GetServerLog Table：Cm.CommonLibrary Author:ZAM  2015-01-22")]
        public DataSet GetServerLog()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Log", typeof(string));
            DataSet ds = new DataSet();

            string serverLog = string.Empty;
            try
            {
                string physicalPath = System.Web.HttpContext.Current.Request.PhysicalPath;
                int index = physicalPath.LastIndexOf('\\');
                string logdir = physicalPath.Substring(0, index);

                string dir = Path.Combine(logdir + "\\Log", Environment.MachineName.ToUpper());
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                DirectoryInfo[] listdir = dirInfo.GetDirectories();
                FileInfo[] listfile = dirInfo.GetFiles();
                if (listfile.Length > 0)
                {
                    FileInfo fileInfo = listfile[listfile.Length - 1];
                    string filedir = fileInfo.DirectoryName;
                    string fileName = fileInfo.Name;

                    FileStream fs = new FileStream(filedir + "\\" + fileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        while (sr.Peek() != '2' && sr.Peek() > -1)
                        {
                            line += sr.ReadLine();
                        }
                        dt.Rows.Add(line);
                    }
                    sr.Close();
                    fs.Close();
                }
                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetServerLog", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ds;
                throw (ex);
            }
            finally
            {
                //_cnCache.DisConnect();
            }
        }

        private int GetServerDate()                 //ZAM 2015-5-13 获取服务器日期(解决频繁连接导致的连接未及时关闭)
        {
            string serverDate = string.Empty;
            int date = 99999999;
            try
            {
                //ZAM 2015-5-7 频繁连接导致的连接未及时关闭
                //if (_cnCache.CacheConnectionObject.State == ConnectionState.Closed)
                //if (_cnCache.CacheConnectionObject.State != ConnectionState.Open)
                {
                    if (!_cnCache.Connect())
                    {
                        return date;
                    }
                }
                serverDate = Cm.CommonLibrary.GetServerDateTime(_cnCache.CacheConnectionObject);    //2014/08/22 15:33:35
                string[] str = serverDate.Split(' ');
                if (str.Length >= 1)
                {
                    serverDate = str[0];
                    serverDate = serverDate.Replace("/", string.Empty);
                    date = Convert.ToInt32(serverDate);
                }

                return date;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetServerDate", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return date;
                throw (ex);
            }
            finally
            {
                _cnCache.DisConnect();
            }

        }
        #endregion

        #region <" 医生首页(患者列表) ZAM ">
        [WebMethod(Description = "获取该医生负责的患者列表  Table:Ps.DoctorInfo Ps.BasicInfo   Author:ZAM 2015-01-17")]
        public DataSet GetPatientsByDoctorId(string DoctorId, string VisitId, string UserRole)
        {
            DataTable DT_Patients = new DataTable();
            DT_Patients.Columns.Add(new DataColumn("PatientId", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("PatientName", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Gender", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Age", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Diagnosis", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Module", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("AlertNumber", typeof(int)));
            DT_Patients.Columns.Add(new DataColumn("CareLevel", typeof(int)));
            DT_Patients.Columns.Add(new DataColumn("ModuleType", typeof(string)));

            DataSet DS_Patients = new DataSet();
            DataTable DT_PatientList = new DataTable();

            string CategoryCode = string.Empty;
            int GenderType = 0;
            int AlertStatus = 0;
            int CareLevel = 0;
            try
            {
                //GetDoctorModuleList: CategoryCode
                if (UserRole == "Doctor")
                {
                    DataTable DT_DoctorModule = PsDoctorInfo.GetDoctorModuleList(_cnCache, DoctorId);
                    //DataCheck
                    if (DT_DoctorModule == null)
                    {
                        return DS_Patients;
                    }
                    foreach (DataRow dr_DoctorModule in DT_DoctorModule.Rows)
                    {
                        CategoryCode = string.Empty;
                        CategoryCode = dr_DoctorModule["CategoryCode"].ToString();
                        DT_PatientList = (PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, DoctorId, CategoryCode));
                        string genderName = "";
                        //DataCheck
                        if (DT_PatientList == null)
                        {
                            return DS_Patients;
                        }
                        foreach (DataRow dr_patient in DT_PatientList.Rows)
                        {
                            DT_Patients.Clear();
                            DT_Patients.TableName = CategoryCode;
                            GetPatientsInfo(ref DT_Patients, dr_patient, CategoryCode, GenderType, genderName, AlertStatus, CareLevel, VisitId);
                            DS_Patients.Merge(DT_Patients);
                        }
                    }
                }
                if (UserRole == "HealthCoach")
                {
                    DT_PatientList = (PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, DoctorId, "HM1"));
                    string genderName = "";
                    if (DT_PatientList != null)
                    {
                        foreach (DataRow dr_patient in DT_PatientList.Rows)
                        {
                            DT_Patients.Clear();
                            DT_Patients.TableName = "HM1";
                            GetPatientsInfo(ref DT_Patients, dr_patient, "HM1", GenderType, genderName, AlertStatus, CareLevel, VisitId);
                            DS_Patients.Merge(DT_Patients);
                        }
                    }

                    DT_PatientList = (PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, DoctorId, "HM2"));
                    genderName = "";
                    if (DT_PatientList != null)
                    {
                        foreach (DataRow dr_patient in DT_PatientList.Rows)
                        {
                            DT_Patients.Clear();
                            DT_Patients.TableName = "HM2";
                            GetPatientsInfo(ref DT_Patients, dr_patient, "HM2", GenderType, genderName, AlertStatus, CareLevel, VisitId);
                            DS_Patients.Merge(DT_Patients);
                        }
                    }

                    DT_PatientList = (PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, DoctorId, "HM3"));
                    genderName = "";
                    if (DT_PatientList != null)
                    {
                        foreach (DataRow dr_patient in DT_PatientList.Rows)
                        {
                            DT_Patients.Clear();
                            DT_Patients.TableName = "HM3";
                            GetPatientsInfo(ref DT_Patients, dr_patient, "HM3", GenderType, genderName, AlertStatus, CareLevel, VisitId);
                            DS_Patients.Merge(DT_Patients);
                        }
                    }
                }
                return DS_Patients;
            }
            catch (Exception ex)
            {

                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientsByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        //获取医生首页的患者信息列表 ZAM 2015-01-04
        public Boolean GetPatientsInfo(ref DataTable DT_Patients, DataRow dr_patient, string CategoryCode, int GenderType, string genderName, int AlertStatus, int CareLevel, string VisitId)
        {
            bool flag = false;
            string patientName = string.Empty;
            string age = string.Empty;
            string gender = string.Empty;
            try
            {
                string patientId = dr_patient["PatientId"].ToString();
                //BasicInfo: PatientID, PatientName, Gender, Age
                CacheSysList BasicInfolist = PsBasicInfo.GetPatientBasicInfo(_cnCache, patientId);
                //DataCheck               
                if (BasicInfolist != null)      //ZAM 2015-1-7
                {
                    patientName = BasicInfolist[0];
                    age = BasicInfolist[1];
                    gender = BasicInfolist[8];      //ZAM 2015-1-13 [2]->[8]
                }
                if ((GenderType != 0) && genderName != gender)
                {
                    return flag;
                }

                //AlertNumber
                int alertNumber = WnTrnAlertRecord.GetUntreatedAlertAmount(_cnCache, patientId);
                //Search By AlertNumber
                if (AlertStatus != 0)
                {
                    //AlertStatus(ProcessStatus) and alertNumber mismatch
                    if ((AlertStatus == 1 && alertNumber == 0) || (AlertStatus == 2 && alertNumber != 0))   //AlertStatus == 1 : unprocessed
                    {
                        return flag;
                    }
                }

                //Carelevel
                string moduleType = CategoryCode;
                int carelevel = PsSpecialList.GetCareLevel(_cnCache, moduleType, patientId);
                //Search By CareLevel
                if (CareLevel != 0 && carelevel != CareLevel)
                {
                    return flag;
                }

                //取最新一条主诊断记录
                int DiagnosisType = 3;
                string diagnosisstr = PsDiagnosis.GetDiagnosisByType(_cnCache, patientId, DiagnosisType);

                DataTable DT_Module = new DataTable();
                //Modules：DataTable to string
                if (CategoryCode.Substring(0, 1) == "M")
                {
                    DT_Module = PsBasicInfoDetail.GetModulesByPID(_cnCache, patientId);   //single column ,several rows
                }
                else
                {
                    DT_Module = PsBasicInfoDetail.GetHModulesByPID(_cnCache, patientId);
                }
                string modulestr = string.Empty;
                //DataCheck
                if (DT_Module != null)
                {
                    foreach (DataRow dr_Module in DT_Module.Rows)
                    {
                        modulestr += "\r\n" + dr_Module["Modules"].ToString();
                    }
                }
                DT_Patients.Rows.Add(patientId, patientName, gender, age, diagnosisstr, modulestr, alertNumber, carelevel, CategoryCode);
                return true;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientsInfo函数", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }

        }

        [WebMethod(Description = "修改患者关注等级 Table:Ps.SpecialList  Author:ZAM 2015-1-12")]
        // ChangCareLevel 修改患者关注等级 ZAM 2015-1-12
        public bool ChangCareLevel(string moduleType, string PatientId, string DoctorId, int carelevel, string revUserId, string pTerminalIP, string pTerminalName, int pDeviceType)
        {
            try
            {
                bool ret = false;
                if (moduleType != "")
                {
                    ret = PsSpecialList.SetData(_cnCache, moduleType, PatientId, DoctorId, carelevel, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                }
                else
                {
                    DataTable DT_Module = PsBasicInfoDetail.GetModulesByPID(_cnCache, PatientId);   //single column ,several rows
                    //DataCheck
                    if (DT_Module != null)
                    {
                        foreach (DataRow dr_Module in DT_Module.Rows)
                        {
                            moduleType = dr_Module["CategoryCode"].ToString();
                            ret = PsSpecialList.SetData(_cnCache, moduleType, PatientId, DoctorId, carelevel, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                        }
                    }
                }
                //ret = WnTrnAlertRecord.SetProcessFlag(_cnCache, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangCareLevel", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        #endregion

        #region <" 医生首页(警报) ZAM ">
        [WebMethod(Description = "获得医生所负责患者的所有警报记录  Table:Wn.TrnAlertRecord  Author:ZAM 2015-1-19")]
        // GetAlertsByDoctorId 获得医生所负责患者的所有警报记录 ZAM 2015-1-19
        public DataSet GetAlertsByDoctorId(string DoctorId)
        {
            try
            {
                DataTable DT_Patients = new DataTable();
                DT_Patients.Columns.Add(new DataColumn("PatientId", typeof(string)));
                DT_Patients.Columns.Add(new DataColumn("PatientName", typeof(string)));
                DT_Patients.Columns.Add(new DataColumn("AlertTypeName", typeof(string)));
                DT_Patients.Columns.Add(new DataColumn("AlertItem", typeof(string)));
                DT_Patients.Columns.Add(new DataColumn("AlertDateTime", typeof(DateTime)));
                DT_Patients.Columns.Add(new DataColumn("PushFlag", typeof(int)));
                DT_Patients.Columns.Add(new DataColumn("ProcessFlag", typeof(int)));
                DT_Patients.Columns.Add(new DataColumn("SortNo", typeof(int)));
                DataSet DS_Patients = new DataSet();
                DataTable DT_PatientList = new DataTable();
                string CategoryCode = string.Empty;

                DataTable DT_DoctorModule = PsDoctorInfo.GetDoctorModuleList(_cnCache, DoctorId);
                //DataCheck
                if (DT_DoctorModule == null)
                {
                    return DS_Patients;
                }
                foreach (DataRow dr_DoctorModule in DT_DoctorModule.Rows)
                {
                    CategoryCode = string.Empty;
                    CategoryCode = dr_DoctorModule["CategoryCode"].ToString();
                    DT_PatientList.Merge(PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, DoctorId, CategoryCode));
                }
                //unique the patientId column
                DataView dv = new DataView(DT_PatientList);
                string[] columns = { "PatientId", "PatientName" };
                DT_PatientList = dv.ToTable(true, columns);

                DataColumn[] cols = new DataColumn[] { DT_PatientList.Columns["PatientId"] };       //PrimaryKey setting
                DT_PatientList.PrimaryKey = cols;

                //DataCheck
                if (DT_PatientList == null)
                {
                    return DS_Patients;
                }
                foreach (DataRow dr_Patient in DT_PatientList.Rows)
                {
                    string patientId = dr_Patient["PatientId"].ToString();
                    string patientName = dr_Patient["PatientName"].ToString();
                    //Split some colunms
                    DataTable DT_AlertList = WnTrnAlertRecord.GetTrnAlertRecordList(_cnCache, patientId);
                    foreach (DataRow dr_AlertList in DT_AlertList.Rows)
                    {
                        string AlertTypeName = dr_AlertList["AlertTypeName"].ToString();
                        string AlertItem = dr_AlertList["AlertItem"].ToString();
                        DateTime AlertDateTime = DateTime.Parse(dr_AlertList["AlertDateTime"].ToString());
                        int PushFlag = Convert.ToInt32(dr_AlertList["PushFlag"]);
                        int processFlag = Convert.ToInt32(dr_AlertList["ProcessFlag"]);
                        int sortNo = Convert.ToInt32(dr_AlertList["SortNo"]);

                        //Assembly
                        //Several Patients, each patient has several alertRecords
                        DT_Patients.Rows.Add(patientId, patientName, AlertTypeName, AlertItem, AlertDateTime, PushFlag, processFlag, sortNo);
                    }
                }
                DS_Patients.Merge(DT_Patients);
                return DS_Patients;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAlertsByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "警报处置状态置位 Table：Wn.TrnAlertRecord Author:ZAM  2014-12-26")]
        //警报处置状态置位 ZAM 2014-12-26
        public bool SetProcessFlag(string UserId, int SortNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool ret = false;
                ret = WnTrnAlertRecord.SetProcessFlag(_cnCache, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetProcessFlag", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        #endregion

        #region <登录页面 TDY>
        [WebMethod(Description = "根据用户名和当前登录日期，获取当前有效密码并与页面端输入的密码进行匹配 Table：Cm.MstUser Author:TDY  2014-12-04")]
        public int CheckPassword(string UserId, string Password)
        {
            int ret = 0;
            try
            {

                ret = CmMstUser.CheckPassword(_cnCache, UserId, Password);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.CheckPassword", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "修改用户上次登录日期 Table：Cm.MstUser Author:TDY  2014-12-04")]
        public int UpdateLastLoginDateTime(string UserId, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {

                ret = CmMstUser.UpdateLastLoginDateTime(_cnCache, UserId, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.UpdateLastLoginDateTime", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "(移动端)根据用户名和当前登录日期，获取当前有效密码并与页面端输入的密码进行匹配 Table：Cm.MstUserDetail Author:TDY  2015-04-09")]//TDY 20150507修改
        public int CheckPasswordByInput(string Type, string Name, string Password)
        {
            int ret = 0;
            try
            {

                ret = CmMstUserDetail.CheckPasswordByInput(_cnCache, Type, Name, Password);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUserDetail.CheckPassworkByInput", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "(移动端)健康专员进行注册 Table：Cm.MstUser Author:TDY  2015-04-09")] //TDY 20150419修改
        public int Register(string Type, string Name, string Value, string Password, string UserName, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {

                ret = CmMstUser.Register(_cnCache, Type, Name, Value, Password, UserName, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.Register", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "(移动端)患者激活账户 Table：Cm.MstUser Author:TDY  2015-04-09")] //TDY 20150419修改
        public int Activition(string UserId, string InviteCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {

                ret = CmMstUser.Activition(_cnCache, UserId, InviteCode, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.Activition", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }


        [WebMethod(Description = "(移动端)检查用户名是否重复 Table：Cm.MstUserDetail Author:TDY  2015-04-09")]
        public int CheckRepeat(string Input, string Type)
        {
            int ret = 0;
            try
            {

                ret = CmMstUserDetail.CheckRepeat(_cnCache, Input, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUserDetail.CheckRepeat", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "(移动端)获取用户的全部角色 Table：Ps.RoleMatch Author:TDY  2015-04-09")]
        public DataSet GetAllRoleMatch(string UserId)
        {
            try
            {
                DataTable DT_MstType = new DataTable();
                DataSet DS_MstType = new DataSet();
                DT_MstType = PsRoleMatch.GetAllRoleMatch(_cnCache, UserId);
                DS_MstType.Tables.Add(DT_MstType);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsRoleMatch.GetAllRoleMatch", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Ps.RoleMatch Author:TDY  2015-04-09")] //TDY 20150526修改
        public int SetPsRoleMatch(string PatientId, string RoleClass, string ActivationCode, string ActivatedState, string Description)
        {
            try
            {
                int ret = 0;
                ret = PsRoleMatch.SetData(_cnCache, PatientId, RoleClass, ActivationCode, ActivatedState, Description);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPsRoleMatch", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstUserDetail Author:TDY  2015-04-09")]
        public int SetCmMstUserDetail(string UserId, int StartDate, string Type, string Name, string Value, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int ret = 0;
                ret = CmMstUserDetail.SetData(_cnCache, UserId, StartDate, Type, Name, Value, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetCmMstUserDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "(移动端)根据手机号码获取用户ID Table：Cm.MstUserDetail Author:TDY  2015-04-14")]//TDY 20150507修改
        public string GetIDByInput(string Type, string Name)
        {
            string ret = "";
            try
            {

                ret = CmMstUserDetail.GetIDByInput(_cnCache, Type, Name);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUserDetail.GetIDByInput", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "SetActivition Table：Ps.RoleMatch Author:TDY  2015-05-26")]
        public int SetActivition(string UserID, string RoleClass, string ActivationCode)
        {
            try
            {
                int ret = 0;
                ret = PsRoleMatch.SetActivition(_cnCache, UserID, RoleClass, ActivationCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetActivition", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetActivition Table：Ps.RoleMatch Author:TDY  2015-05-26")]
        public string GetActivatedState(string UserID, string RoleClass)
        {
            string ret = "";
            try
            {

                ret = PsRoleMatch.GetActivatedState(_cnCache, UserID, RoleClass);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetActivatedState", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw (ex);
            }
        }

        [WebMethod(Description = "绑定手机号码 Table：Cm.MstUserDetail Author:TDY  2015-05-0927")]
        public int SetPhoneNo(string UserId, string Type, string Name, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {

                ret = CmMstUserDetail.SetPhoneNo(_cnCache, UserId, Type, Name, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUserDetail.SetPhoneNo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "获取激活码 GetActivateCode Table：Ps.RoleMatch Author:TDY  2015-06-03")]
        public string GetActivateCode(string UserID, string RoleClass)
        {
            string ret = "";
            try
            {

                ret = PsRoleMatch.GetActivateCode(_cnCache, UserID, RoleClass);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetActivateCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw (ex);
            }
        }
        #endregion

        #region <密码管理 TDY>
        [WebMethod(Description = "获取用户当前的绑定手机号码并与页面端输入的手机号码进行匹配 Table：Ps.BasicInfoDetail Author:TDY  2015-01-13")]
        public int CheckPatientPhoneNumber(string UserId, string piPhoneNumber)
        {
            int ret = 0;
            try
            {

                ret = PsBasicInfoDetail.CheckPatientPhoneNumber(_cnCache, UserId, piPhoneNumber);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.CheckPhoneNumber", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "获取医生当前的绑定手机号码并与页面端输入的手机号码进行匹配 Table：Ps.DoctorInfoDetail Author:TDY  2015-01-13")]
        public int CheckDoctorPhoneNumber(string UserId, string piPhoneNumber)
        {
            int ret = 0;
            try
            {

                ret = PsDoctorInfoDetail.CheckDoctorPhoneNumber(_cnCache, UserId, piPhoneNumber);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.CheckDoctorPhoneNumber", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "修改密码 Table：Cm.MstUser Author:TDY  2014-12-04")]
        public int ChangePassword(string UserId, string OldPassword, string newPassword, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                ret = CmMstUser.ChangePassword(_cnCache, UserId, OldPassword, newPassword, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.ChangePassword", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "重置密码 Table：Cm.MstUser Author:TDY  2014-12-04")]
        public int ResetPassword(string UserId, string OldPassword, string newPassword, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                ret = CmMstUser.ChangePassword(_cnCache, UserId, "#*bme319*#", newPassword, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.ChangePassword", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        [WebMethod(Description = "获取用户当前的绑定手机号码并与页面端输入的手机号码进行匹配 Table：Cm.MstUserDetail Author:TDY  2015-04-13")]//TDY 20150507修改
        public int CheckPhoneNumber(string Type, string piName, string UserId)
        {
            int ret = 0;
            try
            {

                ret = CmMstUserDetail.CheckPhoneNumber(_cnCache, Type, piName, UserId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUserDetail.CheckPhoneNumber", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
                throw ex;
            }
        }

        #endregion

        #region <个人主页 TDY>
        [WebMethod(Description = "根据用户名获取用户的基本信息 Table：Ps.BasicInfo Author:TDY  2014-12-03")]
        public PatientBasicInfo0 GetBasicInfo(string UserId)
        {
            try
            {
                string module = "";
                PatientBasicInfo0 BasicInfo = new PatientBasicInfo0();
                CacheSysList GetBasicInfoList = PsBasicInfo.GetBasicInfo(_cnCache, UserId);
                BasicInfo.UserId = UserId;
                if (GetBasicInfoList != null)
                {
                    BasicInfo.UserName = GetBasicInfoList[0];
                    if (BasicInfo.UserName == null)
                    {
                        BasicInfo.UserName = "";
                    }
                    BasicInfo.Birthday = GetBasicInfoList[1];
                    if (BasicInfo.Birthday == null)
                    {
                        BasicInfo.Birthday = "";
                    }
                    BasicInfo.Gender = GetBasicInfoList[3]; //TDY 12-25 Modify
                    if (BasicInfo.Gender == null)
                    {
                        BasicInfo.Gender = "0";
                    }
                    BasicInfo.IDNo = GetBasicInfoList[2];   //TDY 12-25 Modify
                    if (BasicInfo.IDNo == null)
                    {
                        BasicInfo.IDNo = "";
                    }
                }

                DataTable modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                for (int i = 0; i < modules.Rows.Count; i++)
                {
                    module = module + "|" + modules.Rows[i][1];
                }
                if (module != "")
                {
                    module = module.Substring(1, module.Length - 1);
                }
                BasicInfo.Module = module;
                return BasicInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfo.GetBasicInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }

        }

        [WebMethod(Description = "根据用户名获取用户的详细信息 Table：Ps.BasicInfoDetail Author:TDY  2015-1-15")]
        public PatientDetailInfo0 GetDetailInfo(string UserId)
        {
            try
            {
                string module = "";
                PatientDetailInfo0 DetailInfo = new PatientDetailInfo0();
                CacheSysList GetDetailInfoList = PsBasicInfoDetail.GetDetailInfo(_cnCache, UserId);
                DetailInfo.UserId = UserId;
                if (GetDetailInfoList != null)
                {
                    DetailInfo.PhoneNumber = GetDetailInfoList[0];
                    if (DetailInfo.PhoneNumber == null)
                    {
                        DetailInfo.PhoneNumber = "";
                    }
                    DetailInfo.HomeAddress = GetDetailInfoList[1];
                    if (DetailInfo.HomeAddress == null)
                    {
                        DetailInfo.HomeAddress = "";
                    }
                    DetailInfo.Occupation = GetDetailInfoList[2];
                    if (DetailInfo.Occupation == null)
                    {
                        DetailInfo.Occupation = "";
                    }
                    DetailInfo.Nationality = GetDetailInfoList[3];
                    if (DetailInfo.Nationality == null)
                    {
                        DetailInfo.Nationality = "";
                    }
                    DetailInfo.EmergencyContact = GetDetailInfoList[4];
                    if (DetailInfo.EmergencyContact == null)
                    {
                        DetailInfo.EmergencyContact = "";
                    }
                    DetailInfo.EmergencyContactPhoneNumber = GetDetailInfoList[5];
                    if (DetailInfo.EmergencyContactPhoneNumber == null)
                    {
                        DetailInfo.EmergencyContactPhoneNumber = "";
                    }
                    DetailInfo.PhotoAddress = GetDetailInfoList[6];
                    if (DetailInfo.PhotoAddress == null)
                    {
                        DetailInfo.PhotoAddress = "";
                    }
                    DetailInfo.IDNo = GetDetailInfoList[7]; //TDY 20150115 添加
                    if (DetailInfo.IDNo == null)
                    {
                        DetailInfo.IDNo = "";
                    }
                }

                DataTable modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                for (int i = 0; i < modules.Rows.Count; i++)
                {
                    module = module + "|" + modules.Rows[i][1];
                }
                if (module != "")
                {
                    module = module.Substring(1, module.Length - 1);
                }
                DetailInfo.Module = module;
                return DetailInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetDetailInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }

        }

        [WebMethod(Description = "获取用户全部基本信息 Table：Ps.BasicInfo Author:TDY  2014-12-04")]
        public PatientALLBasicInfo GetUserBasicInfo(string UserId)
        {
            try
            {
                string module = "";
                PatientALLBasicInfo UserBasicInfo = new PatientALLBasicInfo();
                CacheSysList GetUserBasicInfoList = PsBasicInfo.GetUserBasicInfo(_cnCache, UserId);
                UserBasicInfo.UserId = UserId;
                if (GetUserBasicInfoList != null)
                {
                    UserBasicInfo.UserName = GetUserBasicInfoList[0];
                    UserBasicInfo.Birthday = Convert.ToInt32(GetUserBasicInfoList[1]);
                    UserBasicInfo.Gender = GetUserBasicInfoList[2];
                    UserBasicInfo.BloodType = GetUserBasicInfoList[3];
                    UserBasicInfo.IDNo = GetUserBasicInfoList[4];
                    UserBasicInfo.DoctorId = GetUserBasicInfoList[5];
                    UserBasicInfo.InsuranceType = GetUserBasicInfoList[6];
                    UserBasicInfo.InvalidFlag = Convert.ToInt32(GetUserBasicInfoList[7]);
                }

                DataTable modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                for (int i = 0; i < modules.Rows.Count; i++)
                {
                    module = module + "|" + modules.Rows[i][1];
                }
                if (module != "")
                {
                    module = module.Substring(1, module.Length - 1);
                }
                UserBasicInfo.Module = module;
                return UserBasicInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfo.GetUserBasicInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }

        }

        [WebMethod(Description = "获取用户全部详细信息 Table：Ps.BasicInfoDetail Author:TDY  2014-12-04")]
        public DataSet GetPatientBasicInfoDetail(string UserId, string CategoryCode)
        {
            try
            {
                DataTable DT_MstType = new DataTable();
                DataSet DS_MstType = new DataSet();
                DT_MstType = PsBasicInfoDetail.GetPatientBasicInfoDetail(_cnCache, UserId, CategoryCode);
                DS_MstType.Tables.Add(DT_MstType);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "修改用户基本信息 Table：Ps.BasicInfo Author:TDY  2014-12-03")]
        public bool SetBasicInfo(string UserId, string UserName, int Birthday, int Gender, int BloodType, string IDNo, string DoctorId, string InsuranceType, int InvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool Flag = false;
                Flag = PsBasicInfo.SetData(_cnCache, UserId, UserName, Birthday, Gender, BloodType, IDNo, DoctorId, InsuranceType, InvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfo.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "修改用户详细信息 Table：Ps.BasicInfoDetail Author:TDY  2014-12-03")]
        public bool SetBasicInfoDetail(string Patient, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool Flag = false;
                Flag = PsBasicInfoDetail.SetData(_cnCache, Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "根据用户名获取医生基本信息 Table:TDY 2015-01-12")]
        public DataSet GetDoctorInfo(string UserId)
        {
            try
            {
                DataTable DT_MstType = new DataTable();
                DataSet DS_MstType = new DataSet();
                DT_MstType = PsDoctorInfo.GetDoctorInfo(_cnCache, UserId);
                DS_MstType.Tables.Add(DT_MstType);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfo.GetDoctorInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据用户名获取医生详细信息 Table:TDY 2015-01-15")]
        public DoctorDetailInfo0 GetDoctorInfoDetail(string Doctor)
        {
            try
            {
                DoctorDetailInfo0 DetailInfo = new DoctorDetailInfo0();
                CacheSysList GetDetailInfoList = PsDoctorInfoDetail.GetDoctorDetailInfo(_cnCache, Doctor);
                DetailInfo.UserId = Doctor;
                if (GetDetailInfoList != null)
                {
                    DetailInfo.PhoneNumber = GetDetailInfoList[0];
                    if (DetailInfo.PhoneNumber == null)
                    {
                        DetailInfo.PhoneNumber = "";
                    }
                    DetailInfo.HomeAddress = GetDetailInfoList[1];
                    if (DetailInfo.HomeAddress == null)
                    {
                        DetailInfo.HomeAddress = "";
                    }
                    DetailInfo.Occupation = GetDetailInfoList[2];
                    if (DetailInfo.Occupation == null)
                    {
                        DetailInfo.Occupation = "";
                    }
                    DetailInfo.Nationality = GetDetailInfoList[3];
                    if (DetailInfo.Nationality == null)
                    {
                        DetailInfo.Nationality = "";
                    }
                    DetailInfo.EmergencyContact = GetDetailInfoList[4];
                    if (DetailInfo.EmergencyContact == null)
                    {
                        DetailInfo.EmergencyContact = "";
                    }
                    DetailInfo.EmergencyContactPhoneNumber = GetDetailInfoList[5];
                    if (DetailInfo.EmergencyContactPhoneNumber == null)
                    {
                        DetailInfo.EmergencyContactPhoneNumber = "";
                    }
                    DetailInfo.PhotoAddress = GetDetailInfoList[6];
                    if (DetailInfo.PhotoAddress == null)
                    {
                        DetailInfo.PhotoAddress = "";
                    }
                    DetailInfo.IDNo = GetDetailInfoList[7]; //TDY 20150115 添加
                    if (DetailInfo.IDNo == null)
                    {
                        DetailInfo.IDNo = "";
                    }
                }
                return DetailInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.GetDetailDoctorInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Ps.DoctorInfoDetail写数据 Table:Ps.DoctorInfoDetail  Author:TDY 2015-01-12")]
        public bool SetDoctorInfoDetail(string Doctor, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool IsSaved = false;
                IsSaved = PsDoctorInfoDetail.SetData(_cnCache, Doctor, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        #endregion

        #region <" 字典维护界面 YDS">
        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstHospital Author:YDS  2015-01-14")]
        public int GetHospitalMaxCode(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstHospital.GetMaxCode(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHospitalMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstDivision Author:YDS  2015-01-14")]
        public int GetDivisionMaxCode()
        {
            try
            {
                int ret = 0;
                ret = CmMstDivision.GetMaxCode(_cnCache);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDivisionMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstType Author:YDS  2014-12-03")]
        public bool SetType(string Category, int Type, string Name, int InvalidFlag, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstType.SetData(_cnCache, Category, Type, Name, InvalidFlag, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstType Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetType()
        {
            try
            {
                DataTable DT_MstType = new DataTable();
                DataSet DS_MstType = new DataSet();
                DT_MstType = CmMstType.GetType(_cnCache);
                DS_MstType.Tables.Add(DT_MstType);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstType Author:YDS  2014-12-03")]
        public int DeleteType(string Category, int Type)
        {
            try
            {
                int ret = 0;
                ret = CmMstType.DeleteData(_cnCache, Category, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstRole Author:YDS  2014-12-03")]
        public bool SetRole(string Code, string Name, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstRole.SetData(_cnCache, Code, Name, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstRole Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetRole()
        {
            try
            {
                DataTable DT_MstRole = new DataTable();
                DataSet DS_MstRole = new DataSet();
                DT_MstRole = CmMstRole.GetRole(_cnCache);
                DS_MstRole.Tables.Add(DT_MstRole);
                return DS_MstRole;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstRole Author:YDS  2014-12-03")]
        public int DeleteRole(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstRole.DeleteData(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstAuthority Author:YDS  2014-12-03")]
        public bool SetAuthority(string piCode, string piName, int piSortNo, string piRedundance, int piInvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstAuthority.SetData(_cnCache, piCode, piName, piSortNo, piRedundance, piInvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetAuthority", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstAuthority Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetAuthority()
        {
            try
            {
                DataTable DT_MstAuthority = new DataTable();
                DataSet DS_MstAuthority = new DataSet();
                DT_MstAuthority = CmMstAuthority.GetAuthority(_cnCache);
                DS_MstAuthority.Tables.Add(DT_MstAuthority);
                return DS_MstAuthority;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAuthority", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstAuthority Author:YDS  2014-12-03")]
        public int DeleteAuthority(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstAuthority.DeleteData(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteAuthority", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstAuthorityDetail Author:YDS  2014-12-03")]
        public bool SetAuthorityDetail(string Authority, string piCode, string piName, int piSortNo, string piRedundance, int piInvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstAuthorityDetail.SetData(_cnCache, Authority, piCode, piName, piSortNo, piRedundance, piInvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetAuthorityDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstAuthorityDetail Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetAuthorityDetail()
        {
            try
            {
                DataTable DT_MstAuthorityDetail = new DataTable();
                DataSet DS_MstAuthorityDetail = new DataSet();
                DT_MstAuthorityDetail = CmMstAuthorityDetail.GetAuthorityDetail(_cnCache);
                DS_MstAuthorityDetail.Tables.Add(DT_MstAuthorityDetail);
                return DS_MstAuthorityDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAuthorityDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据大分类Code输出所有小分类信息 Table：Cm.MstAuthorityDetail Author:YDS  2014-12-03")]
        public DataSet GetSubAuthority(string Authority)
        {
            try
            {
                DataTable DT_MstAuthorityDetail = new DataTable();
                DataSet DS_MstAuthorityDetail = new DataSet();
                DT_MstAuthorityDetail = CmMstAuthorityDetail.GetSubAuthority(_cnCache, Authority);
                DS_MstAuthorityDetail.Tables.Add(DT_MstAuthorityDetail);
                return DS_MstAuthorityDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSubAuthority", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstAuthorityDetail Author:YDS  2014-12-03")]
        public int DeleteAuthorityDetail(string Authority, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstAuthorityDetail.DeleteData(_cnCache, Authority, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteAuthorityDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }


        [WebMethod(Description = "DeleteData Table：Cm.MstDivision Author:YDS  2014-12-03")]
        public int DeleteDivision(int Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstDivision.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstHospital Author:YDS  2014-12-03")]
        public bool SetHospital(string Code, int Type, string Name, int SortNo, int StartDate, int EndDate, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstHospital.SetData(_cnCache, Code, Type, Name, SortNo, StartDate, EndDate, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetHospital", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstHospital Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetHospital()
        {
            try
            {
                DataTable DT_MstHospital = new DataTable();
                DataSet DS_MstHospital = new DataSet();
                DT_MstHospital = CmMstHospital.GetHospital(_cnCache);
                DS_MstHospital.Tables.Add(DT_MstHospital);
                return DS_MstHospital;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHospital", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstHospital Author:YDS  2014-12-03")]
        public int DeleteHospital(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstHospital.DeleteData(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteHospital", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstSymptoms Author:YDS  2014-12-03")]
        public bool SetSymptoms(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstSymptoms.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstSymptoms Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetSymptoms()
        {
            try
            {
                DataTable DT_MstSymptoms = new DataTable();
                DataSet DS_MstSymptoms = new DataSet();
                DT_MstSymptoms = CmMstSymptoms.GetSymptoms(_cnCache);
                DS_MstSymptoms.Tables.Add(DT_MstSymptoms);
                return DS_MstSymptoms;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptoms", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstSymptoms Author:YDS  2014-12-03")]
        public int DeleteSymptoms(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstSymptoms.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteSymptoms", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstDiagnosis Author:YDS  2014-12-03")]
        public int DeleteDiagnosis(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstDiagnosis.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstVitalSigns Author:YDS  2014-12-03")]
        public bool SetVitalSigns(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstVitalSigns.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetVitalSigns", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstVitalSigns Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetVitalSigns()
        {
            try
            {
                DataTable DT_MstVitalSigns = new DataTable();
                DataSet DS_MstVitalSigns = new DataSet();
                DT_MstVitalSigns = CmMstVitalSigns.GetVitalSigns(_cnCache);
                DS_MstVitalSigns.Tables.Add(DT_MstVitalSigns);
                return DS_MstVitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVitalSigns", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstVitalSigns Author:YDS  2014-12-03")]
        public int DeleteVitalSigns(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstVitalSigns.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteVitalSigns", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstExaminationItem Author:YDS  2014-12-03")]
        public bool SetExaminationItem(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstExaminationItem.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExaminationItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstExaminationItem Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetExaminationItem()
        {
            try
            {
                DataTable DT_MstExaminationItem = new DataTable();
                DataSet DS_MstExaminationItem = new DataSet();
                DT_MstExaminationItem = CmMstExaminationItem.GetExaminationItem(_cnCache);
                DS_MstExaminationItem.Tables.Add(DT_MstExaminationItem);
                return DS_MstExaminationItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExaminationItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstExaminationItem Author:YDS  2014-12-03")]
        public int DeleteExaminationItem(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstExaminationItem.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteExaminationItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstExaminationSubItem Author:YDS  2014-12-03")]
        public bool SetExaminationSubItem(string Code, string Name, int SortNo,string InputCode, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstExaminationSubItem.SetData(_cnCache, Code, Name, SortNo, InputCode,Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExaminationSubItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstExaminationSubItem Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetExaminationSubItem()
        {
            try
            {
                DataTable DT_MstExaminationSubItem = new DataTable();
                DataSet DS_MstExaminationSubItem = new DataSet();
                DT_MstExaminationSubItem = CmMstExaminationSubItem.GetExaminationSubItem(_cnCache);
                DS_MstExaminationSubItem.Tables.Add(DT_MstExaminationSubItem);
                return DS_MstExaminationSubItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExaminationSubItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstExaminationSubItem Author:YDS  2014-12-03")]
        public int DeleteExaminationSubItem(string Code, string SubCode)
        {
            try
            {
                int ret = 0;
                ret = CmMstExaminationSubItem.DeleteData(_cnCache, Code, SubCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteExaminationSubItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstLabTestItems Author:YDS  2014-12-03")]
        public bool SetLabTestItems(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstLabTestItems.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabTestItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstLabTestItems Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetLabTestItems()
        {
            try
            {
                DataTable DT_MstLabTestItems = new DataTable();
                DataSet DS_MstLabTestItems = new DataSet();
                DT_MstLabTestItems = CmMstLabTestItems.GetLabTestItems(_cnCache);
                DS_MstLabTestItems.Tables.Add(DT_MstLabTestItems);
                return DS_MstLabTestItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstLabTestItems Author:YDS  2014-12-03")]
        public int DeleteLabTestItems(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstLabTestItems.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteLabTestItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstLabTestSubItems Author:YDS  2014-12-03")]
        public bool SetLabTestSubItems(string Code, string Name, int SortNo, string InputCode, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstLabTestSubItems.SetData(_cnCache, Code, Name, SortNo, InputCode, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabTestSubItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstLabTestSubItems Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetLabTestSubItems()
        {
            try
            {
                DataTable DT_MstLabTestSubItems = new DataTable();
                DataSet DS_MstLabTestSubItems = new DataSet();
                DT_MstLabTestSubItems = CmMstLabTestSubItems.GetLabTestSubItems(_cnCache);
                DS_MstLabTestSubItems.Tables.Add(DT_MstLabTestSubItems);
                return DS_MstLabTestSubItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestSubItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstLabTestSubItems Author:YDS  2014-12-03")]
        public int DeleteLabTestSubItems(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstLabTestSubItems.DeleteData(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteLabTestSubItems", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstTreatment Author:YDS  2014-12-03")]
        public bool SetTreatment(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstTreatment.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetTreatment", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstTreatment Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetTreatment()
        {
            try
            {
                DataTable DT_MstTreatment = new DataTable();
                DataSet DS_MstTreatment = new DataSet();
                DT_MstTreatment = CmMstTreatment.GetTreatment(_cnCache);
                DS_MstTreatment.Tables.Add(DT_MstTreatment);
                return DS_MstTreatment;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTreatment", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstTreatment Author:YDS  2014-12-03")]
        public int DeleteTreatment(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstTreatment.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteTreatment", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstIndicator Author:YDS  2014-12-03")]
        public bool SetIndicator(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstIndicator.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetIndicator", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstIndicator Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetIndicator()
        {
            try
            {
                DataTable DT_MstIndicator = new DataTable();
                DataSet DS_MstIndicator = new DataSet();
                DT_MstIndicator = CmMstIndicator.GetIndicator(_cnCache);
                DS_MstIndicator.Tables.Add(DT_MstIndicator);
                return DS_MstIndicator;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetIndicator", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstIndicator Author:YDS  2014-12-03")]
        public int DeleteIndicator(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstIndicator.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteIndicator", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstIndicatorParameter Author:YDS  2014-12-03")]
        public bool SetIndicatorParameter(string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstIndicatorParameter.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetIndicatorParameter", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstIndicatorParameter Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetIndicatorParameter()
        {
            try
            {
                DataTable DT_MstIndicatorParameter = new DataTable();
                DataSet DS_MstIndicatorParameter = new DataSet();
                DT_MstIndicatorParameter = CmMstIndicatorParameter.GetIndicatorParameter(_cnCache);
                DS_MstIndicatorParameter.Tables.Add(DT_MstIndicatorParameter);
                return DS_MstIndicatorParameter;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetIndicatorParameter", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstIndicatorParameter Author:YDS  2014-12-03")]
        public int DeleteIndicatorParameter(string Type, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstIndicatorParameter.DeleteData(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteIndicatorParameter", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "自动获取编号 Table：Cm.MstNumbering Author:YDS  2014-12-03")]
        public string GetNo(int NumberingType, string TargetDate)
        {
            try
            {
                string No = string.Empty;
                No = CmMstNumbering.GetNo(_cnCache, NumberingType, TargetDate);
                return No;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstInfoItemCategory Author:YDS  2014-12-03")]
        public bool SetInfoItemCategory(string Code, string Name, int SortNo, int StartDate, int EndDate, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstInfoItemCategory.SetData(_cnCache, Code, Name, SortNo, StartDate, EndDate, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetInfoItemCategory", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstInfoItemCategory Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetInfoItemCategory()
        {
            try
            {
                DataTable DT_MstInfoItemCategory = new DataTable();
                DataSet DS_MstInfoItemCategory = new DataSet();
                DT_MstInfoItemCategory = CmMstInfoItemCategory.GetInfoItemCategory(_cnCache);
                DS_MstInfoItemCategory.Tables.Add(DT_MstInfoItemCategory);
                return DS_MstInfoItemCategory;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInfoItemCategory", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstInfoItemCategory Author:YDS  2014-12-03")]
        public int DeleteInfoItemCategory(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstInfoItemCategory.DeleteData(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteInfoItemCategory", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstInfoItem Author:YDS  2014-12-03")]
        public bool SetInfoItem(string CategoryCode, string Code, string Name, string ParentCode, int SortNo, int StartDate, int EndDate, int GroupHeaderFlag, string ControlType, string OptionCategory, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstInfoItem.SetData(_cnCache, CategoryCode, Code, Name, ParentCode, SortNo, StartDate, EndDate, GroupHeaderFlag, ControlType, OptionCategory, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetInfoItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstInfoItem Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetInfoItem()
        {
            try
            {
                DataTable DT_MstInfoItem = new DataTable();
                DataSet DS_MstInfoItem = new DataSet();
                DT_MstInfoItem = CmMstInfoItem.GetInfoItem(_cnCache);
                DS_MstInfoItem.Tables.Add(DT_MstInfoItem);
                return DS_MstInfoItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInfoItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Cm.MstInfoItem Author:YDS  2014-12-03")]
        public int DeleteInfoItem(string CategoryCode, string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstInfoItem.DeleteData(_cnCache, CategoryCode, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteInfoItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Wn.MstBasicAlert Author:YDS  2014-12-03")]
        public bool SetBasicAlert(string AlertItemCode, string AlertItemName, decimal Min, decimal Max, string Units, string Remarks, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = WnMstBasicAlert.SetData(_cnCache, AlertItemCode, AlertItemName, Min, Max, Units, Remarks, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetBasicAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Wn.MstBasicAlert Author:YDS  2014-12-03")]
        //获取字典表全部信息
        public DataSet GetBasicAlert()
        {
            try
            {
                DataTable DT_WnMstBasicAlert = new DataTable();
                DataSet DS_WnMstBasicAlert = new DataSet();
                DT_WnMstBasicAlert = WnMstBasicAlert.GetBasicAlert(_cnCache);
                DS_WnMstBasicAlert.Tables.Add(DT_WnMstBasicAlert);
                return DS_WnMstBasicAlert;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetBasicAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table：Wn.MstBasicAlert Author:YDS  2014-12-03")]
        public int DeleteBasicAlert(string AlertItemCode)
        {
            try
            {
                int ret = 0;
                ret = WnMstBasicAlert.DeleteData(_cnCache, AlertItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteBasicAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstExaminationItem Author:YDS  2014-12-22")]
        public string GetExaminationItemMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstExaminationItem.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExaminationItemMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstSymptoms Author:YDS  2014-12-22")]
        public string GetSymptomsMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstSymptoms.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptomsMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstDiagnosis Author:YDS  2014-12-22")]
        public string GetDiagnosisMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstDiagnosis.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagnosisMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstVitalSigns Author:YDS  2014-12-22")]
        public string GetVitalSignsMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstVitalSigns.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVitalSignsMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstLabTestItems Author:YDS  2014-12-22")]
        public string GetLabTestItemsMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstLabTestItems.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestItemsMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstTreatment Author:YDS  2014-12-22")]
        public string GetTreatmentMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstTreatment.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTreatmentMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstIndicator Author:YDS  2014-12-22")]
        public string GetIndicatorMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstIndicator.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetIndicatorMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstIndicatorParameter Author:YDS  2014-12-22")]
        public string GetIndicatorParameterMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstIndicatorParameter.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetIndicatorParameterMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表最大编码 Table：Cm.MstType Author:YDS  2014-12-22")]
        public string GetTypeMaxCode(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstType.GetMaxCode(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeMaxCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取某模块下各种分类下的小项目的下一个编码 Table：Cm.MstInfoItem Author:YDS  2014-12-23")]
        public DataSet GetNextCode(string CategoryCode)
        {
            try
            {
                DataTable DT_CmMstInfoItem = new DataTable();
                DataSet DS_CmMstInfoItem = new DataSet();
                DT_CmMstInfoItem = CmMstInfoItem.GetNextCode(_cnCache, CategoryCode);
                DS_CmMstInfoItem.Tables.Add(DT_CmMstInfoItem);
                return DS_CmMstInfoItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNextCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstDrug Author:ZC  2015-04-15")]
        public bool SetDrug(string DrugCode, string DrugName, string DrugSpec, string Units, string DrugIndicator, string InputCode)
        {
            try
            {
                bool ret = false;
                ret = CmMstDrug.SetData(_cnCache, DrugCode, DrugName, DrugSpec, Units, DrugIndicator, InputCode) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDrug", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstDrugDetail Author:ZC  2015-04-15")]
        public bool SetDrugDetail(string DrugCode, string Module, string CurativeEffect, string SideEffect, string Instruction, string HealthEffect, string Unit, string Redundance)
        {
            try
            {
                bool ret = false;
                ret = CmMstDrugDetail.SetData(_cnCache, DrugCode, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, Redundance) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDrugDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstLifeStyle Author:ZC  2015-04-15")]
        public bool SetLifeStyle(string StyleId, string Name, string Redundance)
        {
            try
            {
                bool ret = false;
                ret = CmMstLifeStyle.SetData(_cnCache, StyleId, Name, Redundance) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLifeStyle", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstLifeStyleDetail Author:ZC  2015-04-15")]
        public bool SetLifeStyleDetail(string StyleId, string Module, string CurativeEffect, string SideEffect, string Instruction, string HealthEffect, string Unit, string Redundance)
        {
            try
            {
                bool ret = false;
                ret = CmMstLifeStyleDetail.SetData(_cnCache, StyleId, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, Redundance) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLifeStyleDetail ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table: Cm.MstInsurance Author:LY  2015-06-25")]
        public bool SetInsurance(string Code, string Name, string InputCode, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstInsurance.SetData(_cnCache, Code, Name, InputCode, Redundance, InvalidFlag) == true ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetInsurance ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteData Table: Cm.MstInsurance Author:WF  2015-11-28")]
        public bool DeleteInsurance(string Code)
        {
            try
            {
                bool ret = false;
                ret = CmMstInsurance.DeleteData(_cnCache, Code) == true ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetInsurance ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }
        [WebMethod(Description = "GetInsuranceType Table: Cm.MstInsurance Author:LY  2015-06-25")]
        public DataSet GetInsuranceType()
        {
            try
            {
                DataTable DT_MstInsurance = new DataTable();
                DataSet DS_MstInsurance = new DataSet();
                DT_MstInsurance = CmMstInsurance.GetInsuranceType(_cnCache);
                DS_MstInsurance.Tables.Add(DT_MstInsurance);
                return DS_MstInsurance;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInsuranceType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetInsurance Table: Cm.MstInsurance Author:WF  2015-11-27")]
        public DataSet GetInsurance()
        {
            try
            {
                DataTable DT_MstInsurance = new DataTable();
                DataSet DS_MstInsurance = new DataSet();
                DT_MstInsurance = CmMstInsurance.GetInsurance(_cnCache);
                DS_MstInsurance.Tables.Add(DT_MstInsurance);
                return DS_MstInsurance;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInsurance", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetName Table: Cm.MstInsurance Author:LY  2015-06-25")]
        public string GetInsuranceName(string Code)
        {
            try
            {
                string ret = null;
                ret = CmMstInsurance.GetName(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInsuranceName ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        //#region <" 字典维护界面 集成同步 WF">

        //#region <"科室 字典维护界面 集成同步 WF">
        //[WebMethod(Description = "获取字典表全部信息 Table：Cm.MstDivision Author:WF  2015-07-07")]
        //public DataSet GetDivision()
        //{
        //    try
        //    {
        //        DataTable DT_MstDivision = new DataTable();
        //        DataSet DS_MstDivision = new DataSet();
        //        DT_MstDivision = CmMstDivision.GetDivision(_cnCache);
        //        DS_MstDivision.Tables.Add(DT_MstDivision);
        //        return DS_MstDivision;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "SetData Table：Cm.MstDivision Author:WF  2015-07-07")]
        //public bool SetDivision(int piType, string piCode, string piTypeName, string piName, string piInputCode, string piDescription, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        //{
        //    try
        //    {
        //        bool ret = false;
        //        ret = CmMstDivision.SetData(_cnCache, piType, piCode, piTypeName, piName, piInputCode, piDescription, revUserId, TerminalName, TerminalIP, DeviceType);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "SetData Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        //public bool SetTmpDivisionDict(string HospitalCode, string Type, string Code, string TypeName, string Name, string InputCode, string Description, int Status, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        //{
        //    try
        //    {
        //        bool ret = false;
        //        ret = TmpDivisionDict.SetData(_cnCache, HospitalCode, Type, Code, TypeName, Name, InputCode, Description, Status, revUserId, TerminalName, TerminalIP, DeviceType);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetTmpDivisionDict", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw (ex);
        //    }
        //}


        ////[WebMethod(Description = "ChangeStatus Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        ////public bool ChangeStatusForTmpDivision(string HospitalCode, string Type, string Code, int Status)
        ////{
        ////    try
        ////    {
        ////        bool Flag = false;
        ////        Flag = TmpDivisionDict.ChangeStatus(_cnCache, HospitalCode, Type, Code, Status);
        ////        return Flag;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        ////        return false;
        ////        throw ex;
        ////    }
        ////}

        //[WebMethod(Description = "GetListByStatus Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        //public DataSet GetTmpDivisionByStatus(int Status)
        //{
        //    try
        //    {
        //        DataTable DT_MstDivision = new DataTable();
        //        DataSet DS_MstDivision = new DataSet();
        //        DT_MstDivision = TmpDivisionDict.GetListByStatus(_cnCache, Status);
        //        DS_MstDivision.Tables.Add(DT_MstDivision);
        //        return DS_MstDivision;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpDivisionByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "SetData Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        //public bool SetMpDivisionCmp(string HospitalCode, int Type, string Code, string HZCode, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        //{
        //    try
        //    {
        //        bool ret = false;
        //        ret = MpDivisionCmp.SetData(_cnCache, HospitalCode, Type, Code, HZCode, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "DeleteMpDivisionCmp Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        //public bool DeleteMpDivisionCmp(string HospitalCode, string HZCode)
        //{
        //    try
        //    {
        //        bool Flag = false;
        //        Flag = MpDivisionCmp.Delete(_cnCache, HospitalCode, HZCode);
        //        return Flag;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw ex;
        //    }
        //}

        //[WebMethod(Description = "GetMpDivisionCmp Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        //public DataSet GetMpDivisionCmp()
        //{
        //    try
        //    {
        //        DataTable DT_MstDivision = new DataTable();
        //        DataSet DS_MstDivision = new DataSet();
        //        DT_MstDivision = MpDivisionCmp.GetMpDivisionCmp(_cnCache);
        //        DS_MstDivision.Tables.Add(DT_MstDivision);
        //        return DS_MstDivision;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "GetNamebyCode Table: Cm.MstDivision Author:WF  2015-07-07")]
        //public string GetDivisionNamebyCode(string Code)
        //{
        //    try
        //    {
        //        string ret = null;
        //        ret = CmMstDivision.GetNamebyCode(_cnCache, Code);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNamebyCode ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "GetTypeNamebyType Table: Cm.MstDivision Author:WF  2015-07-07")]
        //public string GetDivisionTypeNamebyType(int Type)
        //{
        //    try
        //    {
        //        string ret = null;
        //        ret = CmMstDivision.GetTypeNamebyType(_cnCache, Type);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //#endregion

        //#region <"诊断 字典维护界面 集成同步 WF">
        //[WebMethod(Description = "SetData Table：Cm.MstDiagnosis Author:YDS  2014-12-03")]
        //public bool SetDiagnosis(string Type, string Code, string TypeName, string Name, string InputCode, string Redundance, int InvalidFlag)
        //{
        //    try
        //    {
        //        bool ret = false;
        //        ret = CmMstDiagnosis.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, Redundance, InvalidFlag);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "获取字典表全部信息 Table：Cm.MstDiagnosis Author:YDS  2014-12-03")]
        //public DataSet GetDiagnosis()
        //{
        //    try
        //    {
        //        DataTable DT_MstDiagnosis = new DataTable();
        //        DataSet DS_MstDiagnosis = new DataSet();
        //        DT_MstDiagnosis = CmMstDiagnosis.GetDiagnosis(_cnCache);
        //        DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
        //        return DS_MstDiagnosis;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "GetNamebyCode Table: Cm.MstDiagnosis Author:WF  2015-07-07")]
        //public string GetDiagnosisNamebyCode(string Code)
        //{
        //    try
        //    {
        //        string ret = null;
        //        ret = CmMstDiagnosis.GetNamebyCode(_cnCache, Code);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNamebyCode ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "GetTypeNamebyType Table: Cm.MstDiagnosis Author:WF  2015-07-07")]
        //public string GetDiagnosisTypeNamebyType(string Type)
        //{
        //    try
        //    {
        //        string ret = null;
        //        ret = CmMstDiagnosis.GetTypeNamebyType(_cnCache, Type);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}


        ////[WebMethod(Description = "ChangeStatus Table：Tmp.DiagnosisDict Author:WF  2015-07-07")]
        ////public bool ChangeStatusForTmpDiagnosis(string HospitalCode, string Code, int Status)
        ////{
        ////    try
        ////    {
        ////        bool Flag = false;
        ////        Flag = TmpDiagnosisDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
        ////        return Flag;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        ////        return false;
        ////        throw ex;
        ////    }
        ////}

        //[WebMethod(Description = "GetListByStatus Table：Tmp.DiagnosisDict Author:WF  2015-07-07")]
        //public DataSet GetTmpDiagnosisByStatus(int Status)
        //{
        //    try
        //    {
        //        DataTable DT_MstDiagnosis = new DataTable();
        //        DataSet DS_MstDiagnosis = new DataSet();
        //        DT_MstDiagnosis = TmpDiagnosisDict.GetListByStatus(_cnCache, Status);
        //        DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
        //        return DS_MstDiagnosis;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpDivisionByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "SetData Table：Mp.DiagnosisCmp Author:WF  2015-07-07")]
        //public bool SetMpDiagnosisCmp(string HospitalCode, string HZCode, string Type, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        //{
        //    try
        //    {
        //        bool ret = false;
        //        ret = MpDiagnosisCmp.SetData(_cnCache, HospitalCode, HZCode, Type, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpDiagnosisCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return false;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "GetMpDiagnosisCmp Table：Mp.DiagnosisCmp Author:WF  2015-07-07")]
        //public DataSet GetMpDiagnosisCmp()
        //{
        //    try
        //    {
        //        DataTable DT_MstDivision = new DataTable();
        //        DataSet DS_MstDivision = new DataSet();
        //        DT_MstDivision = MpDiagnosisCmp.GetMpDiagnosisCmp(_cnCache);
        //        DS_MstDivision.Tables.Add(DT_MstDivision);
        //        return DS_MstDivision;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpDiagnosisCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}


        //#endregion


        //#endregion

        #region <" 用户管理（用户管理） WF ">
        [WebMethod(Description = "Cm.MstUser写数据,用于用户管理页面 Table:Cm.MstUser  Author:ZYF 2015-1-20")]
        // SetData 写数据 ZYF 2015-1-20
        public bool SetMstUserUM(string UserId, string UserName, string Password, string Class, int EndDate, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool IsSaved = false;
                IsSaved = CmMstUser.SetDataUM(_cnCache, UserId, UserName, Password, Class, EndDate, revUserId, TerminalName, TerminalIP, DeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMstUserUM", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除角色信息 Table:Ps.RoleMatch  Author:WF 2015-07-01")]
        // DeleteRoleData 删除角色信息 WF 2015-07-01
        public int DeleteRoleData(string PatientId, string RoleClass)
        {
            try
            {
                int ret = PsRoleMatch.DeleteData(_cnCache, PatientId, RoleClass);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteRoleData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }
        [WebMethod(Description = "Cm.MstUser获取用户的角色 Table:Cm.MstUser  Author:ZC 2015-01-06")]
        // GetClassByUserId 获取用户的角色 Author:ZC 2015-01-06
        public string GetClassByUserId(string UserId)
        {
            string Role = "";
            try
            {
                Role = CmMstUser.GetClassByUserId(_cnCache, UserId);
                return Role;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetClassByUserId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Role;
                throw (ex);
            }
        }


        [WebMethod(Description = "根据用户Id输出Cm.MstUser中用户最新一条有效的基本信息 Table：Cm.MstUser Author:ZYF  2014-12-03")]
        //根据用户Id输出Cm.MstUser中用户最新一条有效的基本信息 ZYF 2014-12-03 WF 2015-05-28
        public DataSet GetUserInfoList(string userId, string userName)
        {
            //string tempName = "";
            DataTable DT_MstUser = new DataTable();
            DataTable DT_MstUser_null = new DataTable();
            DataSet DS_MstUser = new DataSet();
            DataRow dr = DT_MstUser.NewRow();
            try
            {

                if (userId == "")
                {
                    if (userName == "")
                    {
                        DT_MstUser = CmMstUser.GetUserList(_cnCache);
                    }
                    else
                    {
                        DT_MstUser = CmMstUser.GetUserListByName(_cnCache, userName);

                    }
                }
                else
                {
                    DT_MstUser = CmMstUser.GetUserListById(_cnCache, userId);
                    //if (DT_MstUser != null)
                    //{
                    //    if (userName != "")
                    //    {
                    //        dr = DT_MstUser.Rows[0];
                    //        tempName = dr["UserName"].ToString();
                    //        if (tempName != userName)
                    //        {
                    //            DT_MstUser = DT_MstUser_null;
                    //        }
                    //    }
                    //}
                }
                if (DT_MstUser == null)
                {
                    DT_MstUser = DT_MstUser_null;
                }
                DS_MstUser.Tables.Add(DT_MstUser);
                return DS_MstUser;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUserInfoById", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.MstUser写数据 Table:Cm.MstUser  Author:ZYF 2014-12-03")]
        // SetData 写数据 ZYF 2014-12-01
        public bool SetMstUser(string UserId, string UserName, string Password, string Class, string PatientClass, int DoctorClass, int StartDate, int EndDate, DateTime LastLoginDateTime, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool IsSaved = false;
                IsSaved = CmMstUser.SetData(_cnCache, UserId, UserName, Password, Class, PatientClass, DoctorClass, StartDate, EndDate, LastLoginDateTime, revUserId, TerminalName, TerminalIP, DeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.MstUser判断数据是否存在 Table:Cm.MstUser  Author:ZYF 2014-12-08")]
        // CheckUserExist 写数据 ZYF 2014-12-08
        public bool CheckUserExist(string UserId)
        {
            try
            {
                bool exist = false;
                exist = CmMstUser.CheckExist(_cnCache, UserId);
                return exist;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.MstUser获取手机号 Table:Cm.MstUser  Author:WF 2015-06-01")]
        // GetPhoneNoByUserId 获取用户的角色 Author:WF 2015-06-01
        public string GetPhoneNoByUserId(string UserId)
        {
            string PhoneNo = "";
            try
            {
                PhoneNo = CmMstUser.GetPhoneNoByUserId(_cnCache, UserId);
                return PhoneNo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPhoneNoByUserId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return PhoneNo;
                throw (ex);
            }
        }

        [WebMethod(Description = "修改病人姓名 Table：Ps.BasicInfo Author:WF  2015-06-02")]
        public bool SetPatName(string UserId, string UserName)
        {
            try
            {
                bool Flag = false;
                Flag = PsBasicInfo.SetPatName(_cnCache, UserId, UserName);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfo.SetPatName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "修改医生姓名 Table：Ps.DoctorInfoDetail Author:WF  2015-06-02")]
        public bool SetDocName(string UserId, string UserName)
        {
            try
            {
                bool Flag = false;
                Flag = PsDoctorInfo.SetDocName(_cnCache, UserId, UserName);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.SetDocName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获得模块列表字符串 Table：Ps.DoctorInfo Author:WF  2015-06-04")]
        public string GetModuleByDoctorId(string UserId)
        {
            try
            {
                string Module = "";
                Module = PsDoctorInfo.GetModuleByDoctorId(_cnCache, UserId);
                return Module;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetModuleByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        #endregion

        #region <" 用户管理（权限分配） ZYF ">
        [WebMethod(Description = "查询输出Cm.MstRole所有角色名称，用于角色名称选择下拉框 Table：Cm.MstRole Author:ZYF  2014-12-03")]
        //查询输出Cm.MstRole所有角色名称，用于角色名称选择下拉框 ZYF 2014-12-03
        public DataSet GetRoleList()
        {
            try
            {
                DataTable DT_MstRole = new DataTable();
                DataSet DS_MstRole = new DataSet();
                DT_MstRole = CmMstRole.GetRoleList(_cnCache);
                DS_MstRole.Tables.Add(DT_MstRole);
                return DS_MstRole;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetRoleList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "查询输出Cm.MstAuthority中所有权限大类编码和名称 Table：Cm.MstAuthority Author:ZYF  2014-12-03")]
        //查询输出Cm.MstAuthority中所有权限大类编码和名称 ZYF 2014-12-03
        public DataSet GetAuthorityList()
        {
            try
            {
                DataTable DT_MstAuthority = new DataTable();
                DataSet DS_MstAuthority = new DataSet();
                DT_MstAuthority = CmMstAuthority.GetAuthorityList(_cnCache);
                DS_MstAuthority.Tables.Add(DT_MstAuthority);
                return DS_MstAuthority;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAuthorityList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "查询输出Cm.MstAuthority中所有权限大类编码和名称 Table：Cm.Role2Authority,  Author:ZYF  2014-12-03")]
        //查询输出Cm.MstAuthority中所有权限大类编码和名称 ZYF 2014-12-03
        public DataSet GetRoleAuthorityList(string roleCode)
        {
            try
            {
                DataTable DT_MstRoleAuthority = new DataTable();
                DataSet DS_MstRoleAuthority = new DataSet();
                DT_MstRoleAuthority = CmRole2Authority.GetRoleAuthorityList(_cnCache, roleCode);
                DS_MstRoleAuthority.Tables.Add(DT_MstRoleAuthority);
                return DS_MstRoleAuthority;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetRoleAuthorityList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据大分类Code输出所有小分类编码及名称 Table：Cm.MstAuthority Author:ZYF  2014-12-03")]
        //根据大分类Code输出所有小分类编码及名称 ZYF 2014-12-03
        public DataSet GetSubAuthorityList(string AuthorityCode)
        {
            try
            {
                DataTable DT_MstSubAuthority = new DataTable();
                DataSet DS_MstSubAuthority = new DataSet();
                DT_MstSubAuthority = CmMstAuthorityDetail.GetSubAuthorityList(_cnCache, AuthorityCode);
                DS_MstSubAuthority.Tables.Add(DT_MstSubAuthority);
                return DS_MstSubAuthority;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSubAuthorityList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.Role2Authority写数据 Table:Cm.Role2Authority  Author:ZYF 2014-12-03")]
        // Cm.Role2Authority写数据 ZYF 2014-12-03
        public bool SetRole2Authority(string piRoleCode, string piAuthorityCode, string piSubAuthorityCode, string piRedundance, int piInvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool IsSaved = false;
                IsSaved = CmRole2Authority.SetData(_cnCache, piRoleCode, piAuthorityCode, piSubAuthorityCode, piRedundance, piInvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        #endregion

        #region <" 用户管理（医生管理） ZYF ">
        [WebMethod(Description = "根据医生Id或姓名输出Ps.DoctorInfo中医生基本信息的列表 Table：Ps.DoctorInfo Author:ZYF  2014-12-03")]
        //根据医生Id或姓名输出Ps.DoctorInfo中医生基本信息的列表 ZYF 2014-12-03
        public DataSet GetDoctorInfoList(string docId, string docName)
        {
            string tempName = "";
            DataTable DT_MstDoc = new DataTable();
            DataTable DT_MstDoc_null = new DataTable();
            DataSet DS_MstDoc = new DataSet();
            DataRow dr = DT_MstDoc.NewRow();
            try
            {

                if (docId == "")
                {
                    if (docName == "")
                    {
                        DT_MstDoc = PsDoctorInfo.GetDoctorListWithMod(_cnCache);
                    }
                    else
                    {
                        DT_MstDoc = PsDoctorInfo.GetDoctorListByNameWithMod(_cnCache, docName);

                    }
                }
                else
                {
                    DT_MstDoc = PsDoctorInfo.GetDoctorInfoWithMod(_cnCache, docId);
                    if (DT_MstDoc != null)
                    {
                        if (docName != "")
                        {
                            dr = DT_MstDoc.Rows[0];
                            tempName = dr["DoctorName"].ToString();
                            if (tempName != docName)
                            {
                                DT_MstDoc = DT_MstDoc_null;
                            }
                        }
                    }
                }
                if (DT_MstDoc == null)
                {
                    DT_MstDoc = DT_MstDoc_null;
                }
                DS_MstDoc.Tables.Add(DT_MstDoc);
                return DS_MstDoc;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUserInfoById", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据医生Id查询返回Ps.DoctorInfoDetail中医生模块信息 Table：Ps.DoctorInfo Author:ZYF  2014-12-03")]
        //根据医生Id查询返回Ps.DoctorInfoDetail中医生模块信息 ZYF 2014-12-03
        public DataSet GetDoctorModuleList(string DoctorId)
        {
            try
            {
                DataTable DT_DocModule = new DataTable();
                DataSet DS_DocModule = new DataSet();
                DT_DocModule = PsDoctorInfo.GetDoctorModuleList(_cnCache, DoctorId);
                DS_DocModule.Tables.Add(DT_DocModule);
                return DS_DocModule;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDoctorModuleList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "通过Cm.MstInfoItemCategory表查询输出所有模块Code和Name Table：Cm.MstInfoItemCategory Author:ZYF  2014-12-03")]
        //通过Cm.MstInfoItemCategory表查询输出所有模块Code和Name ZYF 2014-12-03
        public DataSet GetModuleList()
        {
            try
            {
                DataTable DT_MstModule = new DataTable();
                DataSet DS_MstModule = new DataSet();
                DT_MstModule = CmMstInfoItemCategory.GetCategoryList(_cnCache);
                DS_MstModule.Tables.Add(DT_MstModule);
                return DS_MstModule;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetModuleList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Ps.DoctorInfo写数据 Table:Ps.DoctorInfo  Author:ZYF 2014-12-09")]
        // SetData 写数据 ZYF 2014-12-09
        public bool SetPsDoctor(string UserId, string UserName, int Birthday, int Gender, string IDNo, int InvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool IsSaved = false;
                IsSaved = PsDoctorInfo.SetData(_cnCache, UserId, UserName, Birthday, Gender, IDNo, InvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "Ps.DoctorInfoDetail写数据 Table:Ps.DoctorInfoDetail  Author:ZYF 2014-12-09")]
        // SetData 写数据 ZYF 2014-12-09
        public bool SetPsDoctorDetail(string Doctor, string CategoryCode, string Value, string Description, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            string ItemCode = "InvalidFlag";
            int ItemSeq = 1;
            try
            {
                bool IsSaved = false;
                IsSaved = PsDoctorInfoDetail.SetData(_cnCache, Doctor, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        #endregion

        #region <" 建档(基本信息) CSQ ">

        [WebMethod(Description = "输入DoctorId，获取没有购买全该医生全部负责模块的病人列表 Table:Ps.BasicInfo  Author:CSQ 2015-01-16")]
        //GetPatListByDoctorId 输入DoctorId，获取没有购买全该医生全部负责模块的病人列表 CSQ 2015-01-16
        public DataSet GetPatListByDoctorId(string DoctorId)
        {
            try
            {
                DataTable DT_PsDoctorInfo = new DataTable();
                DataSet DS_PsDoctorInfo = new DataSet();
                DT_PsDoctorInfo = PsBasicInfo.GetPatListByDoctorId(_cnCache, DoctorId);
                DS_PsDoctorInfo.Tables.Add(DT_PsDoctorInfo);
                return DS_PsDoctorInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatListByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "通过CategoryCode获取医生Id和姓名列表 Table:Ps.DoctorInfoDetail  Author:CSQ 2015-01-07")]
        // GetDoctorListByModule 通过CategoryCode获取医生Id和姓名列表 CSQ 2015-01-07
        public DataSet GetDoctorListByModule(string CategoryCode)
        {
            try
            {
                DataTable DT_PsDoctorInfoDetail = new DataTable();
                DataSet DS_PsDoctorInfoDetail = new DataSet();
                DT_PsDoctorInfoDetail = PsDoctorInfoDetail.GetDoctorListByModule(_cnCache, CategoryCode);

                DataTable list = new DataTable();
                list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
                list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
                list.Columns.Add(new DataColumn("Hospital", typeof(string)));
                list.Columns.Add(new DataColumn("Dept", typeof(string)));

                foreach (DataRow item in DT_PsDoctorInfoDetail.Rows)
                {
                    string uid = item["DoctorId"].ToString();
                    string hospital = CmMstHospital.GetName(_cnCache, PsDoctorInfoDetail.GetValue(_cnCache, uid, "Contact", "Contact001_5", 1));
                    string dept = CmMstDivision.GetNamebyCode(_cnCache, PsDoctorInfoDetail.GetValue(_cnCache, uid, "Contact", "Contact001_8", 1));
                    list.Rows.Add(uid, item["DoctorName"].ToString(), hospital, dept);
                }
                DS_PsDoctorInfoDetail.Tables.Add(list);
                return DS_PsDoctorInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDoctorListByModule", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除医生详细信息表中某个模块的负责患者信息 Table:Ps.DoctorInfoDetail  Author:CSQ 2015-01-06")]
        // DeletePatient 删除医生详细信息表中某个模块的负责患者信息 CSQ 2015-01-06
        public int DeletePatient(string DoctorId, string CategoryCode, string Value)
        {
            try
            {
                int ret = PsDoctorInfoDetail.DeleteData(_cnCache, DoctorId, CategoryCode, Value);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeletePatient", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取体征类型名称列表 Table:Cm.MstVitalSigns  Author:CSQ 2014-12-29")]
        // GetVitalSignsTypeNameList 获取体征类型名称列表 CSQ 2014-12-29
        public DataSet GetVitalSignsTypeNameList()
        {
            try
            {
                DataTable DT_MstVitalSigns = new DataTable();
                DataSet DS_MstVitalSigns = new DataSet();
                DT_MstVitalSigns = CmMstVitalSigns.GetTypeList(_cnCache);
                DS_MstVitalSigns.Tables.Add(DT_MstVitalSigns);
                return DS_MstVitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVitalSignsTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据Type，获取对应体征名称列表 Table:Cm.MstVitalSigns  Author:CSQ 2014-12-29")]
        // GetVitalSignsNameListByType 根据Type，获取对应体征名称列表 CSQ 2014-12-29
        public DataSet GetVitalSignsNameListByType(string Type)
        {
            try
            {
                DataTable DT_MstVitalSigns = new DataTable();
                DataSet DS_MstVitalSigns = new DataSet();
                DT_MstVitalSigns = CmMstVitalSigns.GetNameListByType(_cnCache, Type);
                DS_MstVitalSigns.Tables.Add(DT_MstVitalSigns);
                return DS_MstVitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVitalSignsNameListByType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取医生姓名列表 Table：Ps.DoctorInfo Author:CSQ  2014-12-04")]
        //GetDoctorList 获取医生姓名列表 CSQ 2014-12-04
        public DataSet GetDoctorList()
        {
            try
            {
                DataTable DT_DoctorInfo = new DataTable();
                DataSet DS_DoctorInfo = new DataSet();
                DT_DoctorInfo = PsDoctorInfo.GetDocNameList(_cnCache);
                DS_DoctorInfo.Tables.Add(DT_DoctorInfo);
                return DS_DoctorInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDoctorList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "插入患者基本信息 Table：Ps.BasicInfo Author:CSQ  2014-12-03")]
        //SetPatBasicInfo 插入患者基本信息 CSQ 2014-12-03
        public bool SetPatBasicInfo(string UserId, string UserName, int Birthday, int Gender, int BloodType, string IDNo, string DoctorId, string InsuranceType, int InvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsBasicInfo.SetData(_cnCache, UserId, UserName, Birthday, Gender, BloodType, IDNo, DoctorId, InsuranceType, InvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPatBasicInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "更新患者详细信息 Table:Ps.BasicInfoDetail  Author:CSQ 2014-12-09")]
        // UpdatePatBasicInfoDtl 更新患者详细信息 CSQ 2014-12-09
        public bool UpdatePatBasicInfoDtl(string Patient, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsBasicInfoDetail.SetData(_cnCache, Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdatePatBasicInfoDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "插入患者详细信息 Table:Ps.BasicInfoDetail  Author:CSQ 2014-12-09")]
        //SetPatBasicInfoDtl 插入患者详细信息 CSQ 2014-12-09
        public bool SetPatBasicInfoDtl(string Patient, string CategoryCode, string ItemCode, string Value, string Description, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ItemSeq = 0;
                ItemSeq = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, Patient, CategoryCode, ItemCode) + 1;
                bool ret = false;
                ret = PsBasicInfoDetail.SetData(_cnCache, Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPatBasicInfoDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "删除患者购买的某个模块信息 Table:Ps.BasicInfoDetail  Author:CSQ 2014-12-03")]
        // DeleteModule 删除患者购买的某个模块信息 CSQ 2014-12-03
        public int DeleteModule(string UserId, string CategoryCode, string ItemCode, int ItemSeq)
        {
            try
            {
                int ret = PsBasicInfoDetail.Delete(_cnCache, UserId, CategoryCode, ItemCode, ItemSeq);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteModule", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "删除患者需要关注的体征信息 Table:Ps.VitalSigns Author:CSQ 2014-12-03")]
        // DeleteVitalSignsInfo 删除患者需要关注的体征信息 CSQ 2014-12-03
        public int DeleteVitalSignsInfo(string UserId, string VitalSignsType, string VitalSignsCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ret = PsPatient2VS.DeletePatient2VSInfo(_cnCache, UserId, VitalSignsType, VitalSignsCode, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteVitalSignsInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "插入患者需要关注的体征信息 Table：Ps.Patient2VS Author:CSQ  2014-12-03")]
        //SetPatToVSInfo 插入患者需要关注的体征信息 CSQ 2014-12-03
        public bool SetPatToVSInfo(string UserId, string VitalSignsType, string VitalSignsCode, int InvalidFlag, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsPatient2VS.SetData(_cnCache, UserId, VitalSignsType, VitalSignsCode, InvalidFlag, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPatToVSInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取体征名称列表 Table:Cm.MstVitalSigns  Author:CSQ 2014-12-03")]
        // GetVitalSignsNameList 获取体征名称列表 CSQ 2014-12-03
        public DataSet GetVitalSignsNameList()
        {
            try
            {
                DataTable DT_MstVitalSigns = new DataTable();
                DataSet DS_MstVitalSigns = new DataSet();
                DT_MstVitalSigns = CmMstVitalSigns.GetNameList(_cnCache);
                DS_MstVitalSigns.Tables.Add(DT_MstVitalSigns);
                return DS_MstVitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVitalSignsNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取模块关注详细信息 Table:Cm.MstInfoItem Author:CSQ 2014-12-04")]
        // GetMstInfoItemByCategoryCode 获取模块关注详细信息 CSQ 2014-12-04
        public DataSet GetMstInfoItemByCategoryCode(string CategoryCode)
        {
            try
            {
                DataTable DT_MstInfoItem = new DataTable();
                DataSet DS_MstInfoItem = new DataSet();
                DT_MstInfoItem = CmMstInfoItem.GetMstInfoItemByCategoryCode(_cnCache, CategoryCode);
                DS_MstInfoItem.Tables.Add(DT_MstInfoItem);
                return DS_MstInfoItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMstInfoItemByCategoryCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "自动编号 Table:Cm.MstNumbering Author:CSQ 2014-12-09")]
        // GetNoByNumberingType 自动编号 CSQ 2014-12-09
        public string GetNoByNumberingType(int NumberingType)
        {
            try
            {
                string ret = CmMstNumbering.GetNo(_cnCache, NumberingType, "");
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNoByNumberingType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        [WebMethod(Description = "输入PatientId，获取患者已经购买的模块 Table:Ps.BasicInfoDetail Author:CSQ 2015-04-09")]
        //GetModulesBoughtByPId 输入PatientId，获取患者已经购买的模块 CSQ 2015-04-09
        public DataSet GetModulesBoughtByPId(string PatientId)
        {
            try
            {
                DataTable DT_PsBasicInfoDetail = new DataTable();
                DataSet DS_PsBasicInfoDetail = new DataSet();
                DT_PsBasicInfoDetail = PsBasicInfoDetail.GetModulesByPID(_cnCache, PatientId);
                DS_PsBasicInfoDetail.Tables.Add(DT_PsBasicInfoDetail);
                return DS_PsBasicInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetModulesBoughtByPId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "输入PatientId，获取患者未购买的模块 Table:Ps.BasicInfoDetail Author:CSQ 2015-04-09")]
        //GetModulesUnBoughtByPId 输入PatientId，获取患者未购买的模块 CSQ 2015-04-09
        public DataSet GetModulesUnBoughtByPId(string PatientId)
        {
            try
            {
                DataTable DT_PsBasicInfoDetail = new DataTable();
                DataSet DS_PsBasicInfoDetail = new DataSet();
                DT_PsBasicInfoDetail = PsBasicInfoDetail.GetModulesUnBought(_cnCache, PatientId);
                DS_PsBasicInfoDetail.Tables.Add(DT_PsBasicInfoDetail);
                return DS_PsBasicInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetModulesUnBoughtByPId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "输入PatientId和CategoryCode，获取患者已经购买的某个模块的详细信息 Table:Ps.BasicInfoDetail Author:CSQ 2015-04-09")]
        //GetItemInfoByPIdAndModule 输入PatientId和CategoryCode，获取患者已经购买的某个模块的详细信息 CSQ 2015-04-09
        public DataSet GetItemInfoByPIdAndModule(string PatientId, string CategoryCode)
        {
            try
            {
                DataTable DT_PsBasicInfoDetail = new DataTable();
                DataSet DS_PsBasicInfoDetail = new DataSet();
                DT_PsBasicInfoDetail = PsBasicInfoDetail.GetPatientBasicInfoDetail(_cnCache, PatientId, CategoryCode);
                DS_PsBasicInfoDetail.Tables.Add(DT_PsBasicInfoDetail);
                return DS_PsBasicInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetItemInfoByPIdAndModule", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        #region <" 建档(临床信息) CSQ ">

        [WebMethod(Description = "往数据库中插入患者在就诊医院的就诊号 Table：Ps.UserIdMatch Author:CSQ  20150710")]
        //SetHUserId 往数据库中插入患者在就诊医院的就诊号 CSQ 20150710
        public bool SetHUserId(string UserId, string HUserId, string HospitalCode, string Description, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsUserIdMatch.SetData(_cnCache, UserId, HUserId, HospitalCode, Description, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetHUserId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取患者在就诊医院的就诊号 Table：Ps.UserIdMatch Author:CSQ  20150710")]
        //getLatestHUserIdByHCode 获取患者在就诊医院的就诊号 CSQ 20150710
        public string getLatestHUserIdByHCode(string UserId, string HospitalCode)
        {
            try
            {
                string ret = PsUserIdMatch.getLatestHUserIdByHCode(_cnCache, UserId, HospitalCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "getLatestHUserIdByHCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return "";
                throw ex;
            }
        }

        [WebMethod(Description = "获取科室列表 Table:Cm.MstDivision  Author:CSQ 20150121")]
        // GetDeptList 获取科室列表 CSQ 20150121
        public DataSet GetDeptList(string TypeCode)
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = CmMstDivision.GetDeptList(_cnCache, TypeCode);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDeptList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条住院信息 Table：Ps.InPatientInfo Author:CSQ  20150120")]
        //SetInPatientInfo 往数据库中插入一条住院信息 CSQ 20150120
        public bool SetInPatientInfo(string UserId, string VisitId, int SortNo, DateTime AdmissionDate, DateTime DischargeDate, string HospitalCode, string Department, string Doctor, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsInPatientInfo.SetData(_cnCache, UserId, VisitId, SortNo, AdmissionDate, DischargeDate, HospitalCode, Department, Doctor, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetInPatientInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条门诊信息 Table：Ps.OutPatientInfo Author:CSQ  20150120")]
        //SetOutPatientInfo 往数据库中插入一条门诊信息 CSQ 20150120
        public bool SetOutPatientInfo(string UserId, string VisitId, DateTime ClinicDate, string HospitalCode, string Department, string Doctor, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsOutPatientInfo.SetData(_cnCache, UserId, VisitId, ClinicDate, HospitalCode, Department, Doctor, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetOutPatientInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条住院信息 Table：Ps.InPatientInfo Author:CSQ  20150120")]
        //DeleteInPatientInfo 从数据库中删除一条住院信息 CSQ 20150120
        public int DeleteInPatientInfo(string UserId, string VisitId)
        {
            try
            {
                int ret = PsInPatientInfo.DeleteAll(_cnCache, UserId, VisitId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteInPatientInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条门诊信息 Table：Ps.OutPatientInfo Author:CSQ  20150120")]
        //DeleteOutPatientInfo 从数据库中删除一条门诊信息 CSQ 20150120
        public int DeleteOutPatientInfo(string UserId, string VisitId)
        {
            try
            {
                int ret = PsOutPatientInfo.DeleteAll(_cnCache, UserId, VisitId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteOutPatientInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条症状信息 Table：Ps.Diagnosis Author:LS 2014-12-29")]
        //DeleteDiagnosisInfo 从数据库中删除一条症状信息 LS 2014-12-29
        public int DeleteSymptomsInfo(string UserId, string VisitId, int SymptomsNo)
        {
            try
            {
                int ret = PsSymptoms.DeleteData(_cnCache, UserId, VisitId, SymptomsNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteSymptomsInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取化验类型名称列表 Table:Cm.MstLabTestItems  Author:CSQ 2014-12-29")]
        // GetLabTestItemsTypeNameList 获取化验类型名称列表 CSQ 2014-12-29
        public DataSet GetLabTestItemsTypeNameList()
        {
            try
            {
                DataTable DT_MstLabTestItems = new DataTable();
                DataSet DS_MstLabTestItems = new DataSet();
                DT_MstLabTestItems = CmMstLabTestItems.GetTypeList(_cnCache);
                DS_MstLabTestItems.Tables.Add(DT_MstLabTestItems);
                return DS_MstLabTestItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestItemsTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "根据患者Id，获取诊断信息 Table:Ps.Diagnosis Author:CSQ 2014-12-03")]
        // GetDiagnosisInfoList 根据患者Id，获取诊断信息 CSQ 2014-12-03
        public DataSet GetDiagnosisInfoList(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_Diagnosis = new DataTable();
                DataSet DS_Diagnosis = new DataSet();
                DT_Diagnosis = PsDiagnosis.GetDiagnosisInfo(_cnCache, UserId, VisitId);
                DS_Diagnosis.Tables.Add(DT_Diagnosis);
                return DS_Diagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagnosisInfoList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条诊断信息 Table：Ps.Diagnosis Author:CSQ  2014-12-03")]
        //SetDiagnosisInfo 往数据库中插入一条诊断信息 CSQ 2014-12-03
        public bool SetDiagnosisInfo(string UserId, string VisitId, int DiagnosisType, string DiagnosisNo, string Type, string DiagnosisCode, string Description, string RecordDate, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsDiagnosis.SetData(_cnCache, UserId, VisitId, DiagnosisType, DiagnosisNo, Type, DiagnosisCode, Description, RecordDate, piUserId, piTerminalName, piTerminalIP, piDeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDiagnosisInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条诊断信息 Table：Ps.Diagnosis Author:CSQ  2014-12-03")]
        //DeleteDiagnosisInfo 从数据库中删除一条诊断信息 CSQ 2014-12-03
        public int DeleteDiagnosisInfo(string UserId, string VisitId, int DiagnosisType, string DiagnosisNo)
        {
            try
            {
                int ret = PsDiagnosis.Delete(_cnCache, UserId, VisitId, DiagnosisType, DiagnosisNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteDiagnosisInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取诊断名称列表 Table:Cm.MstDiagnosis Author:CSQ 2014-12-03")]
        // GetDiagNameList 获取诊断名称列表 CSQ 2014-12-03
        public DataSet GetDiagNameList(string Type)
        {
            try
            {
                DataTable DT_MstDiagnosis = new DataTable();
                DataSet DS_MstDiagnosis = new DataSet();
                DT_MstDiagnosis = CmMstDiagnosis.GetDiagnosisList(_cnCache, Type);
                DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
                return DS_MstDiagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取糖尿病药物名称列表 Table:Cm.MstDiabetesDrug Author:CSQ 20150703")]
        // GetDiabetesDrugNameList 获取糖尿病名称列表 CSQ 20150703
        public DataSet GetDiabetesDrugNameList(string Type)
        {
            try
            {
                DataTable DT_MstDiabetesDrug = new DataTable();
                DataSet DS_MstDiabetesDrug = new DataSet();
                DT_MstDiabetesDrug = CmMstDiabetesDrug.GetDiabetesDrugList(_cnCache, Type);
                DS_MstDiabetesDrug.Tables.Add(DT_MstDiabetesDrug);
                return DS_MstDiabetesDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiabetesDrugNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取高血压药物名称列表 Table:Cm.MstHypertensionDrug Author:CSQ 20150703")]
        // GetHypertensionDrugNameList 获取高血压药物名称列表 CSQ 20150703
        public DataSet GetHypertensionDrugNameList(string Type)
        {
            try
            {
                DataTable DT_MstHypertensionDrug = new DataTable();
                DataSet DS_MstHypertensionDrug = new DataSet();
                DT_MstHypertensionDrug = CmMstHypertensionDrug.GetHypertensionDrugList(_cnCache, Type);
                DS_MstHypertensionDrug.Tables.Add(DT_MstHypertensionDrug);
                return DS_MstHypertensionDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHypertensionDrugNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取调脂药药物名称列表 Table:Cm.MstLipidDrug Author:CSQ 20150930")]
        // GetHypertensionDrugNameList 获取调脂药药物名称列表 CSQ 20150930
        public DataSet GetLipidDrugNameList(string Type)
        {
            try
            {
                DataTable DT_MstLipidDrug = new DataTable();
                DataSet DS_MstLipidDrug = new DataSet();
                DT_MstLipidDrug = CmMstLipidDrug.GetLipidDrugList(_cnCache, Type);
                DS_MstLipidDrug.Tables.Add(DT_MstLipidDrug);
                return DS_MstLipidDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLipidDrugNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取降尿酸药药物名称列表 Table:Cm.MstUricAcidReductionDrug Author:CSQ 20150930")]
        // GetAcidDrugNameList 获取降尿酸药药物名称列表 CSQ 20150930
        public DataSet GetAcidDrugNameList(string Type)
        {
            try
            {
                DataTable DT_MstUricAcidReductionDrug = new DataTable();
                DataSet DS_MstUricAcidReductionDrug = new DataSet();
                DT_MstUricAcidReductionDrug = CmMstUricAcidReductionDrug.GetAcidDrugList(_cnCache, Type);
                DS_MstUricAcidReductionDrug.Tables.Add(DT_MstUricAcidReductionDrug);
                return DS_MstUricAcidReductionDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAcidDrugNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取诊断类型名称列表 Table:Cm.MstDiagnosis Author:CSQ 2014-12-03")]
        // GetDiagTypeNameList 获取诊断类型名称列表 CSQ 2014-12-03
        public DataSet GetDiagTypeNameList()
        {
            try
            {
                DataTable DT_MstDiagnosis = new DataTable();
                DataSet DS_MstDiagnosis = new DataSet();
                DT_MstDiagnosis = CmMstDiagnosis.GetTypeList(_cnCache);
                DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
                return DS_MstDiagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取糖尿病药物类型名称列表 Table:Cm.MstDiabetesDrug Author:CSQ 20150703")]
        // GetDiabetesDrugTypeNameList 获取糖尿病药物类型名称列表 CSQ 20150703
        public DataSet GetDiabetesDrugTypeNameList()
        {
            try
            {
                DataTable DT_MstDiabetesDrug = new DataTable();
                DataSet DS_MstDiabetesDrug = new DataSet();
                DT_MstDiabetesDrug = CmMstDiabetesDrug.GetTypeList(_cnCache);
                DS_MstDiabetesDrug.Tables.Add(DT_MstDiabetesDrug);
                return DS_MstDiabetesDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiabetesDrugTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取高血压药物类型名称列表 Table:Cm.MstHypertensionDrug Author:CSQ 20150703")]
        // GetHypertensionDrugTypeNameList 获取高血压药物类型名称列表 CSQ 20150703
        public DataSet GetHypertensionDrugTypeNameList()
        {
            try
            {
                DataTable DT_MstHypertensionDrug = new DataTable();
                DataSet DS_MstHypertensionDrug = new DataSet();
                DT_MstHypertensionDrug = CmMstHypertensionDrug.GetTypeList(_cnCache);
                DS_MstHypertensionDrug.Tables.Add(DT_MstHypertensionDrug);
                return DS_MstHypertensionDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHypertensionDrugTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取调脂药药物类型名称列表 Table:Cm.MstLipidDrug Author:CSQ 20150930")]
        // GetLipidDrugTypeNameList 获取调脂药药物类型名称列表 CSQ 20150930
        public DataSet GetLipidDrugTypeNameList()
        {
            try
            {
                DataTable DT_MstLipidDrug = new DataTable();
                DataSet DS_MstLipidDrug = new DataSet();
                DT_MstLipidDrug = CmMstLipidDrug.GetTypeList(_cnCache);
                DS_MstLipidDrug.Tables.Add(DT_MstLipidDrug);
                return DS_MstLipidDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLipidDrugTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取降尿酸药药物类型名称列表 Table:Cm.MstUricAcidReductionDrug Author:CSQ 20150930")]
        // GetUricAcidReductionDrugTypeNameList 获取降尿酸药药物类型名称列表 CSQ 20150930
        public DataSet GetUricAcidReductionDrugTypeNameList()
        {
            try
            {
                DataTable DT_MstUricAcidReductionDrug = new DataTable();
                DataSet DS_MstUricAcidReductionDrug = new DataSet();
                DT_MstUricAcidReductionDrug = CmMstUricAcidReductionDrug.GetTypeList(_cnCache);
                DS_MstUricAcidReductionDrug.Tables.Add(DT_MstUricAcidReductionDrug);
                return DS_MstUricAcidReductionDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUricAcidReductionDrugTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取患者的就诊信息 Table:Ps.InPatientInfo和Ps.OutPatientInfo Author:CSQ 2015-01-20")]
        // GetClinicalInfoList 获取患者的就诊信息 CSQ 2015-01-20
        public DataSet GetClinicalInfoList(string UserId)
        {
            try
            {
                DataSet DS_ClinicalInfo = new DataSet();
                DataTable DT_InPatientInfo = new DataTable();
                DataTable DT_OutPatientInfo = new DataTable();

                DT_InPatientInfo = PsInPatientInfo.GetInfobyId(_cnCache, UserId);
                DT_OutPatientInfo = PsOutPatientInfo.GetInfobyId(_cnCache, UserId);

                DS_ClinicalInfo.Tables.Add(DT_InPatientInfo);
                DS_ClinicalInfo.Tables.Add(DT_OutPatientInfo);

                return DS_ClinicalInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetClinicalInfoList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条就诊信息 Table：Ps.ClinicalInfo Author:CSQ  2014-12-03")]
        //SetClinicalInfo 往数据库中插入一条就诊信息 CSQ 2014-12-03
        public bool SetClinicalInfo(string UserId, int VisitId, int VisitType, int AdmissionDate, int DischargeDate, string HospitalCode, string Department, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsClinicalInfo.SetData(_cnCache, UserId, VisitId, VisitType, AdmissionDate, DischargeDate, HospitalCode, Department, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetClinicalInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条就诊信息 Table：Ps.ClinicalInfo Author:CSQ  2014-12-03")]
        //DeleteClinicalInfo 从数据库中删除一条就诊信息 CSQ 2014-12-03
        public int DeleteClinicalInfo(string UserId, int VisitId)
        {
            try
            {
                int ret = PsClinicalInfo.Delete(_cnCache, UserId, VisitId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteClinicalInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取医院名称列表 Table:Cm.MstHospital Author:CSQ 2014-12-03")]
        // GetHospitalList 获取医院名称列表 CSQ 2014-12-03
        public DataSet GetHospitalList()
        {
            try
            {
                DataTable DT_MstHospital = new DataTable();
                DataSet DS_MstHospital = new DataSet();
                DT_MstHospital = CmMstHospital.GetHospitalNameList(_cnCache);
                DS_MstHospital.Tables.Add(DT_MstHospital);
                return DS_MstHospital;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHospitalList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据患者Id,获取患者的检查信息  Table:Ps.Examination  Author:CSQ  2014-12-03")]
        // GetExaminationList 根据患者Id,获取患者的检查信息 CSQ 2014-12-03
        public DataSet GetExaminationList(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_Examination = new DataTable();
                DataSet DS_Examination = new DataSet();
                DT_Examination = PsExamination.GetExaminationList(_cnCache, UserId, VisitId);
                DS_Examination.Tables.Add(DT_Examination);
                return DS_Examination;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExaminationList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "更新一条检查信息 Table：Ps.Examination Author:CSQ  2014-12-09")]
        //UpdateExamination 更新一条检查信息 CSQ 2014-12-09
        public bool UpdateExamination(string UserId, string VisitId, int SortNo, string ExamType, string ExamDate, string ItemCode, string ExamPara, string Decription, string Impression, string Recommendation, int IsAbnormal, string Status, string ReportDate, string ImageURL, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsExamination.SetData(_cnCache, UserId, VisitId, SortNo, ExamType, ExamDate, ItemCode, ExamPara, Decription, Impression, Recommendation, IsAbnormal, Status, ReportDate, ImageURL, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateExamination", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条检查信息 Table：Ps.Examination Author:CSQ  2014-12-04")]
        //SetExamination 往数据库中插入一条检查信息 CSQ 2014-12-04
        public bool SetExamination(string UserId, string VisitId, string ExamType, string ExamDate, string ItemCode, string ExamPara, string Decription, string Impression, string Recommendation, int IsAbnormal, string Status, string ReportDate, string ImageURL, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int SortNo = 0;
                //SortNo = PsExamination.GetNextSortNo(_cnCache, UserId) + 1;
                SortNo = PsExamination.GetNextSortNo(_cnCache, UserId, VisitId);     //2014-12-28 ZAM
                bool ret = PsExamination.SetData(_cnCache, UserId, VisitId, SortNo, ExamType, ExamDate, ItemCode, ExamPara, Decription, Impression, Recommendation, IsAbnormal, Status, ReportDate, ImageURL, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExamination", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条检查信息 Table：Ps.Examination Author:CSQ  2014-12-04")]
        //DeleteExamination 从数据库中删除一条检查信息 CSQ 2014-12-04
        public int DeleteExamination(string UserId, string VisitId, int SortNo)
        {
            try
            {
                int ret = PsExamination.DeleteData(_cnCache, UserId, VisitId, SortNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteExamination", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "根据患者Id, 获取患者的化验信息 Table:Ps.LabTest  Author:CSQ  2014-12-03")]
        // GetLabTestList 根据患者Id, 获取患者的化验信息 CSQ 2014-12-03
        public DataSet GetLabTestList(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_LabTest = new DataTable();
                DataSet DS_LabTest = new DataSet();
                DT_LabTest = PsLabTest.GetLabTestList(_cnCache, UserId, VisitId);
                DS_LabTest.Tables.Add(DT_LabTest);
                return DS_LabTest;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "更新化验信息 Table：Ps.LabTest Author:CSQ  2014-12-09")]
        //UpdateLabTest 更新化验信息 CSQ 2014-12-09
        public bool UpdateLabTest(string UserId, string VisitId, string SortNo, string LabItemType, string LabItemCode, string LabTestDate, string Status, string ReportDate, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsLabTest.SetData(_cnCache, UserId, VisitId, SortNo, LabItemType, LabItemCode, LabTestDate, Status, ReportDate, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateLabTest", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条化验信息 Table：Ps.LabTest Author:CSQ  2014-12-04")]
        //SetLabTest 往数据库中插入一条化验信息 CSQ 2014-12-04
        public bool SetLabTest(string UserId, string VisitId, string LabItemType, string LabItemCode, string LabTestDate, string Status, string ReportDate, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                string SortNo = "";
                //SortNo = PsLabTest.GetNextSortNo(_cnCache, UserId) + 1;
                SortNo = PsLabTest.GetNextSortNo(_cnCache, UserId, VisitId).ToString();     //2014-12-28 ZAM
                bool ret = PsLabTest.SetData(_cnCache, UserId, VisitId, SortNo, LabItemType, LabItemCode, LabTestDate, Status, ReportDate, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabTest", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条化验信息 Table：Ps.LabTest Author:CSQ  2014-12-04")]
        //DeleteLabTest 从数据库中删除一条化验信息 CSQ 2014-12-04
        public int DeleteLabTest(string UserId, string VisitId, string SortNo)
        {
            try
            {
                int ret = PsLabTest.DeleteData(_cnCache, UserId, VisitId, SortNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteLabTest", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取药品名称列表 Table:Cm.MstDrug Author:CSQ 2015-05-12")]
        // GetDrugNameList 获取药品名称列表 CSQ 2015-05-12
        public DataSet GetDrugNameList()
        {
            try
            {
                DataTable DT_MstDrug = new DataTable();
                DataSet DS_MstDrug = new DataSet();
                DT_MstDrug = CmMstDrug.GetDrugNameList(_cnCache);
                DS_MstDrug.Tables.Add(DT_MstDrug);
                return DS_MstDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDrugNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据患者Id，获取药物治疗列表  Table:Ps.DrugRecord  Author:CSQ 2014-12-04")]
        // GetDrugRecordList 根据患者Id，获取药物治疗列表 CSQ 2014-12-04
        public DataSet GetDrugRecordList(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_DrugRecord = new DataTable();
                DataSet DS_DrugRecord = new DataSet();
                DT_DrugRecord = PsDrugRecord.GetDrugRecordList(_cnCache, UserId, VisitId);
                DS_DrugRecord.Tables.Add(DT_DrugRecord);
                return DS_DrugRecord;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDrugRecordList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "更新一条药物治疗信息 Table：Ps.DrugRecord Author:CSQ  2014-12-09")]
        //UpdateDrugRecord 更新一条药物治疗信息 CSQ 2014-12-09
        public bool UpdateDrugRecord(string UserId, string VisitId, int OrderNo, int OrderSubNo, int RepeatIndicator, string OrderClass,
            string OrderCode, string OrderContent, decimal Dosage, string DosageUnits, string Administration, DateTime StartDateTime, DateTime StopDateTime,
            int Duration, string DurationUnits, string Frequency, int FreqCounter, int FreqInteval, string FreqIntevalUnit, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsDrugRecord.SetData(_cnCache, UserId, VisitId, OrderNo, OrderSubNo, RepeatIndicator, OrderClass,
                    OrderCode, OrderContent, Dosage, DosageUnits, Administration, StartDateTime, StopDateTime, Duration, DurationUnits, Frequency, FreqCounter,
                    FreqInteval, FreqIntevalUnit, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateDrugRecord", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条药物治疗信息 Table：Ps.DrugRecord Author:CSQ  2014-12-04")]
        //SetDrugRecord 往数据库中插入一条药物治疗信息 CSQ 2014-12-04
        public bool SetDrugRecord(string UserId, string VisitId, int OrderSubNo, int RepeatIndicator, string OrderClass,
            string OrderCode, string OrderContent, decimal Dosage, string DosageUnits, string Administration, DateTime StartDateTime, DateTime StopDateTime,
            int Duration, string DurationUnits, string Frequency, int FreqCounter, int FreqInteval, string FreqIntevalUnit, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int OrderNo = 0;
                //OrderNo = PsDrugRecord.GetNextOrderNo(_cnCache, UserId) + 1;
                OrderNo = PsDrugRecord.GetNextOrderNo(_cnCache, UserId, VisitId);    //2014-12-28 ZAM
                bool ret = PsDrugRecord.SetData(_cnCache, UserId, VisitId, OrderNo, OrderSubNo, RepeatIndicator, OrderClass,
                    OrderCode, OrderContent, Dosage, DosageUnits, Administration, StartDateTime, StopDateTime, Duration, DurationUnits, Frequency, FreqCounter,
                    FreqInteval, FreqIntevalUnit, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDrugRecord", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条药物治疗信息 Table：Ps.DrugRecord Author:CSQ 2014-12-04")]
        // DeleteDrugRecord 从数据库中删除一条药物治疗信息 CSQ 2014-12-04
        public int DeleteDrugRecord(string UserId, string VisitId, int OrderNo, int OrderSubNo)
        {
            try
            {
                int ret = PsDrugRecord.DeleteData(_cnCache, UserId, VisitId, OrderNo, OrderSubNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteDrugRecord", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取检查项目名称列表 Table:Cm.MstExaminationItem  Author:CSQ 2014-12-04")]
        // GetExamItemNameList 获取检查项目名称列表 CSQ 2014-12-04
        public DataSet GetExamItemNameList(string Type)
        {
            try
            {
                DataTable DT_MstExaminationItem = new DataTable();
                DataSet DS_MstExaminationItem = new DataSet();
                DT_MstExaminationItem = CmMstExaminationItem.GetNameListByType(_cnCache, Type);
                DS_MstExaminationItem.Tables.Add(DT_MstExaminationItem);
                return DS_MstExaminationItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamItemNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取化验项目名称列表 Table:Cm.MstLabTestItems  Author:CSQ 2014-12-04")]
        // GetLabTestItemsNameList 获取化验项目名称列表 CSQ 2014-12-04
        public DataSet GetLabTestItemsNameList(string Type)
        {
            try
            {
                DataTable DT_MstLabTestItems = new DataTable();
                DataSet DS_MstLabTestItems = new DataSet();
                DT_MstLabTestItems = CmMstLabTestItems.GetNameList(_cnCache, Type);
                DS_MstLabTestItems.Tables.Add(DT_MstLabTestItems);
                return DS_MstLabTestItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestItemsNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取检查类型名称列表 Table:Cm.MstExaminationItem  Author:CSQ 2014-12-04")]
        // GetExamItemTypeNameList 获取检查类型名称列表 CSQ 2014-12-04
        public DataSet GetExamItemTypeNameList()
        {
            try
            {
                DataTable DT_MstExaminationItem = new DataTable();
                DataSet DS_MstExaminationItem = new DataSet();
                DT_MstExaminationItem = CmMstExaminationItem.GetTypeList(_cnCache);
                DS_MstExaminationItem.Tables.Add(DT_MstExaminationItem);
                return DS_MstExaminationItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamItemTypeNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取检查参数列表 Table:Ps.ExamDetails  Author:CSQ 2014-12-04")]
        // GetExamDtlList 获取检查参数列表 CSQ 2014-12-04
        public DataSet GetExamDtlList(string UserId, string VisitId, string SortNo, string ItemCode)
        {
            try
            {
                DataTable DT_ExamDetails = new DataTable();
                DataSet DS_ExamDetails = new DataSet();
                DT_ExamDetails = PsExamDetails.GetExamDetails(_cnCache, UserId, VisitId, SortNo, ItemCode);
                DS_ExamDetails.Tables.Add(DT_ExamDetails);
                return DS_ExamDetails;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamDtlList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条检查信息具体参数 Table：Ps.ExamDetails Author:CSQ  2014-12-04")]
        public bool SetExamDtl(string UserId, string VisitId, int SortNo, string Code, string Value, int IsAbnormal, string Unit, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsExamDetails.SetData(_cnCache, UserId, VisitId, SortNo, Code, Value, IsAbnormal, Unit, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExamDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条检查参数信息 Table：Ps.ExamDetails Author:CSQ 2014-12-04")]
        // DeleteExamDtl 从数据库中删除一条检查参数信息 CSQ 2014-12-04
        public int DeleteExamDtl(string UserId, string VisitId, int SortNo, string Code)
        {
            try
            {
                int ret = PsExamDetails.DeleteData(_cnCache, UserId, VisitId, SortNo, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteExamDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取检查参数项目名称列表 Table:Cm.MstExaminationSubItem  Author:CSQ 2014-12-04")]
        // GetExamSubItemNameList 获取检查参数项目名称列表 CSQ 2014-12-04
        public DataSet GetExamSubItemNameList(string Code)
        {
            try
            {
                DataTable DT_MstExaminationSubItem = new DataTable();
                DataSet DS_MstExaminationSubItem = new DataSet();
                DT_MstExaminationSubItem = CmMstExaminationSubItem.GetNameList(_cnCache, Code);
                DS_MstExaminationSubItem.Tables.Add(DT_MstExaminationSubItem);
                return DS_MstExaminationSubItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamSubItemNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取化验参数列表 Table:Ps.LabTestDetails  Author:CSQ 20150625")]
        // GetLabTestDtlList 获取化验参数列表 CSQ 20150625
        public DataSet GetLabTestDtlList(string UserId, string VisitId, string SortNo)
        {
            try
            {
                DataTable DT_LabTestDetails = new DataTable();
                DataSet DS_LabTestDetails = new DataSet();
                DT_LabTestDetails = PsLabTestDetails.GetLabTestDetails(_cnCache, UserId, VisitId, SortNo);
                DS_LabTestDetails.Tables.Add(DT_LabTestDetails);
                return DS_LabTestDetails;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestDtlList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条化验信息具体参数 Table：Ps.LabTestDetails Author:CSQ  2014-12-04")]
        public bool SetLabTestDtl(string UserId, string VisitId, string SortNo, string Code, string Value, int IsAbnormal, string Unit, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsLabTestDetails.SetData(_cnCache, UserId, VisitId, SortNo, Code, Value, IsAbnormal, Unit, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabTestDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "从数据库中删除一条化验信息具体参数 Table：Ps.LabTestDetails Author:CSQ 2014-12-04")]
        // DeteleLabTestDtl 从数据库中删除一条化验信息具体参数 CSQ 2014-12-04
        public int DeteleLabTestDtl(string UserId, string VisitId, string SortNo, string Code)
        {
            try
            {
                int ret = PsLabTestDetails.DeleteData(_cnCache, UserId, VisitId, SortNo, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeteleLabTestDtl", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取化验参数项目名称列表 Table:Cm.MstLabTestSubItems Author:CSQ 20150625")]
        // GetLabTestSubItemNameList 获取化验参数项目名称列表 CSQ 20150625
        public DataSet GetLabTestSubItemNameList()
        {
            try
            {
                DataTable DT_MstLabTestSubItems = new DataTable();
                DataSet DS_MstLabTestSubItems = new DataSet();
                DT_MstLabTestSubItems = CmMstLabTestSubItems.GetNameList(_cnCache);
                DS_MstLabTestSubItems.Tables.Add(DT_MstLabTestSubItems);
                return DS_MstLabTestSubItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestSubItemNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Ps.DoctorInfoDetail写入负责患者信息 Table:Ps.DoctorInfoDetail  Author:CSQ 2014-12-09")]
        //SetPsDoctorDetailOnPat Ps.DoctorInfoDetail写入负责患者信息 CSQ 2014-12-09
        public bool SetPsDoctorDetailOnPat(string Doctor, string CategoryCode, string Value, string Description, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            string ItemCode = "Patient";
            try
            {
                int ItemSeq = 0;
                ItemSeq = PsDoctorInfoDetail.GetMaxItemSeq(_cnCache, Doctor, CategoryCode, ItemCode) + 1;
                bool IsSaved = false;
                IsSaved = PsDoctorInfoDetail.SetData(_cnCache, Doctor, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        #endregion

        #region <" 患者信息（BasicInfo） ZC">
        [WebMethod(Description = "获取患者基本信息、模块信息 Author:ZC 2014-12-05")]
        //获取患者基本信息、模块信息 Author:ZC 2014-12-05
        public PatientBasicInfo GetPatBasicInfo(string UserId)
        {
            try
            {
                string module = "";
                PatientBasicInfo patientInfo = new PatientBasicInfo();
                CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, UserId);
                patientInfo.UserId = UserId;
                if (patientList != null)
                {
                    patientInfo.UserName = patientList[0];
                    patientInfo.Age = patientList[1];
                    patientInfo.Gender = patientList[2];
                    patientInfo.BloodType = patientList[3];
                    patientInfo.InsuranceType = patientList[6];
                    patientInfo.Birthday = patientList[7];
                    patientInfo.GenderText = patientList[8];
                    patientInfo.BloodTypeText = patientList[9];
                    patientInfo.InsuranceTypeText = patientList[10];
                }

                DataTable modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                for (int i = 0; i < modules.Rows.Count; i++)
                {
                    module = module + "|" + modules.Rows[i][1];
                }
                if (module != "")
                {
                    module = module.Substring(1, module.Length - 1);
                }
                patientInfo.Module = module;
                return patientInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatBasicInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        [WebMethod(Description = "获取患者未处理警报数 Author:ZC 2014-12-08")]
        //获取患者未处理警报数 Author:ZC 2014-12-08
        public int GetUntreatedAlertAmount(string UserId)
        {
            int UntreatedAlertAmount = 0;
            try
            {
                UntreatedAlertAmount = WnTrnAlertRecord.GetUntreatedAlertAmount(_cnCache, UserId);
                return UntreatedAlertAmount;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUntreatedAlertAmount", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return UntreatedAlertAmount;
                throw ex;
            }
        }

        [WebMethod(Description = "获取关注等级 Author:ZC 2014-12-08")]
        //获取关注等级 Author:ZC 2014-12-08
        public int GetCareLevel(string UserId, string ModuleType)
        {
            int CareLevel = 0;
            try
            {
                CareLevel = PsSpecialList.GetCareLevel(_cnCache, ModuleType, UserId);
                return CareLevel;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetCareLevel", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return CareLevel;
                throw ex;
            }
        }
        #endregion

        #region <" 患者信息（症状管理） WF ">
        [WebMethod(Description = "仅输入PId，获得已有症状列表 Table:Ps.Symptoms  Author:WF 2015-1-20")]
        // 已有症状列表 WF 2014-12-03
        public DataSet GetSymptomsListByPId(string UserId)
        {
            try
            {
                DataTable DT_SymptomsList = new DataTable();
                DataSet DS_SymptomsList = new DataSet();
                DT_SymptomsList = PsSymptoms.GetSymptomsListByPId(_cnCache, UserId);
                DS_SymptomsList.Tables.Add(DT_SymptomsList);
                return DS_SymptomsList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptomsListByPId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取Cm.MstSymptoms中的症状类型列表 Table：Cm.MstSymptoms Author:WF  2014-12-03")]
        //获取Cm.MstSymptoms中的症状类型列表 WF 2014-12-03
        public DataSet GetSymptomsTypeList()
        {
            try
            {
                DataTable DT_SymptomsType = new DataTable();
                DataSet DS_SymptomsType = new DataSet();
                DT_SymptomsType = CmMstSymptoms.GetTypeList(_cnCache);
                DS_SymptomsType.Tables.Add(DT_SymptomsType);
                return DS_SymptomsType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptomsTypeList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获得症状名称列表 Table:Cm.MstSymptoms  Author:WF 2014-12-03")]
        // GetSymptomsNameList 获得症状名称列表 WF 2014-12-03
        public DataSet GetSymptomsNameList(string Type)
        {
            try
            {
                DataTable DT_NameList = new DataTable();
                DataSet DS_NameList = new DataSet();
                DT_NameList = CmMstSymptoms.GetNameList(_cnCache, Type);
                DS_NameList.Tables.Add(DT_NameList);
                return DS_NameList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptomsNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "添加新症状 Table：Ps.Symptoms Author:WF  2014-12-03")]
        //添加新症状 WF 2014-12-03
        public bool SetSymptomsInfo(string UserId, string VisitId, string SymptomsType, string SymptomsCode, string Description, int RecordDate, int RecordTime, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int SynptomsNo = 0;
                SynptomsNo = PsSymptoms.GetMaxSortNo(_cnCache, UserId, VisitId) + 1;
                bool ret = false;
                ret = PsSymptoms.SetData(_cnCache, UserId, VisitId, SynptomsNo, SymptomsType, SymptomsCode, Description, RecordDate, RecordTime, piUserId, piTerminalName, piTerminalIP, piDeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetSymptomsInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "更新症状 Table：Ps.Symptoms Author:WF  2014-12-03")]
        //更新症状 WF 2014-12-03
        public bool UpdateSymptomsInfo(string UserId, string VisitId, int SynptomsNo, string SymptomsType, string SymptomsCode, string Description, int RecordDate, int RecordTime, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = false;
                ret = PsSymptoms.SetData(_cnCache, UserId, VisitId, SynptomsNo, SymptomsType, SymptomsCode, Description, RecordDate, RecordTime, piUserId, piTerminalName, piTerminalIP, piDeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateSymptomsInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "已有症状列表 Table:Ps.Symptoms  Author:WF 2014-12-03")]
        // 已有症状列表 WF 2014-12-03
        public DataSet GetSymptomsList(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_SymptomsList = new DataTable();
                DataSet DS_SymptomsList = new DataSet();
                DT_SymptomsList = PsSymptoms.GetSymptomsList(_cnCache, UserId, VisitId);
                DS_SymptomsList.Tables.Add(DT_SymptomsList);
                return DS_SymptomsList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSymptomsList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <" 患者信息（治疗方案） WF ">
        [WebMethod(Description = "获得特殊人群列表 Table：Cm.MstTreatment Author:WF  2015-1-22")]
        //获得疗程列表 WF 2015-1-22
        public DataSet GetDurationNameList()
        {
            try
            {
                DataTable DT_DurationNameList = new DataTable();
                DataSet DS_DurationNameList = new DataSet();
                DT_DurationNameList = CmMstType.GetTypeList(_cnCache, "Duration");
                DS_DurationNameList.Tables.Add(DT_DurationNameList);
                return DS_DurationNameList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDurationNameList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除治疗方案 Table：Ps.Treatment Author:WF  2015-1-22")]
        //删除治疗方案 WF 2014-12-03
        public int DeleteTreatmentInfo(string UserId, int SortNo)
        {
            try
            {
                int ret = 2;
                ret = PsTreatment.DeleteData(_cnCache, UserId, SortNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteTreatmentInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 2;
                throw ex;
            }
        }

        [WebMethod(Description = "获取治疗目标 Table：Cm.MstTreatment Author:WF  2014-12-03")]
        //获取治疗目标 WF 2014-12-03
        public DataSet GetTreatmentGoalsList()
        {
            try
            {
                DataTable DT_TreatmentGoalsList = new DataTable();
                DataSet DS_TreatmentGoalsList = new DataSet();
                DT_TreatmentGoalsList = CmMstTreatment.GetNameList(_cnCache, "TreatmentGoals");
                DS_TreatmentGoalsList.Tables.Add(DT_TreatmentGoalsList);
                return DS_TreatmentGoalsList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTreatmentGoalsList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取治疗措施 Table：Cm.MstTreatment Author:WF  2014-12-03")]
        //获取治疗措施 WF 2014-12-03
        public DataSet GetTreatmentActionList()
        {
            try
            {
                DataTable DT_TreatmentGoalsList = new DataTable();
                DataSet DS_TreatmentGoalsList = new DataSet();
                DT_TreatmentGoalsList = CmMstTreatment.GetNameList(_cnCache, "TreatmentAction");
                DS_TreatmentGoalsList.Tables.Add(DT_TreatmentGoalsList);
                return DS_TreatmentGoalsList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTreatmentActionList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获得特殊人群列表 Table：Cm.MstTreatment Author:WF  2014-12-03")]
        //获得特殊人群列表 WF 2014-12-03
        public DataSet GetGroupList()
        {
            try
            {
                DataTable DT_TreatmentGoalsList = new DataTable();
                DataSet DS_TreatmentGoalsList = new DataSet();
                DT_TreatmentGoalsList = CmMstTreatment.GetNameList(_cnCache, "Group");
                DS_TreatmentGoalsList.Tables.Add(DT_TreatmentGoalsList);
                return DS_TreatmentGoalsList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetGroupList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "添加新治疗方案 Table：Ps.Treatment Author:WF  2014-12-03")]
        //添加新治疗方案 WF 2014-12-03
        public bool SetTreatmentInfo(string UserId, int TreatmentGoal, int TreatmentAction, int Group, string TreatmentPlan, string Description, DateTime TreatTime, string Duration, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int SortNo = 0;
                SortNo = PsTreatment.GetNextSortNo(_cnCache, UserId);
                bool ret = false;
                ret = PsTreatment.SetData(_cnCache, UserId, SortNo, TreatmentGoal, TreatmentAction, Group, TreatmentPlan, Description, TreatTime, Duration, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetTreatmentInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "更新治疗方案 Table：Ps.Treatment Author:WF  2014-12-03")]
        //更新治疗方案 WF 2014-12-03
        public bool UpdateTreatmentInfo(string UserId, int SortNo, int TreatmentGoal, int TreatmentAction, int Group, string TreatmentPlan, string Description, DateTime TreatTime, string Duration, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = PsTreatment.SetData(_cnCache, UserId, SortNo, TreatmentGoal, TreatmentAction, Group, TreatmentPlan, Description, TreatTime, Duration, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateTreatmentInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "已有治疗情况列表 Table：Ps.Treatment Author:WF  2014-12-03")]
        //已有治疗情况列表 WF 2014-12-03
        public DataSet GetTreatmentList(string UserId)
        {
            try
            {
                DataTable DT_TreatmentList = new DataTable();
                DataSet DS_TreatmentList = new DataSet();
                DT_TreatmentList = PsTreatment.GetTreatmentList(_cnCache, UserId);
                DS_TreatmentList.Tables.Add(DT_TreatmentList);
                return DS_TreatmentList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTreatmentList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <" 患者信息（警报记录） WF ">
        [WebMethod(Description = "处置状态置位 Table：Wn.TrnAlertRecord Author:WF  2015-1-22")]
        //处置状态置位 WF 2014-12-03
        public bool SetTrnProcessFlag(string UserId, int SortNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool ret = false;
                ret = WnTrnAlertRecord.SetProcessFlag(_cnCache, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetProcessFlag", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "添加警报  Table：Wn.TrnAlertRecord Author:WF  2014-12-03")]
        //添加警报 WF 2014-12-03
        public bool SetTrnAlertRecord(string UserId, string AlertItemCode, DateTime AlertDateTime, int AlertType, int PushFlag, int ProcessFlag, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                int SortNo = 0;
                SortNo = WnTrnAlertRecord.GetMaxSortNo(_cnCache, UserId) + 1;
                bool ret = false;
                ret = WnTrnAlertRecord.SetData(_cnCache, UserId, SortNo, AlertItemCode, AlertDateTime, AlertType, PushFlag, ProcessFlag, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetTrnAlertRecord", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获得已有警报列表，根据处置状态和警报时间倒序排列 Table：Wn.TrnAlertRecord Author:WF  2014-12-03")]
        //获得已有警报列表，根据处置状态和警报时间倒序排列 WF 2014-12-03
        public DataSet GetTrnAlertRecordList(string UserId)
        {
            try
            {
                DataTable DT_TrnAlertRecordList = new DataTable();
                DataSet DS_TrnAlertRecordList = new DataSet();
                DT_TrnAlertRecordList = WnTrnAlertRecord.GetTrnAlertRecordList(_cnCache, UserId);
                DS_TrnAlertRecordList.Tables.Add(DT_TrnAlertRecordList);
                return DS_TrnAlertRecordList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTrnAlertRecordList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "给警报患者发信 Table：Mb.MessageRecord Author:WF  2014-12-03")]
        //给警报患者发信 WF 2014-12-03
        public bool SendAlertMessage(string MessageNo, int MessageType, string SendBy, string Title, string Reciever, string Content, int SetCondition, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool ret = false;
                ret = MbMessageRecord.SetMessage(_cnCache, MessageNo, MessageType, SendBy, Title, Reciever, Content, SetCondition, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SendAlertMessage", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "推送状态置位 Table：Wn.TrnAlertRecord Author:WF  2014-12-03")]
        //推送状态置位 WF 2014-12-03
        public bool SetPushFlag(string UserId, int SortNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool ret = false;
                ret = WnTrnAlertRecord.SetPushFlag(_cnCache, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPushFlag", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        #endregion

        #region <" 信箱 GL ">
        #region < " 网页部分 ">

        #region <" 信箱母版 GL ">

        [WebMethod(Description = "对未读消息计数，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public int GetUnreadCount(string UserId)
        {
            try
            {
                int UnreadCount = 0;
                UnreadCount = MbMessageRecord.GetMailCount(_cnCache, UserId);
                return UnreadCount;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUnreadCount", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "对草稿箱中的消息计数，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public int GetDraftCount(string UserId)
        {
            try
            {
                int UnsentCount = 0;
                UnsentCount = MbMessageRecord.GetDraftCount(_cnCache, UserId);
                return UnsentCount;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDraftCount", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "根据用户Id获取用户姓名，表：Cm.MstUser，Author:GL 2014-12-03")]
        public string GetUserName(string UserId)
        {
            try
            {
                string UserName = string.Empty;
                UserName = CmMstUser.GetNameByUserId(_cnCache, UserId);
                return UserName;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUserName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        #endregion

        #region <" 联系人列表 GL ">
        [WebMethod(Description = "Doctor获取对应联系人列表，Table, 表：Ps.DoctorInfoDetail，Author:GL 2015-1-23")]
        public DataSet GetConForDoctor(string UserId, string CategoryCode)
        {
            try
            {
                DataTable DT_ContactList = new DataTable();
                DataSet DS_ContactList = new DataSet();
                DT_ContactList = PsDoctorInfoDetail.GetPatientsByDoctorId(_cnCache, UserId, CategoryCode);
                DS_ContactList.Tables.Add(DT_ContactList);
                return DS_ContactList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetConForDoctor", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Patient获取对应联系人列表，表：Ps.BasicInfoDetail，Author:GL 2015-1-23")]
        public DataSet GetConForPatient(string UserId, string CategoryCode)
        {
            try
            {
                DataTable DT_ContactList = new DataTable();
                DataSet DS_ContactList = new DataSet();
                DT_ContactList = PsBasicInfoDetail.GetDoctorsByPatientId(_cnCache, UserId, CategoryCode);
                DS_ContactList.Tables.Add(DT_ContactList);
                return DS_ContactList;
            }

            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetConForPatient", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        [WebMethod(Description = "根据用户Id获取身份类别，表：Cm.MstUser，Author:GL 2014-12-04")]
        public string GetClass(string UserId)
        {
            try
            {
                //ZAM 2014-12-08 由于CmMstUser.GetUserInfoByUserId返回值变动为DataTable，取值做出相应修改
                //string IdentityClass = CmMstUser.GetUserInfoByUserId(_cnCache, UserId)[3];
                DataTable DT_MstUser = new DataTable();
                DT_MstUser = CmMstUser.GetUserInfoByUserId(_cnCache, UserId);
                string IdentityClass = string.Empty;

                if (DT_MstUser != null)
                {
                    IdentityClass = DT_MstUser.Rows[0]["Class"].ToString();
                }
                return IdentityClass;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetClass", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "Administrator获取对应联系人列表，Table, 表：Cm.MstUser，Author:GL 2014-12-04")]
        public DataSet GetConForAdmin()
        {
            try
            {
                DataTable Dt1 = new DataTable();
                Dt1.Columns.Add(new DataColumn("piUserId", typeof(string)));
                Dt1.Columns.Add(new DataColumn("piUserName", typeof(string)));
                DataTable Dt2 = CmMstUser.GetUserList(_cnCache);
                foreach (System.Data.DataRow item in Dt2.Rows)
                {
                    Dt1.Rows.Add(item[0], item[1]);
                }
                DataSet Ds = new DataSet();
                Ds.Tables.Add(Dt1);
                return Ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetConForAdmin", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <" 草稿箱 GL ">

        [WebMethod(Description = "获取草稿箱消息列表，Table，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public DataSet GetDraftList(string UserId)
        {
            try
            {
                DataTable DT_DraftList = new DataTable();
                DataSet DS_DraftList = new DataSet();
                DT_DraftList = MbMessageRecord.GetDraftList(_cnCache, UserId);
                DS_DraftList.Tables.Add(DT_DraftList);
                return DS_DraftList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDraftList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "将发送成功或存入草稿箱中的消息写入数据库，Table，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public bool SetMessage(string MessageNo, int MessageType, string SendBy, string Title, string Reciever, string Content, int SetCondition, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool SetFlag = false;
                SetFlag = MbMessageRecord.SetMessage(_cnCache, MessageNo, MessageType, SendBy, Title, Reciever, Content, SetCondition, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                return SetFlag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMessage", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取消息详细信息，表：Mb.MessageRecord，Author:GL 2014-12-24")]
        public Message GetMessageDetail(string MessageNo)
        {
            try
            {
                Message Meg = new Message();
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = MbMessageRecord.GetMessageDetail(_cnCache, MessageNo);
                if (list != null)
                {
                    Meg.SendBy = list[0];
                    Meg.SendByName = list[1];
                    Meg.SendDateTime = list[2];
                    Meg.Title = list[3];
                    Meg.Content = list[4];
                    Meg.Reciever = list[5];
                    Meg.RecieverName = list[6];
                }
                return Meg;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMessageDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除草稿箱信息，表：Mb.MessageRecord，Author:GL 2015-01-14")]
        public bool DeleteDraft(string MessageNo)
        {
            try
            {
                bool flag = false;
                int ret = MbMessageRecord.DeleteDraft(_cnCache, MessageNo);
                if (ret == 1)
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteDraft", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }
        #endregion

        #region <" 收信 GL ">

        [WebMethod(Description = "获取已收到的消息列表，Table，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public DataSet GetReceiveList(string UserId)
        {
            try
            {
                DataTable DT_ReceiveList = new DataTable();
                DataSet DS_ReceiveList = new DataSet();
                DT_ReceiveList = MbMessageRecord.GetReceiveList(_cnCache, UserId);
                DS_ReceiveList.Tables.Add(DT_ReceiveList);
                return DS_ReceiveList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReceiveList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "收信页面中，点击查看消息时，使消息状态从未读变为已读，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public bool ChangeReadStatus(string MessageNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool CRFlag = false;
                CRFlag = MbMessageRecord.ChangeReadStatus(_cnCache, MessageNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                return CRFlag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeReadStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        #endregion

        #region <" 已发送 GL ">

        [WebMethod(Description = "获取已发送消息列表，Table，表：Mb.MessageRecord，Author:GL 2014-12-03")]
        public DataSet GetHaveSentList(string UserId)
        {
            try
            {
                DataTable DT_HaveSentList = new DataTable();
                DataSet DS_HaveSentList = new DataSet();
                DT_HaveSentList = MbMessageRecord.GetHaveSentList(_cnCache, UserId);
                DS_HaveSentList.Tables.Add(DT_HaveSentList);
                return DS_HaveSentList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHaveSentList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #endregion

        #region < " 移动部分 ">
        [WebMethod(Description = "将消息写入数据库，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public bool SetSMS(string SendBy, string Reciever, string Content, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = false;
                int flag = 0;
                flag = MbMessageRecord.SetSMS(_cnCache, SendBy, Reciever, Content, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetSMS", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "根据专员Id获取其对应所有患者未读消息总数，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public int GetSMSCountForAll(string DoctorId)
        {
            try
            {
                int ret = 0;
                ret = MbMessageRecord.GetSMSCountForAll(_cnCache, DoctorId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSMSCountForAll", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "获取一对一未读消息数，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public int GetSMSCountForOne(string Reciever, string SendBy)
        {
            try
            {
                int ret = 0;
                ret = MbMessageRecord.GetSMSCountForOne(_cnCache, Reciever, SendBy);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSMSCountForOne", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "根据专员Id获取患者消息列表，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public DataSet GetSMSList(string DoctorId, string CategoryCode)
        {
            try
            {
                DataTable DT = new DataTable();
                DataSet DS = new DataSet();
                DT = MbMessageRecord.GetSMSList(_cnCache, DoctorId, CategoryCode);
                DS.Tables.Add(DT);
                return DS;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSMSList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取消息对话，按时间先后排列，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public DataSet GetSMSDialogue(string Reciever, string SendBy)
        {
            try
            {
                DataTable DT = new DataTable();
                DataSet DS = new DataSet();
                DT = MbMessageRecord.GetSMSDialogue(_cnCache, Reciever, SendBy);
                DS.Tables.Add(DT);
                return DS;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSMSDialogue", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "改写消息阅读状态，表：Mb.MessageRecord，Author:GL 2015-04-07")]
        public bool SetSMSRead(string Reciever, string SendBy, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = false;
                int flag = 0;
                flag = MbMessageRecord.SetSMSRead(_cnCache, Reciever, SendBy, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetSMSRead", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "根据专员Id和患者Id获取专员收到的最新一条消息，表：Mb.MessageRecord，Author:GL 2015-04-10")]
        public Message GetLatestSMS(string DoctorId, string PatientId)
        {
            try
            {
                Message Meg = new Message();
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = MbMessageRecord.GetLatestSMS(_cnCache, DoctorId, PatientId);
                if (list != null)
                {
                    Meg.MessageNo = list[0];
                    Meg.Content = list[1];
                    Meg.SendDateTime = list[2];
                    Meg.SendByName = list[3];
                    Meg.Flag = list[4];
                }
                return Meg;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLatestSMS", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "改写消息推送状态，表：Mb.MessageRecord，Author:GL 2015-04-24")]
        public bool ChangePushStatus(string MessageNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = false;
                int flag = 0;
                flag = MbMessageRecord.ChangePushStatus(_cnCache, MessageNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangePushStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取推送标志，表：Mb.MessageRecord，Author:GL 2015-04-24")]
        public string GetSMSFlag(string MessageNo)
        {
            try
            {
                string SMSFlag = "4"; //默认已推送
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = MbMessageRecord.GetMessageDetail(_cnCache, MessageNo);
                if (list != null)
                {
                    SMSFlag = list[7];
                }
                return SMSFlag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSMSFlag", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取融云Token，Author:GL 2015-04-24")]
        public string GetToken(string UserId, string UserName, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            string Token = "";
            Token = CmMstUserDetail.GetTokenInCache(_cnCache, UserId); //从数据库获取
            if (String.IsNullOrEmpty(Token))
            {
                Token = RongCloudServer.GetToken("sfci50a7chxxi", "piLP0BOVjlRW", UserId, UserName, ""); //从融云获取
                CmMstUserDetail.SetToken(_cnCache, Token, UserId, piUserId, piTerminalName, piTerminalIP, piDeviceType); //将Token写入数据库
            }
            return Token;
        }

        [WebMethod(Description = "获取本机IP，Author:GL 2015-05-15")]
        public string getLocalmachineIPAddress()
        {
            //string strHostName = Dns.GetHostName();
            //IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

            //IPAddress Temp = ipEntry.AddressList[0];
            //foreach (IPAddress ip in ipEntry.AddressList)
            //{
            //    //IPV4
            //    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //    {
            //        Temp = ip;
            //    }
            //}
            //return Temp.ToString();
            return " ";
        }

        #endregion
        #endregion

        #region <基本信息 ZCY>
        [WebMethod(Description = "根据用户名获取用户详细信息 Table：Ps.BasicInfoDetail Author:ZCY  2015-4-14")]
        public PatientDetailInfo1 GetPatientDetailInfo(string UserId)
        {
            try
            {
                string module = "";
                PatientDetailInfo1 PatientDetailInfo = new PatientDetailInfo1();
                CacheSysList GetPatientDetailInfoList = PsBasicInfoDetail.GetPatientDetailInfo(_cnCache, UserId);
                PatientDetailInfo.UserId = UserId;
                if (GetPatientDetailInfoList != null)
                {
                    PatientDetailInfo.PhoneNumber = GetPatientDetailInfoList[0];
                    if (PatientDetailInfo.PhoneNumber == null)
                    {
                        PatientDetailInfo.PhoneNumber = "";
                    }
                    PatientDetailInfo.HomeAddress = GetPatientDetailInfoList[1];
                    if (PatientDetailInfo.HomeAddress == null)
                    {
                        PatientDetailInfo.HomeAddress = "";
                    }
                    PatientDetailInfo.Occupation = GetPatientDetailInfoList[2];
                    if (PatientDetailInfo.Occupation == null)
                    {
                        PatientDetailInfo.Occupation = "";
                    }
                    PatientDetailInfo.Nationality = GetPatientDetailInfoList[3];
                    if (PatientDetailInfo.Nationality == null)
                    {
                        PatientDetailInfo.Nationality = "";
                    }
                    PatientDetailInfo.EmergencyContact = GetPatientDetailInfoList[4];
                    if (PatientDetailInfo.EmergencyContact == null)
                    {
                        PatientDetailInfo.EmergencyContact = "";
                    }
                    PatientDetailInfo.EmergencyContactPhoneNumber = GetPatientDetailInfoList[5];
                    if (PatientDetailInfo.EmergencyContactPhoneNumber == null)
                    {
                        PatientDetailInfo.EmergencyContactPhoneNumber = "";
                    }
                    PatientDetailInfo.PhotoAddress = GetPatientDetailInfoList[6];
                    if (PatientDetailInfo.PhotoAddress == null)
                    {
                        PatientDetailInfo.PhotoAddress = "";
                    }
                    PatientDetailInfo.IDNo = GetPatientDetailInfoList[7];
                    if (PatientDetailInfo.IDNo == null)
                    {
                        PatientDetailInfo.IDNo = "";
                    }
                    PatientDetailInfo.Height = GetPatientDetailInfoList[8];
                    if (PatientDetailInfo.Height == null)
                    {
                        PatientDetailInfo.Height = "";
                    }
                    PatientDetailInfo.Weight = GetPatientDetailInfoList[9];
                    if (PatientDetailInfo.Weight == null)
                    {
                        PatientDetailInfo.Weight = "";
                    }
                }

                DataTable modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                for (int i = 0; i < modules.Rows.Count; i++)
                {
                    module = module + "|" + modules.Rows[i][1];
                }
                if (module != "")
                {
                    module = module.Substring(1, module.Length - 1);
                }
                PatientDetailInfo.Module = module;
                return PatientDetailInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetPatientDetailInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }

        }

        [WebMethod(Description = "根据用户名获取医生身份信息 Table:Ps.DoctorInfoDetail Author:ZCY  2015-4-29")]
        public DoctorDetailInfo1 GetDoctorDetailInfo(string Doctor)
        {
            try
            {
                DoctorDetailInfo1 DetailInfo = new DoctorDetailInfo1();
                CacheSysList GetDetailInfoList = PsDoctorInfoDetail.GetDoctorInfoDetail(_cnCache, Doctor);
                if (GetDetailInfoList != null)
                {
                    DetailInfo.IDNo = GetDetailInfoList[0];
                    if (DetailInfo.IDNo == null)
                    {
                        DetailInfo.IDNo = "";
                    }
                    DetailInfo.PhotoAddress = GetDetailInfoList[1];
                    if (DetailInfo.PhotoAddress == null)
                    {
                        DetailInfo.PhotoAddress = "";
                    }
                    DetailInfo.UnitName = GetDetailInfoList[2];
                    if (DetailInfo.UnitName == null)
                    {
                        DetailInfo.UnitName = "";
                    }
                    DetailInfo.JobTitle = GetDetailInfoList[3];
                    if (DetailInfo.JobTitle == null)
                    {
                        DetailInfo.JobTitle = "";
                    }
                    DetailInfo.Level = GetDetailInfoList[4];
                    if (DetailInfo.Level == null)
                    {
                        DetailInfo.Level = "";
                    }
                    DetailInfo.Dept = GetDetailInfoList[5];
                    if (DetailInfo.Dept == null)
                    {
                        DetailInfo.Dept = "";
                    }
                    DetailInfo.GeneralScore = GetDetailInfoList[8];
                    if (DetailInfo.GeneralScore == null)
                    {
                        DetailInfo.GeneralScore = "";
                    }
                    DetailInfo.ActivityDegree = GetDetailInfoList[9];
                    if (DetailInfo.ActivityDegree == null)
                    {
                        DetailInfo.ActivityDegree = "";
                    }
                    DetailInfo.GeneralComment = GetDetailInfoList[10];
                    if (DetailInfo.GeneralComment == null)
                    {
                        DetailInfo.GeneralComment = "";
                    }
                    DetailInfo.commentNum = GetDetailInfoList[11];
                    if (DetailInfo.commentNum == null)
                    {
                        DetailInfo.commentNum = "";
                    }
                    DetailInfo.AssessmentNum = GetDetailInfoList[13];
                    if (DetailInfo.AssessmentNum == null)
                    {
                        DetailInfo.AssessmentNum = "";
                    }
                    DetailInfo.MSGNum = GetDetailInfoList[14];
                    if (DetailInfo.MSGNum == null)
                    {
                        DetailInfo.MSGNum = "";
                    }
                    DetailInfo.AppointmentNum = GetDetailInfoList[15];
                    if (DetailInfo.AppointmentNum == null)
                    {
                        DetailInfo.AppointmentNum = "";
                    }
                    DetailInfo.Activedays = GetDetailInfoList[16];
                    if (DetailInfo.Activedays == null)
                    {
                        DetailInfo.Activedays = "";
                    }
                }
                return DetailInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.GetDoctorInfoDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        #endregion

        #region <" 患者信息1（BasicInfo）LS ">

        [WebMethod(Description = "插入患者详细信息 Table：Ps.BasicInfoDetail Author:LS  2014-12-03")]
        //插入患者详细信息 LS 2014-12-03
        public bool SetPatBasicInfoDetail(string Patient, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsBasicInfoDetail.SetData(_cnCache, Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPatBasicInfoDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "获取患者购买的模块下详细信息   Table:Ps.BasicInfoDetail  Author:LS  2014-12-04")]
        // GetPatBasicInfoDtlList 获取患者购买的模块下详细信息  LS  2014-12-04
        public DataSet GetPatBasicInfoDtlList(string UserId)
        {
            try
            {
                DataSet DS_BasicInfoDetail = new DataSet();
                DataTable DT_Modules = new DataTable();
                DT_Modules = PsBasicInfoDetail.GetModulesByPID(_cnCache, UserId);
                if (DT_Modules.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow item in DT_Modules.Rows)
                    {
                        string CategoryCode = item[0].ToString();
                        DataTable ModulesdetailedTable = new DataTable();
                        ModulesdetailedTable = PsBasicInfoDetail.GetPatientBasicInfoDetail(_cnCache, UserId, CategoryCode);
                        DS_BasicInfoDetail.Tables.Add(ModulesdetailedTable);
                    }
                }
                return DS_BasicInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatBasicInfoDtlList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "删除患者模块信息 Table：Ps.BasicInfoDetail Author:LY  2015-7-9")]
        //插入患者详细信息 LS 2014-12-03
        public int DeleteModuleDetail(string Patient, string CategoryCode)
        {
            try
            {
                int ret = PsBasicInfoDetail.DeleteAll(_cnCache, Patient, CategoryCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteModuleDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "同步患者购买模块下的某些信息   Table:Ps.Examination, Ps.LabTestDetail  Author:LY  2015-07-15")]
        // SynBasicInfoDetail 同步患者购买模块下的某些信息  LY  2015-07-15
        public DataSet SynBasicInfoDetail(string UserId)
        {
            try
            {
                DataSet DS_SynDetail = new DataSet();
                DataTable DT_SynDetail = new DataTable();

                DT_SynDetail = PsExamination.GetNewExamForM1(_cnCache, UserId);
                DS_SynDetail.Tables.Add(DT_SynDetail);
                DT_SynDetail = PsLabTestDetails.GetNewLabTestForM1(_cnCache, UserId);
                DS_SynDetail.Tables.Add(DT_SynDetail);

                return DS_SynDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SynBasicInfoDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "同步患者糖尿病模块下的某些信息   Table: Ps.LabTestDetail  Author:SYF  2015-09-28")]
        // SynBasicInfoDetailForM2 同步患者糖尿病模块下的某些信息  SYF  2015-09-28
        public DataSet SynBasicInfoDetailForM2(string UserId)
        {
            try
            {
                DataSet DS_SynDetail = new DataSet();
                DataTable DT_SynDetail = new DataTable();
                DT_SynDetail = PsLabTestDetails.GetNewLabTestForM2(_cnCache, UserId);
                if (DT_SynDetail != null)
                {
                    DS_SynDetail.Tables.Add(DT_SynDetail);
                }
                return DS_SynDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SynBasicInfoDetailForM2", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "同步患者心力衰竭模块下的某些信息   Table: Ps.LabTestDetail,Ps.Examination  Author:SYF  2015-09-30")]
        // SynBasicInfoDetailForM3 同步患者心力衰竭模块下的某些信息  SYF  2015-09-30
        public DataSet SynBasicInfoDetailForM3(string UserId)
        {
            try
            {
                DataSet DS_SynDetail = new DataSet();
                DataTable DT_SynDetail = new DataTable();

                DT_SynDetail = PsExamination.GetNewExamForM3(_cnCache, UserId);
                if (DT_SynDetail != null)
                {
                    DS_SynDetail.Tables.Add(DT_SynDetail);
                }

                DT_SynDetail = PsLabTestDetails.GetNewLabTestForM3(_cnCache, UserId);
                if (DT_SynDetail != null)
                {
                    DS_SynDetail.Tables.Add(DT_SynDetail);
                }
                return DS_SynDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SynBasicInfoDetailForM3", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <" 患者信息2（ClinicalInfo）LS">

        [WebMethod(Description = "获取患者的就诊信息   Table:Ps.ClinicalInfo  Author:LS  2014-12-03")]
        // GetClinicalInfo 取患者的就诊信息  LS  2014-12-03
        public DataSet GetClinicalInfo(string UserId)
        {
            try
            {
                DataTable DT_ClinicalInfo = new DataTable();
                DataSet DS_ClinicalInfo = new DataSet();
                DT_ClinicalInfo = PsClinicalInfo.GetClinicalInfo(_cnCache, UserId);
                DS_ClinicalInfo.Tables.Add(DT_ClinicalInfo);
                return DS_ClinicalInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetClinicalInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取患者的诊断信息   Table:Ps.Diagnosis  Author:LS  2014-12-03")]
        // GetClinicalInfo 取患者的就诊信息  LS  2014-12-03
        public DataSet GetDiagnosisInfo(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_Diagnosis = new DataTable();
                DataSet DS_Diagnosis = new DataSet();
                DT_Diagnosis = PsDiagnosis.GetDiagnosisInfo(_cnCache, UserId, VisitId);
                DS_Diagnosis.Tables.Add(DT_Diagnosis);
                return DS_Diagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagnosisInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取患者的检查信息   Table:Ps.Examination  Author:LS  2014-12-03")]
        // GetExaminationInfo 获取患者的检查信息  LS  2014-12-03
        public DataSet GetExaminationInfo(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_Examination = new DataTable();
                DataSet DS_Examination = new DataSet();
                DT_Examination = PsExamination.GetExaminationList(_cnCache, UserId, VisitId);
                DS_Examination.Tables.Add(DT_Examination);
                return DS_Examination;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExaminationList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取患者的检验信息   Table:Ps.LabTest  Author:LS  2014-12-03")]
        // GetLabTestInfo 获取患者的检验信息  LS  2014-12-03
        public DataSet GetLabTestInfo(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_LabTest = new DataTable();
                DataSet DS_LabTest = new DataSet();
                DT_LabTest = PsLabTest.GetLabTestList(_cnCache, UserId, VisitId);
                DS_LabTest.Tables.Add(DT_LabTest);
                return DS_LabTest;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabTestList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取患者的用药信息   Table:Ps.DrugRecord  Author:LS  2014-12-03")]
        // GetDrugRecord 获取患者的用药信息  LS  2014-12-03
        public DataSet GetDrugRecord(string UserId, string VisitId)
        {
            try
            {
                DataTable DT_DrugRecord = new DataTable();
                DataSet DS_DrugRecord = new DataSet();
                DT_DrugRecord = PsDrugRecord.GetDrugRecordList(_cnCache, UserId, VisitId);
                DS_DrugRecord.Tables.Add(DT_DrugRecord);
                return DS_DrugRecord;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDrugRecordList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        #endregion

        #region <" 患者信息3（ClinicalInfo）LS ">

        [WebMethod(Description = "获取某生理参数的警戒阈值 Table:Wn.MstBasicAlert  Author:LS 2015-01-23")]
        // GetWnMstAlert 获取某项目编码的当前警戒阈值  LS  2014-12-04
        public WnMstAlert GetWnMstBasicAlert(string AlertItemCode)
        {
            try
            {
                WnMstAlert alert = new WnMstAlert();
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = WnMstBasicAlert.GetWnMstBasicAlert(_cnCache, AlertItemCode);

                if (list != null)
                {
                    alert.AlertItemName = list[0];
                    alert.Min = list[1];
                    alert.Max = list[2];
                    alert.Units = list[3];
                    alert.Remarks = list[4];
                }
                return alert;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetWnMstBasicAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取UserId和AlertItemCode下，最大SortNo Table:Wn.MstPersonalAlert  Author:LS 2014-12-26")]
        // GetMPAMaxSortNo 获取UserId和AlertItemCode下，最大SortNo  LS  2014-12-04  
        public int GetMPAMaxSortNo(string UserId, string AlertItemCode)
        {
            int maxsort = 0;
            try
            {
                maxsort = WnMstPersonalAlert.GetMaxSortNo(_cnCache, UserId, AlertItemCode);
                return maxsort;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMPAMaxSortNo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return maxsort;
                throw (ex);
            }
        }


        [WebMethod(Description = "插入用户所用的体征参数项 Table:PsPatient2VS  Author:LS 2014-12-25")]
        // SetPsPatient2VS 插入用户所用的体征参数项  LS  2014-12-25
        public bool SetPsPatient2VS(string UserId, string VitalSignsType, string VitalSignsCode, int InvalidFlag, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                bool ret = PsPatient2VS.SetData(_cnCache, UserId, VitalSignsType, VitalSignsCode, InvalidFlag, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPsPatient2VS", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "插入用户体征信息 Table:Ps.VitalSigns  Author:LS 2014-12-25")]
        // SetPatientVitalSigns 插入用户体征信息  LS  2014-12-25
        public bool SetPatientVitalSigns(string UserId, int RecordDate, int RecordTime, string ItemType, string ItemCode, string Value, string Unit, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsVitalSigns.SetData(_cnCache, UserId, RecordDate, RecordTime, ItemType, ItemCode, Value, Unit, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPatientVitalSigns", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "获取用户需要关注的体征项目编码和名称 Table:Ps.Patient2VS  Author:LS 2014-12-04")]
        // GetPatient2VS 获取某个分类的类别 ZAM 2014-12-03
        public DataSet GetPatient2VS(string UserId)
        {
            try
            {
                DataTable DT_Patient2VS = new DataTable();
                DataSet DS_Patient2VS = new DataSet();
                DT_Patient2VS = PsPatient2VS.GetPatient2VS(_cnCache, UserId);
                DS_Patient2VS.Tables.Add(DT_Patient2VS);
                return DS_Patient2VS;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatient2VS", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取用户需要关注的体征项目编码和名称 Table:Ps.VitalSigns  Author:LS 2014-12-04")]
        // GetPatientVitalSigns 获取用户需要关注的体征项目编码和名称  LS  2014-12-04
        public DataSet GetPatientVitalSigns(string UserId, string ItemType, string ItemCode)
        {
            try
            {
                DataTable DT_VitalSigns = new DataTable();
                DataSet DS_VitalSigns = new DataSet();
                DT_VitalSigns = PsVitalSigns.GetPatientVitalSigns(_cnCache, UserId, ItemType, ItemCode);
                DS_VitalSigns.Tables.Add(DT_VitalSigns);
                return DS_VitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientVitalSigns", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取用户体征值和阈值 Table:Ps.VitalSigns  Author:LS 2014-12-04")]
        // GetPatientVitalSigns 获取用户体征值和阈值  LS  2014-12-04
        public DataSet GetPatientVitalSignsAndThreshold(string UserId, string ItemType, string ItemCode)
        {
            try
            {
                DataTable DT_VitalSigns = new DataTable();
                DataSet DS_VitalSigns = new DataSet();
                DT_VitalSigns = PsVitalSigns.GetPatientVitalSignsAndThreshold(_cnCache, UserId, ItemType, ItemCode);
                DS_VitalSigns.Tables.Add(DT_VitalSigns);
                return DS_VitalSigns;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientVitalSignsAndThreshold", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取某项目编码的当前警戒阈值 Table:Wn.MstBasicAlert、Wn.MstPersonalAlert  Author:LS 2014-12-04")]
        // GetWnMstAlert 获取某项目编码的当前警戒阈值  LS  2014-12-04
        public WnMstAlert GetWnMstAlert(string UserId, string AlertItemCode, int CheckDate)
        {
            try
            {
                WnMstAlert alert = new WnMstAlert();
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = WnMstPersonalAlert.GetWnMstPersonalAlert(_cnCache, UserId, AlertItemCode, CheckDate);
                if (list == null)
                {
                    list = WnMstBasicAlert.GetWnMstBasicAlert(_cnCache, AlertItemCode);
                }
                if (list != null)
                {
                    alert.AlertItemName = list[0];
                    alert.Min = list[1];
                    alert.Max = list[2];
                    alert.Units = list[3];
                    alert.Remarks = list[4];
                }
                return alert;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetWnMstAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "医生设置患者某项生理参数的阈值 Table：Wn.MstPersonalAlert Author:LS  2014-12-04")]
        //医生设置患者某项生理参数的阈值 LS 2014-12-04
        public bool SetWnMstPersonalAlert(string UserId, string AlertItemCode, int SortNo, decimal Min, decimal Max, string Units, int StartDate, int EndDate, string Remarks, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            try
            {
                bool ret = WnMstPersonalAlert.SetData(_cnCache, UserId, AlertItemCode, SortNo, Min, Max, Units, StartDate, EndDate, Remarks, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetWnMstPersonalAlert", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "获取已有评估结果列表 Table:Ps.TreatmentIndicators  Author:LS 2014-12-04")]
        // GetPsTreatmentIndicators 已有评估结果列表 LS 2014-12-04
        public DataSet GetPsTreatmentIndicators(string UserId)
        {
            try
            {
                DataTable DT_TreatmentIndicators = new DataTable();
                DataSet DS_TreatmentIndicators = new DataSet();
                DT_TreatmentIndicators = PsTreatmentIndicators.GetPsTreatmentIndicators(_cnCache, UserId);
                DS_TreatmentIndicators.Tables.Add(DT_TreatmentIndicators);
                return DS_TreatmentIndicators;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTreatmentIndicators", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "已有评估结果列表中的参数项 Table:Ps.Parameters  Author:LS 2014-12-04")]
        // GetParameters 已有评估结果列表中的参数项 LS 2014-12-04
        public DataSet GetParameters(string UserId, int SortNo)
        {
            try
            {
                string Indicators = UserId + "||" + SortNo.ToString();
                DataTable DT_Parameter = new DataTable();
                DataSet DS_Parameter = new DataSet();
                DT_Parameter = PsParameters.GetParameters(_cnCache, Indicators);
                DS_Parameter.Tables.Add(DT_Parameter);
                return DS_Parameter;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetParameters", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        #endregion

        #region <" 患者信息7（ClinicalInfo）LS ">

        [WebMethod(Description = "医生/患者设置每日任务 Table：Ps.Reminder Author:LS  2014-12-04")]
        //医生/患者设置每日任务 LS 2014-12-04（修改：ZC 2014-12-25)
        public bool UpdateReminder(string PatientId, string ReminderNo, int ReminderType, int SortNo, string Content, int AlertMode, DateTime StartDateTime, int NextDate, int NextTime, int Interval, int InvalidFlag, string Description, string CreatedBy, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, StartDateTime, NextDate, NextTime, Interval, InvalidFlag, Description, CreatedBy, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }


        [WebMethod(Description = "已有评估结果列表中的参数项 Table:Ps.Reminder  Author:LS 2014-12-04")]
        // GetReminder 已有评估结果列表中的参数项 LS 2014-12-04（修改：ZC 2014-12-25)
        public DataSet GetReminder(string PatientId)
        {
            try
            {
                DataTable DT_Reminder = new DataTable();
                DataSet DS_Reminder = new DataSet();
                DT_Reminder = PsReminder.GetReminder(_cnCache, PatientId);
                DS_Reminder.Tables.Add(DT_Reminder);
                return DS_Reminder;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteReminder Table：Ps.Reminder Author:ZC  2014-12-25")]
        public int DeleteReminder(string piPatientId, string piReminderNo, string UserId, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ret = 0;
                ret = PsReminder.SetInvalidFlag(_cnCache, piPatientId, piReminderNo, UserId, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        //SetReminder 往数据库中插入一条每日任务 ZC 2014-12-25
        public bool SetReminder(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, string StartDateTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                bool ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, 0, DateTime.Parse(StartDateTime), 0, 0, 0, 0, "", Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Once(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, DateTime OnceDateTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DateTime oDT = OnceDateTime;
                int nextDate = Convert.ToInt32(oDT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(oDT.ToString("HHmmss"));
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = OnceDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                if (IsToday(OnceDateTime))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, 0, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Once", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Everyday(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, string EveryDayTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DateTime edT = DateTime.Parse(EveryDayTime);
                DateTime startDT = DateTime.Parse(StartDateTime);
                TimeSpan span = startDT.Subtract(edT);
                int interval = 60 * 24;
                if (span.TotalDays >= 0)
                {
                    edT = edT.AddDays(span.Days + 1);
                }
                int nextDate = Convert.ToInt32(edT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(edT.ToString("HHmmss"));
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = "每天" + EveryDayTime;
                if (IsToday(edT))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                edT = edT.AddDays(1);
                nextDate = Convert.ToInt32(edT.ToString("yyyyMMdd"));
                nextTime = Convert.ToInt32(edT.ToString("HHmmss"));
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, interval, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Everyday", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Weekly(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, string WeeklyWeek, string WeeklyTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                int interval = 7 * 24 * 60;
                DateTime wDT = DateTime.Parse(StartDateTime);
                string week_E = wDT.DayOfWeek.ToString();
                string week = ConvertWeek(week_E);
                int days = Convert.ToInt16(week) - Convert.ToInt16(WeeklyWeek);
                if (days > 0)
                {
                    wDT = wDT.AddDays(7 - days);
                    wDT = DateTime.Parse(wDT.ToString("yyyy-MM-dd") + " " + WeeklyTime);
                }
                else if (days == 0)
                {
                    DateTime weeklyT = DateTime.Parse(wDT.ToString("yyyy-MM-dd") + " " + WeeklyTime);
                    TimeSpan span = wDT.Subtract(weeklyT);
                    if (span.TotalDays >= 0)
                    {
                        wDT = wDT.AddDays(7);
                    }
                }
                else
                {
                    wDT = DateTime.Parse(wDT.ToString("yyyy-MM-dd") + " " + WeeklyTime);
                }
                int nextDate = Convert.ToInt32(wDT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(wDT.ToString("HHmmss"));
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = ConvertWeek_C(week_E) + "," + WeeklyTime;
                if (IsToday(wDT))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                wDT = wDT.AddDays(7);
                nextDate = Convert.ToInt32(wDT.ToString("yyyyMMdd"));
                nextTime = Convert.ToInt32(wDT.ToString("HHmmss"));
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, interval, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Weekly", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Monthly(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, string MonthlyDay, string MonthlyTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DateTime mDT = DateTime.Parse(StartDateTime);
                DateTime monthDT;
                int days = DateTime.DaysInMonth(mDT.Year, mDT.Month);
                if (days >= Convert.ToInt16(MonthlyDay))
                {
                    monthDT = DateTime.Parse(mDT.Year.ToString() + "-" + mDT.Month.ToString() + "-" + MonthlyDay + " " + MonthlyTime);
                    TimeSpan span = mDT.Subtract(monthDT);
                    if (span.TotalDays >= 0)
                    {
                        monthDT = monthDT.AddMonths(1);
                    }
                }
                else
                {
                    monthDT = DateTime.Parse(mDT.AddMonths(1).Year.ToString() + "-" + mDT.AddMonths(1).Month.ToString() + "-" + MonthlyDay + " " + MonthlyTime);
                }
                int nextDate = Convert.ToInt32(monthDT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(monthDT.ToString("HHmmss"));
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = "每月" + MonthlyDay + "日," + MonthlyTime;
                if (IsToday(monthDT))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                monthDT = monthDT.AddMonths(1);
                nextDate = Convert.ToInt32(monthDT.ToString("yyyyMMdd"));
                nextTime = Convert.ToInt32(monthDT.ToString("HHmmss"));
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, 0, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Monthly", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Annual(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, string AnnualMonth, string AnnualDay, string AnnualTime, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DateTime annualStartDT = DateTime.Parse(StartDateTime);
                DateTime annualDT = DateTime.Parse(annualStartDT.Year.ToString() + "-" + AnnualMonth + "-" + AnnualDay + " " + AnnualTime);

                TimeSpan span = annualStartDT.Subtract(annualDT);
                if (span.TotalDays >= 0)
                {
                    annualDT = annualDT.AddYears(1);
                }
                int nextDate = Convert.ToInt32(annualDT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(annualDT.ToString("HHmmss"));
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = "每年" + AnnualMonth + "月" + AnnualDay + "日," + AnnualTime;
                if (IsToday(annualDT))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                annualDT = annualDT.AddYears(1);
                nextDate = Convert.ToInt32(annualDT.ToString("yyyyMMdd"));
                nextTime = Convert.ToInt32(annualDT.ToString("HHmmss"));
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, 0, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Annual", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "往数据库中插入一条每日任务 Table：Ps.Reminder Author:ZC  2015-01-20")]
        public bool SetReminder_Interval(int Type, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, string StartDateTime, int FreqYear, int FreqMonth, int FreqDay, int FreqHour, int FreqMunite, string Creater, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DateTime iDT = DateTime.Parse(StartDateTime);
                int nextDate = Convert.ToInt32(iDT.ToString("yyyyMMdd"));
                int nextTime = Convert.ToInt32(iDT.ToString("HHmmss"));
                int interval = (FreqYear * 365 + FreqMonth * 30 + FreqDay) * 12 * 60 + FreqHour * 60 + FreqMunite;
                if (Type == 1)
                {
                    ReminderNo = CmMstNumbering.GetNo(_cnCache, 7, "");
                }
                if (Creater == null || Creater == "")
                {
                    Creater = revUserId;
                }
                string Description = "间隔：每" + (FreqYear != 0 ? FreqYear + "年" : "") + (FreqMonth != 0 ? FreqMonth + "月" : "")
                    + (FreqDay != 0 ? FreqDay + "天" : "") + (FreqHour != 0 ? FreqHour + "小时" : "")
                    + (FreqMunite != 0 ? FreqMunite + "分种" : "") + "提醒一次";
                while (IsToday(iDT))
                {
                    ret = PsTaskList.SetData(_cnCache, PatientId, ReminderNo, nextDate, nextTime, 0, Description, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                    iDT = iDT.AddMinutes(interval);
                    nextDate = Convert.ToInt32(iDT.ToString("yyyyMMdd"));
                    nextTime = Convert.ToInt32(iDT.ToString("HHmmss"));
                }
                ret = PsReminder.SetData(_cnCache, PatientId, ReminderNo, ReminderType, Content, AlertMode, DateTime.Parse(StartDateTime), nextDate, nextTime, interval, 0, Description, Creater, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetReminder_Interval", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }
        #endregion

        #region <" 时间轴 LS 2015-01-22 ">

        [WebMethod(Description = "获取某项目生理参数的名称单位等 Table:Cm.MstVitalSigns  Author:LS 2015-01-19")]
        // GetDataByTC 获取某项目生理参数的名称单位等  LS  2015-01-19
        public VitalSigns GetDataByTC(string Type, string Code)
        {
            try
            {
                VitalSigns VitalSign = new VitalSigns();
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = CmMstVitalSigns.GetDataByTC(_cnCache, Type, Code);
                if (list != null)
                {
                    VitalSign.Type = list[0];
                    VitalSign.Code = list[1];
                    VitalSign.TypeName = list[2];
                    VitalSign.Name = list[3];
                    VitalSign.InputCode = list[4];
                    VitalSign.SortNo = list[5];
                    VitalSign.Redundance = list[6];
                    VitalSign.InvalidFlag = list[7];
                }
                return VitalSign;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDataByTC", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取目前最新Num条临床数据  Table:Ps...  Author:LS 2015-03-11")]
        // GetClinicalNum 获取目前最新Num条临床数据 LS 2014-1-18   //不然全部拿出 按时间  类型 VID 排序后 合并
        public Clinic GetClinicalNew(string UserId, DateTime AdmissionDate, DateTime ClinicDate, int Num)
        {
            //最终输出
            Clinic Clinic = new Clinic();
            Clinic.UserId = UserId;
            Clinic.History = new List<ClinicBasicInfoandTime>();
            try
            {

                DataTable DT_Clinical_All = new DataTable();   //住院,门诊、检查化验。。。混合表
                DT_Clinical_All.Columns.Add(new DataColumn("精确时间", typeof(DateTime)));
                DT_Clinical_All.Columns.Add(new DataColumn("时间", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("类型", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("VisitId", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("事件", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("关键属性", typeof(string)));  //检查化验的详细查看  例：examnation|

                //颜色由文字决定


                #region 读取两张就诊表，并通过VisitId取出其他数据（检查、化验等）  全部拿出 按时间  VID 类型  排序后，合并

                DataTable DT_Clinical_ClinicInfo = new DataTable();
                DT_Clinical_ClinicInfo = PsClinicalInfo.GetClinicalInfoNum(_cnCache, UserId, AdmissionDate, ClinicDate, Num);

                if (DT_Clinical_ClinicInfo.Rows.Count > 1)    //肯定大于0，最后一条是标记，必须传回；大于1表明时间轴还可以加载
                {

                    #region  拿出临床信息

                    for (int i = 0; i < DT_Clinical_ClinicInfo.Rows.Count - 1; i++)  //最后一条是标记，需要单独拿出
                    {
                        string DateShow = Convert.ToDateTime(DT_Clinical_ClinicInfo.Rows[i]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                        DT_Clinical_All.Rows.Add(DT_Clinical_ClinicInfo.Rows[i]["精确时间"], DateShow, DT_Clinical_ClinicInfo.Rows[i]["类型"], DT_Clinical_ClinicInfo.Rows[i]["VisitId"], DT_Clinical_ClinicInfo.Rows[i]["事件"], "");

                        if (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "入院")
                        {
                            //转科处理  转科内容：什么时候从哪里转出，什么时候转到哪里
                            DataTable DT_Clinical_Trans = new DataTable();
                            DT_Clinical_Trans = PsClinicalInfo.GetTransClinicalInfo(_cnCache, UserId, DT_Clinical_ClinicInfo.Rows[i]["VisitId"].ToString());
                            if (DT_Clinical_Trans.Rows.Count > 0)  //有转科
                            {
                                for (int n = 0; n < DT_Clinical_Trans.Rows.Count; n++)
                                {
                                    string DateShow1 = Convert.ToDateTime(DT_Clinical_Trans.Rows[n]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                                    DT_Clinical_All.Rows.Add(DT_Clinical_Trans.Rows[n]["精确时间"], DateShow1, "转科", DT_Clinical_Trans.Rows[n]["VisitId"], DT_Clinical_Trans.Rows[n]["事件"], "");
                                }
                            }
                        }


                        if ((DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "入院") || (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "门诊") || (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "急诊"))
                        {
                            //诊断检查等
                            DataTable DT_Clinical_Others = new DataTable();
                            DT_Clinical_Others = PsClinicalInfo.GetOtherTable(_cnCache, UserId, DT_Clinical_ClinicInfo.Rows[i]["VisitId"].ToString());


                            if (DT_Clinical_Others.Rows.Count > 0)
                            {
                                for (int n = 0; n < DT_Clinical_Others.Rows.Count; n++)
                                {
                                    string DateShow2 = Convert.ToDateTime(DT_Clinical_Others.Rows[n]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                                    DT_Clinical_All.Rows.Add(DT_Clinical_Others.Rows[n]["精确时间"], DateShow2, DT_Clinical_Others.Rows[n]["类型"], DT_Clinical_Others.Rows[n]["VisitId"], DT_Clinical_Others.Rows[n]["事件"], DT_Clinical_Others.Rows[n]["关键属性"]);
                                }

                                //DataRow[] rows = DT_Clinical_Others.Select();
                                //foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                                //{
                                //    DT_Clinical_All.Rows.Add(row.ItemArray);
                                //}
                                //for(int j=0; j<DT_Clinical_Others.Rows.Count;j++)
                                //{
                                //     DT_Clinical_All.Rows.Add(DT_Clinical_Others.Rows[j]);
                                //}
                            }
                        }

                    } //for循环的结尾
                    #endregion


                    //排序   按“精准时间”, “VID”    排序后, “时间”、“VID”相同的合并  【精准时间到s,时间到天】
                    DataView dv = DT_Clinical_All.DefaultView;
                    dv.Sort = "时间 desc,  VisitId desc, 精确时间 Asc"; //目前采用方案二，
                    //时间轴需要倒序，升序Asc    时间轴最外层 日期倒序 某一天内按照时分升序  注意：遇到同一天 又住院又门诊的，即不同VID  方案：一、不拆开，按时间排即可，问题是会混乱； 二，拆开，时间、VID、精确时间   这样的话，按照目前是在一个方框里 颜色字体大小区分开
                    DataTable dtNew = dv.ToTable();

                    #region 如果两者“时间”、“VID”相同则合并   时间轴方框标签-完成后遍历每一个方框内的事件确定标签

                    List<ClinicBasicInfoandTime> history = new List<ClinicBasicInfoandTime>();  //总  时间、事件的集合
                    ClinicBasicInfoandTime temphistory = new ClinicBasicInfoandTime();
                    if (dtNew.Rows.Count > 0)
                    {
                        string TimeMark = dtNew.Rows[0]["时间"].ToString();
                        string VisitIdMark = dtNew.Rows[0]["VisitId"].ToString();
                        temphistory.Time = TimeMark;
                        temphistory.VisitId = VisitIdMark;

                        List<SomeDayEvent> ItemGroup = new List<SomeDayEvent>();
                        SomeDayEvent SomeDayEvent = new SomeDayEvent();
                        SomeDayEvent.Type = dtNew.Rows[0]["类型"].ToString();    //已有类型集合：入院、出院、转科、门诊、急诊、当前住院中；诊断、检查、化验、用药   【住院中未写入】
                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[0]["精确时间"]).ToString("HH:mm");  //取时分 HH:mm(24) hh:mm(12)
                        SomeDayEvent.Event = dtNew.Rows[0]["事件"].ToString();
                        SomeDayEvent.KeyCode = dtNew.Rows[0]["关键属性"].ToString();
                        ItemGroup.Add(SomeDayEvent);

                        if (dtNew.Rows.Count > 1)
                        {
                            for (int i = 1; i < dtNew.Rows.Count; i++)
                            {
                                string TimeMark1 = dtNew.Rows[i]["时间"].ToString();
                                string VisitIdMark1 = dtNew.Rows[i]["VisitId"].ToString();

                                if (i == dtNew.Rows.Count - 1)
                                {
                                    if ((TimeMark1 == TimeMark) && (VisitIdMark1 == VisitIdMark))
                                    {

                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);
                                    }
                                    else
                                    {

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);

                                        temphistory = new ClinicBasicInfoandTime();
                                        temphistory.Time = TimeMark1;
                                        temphistory.VisitId = VisitIdMark1;

                                        ItemGroup = new List<SomeDayEvent>();
                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);


                                    }
                                }
                                else
                                {

                                    if ((TimeMark1 == TimeMark) && (VisitIdMark1 == VisitIdMark))
                                    {

                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);
                                    }
                                    else
                                    {

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);

                                        temphistory = new ClinicBasicInfoandTime();
                                        temphistory.Time = TimeMark1;
                                        temphistory.VisitId = VisitIdMark1;

                                        ItemGroup = new List<SomeDayEvent>();
                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        TimeMark = TimeMark1;
                                        VisitIdMark = VisitIdMark1;
                                    }

                                }

                            }
                        }
                        else
                        {
                            temphistory.ItemGroup = ItemGroup;
                            history.Add(temphistory);
                        }
                    }
                    #endregion


                    #region 时间轴块标签、颜色
                    //类型 入院、出院、转科、门诊、急诊、当前住院中；诊断、检查、化验、用药   【住院中未写入】
                    //标签 入院、出院、转科、门诊、住院中、急诊
                    for (int n = 0; n < history.Count; n++)
                    {
                        for (int m = 0; m < history[n].ItemGroup.Count; m++)
                        {
                            if ((history[n].ItemGroup[m].Type == "入院") || (history[n].ItemGroup[m].Type == "出院") || (history[n].ItemGroup[m].Type == "转科") || (history[n].ItemGroup[m].Type == "门诊") || (history[n].ItemGroup[m].Type == "急诊") || (history[n].ItemGroup[m].Type == "当前住院中"))
                            {
                                history[n].Tag += history[n].ItemGroup[m].Type + "、";
                            }

                        }

                        if ((history[n].Tag == "") || (history[n].Tag == null))
                        {
                            //防止门诊、急诊逸出
                            if (history[n].VisitId.Substring(0, 1) == "I")  //住院
                            {
                                history[n].Tag = "住院中";
                                history[n].Color = PsClinicalInfo.GetColor("住院中");
                            }
                            else if (history[n].VisitId.Substring(0, 1) == "O") //门诊
                            {
                                history[n].Tag = "";//门诊
                                history[n].Color = PsClinicalInfo.GetColor("门诊");
                            }
                            else if (history[n].VisitId.Substring(0, 1) == "E") //急诊
                            {
                                history[n].Tag = "";//急诊
                                history[n].Color = PsClinicalInfo.GetColor("急诊");
                            }
                            //history[n].Tag = "住院中";
                            //history[n].Color = PsClinicalInfo.GetColor("住院中");
                        }
                        else
                        {
                            int z = history[n].Tag.IndexOf("、");
                            //history[n].Color = PsClinicalInfo.GetColor(history[n].Tag.Substring(0, z));   //若有多个标签，颜色取第一个
                            history[n].Tag = history[n].Tag.Substring(0, history[n].Tag.Length - 1);  //去掉最后的、
                            //int end= history[n].Tag.LastIndexOf("、")+1;
                            history[n].Color = PsClinicalInfo.GetColor(history[n].Tag.Substring(history[n].Tag.LastIndexOf("、") + 1));  //若有多个标签，颜色取最后一个
                        }
                    }

                    #endregion

                    Clinic.History = history;   //时间轴

                }  //if (DT_Clinical_ClinicInfo.Rows.Count > 1)的结尾


                //取出指针标记
                int mark = DT_Clinical_ClinicInfo.Rows.Count - 1;
                Clinic.AdmissionDateMark = DT_Clinical_ClinicInfo.Rows[mark]["精确时间"].ToString();
                Clinic.ClinicDateMark = DT_Clinical_ClinicInfo.Rows[mark]["类型"].ToString();

                //确定是否能继续加载
                if ((DT_Clinical_ClinicInfo.Rows.Count - 1) < Num)
                {
                    Clinic.mark_contitue = "0";
                }
                else
                {
                    string mark_in = PsClinicalInfo.GetNextInDate(_cnCache, UserId, Clinic.AdmissionDateMark);
                    string mark_out = PsClinicalInfo.GetNextOutDate(_cnCache, UserId, Clinic.ClinicDateMark);
                    if (((mark_in == "") && (mark_out == "")) || ((mark_in == null) && (mark_out == null)))
                    {
                        Clinic.mark_contitue = "0";
                    }
                    else
                    {
                        Clinic.mark_contitue = "1";
                    }
                }
                #endregion

                return Clinic;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取临床大类信息 Table:检查、化验..  Author:LS 2015-01-21")]
        // GetReminder 获取临床大类信息 LS 2015-01-21
        public DataSet GetClinicInfoDetail(string UserId, string Type, string VisitId, string Date)
        {
            try
            {
                DataTable DT_ClinicInfoDetail = new DataTable();
                DataSet DS_ClinicInfoDetail = new DataSet();
                //string date = Date.Substring(0, 10) + " " + Date.Substring(10, 8);
                //string date_final = Convert.ToDateTime(date).ToString("yyyy/M/d H:mm:ss");
                string date_final = Date.Substring(0, 10) + " " + Date.Substring(10, 8);
                DT_ClinicInfoDetail = PsClinicalInfo.GetClinicInfoDetail(_cnCache, UserId, Type, VisitId, date_final);
                DS_ClinicInfoDetail.Tables.Add(DT_ClinicInfoDetail);
                return DS_ClinicInfoDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取目前最新Num条临床数据  Table:Ps...  Author:WF 2015-04-24")]
        // GetClinicalNum 获取目前最新Num条临床数据 WF 2015-04-24   //不然全部拿出 按时间  类型 VID 排序后 合并
        public void GetClinicalNewMobile(string UserId, DateTime AdmissionDate, DateTime ClinicDate, int Num)
        {
            //最终输出
            Clinic Clinic = new Clinic();
            Clinic.UserId = UserId;
            Clinic.History = new List<ClinicBasicInfoandTime>();
            try
            {

                DataTable DT_Clinical_All = new DataTable();   //住院,门诊、检查化验。。。混合表
                DT_Clinical_All.Columns.Add(new DataColumn("精确时间", typeof(DateTime)));
                DT_Clinical_All.Columns.Add(new DataColumn("时间", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("类型", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("VisitId", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("事件", typeof(string)));
                DT_Clinical_All.Columns.Add(new DataColumn("关键属性", typeof(string)));  //检查化验的详细查看  例：examnation|

                //颜色由文字决定


                #region 读取两张就诊表，并通过VisitId取出其他数据（检查、化验等）  全部拿出 按时间  VID 类型  排序后，合并

                DataTable DT_Clinical_ClinicInfo = new DataTable();
                DT_Clinical_ClinicInfo = PsClinicalInfo.GetClinicalInfoNum(_cnCache, UserId, AdmissionDate, ClinicDate, Num);

                if (DT_Clinical_ClinicInfo.Rows.Count > 1)    //肯定大于0，最后一条是标记，必须传回；大于1表明时间轴还可以加载
                {

                    #region  拿出临床信息

                    for (int i = 0; i < DT_Clinical_ClinicInfo.Rows.Count - 1; i++)  //最后一条是标记，需要单独拿出
                    {
                        string DateShow = Convert.ToDateTime(DT_Clinical_ClinicInfo.Rows[i]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                        DT_Clinical_All.Rows.Add(DT_Clinical_ClinicInfo.Rows[i]["精确时间"], DateShow, DT_Clinical_ClinicInfo.Rows[i]["类型"], DT_Clinical_ClinicInfo.Rows[i]["VisitId"], DT_Clinical_ClinicInfo.Rows[i]["事件"], "");

                        if (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "入院")
                        {
                            //转科处理  转科内容：什么时候从哪里转出，什么时候转到哪里
                            DataTable DT_Clinical_Trans = new DataTable();
                            DT_Clinical_Trans = PsClinicalInfo.GetTransClinicalInfo(_cnCache, UserId, DT_Clinical_ClinicInfo.Rows[i]["VisitId"].ToString());
                            if (DT_Clinical_Trans.Rows.Count > 0)  //有转科
                            {
                                for (int n = 0; n < DT_Clinical_Trans.Rows.Count; n++)
                                {
                                    string DateShow1 = Convert.ToDateTime(DT_Clinical_Trans.Rows[n]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                                    DT_Clinical_All.Rows.Add(DT_Clinical_Trans.Rows[n]["精确时间"], DateShow1, "转科", DT_Clinical_Trans.Rows[n]["VisitId"], DT_Clinical_Trans.Rows[n]["事件"], "");
                                }
                            }
                        }


                        if ((DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "入院") || (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "门诊") || (DT_Clinical_ClinicInfo.Rows[i]["类型"].ToString() == "急诊"))
                        {
                            //诊断检查等
                            DataTable DT_Clinical_Others = new DataTable();
                            DT_Clinical_Others = PsClinicalInfo.GetOtherTable(_cnCache, UserId, DT_Clinical_ClinicInfo.Rows[i]["VisitId"].ToString());


                            if (DT_Clinical_Others.Rows.Count > 0)
                            {
                                for (int n = 0; n < DT_Clinical_Others.Rows.Count; n++)
                                {
                                    string DateShow2 = Convert.ToDateTime(DT_Clinical_Others.Rows[n]["精确时间"]).ToString("yyyy年MM月dd日");  //取日期
                                    DT_Clinical_All.Rows.Add(DT_Clinical_Others.Rows[n]["精确时间"], DateShow2, DT_Clinical_Others.Rows[n]["类型"], DT_Clinical_Others.Rows[n]["VisitId"], DT_Clinical_Others.Rows[n]["事件"], DT_Clinical_Others.Rows[n]["关键属性"]);
                                }

                                //DataRow[] rows = DT_Clinical_Others.Select();
                                //foreach (DataRow row in rows)  // 将查询的结果添加到dt中； 
                                //{
                                //    DT_Clinical_All.Rows.Add(row.ItemArray);
                                //}
                                //for(int j=0; j<DT_Clinical_Others.Rows.Count;j++)
                                //{
                                //     DT_Clinical_All.Rows.Add(DT_Clinical_Others.Rows[j]);
                                //}
                            }
                        }

                    } //for循环的结尾
                    #endregion


                    //排序   按“精准时间”, “VID”    排序后, “时间”、“VID”相同的合并  【精准时间到s,时间到天】
                    DataView dv = DT_Clinical_All.DefaultView;
                    dv.Sort = "时间 desc,  VisitId desc, 精确时间 Asc"; //目前采用方案二，
                    //时间轴需要倒序，升序Asc    时间轴最外层 日期倒序 某一天内按照时分升序  注意：遇到同一天 又住院又门诊的，即不同VID  方案：一、不拆开，按时间排即可，问题是会混乱； 二，拆开，时间、VID、精确时间   这样的话，按照目前是在一个方框里 颜色字体大小区分开
                    DataTable dtNew = dv.ToTable();

                    #region 如果两者“时间”、“VID”相同则合并   时间轴方框标签-完成后遍历每一个方框内的事件确定标签

                    List<ClinicBasicInfoandTime> history = new List<ClinicBasicInfoandTime>();  //总  时间、事件的集合
                    ClinicBasicInfoandTime temphistory = new ClinicBasicInfoandTime();
                    if (dtNew.Rows.Count > 0)
                    {
                        string TimeMark = dtNew.Rows[0]["时间"].ToString();
                        string VisitIdMark = dtNew.Rows[0]["VisitId"].ToString();
                        temphistory.Time = TimeMark;
                        temphistory.VisitId = VisitIdMark;

                        List<SomeDayEvent> ItemGroup = new List<SomeDayEvent>();
                        SomeDayEvent SomeDayEvent = new SomeDayEvent();
                        SomeDayEvent.Type = dtNew.Rows[0]["类型"].ToString();    //已有类型集合：入院、出院、转科、门诊、急诊、当前住院中；诊断、检查、化验、用药   【住院中未写入】
                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[0]["精确时间"]).ToString("HH:mm");  //取时分 HH:mm(24) hh:mm(12)
                        SomeDayEvent.Event = dtNew.Rows[0]["事件"].ToString();
                        SomeDayEvent.KeyCode = dtNew.Rows[0]["关键属性"].ToString();
                        ItemGroup.Add(SomeDayEvent);

                        if (dtNew.Rows.Count > 1)
                        {
                            for (int i = 1; i < dtNew.Rows.Count; i++)
                            {
                                string TimeMark1 = dtNew.Rows[i]["时间"].ToString();
                                string VisitIdMark1 = dtNew.Rows[i]["VisitId"].ToString();

                                if (i == dtNew.Rows.Count - 1)
                                {
                                    if ((TimeMark1 == TimeMark) && (VisitIdMark1 == VisitIdMark))
                                    {

                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);
                                    }
                                    else
                                    {

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);

                                        temphistory = new ClinicBasicInfoandTime();
                                        temphistory.Time = TimeMark1;
                                        temphistory.VisitId = VisitIdMark1;

                                        ItemGroup = new List<SomeDayEvent>();
                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);


                                    }
                                }
                                else
                                {

                                    if ((TimeMark1 == TimeMark) && (VisitIdMark1 == VisitIdMark))
                                    {

                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);
                                    }
                                    else
                                    {

                                        temphistory.ItemGroup = ItemGroup;
                                        history.Add(temphistory);

                                        temphistory = new ClinicBasicInfoandTime();
                                        temphistory.Time = TimeMark1;
                                        temphistory.VisitId = VisitIdMark1;

                                        ItemGroup = new List<SomeDayEvent>();
                                        SomeDayEvent = new SomeDayEvent();
                                        SomeDayEvent.Type = dtNew.Rows[i]["类型"].ToString();
                                        SomeDayEvent.Time = Convert.ToDateTime(dtNew.Rows[i]["精确时间"]).ToString("HH:mm");
                                        SomeDayEvent.Event = dtNew.Rows[i]["事件"].ToString();
                                        SomeDayEvent.KeyCode = dtNew.Rows[i]["关键属性"].ToString();
                                        ItemGroup.Add(SomeDayEvent);

                                        TimeMark = TimeMark1;
                                        VisitIdMark = VisitIdMark1;
                                    }

                                }

                            }
                        }
                        else
                        {
                            temphistory.ItemGroup = ItemGroup;
                            history.Add(temphistory);
                        }
                    }
                    #endregion


                    #region 时间轴块标签、颜色
                    //类型 入院、出院、转科、门诊、急诊、当前住院中；诊断、检查、化验、用药   【住院中未写入】
                    //标签 入院、出院、转科、门诊、住院中、急诊
                    for (int n = 0; n < history.Count; n++)
                    {
                        for (int m = 0; m < history[n].ItemGroup.Count; m++)
                        {
                            if ((history[n].ItemGroup[m].Type == "入院") || (history[n].ItemGroup[m].Type == "出院") || (history[n].ItemGroup[m].Type == "转科") || (history[n].ItemGroup[m].Type == "门诊") || (history[n].ItemGroup[m].Type == "急诊") || (history[n].ItemGroup[m].Type == "当前住院中"))
                            {
                                history[n].Tag += history[n].ItemGroup[m].Type + "、";
                            }

                        }

                        if ((history[n].Tag == "") || (history[n].Tag == null))
                        {
                            //防止门诊、急诊逸出
                            if (history[n].VisitId.Substring(0, 1) == "I")  //住院
                            {
                                history[n].Tag = "住院中";
                                history[n].Color = PsClinicalInfo.GetColor("住院中");
                            }
                            else if (history[n].VisitId.Substring(0, 1) == "O") //门诊
                            {
                                history[n].Tag = "";//门诊
                                history[n].Color = PsClinicalInfo.GetColor("门诊");
                            }
                            else if (history[n].VisitId.Substring(0, 1) == "E") //急诊
                            {
                                history[n].Tag = "";//急诊
                                history[n].Color = PsClinicalInfo.GetColor("急诊");
                            }
                            //history[n].Tag = "住院中";
                            //history[n].Color = PsClinicalInfo.GetColor("住院中");
                        }
                        else
                        {
                            int z = history[n].Tag.IndexOf("、");
                            //history[n].Color = PsClinicalInfo.GetColor(history[n].Tag.Substring(0, z));   //若有多个标签，颜色取第一个
                            history[n].Tag = history[n].Tag.Substring(0, history[n].Tag.Length - 1);  //去掉最后的、
                            //int end= history[n].Tag.LastIndexOf("、")+1;
                            history[n].Color = PsClinicalInfo.GetColor(history[n].Tag.Substring(history[n].Tag.LastIndexOf("、") + 1));  //若有多个标签，颜色取最后一个
                        }
                    }

                    #endregion

                    Clinic.History = history;   //时间轴

                }  //if (DT_Clinical_ClinicInfo.Rows.Count > 1)的结尾


                //取出指针标记
                int mark = DT_Clinical_ClinicInfo.Rows.Count - 1;
                Clinic.AdmissionDateMark = DT_Clinical_ClinicInfo.Rows[mark]["精确时间"].ToString();
                Clinic.ClinicDateMark = DT_Clinical_ClinicInfo.Rows[mark]["类型"].ToString();

                //确定是否能继续加载
                if ((DT_Clinical_ClinicInfo.Rows.Count - 1) < Num)
                {
                    Clinic.mark_contitue = "0";
                }
                else
                {
                    string mark_in = PsClinicalInfo.GetNextInDate(_cnCache, UserId, Clinic.AdmissionDateMark);
                    string mark_out = PsClinicalInfo.GetNextOutDate(_cnCache, UserId, Clinic.ClinicDateMark);
                    if (((mark_in == "") && (mark_out == "")) || ((mark_in == null) && (mark_out == null)))
                    {
                        Clinic.mark_contitue = "0";
                    }
                    else
                    {
                        Clinic.mark_contitue = "1";
                    }
                }
                #endregion

                //return Clinic;
                string result_final = JSONHelper.ObjectToJson(Clinic);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(result_final);

                Context.Response.End();

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetReminder", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                // return null;
                throw (ex);
            }
        }


        #endregion

        #region <" 任务列表（TaskList）ZC ">
        [WebMethod(Description = "UpdateIsDone Table：Ps.TaskList Author:ZC  2015-01-22")]
        //TaskList中IsDone置位
        public int UpdateIsDone(string PatientId, string ReminderNo, int TaskDate, int TaskTime, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ret = 0;
                ret = PsTaskList.UpdateIsDone(_cnCache, PatientId, ReminderNo, TaskDate, TaskTime, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdateIsDone", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetUndoneNum Table：Ps.TaskList Author:ZC  2015-01-22")]
        //TaskList，获取患者未完成任务数
        public int GetUndoneNum(string PatientId)
        {
            try
            {
                int ret = 0;
                ret = PsTaskList.GetUndoneNum(_cnCache, PatientId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUndoneNum", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetUndoneList Table：Ps.TaskList Author:ZC  2015-01-22")]
        // TaskList.GetUndoneList 获取患者未完成任务列表 Author:ZC  2015-01-22
        public DataSet GetUndoneList(string PatientId)
        {
            try
            {
                DataTable DT_Reminder = new DataTable();
                DataSet DS_Reminder = new DataSet();
                DT_Reminder = PsTaskList.GetUndoneList(_cnCache, PatientId);
                DS_Reminder.Tables.Add(DT_Reminder);
                return DS_Reminder;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetUndoneList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetToDoList Table：Ps.TaskList Author:ZC  2015-01-22")]
        // TaskList.GetToDoList 获取患者待完成任务列表 Author:ZC  2015-01-22
        public DataSet GetToDoList(string PatientId)
        {
            try
            {
                DataTable DT_Reminder = new DataTable();
                DataSet DS_Reminder = new DataSet();
                DT_Reminder = PsTaskList.GetToDoList(_cnCache, PatientId);
                DS_Reminder.Tables.Add(DT_Reminder);
                return DS_Reminder;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetToDoList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        #region <" 创建计划页面 ZC ">
        [WebMethod(Description = "GetPsTarget Table：Ps.Target Author:ZC  2015-04-09")]
        // Ps.Target.GetPsTarget 获取某PlanNo的所有目标 Author:ZC  2015-04-09
        public DataSet GetPsTarget(string PlanNo)
        {
            try
            {
                DataTable DT_PsTarget = new DataTable();
                DataSet DS_PsTarget = new DataSet();
                DT_PsTarget = PsTarget.GetPsTarget(_cnCache, PlanNo);
                DS_PsTarget.Tables.Add(DT_PsTarget);
                return DS_PsTarget;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTarget", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        //[WebMethod(Description = "GetPsTask Table：Ps.Task Author:ZC  2015-04-09")]
        //// Ps.Task.GetPsTask 获取某PlanNo的所有任务 Author:ZC  2015-04-09
        //public DataSet GetPsTask(string PlanNo)
        //{
        //    try
        //    {
        //        DataTable DT_PsTask = new DataTable();
        //        DataSet DS_PsTask = new DataSet();
        //        DT_PsTask = PsTask.GetPsTask(_cnCache, PlanNo);
        //        DS_PsTask.Tables.Add(DT_PsTask);
        //        return DS_PsTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTask", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        [WebMethod(Description = "GetPsTaskByType Table：Ps.Task Author:ZC  2015-04-09")]
        // Ps.Task.GetPsTaskByType 根据Type获取某PlanNo的所有任务 Author:ZC  2015-04-09
        public DataSet GetPsTaskByType(string PlanNo, string Type)
        {
            try
            {
                DataTable DT_PsTask = new DataTable();
                DataSet DS_PsTask = new DataSet();
                DT_PsTask = PsTask.GetPsTaskByType(_cnCache, PlanNo, Type);
                DS_PsTask.Tables.Add(DT_PsTask);
                return DS_PsTask;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTaskByType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetDrugDetail Table：Cm.MstDrugDetail Author:ZC  2015-04-09")]
        // Cm.MstDrugDetail.GetDrugDetail 根据模块Module获取药品列表 Author:ZC  2015-04-09
        public DataSet GetDrugDetail(string Module)
        {
            try
            {
                DataTable DT_DrugDetail = new DataTable();
                DataSet DS_DrugDetail = new DataSet();
                DT_DrugDetail = CmMstDrugDetail.GetDrugDetail(_cnCache, Module);
                DS_DrugDetail.Tables.Add(DT_DrugDetail);
                return DS_DrugDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDrugDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetLifeStyleDetail Table：Cm.MstLifeStyleDetail Author:ZC  2015-04-09")]
        // Cm.MstLifeStyleDetail.GetLifeStyleDetial 根据模块Module获取生活方式 Author:ZC  2015-04-09
        public DataSet GetLifeStyleDetail(string Module)
        {
            try
            {
                DataTable DT_DrugDetail = new DataTable();
                DataSet DS_DrugDetail = new DataSet();
                DT_DrugDetail = CmMstLifeStyleDetail.GetLifeStyleDetail(_cnCache, Module);
                DS_DrugDetail.Tables.Add(DT_DrugDetail);
                return DS_DrugDetail;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLifeStyleDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Ps.Task Author:ZC  2015-04-15")]
        // Ps.Task.SetData 创建计划 Author:ZC  2015-04-15
        public bool SetPsTask(string PlanNo, string Id, string Type, string Code, string Instruction, string UserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = PsTask.SetData(_cnCache, PlanNo, Id, Type, Code, Instruction, UserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPsTask ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Ps.Task Author:ZC  2015-04-15")]
        // Ps.Task.SetData 创建计划 Author:ZC  2015-04-15
        public bool CreateTask(string PlanNo, string Task, string UserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                DataTable DT_PsTask = new DataTable();
                DT_PsTask = PsTask.GetPsTask(_cnCache, PlanNo);
                if (DT_PsTask.Rows.Count != 0)
                {
                    PsTask.DeleteAllByPlanNo(_cnCache, PlanNo);
                }

                string[] task = Task.Split('@');
                for (int i = 0; i < task.Length; i++)
                {
                    string[] content = task[i].Split('#');
                    string Id = " ";
                    string Type = content[0];
                    string Code = content[1];
                    string Instruction = content[2];
                    ret = PsTask.SetData(_cnCache, PlanNo, Id, Type, Code, Instruction, UserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                }
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPsTask ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据患者Id，获取药物治疗列表  Table:Ps.DrugRecord  Author:ZC 2015-04-17")]
        // GetDrugRecordList 根据患者Id，获取药物治疗列表 CSQ 2014-12-04
        public DataSet GetPatientDrugRecord(string PatientId, string Module)
        {
            try
            {
                DataTable DT_DrugRecord_new = new DataTable();
                DataTable DT_DrugRecord = new DataTable();
                DataSet DS_DrugRecord = new DataSet();
                DT_DrugRecord = PsDrugRecord.GetPsDrugRecord(_cnCache, PatientId, Module);
                if (DT_DrugRecord.Rows.Count > 0)
                {

                    //排序
                    DataView dv = DT_DrugRecord.DefaultView;
                    dv.Sort = "StartDateTime desc";
                    DT_DrugRecord_new = dv.ToTable();
                }
                DS_DrugRecord.Tables.Add(DT_DrugRecord_new);
                return DS_DrugRecord;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientDrugRecord", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetGoalValue Table：Ps.Target Author:ZC  2015-04-24")]
        // Ps.Target.GetGoalValue 获取当前血压跟目标血压之间的差值 Author:ZC  2015-04-24
        public int GetGoalValue(string PlanNo)
        {
            int result = 0;
            try
            {
                CacheSysList targetlist = PsTarget.GetTargetByCode(_cnCache, PlanNo, "Bloodpressure", "Bloodpressure_1");
                if (targetlist != null)
                {
                    result = Convert.ToInt16(targetlist[4]) - Convert.ToInt16(targetlist[3]);
                }

                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetPsTask ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return result;
                throw (ex);
            }
        }
        #endregion

        #region <"评估 SYF ">
        [WebMethod(Description = "根据病人Id获取当前最新脉率 Table:Ps.VitalSigns  Author:TDY 2015-07-14")]
        //GetLatestPulseByPatientId 根据病人Id获取当前收缩压  TDY 2015-07-14
        public string GetLatestPulseByPatientId(string PatientId)
        {
            try
            {

                string ItemCode = "Pulserate_1";
                string ItemType = "Pulserate";
                string Value = PsVitalSigns.GetLatestPatientVitalSigns(_cnCache, PatientId, ItemType, ItemCode);
                if (Value != "")
                {
                    return Value;
                }
                else
                {
                    return "0000000";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLatestPulseByPatientId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        //下面两个是从vitalsigns里面获取最新测量的当前血压值
        [WebMethod(Description = "根据病人Id获取当前最新收缩压血压值 Table:Ps.VitalSigns  Author:SYF 2015-04-16")]
        //GetLatestSbpByPatientId 根据病人Id获取当前收缩压  SYF 2015-04-16
        public string GetLatestSbpByPatientId(string PatientId)
        {
            try
            {

                string ItemCode = "Bloodpressure_1";
                string ItemType = "Bloodpressure";
                string Value = PsVitalSigns.GetLatestPatientVitalSigns(_cnCache, PatientId, ItemType, ItemCode);
                if (Value != "")
                {
                    return Value;
                }
                else
                {
                    return "0000000";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLatestSbpByPatientId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据病人Id获取当前最新收缩压血压值 Table:Ps.VitalSigns  Author:SYF 2015-04-16")]
        //GetLatestSbpByPatientId 根据病人Id获取当前舒张压    SYF 2015-04-16        
        public string GetLatestDbpByPatientId(string PatientId)
        {
            try
            {
                string ItemCode = "Bloodpressure_2";
                string ItemType = "Bloodpressure";
                string Value = PsVitalSigns.GetLatestPatientVitalSigns(_cnCache, PatientId, ItemType, ItemCode);
                if (Value != "")
                {
                    return Value;
                }
                else
                {
                    return "请输入";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLatestDbpByPatientId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "插入一条数据到PsTarget Table：Ps.Target Author:施宇帆  2015-04-16")]
        public int SetTarget(string Plan, string Id, string Type, string Code, string Value, string Origin, string Instruction, string Unit, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int Flag = 2;
                Flag = PsTarget.SetData(_cnCache, Plan, Id, Type, Code, Value, Origin, Instruction, Unit, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 520;
                throw ex;
            }
        }

        [WebMethod(Description = "插入一条数据到PsPlan Table：Ps.PlanNo Author:施宇帆  2015-04-17")]
        public int SetPlan(string PlanNo, string PatientId, int StartDate, int EndDate, string Module, int Status, string DoctorId, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int Flag = 2;
                Flag = PsPlan.SetData(_cnCache, PlanNo, PatientId, StartDate, EndDate, Module, Status, DoctorId, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 520;
                throw ex;
            }
        }

        [WebMethod(Description = "GetBPGrades Table：Cm.MstBloodPressure Author:SYF  2015-04-21")]
        //MstBloodPressure.GetBPGrades 获取血压等级字典表的所有信息 Author:SYF  2015-04-21
        public List<MstBloodPressure> GetBPGrades()
        {
            try
            {
                List<MstBloodPressure> result = new List<MstBloodPressure>();
                result = CmMstBloodPressure.GetBPGrades(_cnCache);
                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetBPGrades", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据收缩压获取血压等级说明 Table:Cm.MstBloodPressure  Author:SYF 2015-04-22")]
        //GetDescription SYF 2015-04-22 根据收缩压获取血压等级说明      
        public string GetDescription(int SBP)
        {
            try
            {
                string Value = CmMstBloodPressure.GetDescription(_cnCache, SBP);
                if (Value != "")
                {
                    return Value;
                }
                else
                {
                    return "No";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDescription", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取某计划下某任务的目标值 Table:Ps.Target  Author:SYF 2015-04-26")]
        //GetValueByPlanNoAndId SYF 2015-04-26 获取某计划下某任务的目标值      
        public string GetValueByPlanNoAndId(string PlanNo, string Id)
        {
            try
            {
                string Value = PsTarget.GetValueByPlanNoAndId(_cnCache, PlanNo, Id);
                if (Value != "")
                {
                    return Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetValueByPlanNoAndId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取风险评估所需输入 Table:Ps.BasicInfo,Ps.BasicInfoDetail,Ps.VitalSigns(心率)  Author:SYF 2015-06-26")]
        //GetRiskInput SYF 2015-06-26 获取风险评估所需输入      
        public DataSet GetRiskInput(string UserId)
        {
            try
            {
                #region
                //当用户缺少某项参数时，设置一个默认值
                int Age = 1;//年龄默认1岁（避免出现0岁）
                int Gender = 0;//性别男

                int Height = 0;//身高176cm
                int Weight = 0;//体重69千克

                int AbdominalGirth = 0; //腹围

                int Heartrate = 0;//心率

                int Parent = 0;//父母中至少有一方有高血压
                int Smoke = 0;//不抽烟
                int Stroke = 0;//没有中风
                int Lvh = 0; ;//有左心室肥大
                int Diabetes = 0;//有伴随糖尿病
                int Treat = 0;//高血压是否在治疗（接受过）没有
                int Heartattack = 0;//有过心脏事件（心血管疾病）
                int Af = 0;//没有过房颤
                int Chd = 0;//有冠心病(心肌梗塞)
                int Valve = 0;//没有心脏瓣膜病

                double Tcho = 0;//总胆固醇浓度5.2mmol/L
                double Creatinine = 0;//肌酐浓度140μmoI/L
                double Hdlc = 0;//高密度脂蛋白胆固醇1.21g/ml

                int SBP = 0;//当前收缩压
                int DBP = 0;//当前舒张压

                //用于取得真实值
                int piParent = 0;//父母中至少有一方有高血压
                int piSmoke = 0;//不抽烟
                int piStroke = 0;//没有中风
                int piLvh = 0; ;//有左心室肥大
                int piDiabetes = 0;//有伴随糖尿病
                int piTreat = 0;//高血压是否在治疗（接受过）没有
                int piHeartattack = 0;//有过心脏事件（心血管疾病）
                int piAf = 0;//没有过房颤
                int piChd = 0;//有冠心病(心肌梗塞)
                int piValve = 0;//没有心脏瓣膜病

                CacheSysList BaseList = PsBasicInfo.GetBasicInfo(_cnCache, UserId);
                if (BaseList != null)
                {
                    if (BaseList[1] != "" && BaseList[1] != "0" && BaseList[1] != null)
                    {
                        Age = PsBasicInfo.GetAgeByBirthDay(_cnCache, Convert.ToInt32(BaseList[1]));//年龄
                    }
                    if (BaseList[3] != "" && BaseList[3] != "0" && BaseList[3] != null)
                    {
                        Gender = Convert.ToInt32(BaseList[3]);//性别
                    }
                }
                if (Gender <= 2)
                {
                    Gender = Gender - 1;
                }
                else
                {
                    Gender = 0;
                }

                if (Gender == 1)//为计算方便，性别值对调
                {
                    Gender = 0;
                }
                else
                {
                    Gender=1;
                }
                //上面获取患者的年龄和性别，并调整好数值

                //CacheSysList DetailList = PsBasicInfoDetail.GetPatientDetailInfo(_cnCache, UserId);
                //if (DetailList != null)
                //{
                //    if (DetailList[9] != "")
                //    {
                //        Weight = Convert.ToInt32(DetailList[9]);
                //    }
                //    if (DetailList[8] != "")
                //    {
                //        Height = Convert.ToInt32(DetailList[8]);
                //    }
                //}
                //double BMI = Weight / ((Height / 100) ^ 2);

                //int Weight1 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1006_02");
                // if (Weight1 >= 1)
                //{

                //获取体重，身高和BMI
                string Weight1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_02", 1);
                if (Weight1 != "" && Weight1 != "0" && Weight1 != null)
                {
                    Weight = Convert.ToInt32(Weight1);
                }
                // }

                //int Height1 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1006_01");
                //if (Height1 >= 1)
                // {
                string Height1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_01", 1);
                if (Height1 != "" && Height1 != "0" && Height1 != null)
                {
                    Height = Convert.ToInt32(Height1);
                    //Height = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_01", 1));
                }
                string BMIStr = ((double)Weight / ((double)Height * (double)Height) * 10000).ToString("f2");
                double BMI = double.Parse(BMIStr);

                //获取腹围
                string AbdominalGirth1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_13", 1);
                if (AbdominalGirth1 != "" && AbdominalGirth1 != "0" && AbdominalGirth1 != null)
                {
                    AbdominalGirth = Convert.ToInt32(AbdominalGirth1);
                }

                string Heart = PsVitalSigns.GetLatestPatientVitalSigns(_cnCache, UserId, "HeartRate", "HeartRate_1");
                if (Heart != "" && Heart != "0" && Heart != null)
                {
                    Heartrate = Convert.ToInt32(Heart);
                }
                //获取心率

                // int ItemSeq1 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1002_01");
                // if (ItemSeq1 >= 1)
                // {
                string Parent1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1002_01", 1);
                if (Parent1 != "" && Parent1 != "0" && Parent1 != null)
                {
                    Parent = Convert.ToInt32(Parent1);
                    piParent = Parent;
                }
                if (Parent > 1)
                {
                    Parent = 0;
                }
                //获取遗传信息，即父母有无高血压，1是2否3未知

                // int ItemSeq2 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1005_04");
                // if (ItemSeq2 >= 1)
                //  {
                string Smoke1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1005_04", 1);
                if (Smoke1 != "" && Smoke1 != "0" && Smoke1 != null)
                {
                    Smoke = Convert.ToInt32(Smoke1);
                    piSmoke = Smoke;
                }

                // Smoke = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1005_04", 1));
                //  }
                if (Smoke > 1)
                {
                    Smoke = 0;
                }
                //获取是否抽烟1是2否3未知

                // int ItemSeq3 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_07");
                // if (ItemSeq3 >= 1)
                // {
                string Stroke1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_07", 1);
                if (Stroke1 != "" && Stroke1 != "0" && Stroke1 != null)
                {
                    Stroke = Convert.ToInt32(Stroke1);
                    piStroke = Stroke;
                }
                //  Stroke = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_07", 1));
                // }

                if (Stroke > 1)
                {
                    Stroke = 0;
                }
                //中风M1002_6

                //int ItemSeq4 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_09");
                // if (ItemSeq4 >= 1)
                // {

                string Lvh1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_09", 1);
                if (Lvh1 != "" && Lvh1 != "0" && Lvh1 != null)
                {
                    Lvh = Convert.ToInt32(Lvh1);
                    piLvh = Lvh;
                }
                //   Lvh = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_09", 1));
                // }
                if (Lvh > 1)
                {
                    Lvh = 0;
                }
                //左心室肥大M1001_2

                // int ItemSeq5 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1002_02");
                //if (ItemSeq5 >= 1)
                // {
                string Diabetes1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1002_02", 1);
                if (Diabetes1 != "" && Diabetes1 != "0" && Diabetes1 != null)
                {
                    Diabetes = Convert.ToInt32(Diabetes1);
                    piDiabetes = Diabetes;
                }
                //  Diabetes = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1002_02", 1));
                // }
                if (Diabetes > 1)
                {
                    Diabetes = 0;
                }
                //糖尿病M1005_1

                //  int ItemSeq6 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1003_02");
                // if (ItemSeq6 >= 1)
                // {

                string Treat1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1003_02", 1);
                if (Treat1 != "" && Treat1 != "0" && Treat1 != null)
                {
                    Treat = Convert.ToInt32(Treat1);
                    piTreat = Treat;
                }
                //  Treat = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1003_02", 1));
                // }
                if (Treat > 1)
                {
                    Treat = 0;
                }
                //高血压是否在治疗（是否接受高血压治疗）

                //int ItemSeq7 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_04");
                // if (ItemSeq7 >= 1)
                //  {
                string Heartattack1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_04", 1);
                if (Heartattack1 != "" && Heartattack1 != "0" && Heartattack1 != null)
                {
                    Heartattack = Convert.ToInt32(Heartattack1);
                    piHeartattack = Heartattack;
                }
                //    Heartattack = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_04", 1));
                //   }
                if (Heartattack > 1)
                {
                    Heartattack = 0;
                }
                //是否有心脏事件（心血管疾病,心脏骤停）

                //int ItemSeq8 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_05");
                //if (ItemSeq8 >= 1)
                //{
                string Af1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_05", 1);
                if (Af1 != "" && Af1 != "0" && Af1 != null)
                {
                    Af = Convert.ToInt32(Af1);
                    piAf = Af;
                }
                //  Af = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_05", 1));
                // }
                if (Af > 1)
                {
                    Af = 0;
                }
                //是否有房颤

                //int ItemSeq9 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_02");
                //if (ItemSeq9 >= 1)
                //{
                string Chd1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_02", 1);
                if (Chd1 != "" && Chd1 != "0" && Chd1 != null)
                {
                    Chd = Convert.ToInt32(Chd1);
                    piChd = Chd;
                }
                //   Chd = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_02", 1));
                // }
                if (Chd > 1)
                {
                    Chd = 0;
                }
                //是否有冠心病（心肌梗塞）

                // int ItemSeq10 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1001_06");
                //if (ItemSeq10 >= 1)
                // {
                string Valve1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_06", 1);
                if (Valve1 != "" && Valve1 != "0" && Valve1 != null)
                {
                    Valve = Convert.ToInt32(Valve1);
                    piValve = Valve;
                }
                //  Valve = Convert.ToInt32(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1001_06", 1));
                // }
                if (Valve > 1)
                {
                    Valve = 0;
                }
                //是否有心脏瓣膜病

                //int ItemSeq11 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1006_09");
                //if (ItemSeq11 > 1)
                //{
                string Tcho1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_09", 1);
                if (Tcho1 != "" && Tcho1 != "0" && Tcho1 != null)
                {
                    Tcho = Convert.ToDouble(Tcho1);
                }
                // }
                //总胆固醇浓度

                //int ItemSeq12 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1006_08");
                // if (ItemSeq12 > 1)
                // {
                string Creatinine1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_08", 1);
                if (Creatinine1 != "" && Creatinine1 != "0" && Creatinine1 != null)
                {
                    Creatinine = Convert.ToDouble(Creatinine1);
                }
                // Creatinine = Convert.ToDouble(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_08", 1));
                // }
                //肌酐浓度

                //int ItemSeq13 = PsBasicInfoDetail.GetMaxItemSeq(_cnCache, UserId, "M1", "M1006_10");
                // if (ItemSeq13 > 1)
                //{
                string Hdlc1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_10", 1);
                if (Hdlc1 != "" && Hdlc1 != "0" && Hdlc1 != null)
                {
                    Hdlc = Convert.ToDouble(Hdlc1);
                }

                //收缩压和舒张压
                string SBP1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_05", 1);
                if (SBP1 != "" && SBP1 != "0" && SBP1 != null)
                {
                    SBP = Convert.ToInt32(SBP1);
                }

                string DBP1 = PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_06", 1);
                if (DBP1 != "" && DBP1 != "0" && DBP1 != null)
                {
                    DBP = Convert.ToInt32(DBP1);
                }



                //  Hdlc = Convert.ToDouble(PsBasicInfoDetail.GetValue(_cnCache, UserId, "M1", "M1006_10", 1));
                // }
                //高密度脂蛋白胆固醇

                //高血压风险，除血压外的风险已经计算好放在Hyperother中，界面上取了血压之后，加上血压的风险即可。
                double Hyperother = -0.15641 * Age - 0.20293 * Gender - 0.19073 * Smoke - 0.16612 * Parent - 0.03388 * BMI;

                //HarvardRiskInfactor这个变量存的是Harvard风险评估计算公式中的风险因数，界面上需要做的是加上收缩压的风险因数，然后代入公式计算。
                #region
                int HarvardRiskInfactor = 0;
                if (Gender == 1)
                {
                    if (Age <= 39)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 19;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 0;
                        }
                    }
                    else if (Age <= 44 && Age >= 40)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 7;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 4;
                        }
                    }
                    else if (Age <= 49 && Age >= 45)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 7;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 7;
                        }
                    }
                    else if (Age <= 54 && Age >= 50)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 11;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 6;
                        }
                    }
                    else if (Age <= 59 && Age >= 55)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 14;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 6;
                        }
                    }
                    else if (Age <= 64 && Age >= 60)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 18;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 5;
                        }
                    }
                    else if (Age <= 69 && Age >= 65)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 22;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 4;
                        }
                    }
                    else
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 25;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 4;
                        }
                    }
                    //年龄和抽烟的风险值加成
                    if (Tcho < 5)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    else if (Tcho >= 5.0 && Tcho <= 5.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else if (Tcho >= 6.0 && Tcho <= 6.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 4;
                    }
                    else if (Tcho >= 7.0 && Tcho <= 7.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 5;
                    }
                    else if (Tcho >= 8.0 && Tcho <= 8.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 7;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 9;
                    }
                    //总胆固醇浓度风险值加成
                    if (Height < 145)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 6;
                    }
                    else if (Height >= 145 && Height <= 154)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 4;
                    }
                    else if (Height >= 155 && Height <= 164)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    else if (Height >= 165 && Height <= 174)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    //身高风险值加成
                    if (Creatinine < 50)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    else if (Creatinine >= 50 && Creatinine <= 69)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 1;
                    }
                    else if (Creatinine >= 70 && Creatinine <= 89)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else if (Creatinine >= 90 && Creatinine <= 109)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 4;
                    }
                    //肌酐浓度风险值加成
                    if (Chd == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 8;
                    }
                    //心肌梗塞（冠心病）风险值加成
                    if (Stroke == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 8;
                    }
                    //中风风险值加成 
                    if (Lvh == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    //左室高血压（左心室肥大）风险值加成
                    if (Diabetes == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    //糖尿病风险值加成
                }
                //以上是男性风险值

                else
                {
                    if (Age <= 39)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 13;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 0;
                        }
                    }
                    else if (Age <= 44 && Age >= 40)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 12;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 5;
                        }
                    }
                    else if (Age <= 49 && Age >= 45)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 11;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 9;
                        }
                    }
                    else if (Age <= 54 && Age >= 50)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 10;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 14;
                        }
                    }
                    else if (Age <= 59 && Age >= 55)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 10;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 18;
                        }
                    }
                    else if (Age <= 64 && Age >= 60)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 9;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 23;
                        }
                    }
                    else if (Age <= 69 && Age >= 65)
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 9;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 27;
                        }
                    }
                    else
                    {
                        if (Smoke == 1)
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 8;
                        }
                        else
                        {
                            HarvardRiskInfactor = HarvardRiskInfactor + 32;
                        }
                    }
                    //年龄和抽烟的风险值加成
                    if (Tcho < 5)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    else if (Tcho >= 5.0 && Tcho <= 5.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    else if (Tcho >= 6.0 && Tcho <= 6.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 1;
                    }
                    else if (Tcho >= 7.0 && Tcho <= 7.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 1;
                    }
                    else if (Tcho >= 8.0 && Tcho <= 8.9)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    //总胆固醇浓度风险值加成
                    if (Height < 145)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 6;
                    }
                    else if (Height >= 145 && Height <= 154)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 4;
                    }
                    else if (Height >= 155 && Height <= 164)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    else if (Height >= 165 && Height <= 174)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    //身高风险值加成
                    if (Creatinine < 50)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 0;
                    }
                    else if (Creatinine >= 50 && Creatinine <= 69)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 1;
                    }
                    else if (Creatinine >= 70 && Creatinine <= 89)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 2;
                    }
                    else if (Creatinine >= 90 && Creatinine <= 109)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    else
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 4;
                    }
                    //肌酐浓度风险值加成
                    if (Chd == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 8;
                    }
                    //心肌梗塞（冠心病）风险值加成
                    if (Stroke == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 8;
                    }
                    //中风风险值加成 
                    if (Lvh == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 3;
                    }
                    //左室高血压（左心室肥大）风险值加成
                    if (Diabetes == 1)
                    {
                        HarvardRiskInfactor = HarvardRiskInfactor + 9;
                    }
                    //糖尿病风险值加成
                }
                //以上是女性风险值
                //HarvardRisk = 6.304 * Math.Pow(10, -8) * Math.Pow(HarvardRiskInfactor, 5) - 5.027 * Math.Pow(10, -6) * Math.Pow(HarvardRiskInfactor, 4) + 0.0001768 * Math.Pow(HarvardRiskInfactor, 3) - 0.001998 * Math.Pow(HarvardRiskInfactor, 2) + 0.01294 * HarvardRiskInfactor + 0.0409;
                // HarvardRisk = HarvardRisk / 100;
                #endregion

                //FraminghamRiskInfactor这个变量存的是Framingham风险评估计算公式中的风险因数，界面上需要做的是加上收缩压的风险因数，然后代入公式计算。
                //这个Framingham模型也是需要收缩压值的，分为接受过治疗的血压和未接受过治疗的血压，模型分为男女进行计算，因为不同性别公式不同
                #region
                double FraminghamRiskInfactor = 0.0;
                if (Gender == 1) //男性
                {
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Age) * 3.06117;//性别
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Tcho) * 1.12370;//总胆固醇
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Hdlc) * (-0.93263);//高密度脂蛋白胆固醇
                    if (Smoke == 1)
                    {
                        FraminghamRiskInfactor = FraminghamRiskInfactor + 0.65451;//抽烟
                    }
                    if (Diabetes == 1)
                    {
                        FraminghamRiskInfactor = FraminghamRiskInfactor + 0.57367;//抽烟
                    }
                    //HarvardRisk = 4323 * Math.Exp(-Math.Pow(((FraminghamRiskInfactor-185.2)/52.81),2));
                    //FraminghamRisk = 1 - Math.Pow(0.95015, (Math.Exp(FraminghamRiskInfactor - 26.1931)));
                }
                else //女性
                {
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Age) * 2.3288;//性别
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Tcho) * 1.20904;//总胆固醇
                    FraminghamRiskInfactor = FraminghamRiskInfactor + Math.Log(Hdlc) * (-0.70833);//高密度脂蛋白胆固醇
                    if (Smoke == 1)
                    {
                        FraminghamRiskInfactor = FraminghamRiskInfactor + 0.52873;//抽烟
                    }
                    if (Diabetes == 1)
                    {
                        FraminghamRiskInfactor = FraminghamRiskInfactor + 0.69154;//抽烟
                    }
                    // FraminghamRisk = 1 - Math.Pow(0.88936, (Math.Exp(FraminghamRiskInfactor - 23.9802)));
                }
                #endregion

                //StrokeRiskInfactor这个变量存的是中风风险评估计算公式中的风险因数，界面上需要做的是加上收缩压的风险因数，然后计算。
                #region
                int StrokeRiskInfactor = 0;
                if (Gender == 1) //男性
                {
                    if (Age <= 56)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 0;
                    }
                    else if (Age >= 57 && Age <= 59)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 1;
                    }
                    else if (Age >= 60 && Age <= 62)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 2;
                    }
                    else if (Age >= 63 && Age <= 65)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 3;
                    }
                    else if (Age >= 66 && Age <= 68)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 4;
                    }
                    else if (Age >= 69 && Age <= 72)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 5;
                    }
                    else if (Age >= 73 && Age <= 75)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 6;
                    }
                    else if (Age >= 76 && Age <= 78)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 7;
                    }
                    else if (Age >= 79 && Age <= 81)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 8;
                    }
                    else if (Age >= 82 && Age <= 84)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 9;
                    }
                    else
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 10;
                    }
                    if (Diabetes == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 2;
                    }
                    //糖尿病风险值加成
                    if (Smoke == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 3;
                    }
                    //吸烟风险值加成
                    if (Heartattack == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 4;
                    }
                    //心血管疾病史（心脏事件）风险值加成
                    if (Af == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 4;
                    }
                    //房颤风险值加成
                    if (Lvh == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 5;
                    }
                    //左室高血压（左心室肥大）风险值加成
                    //double[] Risk = new double[] { 3, 3, 4, 4, 5, 5, 6, 7, 8, 10, 11, 13, 15, 17, 20, 22, 26, 29, 33, 37, 42, 47, 52, 57, 63, 68, 74, 79, 84, 88};
                    //StrokeRisk = Risk[StrokeRiskInfactor-1] / 100;
                }
                else //女性
                {
                    if (Age <= 56)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 0;
                    }
                    else if (Age >= 57 && Age <= 59)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 1;
                    }
                    else if (Age >= 60 && Age <= 62)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 2;
                    }
                    else if (Age >= 63 && Age <= 64)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 3;
                    }
                    else if (Age >= 65 && Age <= 67)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 4;
                    }
                    else if (Age >= 68 && Age <= 70)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 5;
                    }
                    else if (Age >= 71 && Age <= 73)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 6;
                    }
                    else if (Age >= 74 && Age <= 76)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 7;
                    }
                    else if (Age >= 77 && Age <= 78)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 8;
                    }
                    else if (Age >= 79 && Age <= 81)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 9;
                    }
                    else
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 10;
                    }
                    if (Diabetes == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 3;
                    }
                    //糖尿病风险值加成
                    if (Smoke == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 3;
                    }
                    //吸烟风险值加成
                    if (Heartattack == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 2;
                    }
                    //心血管疾病史（心脏事件）风险值加成
                    if (Af == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 6;
                    }
                    //房颤风险值加成
                    if (Lvh == 1)
                    {
                        StrokeRiskInfactor = StrokeRiskInfactor + 4;
                    }
                    //左室高血压（左心室肥大）风险值加成
                    // double[] Risk = new double[] { 1, 1, 2, 2, 2, 3, 4, 4, 5, 6, 8, 9, 11, 13, 16, 19, 23, 27, 32, 37, 43, 50, 57, 64, 71, 78, 84};
                    //StrokeRisk = Risk[StrokeRiskInfactor-1] / 100;
                }
                #endregion

                //HeartFailureRiskInfactor这个变量存的是心衰风险评估计算公式中的风险因数，界面上需要做的是加上收缩压的风险因数，然后计算。
                #region
                int HeartFailureRiskInfactor = 0;
                if (Gender == 1) //男性
                {
                    #region
                    if (Age <= 49)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 0;
                    }
                    else if (Age >= 50 && Age <= 54)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    else if (Age >= 55 && Age <= 59)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                    }
                    else if (Age >= 60 && Age <= 64)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 3;
                    }
                    else if (Age >= 65 && Age <= 69)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 4;
                    }
                    else if (Age >= 70 && Age <= 74)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 5;
                    }
                    else if (Age >= 75 && Age <= 79)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 6;
                    }
                    else if (Age >= 80 && Age <= 84)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 7;
                    }
                    else if (Age >= 85 && Age <= 89)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 8;
                    }
                    else
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 9;
                    }
                    if (Heartrate <= 54)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 0;
                    }
                    else if (Heartrate >= 55 && Heartrate <= 64)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    else if (Heartrate >= 65 && Heartrate <= 79)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                    }
                    else if (Heartrate >= 80 && Heartrate <= 89)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 3;
                    }
                    else if (Heartrate >= 90 && Heartrate <= 104)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 4;
                    }
                    else
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 5;
                    }
                    //心率风险值加成
                    if (Lvh == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 4;
                    }
                    //左心室肥大（左室高血压）风险值加成
                    if (Chd == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 8;
                    }
                    //冠心病风险值加成
                    if (Valve == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 5;
                    }
                    //瓣膜疾病风险值加成
                    if (Smoke == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    //糖尿病风险值加成
                    #endregion
                }
                else //女性
                {
                    #region
                    if (Age <= 49)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 0;
                    }
                    else if (Age >= 50 && Age <= 54)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    else if (Age >= 55 && Age <= 59)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                    }
                    else if (Age >= 60 && Age <= 64)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 3;
                    }
                    else if (Age >= 65 && Age <= 69)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 4;
                    }
                    else if (Age >= 70 && Age <= 74)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 5;
                    }
                    else if (Age >= 75 && Age <= 79)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 6;
                    }
                    else if (Age >= 80 && Age <= 84)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 7;
                    }
                    else if (Age >= 85 && Age <= 89)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 8;
                    }
                    else
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 9;
                    }
                    //年龄的风险加权值
                    if (Heartrate < 60)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 0;
                    }
                    else if (Heartrate >= 60 && Heartrate <= 79)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    else if (Heartrate >= 80 && Heartrate <= 104)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                    }
                    else
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 3;
                    }
                    //心率风险值加成
                    if (Lvh == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 5;
                    }
                    //左心室肥大（左室高血压）风险值加成
                    if (Chd == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 6;
                    }
                    //冠心病风险值加成
                    if (Valve == 1)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 6;
                        if (Smoke == 1)
                        {
                            HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                        }
                    }
                    else
                    {
                        if (Smoke == 1)
                        {
                            HeartFailureRiskInfactor = HeartFailureRiskInfactor + 6;
                        }
                    }
                    //瓣膜疾病和糖尿病风险值加成
                    if (BMI < 21)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 0;
                    }
                    else if (BMI >= 21 && BMI <= 25)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 1;
                    }
                    else if (BMI > 25 && BMI <= 29)
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 2;
                    }
                    else
                    {
                        HeartFailureRiskInfactor = HeartFailureRiskInfactor + 3;
                    }
                    //BMI风险值加成
                    #endregion
                }
                #endregion

                DataTable Input = new DataTable();
                Input.Columns.Add(new DataColumn("Age", typeof(int)));
                Input.Columns.Add(new DataColumn("Gender", typeof(int)));
                Input.Columns.Add(new DataColumn("Height", typeof(int)));
                Input.Columns.Add(new DataColumn("Weight", typeof(int)));
                Input.Columns.Add(new DataColumn("AbdominalGirth", typeof(int)));
                Input.Columns.Add(new DataColumn("BMI", typeof(double)));
                Input.Columns.Add(new DataColumn("Heartrate", typeof(int)));
                Input.Columns.Add(new DataColumn("Parent", typeof(int)));
                Input.Columns.Add(new DataColumn("Smoke", typeof(int)));
                Input.Columns.Add(new DataColumn("Stroke", typeof(int)));
                Input.Columns.Add(new DataColumn("Lvh", typeof(int)));
                Input.Columns.Add(new DataColumn("Diabetes", typeof(int)));
                Input.Columns.Add(new DataColumn("Treat", typeof(int)));
                Input.Columns.Add(new DataColumn("Heartattack", typeof(int)));
                Input.Columns.Add(new DataColumn("Af", typeof(int)));
                Input.Columns.Add(new DataColumn("Chd", typeof(int)));
                Input.Columns.Add(new DataColumn("Valve", typeof(int)));
                Input.Columns.Add(new DataColumn("Tcho", typeof(double)));
                Input.Columns.Add(new DataColumn("Creatinine", typeof(double)));
                Input.Columns.Add(new DataColumn("Hdlc", typeof(double)));
                Input.Columns.Add(new DataColumn("Hyperother", typeof(double)));
                Input.Columns.Add(new DataColumn("HarvardRiskInfactor", typeof(int)));
                Input.Columns.Add(new DataColumn("FraminghamRiskInfactor", typeof(double)));
                Input.Columns.Add(new DataColumn("StrokeRiskInfactor", typeof(int)));
                Input.Columns.Add(new DataColumn("HeartFailureRiskInfactor", typeof(int)));

                Input.Columns.Add(new DataColumn("SBP", typeof(int)));
                Input.Columns.Add(new DataColumn("DBP", typeof(int)));

                Input.Columns.Add(new DataColumn("piParent", typeof(int)));
                Input.Columns.Add(new DataColumn("piSmoke", typeof(int)));
                Input.Columns.Add(new DataColumn("piStroke", typeof(int)));
                Input.Columns.Add(new DataColumn("piLvh", typeof(int)));
                Input.Columns.Add(new DataColumn("piDiabetes", typeof(int)));
                Input.Columns.Add(new DataColumn("piTreat", typeof(int)));
                Input.Columns.Add(new DataColumn("piHeartattack", typeof(int)));
                Input.Columns.Add(new DataColumn("piAf", typeof(int)));
                Input.Columns.Add(new DataColumn("piChd", typeof(int)));
                Input.Columns.Add(new DataColumn("piValve", typeof(int)));

                Input.Rows.Add(Age, Gender, Height, Weight, AbdominalGirth, BMI, Heartrate, Parent, Smoke, Stroke, Lvh, Diabetes, Treat, Heartattack, Af, Chd, Valve, Tcho, Creatinine, Hdlc, Hyperother, HarvardRiskInfactor, FraminghamRiskInfactor, StrokeRiskInfactor, HeartFailureRiskInfactor, SBP, DBP, piParent, piSmoke, piStroke, piLvh, piDiabetes, piTreat, piHeartattack, piAf, piChd, piValve);

                DataSet Inputset = new DataSet();
                Inputset.Tables.Add(Input);
                return Inputset;
                #endregion

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetRiskInput", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "插入风险评估结果 Table：Ps.TreatmentIndicators Author:SYF  2015-07-06")]
        public bool SetRiskResult(string UserId, int SortNo, string AssessmentType, string AssessmentName, DateTime AssessmentTime, string Result, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool Flag = false;
                SortNo = PsTreatmentIndicators.GetMaxSortNo(_cnCache, UserId) + 1;//sortNo自增
                Flag = PsTreatmentIndicators.SetData(_cnCache, UserId, SortNo, AssessmentType, AssessmentName, AssessmentTime, Result, revUserId, TerminalName, TerminalIP, DeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TreatmentIndicators.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "根据UserId和SortNo获取最新风险评估结果 Table:Ps.TreatmentIndicators  Author:SYF 2015-07-07")]
        //GetRiskResult SYF 2015-07-07 根据UserId和SortNo获取最新风险评估结果      
        public string GetRiskResult(string UserId)
        {
            try
            {
                int SortNo = PsTreatmentIndicators.GetMaxSortNo(_cnCache, UserId);
                string Result = PsTreatmentIndicators.GetResult(_cnCache, UserId, SortNo);
                if (Result != "")
                {
                    return Result;
                }
                else
                {
                    return "No";
                }
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetRiskResult", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region 任务完成情况 LS
        //[WebMethod(Description = "获取计划完成情况（Pad)-首次进入页面 PlanNo为空  Table:计划、任务、依从..  Author:LS 2015-06-25")]
        //// GetImplementationForPadFirst 获取计划完成情况（Pad）-首次进入页面  LS 2015-06-25 
        //public void GetImplementationForPadFirst(string PatientId, string Module)
        //{
        //    ImplementationInfo ImplementationInfo = new ImplementationInfo();
        //    string str_result = "";
        //    try
        //    {
        //        string PlanNo = "";

        //        //病人基本信息-姓名、头像..
        //        CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, PatientId);
        //        if (patientList != null)
        //        {
        //            ImplementationInfo.PatientInfo.PatientName = patientList[0];

        //            CacheSysList BasicInfoDetail = PsBasicInfoDetail.GetDetailInfo(_cnCache, PatientId);
        //            if (BasicInfoDetail != null)
        //            {
        //                if (BasicInfoDetail[6] != null)
        //                {
        //                    ImplementationInfo.PatientInfo.ImageUrl = BasicInfoDetail[6].ToString();
        //                }
        //                else
        //                {
        //                    ImplementationInfo.PatientInfo.ImageUrl = "";  //js端意外不能识别null
        //                }

        //            }
        //        }

        //        //刚进入页面加载计划列表 (始终存在第一条-当前计划）
        //        ImplementationInfo.PlanList = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);

        //        PlanNo = ImplementationInfo.PlanList[0].PlanNo; //肯定会存在 

        //        #region  存在正在执行的计划

        //        if ((PlanNo != "") && (PlanNo != null))  //存在正在执行的计划
        //        {
        //            //剩余天数和进度
        //            InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
        //            PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
        //            if (PRlist != null)
        //            {
        //                ImplementationInfo.RemainingDays = PRlist[0].ToString();
        //                ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();
        //            }

        //            //正在执行计划的最近一周的依从率
        //            InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
        //            weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.PlanList[0].StartDate);
        //            if (weekPeriod != null)
        //            {
        //                ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
        //            }

        //            //读取任务列表
        //            DataTable TaskList = new DataTable();
        //            TaskList = PsTask.GetTaskList(_cnCache, PlanNo);

        //            //测量-血压 （默认显示-收缩压）
        //            string condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
        //            DataRow[] BPRows = TaskList.Select(condition);

        //            List<MstBloodPressure> reference = new List<MstBloodPressure>();
        //            chartData chartData = new chartData();
        //            List<Graph> graphList = new List<Graph>();
        //            List<GuideList> BPGuide = new List<GuideList>();

        //            if ((BPRows != null) && (BPRows.Length == 2))  //一定会有血压和脉率测量任务
        //            {
        //                //获取血压的分级原则，脉率的分级原则写死
        //                reference = CmMstBloodPressure.GetBPGrades(_cnCache);

        //                //血压数据
        //                graphList = CmMstBloodPressure.GetBPInfo(_cnCache, PatientId, PlanNo, "Bloodpressure", ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate, reference);

        //                //初始值、目标值、分级范围加工
        //                if (graphList.Count > 0)
        //                {
        //                    BPGuide = CmMstBloodPressure.GetBPGuide(_cnCache, PlanNo, "Bloodpressure", reference);
        //                    chartData.BPGuide = BPGuide;
        //                }
        //            }


        //            //必有测量任务，其他任务（例如吃药）可能没有

        //            //依从情况
        //            List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
        //            TasksComByPeriod = PsCompliance.GetTasksComByPeriod(_cnCache, PatientId, PlanNo, ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate);
        //            if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == graphList.Count))
        //            {
        //                for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
        //                {
        //                    graphList[rowsCount].drugValue = "1";
        //                    graphList[rowsCount].drugBullet = TasksComByPeriod[rowsCount].drugBullet;
        //                    graphList[rowsCount].drugColor = TasksComByPeriod[rowsCount].drugColor;
        //                    graphList[rowsCount].drugDescription = TasksComByPeriod[rowsCount].Events;
        //                }
        //            }

        //            chartData.graphList = graphList;
        //            ImplementationInfo.chartData = chartData;
        //        }

        //        #endregion

        //        str_result = JSONHelper.ObjectToJson(ImplementationInfo);
        //        Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
        //        Context.Response.Write(str_result);
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //        //Context.Response.End();
        //        //return ImplementationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPadFirst", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "获取计划完成情况（Pad)-查看往期计划  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        //// GetImplementationForPadSecond 获取计划完成情况（Pad）-查看往期计划  LS 2015-03-27  
        //public void GetImplementationForPadSecond(string PatientId, string PlanNo)
        //{
        //    ImplementationInfo ImplementationInfo = new ImplementationInfo();
        //    try
        //    {
        //        //Pad保证PlanNo输入不为空  为空的表示无当前计划，则显示无执行即可，无需连接网络服务

        //        if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
        //        {
        //            //获取计划的相关信息
        //            int planStatus = 0;
        //            int planStartDate = 0;
        //            int planEndDate = 0;

        //            InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
        //            planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
        //            if (planInfo != null)
        //            {
        //                planStatus = Convert.ToInt32(planInfo[5]);
        //                planStartDate = Convert.ToInt32(planInfo[2]);
        //                planEndDate = Convert.ToInt32(planInfo[3]);
        //            }

        //            if (planStatus == 3) //是正在执行的当前计划
        //            {
        //                //剩余天数和进度
        //                InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
        //                PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
        //                if (PRlist != null)
        //                {
        //                    ImplementationInfo.RemainingDays = PRlist[0].ToString();
        //                    ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();
        //                }

        //                //最近一周的依从率
        //                InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
        //                weekPeriod = PsPlan.GetWeekPeriod(_cnCache, planStartDate);
        //                if (weekPeriod != null)
        //                {
        //                    ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
        //                }
        //            }
        //            else  //已经结束计划
        //            {
        //                ImplementationInfo.RemainingDays = "0";
        //                ImplementationInfo.ProgressRate = "100";
        //                ImplementationInfo.CompliacneValue = "整个计划依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, planStartDate, planEndDate) + "%";
        //            }

        //            #region  读取任务执行情况，血压、用药

        //            //读取任务
        //            DataTable TaskList = new DataTable();
        //            TaskList = PsTask.GetTaskList(_cnCache, PlanNo);

        //            //测量任务-血压  默认显示-收缩压
        //            string condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'";
        //            DataRow[] BPRows = TaskList.Select(condition);

        //            List<MstBloodPressure> reference = new List<MstBloodPressure>();
        //            chartData chartData = new chartData();
        //            List<Graph> graphList = new List<Graph>();
        //            List<GuideList> BPGuide = new List<GuideList>();

        //            if ((BPRows != null) && (BPRows.Length == 2))
        //            {
        //                //获取分级原则
        //                reference = CmMstBloodPressure.GetBPGrades(_cnCache);

        //                //血压数据
        //                graphList = CmMstBloodPressure.GetBPInfo(_cnCache, PatientId, PlanNo, "Bloodpressure", planStartDate, planEndDate, reference);

        //                //初始值、目标值、分级范围加工
        //                if (graphList.Count > 0)
        //                {
        //                    BPGuide = CmMstBloodPressure.GetBPGuide(_cnCache, PlanNo, "Bloodpressure", reference);
        //                    chartData.BPGuide = BPGuide;
        //                }
        //            }


        //            //依从情况
        //            List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
        //            TasksComByPeriod = PsCompliance.GetTasksComByPeriod(_cnCache, PatientId, PlanNo, planStartDate, planEndDate);
        //            if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == graphList.Count))
        //            {
        //                for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
        //                {
        //                    graphList[rowsCount].drugValue = "1";
        //                    graphList[rowsCount].drugBullet = TasksComByPeriod[rowsCount].drugBullet;
        //                    graphList[rowsCount].drugColor = TasksComByPeriod[rowsCount].drugColor;
        //                    graphList[rowsCount].drugDescription = TasksComByPeriod[rowsCount].Events;
        //                }
        //            }

        //            chartData.graphList = graphList;
        //            ImplementationInfo.chartData = chartData;
        //            #endregion
        //        }


        //        string str_result = JSONHelper.ObjectToJson(ImplementationInfo);
        //        Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
        //        Context.Response.Write(str_result);
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //        //Context.Response.End();
        //        //return ImplementationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPadSecond", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "获取计划完成情况（Phone)-查看当前计划近一周的情况  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        //// GetImplementationForPhone 获取计划完成情况（Pad）  LS 2015-03-27  
        //public void GetImplementationForPhone(string PatientId, string Module)
        //{
        //    ImplementationPhone ImplementationPhone = new ImplementationPhone();
        //    string str_result = "";
        //    try
        //    {
        //        //注释
        //        //注释
        //        //病人基本信息-头像、姓名.. (由于手机版只针对换换咋用户，基本信息可不用获取
        //       // CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, PatientId);
        //        //if (patientList != null)
        //        //{
        //            //ImplementationPhone.PatientInfo.PatientName = patientList[0];
        //        //}

        //        int planStartDate = 0;
        //        int planEndDate = 0;
        //        string PlanNo = "";

        //        InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
        //        planInfo = PsPlan.GetExecutingPlanByM(_cnCache, PatientId, Module);
        //        if (planInfo != null)
        //        {
        //            PlanNo = planInfo[0].ToString();
        //            planStartDate = Convert.ToInt32(planInfo[2]);
        //            planEndDate = Convert.ToInt32(planInfo[3]);  //未用到
        //        }

        //        if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
        //        {
        //            //剩余天数和进度
        //            InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
        //            PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
        //            if (PRlist != null)
        //            {
        //                ImplementationPhone.RemainingDays = PRlist[0].ToString();  //"距离本次计划结束还剩"+PRlist[0]+"天";
        //                ImplementationPhone.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();  //"进度："++"%";
        //            }

        //            //最近一周的依从率
        //            InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
        //            weekPeriod = PsPlan.GetWeekPeriod(_cnCache, planStartDate);
        //            if (weekPeriod != null)
        //            {
        //                ImplementationPhone.CompliacneValue = PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]));
        //                ImplementationPhone.StartDate = Convert.ToInt32(weekPeriod[0]);  //用于获取血压的详细数据
        //                ImplementationPhone.EndDate = Convert.ToInt32(weekPeriod[1]);
        //            }

        //            #region  读取任务执行情况，血压、用药-最近一周的数据

        //            //读取任务  phone版 只显示测量和用药任务
        //            DataTable TaskList = new DataTable();
        //            TaskList = PsTask.GetTaskList(_cnCache, PlanNo);

        //            //测试-血压(因为血压分级表的单独存在，决定了可以直接用收缩压/舒张压作为输入
        //            //默认显示 图-收缩压
        //            string condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'";
        //            DataRow[] BPRows = TaskList.Select(condition);

        //            List<MstBloodPressure> reference = new List<MstBloodPressure>();
        //            chartData chartData = new chartData();
        //            List<Graph> graphList = new List<Graph>();
        //            List<GuideList> BPGuide = new List<GuideList>();
        //            SignDetailByP SignDetailByP = new SignDetailByP();

        //            if ((BPRows != null) && (BPRows.Length == 2))
        //            {
        //                //获取分级原则
        //                reference = CmMstBloodPressure.GetBPGrades(_cnCache);

        //                //血压数据
        //                graphList = CmMstBloodPressure.GetBPInfo(_cnCache, PatientId, PlanNo, "Bloodpressure", Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]), reference);

        //                //初始值、目标值、分级范围加工
        //                if (graphList.Count > 0)
        //                {
        //                    BPGuide = CmMstBloodPressure.GetBPGuide(_cnCache, PlanNo, "Bloodpressure", reference);
        //                    chartData.BPGuide = BPGuide;
        //                }
        //            }


        //            //用药情况
        //            #region 用药情况

        //            condition = " Type = 'Drug' ";
        //            DataRow[] DrugRows = TaskList.Select(condition);
        //            if ((DrugRows != null) && (DrugRows.Length != 0))
        //            {

        //                List<CompliacneDetailByD> DrugComByPeriod = new List<CompliacneDetailByD>();
        //                DrugComByPeriod = PsCompliance.GetDrugComByPeriod(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]));
        //                if ((DrugComByPeriod != null) && (DrugComByPeriod.Count == graphList.Count))
        //                {
        //                    for (int rowsCount = 0; rowsCount < DrugComByPeriod.Count; rowsCount++)
        //                    {
        //                        graphList[rowsCount].drugValue = "1";
        //                        graphList[rowsCount].drugBullet = DrugComByPeriod[rowsCount].drugBullet;
        //                        graphList[rowsCount].drugColor = DrugComByPeriod[rowsCount].drugColor;
        //                        graphList[rowsCount].drugDescription = DrugComByPeriod[rowsCount].Events;
        //                    }
        //                }
        //            }

        //            else  //没有用药任务
        //            {
        //                for (int m = 0; m < graphList.Count; m++)
        //                {
        //                    graphList[m].drugBullet = "";
        //                    graphList[m].drugValue = "1";
        //                    graphList[m].drugColor = "#FFFFFF";
        //                    graphList[m].drugDescription = "无用药任务";
        //                }
        //            }
        //            #region
        //            //    #region
        //            //    //获取本次计划内的用药数据 不同药不同表  一般药不会很多
        //            //    //不同用药情况归化在同一天  表行数是一样多的

        //            //    DataSet ds_DrugCompliacneDetails = new DataSet();
        //            //    for (int n = 0; n < DrugRows.Length; n++)
        //            //    {
        //            //        //放在dataset
        //            //        DataTable dt_DrugCompliacneDetail = new DataTable();
        //            //        dt_DrugCompliacneDetail = PsCompliance.GetDrugCompliacneDetailByPeriod(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]), DrugRows[n]["Id"].ToString(), DrugRows[n]["Code"].ToString());
        //            //        ds_DrugCompliacneDetails.Tables.Add(dt_DrugCompliacneDetail);
        //            //    }

        //            //    //各药的整合 并和血压数据整合成一份
        //            //    string a = "已吃："; //已吃     
        //            //    int aa = 0;
        //            //    string b = "未吃：";  //未吃  
        //            //    int bb = 0;

        //            //    if (ds_DrugCompliacneDetails.Tables[0].Rows.Count == graphList.Count)
        //            //    {
        //            //        for (int rowsCount = 0; rowsCount < ds_DrugCompliacneDetails.Tables[0].Rows.Count; rowsCount++)
        //            //        {
        //            //            string drugResultText = "";
        //            //            //drugResultText = "<b><span style='font-size:14px;'> 用药情况：</span></b><br>";

        //            //            for (int tableCount = 0; tableCount < ds_DrugCompliacneDetails.Tables.Count; tableCount++)
        //            //            {
        //            //                if (ds_DrugCompliacneDetails.Tables[tableCount].Rows[rowsCount]["Status"].ToString() == "1")
        //            //                {
        //            //                    a += ds_DrugCompliacneDetails.Tables[tableCount].TableName + "、";
        //            //                    //drugResultText += ds_DrugCompliacneDetails.Tables[tableCount].TableName + "complete  ";
        //            //                    aa++;
        //            //                }
        //            //                else
        //            //                {
        //            //                    //drugResultText += "<b><span style='font-size:14px;color:red;'>" + ds_DrugCompliacneDetails.Tables[tableCount].TableName + "noncomplete  " + "：</span></b>";
        //            //                    b += ds_DrugCompliacneDetails.Tables[tableCount].TableName + "、";
        //            //                    bb++;
        //            //                }
        //            //            }

        //            //            //去除尾部、
        //            //            if (a.Substring(a.Length - 1, 1) == "、")
        //            //            {
        //            //                a = a.Remove(a.LastIndexOf("、"));
        //            //            }

        //            //            if (b.Substring(b.Length - 1, 1) == "、")
        //            //            {
        //            //                b = b.Remove(b.LastIndexOf("、"));
        //            //            }

        //            //            //输出结果

        //            //            Graph Graph = new Graph();

        //            //            graphList[rowsCount].drugValue = "1";
        //            //            //Graph.drugBullet="";
        //            //            if (aa == 0)  //根本没吃
        //            //            {
        //            //                drugResultText = "完全未吃；";
        //            //                drugResultText += b;
        //            //                graphList[rowsCount].drugColor = "#DADADA";
        //            //            }
        //            //            else if ((aa > 0) && (aa < ds_DrugCompliacneDetails.Tables.Count))  //吃了部分
        //            //            {
        //            //                drugResultText = "部分完成；";
        //            //                drugResultText += b;
        //            //                drugResultText += "；";
        //            //                drugResultText += a;

        //            //                graphList[rowsCount].drugBullet = "amcharts-images/drug.png";
        //            //                graphList[rowsCount].drugColor = "";
        //            //            }
        //            //            else   //全吃了
        //            //            {
        //            //                drugResultText = "完成；";
        //            //                drugResultText += a;

        //            //                graphList[rowsCount].drugColor = "#777777";
        //            //            }

        //            //            graphList[rowsCount].drugDescription = drugResultText;

        //            //            a = "吃了："; aa = 0;
        //            //            b = "没吃："; bb = 0;
        //            //        }

        //            //    }

        //            //}
        //            //    #endregion
        //            #endregion







        //            #endregion

        //            chartData.graphList = graphList;
        //            #endregion

        //            ImplementationPhone.chartData = chartData;



        //        }

        //        str_result = JSONHelper.ObjectToJson(ImplementationPhone);
        //        Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
        //        Context.Response.Write(str_result);
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //        //Context.Response.End();
        //        //return ImplementationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPhone", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}

        //[WebMethod(Description = "获取正在执行的计划中，一周内血压（收缩压、舒张压）的数据详细时刻列表-phone  Table:Ps.VitalSigns  Author:LS 2015-04-19")]
        //// GetBPDetailByPeriod 获取正在执行的计划中，一周内血压（收缩压、舒张压）的数据详细时刻列表  LS 2015-03-27
        //public void GetBPDetailByPeriod(string PatientId, string ItemType, int StartDate, int EndDate)
        //{
        //    SignDetailByP result = new SignDetailByP();

        //    try
        //    {
        //        DataTable sysInfo = new DataTable();
        //        sysInfo = PsVitalSigns.GetSignDetailByPeriod(_cnCache, PatientId, "Bloodpressure", "Bloodpressure_1", StartDate, EndDate);

        //        //舒张压表
        //        DataTable diaInfo = new DataTable();
        //        diaInfo = PsVitalSigns.GetSignDetailByPeriod(_cnCache, PatientId, "Bloodpressure", "Bloodpressure_2", StartDate, EndDate);

        //        if ((sysInfo.Rows.Count == diaInfo.Rows.Count) && (sysInfo.Rows.Count > 0))
        //        {

        //            SignDetail SignDetail = new SignDetail();
        //            SignDetail.DetailTime = sysInfo.Rows[0]["RecordTime"].ToString();
        //            SignDetail.Value = sysInfo.Rows[0]["Value"].ToString() + "/" + diaInfo.Rows[0]["Value"].ToString();

        //            SignDetailByD SignDetailByD = new SignDetailByD();
        //            SignDetailByD.Date = sysInfo.Rows[0]["RecordDate"].ToString();
        //            SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(sysInfo.Rows[0]["RecordDate"].ToString());
        //            SignDetailByD.SignDetailList.Add(SignDetail);
        //            //SignDetailByD.Count++;

        //            if (sysInfo.Rows.Count == 1)
        //            {
        //                result.SignDetailByDs.Add(SignDetailByD);
        //            }
        //            else
        //            {
        //                string temp = sysInfo.Rows[0]["RecordDate"].ToString();
        //                for (int rowsCount = 1; rowsCount < sysInfo.Rows.Count; rowsCount++)
        //                {
        //                    //2011/01/03-2011/01/09 血压详细记录 单位：mmph
        //                    //列表形式  -2011/01/03 星期三 
        //                    //08:00 137/95
        //                    //09:00 134/78
        //                    if (rowsCount != sysInfo.Rows.Count - 1)
        //                    {
        //                        if (temp == sysInfo.Rows[rowsCount]["RecordDate"].ToString())
        //                        {
        //                            SignDetail = new SignDetail();
        //                            SignDetail.DetailTime = sysInfo.Rows[rowsCount]["RecordTime"].ToString();
        //                            SignDetail.Value = sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString();
        //                            SignDetailByD.SignDetailList.Add(SignDetail);
        //                            //SignDetailByD.Count++;
        //                        }
        //                        else
        //                        {
        //                            result.SignDetailByDs.Add(SignDetailByD);

        //                            SignDetailByD = new SignDetailByD();
        //                            SignDetailByD.Date = sysInfo.Rows[rowsCount]["RecordDate"].ToString();
        //                            SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(sysInfo.Rows[rowsCount]["RecordDate"].ToString());
        //                            SignDetail = new SignDetail();
        //                            SignDetail.DetailTime = sysInfo.Rows[rowsCount]["RecordTime"].ToString();
        //                            SignDetail.Value = sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString();
        //                            SignDetailByD.SignDetailList.Add(SignDetail);
        //                            //SignDetailByD.Count++;
        //                            temp = sysInfo.Rows[rowsCount]["RecordDate"].ToString();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (temp == sysInfo.Rows[rowsCount]["RecordDate"].ToString())
        //                        {
        //                            SignDetail = new SignDetail();
        //                            SignDetail.DetailTime = sysInfo.Rows[rowsCount]["RecordTime"].ToString();
        //                            SignDetail.Value = sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString();
        //                            SignDetailByD.SignDetailList.Add(SignDetail);
        //                            //SignDetailByD.Count++;
        //                            result.SignDetailByDs.Add(SignDetailByD);
        //                        }
        //                        else
        //                        {
        //                            result.SignDetailByDs.Add(SignDetailByD);
        //                            SignDetailByD = new SignDetailByD();
        //                            SignDetailByD.Date = sysInfo.Rows[rowsCount]["RecordDate"].ToString();
        //                            SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(sysInfo.Rows[rowsCount]["RecordDate"].ToString());
        //                            SignDetail = new SignDetail();
        //                            SignDetail.DetailTime = sysInfo.Rows[rowsCount]["RecordTime"].ToString();
        //                            SignDetail.Value = sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString();
        //                            SignDetailByD.SignDetailList.Add(SignDetail);
        //                            //SignDetailByD.Count++;
        //                            result.SignDetailByDs.Add(SignDetailByD);
        //                            temp = sysInfo.Rows[rowsCount]["RecordDate"].ToString();
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //        //return result;
        //        string a = JSONHelper.ObjectToJson(result);
        //        Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
        //        Context.Response.Write(a);
        //        Context.Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetBPDetailByPeriod", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}


        //[WebMethod(Description = "获取健康计划完成情况（Web)-任务列表、任务完成情况  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        //// GetImplementationForWeb 获取计划完成情况（Web）  LS 2015-03-27  
        //public ImplementationInfo GetImplementationForWebFirst(string PatientId, string Module)
        //{
        //    ImplementationInfo ImplementationInfo = new ImplementationInfo();
        //    try
        //    {
        //        string PlanNo = "";



        //        //首次登入页面,加载计划列表 (始终存在第一条-当前计划）
        //        ImplementationInfo.PlanList = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);

        //        PlanNo = ImplementationInfo.PlanList[0].PlanNo; //肯定会存在 ImplementationForPad.PlanList[0]

        //        #region  存在正在执行的计划

        //        if ((PlanNo != "") && (PlanNo != null))  //存在正在执行的计划
        //        {
        //            //剩余天数和进度
        //            InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
        //            PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
        //            if (PRlist != null)
        //            {
        //                ImplementationInfo.RemainingDays = PRlist[0].ToString();  //"距离本次计划结束还剩"+PRlist[0]+"天";
        //                ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();  //"进度："++"%";
        //            }

        //            //最近一周的依从率
        //            InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
        //            weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.PlanList[0].StartDate);
        //            if (weekPeriod != null)
        //            {
        //                ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
        //            }

        //            //读取任务列表并输出
        //            DataTable TaskList = new DataTable();
        //            TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
        //            ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);



        //            //测量-血压 （默认显示-收缩压）
        //            string condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'";
        //            DataRow[] BPRows = TaskList.Select(condition);

        //            List<MstBloodPressure> reference = new List<MstBloodPressure>();  //血压风险表-来自数据库
        //            chartData chartData = new chartData();               //数据集
        //            List<Graph> graphList = new List<Graph>();           //图-血压、依从情况
        //            List<GuideList> BPGuide = new List<GuideList>();    //图-血压风险表

        //            if ((BPRows != null) && (BPRows.Length == 2))
        //            {
        //                //获取分级原则
        //                reference = CmMstBloodPressure.GetBPGrades(_cnCache);

        //                //血压数据
        //                graphList = CmMstBloodPressure.GetBPInfo(_cnCache, PatientId, PlanNo, "Bloodpressure", ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate, reference);

        //                //初始值、目标值、分级范围加工
        //                if (graphList.Count > 0)
        //                {
        //                    BPGuide = CmMstBloodPressure.GetBPGuide(_cnCache, PlanNo, "Bloodpressure", reference);
        //                    chartData.BPGuide = BPGuide;
        //                }
        //            }


        //            //必有测量任务，其他任务（例如吃药）可能没有

        //            //依从情况
        //            List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
        //            TasksComByPeriod = PsCompliance.GetTasksComByPeriod(_cnCache, PatientId, PlanNo, ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate);
        //            if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == graphList.Count))
        //            {
        //                for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
        //                {
        //                    graphList[rowsCount].drugValue = "1";
        //                    graphList[rowsCount].drugBullet = TasksComByPeriod[rowsCount].drugBullet;
        //                    graphList[rowsCount].drugColor = TasksComByPeriod[rowsCount].drugColor;
        //                    graphList[rowsCount].drugDescription = TasksComByPeriod[rowsCount].Events;
        //                }
        //            }

        //            chartData.graphList = graphList;
        //            ImplementationInfo.chartData = chartData;
        //        }

        //        #endregion

        //        return ImplementationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForWebFirst", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}


        //[WebMethod(Description = "获取健康计划完成情况（Web)-任务列表、任务完成情况  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        ////GetImplementationForWebSecond 获取计划完成情况（Pad）  LS 2015-03-27  
        //public ImplementationInfo GetImplementationForWebSecond(string PatientId, string PlanNo)
        //{
        //    ImplementationInfo ImplementationInfo = new ImplementationInfo();
        //    try
        //    {
        //        //Pad保证PlanNo输入不为空  为空的表示无当前计划，则显示无执行即可，无需连接网络服务

        //        if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
        //        {
        //            //获取计划的相关信息
        //            int planStatus = 0;
        //            int planStartDate = 0;
        //            int planEndDate = 0;

        //            InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
        //            planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
        //            if (planInfo != null)
        //            {
        //                planStatus = Convert.ToInt32(planInfo[5]);
        //                planStartDate = Convert.ToInt32(planInfo[2]);
        //                planEndDate = Convert.ToInt32(planInfo[3]);
        //            }

        //            if (planStatus == 3) //是正在执行的当前计划
        //            {
        //                //剩余天数和进度
        //                InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
        //                PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
        //                if (PRlist != null)
        //                {
        //                    ImplementationInfo.RemainingDays = PRlist[0].ToString();  //"距离本次计划结束还剩"+PRlist[0]+"天";
        //                    ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();  //"进度："++"%";
        //                }

        //                //最近一周的依从率
        //                InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
        //                weekPeriod = PsPlan.GetWeekPeriod(_cnCache, planStartDate);
        //                if (weekPeriod != null)
        //                {
        //                    ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
        //                }
        //            }
        //            else  //已经结束计划
        //            {
        //                ImplementationInfo.RemainingDays = "0";
        //                ImplementationInfo.ProgressRate = "100";
        //                ImplementationInfo.CompliacneValue = "整个计划依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, planStartDate, planEndDate) + "%";
        //            }

        //            #region  读取任务执行情况，血压、用药

        //            //读取任务列表并输出
        //            DataTable TaskList = new DataTable();
        //            TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
        //            ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);


        //            //测量任务-血压  默认显示-收缩压
        //            string condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'";
        //            DataRow[] BPRows = TaskList.Select(condition);

        //            List<MstBloodPressure> reference = new List<MstBloodPressure>();
        //            chartData chartData = new chartData();
        //            List<Graph> graphList = new List<Graph>();
        //            List<GuideList> BPGuide = new List<GuideList>();

        //            if ((BPRows != null) && (BPRows.Length == 2))
        //            {
        //                //获取分级原则
        //                reference = CmMstBloodPressure.GetBPGrades(_cnCache);

        //                //血压数据
        //                graphList = CmMstBloodPressure.GetBPInfo(_cnCache, PatientId, PlanNo, "Bloodpressure", planStartDate, planEndDate, reference);

        //                //初始值、目标值、分级范围加工
        //                if (graphList.Count > 0)
        //                {
        //                    BPGuide = CmMstBloodPressure.GetBPGuide(_cnCache, PlanNo, "Bloodpressure", reference);
        //                    chartData.BPGuide = BPGuide;
        //                }
        //            }


        //            //依从情况
        //            List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
        //            TasksComByPeriod = PsCompliance.GetTasksComByPeriod(_cnCache, PatientId, PlanNo, planStartDate, planEndDate);
        //            if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == graphList.Count))
        //            {
        //                for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
        //                {
        //                    graphList[rowsCount].drugValue = "1";
        //                    graphList[rowsCount].drugBullet = TasksComByPeriod[rowsCount].drugBullet;
        //                    graphList[rowsCount].drugColor = TasksComByPeriod[rowsCount].drugColor;
        //                    graphList[rowsCount].drugDescription = TasksComByPeriod[rowsCount].Events;
        //                }
        //            }

        //            chartData.graphList = graphList;
        //            ImplementationInfo.chartData = chartData;
        //            #endregion
        //        }

        //        return ImplementationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForWebSecond", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        //return null;
        //        throw (ex);
        //    }
        //}
        #endregion

        #region "任务一览 YDS"
        [WebMethod(Description = "获取当天完成的(或未完成的)任务数 Table：Ps.Compliance Author:施宇帆  2015-04-18")]
        //Ps.Compliance，获取当天完成的(或未完成的)任务数 施宇帆 2015-04-18
        public int GetTaskNumber(string PatientId, string PlanNo, int PiStatus)
        {
            try
            {
                int a = 0;
                a = PsCompliance.GetTaskNumber(_cnCache, PatientId, PlanNo, PiStatus);
                return a;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTaskNumber", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }


        [WebMethod(Description = "GetTaskByStatus Table：Ps.Compliance Author:SYF  2015-04-18")]
        //Compliance.GetTaskByStatus 在当天根据任务状态的完成情况输出相应的任务 Author:SYF  2015-04-18
        public DataSet GetTaskByStatus(string PatientId, string PlanNo, int PiStatus)
        {
            try
            {
                DataTable DT_Reminder = new DataTable();
                DataSet DS_Reminder = new DataSet();
                DT_Reminder = PsCompliance.GetTaskByStatus(_cnCache, PatientId, PlanNo, PiStatus);
                DS_Reminder.Tables.Add(DT_Reminder);
                return DS_Reminder;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTaskByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetDateTimebyNow Author:YDS 2015-04-22")]
        //获取当天日期（yyyy-mm-dd）以及星期几
        public string GetDateTimebyNow()
        {
            string serverTime = string.Empty;
            try
            {
                if (!_cnCache.Connect())
                {
                    return serverTime;
                }
                serverTime = Cm.CommonLibrary.GetServerDateTime(_cnCache.CacheConnectionObject);    //2014/08/22 15:33:35
                string year = serverTime.Substring(0, 4);
                string month = serverTime.Substring(5, 2);
                string day = serverTime.Substring(8, 2);
                int c = int.Parse(year.Substring(0, 2));
                int y = int.Parse(year.Substring(2, 2));
                int m = int.Parse(month);
                if (m <= 2)
                {
                    m += 12;
                    if (y == 0)
                    {
                        y = 99;
                        c--;
                    }
                    else y--;
                }
                int d = int.Parse(day);
                int w = (c / 4) - 2 * c + y + (y / 4) + (26 * (m + 1) / 10) + d - 1;
                w %= 7;
                serverTime = serverTime.Substring(0, 10) + "/" + w.ToString();
                return serverTime;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDateTimebyNow", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return serverTime;
                throw (ex);
            }
            finally
            {
                _cnCache.DisConnect();
            }
        }

        [WebMethod(Description = "SetComlianceDetail Author:YDS 2015-04-22")]
        //依从率子表插入or更新数据
        public int SetComlianceDetail(string PatientId, int DateByNow, string PlanNo, string Id, int Status, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ret = 0;
                string parent = PatientId + "||" + DateByNow.ToString() + "||" + PlanNo;
                ret = PsComplianceDetail.SetData(_cnCache, parent, Id, Status, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetComlianceDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetPlanInfobyPID Author:YDS 2015-05-07")]
        //获取病人当前计划以及健康专员 "PlanNo|DoctorId"
        public string GetPlanInfobyPID(string PatientId)
        {
            string Info = string.Empty;
            try
            {
                //if (!_cnCache.Connect())
                //{
                //    return Info;
                //}
                //string PlanNo = PsPlan.GetExecutingPlan(_cnCache, PatientId)[0].ToString();
                //CacheSysList Doctor = PsBasicInfoDetail.GetSDoctor(_cnCache, PatientId);
                //string DoctorId = Doctor[0].ToString();
                //string DoctorName = Doctor[1].ToString();
                //Info = PlanNo + "|" + DoctorId + "|" + DoctorName;

                //ZAM Bug Fix 2015-6-2
                CacheSysList Plan = PsPlan.GetExecutingPlan(_cnCache, PatientId);
                string PlanNo = string.Empty;
                string DoctorId = string.Empty;
                string DoctorName = string.Empty;

                if (Plan != null && Plan.Count > 0)
                {
                    PlanNo = Plan[0].ToString();
                }
                CacheSysList Doctor = PsBasicInfoDetail.GetSDoctor(_cnCache, PatientId);
                if (Doctor != null && Doctor.Count > 1)
                {
                    DoctorId = Doctor[0].ToString();
                    DoctorName = Doctor[1].ToString();
                }
                Info = PlanNo + "|" + DoctorId + "|" + DoctorName;
                return Info;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPlanInfobyPID", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Info;
                throw (ex);
            }
            finally
            {
                //_cnCache.DisConnect();
            }
        }

        #endregion

        #region <" 健康专员主页 ZAM ">
        [WebMethod(Description = "获取健康专员负责的所有患者（最新结束但未达标的）计划列表  Table:Ps.DoctorInfo Ps.BasicInfo   Author:ZAM 2015-05-18")]
        public DataSet GetOverDuePlanList(string DoctorId, string ModuleType)
        {
            DataTable DT_PatientList = new DataTable();
            DT_PatientList.Columns.Add(new DataColumn("PatientId", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("PatientName", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("photoAddress", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("StartDate", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("Process", typeof(double)));
            DT_PatientList.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("VitalSign", typeof(List<string>)));

            DataTable DT_Patients = new DataTable();
            DT_Patients.Columns.Add(new DataColumn("PatientId", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("StartDate", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("EndDate", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("TotalDays", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Status", typeof(string)));

            DataSet DS_Patients = new DataSet();

            try
            {
                int nowDate = GetServerDate();

                DT_Patients = PsPlan.GetOverDuePlanByDoctorId(_cnCache, DoctorId, ModuleType);
                if (DT_Patients == null)
                {
                    return DS_Patients;
                }
                foreach (DataRow item in DT_Patients.Rows)
                {
                    string patientId = item["PatientId"].ToString();
                    string planNo = item["PlanNo"].ToString();

                    string startDate = item["StartDate"].ToString();
                    string endDate = item["EndDate"].ToString();
                    string totalDays = item["TotalDays"].ToString();
                    string remainingDays = item["RemainingDays"].ToString();

                    double process = 0.0;
                    //VitalSign
                    List<string> vitalsigns = new List<string>();

                    if (planNo != "")
                    {
                        //double complianceRate = PsCompliance.GetComplianceByDay(_cnCache, patientId, nowDate, planNo);

                        string itemType = "Bloodpressure";
                        string itemCode = "Bloodpressure_1";
                        int recordDate = Convert.ToInt32(endDate);
                        CacheSysList list = PsVitalSigns.GetLatestVitalSignsByDate(_cnCache, patientId, itemType, itemCode, recordDate);
                        if (list != null)
                        {
                            vitalsigns.Add(list[2]);
                        }

                        CacheSysList targetlist = PsTarget.GetTargetByCode(_cnCache, planNo, itemType, itemCode);
                        if (targetlist != null)
                        {
                            vitalsigns.Add(targetlist[3]);  //value
                        }
                        //非法数据判断 zam 2015-5-18
                        //OverDue Check
                        if (list != null && targetlist != null)
                        {
                            double m, n;
                            bool misNumeric = double.TryParse(list[2].ToString(), out m);
                            bool nisNumeric = double.TryParse(targetlist[3].ToString(), out n);
                            if (misNumeric && nisNumeric)
                            {
                                //if (Convert.ToInt32(list[2]) <= Convert.ToInt32(targetlist[3])) //已达标
                                if (m <= n)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    //PhotoAddress
                    string photoAddress = "";
                    CacheSysList patientInfolist = PsBasicInfoDetail.GetPatientDetailInfo(_cnCache, patientId);
                    if (patientInfolist != null)
                    {
                        photoAddress = patientInfolist[7];

                    }

                    string patientName = "";
                    patientName = CmMstUser.GetNameByUserId(_cnCache, patientId);

                    DT_PatientList.Rows.Add(patientId, patientName, photoAddress, planNo, startDate, process, remainingDays, vitalsigns);
                }
                DS_Patients.Merge(DT_PatientList);
                return DS_Patients;

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetOverDuePlanList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取健康专员负责的患者列表  Table:Ps.DoctorInfo Ps.BasicInfo   Author:ZAM 2015-05-18")]
        public DataSet GetPatientsList(string DoctorId, string ModuleType, int Plan, int Compliance, int Goal)
        {
            int patientTotalCount = 0;  //某个模块下的患者总数
            int planCount = 0;          //已有计划的患者数
            int complianceCount = 0;    //依从的患者数
            int goalCount = 0;          //达标的患者数
            double planRate = 0;
            double complianceRateTotal = 0;
            double goalRate = 0;

            DataTable DT_Rates = new DataTable();
            DT_Rates.TableName = "RateTable";
            DT_Rates.Columns.Add(new DataColumn("PlanRate", typeof(double)));
            DT_Rates.Columns.Add(new DataColumn("ComplianceRate", typeof(double)));
            DT_Rates.Columns.Add(new DataColumn("GoalRate", typeof(double)));

            DataTable DT_PatientList = new DataTable();
            DT_PatientList.TableName = "PatientListTable";
            DT_PatientList.Columns.Add(new DataColumn("PatientId", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("PatientName", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("photoAddress", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("StartDate", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("Process", typeof(double)));
            DT_PatientList.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("VitalSign", typeof(List<string>)));
            DT_PatientList.Columns.Add(new DataColumn("ComplianceRate", typeof(double)));
            DT_PatientList.Columns.Add(new DataColumn("TotalDays", typeof(string)));
            DT_PatientList.Columns.Add(new DataColumn("Status", typeof(string)));

            DataTable DT_Patients = new DataTable();
            DT_Patients.Columns.Add(new DataColumn("PatientId", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("StartDate", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("EndDate", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("TotalDays", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            DT_Patients.Columns.Add(new DataColumn("Status", typeof(string)));

            DataSet DS_Patients = new DataSet();

            try
            {
                int nowDate = GetServerDate();
                DT_Patients = PsPlan.GetPatientsPlanByDoctorId(_cnCache, DoctorId, ModuleType);
                if (DT_Patients != null)
                    patientTotalCount = DT_Patients.Rows.Count;
                else
                    return DS_Patients;

                foreach (DataRow item in DT_Patients.Rows)
                {
                    string patientId = item["PatientId"].ToString();
                    string planNo = item["PlanNo"].ToString();
                    if (planNo != "")
                    {
                        planCount++;
                    }

                    //HavePlan 0 1 2
                    if ((Plan == 1 && planNo == "") || (Plan == 2 && planNo != ""))
                    {
                        continue;
                    }
                    string startDate = item["StartDate"].ToString();
                    string totalDays = item["TotalDays"].ToString();
                    string remainingDays = item["RemainingDays"].ToString();
                    string status = item["Status"].ToString();

                    double process = 0.0;
                    double complianceRate = 0.0;
                    //VitalSign
                    List<string> vitalsigns = new List<string>();

                    if (planNo != "")
                    {
                        complianceRate = PsCompliance.GetComplianceByDay(_cnCache, patientId, nowDate, planNo);
                        if (complianceRate > 0)
                        {
                            complianceCount++;
                        }
                        if (complianceRate < 0)
                        {
                            complianceRate = 0;
                        }
                        //Compliance
                        if (Compliance == 1 && complianceRate <= 0)
                        {
                            continue;
                        }
                        if (Compliance == 2 && complianceRate > 0)
                        {
                            continue;
                        }

                        //Vitalsign 
                        string itemType = "Bloodpressure";
                        string itemCode = "Bloodpressure_1";
                        int recordDate = nowDate; //nowDate
                        //recordDate = 20150422;
                        bool goalFlag = false;
                        CacheSysList list = PsVitalSigns.GetSignByDay(_cnCache, patientId, itemType, itemCode, recordDate);
                        if (list != null)
                        {
                            vitalsigns.Add(list[2]);
                        }
                        else
                        {
                            //   vitalsigns.Add("115");
                            //ZAM 2015-6-17
                            vitalsigns.Add("");
                        }

                        CacheSysList targetlist = PsTarget.GetTargetByCode(_cnCache, planNo, itemType, itemCode);
                        if (targetlist != null)
                        {
                            vitalsigns.Add(targetlist[4]);  //index 4 for Origin value
                            vitalsigns.Add(targetlist[3]);  //index 3 for target value
                        }
                        else
                        {
                            //vitalsigns.Add("200");
                            //vitalsigns.Add("120");
                            //ZAM 2015-6-17
                            vitalsigns.Add("");
                            vitalsigns.Add("");
                        }
                        //非法数据判断 zam 2015-5-18
                        if (list != null && targetlist != null)
                        {
                            double m, n;
                            bool misNumeric = double.TryParse(list[2].ToString(), out m);
                            bool nisNumeric = double.TryParse(targetlist[3].ToString(), out n);
                            if (misNumeric && nisNumeric)
                            {
                                //if (Convert.ToInt32(list[2]) <= Convert.ToInt32(targetlist[3])) //已达标
                                if (m <= n)
                                {
                                    goalCount++;
                                    goalFlag = true;
                                }
                            }
                        }
                        //Goal 
                        if (Goal == 1 && goalFlag == false)
                        {
                            continue;
                        }
                        if (Goal == 2 && goalFlag == true)
                        {
                            continue;
                        }

                        //非法数据判断 zam 2015-5-18
                        if (startDate != "" && totalDays != "" && remainingDays != "")
                        {
                            double m, n;
                            bool misNumeric = double.TryParse(totalDays, out m);
                            bool nisNumeric = double.TryParse(remainingDays, out n);

                            if (misNumeric && nisNumeric)
                            {
                                //process = (Convert.ToDouble(totalDays) - Convert.ToDouble(remainingDays)) / Convert.ToDouble(totalDays);
                                process = m != 0.0 ? (m - n) / m : 0;
                            }

                        }
                    }

                    //PhotoAddress
                    string photoAddress = "";
                    CacheSysList patientInfolist = PsBasicInfoDetail.GetPatientDetailInfo(_cnCache, patientId);
                    if (patientInfolist != null || patientInfolist.Count != 0)
                    {
                        photoAddress = patientInfolist[6];

                    }

                    string patientName = "";
                    patientName = CmMstUser.GetNameByUserId(_cnCache, patientId);
                    DT_PatientList.Rows.Add(patientId, patientName, photoAddress, planNo, startDate, process, remainingDays, vitalsigns, complianceRate, totalDays, status);
                }
                DS_Patients.Merge(DT_PatientList);
                //The main rates for Plan, Compliance , Goal
                planRate = patientTotalCount != 0 ? (double)planCount / patientTotalCount : 0;
                complianceRateTotal = planCount != 0 ? (double)complianceCount / planCount : 0;
                goalRate = planCount != 0 ? (double)goalCount / planCount : 0;
                DT_Rates.Rows.Add(planRate, complianceRateTotal, goalRate);
                DS_Patients.Merge(DT_Rates);
                return DS_Patients;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientsByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        #region <" 健康教育（手机端） WF ">
        [WebMethod(Description = "添加健康教育资源信息 Table：Cm.MstHealthEducation Author:WF  2015-05-15")]
        public bool SetHealthEducation(string Module, string HealthId, int Type, string FileName, string Path, string Introduction, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {

                bool ret = false;
                ret = CmMstHealthEducation.SetData(_cnCache, Module, HealthId, Type, FileName, Path, Introduction, Redundance, revUserId, TerminalName, TerminalIP, DeviceType) == 1 ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetHealthEducation", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "按模块获得健康教育视频地址列表 Table：Cm.MstHealthEducation Author:WF  2015-04-24")]
        public DataSet GetVedioAddressList(string Module)
        {
            try
            {
                DataTable DT_VedioAddressList = new DataTable();
                DataSet DS_VedioAddressList = new DataSet();
                DT_VedioAddressList = CmMstHealthEducation.GetVedioAddress(_cnCache, Module);
                DS_VedioAddressList.Tables.Add(DT_VedioAddressList);
                return DS_VedioAddressList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetVedioAddressList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "按模块获得健康教育文本地址列表 Table：Cm.MstHealthEducation Author:WF  2015-04-24")]
        public DataSet GetTextAddressList(string Module)
        {
            try
            {
                DataTable DT_TextAddressList = new DataTable();
                DataSet DS_TextAddressListt = new DataSet();
                DT_TextAddressList = CmMstHealthEducation.GetTextAddress(_cnCache, Module);
                DS_TextAddressListt.Tables.Add(DT_TextAddressList);
                return DS_TextAddressListt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTextAddressList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "按模块获得健康教育图片地址列表 Table：Cm.MstHealthEducation Author:WF  2015-04-24")]
        public DataSet GetImageAddressList(string Module)
        {
            try
            {
                DataTable DT_ImageAddressList = new DataTable();
                DataSet DS_ImageAddressListt = new DataSet();
                DT_ImageAddressList = CmMstHealthEducation.GetImageAddress(_cnCache, Module);
                DS_ImageAddressListt.Tables.Add(DT_ImageAddressList);
                return DS_ImageAddressListt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImageAddressList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "按模块获得健康教育pdf地址列表 Table：Cm.MstHealthEducation Author:WF  2015-04-24")]
        public DataSet GetPDFAddressList(string Module)
        {
            try
            {
                DataTable DT_PDFAddressList = new DataTable();
                DataSet DS_PDFAddressListt = new DataSet();
                DT_PDFAddressList = CmMstHealthEducation.GetPDFAddress(_cnCache, Module);
                DS_PDFAddressListt.Tables.Add(DT_PDFAddressList);
                return DS_PDFAddressListt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPDFAddressList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "按模块、类型获得健康教育资源地址列表 Table：Cm.MstHealthEducation Author:WF  2015-04-24")]
        public DataSet GetAddressByTypeList(string Module, int Type)
        {
            try
            {
                DataTable DT_PDFAddressList = new DataTable();
                DataSet DS_PDFAddressListt = new DataSet();
                DT_PDFAddressList = CmMstHealthEducation.GetAddressByType(_cnCache, Module, Type);
                DS_PDFAddressListt.Tables.Add(DT_PDFAddressList);
                return DS_PDFAddressListt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAddressByTypeList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "按模块、HealthId获得健康教育资源详情 Table：Cm.MstHealthEducation Author:WF  2015-05-14")]
        public DataSet GetAll(string Module, string HealthId)
        {
            try
            {
                DataTable DT_AllList = new DataTable();
                DataSet DS_AllListt = new DataSet();
                DT_AllList = CmMstHealthEducation.GetAll(_cnCache, Module, HealthId);
                DS_AllListt.Tables.Add(DT_AllList);
                return DS_AllListt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetAddressByTypeList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "从数据库中删除一条住院信息 Table：Cm.MstHealthEducation Author:WY  20150518")]
        //DeleteHealthEducationInfo 从数据库中删除一条住院信息 WY 20150518
        public int DeleteHealthEducationInfo(string Module, string Health)
        {
            try
            {
                int ret = CmMstHealthEducation.DeleteData(_cnCache, Module, Health);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteHealthEducationInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "添加设备信息 Table：Cm.MstDevice Author:ZC 2015-06-07")]
        public bool SetMstDevice(string Device, string Address, string Port, string VersionNo, string Description, string Redundance)
        {
            try
            {
                bool ret = false;
                ret = CmMstDevice.SetData(_cnCache, Device, Address, Port, VersionNo, Description, Redundance);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMstDevice", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取设备IP和端口号 Table：Cm.MstDevice Author:ZC 2015-06-07")]
        //获取设备IP和端口号     
        public string GetIPAndPort(string Device)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstDevice.GetIP(_cnCache, Device) + ":" + CmMstDevice.GetPort(_cnCache, Device);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetIPAndPort", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        #region <" 日历（手机端） SYF ">
        [WebMethod(Description = "GetComplianceListByPeriod Table：Ps.Compliance Author:SYF  2015-04-28")]
        //Compliance.GetComplianceListByPeriod 获取某计划的某段时间(包括端点)的依从率列表 Author:SYF  2015-04-28
        public DataSet GetComplianceListByPeriod(string PatientId, string PlanNo, int EndDate)
        {
            try
            {
                //PatientId = "PID201501070012";
                //PlanNo = "PLN201504270003";
                //StartDate = 20150310;
                //EndDate = 20150311;
                //EndDate调用的时候取昨天
                int StartDate = 0;
                CacheSysList a = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                if (a != null)
                {
                    StartDate = Convert.ToInt32(a[2]);
                }
                DataTable list = new DataTable();
                DataSet DS_MstType = new DataSet();
                list = PsCompliance.GetComplianceListByPeriod(_cnCache, PatientId, PlanNo, StartDate, EndDate);
                DS_MstType.Tables.Add(list);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetComplianceListByPeriod", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetComplianceListByPeriod Table：Ps.Compliance Author:SYF  2015-04-28")]
        //Compliance.GetComplianceListByPeriod 获取某计划的某段时间(包括端点)的依从率列表 Author:SYF  2015-04-28
        public DataSet GetAllComplianceListByPeriod(string PatientId, string PlanNo, int StartDate, int EndDate)
        {
            try
            {
                //PatientId = "PID201501070012";
                //PlanNo = "PLN201504270003";
                //StartDate = 20150310;
                //EndDate = 20150311;
                //EndDate调用的时候取昨天
                //int StartDate = 0;
                //CacheSysList a = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                //if (a != null)
                //{
                //    StartDate = Convert.ToInt32(a[2]);
                //}
                DataTable list = new DataTable();
                DataSet DS_MstType = new DataSet();
                list = PsCompliance.GetComplianceListByPeriod(_cnCache, PatientId, PlanNo, StartDate, EndDate);
                DS_MstType.Tables.Add(list);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetComplianceListByPeriod", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTasksByIndate Table：Ps.Compliance Author:SYF  2015-04-28")]
        //Compliance.GetTasksByDate 根据计划编码和日期，获取依从率 Author:SYF  2015-04-28
        public DataSet GetTasksByIndate(string PatientId, int Indate, string PlanNo)
        {
            try
            {
                DataTable list = new DataTable();
                DataSet DS_MstType = new DataSet();
                list = PsCompliance.GetTasksByDate(_cnCache, PatientId, Indate, PlanNo);
                DS_MstType.Tables.Add(list);
                return DS_MstType;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTasksByIndate", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetPlanList34ByM Table：Ps.Plan Author:SYF  2015-04-28")]
        //Compliance.GetTasksByDate 根据计划编码和日期，获取依从率 Author:SYF  2015-04-28
        public List<PlanDeatil> GetPlanList34ByM(string PatientId, string Module)
        {
            try
            {
                //DataTable list = new DataTable();
                //DataSet DS_MstType = new DataSet();
                //list = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);
                //DS_MstType.Tables.Add(list);
                //return DS_MstType;
                List<PlanDeatil> result = new List<PlanDeatil>();
                result = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);
                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPlanList34ByM", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <" 开始计划页面 LY ">
        //[WebMethod(Description = "GetBPGrades Table：Cm.MstBloodPressure Author:SYF  2015-04-21")]
        ////MstBloodPressure.GetBPGrades 获取血压等级字典表的所有信息 Author:SYF  2015-04-21
        //public List<MstBloodPressure> GetBPGrades()
        //{
        //    try
        //    {
        //        List<MstBloodPressure> result = new List<MstBloodPressure>();
        //        result = CmMstBloodPressure.GetBPGrades(_cnCache);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetBPGrades", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        [WebMethod(Description = "GetPsTask Table：Ps.Task Author:LY  2015-04-20")]
        //Task.GetPsTask 得到某计划的所有任务信息 Author:LY  2015-04-20
        public DataSet GetPsTask(string PlanNo)
        {
            try
            {
                DataTable DT_Task = new DataTable();
                DataSet DS_Task = new DataSet();
                DT_Task = PsTask.GetPsTask(_cnCache, PlanNo);
                DS_Task.Tables.Add(DT_Task);
                return DS_Task;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTask", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        //[WebMethod(Description = "插入一条数据到PsPlan Table：Ps.PlanNo Author:施宇帆  2015-04-17")]
        //public int SetPlan(string PlanNo, string PatientId, int StartDate, int EndDate, string Module, int Status, string DoctorId, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        //{
        //    try
        //    {
        //        int Flag = 2;
        //        Flag = PsPlan.SetData(_cnCache, PlanNo, PatientId, StartDate, EndDate, Module, Status, DoctorId, piUserId, piTerminalName, piTerminalIP, piDeviceType);
        //        return Flag;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return 520;
        //        throw ex;
        //    }
        //}

        //[WebMethod(Description = "GetPsTarget Table：Ps.Target Author:LY  2015-04-21")]
        ////Target.GetPsTarget 得到某计划的所有目标 Author:LY  2015-04-21
        //public DataSet GetPsTarget(string PlanNo)
        //{
        //    try
        //    {
        //        DataTable DT_Target = new DataTable();
        //        DataSet DS_Target = new DataSet();
        //        DT_Target = PsTarget.GetPsTarget(_cnCache, PlanNo);
        //        DS_Target.Tables.Add(DT_Target);
        //        return DS_Target;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPsTarget", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //        throw (ex);
        //    }
        //}

        [WebMethod(Description = "GetPlanInfo Table：Ps.Plan Author:CSQ  2015-05-06")]
        //Plan.GetPlanInfo 获取某计划的相关信息 Author:CSQ  2015-05-06
        public PlanDeatil GetPlanInfo(string PlanNo)
        {
            try
            {
                PlanDeatil planInfo = new PlanDeatil();
                CacheSysList CacheList = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                if (CacheList != null)
                {
                    planInfo.PlanNo = CacheList[0];
                    planInfo.StartDate = Convert.ToInt32(CacheList[2]);
                    planInfo.EndDate = Convert.ToInt32(CacheList[3]);
                }

                return planInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPlanInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "更新计划状态 Table：Ps.Plan Author:CSQ  2015-05-06")]
        public int PlanStart(string PlanNo, int Status, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int Flag = 2;
                Flag = PsPlan.PlanStart(_cnCache, PlanNo, Status, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.SetData", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }

        [WebMethod(Description = "SetCompliance Table：Ps.Compliance AuthorLY  2015-06-04")]
        //Compliance.SetCompliance 插依从率表 AuthorLY  2015-06-04
        public int SetCompliance(string PatientId, int DDate, string PlanNo, double Compliance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int Flag = 2;
                Flag = PsCompliance.SetData(_cnCache, PatientId, DDate, PlanNo, Compliance, revUserId, TerminalName, TerminalIP, DeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetCompliance", "WebService调用异常！ error information" + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetComplianceDetail Table：Ps.ComplianceDetail AuthorLY  2015-06-04")]
        //Compliance.SetComplianceDetail 插依从率子表 AuthorLY  2015-06-04
        public int SetComplianceDetail(string Parent, string Id, int Status, string CoUserId, string CoTerminalName, string CoTerminalIP, int CoDeviceType)
        {
            try
            {
                int Flag = 2;
                Flag = PsComplianceDetail.SetData(_cnCache, Parent, Id, Status, CoUserId, CoTerminalName, CoTerminalIP, CoDeviceType);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetComplianceDetail", "WebService调用异常！ error information" + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        #endregion

        #region<" 资质审核 GL">

        [WebMethod(Description = "根据角色获取未激活用户列表 表：Ps.RoleMatch，Author:GL 2015-05-26")]
        public DataSet GetInactiveUserByRole(string RoleClass)
        {
            try
            {
                DataTable DT = new DataTable();
                DataSet DS = new DataSet();
                DT = PsRoleMatch.GetInactiveUserByRole(_cnCache, RoleClass);
                DS.Tables.Add(DT);
                return DS;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetInactiveUserByRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "将邀请码写入数据库 表：Ps.RoleMatch，Author:GL 2015-05-28")]
        public int SetActivationCode(string UserId, string RoleClass, string SetActivationCode)
        {
            try
            {
                int ret = 0;
                ret = PsRoleMatch.SetActivationCode(_cnCache, UserId, RoleClass, SetActivationCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetActivationCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }
        #endregion

        #region<" 患者模块管理 ZC">
        [WebMethod(Description = "根据Role获取已经激活的用户列表（医生or健康专员） 表：Ps.RoleMatch，Author:ZC 2015-06-03")]
        public DataSet GetActiveUserByRole(string RoleClass)
        {
            try
            {
                DataTable DT_AUList = new DataTable();
                DataSet DS_AUList = new DataSet();
                DT_AUList = PsRoleMatch.GetActiveUserByRole(_cnCache, RoleClass);

                DataTable list = new DataTable();
                list.Columns.Add(new DataColumn("UserId", typeof(string)));
                list.Columns.Add(new DataColumn("UserName", typeof(string)));
                list.Columns.Add(new DataColumn("Hospital", typeof(string)));
                list.Columns.Add(new DataColumn("Dept", typeof(string)));

                foreach (DataRow item in DT_AUList.Rows)
                {
                    string uid = item["UserId"].ToString();
                    string hospitalCode = PsDoctorInfoDetail.GetValue(_cnCache, uid, "Contact", "Contact001_5", 1);
                    string hospital = "";
                    if (hospitalCode != null)
                    {
                        hospital = CmMstHospital.GetName(_cnCache, hospitalCode);
                    }
                    string deptCode = PsDoctorInfoDetail.GetValue(_cnCache, uid, "Contact", "Contact001_8", 1);
                    string dept = "";
                    if (deptCode != null)
                    {
                        dept = CmMstDivision.GetNamebyCode(_cnCache, deptCode);
                    }
                    list.Rows.Add(uid, item["UserName"].ToString(), hospital, dept);
                }


                DS_AUList.Tables.Add(list);
                return DS_AUList;
            }

            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetActiveUserByRole", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw ex;
            }
        }

        [WebMethod(Description = "根据模块获取正在执行的计划 表：Ps.Plan Author:ZC 2015-07-06")]
        public string GetExecutingPlanByModule(string PatientId, string Module)
        {
            try
            {
                string ret = "";
                InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                planInfo = PsPlan.GetExecutingPlanByM(_cnCache, PatientId, Module);
                if (planInfo != null)
                {
                    ret = planInfo[0].ToString();
                }
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExecutingPlanByModule", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return "";
                throw ex;
            }
        }

        [WebMethod(Description = "更改计划状态 表：Ps.Plan Author:ZC 2015-07-06")]
        public int UpdatePlanStatus(string PlanNo, int Status, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            try
            {
                int ret = 0;
                ret = PsPlan.UpdateStatus(_cnCache, PlanNo, Status, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "UpdatePlanStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw ex;
            }
        }
        #endregion

        #region< "字典维护 GL">
        [WebMethod(Description = "获取血压表全部信息 Table：Cm.MstBloodPressure Author:GL  2015-05-29")]
        public DataSet GetBloodPressureList()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstBloodPressure.GetBloodPressureList(_cnCache);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetBloodPressureList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "血压表插数 Table：Cm.MstBloodPressure Author:GL  2015-05-29")]
        public int SetBloodPressure(string Code, string Name, string Description, int SBP, int DBP, string PatientClass, string Redundance)
        {
            try
            {
                int ret = 0;
                ret = CmMstBloodPressure.SetBloodPressure(_cnCache, Code, Name, Description, SBP, DBP, PatientClass, Redundance);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetBloodPressure", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "血压表删数 Table：Cm.MstBloodPressure Author:GL  2015-05-29")]
        public int DeleteBloodPressure(string Code)
        {
            try
            {
                int ret = 0;
                ret = CmMstBloodPressure.DeleteBloodPressure(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteBloodPressure", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取健康教育表全部信息 Table：Cm.MstHealthEducation Author:GL  2015-05-29")]
        public DataSet GetHealthEducationList()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstHealthEducation.GetHealthEducationList(_cnCache);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetHealthEducationList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "获取生活方式表全部信息 Table：Cm.MstLifeStyle Author:GL  2015-05-29")]
        public DataSet GetLifeStyleList()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstLifeStyle.GetLifeStyleList(_cnCache);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLifeStyleList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "生活方式表删数 Table：Cm.MstLifeStyle Author:GL  2015-05-29")]
        public int DeleteLifeStyle(string StyleId)
        {
            try
            {
                int ret = 0;
                ret = CmMstLifeStyle.DeleteData(_cnCache, StyleId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteLifeStyle", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取生活方式详细表全部信息 Table：Cm.MstLifeStyleDetail Author:GL  2015-05-29")]
        public DataSet GetLifeStyleDetailList()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstLifeStyleDetail.GetLifeStyleDetailList(_cnCache);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLifeStyleDetailList", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "生活方式详细表删数 Table：Cm.MstLifeStyleDetail Author:GL  2015-05-29")]
        public int DeleteLifeStyleDetail(string StyleId, string Module)
        {
            try
            {
                int ret = 0;
                ret = CmMstLifeStyleDetail.DeleteData(_cnCache, StyleId, Module);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteLifeStyleDetail", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.MstTask插数 Table：Cm.MstTask Author:CSQ  20151023")]
        public int SetMstTask(string CategoryCode, string Code, string Name, string ParentCode, string Description, int StartDate, int EndDate, int GroupHeaderFlag, int ControlType, string OptionCategory, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                int ret = 0;
                ret = CmMstTask.SetData(_cnCache,CategoryCode, Code, Name, ParentCode, Description, StartDate, EndDate, GroupHeaderFlag, ControlType, OptionCategory, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMstTask", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "Cm.MstTask删数据 Table: Cm.MstTask Author:LY  20151030")]
        public int DeleteMstTask(string CategoryCode, string Code)
        {
            try
            {
                int ret = 2;
                ret = CmMstTask.DeleteMstTask(_cnCache, CategoryCode, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteMstTask", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
                throw (ex);
            }
        }

        [WebMethod(Description = "根据ParentCode获取任务信息 Table: Cm.MstTask Author:LY  20151029")]
        public DataSet GetMstTaskByParentCode(string ParentCode)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstTask.GetMstTaskByParentCode(_cnCache, ParentCode);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMstTaskByParentCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取任务信息列表（StartDate最新 EndDate不过期） Table: Cm.MstTask Author:WF  20151130")]
        public DataSet GetTasks()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable db = new DataTable();
                db = CmMstTask.GetTasks(_cnCache);
                ds.Tables.Add(db);
                return ds;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTasks", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "得到某个任务的详细信息 Table: Cm.MstTask Author:LY  20151029")]
        public TaskDetailInfo GetCmTaskItemInfo(string CategoryCode, string Code)
        {
            try
            {
                TaskDetailInfo DetailInfo = new TaskDetailInfo();
                CacheSysList GetDetailInfoList = CmMstTask.GetCmTaskItemInfo(_cnCache, CategoryCode, Code);
                if (GetDetailInfoList != null)
                {
                    DetailInfo.Name = GetDetailInfoList[0];
                    if (DetailInfo.Name == null)
                    {
                        DetailInfo.Name = "";
                    }
                    DetailInfo.ParentCode = GetDetailInfoList[1];
                    if (DetailInfo.ParentCode == null)
                    {
                        DetailInfo.ParentCode = "";
                    }
                    DetailInfo.Description = GetDetailInfoList[2];
                    if (DetailInfo.Description == null)
                    {
                        DetailInfo.Description = "";
                    }
                    DetailInfo.GroupHeaderFlag = Convert.ToInt32(GetDetailInfoList[3]);
                    if (DetailInfo.GroupHeaderFlag == null)
                    {
                        DetailInfo.GroupHeaderFlag = 0;
                    }
                    DetailInfo.ControlType = Convert.ToInt32(GetDetailInfoList[4]);
                    if (DetailInfo.ControlType == null)
                    {
                        DetailInfo.ControlType = 0;
                    }
                    DetailInfo.OptionCategory = GetDetailInfoList[5];
                    if (DetailInfo.OptionCategory == null)
                    {
                        DetailInfo.OptionCategory = "";
                    }
                    DetailInfo.CreateDateTime = Convert.ToDateTime(GetDetailInfoList[6]);
                    if (DetailInfo.CreateDateTime == null)
                    {
                        DetailInfo.CreateDateTime = new DateTime();
                    }
                    DetailInfo.Author = GetDetailInfoList[7]; //TDY 20150115 添加
                    if (DetailInfo.Author == null)
                    {
                        DetailInfo.Author = "";
                    }
                    DetailInfo.AuthorName = GetDetailInfoList[8]; //TDY 20150115 添加
                    if (DetailInfo.AuthorName == null)
                    {
                        DetailInfo.AuthorName = "";
                    }
                }
                return DetailInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstTask.GetCmTaskItemInfo", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <文件上传>
        [WebMethod]
        public string UpLoadImage(byte[] fs, string fileName, string requestPath)
        {
            try
            {
                string filePath = Path.Combine(Server.MapPath("~/"), requestPath + fileName);
                //string oldName = System.IO.Path.GetFileName(fileName);
                //string expendName = System.IO.Path.GetExtension(oldName);
                //string newName = DateTime.Now.ToString().Replace(" ", "").Replace(":", "").Replace("-", "").Replace("/", "");
                //定义并实例化一个内存流，以存放提交上来的字节数组
                MemoryStream m = new MemoryStream(fs);
                //定义实际文件对象，保存上载的文件。                
                FileStream f = new FileStream(filePath, FileMode.OpenOrCreate);
                //把内内存里的数据写入物理文件      
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;
                return filePath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 任务完成情况（新） LS

        [WebMethod(Description = "获取计划完成情况（Pad)-首次进入页面 PlanNo为空  Table:计划、任务、依从..  Author:LS 2015-06-25")]
        // GetImplementationForPadFirst 获取计划完成情况（Pad）-首次进入页面  LS 2015-06-25 
        public void GetImplementationForPadFirst(string PatientId, string Module)
        {
            ImplementationInfo ImplementationInfo = new ImplementationInfo();
            string str_result = "";  //最终的输出-ImplementationInfo转化成json格式
            try
            {

                //与模块特性无关的公共项 ——病人基本信息、计划列表、计划进度、体征切换  不同模块可共用

                string PlanNo = "";

                //病人基本信息-姓名、头像..
                CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, PatientId);
                if (patientList != null)
                {
                    ImplementationInfo.PatientInfo.PatientName = patientList[0];

                    CacheSysList BasicInfoDetail = PsBasicInfoDetail.GetDetailInfo(_cnCache, PatientId);
                    if (BasicInfoDetail != null)
                    {
                        if (BasicInfoDetail[6] != null)
                        {
                            ImplementationInfo.PatientInfo.ImageUrl = BasicInfoDetail[6].ToString();
                        }
                        else
                        {
                            ImplementationInfo.PatientInfo.ImageUrl = "";  //js端意外不能识别null
                        }

                    }
                }

                //刚进入页面加载计划列表 (始终存在第一条-当前计划）
                ImplementationInfo.PlanList = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);

                PlanNo = ImplementationInfo.PlanList[0].PlanNo; //肯定会存在 

                #region  存在正在执行的计划

                if ((PlanNo != "") && (PlanNo != null))  //存在正在执行的计划
                {
                    //剩余天数和进度
                    InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
                    PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
                    if (PRlist != null)
                    {
                        ImplementationInfo.RemainingDays = PRlist[0].ToString();
                        ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();

                        ImplementationInfo.StartDate = ImplementationInfo.PlanList[0].StartDate;
                        ImplementationInfo.EndDate = ImplementationInfo.PlanList[0].EndDate;
                    }

                    //正在执行计划的最近一周的依从率
                    InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
                    weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.PlanList[0].StartDate);
                    if (weekPeriod != null)
                    {

                        ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
                    }




                    //读取任务列表
                    DataTable TaskList = new DataTable();
                    TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                    //ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);

                    //测量-体征切换下拉框  
                    string condition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = TaskList.Select(condition);
                    List<SignShow> SignList = new List<SignShow>();
                    for (int j = 0; j < VitalSignRows.Length; j++)
                    {
                        SignShow SignShow = new SignShow();
                        SignShow.SignName = VitalSignRows[j]["CodeName"].ToString();
                        SignShow.SignCode = VitalSignRows[j]["Code"].ToString();
                        SignList.Add(SignShow);
                    }
                    ImplementationInfo.SignList = SignList;


                    List<MstBloodPressure> reference = new List<MstBloodPressure>();
                    ChartData ChartData = new ChartData();
                    List<Graph> GraphList = new List<Graph>();
                    GraphGuide GraphGuide = new GraphGuide();

                    if (Module == "M1")  //后期维护的话，在这里添加不同的模块判断
                    {

                        //高血压模块  体征测量-血压（收缩压、舒张压）、脉率   【默认显示-收缩压，血压必有，脉率可能有】  
                        condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
                        DataRow[] HyperTensionRows = TaskList.Select(condition);

                        //注意：需要兼容之前没有脉率的情况
                        if ((HyperTensionRows != null) && (HyperTensionRows.Length >= 2))  //M1 收缩压（默认显示）、舒张压、脉率  前两者肯定有，脉率不一定有
                        {
                            //从数据库获取血压的分级规则，脉率的分级原则写死在webservice
                            reference = CmMstBloodPressure.GetBPGrades(_cnCache);

                            //首次进入，默认加载收缩压
                            GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, "Bloodpressure|Bloodpressure_1", ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate, reference);

                            //初始值、目标值、分级规则加工
                            if (GraphList.Count > 0)
                            {
                                GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, "Bloodpressure|Bloodpressure_1", reference);
                                ChartData.GraphGuide = GraphGuide;
                            }
                        }

                    }
                    else
                    {

                    }


                    //必有测量任务，其他任务（例如吃药）可能没有

                    //其他任务依从情况  所有模块共有的
                    List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                    //是否有其他任务
                    //string condition1 = " Type not in ('VitalSign,')";
                    if (TaskList.Rows.Count == VitalSignRows.Length)
                    {
                        ChartData.OtherTasks = "0";
                    }
                    else
                    {
                        ChartData.OtherTasks = "1";
                        TasksComByPeriod = PsCompliance.GetTasksComCountByPeriod(_cnCache, PatientId, PlanNo, ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate);
                        if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count)) //体征的数据条数一定等于其他任务的条数（天数） ，都是按照compliance的date统计的
                        {
                            for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                            {
                                GraphList[rowsCount].DrugValue = "1";   //已经初始化过
                                GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                                GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                                GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;//+ "<br><a onclick= shuang shuang zz(); shuang shuang;>详细</a>"
                            }
                        }
                    }

                    ChartData.GraphList = GraphList;
                    ImplementationInfo.ChartData = ChartData;
                }

                #endregion

                str_result = JSONHelper.ObjectToJson(ImplementationInfo);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(str_result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();
                //return ImplementationInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPadFirst", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取计划完成情况（Pad)-查看往期计划  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        // GetImplementationForPadSecond 获取计划完成情况（Pad）-查看往期计划  LS 2015-03-27  
        public void GetImplementationForPadSecond(string PatientId, string PlanNo)
        {
            ImplementationInfo ImplementationInfo = new ImplementationInfo();
            string str_result = "";
            string Module = "";
            try
            {
                //Pad保证PlanNo输入不为空  为空的表示无当前计划，则显示无执行即可，无需连接网络服务

                if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
                {
                    //获取计划的相关信息
                    int planStatus = 0;

                    InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                    planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                    if (planInfo != null)
                    {
                        planStatus = Convert.ToInt32(planInfo[5]);
                        Module = planInfo[4].ToString();
                        ImplementationInfo.StartDate = Convert.ToInt32(planInfo[2]);
                        ImplementationInfo.EndDate = Convert.ToInt32(planInfo[3]);

                    }

                    if (planStatus == 3) //是正在执行的当前计划
                    {
                        //剩余天数和进度
                        InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
                        PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
                        if (PRlist != null)
                        {
                            ImplementationInfo.RemainingDays = PRlist[0].ToString();
                            ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();

                        }

                        //最近一周的依从率
                        InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
                        weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.StartDate);
                        if (weekPeriod != null)
                        {
                            ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
                        }
                    }
                    else  //已经结束计划
                    {
                        ImplementationInfo.RemainingDays = "0";
                        ImplementationInfo.ProgressRate = "100";
                        ImplementationInfo.CompliacneValue = "整个计划依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, ImplementationInfo.StartDate, ImplementationInfo.EndDate) + "%";
                    }

                    #region  读取任务执行情况，体征、用药等

                    //读取任务列表
                    DataTable TaskList = new DataTable();
                    TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                    //ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);

                    //测量-体征切换下拉框  
                    string condition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = TaskList.Select(condition);
                    List<SignShow> SignList = new List<SignShow>();
                    for (int j = 0; j < VitalSignRows.Length; j++)
                    {
                        SignShow SignShow = new SignShow();
                        SignShow.SignName = VitalSignRows[j]["CodeName"].ToString();
                        SignShow.SignCode = VitalSignRows[j]["Code"].ToString();
                        SignList.Add(SignShow);
                    }
                    ImplementationInfo.SignList = SignList;



                    List<MstBloodPressure> reference = new List<MstBloodPressure>();
                    ChartData ChartData = new ChartData();
                    List<Graph> GraphList = new List<Graph>();
                    GraphGuide GraphGuide = new GraphGuide();

                    if (Module == "M1")  //后期维护的话，在这里添加不同的模块判断
                    {

                        //高血压模块  体征测量-血压（收缩压、舒张压）、脉率   【默认显示-收缩压，血压必有，脉率可能有】  
                        condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
                        DataRow[] HyperTensionRows = TaskList.Select(condition);

                        //注意：需要兼容之前没有脉率的情况
                        if ((HyperTensionRows != null) && (HyperTensionRows.Length >= 2))  //M1 收缩压（默认显示）、舒张压、脉率  前两者肯定有，脉率不一定有
                        {
                            //获取血压的分级规则，脉率的分级原则写死在webservice
                            reference = CmMstBloodPressure.GetBPGrades(_cnCache);

                            //首次进入，默认加载收缩压
                            GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, "Bloodpressure|Bloodpressure_1", ImplementationInfo.StartDate, ImplementationInfo.EndDate, reference);

                            //初始值、目标值、分级规则加工
                            if (GraphList.Count > 0)
                            {
                                GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, "Bloodpressure|Bloodpressure_1", reference);
                                ChartData.GraphGuide = GraphGuide;
                            }
                        }
                    }


                    //必有测量任务，其他任务（例如吃药）可能没有

                    //其他任务依从情况
                    List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                    //是否有其他任务
                    //string condition1 = " Type not in ('VitalSign,')";
                    if (TaskList.Rows.Count == VitalSignRows.Length)
                    {
                        ChartData.OtherTasks = "0";
                    }
                    else
                    {
                        ChartData.OtherTasks = "1";
                        TasksComByPeriod = PsCompliance.GetTasksComCountByPeriod(_cnCache, PatientId, PlanNo, ImplementationInfo.StartDate, ImplementationInfo.EndDate);
                        if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                        {
                            for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                            {
                                GraphList[rowsCount].DrugValue = "1";
                                GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                                GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                                GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;
                            }
                        }
                    }

                    ChartData.GraphList = GraphList;
                    ImplementationInfo.ChartData = ChartData;

                    #endregion
                }

                str_result = JSONHelper.ObjectToJson(ImplementationInfo);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(str_result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();
                //return ImplementationInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPadSecond", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取某体征的数据和画图信息  Table:Ps.VitalSigns  Author:LS 2015-06-29")]
        // GetSignInfoByCode 获取某体征的数据和画图信息（收缩压、舒张压、脉率）  LS 2015-06-29  Pad和Phone都要用
        //关于输入 StartDate，EndDate  Pad首次没有拿出StartDate，EndDate    Phone拿出了 这样规划比较好
        public void GetSignInfoByCode(string PatientId, string PlanNo, string ItemCode, int StartDate, int EndDate)
        {
            ChartData ChartData = new ChartData();
            List<Graph> GraphList = new List<Graph>();
            GraphGuide GraphGuide = new GraphGuide();
            List<MstBloodPressure> reference = new List<MstBloodPressure>();

            try
            {
                string Module = "";
                InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                if (planInfo != null)
                {
                    Module = planInfo[4].ToString();

                }

                if (Module == "M1")
                {
                    if ((ItemCode == "Bloodpressure|Bloodpressure_1") || (ItemCode == "Bloodpressure|Bloodpressure_2"))
                    {
                        reference = CmMstBloodPressure.GetBPGrades(_cnCache);
                    }

                    GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, ItemCode, StartDate, EndDate, reference);

                    //初始值、目标值、分级规则加工
                    if (GraphList.Count > 0)
                    {
                        GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, ItemCode, reference);
                        ChartData.GraphGuide = GraphGuide;
                    }
                }

                //读取任务列表  必有测量任务，其他任务（例如吃药）可能没有
                DataTable TaskList = new DataTable();
                TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                string condition = " Type = 'VitalSign'";
                DataRow[] VitalSignRows = TaskList.Select(condition);

                //其他任务依从情况
                List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                //是否有其他任务
                if (TaskList.Rows.Count == VitalSignRows.Length)
                {
                    ChartData.OtherTasks = "0";
                }
                else
                {
                    ChartData.OtherTasks = "1";
                    TasksComByPeriod = PsCompliance.GetTasksComCountByPeriod(_cnCache, PatientId, PlanNo, StartDate, EndDate);
                    if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                    {
                        for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                        {
                            GraphList[rowsCount].DrugValue = "1";   //已经初始化过
                            GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                            GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                            GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;
                        }
                    }
                }
                ChartData.GraphList = GraphList;


                //return result;
                string a = JSONHelper.ObjectToJson(ChartData);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(a);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSignInfoByCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }



        [WebMethod(Description = "通过某计划的日期，获取该天的任务完成详情 用于图上点点击时弹框内容..  Author:LS 2015-07-02")]
        //新加 GetImplementationByDate 通过某计划的日期，获取该天的任务完成详情 用于弹框
        public void GetImplementationByDate(string PatientId, string PlanNo, string DateSelected)
        {
            TaskComDetailByD TaskComDetailByD = new TaskComDetailByD(); //voidDateTime
            string str_result = "";  //最终的输出-ImplementationInfo转化成json格式
            try
            {
                //DateSelected形式"20150618" 或"15/06/18"  目前使用前者
                int Date = Convert.ToInt32(DateSelected);
                //string temp = "20" + DateSelected;
                //int Date = Convert.ToInt32(Convert.ToDateTime(temp).ToString("yyyyMMdd"));
                //int Date = Convert.ToInt32(DateSelected.ToString("yyyyMMdd"));
                TaskComDetailByD = PsCompliance.GetImplementationByDate(_cnCache, PatientId, PlanNo, Convert.ToInt32(Date));


                str_result = JSONHelper.ObjectToJson(TaskComDetailByD);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(str_result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();
                //return ImplementationInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationByDate", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }



        //Pad和Phone区别 后者只看最近七天的  其他任务的依从情况也行
        [WebMethod(Description = "获取计划完成情况（Phone)-查看当前计划近一周的情况  Table:计划、任务、依从..  Author:LS 2015-03-27")]
        // GetImplementationForPhone 获取计划完成情况（Pad）  LS 2015-06-29  
        public void GetImplementationForPhone(string PatientId, string Module)
        {
            ImplementationPhone ImplementationPhone = new ImplementationPhone();
            string str_result = "";
            try
            {
                //病人基本信息-头像、姓名.. (由于手机版只针对换换咋用户，基本信息可不用获取
                // CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, PatientId);
                //if (patientList != null)
                //{
                //ImplementationPhone.PatientInfo.PatientName = patientList[0];
                //}

                int planStartDate = 0;
                int planEndDate = 0;
                string PlanNo = "";

                InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                planInfo = PsPlan.GetExecutingPlanByM(_cnCache, PatientId, Module);
                if (planInfo != null)
                {
                    PlanNo = planInfo[0].ToString();
                    planStartDate = Convert.ToInt32(planInfo[2]);
                    planEndDate = Convert.ToInt32(planInfo[3]);  //未用到
                }

                if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
                {
                    ImplementationPhone.NowPlanNo = PlanNo;

                    //剩余天数和进度
                    InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
                    PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
                    if (PRlist != null)
                    {
                        ImplementationPhone.RemainingDays = PRlist[0].ToString();  //"距离本次计划结束还剩"+PRlist[0]+"天";
                        ImplementationPhone.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();  //"进度："++"%";
                    }

                    //最近一周的依从率
                    InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
                    weekPeriod = PsPlan.GetWeekPeriod(_cnCache, planStartDate);
                    if (weekPeriod != null)
                    {
                        ImplementationPhone.CompliacneValue = PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]));
                        ImplementationPhone.StartDate = Convert.ToInt32(weekPeriod[0]);  //用于获取血压的详细数据
                        ImplementationPhone.EndDate = Convert.ToInt32(weekPeriod[1]);
                    }

                    #region  读取任务执行情况，血压、用药-最近一周的数据

                    //读取任务  phone版 此函数其他任务也显示
                    DataTable TaskList = new DataTable();
                    TaskList = PsTask.GetTaskList(_cnCache, PlanNo);

                    //测量-体征切换下拉框  
                    string condition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = TaskList.Select(condition);
                    List<SignShow> SignList = new List<SignShow>();
                    for (int j = 0; j < VitalSignRows.Length; j++)
                    {
                        SignShow SignShow = new SignShow();
                        SignShow.SignName = VitalSignRows[j]["CodeName"].ToString();
                        SignShow.SignCode = VitalSignRows[j]["Code"].ToString();
                        SignList.Add(SignShow);
                    }
                    ImplementationPhone.SignList = SignList;


                    List<MstBloodPressure> reference = new List<MstBloodPressure>();
                    ChartData ChartData = new ChartData();
                    List<Graph> GraphList = new List<Graph>();
                    GraphGuide GraphGuide = new GraphGuide();

                    if (Module == "M1")  //后期维护的话，在这里添加不同的模块判断
                    {
                        condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
                        DataRow[] HyperTensionRows = TaskList.Select(condition);

                        //注意：需要兼容之前没有脉率的情况
                        if ((HyperTensionRows != null) && (HyperTensionRows.Length >= 2))  //M1 收缩压（默认显示）、舒张压、脉率  前两者肯定有，脉率不一定有
                        {
                            //获取血压的分级规则，脉率的分级原则写死在webservice
                            reference = CmMstBloodPressure.GetBPGrades(_cnCache);

                            //首次进入，默认加载收缩压
                            GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, "Bloodpressure|Bloodpressure_1", Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]), reference);

                            //初始值、目标值、分级规则加工
                            if (GraphList.Count > 0)
                            {
                                GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, "Bloodpressure|Bloodpressure_1", reference);
                                ChartData.GraphGuide = GraphGuide;
                            }
                        }

                    }
                    else
                    {

                    }

                    //必有测量任务，其他任务（例如吃药）可能没有
                    //其他任务依从情况 //是否有其他任务
                    List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                    //string condition1 = " Type not in ('VitalSign,')";
                    if (TaskList.Rows.Count == VitalSignRows.Length)
                    {
                        ChartData.OtherTasks = "0";
                    }
                    else
                    {
                        ChartData.OtherTasks = "1";
                        TasksComByPeriod = PsCompliance.GetTasksComCountByPeriod(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1]));
                        if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                        {
                            for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                            {
                                GraphList[rowsCount].DrugValue = "1";   //已经初始化过
                                GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                                GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                                GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;//+ "<br><a onclick= shuang shuang zz(); shuang shuang;>详细</a>"
                            }
                        }
                    }


                    #endregion

                    ChartData.GraphList = GraphList;
                    ImplementationPhone.ChartData = ChartData;
                }

                str_result = JSONHelper.ObjectToJson(ImplementationPhone);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(str_result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();
                //return ImplementationInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPhone", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }

        //可后期优化
        [WebMethod(Description = "获取某日期之前，一定条数血压（收缩压/舒张压）和脉率的数据详细时刻列表-phone  Table:Ps.VitalSigns  Author:LS 2015-07-02")]
        // GetSignsDetailByPeriod 获取某日期之前，一定条数血压（收缩压/舒张压）和脉率的数据详细时刻列表  LS 2015-07-02  用于phone，支持继续加载
        public void GetSignsDetailByPeriod(string PatientId, string Module, int StartDate, int Num)
        {
            SignDetailByP result = new SignDetailByP();

            try
            {

                int CacheStartDate = 0;
                int CacheEndDate = 0;

                /*严格天数平移
               string str_CacheEndDate = PsVitalSigns.GetLatestVitalSignDate(_cnCache, PatientId, StartDate);
               if ((str_CacheEndDate != "") && (str_CacheEndDate != null))
               {
                   CacheEndDate = Convert.ToInt32(str_CacheEndDate);
                   string time = str_CacheEndDate.Substring(0, 4) + "-" + str_CacheEndDate.Substring(4, 2) + "-" + str_CacheEndDate.Substring(6, 2);
                   DateTime starttime = Convert.ToDateTime(time);
                   CacheStartDate = Convert.ToInt32(starttime.AddDays(-Num).ToString("yyyyMMdd"));
                   result.NextStartDate = CacheStartDate;
               }
               else
               {

               }
                */

                //按有数据的天数平移
                CacheSysList dateList = PsVitalSigns.GetVitalSignDates(_cnCache, PatientId, StartDate, Num);
                if (dateList != null)
                {
                    if ((dateList[0] != null) && (dateList[1] != null))
                    {
                        CacheStartDate = Convert.ToInt32(dateList[0]);
                        CacheEndDate = Convert.ToInt32(dateList[1]);
                        result.NextStartDate = CacheStartDate;
                    }
                    else if ((dateList[0] == null) && (dateList[1] != null))
                    {
                        //CacheStartDate =0;
                        CacheEndDate = Convert.ToInt32(dateList[1]);
                        result.NextStartDate = -1; //取完的标志

                    }
                    else if ((dateList[0] == null) && (dateList[1] == null))
                    {
                        //CacheStartDate = 0;
                        // CacheEndDate = 0;
                        result.NextStartDate = -1;
                    }

                }


                //收缩压
                DataTable sysInfo = new DataTable();
                sysInfo = PsVitalSigns.GetTypedSignDetailByPeriod(_cnCache, PatientId, "Bloodpressure", "Bloodpressure_1", CacheStartDate, CacheEndDate);

                //舒张压
                DataTable diaInfo = new DataTable();
                diaInfo = PsVitalSigns.GetTypedSignDetailByPeriod(_cnCache, PatientId, "Bloodpressure", "Bloodpressure_2", CacheStartDate, CacheEndDate);

                //脉率
                DataTable pulInfo = new DataTable();
                pulInfo = PsVitalSigns.GetTypedSignDetailByPeriod(_cnCache, PatientId, "Pulserate", "Pulserate_1", CacheStartDate, CacheEndDate);

                //list.PrimaryKey = new DataColumn[] { list.Columns["RecordDate"], list.Columns["RecordTime"], list.Columns["SignType"], };

                //三张表整合，按时间排序 避免条数可能不一致造成的问题  
                sysInfo.Merge(diaInfo);
                sysInfo.Merge(pulInfo);

                //按RecordDate、RecordTime、SignType排序  再合并成收集需要的形式
                DataView dv = sysInfo.DefaultView;
                dv.Sort = "RecordDate desc, RecordTime asc,SignType asc";
                DataTable dt_Sort = dv.ToTable();

                //1 收缩压, 2 舒张压, 3 脉率 
                //整理成日期、时刻、数值的形式
                //整理成列表形式 2011/01/03 星期三 
                //08:00 137 95 66
                //09:00 134 78 66
                if (dt_Sort.Rows.Count > 0)
                {
                    SignDetail SignDetail = new SignDetail();
                    SignDetail.DetailTime = dt_Sort.Rows[0]["RecordTime"].ToString();
                    if (dt_Sort.Rows[0]["SignType"].ToString() == "1")
                    {
                        SignDetail.SBPValue = dt_Sort.Rows[0]["Value"].ToString();
                    }
                    else if (dt_Sort.Rows[0]["SignType"].ToString() == "2")
                    {
                        SignDetail.DBPValue = dt_Sort.Rows[0]["Value"].ToString();
                    }
                    else
                    {
                        SignDetail.PulseValue = dt_Sort.Rows[0]["Value"].ToString();
                    }

                    SignDetailByD SignDetailByD = new SignDetailByD();
                    SignDetailByD.Date = dt_Sort.Rows[0]["RecordDate"].ToString();
                    SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(dt_Sort.Rows[0]["RecordDate"].ToString());

                    if (dt_Sort.Rows.Count == 1)
                    {
                        SignDetailByD.SignDetailList.Add(SignDetail);
                        result.SignDetailByDs.Add(SignDetailByD);
                    }
                    else
                    {
                        string temp_date = dt_Sort.Rows[0]["RecordDate"].ToString();
                        string temp_hour = dt_Sort.Rows[0]["RecordTime"].ToString();

                        for (int rowsCount = 1; rowsCount < dt_Sort.Rows.Count; rowsCount++)
                        {
                            if (rowsCount != dt_Sort.Rows.Count - 1)
                            {
                                #region 不是最后一条

                                if (temp_date == dt_Sort.Rows[rowsCount]["RecordDate"].ToString())
                                {
                                    #region 同一天
                                    if (temp_hour == dt_Sort.Rows[rowsCount]["RecordTime"].ToString())
                                    {
                                        if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                        {
                                            SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                        {
                                            SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else
                                        {
                                            SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        SignDetailByD.SignDetailList.Add(SignDetail);

                                        SignDetail = new SignDetail();
                                        SignDetail.DetailTime = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                        if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                        {
                                            SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                        {
                                            SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else
                                        {
                                            SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }

                                        temp_hour = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 不同天
                                    SignDetailByD.SignDetailList.Add(SignDetail);
                                    result.SignDetailByDs.Add(SignDetailByD);

                                    SignDetailByD = new SignDetailByD();
                                    SignDetail = new SignDetail();
                                    SignDetail.DetailTime = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                    if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                    {
                                        SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                    {
                                        SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    else
                                    {
                                        SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    SignDetailByD.Date = dt_Sort.Rows[rowsCount]["RecordDate"].ToString();
                                    SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(dt_Sort.Rows[rowsCount]["RecordDate"].ToString());

                                    temp_date = dt_Sort.Rows[rowsCount]["RecordDate"].ToString();
                                    temp_hour = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();

                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region 最后一条

                                if (temp_date == dt_Sort.Rows[rowsCount]["RecordDate"].ToString())
                                {
                                    #region 同一天
                                    if (temp_hour == dt_Sort.Rows[rowsCount]["RecordTime"].ToString())
                                    {
                                        if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                        {
                                            SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                        {
                                            SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else
                                        {
                                            SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        SignDetailByD.SignDetailList.Add(SignDetail);
                                        result.SignDetailByDs.Add(SignDetailByD);
                                    }
                                    else
                                    {
                                        SignDetailByD.SignDetailList.Add(SignDetail);

                                        SignDetail = new SignDetail();
                                        SignDetail.DetailTime = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                        if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                        {
                                            SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                        {
                                            SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }
                                        else
                                        {
                                            SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                        }

                                        temp_hour = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                        SignDetailByD.SignDetailList.Add(SignDetail);
                                        result.SignDetailByDs.Add(SignDetailByD);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 不同天
                                    SignDetailByD.SignDetailList.Add(SignDetail);
                                    result.SignDetailByDs.Add(SignDetailByD);

                                    SignDetailByD = new SignDetailByD();
                                    SignDetail = new SignDetail();
                                    SignDetail.DetailTime = dt_Sort.Rows[rowsCount]["RecordTime"].ToString();
                                    if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "1")
                                    {
                                        SignDetail.SBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    else if (dt_Sort.Rows[rowsCount]["SignType"].ToString() == "2")
                                    {
                                        SignDetail.DBPValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    else
                                    {
                                        SignDetail.PulseValue = dt_Sort.Rows[rowsCount]["Value"].ToString();
                                    }
                                    SignDetailByD.Date = dt_Sort.Rows[rowsCount]["RecordDate"].ToString();
                                    SignDetailByD.WeekDay = PsCompliance.CaculateWeekDay(dt_Sort.Rows[rowsCount]["RecordDate"].ToString());

                                    temp_date = dt_Sort.Rows[rowsCount]["RecordDate"].ToString();
                                    SignDetailByD.SignDetailList.Add(SignDetail);
                                    result.SignDetailByDs.Add(SignDetailByD);

                                    #endregion
                                }

                                #endregion
                            }

                        }
                    }
                }

                //return result;
                string a = JSONHelper.ObjectToJson(result);
                Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                Context.Response.Write(a);
                //Context.Response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSignsDetailByPeriod", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }



        //网站与Pad差别不大，除了输出形式，pad是json，web是xml  web不用基本信息，多出任务列表的输出  web貌似不能直接调json  web目前暂时ballon显示其他任务所有文本
        [WebMethod(Description = "获取健康计划完成情况（Web)-任务列表、任务完成情况  Table:计划、任务、依从..  Author:LS 2015-06-28")]
        // GetImplementationForWeb 获取计划完成情况（Web）  LS 2015-03-27  
        public ImplementationInfo GetImplementationForWebFirst(string PatientId, string Module)
        {
            ImplementationInfo ImplementationInfo = new ImplementationInfo();
            try
            {
                string PlanNo = "";

                //病人基本信息-姓名、头像..
                //CacheSysList patientList = PsBasicInfo.GetPatientBasicInfo(_cnCache, PatientId);
                //if (patientList != null)
                //{
                //    ImplementationInfo.PatientInfo.PatientName = patientList[0];

                //    CacheSysList BasicInfoDetail = PsBasicInfoDetail.GetDetailInfo(_cnCache, PatientId);
                //    if (BasicInfoDetail != null)
                //    {
                //        if (BasicInfoDetail[6] != null)
                //        {
                //            ImplementationInfo.PatientInfo.ImageUrl = BasicInfoDetail[6].ToString();
                //        }
                //        else
                //        {
                //            ImplementationInfo.PatientInfo.ImageUrl = "";  //js端意外不能识别null
                //        }

                //    }
                //}

                //刚进入页面加载计划列表 (始终存在第一条-当前计划）
                ImplementationInfo.PlanList = PsPlan.GetPlanList34ByM(_cnCache, PatientId, Module);

                PlanNo = ImplementationInfo.PlanList[0].PlanNo; //肯定会存在 

                #region  存在正在执行的计划

                if ((PlanNo != "") && (PlanNo != null))  //存在正在执行的计划
                {
                    //剩余天数和进度
                    InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
                    PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
                    if (PRlist != null)
                    {
                        ImplementationInfo.RemainingDays = PRlist[0].ToString();
                        ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();

                        ImplementationInfo.StartDate = ImplementationInfo.PlanList[0].StartDate;
                        ImplementationInfo.EndDate = ImplementationInfo.PlanList[0].EndDate;
                    }

                    //正在执行计划的最近一周的依从率
                    InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
                    weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.PlanList[0].StartDate);
                    if (weekPeriod != null)
                    {

                        ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
                    }

                    //读取任务列表
                    DataTable TaskList = new DataTable();
                    TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                    ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);


                    //测量-体征切换下拉框  
                    string condition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = TaskList.Select(condition);
                    List<SignShow> SignList = new List<SignShow>();
                    for (int j = 0; j < VitalSignRows.Length; j++)
                    {
                        SignShow SignShow = new SignShow();
                        SignShow.SignName = VitalSignRows[j]["CodeName"].ToString();
                        SignShow.SignCode = VitalSignRows[j]["Code"].ToString();
                        SignList.Add(SignShow);
                    }
                    ImplementationInfo.SignList = SignList;

                    List<MstBloodPressure> reference = new List<MstBloodPressure>();
                    ChartData ChartData = new ChartData();
                    List<Graph> GraphList = new List<Graph>();
                    GraphGuide GraphGuide = new GraphGuide();

                    if (Module == "M1")  //后期维护的话，在这里添加不同的模块判断
                    {

                        //高血压模块  体征测量-血压（收缩压、舒张压）、脉率   【默认显示-收缩压，血压必有，脉率可能有】  
                        condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
                        DataRow[] HyperTensionRows = TaskList.Select(condition);

                        //注意：需要兼容之前没有脉率的情况
                        if ((HyperTensionRows != null) && (HyperTensionRows.Length >= 2))  //M1 收缩压（默认显示）、舒张压、脉率  前两者肯定有，脉率不一定有
                        {
                            //获取血压的分级规则，脉率的分级原则写死在webservice
                            reference = CmMstBloodPressure.GetBPGrades(_cnCache);

                            //首次进入，默认加载收缩压
                            GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, "Bloodpressure|Bloodpressure_1", ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate, reference);
                            //GraphList = CmMstBloodPressure.GetSignInfoByBP

                            //初始值、目标值、分级规则加工
                            if (GraphList.Count > 0)
                            {
                                GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, "Bloodpressure|Bloodpressure_1", reference);
                                ChartData.GraphGuide = GraphGuide;
                            }
                        }
                    }


                    //必有测量任务，其他任务（例如吃药）可能没有

                    //其他任务依从情况
                    List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                    if (TaskList.Rows.Count == VitalSignRows.Length)
                    {
                        ChartData.OtherTasks = "0";
                    }
                    else
                    {
                        ChartData.OtherTasks = "1";
                        TasksComByPeriod = PsCompliance.GetTasksComByPeriodWeb(_cnCache, PatientId, PlanNo, ImplementationInfo.PlanList[0].StartDate, ImplementationInfo.PlanList[0].EndDate);
                        if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                        {
                            for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                            {
                                GraphList[rowsCount].DrugValue = "1";   //已经初始化过
                                GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                                GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                                GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;
                            }
                        }
                    }
                    ChartData.GraphList = GraphList;
                    ImplementationInfo.ChartData = ChartData;
                }

                #endregion

                return ImplementationInfo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForWebFirst", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取健康计划完成情况（Web)-任务列表、任务完成情况  Table:计划、任务、依从..  Author:LS 2015-06-28")]
        // GetImplementationForWebSecond 获取计划完成情况（Web）-查看往期计划  LS 2015-06-28  
        public ImplementationInfo GetImplementationForWebSecond(string PatientId, string PlanNo)
        {
            ImplementationInfo ImplementationInfo = new ImplementationInfo();
            //string str_result = "";
            string Module = "";
            try
            {
                //Pad保证PlanNo输入不为空  为空的表示无当前计划，则显示无执行即可，无需连接网络服务

                if ((PlanNo != "") && (PlanNo != null)) //存在正在执行的计划
                {
                    //获取计划的相关信息
                    int planStatus = 0;

                    InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                    planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                    if (planInfo != null)
                    {
                        planStatus = Convert.ToInt32(planInfo[5]);
                        Module = planInfo[4].ToString();

                        ImplementationInfo.StartDate = Convert.ToInt32(planInfo[2]);
                        ImplementationInfo.EndDate = Convert.ToInt32(planInfo[3]);

                    }

                    if (planStatus == 3) //是正在执行的当前计划
                    {
                        //剩余天数和进度
                        InterSystems.Data.CacheTypes.CacheSysList PRlist = null;
                        PRlist = PsPlan.GetProgressRate(_cnCache, PlanNo);
                        if (PRlist != null)
                        {
                            ImplementationInfo.RemainingDays = PRlist[0].ToString();
                            ImplementationInfo.ProgressRate = (Convert.ToDouble(PRlist[1]) * 100).ToString();

                        }

                        //最近一周的依从率
                        InterSystems.Data.CacheTypes.CacheSysList weekPeriod = null;
                        weekPeriod = PsPlan.GetWeekPeriod(_cnCache, ImplementationInfo.StartDate);
                        if (weekPeriod != null)
                        {
                            ImplementationInfo.CompliacneValue = "最近一周依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, Convert.ToInt32(weekPeriod[0]), Convert.ToInt32(weekPeriod[1])) + "%";
                        }
                    }
                    else  //已经结束计划
                    {
                        ImplementationInfo.RemainingDays = "0";
                        ImplementationInfo.ProgressRate = "100";
                        ImplementationInfo.CompliacneValue = "整个计划依从率为：" + PsCompliance.GetCompliacneRate(_cnCache, PatientId, PlanNo, ImplementationInfo.StartDate, ImplementationInfo.EndDate) + "%";
                    }

                    #region  读取任务执行情况，体征、用药等

                    //读取任务列表
                    DataTable TaskList = new DataTable();
                    TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                    ImplementationInfo.TaskList = PsTask.GetSpTaskList(_cnCache, PlanNo);

                    //测量-体征切换下拉框  
                    string condition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = TaskList.Select(condition);
                    List<SignShow> SignList = new List<SignShow>();
                    for (int j = 0; j < VitalSignRows.Length; j++)
                    {
                        SignShow SignShow = new SignShow();
                        SignShow.SignName = VitalSignRows[j]["CodeName"].ToString();
                        SignShow.SignCode = VitalSignRows[j]["Code"].ToString();
                        SignList.Add(SignShow);
                    }
                    ImplementationInfo.SignList = SignList;


                    List<MstBloodPressure> reference = new List<MstBloodPressure>();
                    ChartData ChartData = new ChartData();
                    List<Graph> GraphList = new List<Graph>();
                    GraphGuide GraphGuide = new GraphGuide();

                    if (Module == "M1")  //后期维护的话，在这里添加不同的模块判断
                    {

                        //高血压模块  体征测量-血压（收缩压、舒张压）、脉率   【默认显示-收缩压，血压必有，脉率可能有】  
                        condition = " Code = 'Bloodpressure|Bloodpressure_1' or  Code = 'Bloodpressure|Bloodpressure_2'or  Code = 'Pulserate|Pulserate_1'";
                        DataRow[] HyperTensionRows = TaskList.Select(condition);

                        //注意：需要兼容之前没有脉率的情况
                        if ((HyperTensionRows != null) && (HyperTensionRows.Length >= 2))  //M1 收缩压（默认显示）、舒张压、脉率  前两者肯定有，脉率不一定有
                        {
                            //获取血压的分级规则，脉率的分级原则写死在webservice
                            reference = CmMstBloodPressure.GetBPGrades(_cnCache);

                            //首次进入，默认加载收缩压
                            GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, "Bloodpressure|Bloodpressure_1", ImplementationInfo.StartDate, ImplementationInfo.EndDate, reference);

                            //初始值、目标值、分级规则加工
                            if (GraphList.Count > 0)
                            {
                                GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, "Bloodpressure|Bloodpressure_1", reference);
                                ChartData.GraphGuide = GraphGuide;
                            }
                        }
                    }


                    //必有测量任务，其他任务（例如吃药）可能没有

                    //其他任务依从情况
                    List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                    if (TaskList.Rows.Count == VitalSignRows.Length)
                    {
                        ChartData.OtherTasks = "0";
                    }
                    else
                    {
                        ChartData.OtherTasks = "1";
                        TasksComByPeriod = PsCompliance.GetTasksComByPeriodWeb(_cnCache, PatientId, PlanNo, ImplementationInfo.StartDate, ImplementationInfo.EndDate);
                        if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                        {
                            for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                            {
                                GraphList[rowsCount].DrugValue = "1";
                                GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                                GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                                GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;
                            }
                        }
                    }
                    ChartData.GraphList = GraphList;
                    ImplementationInfo.ChartData = ChartData;

                    #endregion
                }

                return ImplementationInfo;
                //str_result = JSONHelper.ObjectToJson(ImplementationInfo);
                //Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
                //Context.Response.Write(str_result);
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
                //Context.Response.End();

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetImplementationForPadSecond", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取某体征的数据和画图信息（Web）  Table:Ps.VitalSigns  Author:LS 2015-06-29")]
        // GetSignInfoByCodeWeb 获取某体征的数据和画图信息（收缩压、舒张压、脉率）  LS 2015-06-29 
        //关于输入 StartDate，EndDate  Pad首次没有拿出StartDate，EndDate    
        public ChartData GetSignInfoByCodeWeb(string PatientId, string PlanNo, string ItemCode, int StartDate, int EndDate)
        {
            ChartData ChartData = new ChartData();
            List<Graph> GraphList = new List<Graph>();
            GraphGuide GraphGuide = new GraphGuide();
            List<MstBloodPressure> reference = new List<MstBloodPressure>();

            try
            {
                string Module = "";
                InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                planInfo = PsPlan.GetPlanInfo(_cnCache, PlanNo);
                if (planInfo != null)
                {
                    Module = planInfo[4].ToString();

                }

                if (Module == "M1")
                {
                    if ((ItemCode == "Bloodpressure|Bloodpressure_1") || (ItemCode == "Bloodpressure|Bloodpressure_2"))
                    {
                        reference = CmMstBloodPressure.GetBPGrades(_cnCache);
                    }

                    GraphList = CmMstBloodPressure.GetSignInfoByM1(_cnCache, PatientId, PlanNo, ItemCode, StartDate, EndDate, reference);

                    //初始值、目标值、分级规则加工
                    if (GraphList.Count > 0)
                    {
                        GraphGuide = CmMstBloodPressure.GetGuidesByCode(_cnCache, PlanNo, ItemCode, reference);
                        ChartData.GraphGuide = GraphGuide;
                    }
                }

                //读取任务列表  必有测量任务，其他任务（例如吃药）可能没有
                DataTable TaskList = new DataTable();
                TaskList = PsTask.GetTaskList(_cnCache, PlanNo);
                string condition = " Type = 'VitalSign'";
                DataRow[] VitalSignRows = TaskList.Select(condition);

                //其他任务依从情况
                List<CompliacneDetailByD> TasksComByPeriod = new List<CompliacneDetailByD>();
                //是否有其他任务
                if (TaskList.Rows.Count == VitalSignRows.Length)
                {
                    ChartData.OtherTasks = "0";
                }
                else
                {
                    ChartData.OtherTasks = "1";
                    TasksComByPeriod = PsCompliance.GetTasksComByPeriodWeb(_cnCache, PatientId, PlanNo, StartDate, EndDate);
                    if ((TasksComByPeriod != null) && (TasksComByPeriod.Count == GraphList.Count))
                    {
                        for (int rowsCount = 0; rowsCount < TasksComByPeriod.Count; rowsCount++)
                        {
                            GraphList[rowsCount].DrugValue = "1";   //已经初始化过
                            GraphList[rowsCount].DrugBullet = TasksComByPeriod[rowsCount].drugBullet;
                            GraphList[rowsCount].DrugColor = TasksComByPeriod[rowsCount].drugColor;
                            GraphList[rowsCount].DrugDescription = TasksComByPeriod[rowsCount].Events;
                        }
                    }
                }
                ChartData.GraphList = GraphList;


                return ChartData;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetSignInfoByCode", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                //return null;
                throw (ex);
            }
        }
        #endregion

        [WebMethod(Description = "获取远程调用的IP，Author:ZC 2015-07-01")]
        public string getRemoteIPAddress()
        {
            string visitorIP = "";
            visitorIP = HttpContext.Current.Request.UserHostAddress;
            return visitorIP;
        }


        #region <字典维护页面 WY>
        [WebMethod(Description = "GetTmpDrugByStatus Table：Tmp.DrugDict Author:WY  2015-07-09")]
        public DataSet GetTmpDrugByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpDrug = new DataTable();
                DataSet DS_TmpDrug = new DataSet();
                DT_TmpDrug = TmpDrugDict.GetListByStatus(_cnCache, Status);
                DS_TmpDrug.Tables.Add(DT_TmpDrug);
                return DS_TmpDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpDrugByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetMpDrugCmp Table：Mp.DrugCmp Author:WY  2015-07-10")]
        public DataSet GetMpDrugCmp()
        {
            try
            {
                DataTable DT_MpDrug = new DataTable();
                DataSet DS_MpDrug = new DataSet();
                DT_MpDrug = MpDrugCmp.GetMpDrugCmp(_cnCache);
                DS_MpDrug.Tables.Add(DT_MpDrug);
                return DS_MpDrug;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpDrugCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetDrugName Table：Cm.MstDrug Author:WY  2015-07-10")]
        public string GetDrugName(string DrugCode, string DrugSpec)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstDrug.GetDrugName(_cnCache, DrugCode + "**" + DrugSpec);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDrugName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetMpDrugCmp Table：Mp.DrugCmp Author:WY  2015-07-10")]
        public bool SetMpDrugCmp(string HospitalCode, string HZCode, string HzSpec, string DrugCode, string DrugSpec, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpDrugCmp.SetData(_cnCache, HospitalCode, HZCode + "**" + HzSpec, DrugCode + "**" + DrugSpec, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpDrugCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "ChangeStatusForTmpDrug Table：Tmp.DrugDict Author:WY  2015-07-10")]
        public bool ChangeStatusForTmpDrug(string HospitalCode, string DrugCode, string DrugSpec, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpDrugDict.ChangeStatus(_cnCache, HospitalCode, DrugCode, DrugSpec, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpDrug", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetTmpLabItemByStatus Table：Tmp.LabItemDict Author:WY  2015-07-10")]
        public DataSet GetTmpLabItemByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpLabItem = new DataTable();
                DataSet DS_TmpLabItem = new DataSet();
                DT_TmpLabItem = TmpLabItemDict.GetListByStatus(_cnCache, Status);
                DS_TmpLabItem.Tables.Add(DT_TmpLabItem);
                return DS_TmpLabItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpLatItemByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetMpLabItemCmp Table：Mp.LabItemsCmp Author:WY  2015-07-10")]
        public bool SetMpLabItemCmp(string HospitalCode, string HZCode, string Type, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpLabTestItemsCmp.SetData(_cnCache, HospitalCode, HZCode, Type, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpLabItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "ChangeStatusForTmpLabItem Table：Tmp.LabItemDict Author:WY  2015-07-10")]
        public bool ChangeStatusForTmpLabItem(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpLabItemDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpLabItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetMpLabItemCmp Table：Mp.LabItemsCmp Author:WY  2015-07-10")]
        public DataSet GetMpLabItemsCmp()
        {
            try
            {
                DataTable DT_MpLabItems = new DataTable();
                DataSet DS_MpLabItems = new DataSet();
                DT_MpLabItems = MpLabTestItemsCmp.GetMpLabTestItemsCmp(_cnCache);
                DS_MpLabItems.Tables.Add(DT_MpLabItems);
                return DS_MpLabItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpLabItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetLabTestItem Table：Cm.MstLabTestItems Author:WY  2015-07-10")]
        public bool SetLabTestItem(string Type, string Code, string TypeName, string Name, int SortNo, string InputCode, string Description, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                if (SortNo == 0)
                {
                    SortNo = CmMstLabTestItems.GetMaxSortNo(_cnCache) + 1;
                }
                ret = CmMstLabTestItems.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Description, InvalidFlag) ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabTestItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetLabItemTypeNameByType Table：Cm.MstLabTestItems Author:WY  2015-07-13")]
        public string GetLabItemTypeNameByType(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstLabTestItems.GetTypeNameByType(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabItemTypeNameByType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetLabItemName Table：Cm.MstLabTestItems Author:WY  2015-07-13")]
        public string GetLabItemName(string Type, string Code)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstLabTestItems.GetName(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabItemName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTmpLabSubItemByStatus Table：Tmp.LabResultDict Author:WY  2015-07-13")]
        public DataSet GetTmpLabSubItemByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpLabSubItem = new DataTable();
                DataSet DS_TmpLabSubItem = new DataSet();
                DT_TmpLabSubItem = TmpLabResultDict.GetListByStatus(_cnCache, Status);
                DS_TmpLabSubItem.Tables.Add(DT_TmpLabSubItem);
                return DS_TmpLabSubItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpLatSubItemByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetMpLabSubItemCmp Table：Mp.LabSubItemsCmp Author:WY  2015-07-13")]
        public bool SetMpLabSubItemCmp(string HospitalCode, string HZCode, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpLabSubItemsCmp.SetData(_cnCache, HospitalCode, HZCode, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpLabSubItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "ChangeStatusForTmpLabSubItem Table：Tmp.LabItemDict Author:WY  2015-07-13")]
        public bool ChangeStatusForTmpLabSubItem(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpLabResultDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpLabSubItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetMpLabSubItemCmp Table：Mp.LabSubItemsCmp Author:WY  2015-07-13")]
        public DataSet GetMpLabSubItemCmp()
        {
            try
            {
                DataTable DT_MpLabSubItems = new DataTable();
                DataSet DS_MpLabSubItems = new DataSet();
                DT_MpLabSubItems = MpLabSubItemsCmp.GetMpLabTestSubItemsCmp(_cnCache);
                DS_MpLabSubItems.Tables.Add(DT_MpLabSubItems);
                return DS_MpLabSubItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpLabSubItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetLabSubItem Table：Cm.MstLabTestSubItems Author:WY  2015-07-13")]
        public bool SetLabSubItem(string Code, string Name, int SortNo, string InputCode, string Description, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                if (SortNo == 0)
                {
                    SortNo = CmMstLabTestSubItems.GetMaxSortNo(_cnCache) + 1;
                }
                ret = CmMstLabTestSubItems.SetData(_cnCache, Code, Name, SortNo, InputCode, Description, InvalidFlag) ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetLabSubItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetLabSubItemName Table：Cm.MstLabTestSubItems Author:WY  2015-07-13")]
        public string GetLabSubItemName(string Code)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstLabTestSubItems.GetName(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetLabSubItemName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTmpExamItemByStatus Table：Tmp.ExaminationItemDict Author:WY  2015-07-13")]
        public DataSet GetTmpExamItemByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpExamItem = new DataTable();
                DataSet DS_TmpExamItem = new DataSet();
                DT_TmpExamItem = TmpExaminationItemDict.GetListByStatus(_cnCache, Status);
                DS_TmpExamItem.Tables.Add(DT_TmpExamItem);
                return DS_TmpExamItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpExamItemByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetMpExamItemCmp Table：Mp.ExaminationItemsCmp Author:WY  2015-07-13")]
        public bool SetMpExamItemCmp(string HospitalCode, string HZCode, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpExaminationItemCmp.SetData(_cnCache, HospitalCode, HZCode, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpExamItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "ChangeStatusForTmpExamItem Table：Tmp.ExaminationItemDict Author:WY  2015-07-13")]
        public bool ChangeStatusForTmpExamItem(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpExaminationItemDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpExamItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetMpExamItemCmp Table：Mp.ExaminationItemsCmp Author:WY  2015-07-13")]
        public DataSet GetMpExamItemCmp()
        {
            try
            {
                DataTable DT_MpExamItems = new DataTable();
                DataSet DS_MpExamItems = new DataSet();
                DT_MpExamItems = MpExaminationItemCmp.GetMpExaminationItemCmp(_cnCache);
                DS_MpExamItems.Tables.Add(DT_MpExamItems);
                return DS_MpExamItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpExamItemCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        //获取最大SortNo版SetData
        [WebMethod(Description = "SetExaminationSubItemS Table：Cm.MstExaminationSubItem Author:WY  2015-07-13")]
        public bool SetExaminationSubItemS(string Code, string Name, int SortNo, string InputCode, string Description, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                if (SortNo == 0)
                {
                    SortNo = CmMstExaminationSubItem.GetMaxSortNo(_cnCache) + 1;
                }
                ret = CmMstExaminationSubItem.SetData(_cnCache, Code, Name, SortNo, InputCode, Description, InvalidFlag) ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExaminationSubItemS", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetExamSubItemName Table：Cm.MstExaminationSubItem Author:WY  2015-07-13")]
        public string GetExamSubItemName(string Code)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstExaminationSubItem.GetName(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamSubItemName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTmpExaminationByStatus Table：Tmp.ExamDict Author:WY  2015-07-13")]
        public DataSet GetTmpExaminationByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpExamItem = new DataTable();
                DataSet DS_TmpExamItem = new DataSet();
                DT_TmpExamItem = TmpExamDict.GetListByStatus(_cnCache, Status);
                DS_TmpExamItem.Tables.Add(DT_TmpExamItem);
                return DS_TmpExamItem;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpExaminationByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetMpExaminationCmp Table：Mp.ExaminationCmp Author:WY  2015-07-13")]
        public bool SetMpExaminationCmp(string HospitalCode, string HZCode, string Type, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpExaminationCmp.SetData(_cnCache, HospitalCode, HZCode, Type, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpExaminationCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "ChangeStatusForTmpExamination Table：Tmp.ExamDict Author:WY  2015-07-13")]
        public bool ChangeStatusForTmpExamination(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpExamDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpExamination", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetMpExaminationCmp Table：Mp.ExaminationCmp Author:WY  2015-07-13")]
        public DataSet GetMpExaminationCmp()
        {
            try
            {
                DataTable DT_MpExamItems = new DataTable();
                DataSet DS_MpExamItems = new DataSet();
                DT_MpExamItems = MpExaminationCmp.GetMpExaminationItemCmp(_cnCache);
                DS_MpExamItems.Tables.Add(DT_MpExamItems);
                return DS_MpExamItems;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpExaminationCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetExamItem Table：Cm.MstExaminationItem Author:WY  2015-07-13")]
        public bool SetExamItem(string Type, string Code, string TypeName, string Name, int SortNo, string InputCode, string Description, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                if (SortNo == 0)
                {
                    SortNo = CmMstExaminationItem.GetMaxSortNo(_cnCache, Type) + 1;
                }
                ret = CmMstExaminationItem.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, SortNo, Description, InvalidFlag) ? true : false;
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetExamItem", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetExamItemTypeNameByType Table：Cm.MstExaminationItem Author:WY  2015-07-13")]
        public string GetExamItemTypeNameByType(string Type)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstExaminationItem.GetTypeNameByType(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamItemTypeNameByType", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetExamItemName Table：Cm.MstExaminationItem Author:WY  2015-07-13")]
        public string GetExamItemName(string Type, string Code)
        {
            try
            {
                string ret = string.Empty;
                ret = CmMstExaminationItem.GetNameByCode(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetExamItemName", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion

        #region <字典维护LPF>
        [WebMethod(Description = "SetData Table：Cm.MstOperation Author:lpf  2015-07-14")]
        public bool SetMpVitalSignsCmp(string piHospitalCode, string piHZCode, string piType, string piCode, string piDescription, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpVitalSignsCmp.SetData(_cnCache, piHospitalCode, piHZCode, piType, piCode, piDescription, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpVitalSignsCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstOperation Author:lpf  2015-07-10")]
        public bool SetMpOperationCmp(string piHospitalCode, string piHZCode, string piCode, string piDescription, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpOperationCmp.SetData(_cnCache, piHospitalCode, piHZCode, piCode, piDescription, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetOperation", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstOperation Author:lpf  2015-07-10")]
        public bool SetOperation(string piCode, string piName, string piInputCode, string piDescription, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstOperation.SetData(_cnCache, piCode, piName, piInputCode, piDescription, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetOperation", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetName Table: Cm.MstOperation Author:lpf  2015-07-10")]
        public string GetOperationName(string Code)
        {
            try
            {
                string ret = null;
                ret = CmMstOperation.GetName(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetOperationName ", "GetVitalSignsTypeNamebyType调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取手术字典 Table：Cm.MstOperation Author:LPF  2015-07-10")]
        public DataSet GetOperation()
        {
            try
            {
                DataTable DT_CmMstOperation = new DataTable();
                DataSet DS_CmMstOperation = new DataSet();
                DT_CmMstOperation = CmMstOperation.GetOperation(_cnCache);
                DS_CmMstOperation.Tables.Add(DT_CmMstOperation);
                return DS_CmMstOperation;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetCmMstOperation", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "更改未匹配手术状态 Table：Tmp.OperationDict Author:LPF  2015-07-10")]
        public bool ChangeStatusForTmpOperation(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpOperationDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpOperation", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "获取未匹配手术 Table：Tmp.OperationDict Author:LPF  2015-07-10")]
        public DataSet GetTmpOperationByStatus(int Status)
        {
            try
            {
                DataTable DT_TmpOperationDict = new DataTable();
                DataSet DS_TmpOperationDict = new DataSet();
                DT_TmpOperationDict = TmpOperationDict.GetTmpOperationByStatus(_cnCache, Status);
                DS_TmpOperationDict.Tables.Add(DT_TmpOperationDict);
                return DS_TmpOperationDict;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpOperationDict", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取匹配手术 Table：Mp.OperationCmp Author:LPF  2015-07-10")]
        public DataSet GetMpOperationCmp()
        {
            try
            {
                DataTable DT_MpOperationCmp = new DataTable();
                DataSet DS_MpOperationCmp = new DataSet();
                DT_MpOperationCmp = MpOperationCmp.GetMpOperationCmp(_cnCache);
                DS_MpOperationCmp.Tables.Add(DT_MpOperationCmp);
                return DS_MpOperationCmp;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpOperationCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取匹配体征 Table：Mp.VitalSignsCmp Author:LPF  2015-07-09")]
        public DataSet GetMpVitalSignsCmp()
        {
            try
            {
                DataTable DT_MpVitalSignsCmp = new DataTable();
                DataSet DS_MpVitalSignsCmp = new DataSet();
                DT_MpVitalSignsCmp = MpVitalSignsCmp.GetMpVitalSignsCmp(_cnCache);
                DS_MpVitalSignsCmp.Tables.Add(DT_MpVitalSignsCmp);
                return DS_MpVitalSignsCmp;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpVitalSignsCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTypeNamebyType Table: Cm.MstVitalSigns Author:LPF  2015-07-10")]
        public string GetVitalSignsTypeNamebyType(string Type)
        {
            try
            {
                string ret = null;
                ret = CmMstVitalSigns.GetTypeNameByType(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "GetVitalSignsTypeNamebyType调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetVitalSignsName Table: Cm.MstVitalSigns Author:LPF  2015-07-10")]
        public string GetVitalSignsName(string Type, string Code)
        {
            try
            {
                string ret = null;
                ret = CmMstVitalSigns.GetName(_cnCache, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "GetVitalSignsTypeNamebyType调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }
        #endregion


        #region <" 字典维护界面 集成同步 WF">

        #region <"科室 字典维护界面 集成同步 WF">
        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstDivision Author:WF  2015-07-07")]
        public DataSet GetDivision()
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = CmMstDivision.GetDivision(_cnCache);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Cm.MstDivision Author:WF  2015-07-07")]
        public bool SetDivision(int piType, string piCode, string piTypeName, string piName, string piInputCode, string piDescription, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = CmMstDivision.SetData(_cnCache, piType, piCode, piTypeName, piName, piInputCode, piDescription, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        public bool SetTmpDivisionDict(string HospitalCode, string Type, string Code, string TypeName, string Name, string InputCode, string Description, int Status, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = TmpDivisionDict.SetData(_cnCache, HospitalCode, Type, Code, TypeName, Name, InputCode, Description, Status, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetTmpDivisionDict", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }


        [WebMethod(Description = "ChangeStatus Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        public bool ChangeStatusForTmpDivision(string HospitalCode, string Type, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpDivisionDict.ChangeStatus(_cnCache, HospitalCode, Type, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpDivision", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetListByStatus Table：Tmp.DivisionDict Author:WF  2015-07-07")]
        public DataSet GetTmpDivisionByStatus(int Status)
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = TmpDivisionDict.GetListByStatus(_cnCache, Status);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpDivisionByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        public bool SetMpDivisionCmp(string HospitalCode, int Type, string Code, string HZCode, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpDivisionCmp.SetData(_cnCache, HospitalCode, Type, Code, HZCode, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "DeleteMpDivisionCmp Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        public bool DeleteMpDivisionCmp(string HospitalCode, string HZCode)
        {
            try
            {
                bool Flag = false;
                Flag = MpDivisionCmp.Delete(_cnCache, HospitalCode, HZCode);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DeleteMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetMpDivisionCmp Table：Mp.DivisionCmp Author:WF  2015-07-07")]
        public DataSet GetMpDivisionCmp()
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = MpDivisionCmp.GetMpDivisionCmp(_cnCache);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpDivisionCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetNamebyCode Table: Cm.MstDivision Author:WF  2015-07-07")]
        public string GetDivisionNamebyCode(string Code)
        {
            try
            {
                string ret = null;
                ret = CmMstDivision.GetNamebyCode(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNamebyCode ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTypeNamebyType Table: Cm.MstDivision Author:WF  2015-07-07")]
        public string GetDivisionTypeNamebyType(int Type)
        {
            try
            {
                string ret = null;
                ret = CmMstDivision.GetTypeNamebyType(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        #endregion

        #region <"诊断 字典维护界面 集成同步 WF">
        [WebMethod(Description = "SetData Table：Cm.MstDiagnosis Author:YDS  2014-12-03")]
        public bool SetDiagnosis(string Type, string Code, string TypeName, string Name, string InputCode, string Redundance, int InvalidFlag)
        {
            try
            {
                bool ret = false;
                ret = CmMstDiagnosis.SetData(_cnCache, Type, Code, TypeName, Name, InputCode, Redundance, InvalidFlag);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "获取字典表全部信息 Table：Cm.MstDiagnosis Author:YDS  2014-12-03")]
        public DataSet GetDiagnosis()
        {
            try
            {
                DataTable DT_MstDiagnosis = new DataTable();
                DataSet DS_MstDiagnosis = new DataSet();
                DT_MstDiagnosis = CmMstDiagnosis.GetDiagnosis(_cnCache);
                DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
                return DS_MstDiagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetNamebyCode Table: Cm.MstDiagnosis Author:WF  2015-07-07")]
        public string GetDiagnosisNamebyCode(string Code)
        {
            try
            {
                string ret = null;
                ret = CmMstDiagnosis.GetNamebyCode(_cnCache, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetNamebyCode ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetTypeNamebyType Table: Cm.MstDiagnosis Author:WF  2015-07-07")]
        public string GetDiagnosisTypeNamebyType(string Type)
        {
            try
            {
                string ret = null;
                ret = CmMstDiagnosis.GetTypeNamebyType(_cnCache, Type);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTypeNamebyType ", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        [WebMethod(Description = "ChangeStatus Table：Tmp.DiagnosisDict Author:WF  2015-07-07")]
        public bool ChangeStatusForTmpDiagnosis(string HospitalCode, string Code, int Status)
        {
            try
            {
                bool Flag = false;
                Flag = TmpDiagnosisDict.ChangeStatus(_cnCache, HospitalCode, Code, Status);
                return Flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "ChangeStatusForTmpDiagnosis", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw ex;
            }
        }

        [WebMethod(Description = "GetListByStatus Table：Tmp.DiagnosisDict Author:WF  2015-07-07")]
        public DataSet GetTmpDiagnosisByStatus(int Status)
        {
            try
            {
                DataTable DT_MstDiagnosis = new DataTable();
                DataSet DS_MstDiagnosis = new DataSet();
                DT_MstDiagnosis = TmpDiagnosisDict.GetListByStatus(_cnCache, Status);
                DS_MstDiagnosis.Tables.Add(DT_MstDiagnosis);
                return DS_MstDiagnosis;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetTmpDivisionByStatus", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

        [WebMethod(Description = "SetData Table：Mp.DiagnosisCmp Author:WF  2015-07-07")]
        public bool SetMpDiagnosisCmp(string HospitalCode, string HZCode, string Type, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            try
            {
                bool ret = false;
                ret = MpDiagnosisCmp.SetData(_cnCache, HospitalCode, HZCode, Type, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "SetMpDiagnosisCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
                throw (ex);
            }
        }

        [WebMethod(Description = "GetMpDiagnosisCmp Table：Mp.DiagnosisCmp Author:WF  2015-07-07")]
        public DataSet GetMpDiagnosisCmp()
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = MpDiagnosisCmp.GetMpDiagnosisCmp(_cnCache);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetMpDiagnosisCmp", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }


        #endregion

        #endregion

        [WebMethod(Description = "发送短信验证码，Author：LY 2015-11-25")]
        public string sendSMS(string mobile, string smsType, string content)//content的内容示例： 王五，2015年11月11日，海军总医院（目标姓名，预约时间，预约地点）
        {
            try
            {
                string UserId = CmMstUserDetail.GetIDByInput(_cnCache, "PhoneNo", mobile);
                if (UserId == null)
                    return "系统中不存在该手机号码";
                var Client = new RedisClient("127.0.0.1", 6379);
                var token = "849407bfab0cf4c1a998d3d6088d957b";
                var appId = "0593b3c52f7d4f8aa9f9055585407e16";
                var accountSid = "b839794e66174938828d1b8ea9c58412";
                var tplId = "";
                var param = "";
                var Jsonstring1 = "templateSMS";
                var Jsonstring2 = "appId";
                var Jsonstring3 = "param";
                var Jsonstring4 = "templateId";
                var Jsonstring5 = "to";
                var J6 = "{";
                var flag = false;

                Random rand = new Random();
                var randNum = rand.Next(100000, 1000000);
                if (smsType == "verification")
                {
                    tplId = "14452";
                    param = randNum + "," + 3;
                }
                if (smsType == "confirmtoPatient")
                {
                    tplId = "16420";
                    param = content;
                    flag = true;
                }
                if (smsType == "confirmtoHealthCoach")
                {
                    tplId = "16419";
                    param = content;
                    flag = true;
                }
                if (smsType == "Activision")
                {
                    tplId = "17510";
                    param = content;//不要用英文,
                    flag = true;
                }
                string JSONData = J6 + '"' + Jsonstring1 + '"' + ':' + '{' + '"' + Jsonstring2 + '"' + ':' + '"' + appId + '"' + ',' + '"' + Jsonstring3 + '"' + ':' + '"' + param + '"' + ',' + '"' + Jsonstring4 + '"' + ':' + '"' + tplId + '"' + ',' + '"' + Jsonstring5 + '"' + ':' + '"' + mobile + '"' + '}' + '}';

                if (mobile != "" && smsType != "")
                {
                    var Flag1 = Client.Get<string>(mobile + smsType);
                    if (Flag1 == null || flag == true)
                    {
                        Client.Set<int>(mobile + smsType, randNum);
                        Client.Expire(mobile + smsType, 3 * 60);
                        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

                        MD5 MD5 = MD5.Create();
                        var md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(accountSid + token + timestamp, "MD5").ToUpper();

                        System.Text.Encoding encode = System.Text.Encoding.ASCII;
                        byte[] bytedata = encode.GetBytes(accountSid + ":" + timestamp);
                        var authorization = Convert.ToBase64String(bytedata, 0, bytedata.Length);
                        var length1 = md5.Length;
                        var length2 = authorization.Length;

                        string Url = "https://api.ucpaas.com/2014-06-30/Accounts/" + accountSid + "/Messages/templateSMS?sig=" + md5;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                        request.Method = "POST";
                        request.Accept = "application/json";
                        request.ContentType = "application/json;charset=utf-8";
                        request.Headers.Set("Authorization", authorization);
                        //request.ContentLength = 256;
                        //request.Headers.Set("content-type", "application/json;charset=utf-8");
                        byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
                        request.ContentLength = bytes.Length;
                        request.Timeout = 10000;
                        Stream reqstream = request.GetRequestStream();
                        reqstream.Write(bytes, 0, bytes.Length);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream streamReceive = response.GetResponseStream();
                        Encoding encoding = Encoding.UTF8;
                        StreamReader streamReader = new StreamReader(streamReceive, encoding);
                        string strResult = streamReader.ReadToEnd();
                        streamReceive.Dispose();
                        streamReader.Dispose();

                        return strResult;

                    }
                    else
                    {
                        var time = Client.Ttl(mobile + smsType);
                        string codeexist = "您的邀请码已发送，请等待" + time + "后重新获取";
                        return codeexist;
                    }
                }
                return null;
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Console.WriteLine(text);
                    }
                }
                return ex.Message;
            }
        }

        [WebMethod(Description = "验证短信验证码，Author：LY 2015-11-25")]
        public int checkverification(string mobile, string smsType, string verification)
        {
            return CmMstUserDetail.checkverification(mobile, smsType, verification);
        }

        [WebMethod(Description = "获取专员负责患者在院就诊列表 Table:Ps.DoctorInfoDetail  Author:CSQ 20160114")]
        // GetPatientsMatchByDoctorId 获取专员负责患者在院就诊列表 CSQ 20160114
        public DataSet GetPatientsMatchByDoctorId(string DoctorId, string CategoryCode)
        {
            try
            {
                DataTable DT_MstDivision = new DataTable();
                DataSet DS_MstDivision = new DataSet();
                DT_MstDivision = PsDoctorInfoDetail.GetPatientsMatchByDoctorId(_cnCache, DoctorId, CategoryCode);
                DS_MstDivision.Tables.Add(DT_MstDivision);
                return DS_MstDivision;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "GetPatientsMatchByDoctorId", "WebService调用异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
                throw (ex);
            }
        }

    }

}

