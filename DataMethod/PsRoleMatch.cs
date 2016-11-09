using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsRoleMatch
    {
        //TDY 2015-05-26
        public static int SetData(DataConnection pclsCache, string PatientId, string RoleClass, string ActivationCode, string ActivatedState, string Description)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.RoleMatch.SetData(pclsCache.CacheConnectionObject, PatientId, RoleClass, ActivationCode, ActivatedState, Description);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        
        //DeleteData WF 2015-07-01
        public static int DeleteData(DataConnection pclsCache, string PatientId, string RoleClass)
        {
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Ret;

                }
                Ret = (int)Ps.RoleMatch.DeleteData(pclsCache.CacheConnectionObject, PatientId, RoleClass);
                return Ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        // GetAllRoleMatch LS 2015-03-26
        public static DataTable GetAllRoleMatch(DataConnection pclsCache, string UserID)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("RoleClass", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.RoleMatch.GetAllRoleMatch(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserID", CacheDbType.NVarChar).Value = UserID;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RoleClass"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsRoleMatch.GetAllRoleMatch", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // TDY 20150526 SetActivition
        public static int SetActivition(DataConnection pclsCache, string UserID, string RoleClass, string ActivationCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.RoleMatch.SetActivition(pclsCache.CacheConnectionObject, UserID, RoleClass, ActivationCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.SetActivition", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // TDY 20150526 GetActivatedState
        public static string GetActivatedState(DataConnection pclsCache, string UserID, string RoleClass)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.RoleMatch.GetActivatedState(pclsCache.CacheConnectionObject, UserID, RoleClass);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.GetActivatedState", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //根据角色获取未激活用户 2015-05-26 GL
        public static DataTable GetInactiveUserByRole(DataConnection pclsCache, string RoleClass)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("ActivationCode", typeof(string)));
            list.Columns.Add(new DataColumn("PhoneNo", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.RoleMatch.GetInactiveUserByRole(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("RoleClass", CacheDbType.NVarChar).Value = RoleClass;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //string a = cdr["ActivationCode"].ToString();
                    list.Rows.Add(cdr["UserName"].ToString(), cdr["UserId"].ToString(), cdr["ActivationCode"].ToString(), cdr["PhoneNo"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.GetInactiveUserByRole", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //将邀请码写入数据库 GL 2015-05-28 
        public static int SetActivationCode(DataConnection pclsCache, string UserId, string RoleClass, string SetActivationCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.RoleMatch.SetActivationCode(pclsCache.CacheConnectionObject, UserId, RoleClass, SetActivationCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.SetActivationCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // TDY 20150603 GetActivateCode
        public static string GetActivateCode(DataConnection pclsCache, string UserID, string RoleClass)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.RoleMatch.GetActivateCode(pclsCache.CacheConnectionObject, UserID, RoleClass);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.GetActivateCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //根据角色获取激活用户 2015-06-03 ZC
        public static DataTable GetActiveUserByRole(DataConnection pclsCache, string RoleClass)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("UserId", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.RoleMatch.GetActiveUserByRole(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("RoleClass", CacheDbType.NVarChar).Value = RoleClass;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["UserName"].ToString(), cdr["UserId"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.RoleMatch.GetInactiveUserByRole", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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