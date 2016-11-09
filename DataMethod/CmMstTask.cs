using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.CommonLibrary;
using System.Data;
using InterSystems.Data.CacheClient;
using InterSystems.Data.CacheTypes;

namespace WebService.DataMethod
{
    public class CmMstTask
    {
        public static int SetData(DataConnection pclsCache, string CategoryCode, string Code, string Name, string ParentCode, string Description, int StartDate, int EndDate, int GroupHeaderFlag, int ControlType, string OptionCategory, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstTask.SetData(pclsCache.CacheConnectionObject, CategoryCode, Code, Name, ParentCode, Description, StartDate, EndDate, GroupHeaderFlag, ControlType, OptionCategory, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstTask.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static DataTable GetMstTaskByParentCode(DataConnection pclsCache, string ParentCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("ParentCode", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("StartDate", typeof(int)));
            list.Columns.Add(new DataColumn("EndDate", typeof(int)));
            list.Columns.Add(new DataColumn("GroupHeaderFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ControlType", typeof(int)));
            list.Columns.Add(new DataColumn("OptionCategory", typeof(string)));
            list.Columns.Add(new DataColumn("CreateDateTime", typeof(string)));
            list.Columns.Add(new DataColumn("Author", typeof(string)));
            list.Columns.Add(new DataColumn("AuthorName", typeof(string)));
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
                cmd = Cm.MstTask.GetMstTaskByParentCode(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("ParentCode", CacheDbType.NVarChar).Value = ParentCode;
                cdr = cmd.ExecuteReader();
                DateTime CreateDateTime = new DateTime();
                while (cdr.Read())
                {
                    if (cdr["CreateDateTime"].ToString() != "")
                        CreateDateTime = Convert.ToDateTime(cdr["CreateDateTime"]);
                    list.Rows.Add(cdr["CategoryCode"].ToString(), cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["ParentCode"].ToString(), cdr["Description"].ToString(), Convert.ToInt32(cdr["StartDate"]), Convert.ToInt32(cdr["EndDate"]), Convert.ToInt32(cdr["GroupHeaderFlag"]), Convert.ToInt32(cdr["ControlType"]), cdr["OptionCategory"].ToString(), CreateDateTime, cdr["Author"].ToString(), cdr["AuthorName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstTask.GetMstTaskByParentCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static DataTable GetTasks(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("ParentCode", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("GroupHeaderFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ControlType", typeof(int)));
            list.Columns.Add(new DataColumn("OptionCategory", typeof(string)));

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
                cmd = Cm.MstTask.GetTasks(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();

                while (cdr.Read())
                {
                    list.Rows.Add(
                        cdr["CategoryCode"].ToString(), 
                        cdr["Code"].ToString(), 
                        cdr["Name"].ToString(), 
                        cdr["ParentCode"].ToString(), 
                        cdr["Description"].ToString(), 
                        Convert.ToInt32(cdr["GroupHeaderFlag"]), 
                        Convert.ToInt32(cdr["ControlType"]), 
                        cdr["OptionCategory"].ToString()
                        );
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstTask.GetTasks", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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


        public static CacheSysList GetCmTaskItemInfo(DataConnection pclsCache, string CategoryCode, string Code)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Cm.MstTask.GetCmTaskItemInfo(pclsCache.CacheConnectionObject, CategoryCode, Code);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstTask.GetCmTaskItemInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static int DeleteMstTask(DataConnection pclsCache, string CategoryCode, string Code)
        {
            int ret = 2;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstTask.DeleteData(pclsCache.CacheConnectionObject, CategoryCode, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstTask.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}