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
    public class CmMstNumbering
    {
        //GetNo 自动编号 YDS 2014-12-01
        public static string GetNo(DataConnection pclsCache, int NumberingType, string TargetDate)
        {
            string number = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return number;

                }
                number = Cm.MstNumbering.GetNo(pclsCache.CacheConnectionObject, NumberingType, TargetDate);
                return number;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取编号失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstNumbering.GetNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return number;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}