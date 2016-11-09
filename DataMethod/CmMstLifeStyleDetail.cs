using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstLifeStyleDetail
    {
        //WF 20150408
        public static int SetData(DataConnection pclsCache, string StyleId, string Module, string CurativeEffect, string SideEffect, string Instruction, string HealthEffect, string Unit, string Redundance)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstLifeStyleDetail.SetData(pclsCache.CacheConnectionObject, StyleId, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, Redundance);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstLifeStyleDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //WF 20150408
        public static DataTable GetLifeStyleDetail(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("StyleId", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("CurativeEffect", typeof(string)));
            list.Columns.Add(new DataColumn("SideEffect", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
            list.Columns.Add(new DataColumn("HealthEffect", typeof(string)));
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
                cmd = Cm.MstLifeStyleDetail.GetLifeStyleDetail(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["StyleId"].ToString(), cdr["Name"].ToString(), cdr["CurativeEffect"].ToString(), cdr["SideEffect"].ToString(), cdr["Instruction"].ToString(), cdr["HealthEffect"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstLifeStyleDetail.GetLifeStyleDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //获取所有数据 2015-05-29 GL
        public static DataTable GetLifeStyleDetailList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("StyleId", typeof(string)));
            list.Columns.Add(new DataColumn("Module", typeof(string)));
            list.Columns.Add(new DataColumn("CurativeEffect", typeof(string)));
            list.Columns.Add(new DataColumn("SideEffect", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
            list.Columns.Add(new DataColumn("HealthEffect", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));
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
                cmd = Cm.MstLifeStyleDetail.GetLifeStyleDetailList(pclsCache.CacheConnectionObject);

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["StyleId"].ToString(), cdr["Module"].ToString(), cdr["CurativeEffect"].ToString(), cdr["SideEffect"].ToString(), cdr["Instruction"].ToString(), cdr["HealthEffect"].ToString(), cdr["Unit"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstLifeStyleDetail.GetLifeStyleDetailList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //删除数据 2015-05-29 GL
        public static int DeleteData(DataConnection pclsCache, string StyleId, string Module)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstLifeStyleDetail.DeleteData(pclsCache.CacheConnectionObject, StyleId, Module);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstLifeStyleDetail.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }


    }
}