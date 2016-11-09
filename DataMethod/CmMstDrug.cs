using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
namespace WebService.DataMethod
{
    public class CmMstDrug
    {
        public static int SetData(DataConnection pclsCache, string DRUGCODE, string DRUGNAME, string DRUGSPEC, string UNITS, string DRUGINDICATOR, string INPUTCODE)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstDrug.SetData(pclsCache.CacheConnectionObject, DRUGCODE, DRUGNAME, DRUGSPEC, UNITS, DRUGINDICATOR, INPUTCODE);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstDrug.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetDrugName 获取某药物名称 LS 2015-04-18
        public static string GetDrugName(DataConnection pclsCache, string DrugCode)
        {
            try
            {
                string DrugName = "";
                if (!pclsCache.Connect())
                {
                    return null;
                }
                DrugName = Cm.MstDrug.GetName(pclsCache.CacheConnectionObject, DrugCode);
                return DrugName;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstDrug.GetDrugName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetDrugNameList 获取所有药品名称列表 CSQ 2015-05-12
        public static DataTable GetDrugNameList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("DrugCode", typeof(string)));
            list.Columns.Add(new DataColumn("DrugName", typeof(string)));
            list.Columns.Add(new DataColumn("DrugSpec", typeof(string)));
            list.Columns.Add(new DataColumn("Units", typeof(string)));
            list.Columns.Add(new DataColumn("InputCode", typeof(string)));

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
                cmd = Cm.MstDrug.GetNameListByCode(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["DRUGCODE"].ToString(), cdr["DRUGNAME"].ToString(), cdr["DRUGSPEC"].ToString(), cdr["UNITS"].ToString(), cdr["INPUTCODE"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstDrug.GetDrugNameList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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