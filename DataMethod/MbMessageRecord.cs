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
    public class MbMessageRecord
    {
        //SetData GL 2014-12-01
        public static bool SetData(DataConnection pclsCache, string MessageNo, int MessageType, int SendStatus, int ReadStatus, string SendBy, string SendDateTime, string Title, string Content, string Reciever, int SMSFlag, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Mb.MessageRecord.SetData(pclsCache.CacheConnectionObject, MessageNo, MessageType, SendStatus, ReadStatus, SendBy, SendDateTime, Title, Content, Reciever, SMSFlag, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //SetMessage GL 2014-12-01 将发送成功或存入草稿箱中的消息写入数据库
        public static bool SetMessage(DataConnection pclsCache, string MessageNo, int MessageType, string SendBy, string Title, string Reciever, string Content, int SetCondition, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool IsOk = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsOk;

                }
                int flag = (int)Mb.MessageRecord.SetMessage(pclsCache.CacheConnectionObject, MessageNo, MessageType, SendBy, Title, Reciever, Content, SetCondition, revUserId, pTerminalName, pTerminalIP, pDeviceType);
                if (flag == 1)
                {
                    IsOk = true;
                }
                return IsOk;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.SetMessage", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsOk;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetMailCount GL 2014-12-01 对未读消息计数
        public static int GetMailCount(DataConnection pclsCache, string UserId)
        {
            int CountNew = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return CountNew;

                }
                CountNew = (int)Mb.MessageRecord.GetMailCount(pclsCache.CacheConnectionObject, UserId);
                return CountNew;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetMailCount", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return CountNew;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDraftCount GL 2014-12-01 对草稿箱中的消息计数
        public static int GetDraftCount(DataConnection pclsCache, string UserId)
        {
            int CountDraft = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return CountDraft;

                }
                CountDraft = (int)Mb.MessageRecord.GetDraftCount(pclsCache.CacheConnectionObject, UserId);
                return CountDraft;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetDraftCount", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return CountDraft;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //ChangeReadStatus GL 2014-12-01 收信页面中，点击查看消息时，使消息状态从未读变为已读
        public static bool ChangeReadStatus(DataConnection pclsCache, string MessageNo, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            bool CRFlag = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return CRFlag;

                }
                int flag = (int)Mb.MessageRecord.ChangeReadStatus(pclsCache.CacheConnectionObject, MessageNo, revUserId, pTerminalIP, pTerminalName, pDeviceType);
                if (flag == 1)
                {
                    CRFlag = true;
                }
                return CRFlag;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.ChangeReadStatus", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return CRFlag;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetHaveSentList GL 2015-01-28 获取已发送消息列表
        public static DataTable GetHaveSentList(DataConnection pclsCache, string UserId)
        {
            DataTable SendList = new DataTable();

            SendList.Columns.Add(new DataColumn("MessageNo", typeof(string)));
            SendList.Columns.Add(new DataColumn("Reciever", typeof(string)));
            SendList.Columns.Add(new DataColumn("RecieverName", typeof(string)));
            SendList.Columns.Add(new DataColumn("Title", typeof(string)));
            SendList.Columns.Add(new DataColumn("SendDateTime", typeof(string)));
            SendList.Columns.Add(new DataColumn("Content", typeof(string)));
            SendList.Columns.Add(new DataColumn("Flag", typeof(int)));

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
                cmd = Mb.MessageRecord.GetHaveSentList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    SendList.Rows.Add(cdr["MessageNo"].ToString(), cdr["Reciever"].ToString(), cdr["RecieverName"].ToString(), cdr["Title"].ToString(), cdr["SendDateTime"].ToString(), cdr["Content"].ToString(), Convert.ToInt32(cdr["Flag"]));
                }
                return SendList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetHaveSentList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetReceiveList GL 2015-01-28 获取已收到的消息列表
        public static DataTable GetReceiveList(DataConnection pclsCache, string UserId)
        {
            DataTable ReceiveList = new DataTable();

            ReceiveList.Columns.Add(new DataColumn("MessageNo", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("SendBy", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("SendByName", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("Title", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("SendDateTime", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("Content", typeof(string)));
            ReceiveList.Columns.Add(new DataColumn("ReadStatus", typeof(int)));
            ReceiveList.Columns.Add(new DataColumn("Flag", typeof(int)));

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
                cmd = Mb.MessageRecord.GetReceiveList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    ReceiveList.Rows.Add(cdr["MessageNo"].ToString(), cdr["SendBy"].ToString(), cdr["SendByName"].ToString(), cdr["Title"].ToString(), cdr["SendDateTime"].ToString(), cdr["Content"].ToString(), Convert.ToInt32(cdr["ReadStatus"]), Convert.ToInt32(cdr["Flag"]));
                }
                return ReceiveList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetReceiveList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetDraftList GL 2015-01-28 获取草稿箱消息列表
        public static DataTable GetDraftList(DataConnection pclsCache, string UserId)
        {
            DataTable DraftList = new DataTable();

            DraftList.Columns.Add(new DataColumn("MessageNo", typeof(string)));
            DraftList.Columns.Add(new DataColumn("Reciever", typeof(string)));
            DraftList.Columns.Add(new DataColumn("RecieverName", typeof(string)));
            DraftList.Columns.Add(new DataColumn("Title", typeof(string)));
            DraftList.Columns.Add(new DataColumn("SendDateTime", typeof(string)));
            DraftList.Columns.Add(new DataColumn("Content", typeof(string)));
            DraftList.Columns.Add(new DataColumn("Flag", typeof(int)));

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
                cmd = Mb.MessageRecord.GetDraftList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    DraftList.Rows.Add(cdr["MessageNo"].ToString(), cdr["Reciever"].ToString(), cdr["RecieverName"].ToString(), cdr["Title"].ToString(), cdr["SendDateTime"].ToString(), cdr["Content"].ToString(), Convert.ToInt32(cdr["Flag"]));
                }
                return DraftList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetDraftList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetMessageDetail GL 2014-12-24 获取消息详细信息
        public static CacheSysList GetMessageDetail(DataConnection pclsCache, string MessageNo)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Mb.MessageRecord.GetMessageDetail(pclsCache.CacheConnectionObject, MessageNo);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetMessageDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //DeleteDraft GL 2015-01-14 删除草稿箱信息
        public static int DeleteDraft(DataConnection pclsCache, string MessageNo)
        {
            int Ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return Ret;

                }
                Ret = (int)Mb.MessageRecord.DeleteDraft(pclsCache.CacheConnectionObject, MessageNo);
                return Ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.DeleteDraft", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //SetSMS GL 2015-04-07 将消息写入数据库
        public static int SetSMS(DataConnection pclsCache, string SendBy, string Reciever, string Content, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Mb.MessageRecord.SetSMS(pclsCache.CacheConnectionObject, SendBy, Reciever, Content, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.SetSMS", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetSMSCountForAll  GL 2015-04-07 根据专员Id获取其对应所有患者未读消息总数
        public static int GetSMSCountForAll(DataConnection pclsCache, string DoctorId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Mb.MessageRecord.GetSMSCountForAll(pclsCache.CacheConnectionObject, DoctorId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.GetSMSCountForAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetSMSCountForOne  GL 2015-04-07 获取一对一未读消息数
        public static int GetSMSCountForOne(DataConnection pclsCache, string Reciever, string SendBy)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Mb.MessageRecord.GetSMSCountForOne(pclsCache.CacheConnectionObject, Reciever, SendBy);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.GetSMSCountForOne", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetSMSList  GL 2015-04-07 根据专员Id获取患者消息列表
        public static DataTable GetSMSList(DataConnection pclsCache, string DoctorId, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("PatientId", typeof(string)));
            list.Columns.Add(new DataColumn("PatientName", typeof(string)));
            list.Columns.Add(new DataColumn("Count", typeof(string)));
            list.Columns.Add(new DataColumn("Content", typeof(string)));
            list.Columns.Add(new DataColumn("SendDateTime", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Mb.MessageRecord.GetSMSList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["PatientId"].ToString(), cdr["PatientName"].ToString(), cdr["Count"].ToString(), cdr["Content"].ToString(), cdr["SendDateTime"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.GetSMSList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetSMSDialogue  GL 2015-04-07 获取消息对话，按时间先后排列
        public static DataTable GetSMSDialogue(DataConnection pclsCache, string Reciever, string SendBy)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("IDFlag", typeof(string)));
            list.Columns.Add(new DataColumn("SendDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("Content", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Mb.MessageRecord.GetSMSDialogue(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Reciever", CacheDbType.NVarChar).Value = Reciever;
                cmd.Parameters.Add("SendBy", CacheDbType.NVarChar).Value = SendBy;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["IDFlag"].ToString(), cdr["SendDateTime"].ToString(), cdr["Content"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.GetSMSDialogue", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //SetSMSRead  GL 2015-04-07 改写消息阅读状态
        public static int SetSMSRead(DataConnection pclsCache, string Reciever, string SendBy, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Mb.MessageRecord.SetSMSRead(pclsCache.CacheConnectionObject, Reciever, SendBy, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.SetSMSRead", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetLatestSMS GL 2015-04-10 根据专员Id和患者Id获取专员收到的最新一条消息 
        public static CacheSysList GetLatestSMS(DataConnection pclsCache, string DoctorId, string PatientId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Mb.MessageRecord.GetLatestSMS(pclsCache.CacheConnectionObject, DoctorId, PatientId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MbMessageRecord.GetLatestSMS", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //ChangePushStatus GL 2015-04-24 改写推送状态
        public static int ChangePushStatus(DataConnection pclsCache, string MessageNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Mb.MessageRecord.ChangePushStatus(pclsCache.CacheConnectionObject, MessageNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Mb.MessageRecord.ChangePushStatus", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    
    }
}