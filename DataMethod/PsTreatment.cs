using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsTreatment
    {
        //DeleteData ZC 2014-12-2
        public static int DeleteData(DataConnection pclsCache, string UserId, int SortNo)
        {
            int flag = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return flag;
                }
                flag = (int)Ps.Treatment.DeleteData(pclsCache.CacheConnectionObject, UserId, SortNo);
                return flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsTreatment.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return flag;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetNextSortNo 获取最大序号，并加1 ZC 2014-12-2
        public static int GetNextSortNo(DataConnection pclsCache, string UserId)
        {
            int sortNo = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return sortNo;
                }
                sortNo = (int)Ps.Treatment.GetNextSortNo(pclsCache.CacheConnectionObject, UserId);
                return sortNo;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsTreatment.GetNextSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return sortNo;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int SetData(DataConnection pclsCache, string UserId, int SortNo, int TreatmentGoal, int TreatmentAction, int Group, string TreatmentPlan, string Description, DateTime TreatTime, string Duration, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Treatment.SetData(pclsCache.CacheConnectionObject, UserId, SortNo, TreatmentGoal, TreatmentAction, Group, TreatmentPlan, Description, TreatTime, Duration, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Treatment.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //WF 2015-1-22
        public static DataTable GetTreatmentList(DataConnection pclsCache, string piUserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("SortNo", typeof(string)));
            list.Columns.Add(new DataColumn("TreatmentGoal", typeof(string)));
            list.Columns.Add(new DataColumn("TreatmentGoalName", typeof(string)));
            list.Columns.Add(new DataColumn("TreatmentAction", typeof(string)));
            list.Columns.Add(new DataColumn("TreatmentActionName", typeof(string)));
            list.Columns.Add(new DataColumn("Group", typeof(string)));
            list.Columns.Add(new DataColumn("GroupName", typeof(string)));
            list.Columns.Add(new DataColumn("TreatmentPlan", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("TreatTime", typeof(string)));
            list.Columns.Add(new DataColumn("Duration", typeof(string)));
            list.Columns.Add(new DataColumn("DurationName", typeof(string)));
            list.Columns.Add(new DataColumn("ReInUserId", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Treatment.GetTreatmentList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["SortNo"].ToString(), cdr["TreatmentGoal"].ToString(), cdr["TreatmentGoalName"].ToString(), cdr["TreatmentAction"].ToString(), cdr["TreatmentActionName"].ToString(), cdr["Group"].ToString(), cdr["GroupName"].ToString(), cdr["TreatmentPlan"].ToString(), cdr["Description"].ToString(), cdr["TreatTime"].ToString(), cdr["Duration"].ToString(), cdr["DurationName"].ToString(), cdr["ReInUserId"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Treatment.GetTreatmentList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

    }
}