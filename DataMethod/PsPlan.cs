using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
using InterSystems.Data.CacheTypes;
using WebService.DataClass;

namespace WebService.DataMethod
{
    public class PsPlan
    {
        //CSQ 20150407
        public static int SetData(DataConnection pclsCache, string PlanNo, string PatientId, int StartDate, int EndDate, string Module, int Status, string DoctorId, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Plan.SetData(pclsCache.CacheConnectionObject, PlanNo, PatientId, StartDate, EndDate, Module, Status, DoctorId, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //ZAM 20150422 获取健康专员负责的所有患者列表
        public static DataTable GetPatientsPlanByDoctorId(DataConnection pclsCache, string DoctorId, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("PatientId", typeof(string)));
            list.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(string)));
            list.Columns.Add(new DataColumn("EndDate", typeof(string)));
            list.Columns.Add(new DataColumn("TotalDays", typeof(string)));
            list.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Plan.GetPatientsPlanByDoctorId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //DataCheck ZAM 2015-4-16
                    if (cdr["PatientId"].ToString() == string.Empty)
                    {
                        continue;
                    }

                    list.Rows.Add(cdr["PatientId"].ToString(), cdr["PlanNo"].ToString(), cdr["StartDate"].ToString(), cdr["EndDate"].ToString(), cdr["TotalDays"].ToString(), cdr["RemainingDays"].ToString(), cdr["Status"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.GetPatientsPlanByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if ((cdr != null))
                {
                    cdr.Close();
                    cdr.Dispose(true);
                    cdr = null;
                }
                if ((cmd != null))
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    cmd = null;
                }
                pclsCache.DisConnect();
            }
        }

        //CSQ 20150407
        public static int PlanStart(DataConnection pclsCache, string PlanNo, int Status, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Plan.PlanStart(pclsCache.CacheConnectionObject, PlanNo, Status, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.PlanStart", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // ZAM 2015-4-24 获取健康专员负责的所有患者最新结束(status = 4)计划列表
        public static DataTable GetOverDuePlanByDoctorId(DataConnection pclsCache, string DoctorId, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("PatientId", typeof(string)));
            list.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(string)));
            list.Columns.Add(new DataColumn("EndDate", typeof(string)));
            list.Columns.Add(new DataColumn("TotalDays", typeof(string)));
            list.Columns.Add(new DataColumn("RemainingDays", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Plan.GetOverDuePlanByDoctorId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["PatientId"].ToString(), cdr["PlanNo"].ToString(), cdr["StartDate"].ToString(), cdr["EndDate"].ToString(), cdr["TotalDays"].ToString(), cdr["RemainingDays"].ToString(), cdr["Status"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.GetOverDuePlanByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if ((cdr != null))
                {
                    cdr.Close();
                    cdr.Dispose(true);
                    cdr = null;
                }
                if ((cmd != null))
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    cmd = null;
                }
                pclsCache.DisConnect();
            }
        }


       //以下是任务完成情况用到的函数

