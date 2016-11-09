using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsLabTest
    {
        #region <" ZAM 2015-1-20 ">

        public static int DeleteData(DataConnection pclsCache, string piUserId, string piVisitId, string piSortNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.LabTest.DeleteData(pclsCache.CacheConnectionObject, piUserId, piVisitId, piSortNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.LabTest.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

                ret = (int)Ps.LabTest.GetNextSortNo(pclsCache.CacheConnectionObject, piUserId, piVisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.LabTest.GetNextSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int DeleteAll(DataConnection pclsCache, string UserId, string VisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.LabTest.DeleteAll(pclsCache.CacheConnectionObject, UserId, VisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.LabTest.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        #endregion

        #region <" CSQ 2015-3-11 ">
        public static int SetData(DataConnection pclsCache, string UserId, string VisitId, string SortNo, string LabItemType, string LabItemCode, string LabTestDate, string Status, string ReportDate, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }
                DateTime LabTest_Date = Convert.ToDateTime(LabTestDate);
                DateTime Report_Date = new DateTime();
                if (ReportDate != "")
                {
                    Report_Date = Convert.ToDateTime(ReportDate);
                }
                else
                {
                    Report_Date = Convert.ToDateTime("1900/01/01 0:00:00");
                }
                ret = (int)Ps.LabTest.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, SortNo, LabItemType, LabItemCode, LabTest_Date, Status, Report_Date, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.LabTest.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        //CSQ 2015-3-11
        public static DataTable GetLabTestList(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(string)));
            list.Columns.Add(new DataColumn("LabItemType", typeof(string)));
            list.Columns.Add(new DataColumn("LabItemTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("LabItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("LabItemName", typeof(string)));
            list.Columns.Add(new DataColumn("LabTestDate", typeof(string)));
            list.Columns.Add(new DataColumn("StatusCode", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(string)));
            list.Columns.Add(new DataColumn("ReportDate", typeof(string)));
            list.Columns.Add(new DataColumn("DeptCode", typeof(string)));
            list.Columns.Add(new DataColumn("DeptName", typeof(string)));
            //list.Columns.Add(new DataColumn("LabTestDateShow", typeof(string)));
            //list.Columns.Add(new DataColumn("ReportDateShow", typeof(string)));
            list.Columns.Add(new DataColumn("Creator", typeof(string)));
            list.Columns.Add(new DataColumn("LabTestDateCom", typeof(string)));   //20150709 LS

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.LabTest.GetLabTestList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
                cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //string LabTestDateShow = "", ReportDateShow = "";
                    //string str = cdr["LabTestDate"].ToString();
                    //if (str != "0")
                    //{
                    //    LabTestDateShow = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
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
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["SortNo"].ToString(), cdr["LabItemType"].ToString(), cdr["LabItemTypeName"].ToString(), cdr["LabItemCode"].ToString(), cdr["LabItemName"].ToString(), cdr["LabTestDate"].ToString(), cdr["StatusCode"].ToString(), cdr["Status"].ToString(), ReportDateShow, cdr["DeptCode"].ToString(), cdr["DeptName"].ToString(), cdr["Creator"].ToString(), Convert.ToDateTime(cdr["LabTestDate"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.LabTest.GetLabTestList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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
    }
}