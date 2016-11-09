using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmRole2Authority
    {
        //SetData TDY 2014-12-1
        public static bool SetData(DataConnection pclsCache, string piRoleCode, string piAuthorityCode, string piSubAuthorityCode, string piRedundance, int piInvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.Role2Authority.SetData(pclsCache.CacheConnectionObject, piRoleCode, piAuthorityCode, piSubAuthorityCode, piRedundance, piInvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmRole2Authority.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // DeleteData TDY 2014-12-1
        public static int DeleteData(DataConnection pclsCache, string RoleCode, string AuthorityCode, string SubAuthorityCode)
        {
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Ret;
                }
                Ret = (int)Cm.Role2Authority.DeleteData(pclsCache.CacheConnectionObject, RoleCode, AuthorityCode, SubAuthorityCode);
                return Ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmRole2Authority.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetRoleAuthorityList  输入角色RoleCode从Cm.Role2Authority查询输出该角色拥有的所有权限 TDY 2014-12-1
        public static DataTable GetRoleAuthorityList(DataConnection pclsCache, string RoleCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("AuthorityCode", typeof(string)));
            list.Columns.Add(new DataColumn("AuthorityName", typeof(string)));
            list.Columns.Add(new DataColumn("SubAuthorityCode", typeof(string)));
            list.Columns.Add(new DataColumn("SubAuthorityName", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Cm.Role2Authority.GetRoleAuthorityList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("RoleCode", CacheDbType.NVarChar).Value = RoleCode;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["AuthorityCode"].ToString(), cdr["AuthorityName"].ToString(), cdr["SubAuthorityCode"].ToString(), cdr["SubAuthorityName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmRole2Authority.GetRoleAuthorityList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetRole2Authority  TDY 2014-12-1
        public static DataTable GetRole2Authority(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("RoleCode", typeof(string)));
            list.Columns.Add(new DataColumn("AuthorityCode", typeof(string)));
            list.Columns.Add(new DataColumn("SubAuthorityCode", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Cm.Role2Authority.GetRole2Authority(pclsCache.CacheConnectionObject);
                //cmd.Parameters.Add("RoleCode", CacheDbType.NVarChar).Value = RoleCode;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RoleCode"].ToString(), cdr["AuthorityCode"].ToString(), cdr["SubAuthorityCode"].ToString(), cdr["Redundance"].ToString(), cdr["InvalidFlag"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmRole2Authority.GetRole2Authority", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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