        //GetPlanInfo 获取某计划的相关信息 LS 2015-03-30
        public static CacheSysList GetPlanInfo(DataConnection pclsCache, string PlanNo)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Plan.GetPlanInfo(pclsCache.CacheConnectionObject, PlanNo);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetPlanInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetExecutingPlan 获取正在执行的计划 LS 2015-03-30
        public static CacheSysList GetExecutingPlan(DataConnection pclsCache, string PatientId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Plan.GetExecutingPlan(pclsCache.CacheConnectionObject, PatientId);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetExecutingPlan", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetExecutingPlanByM  获取某模块正在执行的计划 LS 2015-03-30
        public static CacheSysList GetExecutingPlanByM(DataConnection pclsCache, string PatientId, string Module)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Plan.GetExecutingPlanByM(pclsCache.CacheConnectionObject, PatientId, Module);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetExecutingPlan", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetEndingPlan 获取某模块已经结束的计划 LS 2015-06-24
        public static DataTable GetEndingPlan(DataConnection pclsCache, string PatientId, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("PlanNo", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(int)));
            list.Columns.Add(new DataColumn("EndDate", typeof(int)));
            //list.Columns.Add(new DataColumn("Module", typeof(string)));
            //list.Columns.Add(new DataColumn("Status", typeof(string)));
            // list.Columns.Add(new DataColumn("DoctorId", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Plan.GetPlanList4ByM(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["PlanNo"].ToString(), Convert.ToInt32(cdr["StartDate"]), Convert.ToInt32(cdr["EndDate"]));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.GetEndingPlan", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                if ((cdr != null))
                {
                    cdr.Close();
                    cdr.Dispose(true);
                    cdr = null;
                }
                if ((cmd != null))
                {
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    cmd = null;
                }
                pclsCache.DisConnect();
            }
        }

        //GetPlanList34ByM 获取某模块患者的正在执行的和结束的计划列表 LS 2015-03-30
        public static List<PlanDeatil> GetPlanList34ByM(DataConnection pclsCache, string PatientId, string Module)
        {
            List<PlanDeatil> result = new List<PlanDeatil>();
            try
            {
                InterSystems.Data.CacheTypes.CacheSysList list = null;
                list = PsPlan.GetExecutingPlanByM(pclsCache, PatientId, Module);

                if (list != null)
                {
                    PlanDeatil PlanDeatil = new PlanDeatil();
                    PlanDeatil.PlanNo = list[0].ToString();
                    PlanDeatil.StartDate = Convert.ToInt32(list[2]);
                    PlanDeatil.EndDate = Convert.ToInt32(list[3]);
                    string temp = PlanDeatil.StartDate.ToString().Substring(0, 4) + "/" + PlanDeatil.StartDate.ToString().Substring(4, 2) + "/" + PlanDeatil.StartDate.ToString().Substring(6, 2);
                    string temp1 = PlanDeatil.EndDate.ToString().Substring(0, 4) + "/" + PlanDeatil.EndDate.ToString().Substring(4, 2) + "/" + PlanDeatil.EndDate.ToString().Substring(6, 2);
                    PlanDeatil.PlanName = "当前计划：" + temp + "-" + temp1;
                    //PlanDeatil.Module = list[4].ToString();
                    //PlanDeatil.Status = list[5].ToString();
                    //PlanDeatil.DoctorId = list[6].ToString();
                    result.Add(PlanDeatil);
                }
                else
                {
                    PlanDeatil PlanDeatil = new PlanDeatil();
                    PlanDeatil.PlanNo = "";
                    PlanDeatil.PlanName = "当前计划";
                    result.Add(PlanDeatil);
                }


                DataTable endingPlanList = new DataTable();
                endingPlanList = GetEndingPlan(pclsCache, PatientId, Module);
                for (int i = 0; i < endingPlanList.Rows.Count; i++)
                {
                    PlanDeatil PlanDeatil = new PlanDeatil();
                    PlanDeatil.PlanNo = endingPlanList.Rows[i]["PlanNo"].ToString();
                    PlanDeatil.StartDate = Convert.ToInt32(endingPlanList.Rows[i]["StartDate"]);
                    PlanDeatil.EndDate = Convert.ToInt32(endingPlanList.Rows[i]["EndDate"]);
                    string temp = PlanDeatil.StartDate.ToString().Substring(0, 4) + "/" + PlanDeatil.StartDate.ToString().Substring(4, 2) + "/" + PlanDeatil.StartDate.ToString().Substring(6, 2);
                    string temp1 = PlanDeatil.EndDate.ToString().Substring(0, 4) + "/" + PlanDeatil.EndDate.ToString().Substring(4, 2) + "/" + PlanDeatil.EndDate.ToString().Substring(6, 2);
                    PlanDeatil.PlanName = "往期：" + temp + "-" + temp1;
                    result.Add(PlanDeatil);
                }

                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetPlanList34ByM", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
            }
        }

        //GetProgressRate 获取某计划的进度和剩余天数 LS 2015-03-30
        public static CacheSysList GetProgressRate(DataConnection pclsCache, string PlanNo)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Plan.GetProgressRate(pclsCache.CacheConnectionObject, PlanNo);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetProgressRate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetWeekPeriod 获取某计划的相关信息 LS 2015-03-30
        public static CacheSysList GetWeekPeriod(DataConnection pclsCache, int PlanStartDate)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Plan.GetWeekPeriod(pclsCache.CacheConnectionObject, PlanStartDate);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPlan.GetWeekPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //CSQ 20150407
        public static int UpdateStatus(DataConnection pclsCache, string PlanNo, int Status, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Plan.UpdateData(pclsCache.CacheConnectionObject, PlanNo, Status, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Plan.UpdateData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}