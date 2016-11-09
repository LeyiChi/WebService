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
    public class PsComplianceDetail
    {
        // 施宇帆 2015-04-08 插入某条数据
        public static int SetData(DataConnection pclsCache, string Parent, string Id, int Status, string CoUserId, string CoTerminalName, string CoTerminalIP, int CoDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.ComplianceDetail.SetData(pclsCache.CacheConnectionObject, Parent, Id, Status, CoUserId, CoTerminalName, CoTerminalIP, CoDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.ComplianceDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 施宇帆 2015-04-08 获取某计划下某任务的某天依从状态
        public static int GetComplianceDetailByDay(DataConnection pclsCache, string PatientId, int Date, string PlanNo, string Id)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.ComplianceDetail.GetComplianceDetailByDay(pclsCache.CacheConnectionObject, PatientId, Date, PlanNo, Id);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.ComplianceDetail.GetComplianceDetailByDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 施宇帆 2015-04-08 获取某计划下某任务的某段时间(包括端点)的依从状态列表
        public static DataTable GetComplianceDetailByPeriod(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate, int Id)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
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
                cmd = Ps.ComplianceDetail.GetComplianceDetailByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.Int).Value = EndDate;
                cmd.Parameters.Add("Id", CacheDbType.Int).Value = Id;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Date"].ToString(), cdr["Status"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.ComplianceDetail.GetComplianceDetailByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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