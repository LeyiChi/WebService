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
    public class CmMstHealthEducation
    {
        //WF 20150408
        public static int SetData(DataConnection pclsCache, string Module, string HealthId, int Type, string FileName, string Path, string Introduction, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstHealthEducation.SetData(pclsCache.CacheConnectionObject, Module, HealthId, Type, FileName, Path, Introduction, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //WF 20150408
        public static DataTable GetVedioAddress(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetVedioAddress(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetVedioAddress", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150408
        public static DataTable GetTextAddress(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetTextAddress(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetTextAddress", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150408
        public static DataTable GetImageAddress(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetImageAddress(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetImageAddress", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150408
        public static DataTable GetPDFAddress(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetPDFAddress(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetPDFAddress", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150514
        public static DataTable GetAddressByType(DataConnection pclsCache, string Module, int Type)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Module", typeof(string)));
            list.Columns.Add(new DataColumn("ModuleName", typeof(string)));
            list.Columns.Add(new DataColumn("HealthId", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(int)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));
            list.Columns.Add(new DataColumn("Author", typeof(string)));
            list.Columns.Add(new DataColumn("AuthorName", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetAddressByType(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;
                cmd.Parameters.Add("Type", CacheDbType.Int).Value = Type;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Module"].ToString(), cdr["ModuleName"].ToString(), cdr["HealthId"].ToString(), cdr["Type"], cdr["TypeName"].ToString(), cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString(), cdr["Redundance"].ToString(), cdr["Author"].ToString(), cdr["AuthorName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetAddressByType", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //WF 20150514
        public static DataTable GetAll(DataConnection pclsCache, string Module, string HealthId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Module", typeof(string)));
            list.Columns.Add(new DataColumn("HealthId", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(int)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));
            list.Columns.Add(new DataColumn("ReUserId", typeof(string)));
            list.Columns.Add(new DataColumn("ReUserName", typeof(string)));
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Cm.MstHealthEducation.GetAll(pclsCache.CacheConnectionObject, Module, HealthId);
                if (CacheList != null)
                {
                    list.Rows.Add(CacheList[0].ToString(), CacheList[1].ToString(), CacheList[2], CacheList[3].ToString(), CacheList[4].ToString(), CacheList[5].ToString(), CacheList[6].ToString(), CacheList[7].ToString(), CacheList[8].ToString(), CacheList[9].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetAll", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }

        }

        //WY 20150518
        public static int DeleteData(DataConnection pclsCache, string Module, string HealthId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstHealthEducation.DeleteData(pclsCache.CacheConnectionObject, Module, HealthId);

                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //获取健康教育表所有信息 2015-05-29 GL
        public static DataTable GetHealthEducationList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Module", typeof(string)));
            list.Columns.Add(new DataColumn("HealthId", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("FileName", typeof(string)));
            list.Columns.Add(new DataColumn("Path", typeof(string)));
            list.Columns.Add(new DataColumn("Introduction", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }
                cmd = new CacheCommand();
                cmd = Cm.MstHealthEducation.GetHealthEducationList(pclsCache.CacheConnectionObject);

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Module"].ToString(), cdr["HealthId"].ToString(), cdr["Type"].ToString(), cdr["FileName"].ToString(), cdr["Path"].ToString(), cdr["Introduction"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstHealthEducation.GetHealthEducationList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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