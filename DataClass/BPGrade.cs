using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class BPGrade
    {
    }

    public class MstBloodPressure
    {
        public string Code { get; set; }                //编码  

        public string Name { get; set; }               //名称：很高、偏高、警戒、正常

        public string Description { get; set; }       //描述

        public int SBP { get; set; }              //收缩压

        public int DBP { get; set; }             //舒张压

        public string PatientClass { get; set; }       //患者类型

        public string Redundance { get; set; }        //冗余
    }

}