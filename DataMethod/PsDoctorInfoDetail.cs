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
    public class PsDoctorInfoDetail
    {
        //SetData  LS 2014-12-1
        public static bool SetData(DataConnection pclsCache, string Doctor, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.DoctorInfoDetail.SetData(pclsCache.CacheConnectionObject, Doctor, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetMaxItemSeq LS 2014-12-1
        public static int GetMaxItemSeq(DataConnection pclsCache, string DoctorId, string CategoryCode, string ItemCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.DoctorInfoDetail.GetMaxItemSeq(pclsCache.CacheConnectionObject, DoctorId, CategoryCode, ItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.GetMaxItemSeq", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetPatientsByDoctorId   LS 2014-12-1
        public static DataTable GetPatientsByDoctorId(DataConnection pclsCache, string DoctorId, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("PatientId", typeof(string)));
            list.Columns.Add(new DataColumn("PatientName", typeof(string)));

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
                cmd = Ps.DoctorInfoDetail.GetPatientsByDoctorId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["PatientId"].ToString(), cdr["PatientName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.GetPatientsByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData CSQ 2015-01-06
        public static int DeleteData(DataConnection pclsCache, string DoctorId, string CategoryCode, string Value)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.DoctorInfoDetail.DeleteData(pclsCache.CacheConnectionObject, DoctorId, CategoryCode, Value);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDoctorListByModule   CSQ 2015-01-07
        public static DataTable GetDoctorListByModule(DataConnection pclsCache,  string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));

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
                cmd = Ps.DoctorInfoDetail.GetDoctorListByModule(pclsCache.CacheConnectionObject);             
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.GetDoctorListByModule", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetDoctorDetailInfo TDY 2015-01-12
        public static CacheSysList GetDoctorDetailInfo(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.DoctorInfoDetail.GetDoctorDetailInfo(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.GetDoctorDetailInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //CheckDoctorPhoneNumber TDY 2015-1-14
        public static int CheckDoctorPhoneNumber(DataConnection pclsCache, string UserId, string piPhoneNumber)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.DoctorInfoDetail.CheckDoctorPhoneNumber(pclsCache.CacheConnectionObject, UserId, piPhoneNumber);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.CheckDoctorPhoneNumber", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDoctorInfoDetail ZCY 2015-04-29
        public static CacheSysList GetDoctorInfoDetail(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.DoctorInfoDetail.GetDoctorInfoDetail(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfoDetail.GetDoctorInfoDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }


        // 获取需要知道的项目的值 ZC 20150603
        public static string GetValue(DataConnection pclsCache, string UserId, string CategoryCode, string ItemCode, int ItemSeq)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (string)Ps.DoctorInfoDetail.GetValue(pclsCache.CacheConnectionObject, UserId, CategoryCode, ItemCode, ItemSeq);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetValue", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetPatientsMatchByDoctorId   CSQ 20160114
        public static DataTable GetPatientsMatchByDoctorId(DataConnection pclsCache, string DoctorId,string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("PatientId", typeof(string)));
            list.Columns.Add(new DataColumn("PatientName", typeof(string)));
            list.Columns.Add(new DataColumn("HUserId", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
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
                cmd = Ps.DoctorInfoDetail.GetPatientsMatchByDoctorId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(),
                        cdr["PatientId"].ToString(), cdr["PatientName"].ToString(), cdr["HUserId"].ToString(), cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Description"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.DoctorInfoDetail.GetPatientsMatchByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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