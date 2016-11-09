using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.CommonLibrary;
using System.Data;
using InterSystems.Data.CacheClient;
using InterSystems.Data.CacheTypes;

namespace WebService.DataMethod
{
    public class TmpOperationDict
    {
        public static DataTable GetTmpOperationByStatus(DataConnection pclsCache, int status)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("InputCode", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("Status", typeof(int)));

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
                cmd = Tmp.OperationDict.GetListByStatus(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("Status", CacheDbType.Int).Value = status;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["InputCode"].ToString(), cdr["Description"].ToString(), status);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Tmp.OperationDict.GetList", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        public static string GetName(DataConnection pclsCache, string HospitalCode, string Code)
        {
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }
                string Name = Tmp.OperationDict.GetName(pclsCache.CacheConnectionObject, HospitalCode, Code);
                return Name;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "TmpOperationDict.GetName", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        public static bool ChangeStatus(DataConnection pclsCache, string HospitalCode, string Code, int Status)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;
                }
                int flag = (int)Tmp.OperationDict.ChangeStatus(pclsCache.CacheConnectionObject, HospitalCode, Code, Status);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "获取名称失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Tmp.OperationDict.ChangeStatus", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
    }
}