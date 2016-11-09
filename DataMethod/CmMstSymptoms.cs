using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstSymptoms
    {
        //SetData 写数据 ZYF 2014-12-01
        public static bool SetData(DataConnection pclsCache, string Type, string Code, string TypeName, string Name, string InputCode, int SortNo, string Redundance, int InvalidFlag)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Cm.MstSymptoms.SetData(pclsCache.CacheConnectionObject, Type, Code, TypeName, Name, InputCode, SortNo, Redundance, InvalidFlag);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetNameByCode 获取症状名称 ZYF 2014-12-01
        public static string GetNameByCode(DataConnection pclsCache, string Type, string Code)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstSymptoms.GetNameByCode(pclsCache.CacheConnectionObject, Type, Code);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetNameByCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetTypeNameByType 获取症状类型名称 ZYF 2014-12-01
        public static string GetTypeNameByType(DataConnection pclsCache, string Type)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Cm.MstSymptoms.GetTypeNameByType(pclsCache.CacheConnectionObject, Type);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetTypeNameByType", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetSymptoms 返回所有症状信息 ZYF 2014-12-01
        public static DataTable GetSymptoms(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("InputCode", typeof(string)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));
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
                cmd = Cm.MstSymptoms.GetSymptoms(pclsCache.CacheConnectionObject);
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

                    list.Rows.Add(cdr["Type"].ToString(), cdr["Code"].ToString(), cdr["TypeName"].ToString(), cdr["Name"].ToString(), cdr["InputCode"].ToString(), Convert.ToInt32(cdr["SortNo"]), cdr["Redundance"].ToString(), int_InvalidFlag);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetSymptoms", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        // GetNameList 返回某类型所有症状代码及名称 ZYF 2014-12-01
        public static DataTable GetNameList(DataConnection pclsCache, string Type)
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
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Cm.MstSymptoms.GetNameList(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Type", CacheDbType.NVarChar).Value = Type;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetNameList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //DeleteData 删数据 ZYF 2014-12-01
        public static int DeleteData(DataConnection pclsCache, string Type, string Code)
        {
            int flag = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return flag;

                }
                flag = (int)Cm.MstSymptoms.DeleteData(pclsCache.CacheConnectionObject, Type, Code);

                return flag;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return flag;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetTypeList 返回所有症状类型代码及名称 ZYF 2014-12-01
        public static DataTable GetTypeList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));

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
                cmd = Cm.MstSymptoms.GetTypeList(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Type"].ToString(), cdr["TypeName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetTypeList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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
        
        //GetMaxCode YDS 2014-12-22
        public static string GetMaxCode(DataConnection pclsCache, string Type)
        {
            string ret = string.Empty;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = Cm.MstSymptoms.GetMaxCode(pclsCache.CacheConnectionObject, Type);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstSymptoms.GetMaxCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}