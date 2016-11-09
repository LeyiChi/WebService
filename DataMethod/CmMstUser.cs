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
    public class CmMstUser
    {
        // 删除用户所有旧信息 zam 2015-1-20
        public static int DeleteAll(DataConnection pclsCache, string UserId, int Date)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstUser.DeleteAll(pclsCache.CacheConnectionObject, UserId, Date);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.DeleteAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //SetDataUM ZYF 2015-1-20
        public static bool SetDataUM(DataConnection pclsCache, string UserId, string UserName, string Password, string Class, int EndDate, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstUser.SetDataUM(pclsCache.CacheConnectionObject, UserId, UserName, Password, Class, EndDate, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.SetDataUM", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //SetData ZAM 2014-11-26
        public static bool SetData(DataConnection pclsCache, string UserId, string UserName, string Password, string Class, string PatientClass, int DoctorClass, int StartDate, int EndDate, DateTime LastLoginDateTime, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstUser.SetData(pclsCache.CacheConnectionObject, UserId, UserName, Password, Class, PatientClass, DoctorClass, StartDate, EndDate, LastLoginDateTime, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        // GetUserInfoByUserId ZAM 2014-12-02
        public static DataTable GetUserInfoByUserId(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("Password", typeof(string)));
            list.Columns.Add(new DataColumn("Class", typeof(string)));
            list.Columns.Add(new DataColumn("ClassName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(string)));
            list.Columns.Add(new DataColumn("EndDate", typeof(string)));
            //增加字段，存邀请码。修改：CSQ 2015-05-15
            list.Columns.Add(new DataColumn("PatientClass", typeof(string)));
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Cm.MstUser.GetUserInfoByUserId(pclsCache.CacheConnectionObject, UserId);
                if (CacheList != null)
                {
                    list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2].ToString(), CacheList[3].ToString(), CacheList[4], CacheList[5], CacheList[6], CacheList[7]);
                }
                return list;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetUserInfoByUserId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        // GetUserList ZAM 2014-12-02
        public static DataTable GetUserList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("Password", typeof(string)));
            list.Columns.Add(new DataColumn("Class", typeof(string)));
            list.Columns.Add(new DataColumn("ClassName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(int)));
            list.Columns.Add(new DataColumn("EndDate", typeof(int)));

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
                cmd = Cm.MstUser.GetUserList(pclsCache.CacheConnectionObject);
                //cmd.Parameters.Add("Category", CacheDbType.NVarChar).Value = Category;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int StartDate = 0;      //value check ZAM 2014-1-7
                    int EndDate = 99999999;
                    if (cdr["StartDate"].ToString() != "")
                    {
                        StartDate = Convert.ToInt32(cdr["StartDate"]);
                    }
                    if (cdr["EndDate"].ToString() != "")
                    {
                        EndDate = Convert.ToInt32(cdr["EndDate"]);
                    }
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["UserName"].ToString(), cdr["Password"].ToString(), cdr["Class"].ToString(), cdr["ClassName"], StartDate, EndDate);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetUserList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetUserListById WF 2015-05-28
        public static DataTable GetUserListById(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("Password", typeof(string)));
            list.Columns.Add(new DataColumn("Class", typeof(string)));
            list.Columns.Add(new DataColumn("ClassName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(int)));
            list.Columns.Add(new DataColumn("EndDate", typeof(int)));

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
                cmd = Cm.MstUser.GetUserListById(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                //cmd.Parameters.Add("Category", CacheDbType.NVarChar).Value = Category;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int StartDate = 0;      //value check ZAM 2014-1-7
                    int EndDate = 99999999;
                    if (cdr["StartDate"].ToString() != "")
                    {
                        StartDate = Convert.ToInt32(cdr["StartDate"]);
                    }
                    if (cdr["EndDate"].ToString() != "")
                    {
                        EndDate = Convert.ToInt32(cdr["EndDate"]);
                    }
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["UserName"].ToString(), cdr["Password"].ToString(), cdr["Class"].ToString(), cdr["ClassName"], StartDate, EndDate);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetUserListById", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetUserListByName ZAM 2014-12-02
        public static DataTable GetUserListByName(DataConnection pclsCache, string UserName)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("UserId", typeof(string)));
            list.Columns.Add(new DataColumn("UserName", typeof(string)));
            list.Columns.Add(new DataColumn("Password", typeof(string)));
            list.Columns.Add(new DataColumn("Class", typeof(string)));
            list.Columns.Add(new DataColumn("ClassName", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(int)));
            list.Columns.Add(new DataColumn("EndDate", typeof(int)));

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
                cmd = Cm.MstUser.GetUserListByName(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserName", CacheDbType.NVarChar).Value = UserName;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int StartDate = 0;      //value check ZAM 2014-1-7
                    int EndDate = 99999999;
                    if (cdr["StartDate"].ToString() != "")
                    {
                        StartDate = Convert.ToInt32(cdr["StartDate"]);
                    }
                    if (cdr["EndDate"].ToString() != "")
                    {
                        EndDate = Convert.ToInt32(cdr["EndDate"]);
                    }
                    list.Rows.Add(cdr["UserId"].ToString(), cdr["UserName"].ToString(), cdr["Password"].ToString(), cdr["Class"].ToString(), cdr["ClassName"].ToString(), StartDate, EndDate);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetUserList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetNameByUserId ZAM 2014-11-26
        public static string GetNameByUserId(DataConnection pclsCache, string UserId)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstUser.GetNameByUserId(pclsCache.CacheConnectionObject, UserId);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetNameByUserId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        // GetClassByUserId ZAM 2014-11-26
        public static string GetClassByUserId(DataConnection pclsCache, string UserId)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Class = Cm.MstUser.GetClassByUserId(pclsCache.CacheConnectionObject, UserId);
                return Class;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetClassByUserId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //CheckPassword ZAM 2014-12-01
        public static int CheckPassword(DataConnection pclsCache, string UserId, string Password)
        {
            //bool IsSaved = false;
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstUser.CheckPassword(pclsCache.CacheConnectionObject, UserId, Password);
                //if (flag == 1)
                //{
                //    IsSaved = true;
                //}
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.CheckPassword", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //ChangePassword ZAM 2014-12-01
        public static int ChangePassword(DataConnection pclsCache, string UserId, string OldPassword, string newPassword, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstUser.ChangePassword(pclsCache.CacheConnectionObject, UserId, OldPassword, newPassword, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.ChangePassword", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //UpdateLastLoginDateTime ZAM 2014-11-26
        public static int UpdateLastLoginDateTime(DataConnection pclsCache, string UserId, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstUser.UpdateLastLoginDateTime(pclsCache.CacheConnectionObject, UserId, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.UpdateLastLoginDateTime", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //UpdateOldValidDate ZAM 2014-11-26
        public static int UpdateOldValidDate(DataConnection pclsCache, string UserId, int StartDate, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;
                }
                ret = (int)Cm.MstUser.UpdateOldValidDate(pclsCache.CacheConnectionObject, UserId, StartDate, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.UpdateOldValidDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //CheckExist ZYF 2014-12-08
        public static bool CheckExist(DataConnection pclsCache, string UserId)
        {
            bool exist = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return exist;
                }

                int flag = (int)Cm.MstUser.CheckExist(pclsCache.CacheConnectionObject, UserId);
                if (flag == 1)
                {
                    exist = true;
                }
                return exist;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.CheckExist", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return exist;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }


        //Register TDY 2015-04-07 专员注册 //TDY 20150419修改
        public static int Register(DataConnection pclsCache, string Type, string Name, string Value, string Password, string UserName, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstUser.Register(pclsCache.CacheConnectionObject, Type, Name, Value, Password, UserName, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.Register", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //Activition TDY 2015-04-07 患者激活 //TDY 20150419修改
        public static int Activition(DataConnection pclsCache, string UserId, string InviteCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstUser.Activition(pclsCache.CacheConnectionObject, UserId, InviteCode, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstUser.Activition", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetPhoneNoByUserId WF 2014-11-26
        public static string GetPhoneNoByUserId(DataConnection pclsCache, string UserId)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstUser.GetPhoneNoByUserId(pclsCache.CacheConnectionObject, UserId);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstUser.GetPhoneNoByUserId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

    }
}