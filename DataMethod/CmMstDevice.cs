using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstDevice
    {
        //SetData ZC 2015-06-07
        public static bool SetData(DataConnection pclsCache, string Device, string Address, string Port, string VersionNo, string Description, string Redundance)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    return IsSaved;

                }
                int flag = (int)Cm.MstDevice.SetData(pclsCache.CacheConnectionObject, Device, Address, Port, VersionNo, Description, Redundance);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstDevice.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetIP ZC 2015-06-07
        public static string GetIP(DataConnection pclsCache, string Device)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                string IP = Cm.MstDevice.GetIP(pclsCache.CacheConnectionObject, Device);
                return IP;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstDevice.GetIP", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        // GetPort ZC 2015-06-07
        public static string GetPort(DataConnection pclsCache, string Device)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                string Port = Cm.MstDevice.GetPort(pclsCache.CacheConnectionObject, Device);
                return Port;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstDevice.GetPort", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }
    }
}