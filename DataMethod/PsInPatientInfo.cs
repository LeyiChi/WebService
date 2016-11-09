using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsInPatientInfo
    {
        #region <" ZAM 2015-1-19 ">
        public static bool SetData(DataConnection pclsCache, string UserId, string VisitId, int SortNo, DateTime AdmissionDate, DateTime DischargeDate, string HospitalCode, string Department, string Doctor, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.InPatientInfo.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, SortNo, AdmissionDate, DischargeDate, HospitalCode, Department, Doctor, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.InPatientInfo.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        //CSQ 2015-1-21
        public static DataTable GetInfobyId(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(string)));
            list.Columns.Add(new DataColumn("AdmissionDate", typeof(string)));
            list.Columns.Add(new DataColumn("DischargeDate", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("Department", typeof(string)));
            list.Columns.Add(new DataColumn("DepartmentName", typeof(string)));
            list.Columns.Add(new DataColumn("Doctor", typeof(string)));
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
                cmd = Ps.InPatientInfo.GetInfobyId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SortNo"].ToString(), cdr["AdmissionDate"].ToString(), cdr["DischargeDate"].ToString(), cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Department"].ToString(), cdr["DepartmentName"].ToString(), cdr["Doctor"].ToString(), cdr["Creator"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsInPatientInfo.GetInfobyId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static DataTable GetInfobyDate(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(string)));
            list.Columns.Add(new DataColumn("AdmissionDate", typeof(string)));
            list.Columns.Add(new DataColumn("DischargeDate", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("Department", typeof(string)));
            list.Columns.Add(new DataColumn("DepartmentName", typeof(string)));
            list.Columns.Add(new DataColumn("Doctor", typeof(string)));


            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.InPatientInfo.GetInfobyDate(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SortNo"].ToString(), cdr["AdmissionDate"].ToString(), cdr["DischargeDate"].ToString(), cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Department"].ToString(), cdr["DepartmentName"].ToString(), cdr["Doctor"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsInPatientInfo.GetInfobyId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static int DeleteAll(DataConnection pclsCache, string UserId, string VisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }
                ret = (int)Ps.InPatientInfo.DeleteAll(pclsCache.CacheConnectionObject, UserId, VisitId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.InPatientInfo.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static string GetMaxVisitId(DataConnection pclsCache, string UserId)
        {
            string ret = string.Empty;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }
                ret = Ps.InPatientInfo.GetMaxVisitId(pclsCache.CacheConnectionObject, UserId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.InPatientInfo.GetMaxVisitID", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }
        #endregion
    }
}