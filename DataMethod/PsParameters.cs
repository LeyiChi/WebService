using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;


namespace WebService.DataMethod
{
    public class PsParameters
    {
        // GetParameters ZAM 2014-12-4
        public static DataTable GetParameters(DataConnection pclsCache, string Indicators)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("Unit", typeof(string)));

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
                cmd = Ps.Parameters.GetParameters(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Indicators", CacheDbType.NVarChar).Value = Indicators;
                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Name"].ToString(), cdr["Value"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsParameters.GetParameters", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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