using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
using WebService.DataClass;

namespace WebService.DataMethod
{
    public class PsTask
    {
        //WF 20150408
        public static int SetData(DataConnection pclsCache, string PlanNo, string Id, string Type, string Code, string Instruction, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Task.SetData(pclsCache.CacheConnectionObject, PlanNo, Id, Type, Code, Instruction, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Task.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //WF 20150408
        public static DataTable GetPsTaskByType(DataConnection pclsCache, string PlanNo, string Type)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("CodeName", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Task.GetPsTaskByType(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("Type", CacheDbType.NVarChar).Value = Type;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Code"].ToString(), cdr["CodeName"].ToString(), cdr["Instruction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Task.GetPsTaskByType", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150408
        public static DataTable GetPsTask(DataConnection pclsCache, string PlanNo)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("CodeName", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Task.GetPsTask(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Type"].ToString(), cdr["Code"].ToString(), cdr["CodeName"].ToString(), cdr["Instruction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Task.GetPsTask", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //ZC 20150416
        public static int DeleteAllByPlanNo(DataConnection pclsCache, string PlanNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Task.DeleteAllByPlanNo(pclsCache.CacheConnectionObject, PlanNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Task.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetTaskList 获取某计划下的所有任务 LS 2015-4-22
        public static DataTable GetTaskList(DataConnection pclsCache, string PlanNo)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("CodeName", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.Task.GetPsTask(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = PlanNo;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Type"].ToString(), cdr["Code"].ToString(), cdr["CodeName"].ToString(), cdr["Instruction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsTask.GetTaskList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetTaskList 获取某计划下的所有任务，并进行分类加工 LS 2015-4-22
        public static List<Task> GetSpTaskList(DataConnection pclsCache, string PlanNo)
        {

            List<Task> TaskList = new List<Task>();

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("CodeName", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.Task.GetPsTask(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = PlanNo;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Type"].ToString(), cdr["Code"].ToString(), cdr["CodeName"].ToString(), cdr["Instruction"].ToString());
                }


                //确保排序
                if (list.Rows.Count > 0)
                {
                    DataView dv = list.DefaultView;
                    dv.Sort = "Type desc";
                    DataTable list_sort = dv.ToTable();
                    list_sort.Rows.Add("end", "end", "end", "end", "end");  //用于最后一天输出

                    if (list_sort.Rows.Count > 1)
                    {
                        string temp_type = list_sort.Rows[0]["Type"].ToString();
                        Task Task = new Task();
                        Task.TaskType = list_sort.Rows[0]["Type"].ToString();

                        TaskDeatil TaskDeatil = new TaskDeatil();
                        TaskDeatil.TaskName = list_sort.Rows[0]["CodeName"].ToString();
                        TaskDeatil.Instruction = list_sort.Rows[0]["Instruction"].ToString();
                        Task.TaskDeatilList.Add(TaskDeatil);

                        if (list.Rows.Count > 2)
                        {
                            for (int i = 1; i <= list_sort.Rows.Count - 1; i++)
                            {
                                if (temp_type == list_sort.Rows[i]["Type"].ToString())
                                {
                                    TaskDeatil = new TaskDeatil();
                                    TaskDeatil.TaskName = list_sort.Rows[i]["CodeName"].ToString();
                                    TaskDeatil.Instruction = list_sort.Rows[i]["Instruction"].ToString();
                                    Task.TaskDeatilList.Add(TaskDeatil);
                                }
                                else
                                {
                                    TaskList.Add(Task);

                                    if (list_sort.Rows[i]["Type"].ToString() != "end")
                                    {

                                        Task = new Task();
                                        Task.TaskType = list_sort.Rows[i]["Type"].ToString();

                                        TaskDeatil = new TaskDeatil();
                                        TaskDeatil.TaskName = list_sort.Rows[i]["CodeName"].ToString();
                                        TaskDeatil.Instruction = list_sort.Rows[i]["Instruction"].ToString();
                                        Task.TaskDeatilList.Add(TaskDeatil);

                                        temp_type = list_sort.Rows[i]["Type"].ToString();

                                    }

                                }
                            }
                        }
                    }
                }
                return TaskList;

            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsTask.GetSpTaskList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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