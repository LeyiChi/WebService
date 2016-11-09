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
    public class PsBasicInfo
    {
        //SetData WF 2014-12-2 
        public static bool SetData(DataConnection pclsCache, string UserId, string UserName, int Birthday, int Gender, int BloodType, string IDNo, string DoctorId, string InsuranceType, int InvalidFlag, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.BasicInfo.SetData(pclsCache.CacheConnectionObject, UserId, UserName, Birthday, Gender, BloodType, IDNo, DoctorId, InsuranceType, InvalidFlag, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //GetBasicInfo WF 2014-12-2 
        public static CacheSysList GetBasicInfo(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.BasicInfo.GetBasicInfo(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.GetBasicInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //GetBasicInfo WF 2014-12-2 
        public static CacheSysList GetPatientBasicInfo(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.BasicInfo.GetPatientBasicInfo(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.GetPatientBasicInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //GetUserBasicInfo TDY 2014-12-4 
        public static CacheSysList GetUserBasicInfo(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.BasicInfo.GetUserBasicInfo(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.GetUserBasicInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }
        
        //GetPatListByDoctorId CSQ 2015-01-16 输入DoctorId 获取没有购买全该医生全部负责模块的病人列表
        public static DataTable GetPatListByDoctorId(DataConnection pclsCache, string DoctorId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("Age", typeof(string)));
            list.Columns.Add(new DataColumn("Gender", typeof(string)));
            list.Columns.Add(new DataColumn("Module", typeof(string)));

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
                cmd = Ps.BasicInfo.GetPatListByDoctorId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["UserName"].ToString(), cdr["Age"].ToString(), cdr["Gender"].ToString(), cdr["Module"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.GetPatListByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetAgeByBirthDay SYF 2015-04-23
        public static int GetAgeByBirthDay(DataConnection pclsCache, int Birthday)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return 0;

                }
                else
                {
                    int Age = (int)Ps.BasicInfo.GetAgeByBirthDay(pclsCache.CacheConnectionObject, Birthday);
                    return Age;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.GetUserBasicInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return 0;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //SetData WF 2014-12-2 
        public static bool SetPatName(DataConnection pclsCache, string UserId, string UserName)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.BasicInfo.SetPatName(pclsCache.CacheConnectionObject, UserId, UserName);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfo.SetPatName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

    }
}