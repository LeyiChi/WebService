using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsUserIdMatch
    {
        //SetData CSQ 20150710
        public static bool SetData(DataConnection pclsCache, string UserId, string HUserId, string HospitalCode, string Description, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    return IsSaved;
                }
                int flag = (int)Ps.UserIdMatch.SetData(pclsCache.CacheConnectionObject, UserId, HUserId, HospitalCode, Description, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsUserIdMatch.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static string getLatestHUserIdByHCode(DataConnection pclsCache, string UserId, string HospitalCode)
        {
            string HUserId = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return "";
                }
                HUserId = Ps.UserIdMatch.GetLatestHUserIdByHospitalCode(pclsCache.CacheConnectionObject, UserId, HospitalCode);

                return HUserId;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsUserIdMatch.getLatestHUserIdByHCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return "";
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

    }
}