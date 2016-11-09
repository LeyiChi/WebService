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
    public class PsDoctorInfo
    {

        //SetData  LS 2014-12-1
        public static bool SetData(DataConnection pclsCache, string UserId, string UserName, int Birthday, int Gender, string IDNo, int InvalidFlag, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.DoctorInfo.SetData(pclsCache.CacheConnectionObject, UserId, UserName, Birthday, Gender, IDNo, InvalidFlag, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDoctorList LS LS 2014-12-1
        public static DataTable GetDoctorList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));

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
                cmd = Ps.DoctorInfo.GetDoctorList(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(), Convert.ToInt32(cdr["Birthday"]), Convert.ToInt32(cdr["Gender"]), cdr["IDNo"].ToString(), Convert.ToInt32(cdr["InvalidFlag"]));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetDoctorModuleList  LS 2014-12-1
        public static DataTable GetDoctorModuleList(DataConnection pclsCache, string DoctorId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("CategoryName", typeof(string)));
            list.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            list.Columns.Add(new DataColumn("ItemName", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));

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
                cmd = Ps.DoctorInfo.GetDoctorModuleList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorId", CacheDbType.NVarChar).Value = DoctorId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["CategoryCode"].ToString(), cdr["CategoryName"].ToString(), cdr["ItemCode"].ToString(), cdr["ItemName"].ToString(), cdr["Value"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorModuleList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetDoctorListByName  LS 2014-12-1
        public static DataTable GetDoctorListByName(DataConnection pclsCache, string DoctorName)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));

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
                cmd = Ps.DoctorInfo.GetDoctorListByName(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorName", CacheDbType.NVarChar).Value = DoctorName;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(), Convert.ToInt32(cdr["Birthday"]), Convert.ToInt32(cdr["Gender"]), cdr["IDNo"].ToString(), Convert.ToInt32(cdr["InvalidFlag"]));
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorListByName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetDoctorInfo返回医生基本信息 ZYF 2014-12-4
        public static DataTable GetDoctorInfo(DataConnection pclsCache, string DoctorId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.DoctorInfo.GetDoctorInfo(pclsCache.CacheConnectionObject, DoctorId);
                //DataCheck ZAM 2015-1-7
                if (CacheList != null)
                {
                    list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2].ToString(), CacheList[3].ToString(), CacheList[4], CacheList[5]);
                }
                //
                //list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2].ToString(), CacheList[3].ToString(), CacheList[4], CacheList[5]);
                return list;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetDoctorInfoWithMod, 返回包含模块信息的医生信息 ZYF 2014-12-4
        public static DataTable GetDoctorInfoWithMod(DataConnection pclsCache, string DoctorId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ModuleString", typeof(string)));
            string strMod = "";
            DataTable DTMod = new DataTable();
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.DoctorInfo.GetDoctorInfo(pclsCache.CacheConnectionObject, DoctorId);
                DTMod = GetDoctorModuleList(pclsCache, DoctorId);
                foreach (DataRow dr in DTMod.Rows)
                {
                    if (dr["Value"] != null)
                    {
                        if (dr["Value"].ToString() == "0")
                        {
                            if (strMod == "")
                            {
                                strMod = strMod + dr["CategoryName"].ToString();
                            }
                            else
                            {
                                strMod = strMod + "," + dr["CategoryName"].ToString();
                            }
                        }
                    }
                }
                if (CacheList != null)       //value check ZAM 2015-1-7
                {
                    list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2].ToString(), CacheList[3].ToString(), CacheList[4], CacheList[5], strMod);

                }
                //list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2].ToString(), CacheList[3].ToString(), CacheList[4], CacheList[5], strMod);
                return list;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorInfoWithMod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDoctorListWithMod  ZYF 2014-12-4
        public static DataTable GetDoctorListWithMod(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ModuleString", typeof(string)));
            string strMod = "";
            DataTable DTMod = new DataTable();

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
                cmd = Ps.DoctorInfo.GetDoctorList(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    strMod = "";
                    DTMod = GetDoctorModuleList(pclsCache, cdr["DoctorId"].ToString());
                    foreach (DataRow dr in DTMod.Rows)
                    {
                        if (dr["Value"] != null)
                        {
                            if (dr["Value"].ToString() == "0")
                            {
                                if (strMod == "")
                                {
                                    strMod = strMod + dr["CategoryName"].ToString();
                                }
                                else
                                {
                                    strMod = strMod + "," + dr["CategoryName"].ToString();
                                }

                            }
                        }
                    }

                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(), Convert.ToInt32(cdr["Birthday"]), Convert.ToInt32(cdr["Gender"]), cdr["IDNo"].ToString(), Convert.ToInt32(cdr["InvalidFlag"]), strMod);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorListWithMod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetDoctorListByNameWithMod  ZYF 2014-12-4
        public static DataTable GetDoctorListByNameWithMod(DataConnection pclsCache, string DoctorName)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DoctorId", typeof(string)));
            list.Columns.Add(new DataColumn("DoctorName", typeof(string)));
            list.Columns.Add(new DataColumn("Birthday", typeof(int)));
            list.Columns.Add(new DataColumn("Gender", typeof(int)));
            list.Columns.Add(new DataColumn("IDNo", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ModuleString", typeof(string)));
            string strMod = "";
            DataTable DTMod = new DataTable();

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
                cmd = Ps.DoctorInfo.GetDoctorListByName(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("DoctorName", CacheDbType.NVarChar).Value = DoctorName;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    strMod = "";
                    DTMod = GetDoctorModuleList(pclsCache, cdr["DoctorId"].ToString());
                    foreach (DataRow dr in DTMod.Rows)
                    {
                        if (dr["Value"] != null)
                        {
                            if (dr["Value"].ToString() == "0")
                            {
                                if (strMod == "")
                                {
                                    strMod = strMod + dr["CategoryName"].ToString();
                                }
                                else
                                {
                                    strMod = strMod + "," + dr["CategoryName"].ToString();
                                }
                            }
                        }
                    }
                    list.Rows.Add(cdr["DoctorId"].ToString(), cdr["DoctorName"].ToString(), Convert.ToInt32(cdr["Birthday"]), Convert.ToInt32(cdr["Gender"]), cdr["IDNo"].ToString(), Convert.ToInt32(cdr["InvalidFlag"]), strMod);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDoctorListByNameWithMod", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetDocNameList CSQ 2014-12-04
        public static DataTable GetDocNameList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));

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
                cmd = Ps.DoctorInfo.GetNameList(pclsCache.CacheConnectionObject);

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["UserName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsDoctorInfo.GetDocNameList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //SetData WF 2014-12-2 
        public static bool SetDocName(DataConnection pclsCache, string UserId, string UserName)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.DoctorInfo.SetDocName(pclsCache.CacheConnectionObject, UserId, UserName);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DoctorInfo.SetDocName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetModuleByDoctorId WF 2015-06-04 
        public static string GetModuleByDoctorId(DataConnection pclsCache, string UserId)
        {
            string Module = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;

                }
                Module = Ps.DoctorInfo.GetModuleByDoctorId(pclsCache.CacheConnectionObject, UserId);

                return Module;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "DoctorInfo.GetModuleByDoctorId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return Module;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}