using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsExamination
    {
        #region <" ZAM 2015-1-20 ">

        public static int DeleteData(DataConnection pclsCache, string piUserId, string piVisitId, int piSortNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Examination.DeleteData(pclsCache.CacheConnectionObject, piUserId, piVisitId, piSortNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int GetNextSortNo(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Examination.GetNextSortNo(pclsCache.CacheConnectionObject, piUserId, piVisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.GetNextSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int DeleteAll(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Examination.DeleteAll(pclsCache.CacheConnectionObject, piUserId, piVisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        #endregion

        #region <" CSQ 2015-3-11 ">

        public static int SetData(DataConnection pclsCache, string UserId, string VisitId, int SortNo, string ExamType, string ExamDate, string ItemCode, string ExamPara, string Description, string Impression, string Recommendation, int IsAbnormal, string Status, string ReportDate, string ImageURL, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }
                DateTime Exam_Date = Convert.ToDateTime(ExamDate);
                DateTime Report_Date = new DateTime();
                if (ReportDate != "")
                {
                    Report_Date = Convert.ToDateTime(ReportDate);
                }
                else
                {
                    Report_Date = Convert.ToDateTime("1900/01/01 0:00:00");
                }
                ret = (int)Ps.Examination.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, SortNo, ExamType, Exam_Date, ItemCode, ExamPara, Description, Impression, Recommendation, IsAbnormal, Status, Report_Date, ImageURL, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        
        //CSQ 2015-3-11
        public static DataTable GetExaminationList(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(string)));
            list.Columns.Add(new DataColumn("ExamType", typeof(string)));
            list.Columns.Add(new DataColumn("ExamTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("ExamDate", typeof(string)));
            list.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("ItemName", typeof(string)));
            list.Columns.Add(new DataColumn("ExamPara", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("Impression", typeof(string)));
            list.Columns.Add(new DataColumn("Recommendation", typeof(string)));
            list.Columns.Add(new DataColumn("IsAbnormalCode", typeof(string)));
            list.Columns.Add(new DataColumn("IsAbnormal", typeof(string)));
            list.Columns.Add(new DataColumn("StatusCode", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));
            list.Columns.Add(new DataColumn("ReportDate", typeof(string)));
            list.Columns.Add(new DataColumn("ImageURL", typeof(string)));
            list.Columns.Add(new DataColumn("DeptCode", typeof(string)));
            list.Columns.Add(new DataColumn("DeptName", typeof(string)));
            //list.Columns.Add(new DataColumn("ExamDateShow", typeof(string)));
            //list.Columns.Add(new DataColumn("ReportDateShow", typeof(string)));
            list.Columns.Add(new DataColumn("Creator", typeof(string)));
            list.Columns.Add(new DataColumn("ExamDateCom", typeof(string)));   //20150709 LS

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Examination.GetExaminationList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
                cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //string ExamDateShow = "", ReportDateShow = "";
                    //string str = cdr["ExamDate"].ToString();
                    //if (str != "0")
                    //{
                    //    ExamDateShow = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                    //}
                    //string str1 = cdr["ReportDate"].ToString();
                    //if (str1 != "0")
                    //{
                    //    ReportDateShow = str1.Substring(0, 4) + "-" + str1.Substring(4, 2) + "-" + str1.Substring(6, 2);
                    //}
                    string ReportDateShow = "";
                    if (cdr["ReportDate"].ToString() == "9999/1/1 0:00:00")
                    {
                        ReportDateShow = "";
                    }
                    else
                    {
                        ReportDateShow = cdr["ReportDate"].ToString();
                    }
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SortNo"].ToString(), cdr["ExamType"].ToString(), cdr["ExamTypeName"].ToString(), cdr["ExamDate"].ToString(), cdr["ItemCode"].ToString(), cdr["ItemName"].ToString(), cdr["ExamPara"].ToString(), cdr["Description"].ToString(), cdr["Impression"].ToString(), cdr["Recommendation"].ToString(), cdr["IsAbnormalCode"].ToString(), cdr["IsAbnormal"].ToString(), cdr["StatusCode"].ToString(), cdr["Status"].ToString(), ReportDateShow, cdr["ImageURL"].ToString(), cdr["DeptCode"].ToString(), cdr["DeptName"].ToString(), cdr["Creator"].ToString(),Convert.ToDateTime(cdr["ExamDate"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.GetExaminationList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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
        #endregion

        //CSQ 20150714
        public static DataTable GetNewExamForM1(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Name1", typeof(string)));
            list.Columns.Add(new DataColumn("Value1", typeof(string)));
            list.Columns.Add(new DataColumn("Name2", typeof(string)));
            list.Columns.Add(new DataColumn("Value2", typeof(string)));
            list.Columns.Add(new DataColumn("Name3", typeof(string)));
            list.Columns.Add(new DataColumn("Value3", typeof(string)));
            list.Columns.Add(new DataColumn("Date", typeof(DateTime)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Examination.GetNewExamForM1(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //string ExamDateShow = "", ReportDateShow = "";
                    //string str = cdr["ExamDate"].ToString();
                    //if (str != "0")
                    //{
                    //    ExamDateShow = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                    //}
                    //string str1 = cdr["ReportDate"].ToString();
                    //if (str1 != "0")
                    //{
                    //    ReportDateShow = str1.Substring(0, 4) + "-" + str1.Substring(4, 2) + "-" + str1.Substring(6, 2);
                    //}
                    if (cdr["Date"].ToString() != "")
                    {
                        list.Rows.Add(cdr["Name1"].ToString(), cdr["Value1"].ToString(), cdr["Name2"].ToString(), cdr["Value2"].ToString(), cdr["Name3"].ToString(), cdr["Value3"].ToString(), Convert.ToDateTime(cdr["Date"]));
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.GetNewExam", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //SYF 20150930 心力衰竭模块中获取患者最新检查信息（ECG）
        public static DataTable GetNewExamForM3(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.Examination.GetNewExamForM3(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["ItemCode"].ToString(), cdr["Value"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Examination.GetNewExamForM3", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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