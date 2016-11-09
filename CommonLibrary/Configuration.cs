using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.CommonLibrary
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            this.Server = System.Configuration.ConfigurationManager.AppSettings["Server"];
            this.Port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            this.NameSpace = System.Configuration.ConfigurationManager.AppSettings["NameSpace"];
            this.UserID = System.Configuration.ConfigurationManager.AppSettings["UserID"];
            this.Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
        }

        public string Server { get; private set; }
        public string Port { get; private set; }
        public string NameSpace { get; private set; }
        public string UserID { get; private set; }
        public string Password { get; private set; }
    }
}