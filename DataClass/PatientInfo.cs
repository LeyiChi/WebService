using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class PatientInfo
    {
    }

    public class PatientBasicInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string InsuranceType { get; set; }
        public string Birthday { get; set; }
        public string GenderText { get; set; }
        public string BloodTypeText { get; set; }
        public string InsuranceTypeText { get; set; }
        public string Module { get; set; }
    }
}