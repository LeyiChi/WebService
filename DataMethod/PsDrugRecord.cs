using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsDrugRecord
    {
        #region <" ZAM 2015-1-20 ">

        public static int SetData(DataConnection pclsCache, string UserId, string VisitId, int OrderNo, int OrderSubNo, int RepeatIndicator, string OrderClass, string OrderCode, string OrderContent, decimal Dosage, string DosageUnits, string Administration, DateTime StartDateTime, DateTime StopDateTime, int Duration, string DurationUnits, string Frequency, int FreqCounter, int FreqInteval, string FreqIntevalUnit, string DeptCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.DrugRecord.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, OrderNo, OrderSubNo, RepeatIndicator, OrderClass, OrderCode, OrderContent, Dosage, DosageUnits, Administration, StartDateTime, StopDateTime, Duration, DurationUnits, Frequency, FreqCounter, FreqInteval, FreqIntevalUnit, DeptCode, revUserId, TerminalName, TerminalIP, DeviceType);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //CSQ 2015-1-21
        public static DataTable GetDrugRecordList(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("OrderNo", typeof(string)));
            list.Columns.Add(new DataColumn("OrderSubNo", typeof(string)));
            list.Columns.Add(new DataColumn("RepeatIndicatorCode", typeof(string)));
            list.Columns.Add(new DataColumn("RepeatIndicator", typeof(string)));
            list.Columns.Add(new DataColumn("OrderClassCode", typeof(string)));
            list.Columns.Add(new DataColumn("OrderClass", typeof(string)));
            list.Columns.Add(new DataColumn("OrderCode", typeof(string)));
            list.Columns.Add(new DataColumn("OrderContent", typeof(string)));
            list.Columns.Add(new DataColumn("Dosage", typeof(string)));
            list.Columns.Add(new DataColumn("DosageUnitsCode", typeof(string)));
            list.Columns.Add(new DataColumn("DosageUnits", typeof(string)));
            list.Columns.Add(new DataColumn("AdministrationCode", typeof(string)));
            list.Columns.Add(new DataColumn("Administration", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("StopDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("Frequency", typeof(string)));
            list.Columns.Add(new DataColumn("FreqCounter", typeof(string)));
            list.Columns.Add(new DataColumn("FreqInteval", typeof(string)));
            list.Columns.Add(new DataColumn("FreqIntevalUnitCode", typeof(string)));
            list.Columns.Add(new DataColumn("FreqIntevalUnit", typeof(string)));
            list.Columns.Add(new DataColumn("DeptCode", typeof(string)));
            list.Columns.Add(new DataColumn("DeptName", typeof(string)));
            list.Columns.Add(new DataColumn("Creator", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTimeCom", typeof(string))); 

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.DrugRecord.GetDrugRecordList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
                cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    string startTime = "";
                    if (cdr["StartDateTime"].ToString() != null && cdr["StartDateTime"].ToString() != "")
                    {
                        startTime = Convert.ToDateTime(cdr["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    string endTime = "";
                    if (cdr["StopDateTime"].ToString() != null && cdr["StopDateTime"].ToString() != "")
                    {
                        endTime = Convert.ToDateTime(cdr["StopDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["OrderNo"].ToString(), cdr["OrderSubNo"].ToString(), cdr["RepeatIndicatorCode"].ToString()
                        , cdr["RepeatIndicator"].ToString(), cdr["OrderClassCode"].ToString(), cdr["OrderClass"].ToString(), cdr["OrderCode"].ToString()
                        , cdr["OrderContent"].ToString(), cdr["Dosage"].ToString(), cdr["DosageUnitsCode"].ToString(), cdr["DosageUnits"].ToString()
                        , cdr["AdministrationCode"].ToString(), cdr["Administration"].ToString(), startTime, endTime, cdr["Frequency"].ToString(), cdr["FreqCounter"].ToString()
                        , cdr["FreqInteval"].ToString(), cdr["FreqIntevalUnitCode"].ToString(), cdr["FreqIntevalUnit"].ToString(), cdr["DeptCode"].ToString(), cdr["DeptName"].ToString(), cdr["Creator"].ToString(), Convert.ToDateTime(cdr["StartDateTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.GetDrugRecordList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static int DeleteData(DataConnection pclsCache, string piUserId, string piVisitId, int piOrderNo, int piOrderSubNo)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.DrugRecord.DeleteData(pclsCache.CacheConnectionObject, piUserId, piVisitId, piOrderNo, piOrderSubNo);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int GetNextOrderNo(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.DrugRecord.GetNextOrderNo(pclsCache.CacheConnectionObject, piUserId, piVisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.GetNextOrderNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

                ret = (int)Ps.DrugRecord.DeleteAll(pclsCache.CacheConnectionObject, UserId, VisitId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        ////LS 2015-1-21
        //public static DataTable GetDrugRecord(DataConnection pclsCache, string piUserId, string piVisitId)
        //{
        //    DataTable list = new DataTable();
        //    list.Columns.Add(new DataColumn("VisitId", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderNo", typeof(int)));
        //    list.Columns.Add(new DataColumn("OrderSubNo", typeof(int)));
        //    list.Columns.Add(new DataColumn("RepeatIndicatorCode", typeof(int)));
        //    list.Columns.Add(new DataColumn("RepeatIndicator", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderClassCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderClass", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderContent", typeof(string)));
        //    list.Columns.Add(new DataColumn("Dosage", typeof(int)));
        //    list.Columns.Add(new DataColumn("DosageUnitsCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("DosageUnits", typeof(string)));
        //    list.Columns.Add(new DataColumn("AdministrationCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("Administration", typeof(string)));
        //    list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
        //    list.Columns.Add(new DataColumn("StopDateTime", typeof(string)));
        //    list.Columns.Add(new DataColumn("Frequency", typeof(string)));
        //    list.Columns.Add(new DataColumn("FreqCounter", typeof(int)));
        //    list.Columns.Add(new DataColumn("FreqInteval", typeof(int)));
        //    list.Columns.Add(new DataColumn("FreqIntevalUnitCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("FreqIntevalUnit", typeof(string)));
        //    list.Columns.Add(new DataColumn("HistoryContent", typeof(string)));
        //    list.Columns.Add(new DataColumn("StartDate", typeof(string)));
        //    list.Columns.Add(new DataColumn("StopDate", typeof(string)));
        //    CacheCommand cmd = null;
        //    CacheDataReader cdr = null;

        //    try
        //    {
        //        if (!pclsCache.Connect())
        //        {
        //            return null;
        //        }
        //        cmd = new CacheCommand();
        //        cmd = Ps.DrugRecord.GetDrugRecordList(pclsCache.CacheConnectionObject);
        //        cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
        //        cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;
        //        cdr = cmd.ExecuteReader();

        //        while (cdr.Read())
        //        {
        //            int OrderSubNo = 0;
        //            int Dosage = 0;
        //            int FreqCounter = 0;
        //            int FreqInteval = 0;
        //            if (cdr["OrderSubNo"].ToString() != "")
        //            {
        //                OrderSubNo = Convert.ToInt32(cdr["OrderSubNo"].ToString());
        //            }

        //            if (OrderSubNo == 1)       //只拿出OrderSubNo为1的记录
        //            {

        //                if (cdr["Dosage"].ToString() != "")
        //                {
        //                    Dosage = Convert.ToInt32(cdr["Dosage"].ToString());
        //                }
        //                if (cdr["FreqCounter"].ToString() != "")
        //                {
        //                    FreqCounter = Convert.ToInt32(cdr["FreqCounter"].ToString());
        //                }
        //                if (cdr["FreqInteval"].ToString() != "")
        //                {
        //                    FreqInteval = Convert.ToInt32(cdr["FreqInteval"].ToString());
        //                }

        //                //用药开始时间处理（时间轴标准）
        //                string StartDate = DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyyMMdd");
        //                int iStartDate = Convert.ToInt32(StartDate);

        //                //用药结束时间处理
        //                string EndDate = DateTime.Parse(cdr["StopDateTime"].ToString()).ToString("yyyyMMdd");
        //                int iEndDate = Convert.ToInt32(EndDate);
        //                int iDays = iEndDate - iStartDate;

        //                string Content = "";
        //                Content += cdr["OrderContent"].ToString();

        //                if (cdr["RepeatIndicatorCode"].ToString() == "1")    //医嘱：长期几天以上？
        //                {
        //                    Content += "（长期";
        //                }
        //                else
        //                {
        //                    Content += "（临时";
        //                }

        //                if (iDays == 0)     //计算用药时长  应该数据库，或者webservice完成
        //                {
        //                    Content += "，1天）";
        //                }
        //                else if (iDays > 0)
        //                {
        //                    string interval = (iDays).ToString();
        //                    Content += "，";
        //                    Content += interval;
        //                    Content += "天）";
        //                }
        //                else
        //                {
        //                    Content += "）";
        //                }

        //                list.Rows.Add(cdr["VisitId"].ToString(), Convert.ToInt32(cdr["OrderNo"].ToString()), OrderSubNo, Convert.ToInt32(cdr["RepeatIndicatorCode"].ToString()), cdr["RepeatIndicator"].ToString(),
        //                    cdr["OrderClassCode"].ToString(), cdr["OrderClass"].ToString(), cdr["OrderCode"].ToString(), cdr["OrderContent"].ToString(),
        //                    Dosage, cdr["DosageUnitsCode"].ToString(), cdr["DosageUnits"].ToString(), cdr["AdministrationCode"].ToString(),
        //                    cdr["Administration"].ToString(), DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Parse(cdr["StopDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), cdr["Frequency"].ToString(),
        //                    FreqCounter, FreqInteval, cdr["FreqIntevalUnitCode"].ToString(), cdr["FreqIntevalUnit"].ToString(), Content, StartDate, EndDate);
        //            }
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDrugRecord.GetDrugRecordList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //    }
        //    finally
        //    {
        //        if ((cdr != null))
        //        {
        //            cdr.Close();
        //            cdr.Dispose(true);
        //            cdr = null;
        //        }
        //        if ((cmd != null))
        //        {
        //            cmd.Parameters.Clear();
        //            cmd.Dispose();
        //            cmd = null;
        //        }
        //        pclsCache.DisConnect();
        //    }
        //}

        //20150618 CSQ
        //public static DataTable GetDrugRecord(DataConnection pclsCache, string piUserId, string piVisitId)
        //{
        //    DataTable list = new DataTable();
        //    list.Columns.Add(new DataColumn("VisitId", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderNo", typeof(int)));
        //    list.Columns.Add(new DataColumn("OrderSubNo", typeof(int)));
        //    list.Columns.Add(new DataColumn("RepeatIndicatorCode", typeof(int)));
        //    list.Columns.Add(new DataColumn("RepeatIndicator", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderClassCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderClass", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("OrderContent", typeof(string)));
        //    list.Columns.Add(new DataColumn("Dosage", typeof(string)));
        //    list.Columns.Add(new DataColumn("DosageUnitsCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("DosageUnits", typeof(string)));
        //    list.Columns.Add(new DataColumn("AdministrationCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("Administration", typeof(string)));
        //    list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
        //    list.Columns.Add(new DataColumn("StopDateTime", typeof(string)));
        //    list.Columns.Add(new DataColumn("Frequency", typeof(string)));
        //    list.Columns.Add(new DataColumn("FreqCounter", typeof(int)));
        //    list.Columns.Add(new DataColumn("FreqInteval", typeof(int)));
        //    list.Columns.Add(new DataColumn("FreqIntevalUnitCode", typeof(string)));
        //    list.Columns.Add(new DataColumn("FreqIntevalUnit", typeof(string)));
        //    list.Columns.Add(new DataColumn("HistoryContent", typeof(string)));
        //    list.Columns.Add(new DataColumn("StartDate", typeof(string)));
        //    list.Columns.Add(new DataColumn("StopDate", typeof(string)));
        //    CacheCommand cmd = null;
        //    CacheDataReader cdr = null;

        //    try
        //    {
        //        if (!pclsCache.Connect())
        //        {
        //            return null;
        //        }
        //        cmd = new CacheCommand();
        //        cmd = Ps.DrugRecord.GetDrugRecordList(pclsCache.CacheConnectionObject);
        //        cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
        //        cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;
        //        cdr = cmd.ExecuteReader();

        //        while (cdr.Read())
        //        {
        //            int OrderSubNo = 0;
        //            int Dosage = 0;
        //            int FreqCounter = 0;
        //            int FreqInteval = 0;
        //            if (cdr["OrderSubNo"].ToString() != "")
        //            {
        //                OrderSubNo = Convert.ToInt32(cdr["OrderSubNo"].ToString());
        //            }

        //            if (OrderSubNo == 1)       //只拿出OrderSubNo为1的记录
        //            {

        //                if (cdr["Dosage"].ToString() != "")
        //                {
        //                    Dosage = Convert.ToInt32(cdr["Dosage"].ToString());
        //                }
        //                if (cdr["FreqCounter"].ToString() != "")
        //                {
        //                    FreqCounter = Convert.ToInt32(cdr["FreqCounter"].ToString());
        //                }
        //                if (cdr["FreqInteval"].ToString() != "")
        //                {
        //                    FreqInteval = Convert.ToInt32(cdr["FreqInteval"].ToString());
        //                }

        //                //用药开始时间处理（时间轴标准）
        //                string StartDate = DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyyMMdd");
        //                //int iStartDate = Convert.ToInt32(StartDate);

        //                //用药结束时间处理
        //                string EndDate = cdr["StopDateTime"].ToString();
        //                // int iEndDate = Convert.ToInt32(EndDate);
        //                // int iDays = iEndDate - iStartDate;

        //                string Content = "";
        //                Content += cdr["OrderContent"].ToString();
        //                //Content += cdr["RepeatIndicator"].ToString();
        //                /*
        //                if (cdr["RepeatIndicatorCode"].ToString() == "1")    //医嘱：长期几天以上？
        //                {
        //                    Content += "（长期)";
        //                }
        //                else
        //                {
        //                    Content += "（临时)";
        //                }
                        
        //                if (iDays == 0)     //计算用药时长  应该数据库，或者webservice完成
        //                {
        //                    Content += "，1天）";
        //                }
        //                else if (iDays > 0)
        //                {
        //                    string interval = (iDays).ToString();
        //                    Content += "，";
        //                    Content += interval;
        //                    Content += "天）";
        //                }
        //                else
        //                {
        //                    Content += "）";
        //                }
        //                */
        //                list.Rows.Add(cdr["VisitId"].ToString(), Convert.ToInt32(cdr["OrderNo"].ToString()), OrderSubNo, Convert.ToInt32(cdr["RepeatIndicatorCode"].ToString()), cdr["RepeatIndicator"].ToString(),
        //                    cdr["OrderClassCode"].ToString(), cdr["OrderClass"].ToString(), cdr["OrderCode"].ToString(), cdr["OrderContent"].ToString(),
        //                    Dosage, cdr["DosageUnitsCode"].ToString(), cdr["DosageUnits"].ToString(), cdr["AdministrationCode"].ToString(),
        //                    cdr["Administration"].ToString(), DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), cdr["StopDateTime"].ToString(), cdr["Frequency"].ToString(),
        //                    FreqCounter, FreqInteval, cdr["FreqIntevalUnitCode"].ToString(), cdr["FreqIntevalUnit"].ToString(), Content, StartDate, EndDate);
        //            }
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDrugRecord.GetDrugRecordList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return null;
        //    }
        //    finally
        //    {
        //        if ((cdr != null))
        //        {
        //            cdr.Close();
        //            cdr.Dispose(true);
        //            cdr = null;
        //        }
        //        if ((cmd != null))
        //        {
        //            cmd.Parameters.Clear();
        //            cmd.Dispose();
        //            cmd = null;
        //        }
        //        pclsCache.DisConnect();
        //    }
        //}

        public static DataTable GetDrugRecord(DataConnection pclsCache, string piUserId, string piVisitId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("OrderNo", typeof(int)));
            list.Columns.Add(new DataColumn("OrderSubNo", typeof(int)));
            list.Columns.Add(new DataColumn("RepeatIndicatorCode", typeof(int)));
            list.Columns.Add(new DataColumn("RepeatIndicator", typeof(string)));
            list.Columns.Add(new DataColumn("OrderClassCode", typeof(string)));
            list.Columns.Add(new DataColumn("OrderClass", typeof(string)));
            list.Columns.Add(new DataColumn("OrderCode", typeof(string)));
            list.Columns.Add(new DataColumn("OrderContent", typeof(string)));
            //list.Columns.Add(new DataColumn("Dosage", typeof(int)));
            list.Columns.Add(new DataColumn("Dosage", typeof(string)));
            list.Columns.Add(new DataColumn("DosageUnitsCode", typeof(string)));
            list.Columns.Add(new DataColumn("DosageUnits", typeof(string)));
            list.Columns.Add(new DataColumn("AdministrationCode", typeof(string)));
            list.Columns.Add(new DataColumn("Administration", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("StopDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("Frequency", typeof(string)));
            list.Columns.Add(new DataColumn("FreqCounter", typeof(int)));
            list.Columns.Add(new DataColumn("FreqInteval", typeof(int)));
            list.Columns.Add(new DataColumn("FreqIntevalUnitCode", typeof(string)));
            list.Columns.Add(new DataColumn("FreqIntevalUnit", typeof(string)));
            list.Columns.Add(new DataColumn("HistoryContent", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(string)));
            list.Columns.Add(new DataColumn("StopDate", typeof(string)));
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.DrugRecord.GetDrugRecordList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
                cmd.Parameters.Add("piVisitId", CacheDbType.NVarChar).Value = piVisitId;
                cdr = cmd.ExecuteReader();

                while (cdr.Read())
                {
                    int OrderSubNo = 0;
                    //int Dosage = 0;
                    int FreqCounter = 0;
                    int FreqInteval = 0;
                    if (cdr["OrderSubNo"].ToString() != "")
                    {
                        OrderSubNo = Convert.ToInt32(cdr["OrderSubNo"].ToString());
                    }

                    if (OrderSubNo == 1)       //只拿出OrderSubNo为1的记录
                    {
                        /*
                        if (cdr["Dosage"].ToString() != "")
                        {
                            Dosage = Convert.ToInt32(cdr["Dosage"].ToString());
                        }
                         */
                        if (cdr["FreqCounter"].ToString() != "")
                        {
                            FreqCounter = Convert.ToInt32(cdr["FreqCounter"].ToString());
                        }
                        if (cdr["FreqInteval"].ToString() != "")
                        {
                            FreqInteval = Convert.ToInt32(cdr["FreqInteval"].ToString());
                        }

                        //用药开始时间处理（时间轴标准）
                        string StartDate = DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyyMMdd");
                        //int iStartDate = Convert.ToInt32(StartDate);

                        //用药结束时间处理
                        string EndDate = cdr["StopDateTime"].ToString();
                        // int iEndDate = Convert.ToInt32(EndDate);
                        // int iDays = iEndDate - iStartDate;

                        string Content = "";
                        Content += cdr["OrderContent"].ToString();
                        //Content += cdr["RepeatIndicator"].ToString();
                        /*
                        if (cdr["RepeatIndicatorCode"].ToString() == "1")    //医嘱：长期几天以上？
                        {
                            Content += "（长期)";
                        }
                        else
                        {
                            Content += "（临时)";
                        }
                        
                        if (iDays == 0)     //计算用药时长  应该数据库，或者webservice完成
                        {
                            Content += "，1天）";
                        }
                        else if (iDays > 0)
                        {
                            string interval = (iDays).ToString();
                            Content += "，";
                            Content += interval;
                            Content += "天）";
                        }
                        else
                        {
                            Content += "）";
                        }
                        */
                        list.Rows.Add(cdr["VisitId"].ToString(), Convert.ToInt32(cdr["OrderNo"].ToString()), OrderSubNo, Convert.ToInt32(cdr["RepeatIndicatorCode"].ToString()), cdr["RepeatIndicator"].ToString(),
                            cdr["OrderClassCode"].ToString(), cdr["OrderClass"].ToString(), cdr["OrderCode"].ToString(), cdr["OrderContent"].ToString(),
                            cdr["Dosage"].ToString(), cdr["DosageUnitsCode"].ToString(), cdr["DosageUnits"].ToString(), cdr["AdministrationCode"].ToString(),
                            cdr["Administration"].ToString(), DateTime.Parse(cdr["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), cdr["StopDateTime"].ToString(), cdr["Frequency"].ToString(),
                            FreqCounter, FreqInteval, cdr["FreqIntevalUnitCode"].ToString(), cdr["FreqIntevalUnit"].ToString(), Content, StartDate, EndDate);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDrugRecord.GetDrugRecordList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //ZC 2015-04-17 获取某病人所有药嘱
        public static DataTable GetPsDrugRecord(DataConnection pclsCache, string piUserId, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(string)));
            list.Columns.Add(new DataColumn("OrderNo", typeof(int)));
            list.Columns.Add(new DataColumn("OrderSubNo", typeof(int)));
            list.Columns.Add(new DataColumn("RepeatIndicator", typeof(string)));
            list.Columns.Add(new DataColumn("OrderClass", typeof(string)));
            list.Columns.Add(new DataColumn("OrderCode", typeof(string)));
            list.Columns.Add(new DataColumn("DrugName", typeof(string)));
            list.Columns.Add(new DataColumn("CurativeEffect", typeof(string)));
            list.Columns.Add(new DataColumn("SideEffect", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
            list.Columns.Add(new DataColumn("HealthEffect", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));
            list.Columns.Add(new DataColumn("OrderContent", typeof(string)));
            list.Columns.Add(new DataColumn("Dosage", typeof(string)));
            list.Columns.Add(new DataColumn("DosageUnits", typeof(string)));
            list.Columns.Add(new DataColumn("Administration", typeof(string)));
            list.Columns.Add(new DataColumn("StartDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("StopDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("Frequency", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.DrugRecord.GetPsDrugRecord(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = piUserId;
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["VisitId"].ToString(), cdr["OrderNo"].ToString(), cdr["OrderSubNo"].ToString(), cdr["RepeatIndicator"].ToString(), cdr["OrderClass"].ToString(), cdr["OrderCode"].ToString(), cdr["DrugName"].ToString(), cdr["CurativeEffect"].ToString(), cdr["SideEffect"].ToString(), cdr["Instruction"].ToString(), cdr["HealthEffect"].ToString(), cdr["Unit"].ToString(), cdr["OrderContent"].ToString(), cdr["Dosage"].ToString(), cdr["DosageUnits"].ToString(), cdr["Administration"].ToString(), cdr["StartDateTime"].ToString(), cdr["StopDateTime"].ToString(), cdr["Frequency"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DrugRecord.GetPsDrugRecord", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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