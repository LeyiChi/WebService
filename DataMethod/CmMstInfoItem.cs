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
    public class CmMstInfoItem
    {
        //SetData WF 2014-12-2 
        public static bool SetData(DataConnection pclsCache, string CategoryCode, string Code, string Name, string ParentCode, int SortNo, int StartDate, int EndDate, int GroupHeaderFlag, string ControlType, string OptionCategory, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstInfoItem.SetData(pclsCache.CacheConnectionObject, CategoryCode, Code, Name, ParentCode, SortNo, StartDate, EndDate, GroupHeaderFlag, ControlType, OptionCategory, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }


        }

        //GetInfoItem WF 2014-12-2 
        public static DataTable GetInfoItem(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("CategoryCode", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("ParentCode", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
            list.Columns.Add(new DataColumn("StartDate", typeof(string)));
            list.Columns.Add(new DataColumn("EndDate", typeof(string)));
            list.Columns.Add(new DataColumn("GroupHeaderFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ControlType", typeof(string)));
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
                cmd = Cm.MstInfoItem.GetInfoItem(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                int SortNo;
                int GroupHeaderFlag;
                //int ControlType;
                while (cdr.Read())
                {
                    if (cdr["SortNo"].ToString() == "")
                    {
                        SortNo = 0;
                    }
                    else
                    {
                        SortNo = Convert.ToInt32(cdr["SortNo"]);
                    }
                    if (cdr["GroupHeaderFlag"].ToString() == "")
                    {
                        GroupHeaderFlag = 0;
                    }
                    else
                    {
                        GroupHeaderFlag = Convert.ToInt32(cdr["GroupHeaderFlag"]);
                    }
                    list.Rows.Add(cdr["CategoryCode"].ToString(), cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["ParentCode"].ToString(), SortNo, cdr["StartDate"].ToString(), cdr["EndDate"].ToString(), GroupHeaderFlag, cdr["ControlType"].ToString(), cdr["OptionCategory"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.GetInfoItem", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData WF 2014-12-2 
        public static int DeleteData(DataConnection pclsCache, string CategoryCode, string Code)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstInfoItem.DeleteData(pclsCache.CacheConnectionObject, CategoryCode, Code);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetItemName WF 2014-12-2 
        public static string GetItemName(DataConnection pclsCache, string CategoryCode, string Code)
        {
            string ret = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (Cm.MstInfoItem.GetItemName(pclsCache.CacheConnectionObject, CategoryCode, Code)).ToString();
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.GetItemName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetInfoItemDetail WF 2014-12-2 
        public static CacheSysList GetInfoItemDetail(DataConnection pclsCache, string CategoryCode, string Code)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Cm.MstInfoItem.GetInfoItemDetail(pclsCache.CacheConnectionObject, CategoryCode, Code);
                return CacheList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.GetInfoItemDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //GetMstInfoItemByCategoryCode CSQ 2014-12-04 
        public static DataTable GetMstInfoItemByCategoryCode(DataConnection pclsCache, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("ParentCode", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
            list.Columns.Add(new DataColumn("GroupHeaderFlag", typeof(int)));
            list.Columns.Add(new DataColumn("ControlType", typeof(string)));
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
                cmd = Cm.MstInfoItem.GetMstInfoItemByCategoryCode(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                int SortNo;
                int GroupHeaderFlag;
                //int ControlType;
                while (cdr.Read())
                {
                    if (cdr["SortNo"].ToString() == "")
                    {
                        SortNo = 0;
                    }
                    else
                    {
                        SortNo = Convert.ToInt32(cdr["SortNo"]);
                    }
                    if (cdr["GroupHeaderFlag"].ToString() == "")
                    {
                        GroupHeaderFlag = 0;
                    }
                    else
                    {
                        GroupHeaderFlag = Convert.ToInt32(cdr["GroupHeaderFlag"]);
                    }
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["ParentCode"].ToString(), SortNo, GroupHeaderFlag, cdr["ControlType"].ToString(), cdr["OptionCategory"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.GetMstInfoItemByCategoryCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetNextCode YDS 2014-12-23
        public static DataTable GetNextCode(DataConnection pclsCache, string CategoryCode)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));

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
                cmd = Cm.MstInfoItem.GetNextCode(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("CategoryCode", CacheDbType.NVarChar).Value = CategoryCode;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstInfoItem.GetNextCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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