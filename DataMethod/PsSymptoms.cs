using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsSymptoms
    {
        #region <" ZAM 2015-1-20 ">
        public static int SetData(DataConnection pclsCache, string UserId, string VisitId, int SynptomsNo, string SymptomsType, string SymptomsCode, string Description, int RecordDate, int RecordTime, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Symptoms.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, SynptomsNo, SymptomsType, SymptomsCode, Description, RecordDate, RecordTime, piUserId, piTerminalName, piTerminalIP, piDeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //CSQ 2015-1-21
        public static DataTable GetSymptomsList(DataConnection pclsCache, string UserId, string VisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SynptomsNo", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsType", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsCode", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsName", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("RecordDate", typeof(string)));
            list.Columns.Add(new DataColumn("RecordTime", typeof(string)));
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
                cmd = Ps.Symptoms.GetSymptomsList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("VisitId", CacheDbType.NVarChar).Value = VisitId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SynptomsNo"].ToString(), cdr["SymptomsType"].ToString(), cdr["SymptomsTypeName"].ToString(), cdr["SymptomsCode"].ToString(), cdr["SymptomsName"].ToString(), cdr["Description"].ToString(), cdr["RecordDate"].ToString(), cdr["RecordTime"].ToString(), cdr["Creator"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.GetSymptomsList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static int GetMaxSortNo(DataConnection pclsCache, string UserId, string VisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Symptoms.GetMaxSortNo(pclsCache.CacheConnectionObject, UserId, VisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.GetMaxSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int DeleteData(DataConnection pclsCache, string UserId, string VisitId, int SynptomsNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Symptoms.DeleteData(pclsCache.CacheConnectionObject, UserId, VisitId, SynptomsNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int DeleteAll(DataConnection pclsCache, string UserId, string VisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Symptoms.DeleteAll(pclsCache.CacheConnectionObject, UserId, VisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //WF 2015-1-21
        public static DataTable GetSymptomsListByPId(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SynptomsNo", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsType", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsCode", typeof(string)));
            list.Columns.Add(new DataColumn("SymptomsName", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("RecordDate", typeof(string)));
            list.Columns.Add(new DataColumn("RecordTime", typeof(string)));
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
                cmd = Ps.Symptoms.GetSymptomsListByPId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SynptomsNo"].ToString(), cdr["SymptomsType"].ToString(), cdr["SymptomsTypeName"].ToString(), cdr["SymptomsCode"].ToString(), cdr["SymptomsName"].ToString(), cdr["Description"].ToString(), cdr["RecordDate"].ToString(), cdr["RecordTime"].ToString(), cdr["ReInUserId"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Symptoms.GetSymptomsListByPId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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