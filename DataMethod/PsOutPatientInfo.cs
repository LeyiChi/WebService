using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsOutPatientInfo
    {
        #region <" ZAM 2015-1-20 ">
        public static bool SetData(DataConnection pclsCache, string UserId, string VisitId, DateTime ClinicDate, string HospitalCode, string Department, string Doctor, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return IsSaved;
                }

                ret = (int)Ps.OutPatientInfo.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, ClinicDate, HospitalCode, Department, Doctor, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (ret == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static DataTable GetInfobyId(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("ClinicDate", typeof(string)));
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
                cmd = Ps.OutPatientInfo.GetInfobyId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["ClinicDate"].ToString(), cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Department"].ToString(), cdr["DepartmentName"].ToString(), cdr["Doctor"].ToString(), cdr["Creator"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.GetInfobyId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static DataTable GetInfobyDate(DataConnection pclsCache, string UserId, DateTime ClinicDate, int Num)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("ClinicDate", typeof(string)));
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
                cmd = Ps.OutPatientInfo.GetInfobyDate(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("ClinicDate", CacheDbType.DateTime).Value = ClinicDate;
                cmd.Parameters.Add("Num", CacheDbType.Int).Value = Num;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["ClinicDate"].ToString(), cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Department"].ToString(), cdr["DepartmentName"].ToString(), cdr["Doctor"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.GetInfobyDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return Ret;
                }
                Ret = (int)Ps.OutPatientInfo.DeleteAll(pclsCache.CacheConnectionObject, UserId, VisitId);
                return Ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static string GetMaxVisitId(DataConnection pclsCache, string UserId)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.OutPatientInfo.GetMaxVisitId(pclsCache.CacheConnectionObject, UserId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.GetMaxVisitId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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