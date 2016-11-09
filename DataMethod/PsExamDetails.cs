using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsExamDetails
    {
        //SetData ZC 2014-12-2
        public static bool SetData(DataConnection pclsCache, string UserId, string VisitId, int SortNo, string Code, string Value, int IsAbnormal, string Unit, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    return IsSaved;
                }
                string Exam = UserId + "||" + VisitId + "||" + SortNo.ToString();
                int flag = (int)Ps.ExamDetails.SetData(pclsCache.CacheConnectionObject, Exam, Code, Value, IsAbnormal, Unit, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsExamDetails.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetExamDetails 获取患者某次检查的所有详细信息 ZC 2014-12-2
        //CSQ 2015-1-21
        public static DataTable GetExamDetails(DataConnection pclsCache, string UserId, string VisitId, string SortNo, string ItemCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("IsAbnormalCode", typeof(int)));
            list.Columns.Add(new DataColumn("IsAbnormal", typeof(string)));
            list.Columns.Add(new DataColumn("UnitCode", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));
            list.Columns.Add(new DataColumn("Creator", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.ExamDetails.GetExamDetails(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("VisitId", CacheDbType.NVarChar).Value = VisitId;
                cmd.Parameters.Add("SortNo", CacheDbType.Int).Value = SortNo;
                cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;
                cdr = cmd.ExecuteReader();

                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["Value"].ToString(),
                        Convert.ToInt32(cdr["IsAbnormalCode"].ToString()), cdr["IsAbnormal"].ToString(),
                        cdr["UnitCode"].ToString(), cdr["Unit"].ToString(), cdr["Creator"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsExamDetails.GetExamDetails", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData ZC 2014-12-2
        public static int DeleteData(DataConnection pclsCache, string UserId, string VisitId,int SortNo, string Code)
        {
            int flag = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return flag;
                }
                flag = (int)Ps.ExamDetails.DeleteData(pclsCache.CacheConnectionObject, UserId, VisitId, SortNo, Code);
                return flag;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsExamDetails.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return flag;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}