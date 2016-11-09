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
    public class PsBasicInfoDetail
    {
        //SetData WF 2014-12-2 
        public static bool SetData(DataConnection pclsCache, string Patient, string CategoryCode, string ItemCode, int ItemSeq, string Value, string Description, int SortNo, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.BasicInfoDetail.SetData(pclsCache.CacheConnectionObject, Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //GetBasicInfo WF 2014-12-2 
        public static CacheSysList GetDetailInfo(DataConnection pclsCache, string UserId)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.BasicInfoDetail.GetDetailInfo(pclsCache.CacheConnectionObject, UserId);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetDetailInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //GetModulesByPID LS 2014-12-4 
        public static DataTable GetModulesByPID(DataConnection pclsCache, string PatientId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("Modules", typeof(string)));

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
                cmd = Ps.BasicInfoDetail.GetModulesByPID(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["CategoryCode"].ToString(), cdr["Modules"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetModulesByPID", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetHModulesByPID LY 2015-12-16 
        public static DataTable GetHModulesByPID(DataConnection pclsCache, string PatientId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("Modules", typeof(string)));

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
                cmd = Ps.BasicInfoDetail.GetHModulesByPID(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["CategoryCode"].ToString(), cdr["Modules"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetHModulesByPID", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //Delete WF 2014-12-2 
        public static int Delete(DataConnection pclsCache, string UserId, string CategoryCode, string ItemCode, int ItemSeq)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.BasicInfoDetail.Delete(pclsCache.CacheConnectionObject, UserId, CategoryCode, ItemCode, ItemSeq);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //DeleteAll LY 2015-7-9 
        public static int DeleteAll(DataConnection pclsCache, string UserId, string CategoryCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.BasicInfoDetail.DeleteAll(pclsCache.CacheConnectionObject, UserId, CategoryCode);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetMaxItemSeq WF 2014-12-2 
        public static int GetMaxItemSeq(DataConnection pclsCache, string UserId, string CategoryCode, string ItemCode)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.BasicInfoDetail.GetMaxItemSeq(pclsCache.CacheConnectionObject, UserId, CategoryCode, ItemCode);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetMaxItemSeq", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetPatientBasicInfoDetail LS 2014-12-4 
        public static DataTable GetPatientBasicInfoDetail(DataConnection pclsCache, string UserId, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("CategoryName", typeof(string)));
            list.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("ItemName", typeof(string)));
            list.Columns.Add(new DataColumn("ParentCode", typeof(string)));
            list.Columns.Add(new DataColumn("ItemSeq", typeof(int)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("Content", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
            list.Columns.Add(new DataColumn("ControlType", typeof(int)));
            list.Columns.Add(new DataColumn("OptionCategory", typeof(string)));
            list.Columns.Add(new DataColumn("GroupHeaderFlag", typeof(int)));


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
                cmd = Ps.BasicInfoDetail.GetPatientBasicInfoDetail(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                int ItemSeq;
                int SortNo;
                int ControlType;
                int GroupHeaderFlag;
                while (cdr.Read())
                {
                    if (cdr["ItemSeq"].ToString() == "")
                    {
                        ItemSeq = 0;
                    }
                    else
                    {
                        ItemSeq = Convert.ToInt32(cdr["ItemSeq"]);
                    }
                    if (cdr["SortNo"].ToString() == "")
                    {
                        SortNo = 0;
                    }
                    else
                    {
                        SortNo = Convert.ToInt32(cdr["SortNo"]);
                    }
                    if (cdr["ControlType"].ToString() == "")
                    {
                        ControlType = 0;
                    }
                    else
                    {
                        ControlType = Convert.ToInt32(cdr["ControlType"]);
                    }
                    if (cdr["GroupHeaderFlag"].ToString() == "")
                    {
                        GroupHeaderFlag = 0;
                    }
                    else
                    {
                        GroupHeaderFlag = Convert.ToInt32(cdr["GroupHeaderFlag"]);
                    }
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["CategoryCode"].ToString(), cdr["CategoryName"].ToString(), cdr["ItemCode"].ToString(),
                                   cdr["ItemName"].ToString(), cdr["ParentCode"].ToString(), ItemSeq, cdr["Value"].ToString(), cdr["Content"].ToString(),
                                   cdr["Description"].ToString(), SortNo, ControlType, cdr["OptionCategory"].ToString(), GroupHeaderFlag);



                    //cdr["UserId"].ToString(), cdr["CategoryCode"].ToString(), cdr["CategoryName"].ToString(), cdr["ItemCode"].ToString(),
                    // cdr["ItemName"].ToString(), ItemSeq, cdr["Value"].ToString(), cdr["Description"].ToString(), SortNo);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.GetPatientBasicInfoDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //CheckPhoneNumber WF 2014-12-2 TDY 2015-1-14
        public static int CheckPatientPhoneNumber(DataConnection pclsCache, string UserId, string piPhoneNumber)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.BasicInfoDetail.CheckPatientPhoneNumber(pclsCache.CacheConnectionObject, UserId, piPhoneNumber);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsBasicInfoDetail.CheckPatientPhoneNumber", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDoctorsByPatientId   GL 2015-01-20
        public static DataTable GetDoctorsByPatientId(DataConnection pclsCache, string PatientId, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("ItemSeq", typeof(int)));

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
                cmd = Ps.BasicInfoDetail.GetDoctorsByPatientId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PatientId", CacheDbType.NVarChar).Value = PatientId;
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(), Convert.ToInt32(cdr["ItemSeq"]));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetDoctorsByPatientId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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


        //GetPatientDetailInfo TDY 2015-04-07
        public static CacheSysList GetPatientDetailInfo(DataConnection pclsCache, string UserId)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.BasicInfoDetail.GetPatientDetailInfo(pclsCache.CacheConnectionObject, UserId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetPatientDetailInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetModulesUnBought TDY 2015-04-07
        public static DataTable GetModulesUnBought(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Ps.BasicInfoDetail.GetModulesUnBought(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetModulesUnBought", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetSDoctor TDY 2015-04-07
        public static CacheSysList GetSDoctor(DataConnection pclsCache, string PatientId)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.BasicInfoDetail.GetSDoctor(pclsCache.CacheConnectionObject, PatientId);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.BasicInfoDetail.GetSDoctor", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 获取需要知道的项目的值 SYF 20150423
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
                ret = (string)Ps.BasicInfoDetail.GetValue(pclsCache.CacheConnectionObject, UserId, CategoryCode, ItemCode, ItemSeq);
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
    
    }
}