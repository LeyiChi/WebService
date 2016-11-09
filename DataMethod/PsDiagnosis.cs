using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsDiagnosis
    {
        #region <" ZAM 2015-1-20 ">
        public static int SetData(DataConnection pclsCache, string UserId, string VisitId, int DiagnosisType, string DiagnosisNo, string Type, string DiagnosisCode, string Description, string RecordDate, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }
                DateTime Record_Date = Convert.ToDateTime(RecordDate);
                ret = (int)Ps.Diagnosis.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, DiagnosisType, DiagnosisNo, Type, DiagnosisCode, Description, Record_Date, piUserId, piTerminalName, piTerminalIP, piDeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Diagnosis.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int Delete(DataConnection pclsCache, string UserId, string VisitId, int DiagnosisType, string DiagnosisNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Diagnosis.Delete(pclsCache.CacheConnectionObject, UserId, VisitId, DiagnosisType, DiagnosisNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Diagnosis.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 获取某诊断类型的最新诊断记录 ZAM 2015-1-26
        public static string GetDiagnosisByType(DataConnection pclsCache, string UserId, int DiagnosisType)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.Diagnosis.GetDiagnosisByType(pclsCache.CacheConnectionObject, UserId, DiagnosisType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Diagnosis.GetDiagnosisByType", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //LS 2015-1-23
        public static DataTable GetDiagnosisInfo(DataConnection pclsCache, string UserId, string VisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("DiagnosisType", typeof(string)));
            list.Columns.Add(new DataColumn("DiagnosisTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("DiagnosisNo", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("DiagnosisCode", typeof(string)));
            list.Columns.Add(new DataColumn("DiagnosisName", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("RecordDate", typeof(string)));
            list.Columns.Add(new DataColumn("RecordDateShow", typeof(string)));
            list.Columns.Add(new DataColumn("Creator", typeof(string)));   //20150124 CSQ
            list.Columns.Add(new DataColumn("RecordDateCom", typeof(string)));   //20150709 LS

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Diagnosis.GetDiagnosisInfo(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("VisitId", CacheDbType.NVarChar).Value = VisitId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    string RecordDateShow = "";
                    string str1 = cdr["RecordDate"].ToString();
                    if (str1 != "")
                    {
                        RecordDateShow = str1.Substring(0, 4) + "-" + str1.Substring(4, 2) + "-" + str1.Substring(6, 2);
                    }
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["DiagnosisType"].ToString(), cdr["DiagnosisTypeName"].ToString(), cdr["DiagnosisNo"].ToString(), cdr["Type"].ToString(), cdr["TypeName"].ToString(), cdr["DiagnosisCode"].ToString(), cdr["DiagnosisName"].ToString(), cdr["Description"].ToString(), cdr["RecordDate"].ToString(), RecordDateShow, cdr["Creator"].ToString(), Convert.ToDateTime(cdr["RecordDate"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Diagnosis.GetDiagnosisInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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
        #endregion
    }
}