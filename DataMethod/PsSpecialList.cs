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
    public class PsSpecialList
    {
        //SetData YDS 2014-12-01 ZAM 更新ModuleType类型int改为string 与2014-12-08
        public static bool SetData(DataConnection pclsCache, string ModuleType, string PatientId, string DoctorId, int Level, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.SpecialList.SetData(pclsCache.CacheConnectionObject, ModuleType, PatientId, DoctorId, Level, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsSpecialList.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetCareLevel 根据模块编码和患者ID获取关注等级 YDS 2014-12-01 ZAM 更新ModuleType类型int改为string 与2014-12-08
        public static int GetCareLevel(DataConnection pclsCache, string ModuleType, string PatientId)
        {
            int Level = 1;  //ZAM 2015-1-13 "尚未关注"
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Level;

                }
                Level = (int)Ps.SpecialList.GetCareLevel(pclsCache.CacheConnectionObject, ModuleType, PatientId);
                return Level;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsSpecialList.GetCareLevel", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Level;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}