using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using System.Data;
using WebService.CommonLibrary;
using WebService.DataClass;

namespace WebService.DataMethod
{
    public class CmMstBloodPressure
    {

        //GetDescription SYF 2015-04-22 根据收缩压获取血压等级说明
        public static string GetDescription(DataConnection pclsCache, int SBP)
        {
            string ret = "";
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (string)Cm.MstBloodPressure.GetDescription(pclsCache.CacheConnectionObject, SBP);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstBloodPressure.GetDescription", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //获取血压表全部信息 2015-05-29 GL
        public static DataTable GetBloodPressureList(DataConnection pclsCache)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Code", typeof(string)));
            list.Columns.Add(new DataColumn("Name", typeof(string)));
            list.Columns.Add(new DataColumn("Description", typeof(string)));
            list.Columns.Add(new DataColumn("SBP", typeof(string)));
            list.Columns.Add(new DataColumn("DBP", typeof(string)));
            list.Columns.Add(new DataColumn("PatientClass", typeof(string)));
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
                cmd = Cm.MstBloodPressure.GetBPGrades(pclsCache.CacheConnectionObject);

                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    list.Rows.Add(cdr["Code"].ToString(), cdr["Name"].ToString(), cdr["Description"].ToString(), cdr["SBP"].ToString(), cdr["DBP"].ToString(), cdr["PatientClass"].ToString(), cdr["Redundance"].ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstBloodPressure.GetBPGrades", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //SetData 血压表插数 2015-05-29 GL
        public static int SetBloodPressure(DataConnection pclsCache, string Code, string Name, string Description, int SBP, int DBP, string PatientClass, string Redundance)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstBloodPressure.SetData(pclsCache.CacheConnectionObject, Code, Name, Description, SBP, DBP, PatientClass, Redundance);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstBloodPressure.SetData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }

        //DeleteData 血压表删数 2015-05-29 GL
        public static int DeleteBloodPressure(DataConnection pclsCache, string Code)
        {
            int ret = 0;
            try
            {
                if (!pclsCache.Connect())
                {
                    return ret;
                }

                ret = (int)Cm.MstBloodPressure.DeleteData(pclsCache.CacheConnectionObject, Code);
                return ret;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Cm.MstBloodPressure.DeleteData", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return ret;
            }
            finally
            {
                pclsCache.DisConnect();
            }
        }


        #region 高血压模块 M1 LS

        //GetBPGrades LS 2015-03-27  从数据库获取血压分级规则
        public static List<MstBloodPressure> GetBPGrades(DataConnection pclsCache)
        {

            List<MstBloodPressure> result = new List<MstBloodPressure>();
            CacheCommand cmd = null;
            CacheDataReader cdr = null;

            try
            {
                if (!pclsCache.Connect())
                {
                    return null;
                }

                cmd = new CacheCommand();
                cmd = Cm.MstBloodPressure.GetBPGrades(pclsCache.CacheConnectionObject);
                cdr = cmd.ExecuteReader();
                while (cdr.Read())
                {
                    MstBloodPressure MstBloodPressure = new MstBloodPressure();
                    MstBloodPressure.Code = cdr["Code"].ToString();
                    MstBloodPressure.Name = cdr["Name"].ToString();
                    MstBloodPressure.Description = cdr["Description"].ToString();
                    MstBloodPressure.SBP = Convert.ToInt32(cdr["SBP"]);
                    MstBloodPressure.DBP = Convert.ToInt32(cdr["DBP"]);
                    MstBloodPressure.PatientClass = cdr["PatientClass"].ToString();
                    MstBloodPressure.Redundance = cdr["Redundance"].ToString();
                    result.Add(MstBloodPressure);

                }
                return result;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstBloodPressure.GetBPGrades", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
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

        //GetBPColor LS 2015-04-18  获取血压点 和 数据图区块的颜色  输入：血压值级别  点/区域  是否需要点和区域都有颜色？
        public static string GetBPColor(string name, string type)
        {
            string colorShow = "";  //默认颜色
            try
            {
                if (type == "bullet")
                {
                    colorShow = "#FFC78E";
                    switch (name)
                    {
                        case "很高": colorShow = "#930000";  //红色
                            break;
                        case "偏高": colorShow = "#CC0000";  //
                            break;
                        case "警戒": colorShow = "#0000cc"; //
                            break;
                        case "正常": colorShow = "#2894FF";  //
                            break;
                        case "偏低": colorShow = "#FFC78E";  //
                            break;
                        default: break;
                    }
                }
                else   //fill
                {
                    colorShow = "#8080C0";
                    switch (name)
                    {
                        case "很高": colorShow = "#FF0000";  //深红色
                            break;
                        case "偏高": colorShow = "#FF60AF";  //微红
                            break;
                        case "警戒": colorShow = "#FFA042"; //橙色
                            break;
                        case "正常": colorShow = "#00DB00";  //绿色
                            break;
                        case "偏低": colorShow = "#8080C0";  //微紫
                            break;
                        default: break;
                    }
                }

                return colorShow;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstBloodPressure.GetBPColor", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //GetSignBPGrade LS 2015-03-27  获取某血压值的级别   输出 Cm.MstBloodPressure.Name -很高、偏高、警戒、正常等
        public static string GetSignBPGrade(string BPType, int Value, List<MstBloodPressure> Reference)
        {
            //算法：将值Value与分级的值一一相比，注意  >Value；   <= Value <；   <=Value 等区间，确定其级别（取左边的值）  目前Name少了一个，暂时默认少了偏高
            string Name = "不明范围";  //待标记颜色
            try
            {
                if (BPType == "Bloodpressure_1")  //收缩压 3  标准肯定不只1个值！
                {

                    //按照血压规定  分级保证>2
                    if (Value < Reference[0].SBP)
                    {
                        Name = "错误";
                    }

                    if (Value > Reference[Reference.Count - 1].SBP)
                    {
                        Name = "错误";
                    }

                    if (Name == "不明范围")
                    {
                        if (Reference.Count >= 2)  //前两步已经保证了，数量肯定》2   
                        {
                            for (int i = 0; i <= Reference.Count - 2; i++)  //要求个数>=2
                            {
                                if ((Value >= Reference[i].SBP) && (Value < Reference[i + 1].SBP)) //左闭右开
                                {
                                    Name = Reference[i].Name;  //名字就低
                                    break;
                                }
                            }
                        }
                    }


                }
                else  //舒张压
                {
                    if (Value < Reference[0].DBP)
                    {
                        Name = "错误";
                    }

                    if (Value > Reference[Reference.Count - 1].DBP)
                    {
                        Name = "错误";
                    }

                    if (Name == "不明范围")
                    {
                        if (Reference.Count >= 2)  //前两步已经保证了，数量肯定》2
                        {
                            for (int i = 0; i <= Reference.Count - 2; i++)  //要求个数>=2
                            {
                                if ((Value >= Reference[i].DBP) && (Value < Reference[i + 1].DBP))
                                {
                                    Name = Reference[i].Name;
                                    break;
                                }
                            }
                        }
                    }
                }

                return Name;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstBloodPressure.GetSignBPGrade", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        //GetSignInfoByBP  LS 2015-06-28  生成收缩压/舒张压/脉率点图，并分级   需要三者拼接字段  注意：可能没有脉率任务 
        public static List<Graph> GetSignInfoByM1(DataConnection pclsCache, string UserId, string PlanNo, string Code, int StartDate, int EndDate, List<MstBloodPressure> reference)
        {
            List<Graph> graphList = new List<Graph>();

            //获取系统时间  数据库连接，这样写，应该对的
            string serverTime = "";
            if (pclsCache.Connect())
            {
                serverTime = Convert.ToDateTime(Cm.CommonLibrary.GetServerDateTime(pclsCache.CacheConnectionObject)).ToString("yyyyMMdd");

            }
            pclsCache.DisConnect();

            try
            {
                //输入的code是拼接字段
                //string[] strCode = Code.Split(new char[] { '|' });
                //string ItemType = strCode[0];
                //string ItemCode = strCode[1];

                //M1的关注含下列几表   PsCompliance.GetSignDetailByPeriod方法保证几表条数相等 即使某天没体征数据，也会输出""字符串  

                //收缩压表
                DataTable sysInfo = new DataTable();
                sysInfo = PsCompliance.GetSignDetailByPeriod(pclsCache, UserId, PlanNo, "Bloodpressure", "Bloodpressure_1", StartDate, EndDate);
                //RecordDate、RecordTime、Value、Unit

                //舒张压表
                DataTable diaInfo = new DataTable();
                diaInfo = PsCompliance.GetSignDetailByPeriod(pclsCache, UserId, PlanNo, "Bloodpressure", "Bloodpressure_2", StartDate, EndDate);

                //脉率表
                DataTable pulInfo = new DataTable();
                pulInfo = PsCompliance.GetSignDetailByPeriod(pclsCache, UserId, PlanNo, "Pulserate", "Pulserate_1", StartDate, EndDate);

                //三张表都有数据
                if ((sysInfo.Rows.Count == diaInfo.Rows.Count) && (sysInfo.Rows.Count == pulInfo.Rows.Count) && (sysInfo.Rows.Count > 0))
                {

                    for (int rowsCount = 0; rowsCount < sysInfo.Rows.Count; rowsCount++)
                    {
                        Graph Graph = new Graph();
                        Graph.Date = sysInfo.Rows[rowsCount]["RecordDate"].ToString();

                        #region 值、等级、颜色

                        //值、等级、颜色、描述文本
                        if ((Code == "Bloodpressure|Bloodpressure_1") && (reference != null)) //血压要求 reference 不为空
                        {

                            #region 收缩压
                            Graph.SignValue = "#";
                            if (sysInfo.Rows[rowsCount]["Value"].ToString() != "")
                            {
                                Graph.SignValue = sysInfo.Rows[rowsCount]["Value"].ToString();
                            }
                            //Graph.SignValue = sysInfo.Rows[rowsCount]["Value"].ToString();
                            if (Graph.SignValue != "#")
                            {
                                Graph.SignGrade = CmMstBloodPressure.GetSignBPGrade("Bloodpressure_1", Convert.ToInt32(Graph.SignValue), reference);
                                Graph.SignColor = CmMstBloodPressure.GetBPColor(Graph.SignGrade, "bullet");

                                //判断是否都有值
                                if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") && (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                                {
                                    Graph.SignDescription = "血压：<b><span style='font-size:14px;'>" + sysInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg<br>脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";
                                }
                                else if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") || (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() == ""))
                                {
                                    Graph.SignDescription = "血压：<b><span style='font-size:14px;'>" + sysInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg";
                                }
                                else if ((sysInfo.Rows[rowsCount]["Value"].ToString() == "") && (diaInfo.Rows[rowsCount]["Value"].ToString() == "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                                {
                                    Graph.SignDescription = "脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";
                                }
                                //Graph.SignDescription = "血压：<b><span style='font-size:14px;'>" + sysInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg<br>脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";
                                //"[[category]]<br>血压：<b><span style='font-size:14px;'>[[SBPvalue]] </span></b>/[[DBPvalue]]mmHg<br>脉搏：66次/分"
                            }

                            else
                            {
                                Graph.SignGrade = "";
                                Graph.SignColor = "";
                                Graph.SignDescription = "";
                            }
                            #endregion
                        }
                        else if ((Code == "Bloodpressure|Bloodpressure_2") && (reference != null))  //舒张压
                        {
                            #region 舒张压
                            Graph.SignValue = "#";
                            if (diaInfo.Rows[rowsCount]["Value"].ToString() != "")
                            {
                                Graph.SignValue = diaInfo.Rows[rowsCount]["Value"].ToString();
                            }
                            //Graph.SignValue = diaInfo.Rows[rowsCount]["Value"].ToString();
                            if (Graph.SignValue != "#")
                            {
                                Graph.SignGrade = CmMstBloodPressure.GetSignBPGrade("Bloodpressure_2", Convert.ToInt32(Graph.SignValue), reference);
                                Graph.SignColor = CmMstBloodPressure.GetBPColor(Graph.SignGrade, "bullet");
                                //Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/<b><span style='font-size:14px;'>" + diaInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>mmHg<br>脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";

                                //判断是否都有值
                                if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") && (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                                {
                                    Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/<b><span style='font-size:14px;'>" + diaInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>mmHg<br>脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";
                                }
                                else if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") || (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() == ""))
                                {
                                    Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/<b><span style='font-size:14px;'>" + diaInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>mmHg";
                                }
                                else if ((sysInfo.Rows[rowsCount]["Value"].ToString() == "") && (diaInfo.Rows[rowsCount]["Value"].ToString() == "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                                {
                                    Graph.SignDescription = "脉搏：" + pulInfo.Rows[rowsCount]["Value"].ToString() + "次/分";
                                }
                            }

                            else
                            {
                                Graph.SignGrade = "";
                                Graph.SignColor = "";
                                Graph.SignDescription = "";
                            }
                            #endregion
                        }
                        else if (Code == "Pulserate|Pulserate_1")  //脉率
                        {
                            #region 脉率
                            Graph.SignValue = "#";
                            if (pulInfo.Rows[rowsCount]["Value"].ToString() != "")
                            {
                                Graph.SignValue = pulInfo.Rows[rowsCount]["Value"].ToString();
                            }

                            //Graph.SignValue = pulInfo.Rows[rowsCount]["Value"].ToString();
                            //Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg<br>脉率：<b><span style='font-size:14px;'>" + pulInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>次/分";

                            //判断是否都有值
                            if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") && (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                            {
                                Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg<br>脉率：<b><span style='font-size:14px;'>" + pulInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>次/分";
                            }
                            else if ((sysInfo.Rows[rowsCount]["Value"].ToString() != "") || (diaInfo.Rows[rowsCount]["Value"].ToString() != "") && (pulInfo.Rows[rowsCount]["Value"].ToString() == ""))
                            {
                                Graph.SignDescription = "血压：" + sysInfo.Rows[rowsCount]["Value"].ToString() + "/" + diaInfo.Rows[rowsCount]["Value"].ToString() + "mmHg";
                            }
                            else if ((sysInfo.Rows[rowsCount]["Value"].ToString() == "") && (diaInfo.Rows[rowsCount]["Value"].ToString() == "") && (pulInfo.Rows[rowsCount]["Value"].ToString() != ""))
                            {
                                Graph.SignDescription = "脉率：<b><span style='font-size:14px;'>" + pulInfo.Rows[rowsCount]["Value"].ToString() + "</span></b>次/分";
                            }

                            if (Graph.SignValue != "#")
                            {
                                //脉率的分级 写死
                                if (Convert.ToDouble(Graph.SignValue) < 60)  //过慢
                                {
                                    Graph.SignGrade = "过慢";
                                    Graph.SignColor = "#8080C0"; //微紫
                                }
                                else if (Convert.ToDouble(Graph.SignValue) > 100) //过快
                                {
                                    Graph.SignGrade = "过快";
                                    Graph.SignColor = "#FF60AF";  //微红
                                }
                                else //成人 60~100之间包括端点 正常
                                {
                                    Graph.SignGrade = "正常";
                                    Graph.SignColor = "#00DB00";  //绿色
                                }

                            }
                            else
                            {
                                Graph.SignGrade = "";
                                Graph.SignColor = "";
                                Graph.SignDescription = "";
                            }
                            #endregion
                        }

                        #endregion

                        //形状
                        if (rowsCount != sysInfo.Rows.Count - 1)
                        {
                            Graph.SignShape = "round";
                            Graph.SignShape = "round";
                        }
                        else
                        {
                            if (serverTime == Graph.Date)  //当天的血压点形状用菱形
                            {
                                Graph.SignShape = "diamond";
                                Graph.SignShape = "diamond";
                            }
                            else
                            {
                                Graph.SignShape = "round";
                                Graph.SignShape = "round";
                            }

                        }

                        graphList.Add(Graph);
                    }

                }

                //有血压任务，没有脉率
                return graphList;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstBloodPressure.GetSignInfoByBP", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {

            }
        }

        //GetGuidesByCode LS 2015-06-28 输出用于图形的分级区域
        public static GraphGuide GetGuidesByCode(DataConnection pclsCache, string PlanNo, string Code, List<MstBloodPressure> Reference)
        {

            GraphGuide GraphGuide = new GraphGuide();   //输出

            List<Guide> GuideList = new List<Guide>();


            try
            {


                if ((Code == "Bloodpressure|Bloodpressure_1") && (Reference != null))
                {
                    #region 收缩压

                    GraphGuide.minimum = Convert.ToDouble(Reference[0].SBP);
                    GraphGuide.maximum = Convert.ToDouble(Reference[Reference.Count - 1].SBP);

                    InterSystems.Data.CacheTypes.CacheSysList SysTarget = null;
                    SysTarget = PsTarget.GetTargetByCode(pclsCache, PlanNo, "Bloodpressure", "Bloodpressure_1");
                    if (SysTarget != null)
                    {
                        //初始值
                        Guide originalGuide = new Guide();
                        originalGuide.value = SysTarget[4].ToString(); //值或起始值
                        originalGuide.toValue = "#CC0000";           //终值或""
                        originalGuide.label = "";      //中文定义 目标线、偏低、偏高等
                        //originalGuide.label = "起始" + "：" + originalGuide.value;
                        originalGuide.lineColor = "#FF5151";          //直线颜色  目标线  初始线
                        originalGuide.lineAlpha = "1";//直线透明度 0全透~1
                        originalGuide.dashLength = "8"; //虚线密度  4  8
                        originalGuide.color = "#CC0000";    //字体颜色
                        originalGuide.fontSize = "8"; //字体大小  默认14
                        originalGuide.position = "right"; //字体位置 right left
                        originalGuide.inside = "";//坐标系的内或外  false
                        originalGuide.fillAlpha = "";
                        originalGuide.fillColor = "";

                        GuideList.Add(originalGuide);
                        GraphGuide.original = originalGuide.value + "mmHg";


                        //目标值
                        Guide tagetGuide = new Guide();
                        tagetGuide.value = SysTarget[3].ToString();
                        tagetGuide.toValue = "#CC0000";
                        tagetGuide.label = "";
                        //tagetGuide.label = "目标" + "：" + tagetGuide.value;
                        tagetGuide.lineColor = "#CC0000";
                        tagetGuide.lineAlpha = "1";
                        tagetGuide.dashLength = "4";
                        tagetGuide.color = "#CC0000";
                        tagetGuide.fontSize = "14";
                        tagetGuide.position = "right";
                        tagetGuide.inside = "";
                        tagetGuide.fillAlpha = "";
                        tagetGuide.fillColor = "";

                        GuideList.Add(tagetGuide);
                        GraphGuide.target = tagetGuide.value + "mmHg";
                    }

                    //风险范围
                    for (int i = 0; i <= Reference.Count - 2; i++)
                    {
                        //收缩压
                        Guide SysGuide = new Guide();
                        SysGuide.value = Reference[i].SBP.ToString(); //起始值
                        SysGuide.toValue = Reference[i + 1].SBP.ToString();                //终值
                        SysGuide.label = Reference[i].Name;          //偏低、偏高等
                        SysGuide.lineColor = "";     //直线颜色  目标线  初始线
                        SysGuide.lineAlpha = "";   //直线透明度 0全透~1
                        SysGuide.dashLength = "";  //虚线密度  4  8
                        SysGuide.color = "#CC0000";     //字体颜色
                        SysGuide.fontSize = "14";   //字体大小  默认14
                        SysGuide.position = "right";    //字体位置 right left
                        SysGuide.inside = "true";      //坐标系的内或外  false
                        SysGuide.fillAlpha = "0.1";
                        SysGuide.fillColor = CmMstBloodPressure.GetBPColor(SysGuide.label, "fill");   //GetFillColor
                        GuideList.Add(SysGuide);

                    }

                    //一般线
                    for (int i = 0; i <= Reference.Count - 1; i++)
                    {
                        //收缩压
                        Guide SysGuide = new Guide();
                        SysGuide.value = Reference[i].SBP.ToString();
                        SysGuide.toValue = "#CC0000";
                        SysGuide.label = Reference[i].SBP.ToString();
                        SysGuide.lineColor = "#CC0000";
                        SysGuide.lineAlpha = "0.15";
                        SysGuide.dashLength = "";
                        SysGuide.color = "#CC0000";
                        SysGuide.fontSize = "8";
                        SysGuide.position = "left";
                        SysGuide.inside = "";
                        SysGuide.fillAlpha = "";
                        SysGuide.fillColor = "";
                        GuideList.Add(SysGuide);

                    }

                    #endregion
                }
                else if ((Code == "Bloodpressure|Bloodpressure_2") && (Reference != null))
                {
                    #region 舒张压
                    GraphGuide.minimum = Convert.ToDouble(Reference[0].DBP);
                    GraphGuide.maximum = Convert.ToDouble(Reference[Reference.Count - 1].DBP);

                    InterSystems.Data.CacheTypes.CacheSysList DiaTarget = null;
                    DiaTarget = PsTarget.GetTargetByCode(pclsCache, PlanNo, "Bloodpressure", "Bloodpressure_2");

                    if (DiaTarget != null)
                    {
                        //初始值
                        Guide originalGuide = new Guide();
                        originalGuide.value = DiaTarget[4].ToString(); //值或起始值
                        originalGuide.toValue = "#CC0000";           //终值或""
                        originalGuide.label = "";      //中文定义 目标线、偏低、偏高等
                        //originalGuide.label = "起始" + "：" + originalGuide.value;
                        originalGuide.lineColor = "#FF5151";          //直线颜色  目标线  初始线
                        originalGuide.lineAlpha = "1";//直线透明度 0全透~1
                        originalGuide.dashLength = "8"; //虚线密度  4  8
                        originalGuide.color = "#CC0000";    //字体颜色
                        originalGuide.fontSize = "8"; //字体大小  默认14
                        originalGuide.position = "right"; //字体位置 right left
                        originalGuide.inside = "";//坐标系的内或外  false
                        originalGuide.fillAlpha = "";
                        originalGuide.fillColor = "";

                        GuideList.Add(originalGuide);
                        GraphGuide.original = originalGuide.value + "mmHg";

                        //目标值
                        Guide tagetGuide = new Guide();
                        tagetGuide.value = DiaTarget[3].ToString();
                        tagetGuide.toValue = "#CC0000";
                        tagetGuide.label = "";
                        //tagetGuide.label = "目标" + "：" + tagetGuide.value;
                        tagetGuide.lineColor = "#CC0000";
                        tagetGuide.lineAlpha = "1";
                        tagetGuide.dashLength = "4";
                        tagetGuide.color = "#CC0000";
                        tagetGuide.fontSize = "14";
                        tagetGuide.position = "right";
                        tagetGuide.inside = "";
                        tagetGuide.fillColor = "";
                        tagetGuide.fillAlpha = "";

                        GuideList.Add(tagetGuide);
                        GraphGuide.target = tagetGuide.value + "mmHg";
                    }

                    //风险范围
                    for (int i = 0; i <= Reference.Count - 2; i++)
                    {
                        //舒张压
                        Guide DiaGuide = new Guide();
                        DiaGuide.value = Reference[i].DBP.ToString();
                        DiaGuide.toValue = Reference[i + 1].DBP.ToString();
                        DiaGuide.label = Reference[i].Name;
                        DiaGuide.lineColor = "";
                        DiaGuide.lineAlpha = "";
                        DiaGuide.dashLength = "";
                        DiaGuide.color = "#CC0000";
                        DiaGuide.fontSize = "14";
                        DiaGuide.position = "right";
                        DiaGuide.inside = "true";
                        DiaGuide.fillAlpha = "0.1";
                        DiaGuide.fillColor = CmMstBloodPressure.GetBPColor(DiaGuide.label, "fill");

                        GuideList.Add(DiaGuide);

                    }

                    //一般线
                    for (int i = 0; i <= Reference.Count - 1; i++)
                    {
                        //舒张压
                        Guide DiaGuide = new Guide();
                        DiaGuide.value = Reference[i].DBP.ToString();
                        DiaGuide.toValue = "#CC0000";
                        DiaGuide.label = Reference[i].DBP.ToString();
                        DiaGuide.lineColor = "#CC0000";
                        DiaGuide.lineAlpha = "0.15";
                        DiaGuide.dashLength = "";
                        DiaGuide.color = "#CC0000";
                        DiaGuide.fontSize = "8";
                        DiaGuide.position = "left";
                        DiaGuide.inside = "";
                        DiaGuide.fillAlpha = "";
                        DiaGuide.fillColor = "";

                        GuideList.Add(DiaGuide);

                    }

                    #endregion
                }
                else if (Code == "Pulserate|Pulserate_1") //脉率没有 初始值和目标值
                {
                    #region 脉率
                    GraphGuide.minimum = 30;
                    GraphGuide.maximum = 150;

                    ////初始值
                    //Guide originalGuide = new Guide();
                    //originalGuide.value =""; //值或起始值
                    //originalGuide.toValue = "#CC0000";           //终值或""
                    //originalGuide.label = "";      //中文定义 目标线、偏低、偏高等
                    ////originalGuide.label = "起始" + "：" + originalGuide.value;
                    //originalGuide.lineColor = "#FF5151";          //直线颜色  目标线  初始线
                    //originalGuide.lineAlpha = "1";//直线透明度 0全透~1
                    //originalGuide.dashLength = "8"; //虚线密度  4  8
                    //originalGuide.color = "#CC0000";    //字体颜色
                    //originalGuide.fontSize = "8"; //字体大小  默认14
                    //originalGuide.position = "right"; //字体位置 right left
                    //originalGuide.inside = "";//坐标系的内或外  false
                    //originalGuide.fillAlpha = "";
                    //originalGuide.fillColor = "";

                    //GuideList.Add(originalGuide);
                    //GraphGuide.original = originalGuide.value;

                    ////目标值
                    //Guide tagetGuide = new Guide();
                    //tagetGuide.value = "";
                    //tagetGuide.toValue = "#CC0000";
                    //tagetGuide.label = "";
                    ////tagetGuide.label = "目标" + "：" + tagetGuide.value;
                    //tagetGuide.lineColor = "#CC0000";
                    //tagetGuide.lineAlpha = "1";
                    //tagetGuide.dashLength = "4";
                    //tagetGuide.color = "#CC0000";
                    //tagetGuide.fontSize = "14";
                    //tagetGuide.position = "right";
                    //tagetGuide.inside = "";
                    //tagetGuide.fillColor = "";
                    //tagetGuide.fillAlpha = "";


                    //风险范围
                    //正常
                    Guide PulseGuide = new Guide();
                    PulseGuide.value = "60";
                    PulseGuide.toValue = "100";
                    PulseGuide.label = "正常";
                    PulseGuide.lineColor = "";
                    PulseGuide.lineAlpha = "";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#00DB00";  //字的颜色
                    PulseGuide.fontSize = "14";
                    PulseGuide.position = "right";
                    PulseGuide.inside = "true";
                    PulseGuide.fillAlpha = "0.1";
                    PulseGuide.fillColor = "#2894FF";  //区域颜色
                    GuideList.Add(PulseGuide);

                    //偏高 #CC0000
                    PulseGuide = new Guide();
                    PulseGuide.value = "100";
                    PulseGuide.toValue = "150";
                    PulseGuide.label = "偏高";
                    PulseGuide.lineColor = "";
                    PulseGuide.lineAlpha = "";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#FF60AF";
                    PulseGuide.fontSize = "14";
                    PulseGuide.position = "right";
                    PulseGuide.inside = "true";
                    PulseGuide.fillAlpha = "0.1";
                    PulseGuide.fillColor = "#CC0000";
                    GuideList.Add(PulseGuide);

                    //偏低
                    PulseGuide = new Guide();
                    PulseGuide.value = "30";
                    PulseGuide.toValue = "60";
                    PulseGuide.label = "偏低";
                    PulseGuide.lineColor = "";
                    PulseGuide.lineAlpha = "";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#8080C0";
                    PulseGuide.fontSize = "14";
                    PulseGuide.position = "right";
                    PulseGuide.inside = "true";
                    PulseGuide.fillAlpha = "0.1";
                    PulseGuide.fillColor = "#FFC78E";
                    GuideList.Add(PulseGuide);


                    //一般线

                    //30
                    PulseGuide = new Guide();
                    PulseGuide.value = "30";
                    PulseGuide.toValue = "#CC0000";
                    PulseGuide.label = "30";
                    PulseGuide.lineColor = "#CC0000";
                    PulseGuide.lineAlpha = "0.15";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#CC0000";
                    PulseGuide.fontSize = "8";
                    PulseGuide.position = "left";
                    PulseGuide.inside = "";
                    PulseGuide.fillAlpha = "";
                    PulseGuide.fillColor = "";
                    GuideList.Add(PulseGuide);


                    //60
                    PulseGuide = new Guide();
                    PulseGuide.value = "60";
                    PulseGuide.toValue = "#CC0000";
                    PulseGuide.label = "60";
                    PulseGuide.lineColor = "#CC0000";
                    PulseGuide.lineAlpha = "0.15";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#CC0000";
                    PulseGuide.fontSize = "8";
                    PulseGuide.position = "left";
                    PulseGuide.inside = "";
                    PulseGuide.fillAlpha = "";
                    PulseGuide.fillColor = "";
                    GuideList.Add(PulseGuide);

                    //100
                    PulseGuide = new Guide();
                    PulseGuide.value = "100";
                    PulseGuide.toValue = "#CC0000";
                    PulseGuide.label = "100";
                    PulseGuide.lineColor = "#CC0000";
                    PulseGuide.lineAlpha = "0.15";
                    PulseGuide.dashLength = "";
                    PulseGuide.color = "#CC0000";
                    PulseGuide.fontSize = "8";
                    PulseGuide.position = "left";
                    PulseGuide.inside = "";
                    PulseGuide.fillAlpha = "";
                    PulseGuide.fillColor = "";
                    GuideList.Add(PulseGuide);

                    #endregion
                }

                GraphGuide.GuideList = GuideList;
                return GraphGuide;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "CmMstBloodPressure.GetGuidesByCode", "数据库操作异常！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            finally
            {
            }
        }

        #endregion 
    
    }
}