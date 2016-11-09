using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class ClinicInfo
    {
    }
    public class ClinicBasicInfoandTime
    {
        public string Time { get; set; }       //时间轴左侧日期：年月日  

        public List<SomeDayEvent> ItemGroup { get; set; }     //时间轴右侧事件集 

        public string Tag { get; set; }   //标签：入院、出院、住院中、转科、门诊、急诊    右侧事件右上标

        public string Color { get; set; }   //颜色：主要按照标签Tag赋颜色，但应避免同一天多个标签，导致颜色不定的情况-默认解决方案：取第一种

        public string VisitId { get; set; }

        public ClinicBasicInfoandTime()
        {
            ItemGroup = new List<SomeDayEvent>();
        }
    }

    //时间轴 同一天下：类型、事件
    public class SomeDayEvent
    {
        public string Time { get; set; }         //时间 某天下的 时分 截取自 精确时间
        public string Type { get; set; }         //类型
        public string Event { get; set; }        //事件
        public string KeyCode { get; set; }         //关键主键（用于查看详细）
    }

    public class Clinic
    {
        public string UserId { get; set; }

        public List<ClinicBasicInfoandTime> History { get; set; }

        public string AdmissionDateMark { get; set; }

        public string ClinicDateMark { get; set; }

        public string mark_contitue { get; set; }

        public Clinic()
        {
            History = new List<ClinicBasicInfoandTime>();
        }
    }

}

//namespace WebService.DataClass
//{
//    public class ClinicInfo
//    {
//    }
//    public class ClinicBasicInfoandTime
//    {
//        public string Time { get; set; }

//        public List<SomeDayEvent> ItemGroup { get; set; }     //时间轴右侧事件集 

//        public string Type { get; set; }   //排序序号(针对 入院、出院、门诊、其他）+类型
//        ////类型应该统一编码【就诊00（入院01In  出院02In  门诊03Out）  其他10（11Diagnosis  12Examination  13LabTest  14DrugRecord） 】

//        public string VisitId { get; set; }

//        public string Color { get; set; }   //作为class的名字

//        public int Number { get; set; }  //同一Vid下的编号  （编号的代价：增加两个字段Number和）  从1开始

//        public int Sort { get; set; } //唯一标识  从1开始 区分该段时间轴每一块 整体的排序

//        public ClinicBasicInfoandTime()
//        {
//            ItemGroup = new List<SomeDayEvent>();
//        }
//    }

//    //时间轴 同一天下：类型、事件
//    public class SomeDayEvent
//    {
//        public string Type { get; set; }         //类型
//        public string Event { get; set; }        //事件
//        public string KeyCode { get; set; }         //关键主键（用于查看详细）
//    }

//    public class Clinic
//    {
//        public string UserId { get; set; }

//        public List<ClinicBasicInfoandTime> History { get; set; }

//        public string AdmissionDateMark { get; set; }

//        public string ClinicDateMark { get; set; }

//        public int InId { get; set; }  //住院唯一标识  当前最大

//        public int OutId { get; set; } //门诊唯一标识  当前最大 

//        public Clinic()
//        {
//            History = new List<ClinicBasicInfoandTime>();
//        }
//    }

//}