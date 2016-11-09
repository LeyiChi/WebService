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
    public class PsVitalSigns
    {
        //SetData ZC 2014-12-2
        public static bool SetData(DataConnection pclsCache, string UserId, int RecordDate, int RecordTime, string ItemType, string ItemCode, string Value, string Unit, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    return IsSaved;
                }
                int flag = (int)Ps.VitalSigns.SetData(pclsCache.CacheConnectionObject, UserId, RecordDate, RecordTime, ItemType, ItemCode, Value, Unit, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetPatientVitalSigns 获取患者某项生理参数的所有数据 ZC 2014-12-2
        public static DataTable GetPatientVitalSigns(DataConnection pclsCache, string UserId, string ItemType, string ItemCode)
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
                cmd = Ps.VitalSigns.GetPatientVitalSigns(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("piItemType", CacheDbType.NVarChar).Value = ItemType;
                cmd.Parameters.Add("piItemCode", CacheDbType.NVarChar).Value = ItemCode;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RecordDate"].ToString(), cdr["RecordTime"].ToString(), cdr["Value"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.GetPatientVitalSigns", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetPatientVitalSignsAndThreshold   LS  2014-12-4
        public static DataTable GetPatientVitalSignsAndThreshold(DataConnection pclsCache, string UserId, string ItemType, string ItemCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("RecordDate", typeof(int)));
            list.Columns.Add(new DataColumn("RecordTime", typeof(int)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));
            list.Columns.Add(new DataColumn("ThreholdMin", typeof(decimal)));
            list.Columns.Add(new DataColumn("ThreholdMax", typeof(decimal)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Ps.VitalSigns.GetPatientVitalSigns(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("piUserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("piItemType", CacheDbType.NVarChar).Value = ItemType;
                cmd.Parameters.Add("piItemCode", CacheDbType.NVarChar).Value = ItemCode;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int RecordDate = Convert.ToInt32(cdr["RecordDate"]);
                    int RecordTime = Convert.ToInt32(cdr["RecordTime"]);
                    string Value = cdr["Value"].ToString();
                    string Unit = cdr["Unit"].ToString();
                    InterSystems.Data.CacheTypes.CacheSysList Threholdlist = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                    Threholdlist = Wn.MstPersonalAlert.GetWnMstPersonalAlert(pclsCache.CacheConnectionObject, UserId, ItemCode, RecordDate);
                    //ZAM 2015-1-12
                    //if (Threholdlist == null)
                    if ((Threholdlist == null) || (Threholdlist.Count == 0))
                    {
                        Threholdlist = Wn.MstBasicAlert.GetWnMstBasicAlert(pclsCache.CacheConnectionObject, ItemCode);
                    }
                    if (Threholdlist != null)
                    {
                        list.Rows.Add(RecordDate, RecordTime, Value, Unit, Threholdlist[1], Threholdlist[2]);
                    }
                    //
                    //if (Threholdlist == null)
                    //{
                    //    Threholdlist = Wn.MstBasicAlert.GetWnMstBasicAlert(pclsCache.CacheConnectionObject, ItemCode);
                    //}
                    //list.Rows.Add(RecordDate, RecordTime, Value, Unit, Threholdlist[1], Threholdlist[2]);
                    //
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.GetPatientVitalSignsAndThreshold", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //RecordDisplay TDY 2015-04-07
        public static DataTable RecordDisplay(DataConnection pclsCache, string PatinetID, string ItemType, string ItemCode)
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
                cmd = Ps.VitalSigns.RecordDisplay(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatinetID", CacheDbType.NVarChar).Value = PatinetID;
                cmd.Parameters.Add("ItemType", CacheDbType.NVarChar).Value = ItemType;
                cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RecordDate"].ToString(), cdr["RecordTime"].ToString(), cdr["Value"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.RecordDisplay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetSignByDay TDY 2015-04-07
        public static CacheSysList GetSignByDay(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int RecordDate)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.VitalSigns.GetSignByDay(pclsCache.CacheConnectionObject, UserId, ItemType, ItemCode, RecordDate);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetSignByDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetSignDetailByDay TDY 2015-04-07
        public static DataTable GetSignDetailByDay(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int RecordDate)
        {
            DataTable list = new DataTable();
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
                cmd = Ps.VitalSigns.GetSignDetailByDay(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("ItemType", CacheDbType.NVarChar).Value = ItemType;
                cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;
                cmd.Parameters.Add("RecordDate", CacheDbType.Int).Value = RecordDate;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["RecordTime"].ToString(), cdr["Value"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetSignDetailByDay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetSignByPeriod LS 2015-03-30  只针对一种参数  
        public static DataTable GetSignByPeriod(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int StartDate, int EndDate)
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
                cmd = Ps.VitalSigns.GetSignByPeriod(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
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
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.GetSignByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetLatestPatientVitalSigns SYF 2015-04-15 获取病人最新体征情况
        public static string GetLatestPatientVitalSigns(DataConnection pclsCache, string UserId, string ItemType, string ItemCode)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Ps.VitalSigns.GetLatestPatientVitalSigns(pclsCache.CacheConnectionObject, UserId, ItemType, ItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetLatestPatientVitalSigns", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // ZAM 2015-4-23 获取患者某项生理参数的RecordDate前的最近一条数据
        public static CacheSysList GetLatestVitalSignsByDate(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int RecordDate)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.VitalSigns.GetLatestVitalSignsByDate(pclsCache.CacheConnectionObject, UserId, ItemType, ItemCode, RecordDate);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetLatestVitalSignsByDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetSignDetailByDay LS 2015-03-27  只针对一种参数  血压包括收缩压、舒张压，量参数应一块显示  暂时没用到!
        public static DataTable GetSignDetailByPeriod(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int StartDate, int EndDate)
        {

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
                    cmd = Ps.VitalSigns.GetSignDetailByPeriod(pclsCache.CacheConnectionObject);
                    cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                    cmd.Parameters.Add("ItemType", CacheDbType.NVarChar).Value = ItemType;
                    cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;
                    cmd.Parameters.Add("StartDate", CacheDbType.NVarChar).Value = StartDate;
                    cmd.Parameters.Add("EndDate", CacheDbType.NVarChar).Value = EndDate;

                    cdr = cmd.ExecuteReader();
                    while (cdr.Read())
                    {
                        string RecordDate = cdr["RecordDate"].ToString();
                        RecordDate = RecordDate.Substring(0, 4) + "-" + RecordDate.Substring(4, 2) + "-" + RecordDate.Substring(6, 2);
                        //RecordDate.Insert(4, "-") + RecordDate.Insert(6, "-");
                        string RecordTime = PsVitalSigns.TransTime(cdr["RecordTime"].ToString());
                        //RecordTime = RecordTime.Substring(0, 2) + "：" + RecordTime.Substring(2, 2);
                        list.Rows.Add(RecordDate, RecordTime, cdr["Value"].ToString(), cdr["Unit"].ToString());
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.GetSignByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //以下是任务完成情况用到的函数

        //CaculateWeekDay LS //不明位数时分秒
        public static string TransTime(string time)
        {
            int length = time.Length;
            string result = "";
            try
            {
                switch (length)
                {
                    case 1:
                        result = "00：0" + time;
                        break;
                    case 2:
                        result = "00：" + time;
                        break;
                    case 3:
                        result = "0" + time.Substring(0, 1) + "：" + time.Substring(1, 2);
                        break;
                    case 4:
                        result = time.Substring(0, 2) + "：" + time.Substring(2, 2);  //Substring(起始, 截取长度)
                        break;
                    default: break;
                }

                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.TransTime", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //从数据库拿出相关数据 并增加type字段 区别收缩压、舒张压、脉率  用于phone 体征详细时刻表
        public static DataTable GetTypedSignDetailByPeriod(DataConnection pclsCache, string UserId, string ItemType, string ItemCode, int StartDate, int EndDate)
        {

            {

                DataTable list = new DataTable();
                list.Columns.Add(new DataColumn("SignType", typeof(string)));
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
                    cmd = Ps.VitalSigns.GetSignDetailByPeriod(pclsCache.CacheConnectionObject);
                    cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                    cmd.Parameters.Add("ItemType", CacheDbType.NVarChar).Value = ItemType;
                    cmd.Parameters.Add("ItemCode", CacheDbType.NVarChar).Value = ItemCode;
                    cmd.Parameters.Add("StartDate", CacheDbType.NVarChar).Value = StartDate;
                    cmd.Parameters.Add("EndDate", CacheDbType.NVarChar).Value = EndDate;

                    cdr = cmd.ExecuteReader();
                    while (cdr.Read())
                    {
                        string RecordDate = cdr["RecordDate"].ToString();
                        RecordDate = RecordDate.Substring(0, 4) + "-" + RecordDate.Substring(4, 2) + "-" + RecordDate.Substring(6, 2);
                        //RecordDate.Insert(4, "-") + RecordDate.Insert(6, "-");
                        string RecordTime = PsVitalSigns.TransTime(cdr["RecordTime"].ToString());
                        //RecordTime = RecordTime.Substring(0, 2) + "：" + RecordTime.Substring(2, 2);

                        //list.Rows.Add("收缩压", RecordDate, RecordTime, cdr["Value"].ToString(), cdr["Unit"].ToString());
                        if (ItemCode == "Bloodpressure_1")
                        {
                            list.Rows.Add("1", RecordDate, RecordTime, cdr["Value"].ToString(), cdr["Unit"].ToString());
                        }
                        else if (ItemCode == "Bloodpressure_2")  //舒张压
                        {
                            list.Rows.Add("2", RecordDate, RecordTime, cdr["Value"].ToString(), cdr["Unit"].ToString());
                        }
                        else   //脉率"Pulserate_1"
                        {
                            list.Rows.Add("3", RecordDate, RecordTime, cdr["Value"].ToString(), cdr["Unit"].ToString());
                        }
                    }

                    //DataView dv = list.DefaultView;
                    //dv.Sort = "RecordDate desc, RecordTime asc";
                    //DataTable dt_Sort = dv.ToTable();

                    return list;
                }
                catch (Exception ex)
                {
                    HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsVitalSigns.GetTypedSignDetailByPeriod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // LS 2015-07-02 获取患者某项生理参数的最近一条数据的记录日期
        public static string GetLatestVitalSignDate(DataConnection pclsCache, string UserId, int date)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Ps.VitalSigns.GetLatestVitalSignDate(pclsCache.CacheConnectionObject, UserId, date);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetLatestVitalSignDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // LS 2015-07-02 获取某日期之后的有数据的起止时间
        public static CacheSysList GetVitalSignDates(DataConnection pclsCache, string UserId, int date, int Num)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.VitalSigns.GetVitalSignDates(pclsCache.CacheConnectionObject, UserId, date, Num);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.VitalSigns.GetVitalSignDates", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}