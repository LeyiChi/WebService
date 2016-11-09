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
    public class PsReminder
    {
        //ZC 2015-1-21
        public static int SetData(DataConnection pclsCache, string PatientId, string ReminderNo, int ReminderType, string Content, int AlertMode, DateTime StartDateTime, int NextDate, int NextTime, int Interval, int InvalidFlag, string Description, string CreatedBy, string revUserId, string pTerminalName, string pTerminalIP, int pDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Reminder.SetData(pclsCache.CacheConnectionObject, PatientId, ReminderNo, ReminderType, Content, AlertMode, StartDateTime, NextDate, NextTime, Interval, InvalidFlag, Description, CreatedBy, revUserId, pTerminalName, pTerminalIP, pDeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Reminder.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //ZC 2015-1-21
        public static int SetInvalidFlag(DataConnection pclsCache, string piPatientId, string piReminderNo, string UserId, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Reminder.SetInvalidFlag(pclsCache.CacheConnectionObject, piPatientId, piReminderNo, UserId, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Reminder.SetInvalidFlag", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //ZC 2015-1-21
        public static DataTable GetReminder(DataConnection pclsCache, string PatientId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ReminderNo", typeof(string)));
            list.Columns.Add(new DataColumn("ReminderType", typeof(string)));
            list.Columns.Add(new DataColumn("ReminderTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Content", typeof(string)));
            list.Columns.Add(new DataColumn("AlertMode", typeof(string)));
            list.Columns.Add(new DataColumn("AlertModeName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("NextDate", typeof(string)));
            list.Columns.Add(new DataColumn("NextTime", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("CreatedBy", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Reminder.GetReminder(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ReminderNo"].ToString(), cdr["ReminderType"].ToString(), cdr["ReminderTypeName"].ToString(), cdr["Content"].ToString(), cdr["AlertMode"].ToString(), cdr["AlertModeName"].ToString(), cdr["StartDateTime"].ToString(), cdr["NextDate"].ToString(), cdr["NextTime"].ToString(), cdr["Description"].ToString(), cdr["CreatedBy"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Reminder.GetReminder", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //ZC 2015-1-21
        public static DataTable GetReminderbyDay(DataConnection pclsCache, string PatientId, int StartDate, int StartTime)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ReminderNo", typeof(string)));
            list.Columns.Add(new DataColumn("ReminderType", typeof(string)));
            list.Columns.Add(new DataColumn("ReminderTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Content", typeof(string)));
            list.Columns.Add(new DataColumn("AlertMode", typeof(string)));
            list.Columns.Add(new DataColumn("AlertModeName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("NextDate", typeof(string)));
            list.Columns.Add(new DataColumn("NextTime", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Reminder.GetReminderbyDay(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("StartTime", CacheDbType.Int).Value = StartTime;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ReminderNo"].ToString(), cdr["ReminderType"].ToString(), cdr["ReminderTypeName"].ToString(), cdr["Content"].ToString(), cdr["AlertMode"].ToString(), cdr["AlertModeName"].ToString(), cdr["StartDateTime"].ToString(), cdr["NextDate"].ToString(), cdr["NextTime"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Reminder.GetReminderbyDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

    }
}