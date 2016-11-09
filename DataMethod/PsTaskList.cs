using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsTaskList
    {
        #region <" ZAM 2015-1-20 ">
        public static int SetData(DataConnection pclsCache, string PatientId, string ReminderNo, int TaskDate, int TaskTime, int IsDone, string Description, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.TaskList.SetData(pclsCache.CacheConnectionObject, PatientId, ReminderNo, TaskDate, TaskTime, IsDone, Description, piUserId, piTerminalName, piTerminalIP, piDeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static DataTable GetListbyDay(DataConnection pclsCache, string PatientId, int StartDate, int StartTime)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ReminderNo", typeof(string)));
            list.Columns.Add(new DataColumn("TaskDate", typeof(string)));
            list.Columns.Add(new DataColumn("TaskTime", typeof(string)));
            list.Columns.Add(new DataColumn("IsDone", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.TaskList.GetListbyDay(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("StartDate", CacheDbType.NVarChar).Value = StartDate;
                cmd.Parameters.Add("StartTime", CacheDbType.NVarChar).Value = StartTime;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ReminderNo"].ToString(), cdr["TaskDate"].ToString(), cdr["TaskTime"].ToString(), cdr["IsDone"].ToString(), cdr["Description"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.GetListbyDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetUndoneList ZC 2015-01-22
        public static DataTable GetUndoneList(DataConnection pclsCache, string PatientId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ReminderNo", typeof(string)));
            list.Columns.Add(new DataColumn("TaskDate", typeof(string)));
            list.Columns.Add(new DataColumn("TaskTime", typeof(string)));
            list.Columns.Add(new DataColumn("IsDone", typeof(int)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
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
                cmd = Ps.TaskList.GetPrevious(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ReminderNo"].ToString(), cdr["TaskDate"].ToString(), cdr["TaskTime"].ToString(), Convert.ToInt16(cdr["IsDone"].ToString()), cdr["Description"].ToString(), cdr["Content"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.GetPrevious", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetToDoList ZC 2015-01-22
        public static DataTable GetToDoList(DataConnection pclsCache, string PatientId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ReminderNo", typeof(string)));
            list.Columns.Add(new DataColumn("TaskDate", typeof(string)));
            list.Columns.Add(new DataColumn("TaskTime", typeof(string)));
            list.Columns.Add(new DataColumn("IsDone", typeof(int)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
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
                cmd = Ps.TaskList.GetLater(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ReminderNo"].ToString(), cdr["TaskDate"].ToString(), cdr["TaskTime"].ToString(), Convert.ToInt16(cdr["IsDone"].ToString()), cdr["Description"].ToString(), cdr["Content"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.GetLater", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static int UpdateIsDone(DataConnection pclsCache, string PatientId, string ReminderNo, int TaskDate, int TaskTime, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.TaskList.UpdateIsDone(pclsCache.CacheConnectionObject, PatientId, ReminderNo, TaskDate, TaskTime, piUserId, piTerminalName, piTerminalIP, piDeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.UpdateIsDone", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int GetUndoneNum(DataConnection pclsCache, string PatientId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.TaskList.GetUndoneNum(pclsCache.CacheConnectionObject, PatientId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.TaskList.GetUndoneNum", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        #endregion
    }
}