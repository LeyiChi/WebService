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
    public class WnMstBasicAlert
    {
        //SetData YDS 2014-12-01
        public static bool SetData(DataConnection pclsCache, string AlertItemCode, string AlertItemName, decimal Min, decimal Max, string Units, string Remarks, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Wn.MstBasicAlert.SetData(pclsCache.CacheConnectionObject, AlertItemCode, AlertItemName, Min, Max, Units, Remarks, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstBasicAlert.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //DeleteData YDS 2014-12-01
        public static int DeleteData(DataConnection pclsCache, string AlertItemCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Wn.MstBasicAlert.DeleteData(pclsCache.CacheConnectionObject, AlertItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "删除失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstBasicAlert.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetBasicAlert 获得字典表全部信息 YDS 2014-12-09
        public static DataTable GetBasicAlert(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("AlertItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("AlertItemName", typeof(string)));
            list.Columns.Add(new DataColumn("Min", typeof(decimal)));
            list.Columns.Add(new DataColumn("Max", typeof(decimal)));
            list.Columns.Add(new DataColumn("Units", typeof(string)));
            list.Columns.Add(new DataColumn("Remarks", typeof(string)));
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
                cmd = Wn.MstBasicAlert.GetBasicAlert(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    decimal Min = 0;
                    decimal Max = 0;
                    if (cdr["Min"].ToString() != "")
                    {
                        Min = Convert.ToDecimal(cdr["Min"]);
                    }
                    if (cdr["Max"].ToString() != "")
                    {
                        Max = Convert.ToDecimal(cdr["Max"]);
                    }
                    list.Rows.Add(cdr["AlertItemCode"].ToString(), cdr["AlertItemName"].ToString(), Min, Max, cdr["Units"].ToString(), cdr["Remarks"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstBasicAlert.GetBasicAlert", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetWnMstBasicAlert YDS 2014-12-01
        public static CacheSysList GetWnMstBasicAlert(DataConnection pclsCache, string AlertItemCode)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Wn.MstBasicAlert.GetWnMstBasicAlert(pclsCache.CacheConnectionObject, AlertItemCode);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstBasicAlert.GetWnMstBasicAlert", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }
    }
}