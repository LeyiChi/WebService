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
    public class WnTrnAlertRecord
    {
        //SetData GL 2014-12-01
        public static bool SetData(DataConnection pclsCache, string UserId, int SortNo, string AlertItemCode, DateTime AlertDateTime, int AlertType, int PushFlag, int ProcessFlag, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Wn.TrnAlertRecord.SetData(pclsCache.CacheConnectionObject, UserId, SortNo, AlertItemCode, AlertDateTime, AlertType, PushFlag, ProcessFlag, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetMaxSortNo WF 2014-12-10
        public static int GetMaxSortNo(DataConnection pclsCache, string UserId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Wn.TrnAlertRecord.GetMaxSortNo(pclsCache.CacheConnectionObject, UserId);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.GetMaxSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetUntreatedAlertAmount GL 2014-12-01 根据用户ID，获取患者未处理警报数
        public static int GetUntreatedAlertAmount(DataConnection pclsCache, string PatientId)
        {
            int UntreatedAlertAmount = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return UntreatedAlertAmount;

                }
                UntreatedAlertAmount = (int)Wn.TrnAlertRecord.GetUntreatedAlertAmount(pclsCache.CacheConnectionObject, PatientId);
                return UntreatedAlertAmount;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.GetUntreatedAlertAmount", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return UntreatedAlertAmount;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //SetPushFlag GL 2014-12-01 推送状态置位
        public static bool SetPushFlag(DataConnection pclsCache, string UserId, int SortNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsOk = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsOk;

                }
                int flag = (int)Wn.TrnAlertRecord.SetPushFlag(pclsCache.CacheConnectionObject, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                if (flag == 1)
                {
                    IsOk = true;
                }
                return IsOk;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.SetPushFlag", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsOk;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetTrnAlertRecordList GL 2014-12-26 根据PatientId获取警报列表 (ZAM 12-26: + SortNo )
        public static DataTable GetTrnAlertRecordList(DataConnection pclsCache, string PatientId)
        {
            DataTable AlertRecordList = new DataTable();
           
            AlertRecordList.Columns.Add(new DataColumn("AlertType", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("AlertTypeName", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("AlertItemCode", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("AlertItem", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("AlertDateTime", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("PushFlag", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("ProcessFlag", typeof(string)));
            AlertRecordList.Columns.Add(new DataColumn("SortNo", typeof(string)));

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
                cmd = Wn.TrnAlertRecord.GetTrnAlertRecordList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    AlertRecordList.Rows.Add(cdr["AlertType"].ToString(), cdr["AlertTypeName"].ToString(), cdr["AlertItemCode"].ToString(), cdr["AlertItem"].ToString(), cdr["AlertDateTime"].ToString(), cdr["PushFlag"].ToString(), cdr["ProcessFlag"].ToString(), cdr["SortNo"].ToString());
                }
                return AlertRecordList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.GetTrnAlertRecordList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //SetProcessFlag GL 2014-12-26 警报处置状态置位
        public static bool SetProcessFlag(DataConnection pclsCache, string UserId, int SortNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsOk = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsOk;

                }
                int flag = (int)Wn.TrnAlertRecord.SetProcessFlag(pclsCache.CacheConnectionObject, UserId, SortNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                if (flag == 1)
                {
                    IsOk = true;
                }
                return IsOk;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "WnTrnAlertRecord.SetProcessFlag", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsOk;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

    }
}