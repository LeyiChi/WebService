using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstAuthorityDetail
    {
        //SetData TDY 2014-12-1
        public static bool SetData(DataConnection pclsCache, string Authority, string piCode, string piName, int piSortNo, string piRedundance, int piInvalidFlag)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstAuthorityDetail.SetData(pclsCache.CacheConnectionObject, Authority, piCode, piName, piSortNo, piRedundance, piInvalidFlag);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // DeleteData TDY 2014-12-1
        public static int DeleteData(DataConnection pclsCache, string Authority, string Code)
        {
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Ret;
                }
                Ret = (int)Cm.MstAuthorityDetail.DeleteData(pclsCache.CacheConnectionObject, Authority, Code);
                return Ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetSubAuthorityName 根据小分类编码输出小分类名称 TDY 2014-12-1
        public static string GetSubAuthorityName(DataConnection pclsCache, string AuthorityCode, string SubAuthorityCode)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string SubAuthorityName = Cm.MstAuthorityDetail.GetSubAuthorityName(pclsCache.CacheConnectionObject, AuthorityCode, SubAuthorityCode);
                return SubAuthorityName;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.GetSubAuthorityName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetSubAuthorityList 根据大分类Code输出所有小分类编码及名称 TDY 2014-12-1
        public static DataTable GetSubAuthorityList(DataConnection pclsCache, string AuthorityCode)
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
                cmd = Cm.MstAuthorityDetail.GetSubAuthorityList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("AuthorityCode", CacheDbType.NVarChar).Value = AuthorityCode;
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
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.GetSubAuthorityList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetAuthorityDetail  TDY 2014-12-1
        public static DataTable GetAuthorityDetail(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            list.Columns.Add(new DataColumn("Authority", typeof(string)));
            list.Columns.Add(new DataColumn("AuthorityName", typeof(string))); //ZYF  2015-1-21

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
                cmd = Cm.MstAuthorityDetail.GetAuthorityDetail(pclsCache.CacheConnectionObject);
                //cmd.Parameters.Add("AuthorityCode", CacheDbType.NVarChar).Value = AuthorityCode;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["SortNo"].ToString(), cdr["Redundance"].ToString(), cdr["InvalidFlag"].ToString(), cdr["Authority"].ToString(), cdr["AuthorityName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.GetAuthorityDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetSubAuthority 根据大分类Code输出所有小分类信息 TDY 2014-12-1
        public static DataTable GetSubAuthority(DataConnection pclsCache, string Authority)
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
                cmd = Cm.MstAuthorityDetail.GetSubAuthority(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Authority", CacheDbType.NVarChar).Value = Authority;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["SortNo"].ToString(), cdr["Redundance"].ToString(), cdr["InvalidFlag"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstAuthorityDetail.GetSubAuthority", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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