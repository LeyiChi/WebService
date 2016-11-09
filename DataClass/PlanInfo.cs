using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class PlanInfo
    {
    }

    public class PlanDeatil
    {
        public string PlanName { get; set; }           //计划名称  “计划”+序号+起止时间  用于计划列表的显示

        public string PlanNo { get; set; }            //计划编码

        public int StartDate { get; set; }           //起始日期

        public int EndDate { get; set; }             //截止日期


        ///////////////////以下暂时未用到
        public string Module { get; set; }           //患者类型

        public string Status { get; set; }          //模块

        public string DoctorId { get; set; }        //社区专员医生ID
    }
}