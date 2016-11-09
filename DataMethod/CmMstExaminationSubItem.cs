using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstExaminationSubItem
    {
        //SetData 写数据 ZYF 2014-12-01
        public static bool SetData(DataConnection pclsCache, string Code, string Name, int SortNo, string InputCode, string Redundance, int InvalidFlag)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstExaminationSubItem.SetData(pclsCache.CacheConnectionObject, Code, Name, SortNo, InputCode, Redundance, InvalidFlag);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstExaminationSubItem.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //DeleteData 删数据 ZYF 2014-12-01
        public static int DeleteData(DataConnection pclsCache, string Code, string SubCode)
        {
            int flag = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return flag;

                }
                flag = (int)Cm.MstExaminationSubItem.DeleteData(pclsCache.CacheConnectionObject, Code, SubCode);

                return flag;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstExaminationSubItem.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return flag;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetName 获取名称 ZYF 2014-12-01
        public static string GetName(DataConnection pclsCache, string Code)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstExaminationSubItem.GetName(pclsCache.CacheConnectionObject, Code);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstExaminationSubItem.GetName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetNameList 返回某类型所有检查项目子分类代码及名称 ZYF 2014-12-01
        public static DataTable GetNameList(DataConnection pclsCache, string Code)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("SubCode", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));

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
                cmd = Cm.MstExaminationSubItem.GetNameList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Code", CacheDbType.NVarChar).Value = Code;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["SubCode"].ToString(), cdr["Name"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstExaminationSubItem.GetNameList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetExaminationSubItem 返回所有检查项目子分类信息 ZYF 2014-12-01
        public static DataTable GetExaminationSubItem(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            //list.Columns.Add(new DataColumn("SubCode", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
            list.Columns.Add(new DataColumn("InputCode", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            int int_InvalidFlag = 0;

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
                cmd = Cm.MstExaminationSubItem.GetExaminationSubItem(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    if (cdr["InvalidFlag"].ToString() == "")
                    {
                        int_InvalidFlag = 0;
                    }
                    else
                    {
                        int_InvalidFlag = Convert.ToInt32(cdr["InvalidFlag"]);
                    }

                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), Convert.ToInt32(cdr["SortNo"]), cdr["InputCode"].ToString(), cdr["Redundance"].ToString(), int_InvalidFlag);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstExaminationSubItem.GetExaminationSubItem", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static int GetMaxSortNo(DataConnection pclsCache)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Cm.MstExaminationSubItem.GetMaxSortNo(pclsCache.CacheConnectionObject);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MstExaminationSubItem.GetMaxSortNo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

    }
}