using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class MpLabTestItemsCmp
    {
        //SetData WY 2015-07-10
        public static bool SetData(DataConnection pclsCache, string HospitalCode, string HZCode, string Type, string Code, string Redundance, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Mp.LabTestItemsCmp.SetData(pclsCache.CacheConnectionObject, HospitalCode, HZCode, Type, Code, Redundance, revUserId, TerminalName, TerminalIP, DeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "LabTestItemsCmp.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // ChangeStatus WY 2015-07-10
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
                //int flag = (int)Mp.LabTestItemsCmp.Delete(pclsCache.CacheConnectionObject, HospitalCode, HZCode);
                //if (flag == 1)
                //{
                //    IsSaved = true;
                //}
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "LabTestItemsCmp.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // GetListByStatus WY 2015-07-10
        public static DataTable GetMpLabTestItemsCmp(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(int)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
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
                cmd = Mp.LabTestItemsCmp.GetMpLabTestItemsCmp(pclsCache.CacheConnectionObject);

                //cmd.Parameters.Add("InvalidFlag", CacheDbType.Int).Value = InvalidFlag;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Type"], cdr["TypeName"].ToString(), cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["HZCode"].ToString(), cdr["HZName"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "LabTestItemsCmp.GetMpLabTestItemsCmp", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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