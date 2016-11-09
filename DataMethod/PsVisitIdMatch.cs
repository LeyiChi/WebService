using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsVisitIdMatch
    {
        //ZAM 2015-1-20
    //    public static int SetData(DataConnection pclsCache, string UserId, int VisitId, string HospitalCode, string V1, string V2, string V3, string V4, string V5, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
    //    {
    //        int ret = 0;
    //        try
    //        {
    //            if (!pclsCache.Connect())
    //            {
    //                return ret;
    //            }

    //            ret = (int)Ps.VisitIdMatch.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, HospitalCode, V1, V2, V3, V4, V5, piUserId, piTerminalName, piTerminalIP, piDeviceType);

    //            return ret;
    //        }
    //        catch (Exception ex)
    //        {
    //            HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VisitIdMatch.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
    //            return ret;
    //        }
    //        finally
    //        {
    //            pclsCache.DisConnect();
    //        }
    //    }
    }
}