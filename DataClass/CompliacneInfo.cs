using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class CompliacneInfo
    {
    }

    public class SignDetailByP  //高血压体征详细-日期段
    {
        public int NextStartDate { get; set; } //需要查询起始的时间

        public List<SignDetailByD> SignDetailByDs { get; set; }

        public SignDetailByP()
        {
            SignDetailByDs = new List<SignDetailByD>();
        }

    }

    public class SignDetailByD  //高血压体征详细-某天
    {
        public string Date { get; set; }        //日期

        public string WeekDay { get; set; }        //星期几
        //public int Count { get; set; }           //某一日下有几个

        public List<SignDetail> SignDetailList { get; set; }

        public SignDetailByD()
        {
            SignDetailList = new List<SignDetail>();
        }

    }


    public class SignDetail   //高血压体征详细-到分 
    {
        public string DetailTime { get; set; }           //详细时间 到时分

        //血压 "135/78"
        //public string BPValue { get; set; }

        public string SBPValue { get; set; }        //收缩压

        public string DBPValue { get; set; }        //舒张压

        public string PulseValue { get; set; }        //脉率  "78"

        public SignDetail()
        {
            SBPValue = "";
            DBPValue = "";
            PulseValue = "";
        }
    }

    public class SignDetail1   //高血压体征详细-到分 
    {
        public string DetailTime { get; set; }           //详细时间 到时分

        //血压 "135/78"
        //public string BPValue { get; set; }

        public string SignValue { get; set; }        //值 处理后血压值等

        public string Unit { get; set; }        //脉率  "78"

        public SignDetail1()
        {
        }
    }




    //任务依从情况 ？
    public class CompliacneDetailByD
    {

        public string Date { get; set; }

        //public string ComplianceValue { get; set; }      

        public string drugBullet { get; set; }

        public string drugColor { get; set; }

        public string Events { get; set; }

    }


    //某天任务依从情况详细 用于弹框显示该天全部详情  目前按类别来分
    public class TaskComDetailByD
    {

        public string Date { get; set; }

        public string WeekDay { get; set; }

        public string ComplianceValue { get; set; }

        public List<VitalTaskCom> VitalTaskComList { get; set; } //体征测量

        public List<TaskComByType> TaskComByTypeList { get; set; }//生活方式和用药情况的共同集合类

        //public List<TaskCom> LifeTaskComList { get; set; }//生活方式

        //public List<TaskCom> DrugTaskComList { get; set; } //用药情况


        public TaskComDetailByD()
        {
            VitalTaskComList = new List<VitalTaskCom>();

            TaskComByTypeList = new List<TaskComByType>();

            //LifeTaskComList = new List<TaskCom>();

            //DrugTaskComList = new List<TaskCom>();


        }

    }


    //任务详细依从情况类  适用于体征测量和用药情况 
    public class TaskComByType
    {

        public string TaskType { get; set; }  //任务类型：体征测量 生活方式 用药情况   

        public List<TaskCom> TaskComList { get; set; }

        public TaskComByType()
        {
            TaskComList = new List<TaskCom>();
        }


    }

    //任务详细依从情况类
    public class TaskCom
    {

        public string TaskName { get; set; }  //体征名称 药物名称 生活方式名称   

        public string TaskStatus { get; set; }  //是否完成 对应勾叉

        //public string Description { get; set; }  //描述 用于显示生理参数的值

        //public string Description1 { get; set; }  //描述 用于显示生理参数的单位

    }

    //体征任务详细情况类
    public class VitalTaskCom
    {
        public string Status { get; set; }  //是否完成 对应勾叉

        public string Time { get; set; }  //  

        public string SignName { get; set; }  //  

        public string Value { get; set; }  //

        public string Unit { get; set; }  //


    }
}