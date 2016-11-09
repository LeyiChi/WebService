using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;

namespace WebService.DataMethod
{
    public class PsClinicalInfo
    {
        //SetData  LS 2014-12-1
        public static bool SetData(DataConnection pclsCache, string UserId, int VisitId, int VisitType, int AdmissionDate, int DischargeDate, string HospitalCode, string Department, string piUserId, string piTerminalName, string piTerminalIP, int piDeviceType)
        {
            bool IsSaved = false;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return IsSaved;

                }
                int flag = (int)Ps.ClinicalInfo.SetData(pclsCache.CacheConnectionObject, UserId, VisitId, VisitType, AdmissionDate, DischargeDate, HospitalCode, Department, piUserId, piTerminalName, piTerminalIP, piDeviceType);
                if (flag == 1)
                {
                    IsSaved = true;
                }
                return IsSaved;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return IsSaved;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //Delete   LS 2014-12-1
        public static int Delete(DataConnection pclsCache, string UserId, int VisitId)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return ret;

                }
                ret = (int)Ps.ClinicalInfo.Delete(pclsCache.CacheConnectionObject, UserId, VisitId);
                return ret;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "保存失败！");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.Delete", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //GetClinicalInfo   LS 2014-12-1
        public static DataTable GetClinicalInfo(DataConnection pclsCache, string UserId)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("VisitId", typeof(int)));
            list.Columns.Add(new DataColumn("VisitType", typeof(int)));
            list.Columns.Add(new DataColumn("VisitTypeName", typeof(string)));
            list.Columns.Add(new DataColumn("AdmissionDate", typeof(int)));
            list.Columns.Add(new DataColumn("DischargeDate", typeof(int)));
            list.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
            list.Columns.Add(new DataColumn("HospitalName", typeof(string)));
            list.Columns.Add(new DataColumn("Department", typeof(string)));
            list.Columns.Add(new DataColumn("DepartmentName", typeof(string)));

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
                cmd = Ps.ClinicalInfo.GetClinicalInfo(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    int DischargeDate;
                    if (cdr["DischargeDate"].ToString() == "")
                    {
                        DischargeDate = 0;
                    }
                    else
                    {
                        DischargeDate = Convert.ToInt32(cdr["DischargeDate"]);
                    }
                    list.Rows.Add(Convert.ToInt32(cdr["VisitId"]), Convert.ToInt32(cdr["VisitType"]), cdr["VisitTypeName"].ToString(), Convert.ToInt32(cdr["AdmissionDate"]),
                                  DischargeDate, cdr["HospitalCode"].ToString(), cdr["HospitalName"].ToString(), cdr["Department"].ToString(), cdr["DepartmentName"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetClinicalInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        #region LS 时间轴 2015/03/11

        //住院（入院、出院）、门诊、急诊——住院拆分成入院、出院   再排序  共取出Num条
        public static DataTable GetClinicalInfoNum(DataConnection pclsCache, string UserId, DateTime AdmissionDate, DateTime ClinicDate, int Num)
        {
            //实际最终输出
            DataTable NewTable = new DataTable(); //最终输出（表格形式）【总不为空：会输出标记AdmissionDateMark和ClinicDateMark】
            DateTime AdmissionDateMark = new DateTime();  //指针标记 放在NewTable最后     住院
            DateTime ClinicDateMark = new DateTime();   //门诊

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }

                DataTable list = new DataTable();
                list.Columns.Add(new DataColumn("精确时间", typeof(DateTime)));       //精确时间 到s  
                list.Columns.Add(new DataColumn("类型", typeof(string)));            //区分标志   目前只能  入院、出院、门诊、急诊、当前住院中 
                list.Columns.Add(new DataColumn("VisitId", typeof(string)));         //阶段区分
                list.Columns.Add(new DataColumn("事件", typeof(string)));            // 主要指地点：HospitalName + DepartmentName+（类型）
                //list.Columns.Add(new DataColumn("Key", typeof(string)));          //搜索标志、关键属性：UserId+DateSort  可能后期开放

                NewTable = list.Clone();


                #region 住院、门诊 取数据，放在list中

                //Ps.InPatientInfo  取入院时间与出院时间  
                cmd = new CacheCommand();
                cmd = Ps.InPatientInfo.GetInfobyDate(pclsCache.CacheConnectionObject);  //只取了Num数量的VId,还需要取完整
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("AdmissionDate", CacheDbType.NVarChar).Value = AdmissionDate;
                cmd.Parameters.Add("Num", CacheDbType.NVarChar).Value = Num;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //住院-入院（AdmissionDate肯定不为空）  
                    list.Rows.Add(Convert.ToDateTime(cdr["AdmissionDate"]), "入院", cdr["VisitId"].ToString(), cdr["HospitalName"].ToString() + "：" + cdr["DepartmentName"].ToString() + "（入院）");


                    //住院-出院（DischargeDate为1900/1/1 0:00:00，表示目前正在住院）  【注意：应该取该Vid下最大sortNo】   出院时间必须提前取出，考虑门诊在住院期间的情况
                    DateTime DischargeDate;
                    DischargeDate = (DateTime)Ps.InPatientInfo.GetDischargeDate(pclsCache.CacheConnectionObject, UserId, cdr["VisitId"].ToString());
                    if (DischargeDate.ToString() == "9999/1/1 0:00:00")
                    {
                        DischargeDate = DateTime.Now; //取当前时间
                        list.Rows.Add(DischargeDate, "当前住院中", cdr["VisitId"].ToString(), cdr["HospitalName"].ToString() + "：" + cdr["DepartmentName"].ToString() + "（当前住院中）");
                    }
                    else
                    {
                        //已经出院
                        list.Rows.Add(DischargeDate, "出院", cdr["VisitId"].ToString(), cdr["HospitalName"].ToString() + "：" + cdr["DepartmentName"].ToString() + "（出院）");
                    }

                }

                //Ps.OutPatientInfo 包括门诊和急诊
                cmd = new CacheCommand();
                cmd = Ps.OutPatientInfo.GetInfobyDate(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("ClinicDate", CacheDbType.NVarChar).Value = ClinicDate;
                cmd.Parameters.Add("Num", CacheDbType.NVarChar).Value = Num;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    //就诊时间（ClinicDate肯定不为空）
                    //门诊
                    if (cdr["VisitId"].ToString().Substring(0, 1) == "O")
                    {
                        list.Rows.Add(Convert.ToDateTime(cdr["ClinicDate"]), "门诊", cdr["VisitId"].ToString(), cdr["HospitalName"].ToString() + "：" + cdr["DepartmentName"].ToString() + "（门诊）");
                    }
                    //急诊
                    else
                    {
                        list.Rows.Add(Convert.ToDateTime(cdr["ClinicDate"]), "急诊", cdr["VisitId"].ToString(), cdr["HospitalName"].ToString() + "：" + cdr["DepartmentName"].ToString() + "（急诊）");
                    }
                }

                #endregion

                #region list有数据时→排序,取Num条，并确定标记 （注意：极端情况-住院期间嵌套很多门诊  解决方案，后期判断取够或到底为止，满足 门诊日期<入院日期）

                //就诊有数据时，排序→确定标记（取出<=Num条数据）
                if (list.Rows.Count > 0)   //保证有数据
                {
                    //住院、门诊按时间排序
                    DataView dv = list.DefaultView;
                    dv.Sort = "精确时间 desc";             //时间轴需要倒序
                    DataTable dtNew = dv.ToTable();

                    int NumMark = 0;
                    int TypeMark = 0;     //TypeMark：遇到入院，变为1；出院变为0 （确定是否在住院期间）
                    int j = 0;
                    int CoNum = 0;  //从dtNew中拿出的实际条数
                    for (j = 0; j < dtNew.Rows.Count; j++)  //for循环是在，内容执行完成后，j才加1；若中途break
                    {
                        if ((dtNew.Rows[j]["类型"].ToString() == "出院") || (dtNew.Rows[j]["类型"].ToString() == "当前住院中"))
                        {
                            TypeMark = 1; //表明在住院
                        }
                        if (dtNew.Rows[j]["类型"].ToString() == "入院")
                        {
                            TypeMark = 0; //表明某一住院阶段结束
                        }
                        if (((dtNew.Rows[j]["类型"].ToString() == "门诊") && (TypeMark == 0)) || ((dtNew.Rows[j]["类型"].ToString() == "急诊") && (TypeMark == 0))) { NumMark++; } //在住院期间的门诊不作为NumMark的计数
                        if (dtNew.Rows[j]["类型"].ToString() == "入院") { NumMark++; }

                        if (NumMark == Num)
                        {
                            CoNum = j + 1; //break的时候 j不会继续自加直接跳出for循环
                            break;
                        }
                    }

                    if (CoNum == 0)   //不是所需条数达到Num、break的情况
                    {
                        CoNum = j;
                    }

                    //分析：最后一条 要么是门诊（不在住院阶段），要么入院



                    //表格输出（注意入院和出院拆开了，一条变两条影响计数，所以用j,而不是Num)
                    DataRow[] rows = list.Select("1=1");
                    for (int i = 0; i < CoNum; i++)     //j+1的情况：是有剩余   【有问题：只有门诊的话 ，只取到j】
                    {
                        //NewTable.ImportRow((DataRow)rows[i]);
                        NewTable.Rows.Add(rows[i].ItemArray);
                    }


                    //获取两张表的当前标记  由于表默认取出（是按时间倒序，所以应该去最后一条，才是标记
                    //住院
                    string condition = " 类型 = '入院' ";
                    DataRow[] InRows = NewTable.Select(condition, "精确时间 desc");
                    if (InRows == null || InRows.Length == 0)
                    {
                        AdmissionDateMark = AdmissionDate;

                    }
                    else
                    {
                        AdmissionDateMark = DateTime.Parse(InRows[InRows.Length - 1]["精确时间"].ToString());
                    }

                    //门诊
                    condition = " 类型 = '门诊' or 类型 = '急诊'";
                    DataRow[] OutRows = NewTable.Select(condition, "精确时间 desc");
                    if (OutRows == null || OutRows.Length == 0)
                    {
                        ClinicDateMark = ClinicDate;
                    }
                    else
                    {
                        ClinicDateMark = DateTime.Parse(OutRows[OutRows.Length - 1]["精确时间"].ToString());
                    }


                    //将两张表当前指针标记，放在表的最后，用VisitTypeName="标志"，做区分
                    NewTable.Rows.Add(AdmissionDateMark, ClinicDateMark.ToString(), "指针", "");
                }
                else
                {
                    NewTable.Rows.Add(AdmissionDate, ClinicDate.ToString(), "指针", "");
                }

                #endregion

                //DateSort、LeftShow、VisitTypeName、VisitId、Location

                return NewTable;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetClinicalInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //住院-转科处理  
        public static DataTable GetTransClinicalInfo(DataConnection pclsCache, string UserId, string VisitId)
        {
            //最终输出
            DataTable DT_Clinical_Trans = new DataTable();
            DT_Clinical_Trans.Columns.Add(new DataColumn("精确时间", typeof(DateTime)));       //精确时间 转科的首要时间 定为转入时间  
            DT_Clinical_Trans.Columns.Add(new DataColumn("类型", typeof(string)));            //区分标志： 转科
            DT_Clinical_Trans.Columns.Add(new DataColumn("VisitId", typeof(string)));         //即输入
            DT_Clinical_Trans.Columns.Add(new DataColumn("事件", typeof(string)));            // 主要指地点：从HospitalName + DepartmentName转出，哪里转入+（转科）
            //list.Columns.Add(new DataColumn("Key", typeof(string)));          //搜索标志、关键属性：UserId+DateSort  可能后期开放

            CacheCommand cmd = null;
            CacheDataReader cdr = null;
            try
            {
                if (!pclsCache.Connect())
                {
                    //MessageBox.Show("Cache数据库连接失败");
                    return null;
                }

                //取出原始数据
                DataTable DT_Clinical_Temp = new DataTable();
                DT_Clinical_Temp.Columns.Add(new DataColumn("SortNo", typeof(int)));
                DT_Clinical_Temp.Columns.Add(new DataColumn("AdmissionDate", typeof(DateTime)));
                DT_Clinical_Temp.Columns.Add(new DataColumn("DischargeDate", typeof(DateTime)));
                //DT_Clinical_Temp.Columns.Add(new DataColumn("HospitalCode", typeof(string)));
                DT_Clinical_Temp.Columns.Add(new DataColumn("HospitalName", typeof(string)));
                //DT_Clinical_Temp.Columns.Add(new DataColumn("Department", typeof(string)));
                DT_Clinical_Temp.Columns.Add(new DataColumn("DepartmentName", typeof(string)));


                cmd = new CacheCommand();
                cmd = Ps.InPatientInfo.GetInfobyVisitId(pclsCache.CacheConnectionObject);
                cmd.Parameters.Add("UserId", CacheDbType.NVarChar).Value = UserId;
                cmd.Parameters.Add("VisitId", CacheDbType.NVarChar).Value = VisitId;
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    DT_Clinical_Temp.Rows.Add(Convert.ToInt32(cdr["SortNo"]), Convert.ToDateTime(cdr["AdmissionDate"]), Convert.ToDateTime(cdr["DischargeDate"]), cdr["HospitalName"].ToString(), cdr["DepartmentName"].ToString());
                }

                //有转科 
                //转科处理  转科内容：什么时候从哪里转出，什么时候转到哪
                if (DT_Clinical_Temp.Rows.Count > 1)
                {
                    for (int n = 0; n < DT_Clinical_Temp.Rows.Count - 1; n++)
                    {
                        //医院+科室
                        //string things = DT_Clinical_Temp.Rows[n]["HospitalName"].ToString() + DT_Clinical_Temp.Rows[n]["DepartmentName"].ToString() + "(转出)" + "  ";
                        //things += DT_Clinical_Temp.Rows[n + 1]["HospitalName"].ToString() + DT_Clinical_Temp.Rows[n + 1]["DepartmentName"].ToString() + "(转入)";

                        //只科室
                        string things = DT_Clinical_Temp.Rows[n]["DepartmentName"].ToString() + "(转出)" + "  ";
                        things += DT_Clinical_Temp.Rows[n + 1]["DepartmentName"].ToString() + "(转入)";
                        DT_Clinical_Trans.Rows.Add(Convert.ToDateTime(DT_Clinical_Temp.Rows[n + 1]["AdmissionDate"]), "转科", VisitId, things);
                    }
                }

                return DT_Clinical_Trans;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetClinicalInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //检查化验等信息
        public static DataTable GetOtherTable(DataConnection pclsCache, string UserId, string VisitId)
        {
            //输出表
            DataTable dt_Out = new System.Data.DataTable();
            dt_Out.Columns.Add(new DataColumn("精确时间", typeof(DateTime)));  //年月日时分秒
            dt_Out.Columns.Add(new DataColumn("类型", typeof(string)));        //早期排序，目前可能不需要了 那来干嘛？
            dt_Out.Columns.Add(new DataColumn("VisitId", typeof(string)));
            dt_Out.Columns.Add(new DataColumn("事件", typeof(string)));
            dt_Out.Columns.Add(new DataColumn("关键属性", typeof(string)));     //取当前标记时方便区分  类型+vid+时间-这样基本就能确定了  或者主键  


            try
            {

                //诊断
                DataTable DiagnosisDT = new System.Data.DataTable();
                DiagnosisDT = PsDiagnosis.GetDiagnosisInfo(pclsCache, UserId, VisitId);
                for (int i = 0; i < DiagnosisDT.Rows.Count; i++)
                {
                    dt_Out.Rows.Add(Convert.ToDateTime(DiagnosisDT.Rows[i]["RecordDate"]), "诊断", VisitId, "诊断：" + DiagnosisDT.Rows[i]["TypeName"].ToString(), "DiagnosisInfo" + "|" + VisitId + "|" + Convert.ToDateTime(DiagnosisDT.Rows[i]["RecordDate"].ToString()).ToString("yyyy-MM-ddHH:mm:ss"));
                }

                //检查
                DataTable ExaminationDT = new System.Data.DataTable();
                ExaminationDT = PsExamination.GetExaminationList(pclsCache, UserId, VisitId);
                for (int i = 0; i < ExaminationDT.Rows.Count; i++)
                {
                    dt_Out.Rows.Add(Convert.ToDateTime(ExaminationDT.Rows[i]["ExamDate"]), "检查", VisitId, "检查：" + ExaminationDT.Rows[i]["ExamTypeName"].ToString(), "ExaminationInfo" + "|" + VisitId + "|" + Convert.ToDateTime(ExaminationDT.Rows[i]["ExamDate"].ToString()).ToString("yyyy-MM-ddHH:mm:ss"));
                }

                //化验
                DataTable LabTestDT = new System.Data.DataTable();
                LabTestDT = PsLabTest.GetLabTestList(pclsCache, UserId, VisitId);
                for (int i = 0; i < LabTestDT.Rows.Count; i++)
                {
                    dt_Out.Rows.Add(Convert.ToDateTime(LabTestDT.Rows[i]["LabTestDate"]), "化验", VisitId, "化验：" + LabTestDT.Rows[i]["LabItemName"].ToString(), "LabTestInfo" + "|" + VisitId + "|" + Convert.ToDateTime(LabTestDT.Rows[i]["LabTestDate"].ToString()).ToString("yyyy-MM-ddHH:mm:ss"));
                }

                //用药


                DataTable DrugRecordDT = new System.Data.DataTable();
                DrugRecordDT = PsDrugRecord.GetDrugRecord(pclsCache, UserId, VisitId);
                for (int i = 0; i < DrugRecordDT.Rows.Count; i++)
                {
                    dt_Out.Rows.Add(Convert.ToDateTime(DrugRecordDT.Rows[i]["StartDateTime"]), "用药", VisitId, "用药：" + DrugRecordDT.Rows[i]["HistoryContent"].ToString(), "DrugRecord" + "|" + VisitId + "|" + Convert.ToDateTime(DrugRecordDT.Rows[i]["StartDateTime"].ToString()).ToString("yyyy-MM-ddHH:mm:ss"));
                }

                return dt_Out;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetClinicalInfo", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {

            }
        }


        //颜色分配：根据首标签决定
        public static string GetColor(string type)
        {
            string colorShow = "clolor_default";
            try
            {
                switch (type)
                {
                    case "入院": colorShow = "clolor_in";
                        break;
                    case "出院": colorShow = "clolor_in";
                        break;
                    case "转科": colorShow = "clolor_trans";
                        break;
                    case "门诊": colorShow = "clolor_out";
                        break;
                    case "急诊": colorShow = "clolor_emer";
                        break;
                    case "住院中": colorShow = "clolor_inHos";
                        break;
                    case "当前住院中": colorShow = "clolor_inHos";
                        break;
                    default: break;
                }

                return colorShow;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetColor", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }


        //检查化验详细信息查看
        public static DataTable GetClinicInfoDetail(DataConnection pclsCache, string UserId, string Type, string VisitId, string Date)
        {
            DataTable list = new DataTable();
            string condition = "";
            try
            {
                switch (Type)
                {
                    //case "ClinicalInfo": list = _ServicesSoapClient.GetClinicalInfo(PatientId); //就诊表 
                    //break;
                    case "DiagnosisInfo": list = PsDiagnosis.GetDiagnosisInfo(pclsCache, UserId, VisitId);//诊断表
                        condition = "RecordDateCom = '" + Date + "'";
                        break;
                    case "ExaminationInfo": list = PsExamination.GetExaminationList(pclsCache, UserId, VisitId); //检查表（有子表）
                        condition = "ExamDateCom = '" + Date + "'";
                        break;
                    case "LabTestInfo": list = PsLabTest.GetLabTestList(pclsCache, UserId, VisitId); //化验表（有子表）
                        condition = "LabTestDateCom = '" + Date + "'";
                        break;
                    //case "DrugRecord": list = PsDrugRecord.GetDrugRecord(pclsCache, UserId, VisitId); //用药
                    case "DrugRecord": list = PsDrugRecord.GetDrugRecordList(pclsCache, UserId, VisitId); //用药
                        condition = "StartDateTimeCom = '" + Date + "'";
                        break;
                    default: break;
                }

                //list肯定不为空

                DataTable newdt = new DataTable();
                newdt = list.Clone();
                DataRow[] rows = list.Select(condition);
                foreach (DataRow row in rows)
                {
                    newdt.Rows.Add(row.ItemArray);
                }
                return newdt;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "PsClinicalInfo.GetClinicInfoDetail", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //获取住院的下一日期
        public static string GetNextInDate(DataConnection pclsCache, string UserId, string AdmissionDate)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Ps.InPatientInfo.GetNextDatebyDate(pclsCache.CacheConnectionObject, UserId, Convert.ToDateTime(AdmissionDate).ToString("yyyy-MM-dd HH:mm:ss"));
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.InPatientInfo.GetNextDatebyDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //获取门诊的下一日期
        public static string GetNextOutDate(DataConnection pclsCache, string UserId, string ClinicDate)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Ps.OutPatientInfo.GetNextDatebyDate(pclsCache.CacheConnectionObject, UserId, Convert.ToDateTime(ClinicDate).ToString("yyyy-MM-dd HH:mm:ss"));
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Ps.OutPatientInfo.GetNextDatebyDate", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }
        #endregion

    }
}