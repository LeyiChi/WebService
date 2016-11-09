using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
using InterSystems.Data.CacheTypes;

namespace WebService.DataMethod
{
    public class CmMstIndicator
    {
        //SetData YDS 2014-12-01
        public static bool SetData(DataConnection pclsCache, string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstIndicator.SetData(pclsCache.CacheConnectionObject, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstIndicator.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetIndicator 获得字典表全部信息 YDS 2014-12-01
        public static DataTable GetIndicator(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("InputCode", typeof(string)));
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
                cmd = Cm.MstIndicator.GetIndicator(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int SortNo = 0;
                    int InvalidFlag = 0;
                    if (cdr["SortNo"].ToString() != "")
                    {
                        SortNo = Convert.ToInt32(cdr["SortNo"]);
                    }
                    if (cdr["InvalidFlag"].ToString() != "")
                    {
                        InvalidFlag = Convert.ToInt32(cdr["InvalidFlag"]);
                    }
                    list.Rows.Add(cdr["Type"].ToString(), cdr["Code"].ToString(), cdr["TypeName"].ToString(), cdr["Name"].ToString(), cdr["InputCode"].ToString(), SortNo, cdr["Redundance"].ToString(), InvalidFlag);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstIndicator.GetIndicator", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData YDS 2014-12-01
        public static int DeleteData(DataConnection pclsCache, string Type, string Code)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstIndicator.DeleteData(pclsCache.CacheConnectionObject, Type, Code);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "删除失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstIndicator.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetMaxCode YDS 2014-12-22
        public static string GetMaxCode(DataConnection pclsCache, string Type)
        {
            string ret = string.Empty;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = Cm.MstIndicator.GetMaxCode(pclsCache.CacheConnectionObject, Type);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstIndicator.GetMaxCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}