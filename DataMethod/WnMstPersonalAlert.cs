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
    public class WnMstPersonalAlert
    {
        //SetData GL 2014-12-01 
        public static bool SetData(DataConnection pclsCache, string UserId, string AlertItemCode, int SortNo, decimal Min, decimal Max, string Units, int StartDate, int EndDate, string Remarks, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Wn.MstPersonalAlert.SetData(pclsCache.CacheConnectionObject, UserId, AlertItemCode, SortNo, Min, Max, Units, StartDate, EndDate, Remarks, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstPersonalAlert.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetWnMstPersonalAlert GL 2014-12-01 获取最新一条警告
        public static CacheSysList GetWnMstPersonalAlert(DataConnection pclsCache, string UserId, string AlertItemCode, int CheckDate)
        {
            try
            {
                CacheSysList AlertList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                AlertList = Wn.MstPersonalAlert.GetWnMstPersonalAlert(pclsCache.CacheConnectionObject, UserId, AlertItemCode, CheckDate);
                return AlertList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstPersonalAlert.GetWnMstPersonalAlert", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //GetMaxItemSeq LS 2014-12-1
        public static int GetMaxSortNo(DataConnection pclsCache, string UserId, string AlertItemCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Wn.MstPersonalAlert.GetMaxSortNo(pclsCache.CacheConnectionObject, UserId, AlertItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnMstPersonalAlert.GetMaxSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

    }
}