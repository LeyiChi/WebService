using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
namespace WebService.DataMethod
{
    public class PsPatient2VS
    {
        //SetData  LS 2014-12-1
        public static bool SetData(DataConnection pclsCache, string UserId, string VitalSignsType, string VitalSignsCode, int InvalidFlag, int SortNo, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.Patient2VS.SetData(pclsCache.CacheConnectionObject, UserId, VitalSignsType, VitalSignsCode, InvalidFlag, SortNo, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPatient2VS.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //DeletePatient2VSInfo   LS 2014-12-1
        public static int DeletePatient2VSInfo(DataConnection pclsCache, string UserId, string VitalSignsType, string VitalSignsCode, string revUserId, string TerminalName, string TerminalIP, int DeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.Patient2VS.DeletePatient2VSInfo(pclsCache.CacheConnectionObject, UserId, VitalSignsType, VitalSignsCode, revUserId, TerminalName, TerminalIP, DeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPatient2VS.DeletePatient2VSInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetPatient2VS   LS 2014-12-1
        public static DataTable GetPatient2VS(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VitalSignsType", typeof(string)));
            list.Columns.Add(new DataColumn("VitalSignsTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("VitalSignsCode", typeof(string)));
            list.Columns.Add(new DataColumn("VitalSignsName", typeof(string)));
            list.Columns.Add(new DataColumn("InvalidFlag", typeof(int)));
            list.Columns.Add(new DataColumn("SortNo", typeof(int)));

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
                cmd = Ps.Patient2VS.GetPatient2VS(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int InvalidFlag;
                    if (cdr["InvalidFlag"].ToString() == "")
                    {
                        InvalidFlag = 0;
                    }
                    else
                    {
                        InvalidFlag = Convert.ToInt32(cdr["InvalidFlag"]);
                    }

                    int SortNo;
                    if (cdr["SortNo"].ToString() == "")
                    {
                        SortNo = 0;
                    }
                    else
                    {
                        SortNo = Convert.ToInt32(cdr["SortNo"]);
                    }
                    list.Rows.Add(cdr["VitalSignsType"].ToString(), cdr["VitalSignsTypeName"].ToString(), cdr["VitalSignsCode"].ToString(), cdr["VitalSignsName"].ToString(), InvalidFlag, SortNo);
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsPatient2VS.GetPatient2VS", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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