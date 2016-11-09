using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.CommonLibrary;
using InterSystems.Data.CacheTypes;
using System.Data;
using InterSystems.Data.CacheClient;

namespace WebService.DataMethod
{
    public class PsTarget
    {
        //SetData TDY 2015-04-07
        public static int SetData(DataConnection pclsCache, string Plan, string Id, string Type, string Code, string Value, string Origin, string Instruction, string Unit, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Ps.Target.SetData(pclsCache.CacheConnectionObject, Plan, Id, Type, Code, Value, Origin, Instruction, Unit, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetTargetById TDY 2015-04-07
        public static CacheSysList GetTargetById(DataConnection pclsCache, string Plan, string Id)
        {
            CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = Ps.Target.GetTargetById(pclsCache.CacheConnectionObject, Plan, Id);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.GetTargetById", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        ////GetTargetByCode TDY 2015-04-07
        //public static CacheSysList GetTargetByCode(DataConnection pclsCache, string Plan, string Type, string Code)
        //{
        //    CacheSysList ret = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);

        //    try
        //    {
        //        if (!pclsCache.Connect())
        //        {
        //            return ret;
        //        }

        //        ret = Ps.Target.GetTargetByCode(pclsCache.CacheConnectionObject, Plan, Type, Code);
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.GetTargetByCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return ret;
        //    }
        //    finally
        //    {
        //        pclsCache.DisConnect();
        //    }
        //}

        //GetPsTarget TDY 2015-04-07
        public static DataTable GetPsTarget(DataConnection pclsCache, string PlanNo)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Id", typeof(string)));
            list.Columns.Add(new DataColumn("Type", typeof(string)));
            list.Columns.Add(new DataColumn("TypeName", typeof(string)));
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("CodeName", typeof(string)));
            list.Columns.Add(new DataColumn("Value", typeof(string)));
            list.Columns.Add(new DataColumn("Origin", typeof(string)));
            list.Columns.Add(new DataColumn("Instruction", typeof(string)));
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
                cmd = Ps.Target.GetPsTarget(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Id"].ToString(), cdr["Type"].ToString(), cdr["TypeName"].ToString(), cdr["Code"].ToString(), cdr["CodeName"].ToString(), cdr["Value"].ToString(), cdr["Origin"].ToString(), cdr["Instruction"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.GetPsTarget", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //TargetDisplay TDY 2015-04-07
        public static DataTable TargetDisplay(DataConnection pclsCache, string PlanNo)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("TargetID", typeof(string)));
            list.Columns.Add(new DataColumn("TargetIns", typeof(string)));
            list.Columns.Add(new DataColumn("TargetVal", typeof(string)));
            list.Columns.Add(new DataColumn("Origin", typeof(string)));
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
                cmd = Ps.Target.TargetDisplay(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("PlanNo", CacheDbType.NVarChar).Value = PlanNo;

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["TargetID"].ToString(), cdr["TargetIns"].ToString(), cdr["TargetVal"].ToString(), cdr["Origin"].ToString(), cdr["Unit"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.TargetDisplay", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetTargetByCode LS 2015-03-30  只针对一种参数   使用
        public static CacheSysList GetTargetByCode(DataConnection pclsCache, string PlanNo, string Type, string Code)
        {
            try
            {
                CacheSysList CacheList = new InterSystems.Data.CacheTypes.CacheSysList(System.Text.Encoding.Unicode, true, true);
                if (!pclsCache.Connect())
                {
                    return null;
                }
                CacheList = Ps.Target.GetTargetByCode(pclsCache.CacheConnectionObject, PlanNo, Type, Code);
                return CacheList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsTarget.GetTargetByCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        // 施宇帆 2015-04-26 获取某计划下某任务的目标值
        public static string GetValueByPlanNoAndId(DataConnection pclsCache, string PlanNo, string Id)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Ps.Target.GetValueByPlanNoAndId(pclsCache.CacheConnectionObject, PlanNo, Id);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.Target.GetValueByPlanNoAndId", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

    }
}