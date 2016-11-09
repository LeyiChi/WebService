using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class MpDrugCmp
    {
        //SetData WY 2015-07-10
        public static bool SetData(DataConnection pclsCache, string HospitalCode, string HZCode, string Code, string Redundance,string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Mp.DrugCmp.SetData(pclsCache.CacheConnectionObject, HospitalCode, HZCode, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MpDrugCmp.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 
        public static bool Delete(DataConnection pclsCache, string HospitalCode, string HZCode)
        {
            bool IsSaved = false;
            try
            {
                //if (!pclsCache.Connect())
                //{
                //    //MessageBox.Show("Cache数据库连接失败");
                //    return IsSaved;
                //}
                //int flag = (int)Mp.DrugCmp.Delete(pclsCache.CacheConnectionObject, HospitalCode, HZCode);
                //if (flag == 1)
                //{
                //    IsSaved = true;
                //}
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MpDrugCmp.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetListByStatus WY 2015-07-10
        public static DataTable GetMpDrugCmp(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("DrugCode", typeof(string)));
            list.Columns.Add(new DataColumn("DrugName", typeof(string)));
            list.Columns.Add(new DataColumn("DrugSpec", typeof(string)));
            list.Columns.Add(new DataColumn("HZCode", typeof(string)));
            list.Columns.Add(new DataColumn("HZName", typeof(string)));
            list.Columns.Add(new DataColumn("Redundance", typeof(string)));

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
                cmd = Mp.DrugCmp.GetMpDrugCmp(pclsCache.CacheConnectionObject);
      
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["DrugCode"], cdr["DrugName"].ToString(), cdr["DrugSpec"].ToString(), cdr["HZCode"].ToString(), cdr["HZName"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "MpDrugCmp.GetMpDrugCmp", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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