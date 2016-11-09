using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;


namespace WebService.DataMethod
{
    public class CmMstRole
    {
        //SetData ZAM 2014-11-26
        public static bool SetData(DataConnection pclsCache, string Code, string Name, int SortNo, string Redundance, int InvalidFlag)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstRole.SetData(pclsCache.CacheConnectionObject, Code, Name, SortNo, Redundance, InvalidFlag);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstRole.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        // GetRole ZAM 2014-12-02
        public static DataTable GetRole(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
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
                cmd = Cm.MstRole.GetRole(pclsCache.CacheConnectionObject);
                //cmd.Parameters.Add("Category", CacheDbType.NVarChar).Value = Category;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int InvalidFlag = 0;        //value check ZAM 2014-1-7
                    if (cdr["InvalidFlag"].ToString() != "")
                    {
                        InvalidFlag = Convert.ToInt32(cdr["InvalidFlag"]);
                    }
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["SortNo"], cdr["Redundance"].ToString(), InvalidFlag);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstRole.GetRole", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData ZAM 2014-11-26
        public static int DeleteData(DataConnection pclsCache, string Code)
        {
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Ret;

                }
                Ret = (int)Cm.MstRole.DeleteData(pclsCache.CacheConnectionObject, Code);
                return Ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstRole.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetRoleNameByCode ZAM 2014-11-26
        public static string GetRoleNameByCode(DataConnection pclsCache, string Code)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstRole.GetRoleNameByCode(pclsCache.CacheConnectionObject, Code);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstRole.GetRoleNameByCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        // GetRoleList ZYF 2014-12-03
        public static DataTable GetRoleList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));

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
                cmd = Cm.MstRole.GetRole(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstRole.GetRoleList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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