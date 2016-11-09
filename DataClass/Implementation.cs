using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class Implementation
    {
    }

    public class ImplementationInfo   //Pad和Web版通用  区别：Web显示计划的任务列表，Pad不
    {
        public PatientInfo1 PatientInfo { get; set; }          //病人基本信息-姓名、头像

        public string ProgressRate { get; set; }              //此计划的进度

        public string RemainingDays { get; set; }            //此计划的剩余天数

        public string CompliacneValue { get; set; }         //此计划 最近一周的依从率 或 整个计划依从率

        public int StartDate { get; set; }                //计划的时间起始，作为体征切换的时间输入

        public int EndDate { get; set; }

        //是否有其他任务（除体征测量之外）的标志位？   
        //public string OtherTasks { get; set; }    // 1有 0没有

        public List<PlanDeatil> PlanList { get; set; }     //所有计划列表(正在实施中的 和 已经结束的)

        public List<Task> TaskList { get; set; }          //此计划的任务列表

        public ChartData ChartData { get; set; }        //画图数据-血压、脉率值，分级情况，依从情况

        public List<SignShow> SignList { get; set; }          //图上体征切换显示

        public ImplementationInfo()
        {
            PatientInfo = new PatientInfo1();          //初始化
            PlanList = new List<PlanDeatil>();
            TaskList = new List<Task>();
            ChartData = new ChartData();
            SignList = new List<SignShow>();
        }

    }

    public class ImplementationPhone           //Phone版
    {
        public string NowPlanNo { get; set; }       //正在执行的计划编号 ""则无

        public string ProgressRate { get; set; }       //进度

        public string RemainingDays { get; set; }       //剩余天数

        public string CompliacneValue { get; set; }       //最近一周的依从率

        public ChartData ChartData { get; set; }

        public int StartDate { get; set; }    //最近一周的时间起始，作为血压详细查看（时刻）的时间输入

        public int EndDate { get; set; }

        public List<SignShow> SignList { get; set; }          //图上体征切换显示

        public ImplementationPhone()
        {
            ChartData = new ChartData();
            SignList = new List<SignShow>();
        }

    }


    public class PatientInfo1   //病人基本信息
    {

        public string PatientName { get; set; } //姓名

        public string ImageUrl { get; set; }   //头像


    }


    public class ChartData //画图数据集合
    {

        public List<Graph> GraphList { get; set; }   //图：点

        public GraphGuide GraphGuide { get; set; }  //图：血。压分级区域和最大最小值 种类：收缩压、舒张压

        //是否有其他任务（除体征测量之外）的标志位？   
        public string OtherTasks { get; set; }    // 1有 0没有

        public ChartData()
        {
            GraphList = new List<Graph>();
            GraphGuide = new GraphGuide();
            OtherTasks = "0";
        }
    }

    public class Graph         //图的主要点数据
    {
        //日期
        public string Date { get; set; }       //日期，到天

        //图-测量任务，体征数据部分
        public string SignValue { get; set; }          //Y值

        public string SignGrade { get; set; }          //Y值级别  暂时只用来确定颜色，后期可作文字显示 “偏高、很高等”

        public string SignColor { get; set; }         //点颜色  

        public string SignShape { get; set; }         //点形状  

        public string SignDescription { get; set; }    //点的气球文本   样式——日期  <br> 收缩压/舒张压 mmHg  <br> 脉率 次/分



        //图-其他任务依从情况（包括用药、生活方式等）
        public string DrugValue { get; set; }         //画在下部图，保持Y=1

        public string DrugBullet { get; set; }       //客制化颜色 用图片-部分完成 "amcharts-images/drug.png" 半白半黑图片

        public string DrugColor { get; set; }        //药的其他颜色-完全未完成、完成	

        public string DrugDescription { get; set; }       //任务依从情况描述 "部分完成；未吃:阿司匹林、青霉素；已吃：钙片、板蓝根"  使用叉勾图标


        //暂时用不到的
        //public string BPBullet { get; set; }       //客制化血压点 "amcharts-images/star.png"

        //public string timeDetail { get; set; }    //最新测试的具体时间，到min

        public Graph()
        {

            //初始化  初始化为无记录状态，还是未完成任务状态？
            //暂时未完成任务状态  因为从PsCompliance取出那天，最初默认的就是未完成任务！
            //SignValue = "";  //string默认初始化为""，所以不需要再赋值
            //SignGrade = "";
            // SignColor = "";
            // SignShape = "";
            //SignDescription = "";

            DrugValue = "";            //可能没有用药或其他任务
            DrugBullet = "";         //初始化  时间肯定有  默认所有任务（生理测量、用药）为未完成任务状态
            DrugColor = "";  //白色 "#FFFFFF"
            DrugDescription = "";  //可能无任务，也可能任务未完成 不确定状态  "无记录";
        }


    }

    public class GraphGuide      //血压分级区域和最大最小值     
    {

        public List<Guide> GuideList { get; set; }   //血压分级区域

        public string original { get; set; }      //初始值

        public string target { get; set; }        //目标值

        public double minimum { get; set; }       //Y值下限

        public double maximum { get; set; }       //Y值上限

        public GraphGuide()
        {
            GuideList = new List<Guide>();
        }
    }

    public class Guide          //图的区域划分-风险分级、目标线、初始线    目标线、初始线 字体、线密度不同  分级区域颜色不同，文字不同
    {
        //变量-来自数据库
        public string value { get; set; }       //值或起始值

        public string toValue { get; set; }       //终值或""

        public string label { get; set; }        //中文定义 目标线、偏低、偏高等


        //恒量-根据图设定
        public string lineColor { get; set; }       //直线颜色  目标线  初始线
        public string lineAlpha { get; set; }       //直线透明度 0全透~1
        public string dashLength { get; set; }       //虚线密度  4  8

        public string color { get; set; }            //字体颜色
        public string fontSize { get; set; }       //字体大小  默认14
        public string position { get; set; }       //字体位置 right left
        public string inside { get; set; }        //坐标系的内或外  false

        public string fillAlpha { get; set; }       //区域透明度
        public string fillColor { get; set; }       //
        //public string balloonText { get; set; }       //气球弹出框   

    }

    public class TaskDeatil  //某任务的详细属性
    {
        public string TaskType { get; set; }

        public string TaskId { get; set; }

        public string TaskName { get; set; }

        public string Instruction { get; set; }

    }

    public class Task  //某类型任务的集合
    {
        public string TaskType { get; set; }

        public List<TaskDeatil> TaskDeatilList { get; set; }

        public Task()
        {
            TaskDeatilList = new List<TaskDeatil>();
        }
    }

    public class SignShow  //图上体征切换显示
    {
        public string SignName { get; set; }

        public string SignCode { get; set; }

    }

}