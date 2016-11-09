using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class CmMstDrugDetail
    {
        public static int SetData(DataConnection pclsCache, string DrugCode, string Module, string CurativeEffect, string SideEffect, string Instruction, string HealthEffect, string Unit, string Redundance)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstDrugDetail.SetData(pclsCache.CacheConnectionObject, DrugCode, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, Redundance);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstDrugDetail.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static DataTable GetDrugDetail(DataConnection pclsCache, string Module)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DrugCode", typeof(string)));
            list.Columns.Add(new DataColumn("DrugName", typeof(string)));
            list.Columns.Add(new DataColumn("DrugSpec", typeof(string)));
            list.Columns.Add(new DataColumn("Units", typeof(string)));
            list.Columns.Add(new DataColumn("DrugIndicator", typeof(string)));
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
                cmd = Cm.MstDrugDetail.GetDrugDetail(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Module", CacheDbType.NVarChar).Value = Module;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DrugCode"].ToString(), cdr["DrugName"].ToString(), cdr["DrugSpec"].ToString(), cdr["Units"].ToString(), cdr["DrugIndicator"].ToString(), cdr["CurativeEffect"].ToString(), cdr["SideEffect"].ToString(), cdr["Instruction"].ToString(), cdr["HealthEffect"].ToString(), cdr["Unit"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstDrugDetail.GetDrugDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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