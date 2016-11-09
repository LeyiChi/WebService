using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
using InterSystems.Data.CacheTypes;
using WebService.DataClass;

namespace WebService.DataMethod
{
    public class PsCompliance
    {
        // 陆遥 2015-04-08 插入某条数据
        public static int SetData(DataConnection pclsCache, string PatientId, int Date, string PlanNo, Double Compliance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Compliance.SetData(pclsCache.CacheConnectionObject, PatientId, Date, PlanNo, Compliance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 陆遥 2015-04-08 获取患者某计划内某天的依从率
        public static double GetComplianceByDay(DataConnection pclsCache, string PatientId, int Date, string PlanNo)
        {
            double ret = 0.0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (double)Ps.Compliance.GetComplianceByDay(pclsCache.CacheConnectionObject, PatientId, Date, PlanNo);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetComplianceByDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 陆遥 2015-04-08 获取某计划的某段时间(包括端点)的依从率列表
        public static DataTable GetComplianceListByPeriod(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
            list.Columns.Add(new DataColumn("Compliance", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetComplianceListByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.Int).Value = EndDate;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Date"].ToString(), cdr["Compliance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetComplianceListByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // 陆遥 2015-04-08根据计划编码和日期，获取依从率
        public static DataTable GetTasksByDate(DataConnection pclsCache, string PatientId, int Date, string PlanNo)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("TaskID", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetTasksByDate(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("Date", CacheDbType.Int).Value = Date;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["TaskID"].ToString(), cdr["TaskName"].ToString(), cdr["Status"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTasksByDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // 陆遥 2015-04-08 在当天根据任务状态的完成情况输出相应的任务
        public static DataTable GetTaskByStatus(DataConnection pclsCache, string PatientId, string PlanNo, int PiStatus)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));
            list.Columns.Add(new DataColumn("TaskCode", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("TaskType", typeof(string)));
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
                cmd = Ps.Compliance.GetTaskByStatus(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("PiStatus", CacheDbType.Int).Value = PiStatus;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Status"].ToString(), cdr["TaskCode"].ToString(), cdr["TaskName"].ToString(), cdr["TaskType"].ToString(), cdr["Instruction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTaskByStatus", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //陆遥 2015-04-17 获取当天完成的(或未完成的)任务数
        public static int GetTaskNumber(DataConnection pclsCache, string PatientId, string PlanNo, int PiStatus)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Compliance.GetTaskNumber(pclsCache.CacheConnectionObject, PatientId, PlanNo, PiStatus);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTaskNumber", "数据库操作异常！ error information" + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }


        //以下是任务完成情况用到的函数

        //GetCompliacneRate 计算某段时间的总依从率 LS 2015-03-27 
        public static string GetCompliacneRate(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {
            string compliacneRate = "";
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetComplianceListByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.NVarChar).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.NVarChar).Value = EndDate;

                cdr = cmd.ExecuteReader();

                double sum = 0;
                int count = 0;
                while (cdr.Read())
                {
                    sum += Convert.ToDouble(cdr["Compliance"]);
                    count++;
                }

                if (count != 0)
                {
                    //compliacneRate = ((int)((sum / count) * 100)).ToString();
                    compliacneRate = (Math.Round(sum / count, 2, MidpointRounding.AwayFromZero) * 100).ToString(); //保留整数

                }

                return compliacneRate;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsCompliacne.GetCompliacneRate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetSignDetailByPeriod 通过Ps.Compliance中的date获取当天某项生理参数值，形成系列  DataTable 形式LS 2015-04-17
        public static DataTable GetSignDetailByPeriod(DataConnection pclsCache, string PatientId, string PlanNo, string ItemType, string ItemCode, int StartDate, int EndDate)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("RecordDate", typeof(string)));
            list.Columns.Add(new DataColumn("RecordTime", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;


            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.ComplianceDetail.GetSignDetailByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("ItemType", CacheDbType.NVarChar).Value = ItemType;
                cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;
                cmd.Parameters.Add("StartDate", CacheDbType.NVarChar).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.NVarChar).Value = EndDate;

                cdr = cmd.ExecuteReader();

                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RecordDate"].ToString(), cdr["RecordTime"].ToString(), cdr["Value"].ToString(), cdr["Unit"].ToString());
                }

                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsCompliacne.GetSignDetailByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //CaculateWeekDay 判断日期是星期几 LS 2015-03-27 
        public static string CaculateWeekDay(string date)
        {
            string week = "星期一";  //待标记颜色
            try
            {
                string weekDayEn = Convert.ToDateTime(date).DayOfWeek.ToString();
                switch (weekDayEn)
                {
                    case "Monday":
                        week = "星期一";
                        break;
                    case "Tuesday":
                        week = "星期二";
                        break;
                    case "Wednesday":
                        week = "星期三";
                        break;
                    case "Thursday":
                        week = "星期四";
                        break;
                    case "Friday":
                        week = "星期五";
                        break;
                    case "Saturday":
                        week = "星期六";
                        break;
                    case "Sunday":
                        week = "星期日";
                        break;
                    default: break;
                }

                return week;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsCompliance.CaculateWeekDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //GetTasksComByPeriod  某段时间所有任务的依从情况  DataTable数据库形式  LS 2010703
        public static DataTable GetTasksComByPeriodDT(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
            list.Columns.Add(new DataColumn("ComplianceValue", typeof(double)));
            list.Columns.Add(new DataColumn("TaskType", typeof(string))); //中文
            list.Columns.Add(new DataColumn("TaskId", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(int)));
            list.Columns.Add(new DataColumn("Type", typeof(string))); //英文

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetTasksComByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.Int).Value = EndDate;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Date"].ToString(), Convert.ToDouble(cdr["ComplianceValue"]), cdr["TaskType"].ToString(), cdr["TaskId"].ToString(), cdr["TaskName"].ToString(), Convert.ToInt32(cdr["Status"]), cdr["Type"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTasksComByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetTasksComCountByPeriod 所有任务的依从情况简化版 不再全部显示，只对完成数量就行统计  pad、phone使用中
        public static List<CompliacneDetailByD> GetTasksComCountByPeriod(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {

            List<CompliacneDetailByD> resultList = new List<CompliacneDetailByD>();

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                /*
                //读取各类型任务的数量
                int signCount = 0;
                int lifeCount = 0;
                int drugCount = 0;

                DataTable TaskList = new DataTable();
                TaskList = PsTask.GetTaskList(pclsCache, PlanNo);


                //体征测量
                string condition = " Type = 'VitalSign'";
                DataRow[] VitalSignRows = TaskList.Select(condition);
                if (VitalSignRows != null)
                {
                    signCount = VitalSignRows.Length;
                }

                //生活方式 
                condition = " Type = 'LifeStyle'";
                DataRow[] LifeStyleRows = TaskList.Select(condition);
                if (LifeStyleRows != null)
                {
                    lifeCount = LifeStyleRows.Length;
                }

                //用药情况
                condition = " Type = 'Drug'";
                DataRow[] DrugRows = TaskList.Select(condition);
                if (DrugRows != null)
                {
                    drugCount = DrugRows.Length;
                }
                */

                DataTable list = new DataTable();
                list = PsCompliance.GetTasksComByPeriodDT(pclsCache, PatientId, PlanNo, StartDate, EndDate);

                //确保排序
                DataView dv = list.DefaultView;
                dv.Sort = "Date Asc, Type desc, Status Asc"; //体征s 生活l 用药d   前提：某计划内任务维持不变  即计划内每天的任务是一样的
                DataTable list_sort = dv.ToTable();
                list_sort.Rows.Add("end", 0, "", "", "", 0);  //用于最后一天输出

                if (list_sort.Rows.Count > 1)
                {
                    string temp_date = list_sort.Rows[0]["Date"].ToString();
                    string temp_type = list_sort.Rows[0]["TaskType"].ToString();  //中文
                    int complete = 0; int count = 0;  //完成数量统计


                    string temp_str = "";
                    temp_str += "该天依从率：" + list_sort.Rows[0]["ComplianceValue"].ToString() + "<br>";
                    temp_str += "<b><span style='font-size:14px;'>" + list_sort.Rows[0]["TaskType"].ToString() + "：</span></b>";

                    CompliacneDetailByD CompliacneDetailByD = new CompliacneDetailByD();
                    CompliacneDetailByD.Date = list_sort.Rows[0]["Date"].ToString();
                    //CompliacneDetailByD.ComplianceValue = list_sort.Rows[0]["ComplianceValue"].ToString();

                    if (Convert.ToDouble(list_sort.Rows[0]["ComplianceValue"]) == 0)  //点的颜色由该天依从率决定
                    {
                        CompliacneDetailByD.drugBullet = "";
                        CompliacneDetailByD.drugColor = "#DADADA";
                    }
                    else if (Convert.ToDouble(list_sort.Rows[0]["ComplianceValue"]) == 1)
                    {
                        CompliacneDetailByD.drugBullet = "";
                        CompliacneDetailByD.drugColor = "#777777";
                    }
                    else
                    {
                        CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                        CompliacneDetailByD.drugColor = "";
                    }


                    if (Convert.ToInt32(list_sort.Rows[0]["Status"]) == 1)  //某天某项任务的完成情况
                    {
                        complete++;
                        count++;
                    }
                    else
                    {
                        count++;
                    }


                    //只有一条数据
                    if (list_sort.Rows.Count == 2)
                    {
                        temp_str += complete + "/" + count;
                        CompliacneDetailByD.Events = temp_str;
                        resultList.Add(CompliacneDetailByD);
                    }

                    //＞一条数据
                    if (list_sort.Rows.Count > 2)
                    {
                        for (int i = 1; i <= list_sort.Rows.Count - 1; i++)
                        {
                            if (temp_date == list_sort.Rows[i]["Date"].ToString())  //同一天
                            {
                                if (temp_type == list_sort.Rows[i]["TaskType"].ToString())     //同天同任务类型
                                {
                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        complete++;
                                        count++;
                                    }
                                    else
                                    {
                                        count++;
                                    }
                                }
                                else   //同天不同任务类型
                                {
                                    temp_str += complete + "/" + count;
                                    complete = 0; count = 0;  //清空统计量
                                    temp_str += "<br><b><span style='font-size:14px;'>" + list_sort.Rows[i]["TaskType"].ToString() + "：</span></b>";

                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        complete++;
                                        count++;
                                    }
                                    else
                                    {
                                        count++;
                                    }

                                    temp_type = list_sort.Rows[i]["TaskType"].ToString();
                                }

                            }
                            else   //不同天
                            {
                                //上一天输出

                                temp_str += complete + "/" + count;
                                complete = 0; count = 0;  //清空统计量
                                CompliacneDetailByD.Events = temp_str;
                                resultList.Add(CompliacneDetailByD);

                                if (list_sort.Rows[i]["Date"].ToString() != "end")
                                {
                                    //获取新一天
                                    CompliacneDetailByD = new CompliacneDetailByD();
                                    CompliacneDetailByD.Date = list_sort.Rows[i]["Date"].ToString();
                                    //CompliacneDetailByD.ComplianceValue = list_sort.Rows[i]["ComplianceValue"].ToString();

                                    if (Convert.ToDouble(list_sort.Rows[i]["ComplianceValue"]) == 0)  //某天依从率
                                    {
                                        CompliacneDetailByD.drugBullet = "";
                                        CompliacneDetailByD.drugColor = "#DADADA";
                                    }
                                    else if (Convert.ToDouble(list_sort.Rows[i]["ComplianceValue"]) == 1)
                                    {
                                        CompliacneDetailByD.drugBullet = "";
                                        CompliacneDetailByD.drugColor = "#777777";
                                    }
                                    else
                                    {
                                        CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                                        CompliacneDetailByD.drugColor = "";
                                    }

                                    temp_str = "";
                                    temp_str += "该天依从率：" + list_sort.Rows[i]["ComplianceValue"].ToString() + "<br>";
                                    temp_str += "<b><span style='font-size:14px;'>" + list_sort.Rows[i]["TaskType"].ToString() + "：</span></b>";

                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        complete++;
                                        count++;
                                    }
                                    else
                                    {
                                        count++;
                                    }

                                    temp_date = list_sort.Rows[i]["Date"].ToString();
                                    temp_type = list_sort.Rows[i]["TaskType"].ToString();
                                }
                            }
                        }

                    }

                }

                return resultList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsCompliance.GetTasksComCountByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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


        //GetTasksComByPeriod  某一天所有任务的依从情况 DataTable数据库形式   用于点击事件显示详细   LS 2010706
        public static DataTable GetTasksComListByDate(DataConnection pclsCache, string PatientId, string PlanNo, int Date)
        {

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
            list.Columns.Add(new DataColumn("ComplianceValue", typeof(double)));
            list.Columns.Add(new DataColumn("TaskType", typeof(string)));
            list.Columns.Add(new DataColumn("TaskId", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(int)));
            list.Columns.Add(new DataColumn("TaskCode", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetTasksComListByDate(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("Date", CacheDbType.Int).Value = Date;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Date"].ToString(), Convert.ToDouble(cdr["ComplianceValue"]), cdr["TaskType"].ToString(), cdr["TaskId"].ToString(), cdr["TaskName"].ToString(), Convert.ToInt32(cdr["Status"]), cdr["TaskCode"].ToString(), cdr["Type"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTasksComListByDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetImplementationByDate 某天所有任务的依从情况 整理加工  LS 2010706
        public static TaskComDetailByD GetImplementationByDate(DataConnection pclsCache, string PatientId, string PlanNo, int Date)
        {

            TaskComDetailByD TaskComDetailByD = new TaskComDetailByD();

            try
            {
                TaskComDetailByD.Date = Date.ToString().Substring(0, 4) + "-" + Date.ToString().Substring(4, 2) + "-" + Date.ToString().Substring(6, 2);
                TaskComDetailByD.WeekDay = PsCompliance.CaculateWeekDay(TaskComDetailByD.Date);

                DataTable ComplianceList = new DataTable();
                ComplianceList = PsCompliance.GetTasksComListByDate(pclsCache, PatientId, PlanNo, Date);

                #region 后期可能用于优化
                //先读任务表，读取体征，拿出新数据；再读药物表，超过三个则省略号
                //DataTable TaskList = new DataTable();
                //TaskList = PsTask.GetTaskList(pclsCache, PlanNo);

                ////读取体征，拿出当天最新数据
                //string condition = " Type = 'VitalSign'";
                //DataRow[] VitalSignRows = TaskList.Select(condition);

                //CacheSysList VitalSignList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                //for (int j=0; j < VitalSignRows.Length; j++)
                //{
                //    string code = VitalSignRows[j]["Type"].ToString();
                //    string[] sArray = code.Split(new char[] { '|' });;//拆分
                //    string type = sArray[0].ToString();
                //    VitalSignList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                //    VitalSignList = PsVitalSigns.GetSignByDay(pclsCache, PatientId, code, type, Date);
                //    if (VitalSignList != null)
                //    {

                //    }
                //}





                //体征
                /*
                string condition = " Type = 'VitalSign'";
                DataRow[] VitalSignRows = ComplianceList.Select(condition);

                List<TaskCom> TaskComList = new List<TaskCom>();
                TaskCom TaskCom = new TaskCom();
                for (int j = 0; j < VitalSignRows.Length; j++)
                {
                    VitalTaskCom = new VitalTaskCom();
                    VitalTaskCom.SignName = VitalSignRows[j]["TaskName"].ToString();
                    VitalTaskCom.Status = VitalSignRows[j]["Status"].ToString();
                    if (TaskCom.TaskStatus == "1")
                    {
                        string code = VitalSignRows[j]["TaskCode"].ToString();
                        string[] sArray = code.Split(new char[] { '|' }); ;//拆分
                        string type = sArray[0].ToString();
                        //CacheSysList VitalSignList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                        CacheSysList VitalSignList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                        VitalSignList = PsVitalSigns.GetSignByDay(pclsCache, PatientId, code, type, Date);
                        if (VitalSignList != null)
                        {
                            
                            VitalTaskCom.Time = PulList[1].ToString();
                            VitalTaskCom.Value = PulList[2].ToString();
                            VitalTaskCom.Unit = PulList[3].ToString();
                            
                        } 
                    }
                    VitalTaskComList.Add(VitalTaskCom);
                }
                TaskComDetailByD.VitalTaskComList = VitalTaskComList;

                
                 string vitalCondition = " Type = 'VitalSign'";
                    DataRow[] VitalSignRows = ComplianceList.Select(vitalCondition);

                    if ((VitalSignRows != null) && (VitalSignRows.Length >= 2))
                    {



                        if (VitalSignRows.Length == 2)  //只有血压
                        {

                        }
                        else //血压和脉率
                        {

                        }
                    }
                
                */
                #endregion

                //取出当天的体征测量 若有与测试任务拼接好了
                //先写死取的生理参数
                List<VitalTaskCom> VitalTaskComList = new List<VitalTaskCom>();
                VitalTaskCom VitalTaskCom = new VitalTaskCom();

                string Module = "";
                InterSystems.Data.CacheTypes.CacheSysList planInfo = null;
                planInfo = PsPlan.GetPlanInfo(pclsCache, PlanNo);
                if (planInfo != null)
                {

                    Module = planInfo[4].ToString();

                }

                if (Module == "M1")
                {
                    #region  高血压模块  需要考虑没有脉率任务的情况

                    //血压任务肯定有
                    int BPTime = 0;
                    int mark = 0;
                    string SysValue = "";
                    string DiaValue = "";
                    string Unit = "";

                     string conditionBP1 = " TaskCode = 'Bloodpressure|Bloodpressure_1'";
                    DataRow[] BP1Rows = ComplianceList.Select(conditionBP1);
                    if ((BP1Rows != null) && (BP1Rows.Length == 1))
                    {
                        if (BP1Rows[0]["Status"].ToString() == "1")
                        {
                            CacheSysList SysList = PsVitalSigns.GetSignByDay(pclsCache, PatientId, "Bloodpressure", "Bloodpressure_1", Date);
                            if (SysList != null)
                            {
                                mark = 1;
                                BPTime = Convert.ToInt32(SysList[1].ToString());  //时刻数据库是"1043"形式，需要转换  取两者最新的那个时间好了 即谁大取谁
                                SysValue = SysList[2].ToString();
                                Unit = SysList[3].ToString();
                            }
                        }
                    }

                     string conditionBP2 = " TaskCode = 'Bloodpressure|Bloodpressure_2'";
                    DataRow[] BP2Rows = ComplianceList.Select(conditionBP2);
                    if ((BP2Rows != null) && (BP2Rows.Length == 1))
                    {
                        if (BP2Rows[0]["Status"].ToString() == "1")
                        {
                            CacheSysList DiaList = PsVitalSigns.GetSignByDay(pclsCache, PatientId, "Bloodpressure", "Bloodpressure_2", Date);
                            if (DiaList != null)
                            {
                                mark = 1;
                                int BPTime1 = Convert.ToInt32(DiaList[1].ToString());
                                if (BPTime <= BPTime1)
                                {
                                    BPTime = BPTime1;
                                }
                                DiaValue = DiaList[2].ToString();
                            }
                        }
                    }

                    VitalTaskCom = new VitalTaskCom();
                    VitalTaskCom.SignName = "血压";
                    if (mark == 1)
                    {
                        VitalTaskCom.Status = "1";
                        VitalTaskCom.Time = PsVitalSigns.TransTime(BPTime.ToString());
                        VitalTaskCom.Value = SysValue + "/" + DiaValue;
                        VitalTaskCom.Unit = Unit;
                    }
                    else
                    {
                        VitalTaskCom.Status = "0";
                    }
                    VitalTaskComList.Add(VitalTaskCom);



                    //脉率任务可能没没有，需要确认
                    string conditionPR = " TaskCode = 'Pulserate|Pulserate_1'";
                    DataRow[] PulserateRows = ComplianceList.Select(conditionPR);

                    if ((PulserateRows != null) && (PulserateRows.Length == 1))
                    {
                        VitalTaskCom = new VitalTaskCom();
                        VitalTaskCom.SignName = "脉率";

                        if (PulserateRows[0]["Status"].ToString() == "1")
                        {
                            CacheSysList PulList = PsVitalSigns.GetSignByDay(pclsCache, PatientId, "Pulserate", "Pulserate_1", Date);
                            if (PulList != null)
                            {

                                VitalTaskCom.Status = "1";
                                VitalTaskCom.Time = PsVitalSigns.TransTime(PulList[1].ToString());
                                VitalTaskCom.Value = PulList[2].ToString();
                                VitalTaskCom.Unit = PulList[3].ToString();
                            }
                            else
                            {
                                VitalTaskCom.Status = "0";

                            }
                        }
                        else
                        {
                            VitalTaskCom.Status = "0";

                        }
                        VitalTaskComList.Add(VitalTaskCom);
                    }
                    #endregion
                }

                TaskComDetailByD.VitalTaskComList = VitalTaskComList;

                TaskComByType TaskComByType = new TaskComByType();
                List<TaskCom> TaskComList = new List<TaskCom>();
                TaskCom TaskCom = new TaskCom();

                //生活方式 
                string condition = " Type = 'LifeStyle'";
                DataRow[] LifeStyleRows = ComplianceList.Select(condition);

                if ((LifeStyleRows != null) && (LifeStyleRows.Length > 0))
                {
                    TaskComByType = new TaskComByType();
                    TaskComByType.TaskType = "生活方式";
                    TaskComList = new List<TaskCom>();
                    TaskCom = new TaskCom();

                    for (int j = 0; j < LifeStyleRows.Length; j++)
                    {
                        TaskCom = new TaskCom();
                        TaskCom.TaskName = LifeStyleRows[j]["TaskName"].ToString();
                        TaskCom.TaskStatus = LifeStyleRows[j]["Status"].ToString();
                        TaskComList.Add(TaskCom);
                    }
                    TaskComByType.TaskComList = TaskComList;
                    TaskComDetailByD.TaskComByTypeList.Add(TaskComByType);
                }

                //用药情况
                condition = " Type = 'Drug'";
                DataRow[] DrugRows = ComplianceList.Select(condition);

                if ((DrugRows != null) && (DrugRows.Length > 0))
                {
                    TaskComByType = new TaskComByType();
                    TaskComByType.TaskType = "用药情况";
                    TaskComList = new List<TaskCom>();
                    TaskCom = new TaskCom();
                    for (int j = 0; j < DrugRows.Length; j++)
                    {
                        TaskCom = new TaskCom();
                        TaskCom.TaskName = DrugRows[j]["TaskName"].ToString();
                        TaskCom.TaskStatus = DrugRows[j]["Status"].ToString();
                        TaskComList.Add(TaskCom);
                    }
                    TaskComByType.TaskComList = TaskComList;
                    TaskComDetailByD.TaskComByTypeList.Add(TaskComByType);
                }

                return TaskComDetailByD;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsCompliance.GetImplementationByDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }


        //GetTasksComByPeriod 其他任务的依从情况（不包括生理测量） LS 2010505  文本集中在ballon web暂用
        public static List<CompliacneDetailByD> GetTasksComByPeriodWeb(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {

            List<CompliacneDetailByD> resultList = new List<CompliacneDetailByD>();

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
            list.Columns.Add(new DataColumn("ComplianceValue", typeof(double)));
            list.Columns.Add(new DataColumn("TaskType", typeof(string)));
            list.Columns.Add(new DataColumn("TaskId", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(int)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetTasksComByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.Int).Value = EndDate;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    if (cdr["TaskType"].ToString() != "体征测量")
                    {
                        list.Rows.Add(cdr["Date"].ToString(), Convert.ToDouble(cdr["ComplianceValue"]), cdr["TaskType"].ToString(), cdr["TaskId"].ToString(), cdr["TaskName"].ToString(), Convert.ToInt32(cdr["Status"]));
                    }
                }

                //确保排序
                DataView dv = list.DefaultView;
                dv.Sort = "Date Asc, TaskType Asc, Status Asc";
                DataTable list_sort = dv.ToTable();
                list_sort.Rows.Add("end", 0, "", "", "", 0);  //用于最后一天输出

                if (list_sort.Rows.Count > 1)
                {
                    string temp_date = list_sort.Rows[0]["Date"].ToString();
                    string temp_type = list_sort.Rows[0]["TaskType"].ToString();
                    string temp_str = "";
                    temp_str += "该天依从率：" + list_sort.Rows[0]["ComplianceValue"].ToString() + "<br>";

                    if (list_sort.Rows[0]["TaskType"].ToString() == "生活方式")
                    { }
                    temp_str += "<b><span style='font-size:14px;'>" + list_sort.Rows[0]["TaskType"].ToString() + "：</span></b>";

                    CompliacneDetailByD CompliacneDetailByD = new CompliacneDetailByD();
                    CompliacneDetailByD.Date = list_sort.Rows[0]["Date"].ToString();
                    // CompliacneDetailByD.ComplianceValue = list_sort.Rows[0]["ComplianceValue"].ToString();

                    if (Convert.ToDouble(list_sort.Rows[0]["ComplianceValue"]) == 0)  //某天依从率
                    {
                        CompliacneDetailByD.drugBullet = "";
                        CompliacneDetailByD.drugColor = "#DADADA";
                    }
                    else if (Convert.ToDouble(list_sort.Rows[0]["ComplianceValue"]) == 1)
                    {
                        CompliacneDetailByD.drugBullet = "";
                        CompliacneDetailByD.drugColor = "#777777";
                    }
                    else
                    {
                        CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                        CompliacneDetailByD.drugColor = "";
                    }


                    if (Convert.ToInt32(list_sort.Rows[0]["Status"]) == 1)  //某天某项任务的完成情况
                    {
                        temp_str += list_sort.Rows[0]["TaskName"].ToString() + "complete  ";
                    }
                    else
                    {
                        //temp_str += list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  ";
                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  " + "</span></b>";
                    }


                    //只有一条数据
                    if (list_sort.Rows.Count == 2)
                    {
                        CompliacneDetailByD.Events = temp_str;
                        resultList.Add(CompliacneDetailByD);
                    }

                    //＞一条数据
                    if (list_sort.Rows.Count > 2)
                    {
                        for (int i = 1; i <= list_sort.Rows.Count - 1; i++)
                        {
                            if (temp_date == list_sort.Rows[i]["Date"].ToString())  //同一天
                            {
                                if (temp_type == list_sort.Rows[i]["TaskType"].ToString())     //同天同任务类型
                                {
                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        temp_str += list_sort.Rows[i]["TaskName"].ToString() + "complete  ";
                                    }
                                    else
                                    {
                                        //temp_str += list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  ";
                                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[i]["TaskName"].ToString() + "noncomplete " + "</span></b>";
                                    }
                                }
                                else   //同天不同任务类型
                                {
                                    temp_str += "<br>";
                                    temp_str += "<b><span style='font-size:14px;'>" + list_sort.Rows[i]["TaskType"].ToString() + "：</span></b>";

                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        temp_str += list_sort.Rows[i]["TaskName"].ToString() + "complete  ";
                                    }
                                    else
                                    {
                                        //temp_str += list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  ";
                                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  </span></b>";
                                    }


                                    temp_type = list_sort.Rows[i]["TaskType"].ToString();
                                }

                            }
                            else   //不同天
                            {
                                //上一天输出
                                CompliacneDetailByD.Events = temp_str;
                                resultList.Add(CompliacneDetailByD);

                                if (list_sort.Rows[i]["Date"].ToString() != "end")
                                {
                                    //获取新一天
                                    CompliacneDetailByD = new CompliacneDetailByD();
                                    CompliacneDetailByD.Date = list_sort.Rows[i]["Date"].ToString();
                                    //CompliacneDetailByD.ComplianceValue = list_sort.Rows[i]["ComplianceValue"].ToString();

                                    if (Convert.ToDouble(list_sort.Rows[i]["ComplianceValue"]) == 0)  //某天依从率
                                    {
                                        CompliacneDetailByD.drugBullet = "";
                                        CompliacneDetailByD.drugColor = "#DADADA";
                                    }
                                    else if (Convert.ToDouble(list_sort.Rows[i]["ComplianceValue"]) == 1)
                                    {
                                        CompliacneDetailByD.drugBullet = "";
                                        CompliacneDetailByD.drugColor = "#777777";
                                    }
                                    else
                                    {
                                        CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                                        CompliacneDetailByD.drugColor = "";
                                    }

                                    temp_str = "";
                                    temp_str += "该天依从率：" + list_sort.Rows[i]["ComplianceValue"].ToString() + "<br>";
                                    temp_str += "<b><span style='font-size:14px;'>" + list_sort.Rows[i]["TaskType"].ToString() + "：</span></b>";

                                    if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        temp_str += list_sort.Rows[i]["TaskName"].ToString() + "complete  ";
                                    }
                                    else
                                    {
                                        //temp_str += list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  ";
                                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  </span></b>";
                                    }

                                    temp_date = list_sort.Rows[i]["Date"].ToString();
                                    temp_type = list_sort.Rows[i]["TaskType"].ToString();
                                }
                            }
                        }

                    }

                }

                return resultList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTasksComByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //废弃，暂停使用 GetDrugComByPeriod 李山 2010505 药物的依从情况  phone版使用  
        public static List<CompliacneDetailByD> GetDrugComByPeriod(DataConnection pclsCache, string PatientId, string PlanNo, int StartDate, int EndDate)
        {


            List<CompliacneDetailByD> resultList = new List<CompliacneDetailByD>();

            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Date", typeof(string)));
            //list.Columns.Add(new DataColumn("ComplianceValue", typeof(double)));  药物的依从情况单独算出
            //list.Columns.Add(new DataColumn("TaskType", typeof(string)));   //只用药
            list.Columns.Add(new DataColumn("TaskId", typeof(string)));
            list.Columns.Add(new DataColumn("TaskName", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(int)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Compliance.GetDrugComByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;
                cmd.Parameters.Add("StartDate", CacheDbType.Int).Value = StartDate;
                cmd.Parameters.Add("EndDate", CacheDbType.Int).Value = EndDate;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())   //, cdr["TaskType"].ToString()
                {
                    list.Rows.Add(cdr["Date"].ToString(), cdr["TaskId"].ToString(), cdr["TaskName"].ToString(), Convert.ToInt32(cdr["Status"]));
                }

                //确保排序
                DataView dv = list.DefaultView;
                dv.Sort = "Date Asc, Status Asc";
                DataTable list_sort = dv.ToTable();
                list_sort.Rows.Add("end", "", "", 0);  //用于最后一天输出

                if (list_sort.Rows.Count > 1)
                {
                    string temp_date = list_sort.Rows[0]["Date"].ToString();
                    string temp_str = "";
                    //temp_str += "该天用药依从率："+ "<br>";
                    //temp_str += "<b><span style='font-size:14px;'>用药情况：</span></b><br>";
                    double count = 0;
                    double noncomplete = 0;

                    CompliacneDetailByD CompliacneDetailByD = new CompliacneDetailByD();
                    CompliacneDetailByD.Date = list_sort.Rows[0]["Date"].ToString();


                    if (Convert.ToInt32(list_sort.Rows[0]["Status"]) == 1)  //某天某项任务的完成情况
                    {
                        temp_str += list_sort.Rows[0]["TaskName"].ToString() + "complete  ";
                        count++;
                    }
                    else
                    {
                        //temp_str += list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  ";
                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  " + "</span></b>";
                        count++;
                        noncomplete++;
                    }


                    //只有一条数据
                    if (list_sort.Rows.Count == 2)
                    {
                        if ((noncomplete / count) == 1)          //某天药物依从率
                        {
                            //根本未完成
                            CompliacneDetailByD.drugBullet = "";
                            CompliacneDetailByD.drugColor = "#DADADA";
                        }
                        else if ((noncomplete / count) == 0)
                        {
                            //完成
                            CompliacneDetailByD.drugBullet = "";
                            CompliacneDetailByD.drugColor = "#777777";
                        }
                        else
                        {
                            CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                        }

                        CompliacneDetailByD.Events = temp_str;
                        resultList.Add(CompliacneDetailByD);
                    }

                    //＞一条数据
                    if (list_sort.Rows.Count > 2)
                    {
                        for (int i = 1; i <= list_sort.Rows.Count - 1; i++)
                        {
                            if (temp_date == list_sort.Rows[i]["Date"].ToString())  //同一天
                            {
                                if (Convert.ToInt32(list_sort.Rows[i]["Status"]) == 1)  //某天某项任务的完成情况
                                {
                                    temp_str += list_sort.Rows[i]["TaskName"].ToString() + "complete  ";
                                    count++;
                                }
                                else
                                {
                                    //temp_str += list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  ";
                                    temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  " + "</span></b>";
                                    count++;
                                    noncomplete++;
                                }

                            }
                            else   //不同天
                            {
                                //上一天输出
                                if ((noncomplete / count) == 1)          //某天药物依从率
                                {
                                    //根本未完成
                                    CompliacneDetailByD.drugBullet = "";
                                    CompliacneDetailByD.drugColor = "#DADADA";
                                }
                                else if ((noncomplete / count) == 0)
                                {
                                    //完成
                                    CompliacneDetailByD.drugBullet = "";
                                    CompliacneDetailByD.drugColor = "#777777";
                                }
                                else
                                {
                                    CompliacneDetailByD.drugBullet = "amcharts-images/drug.png";
                                }


                                CompliacneDetailByD.Events = temp_str;
                                resultList.Add(CompliacneDetailByD);

                                if (list_sort.Rows[i]["Date"].ToString() != "end")
                                {
                                    //获取新一天
                                    CompliacneDetailByD = new CompliacneDetailByD();
                                    CompliacneDetailByD.Date = list_sort.Rows[i]["Date"].ToString();

                                    temp_str = "";
                                    //temp_str += "该天用药依从率："+ "<br>";
                                    //temp_str += "<b><span style='font-size:14px;'>用药情况：</span></b><br>";
                                    count = 0;
                                    noncomplete = 0;

                                    if (Convert.ToInt32(list_sort.Rows[0]["Status"]) == 1)  //某天某项任务的完成情况
                                    {
                                        temp_str += list_sort.Rows[i]["TaskName"].ToString() + "complete  ";
                                        count++;
                                    }
                                    else
                                    {
                                        //temp_str += list_sort.Rows[0]["TaskName"].ToString() + "noncomplete  ";
                                        temp_str += "<b><span style='font-size:14px;color:red;'>" + list_sort.Rows[i]["TaskName"].ToString() + "noncomplete  " + "</span></b>";
                                        count++;
                                        noncomplete++;
                                    }

                                    temp_date = list_sort.Rows[i]["Date"].ToString();
                                }
                            }
                        }

                    }

                }

                return resultList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Compliance.GetTasksComByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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