using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class WnMstAlert
    {
        public string AlertItemName { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Units { get; set; }
        public string Remarks { get; set; }
    }
}