using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class MstVitalSigns
    {
    }

    public class VitalSigns
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public string InputCode { get; set; }
        public string SortNo { get; set; }
        public string Redundance { get; set; }  //存单位
        public string InvalidFlag { get; set; }
    }
}