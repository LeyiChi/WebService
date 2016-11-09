using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.CommonLibrary
{
    public class HygeiaConst
    {
        #region "< Constructor >"

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HygeiaConst()
        {
        }

        #endregion

        #region "< Configuration >"

        //< Common >

        public const string CONFIG_FILENAME = "HygeiaConfig.xml";

        public const string CONFIG_ROOT = "HygeiaConfig";
        //< Caché >

        public const string CONFIG_CACHE_NODE = "Cache";

        public const string CONFIG_CACHE_KEY_SERVER = "Server";

        public const string CONFIG_CACHE_KEY_PORT = "Port";

        public const string CONFIG_CACHE_KEY_NAMESPACE = "NameSpace";

        public const string CONFIG_CACHE_KEY_USERID = "UserID";

        public const string CONFIG_CACHE_KEY_PASSWORD = "Password";
        //< Culture Information >

        public const string CONFIG_CULTURE_NODE = "Culture";

        public const string CONFIG_CULTURE_NAME = "Name";
        #endregion

        #region "< System Log >"


        public const string CLIENT_LOG_DIR = "..\\Log";
        //20141202 ZAM
        //public const string CLIENT_LOG_BASENAME = "HYGEIA"; 
        public const string CLIENT_LOG_BASENAME = "WebService"; 


        public const int CLIENT_LOG_RETRY = 1;

        public const int CLIENT_LOG_SAVEDAYS = 30;
        //Public Const CLIENT_LOG_SEPARATOR As String = ","

        public const string CLIENT_LOG_SEPARATOR = "\t";    //Convert.ToString('\t')
        #endregion
    }
}