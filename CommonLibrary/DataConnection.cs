using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterSystems.Data.CacheClient;
using InterSystems.Data.CacheTypes;
using WebService.CommonLibrary;

namespace WebService.CommonLibrary
{
    public class DataConnection
    {
        #region<"DataConnection Field">
        public static int count = 0;    //Count for opening connection
        static int ConnectIDTrack = 0;
        public int ConnectID;
        public CacheConnection CacheConnectionObject = new CacheConnection();
        int serviceID = 0;
        #endregion

        //Constructor for the CacheConnectionPool intializing ZAM 2015-5-22
        public DataConnection()
        {
            this.ConnectID = ++ConnectIDTrack;
            ServiceConfiguration Config = new ServiceConfiguration();
            CacheConnectionObject = new CacheConnection();
            CacheConnectionObject.ConnectionString = "Server=" + Config.Server + "; Port=" + Config.Port + "; Namespace=" + Config.NameSpace + "; "
+ "Password=" + Config.Password + "; User ID=" + Config.UserID + "; Min Pool Size = 2;" + " Max Pool Size = 6;" + " Connection Reset = true;"
    + " Connection Lifetime = 2;";
        }

        //Constructor for the CacheConnectionPool intializing ZAM 2015-5-22
        public DataConnection(int serviceID)
        {
            this.ConnectID = ++ConnectIDTrack;
            ServiceConfiguration Config = new ServiceConfiguration();
            CacheConnectionObject = new CacheConnection();
            CacheConnectionObject.ConnectionString = "Server=" + Config.Server + "; Port=" + Config.Port + "; Namespace=" + Config.NameSpace + "; "
+ "Password=" + Config.Password + "; User ID=" + Config.UserID + "; Min Pool Size = 2;" + " Max Pool Size = 6;" + " Connection Reset = true;"
    + " Connection Lifetime = 2;";
            this.serviceID = serviceID;
        }

        //数据库连接 ZAM 2015-5-22
        public bool Connect()
        {
            bool ret = false;
            try
            {
                DataConnection.count++;
                CacheConnectionObject.Open();

                /*#region<"CachePool Monitor">
                // CachePool Monitor ZAM 2015-5-22
                int activeCount = CachePoolManager.ActiveConnectionCount;
                int idleCount = CachePoolManager.IdleCount(CacheConnectionObject);
                int inUseCount = CachePoolManager.InUseCount(CacheConnectionObject);

                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Service: " + this.serviceID + " DCount: " + DataConnection.count + " CID: " + ConnectID + " State: " + CacheConnectionObject.State.ToString() + " ID: " + CacheConnectionObject.CacheJobID, "");
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "Active " + activeCount + " idle: " + idleCount + " inUse:" + inUseCount + " ", "");

                #endregion*/

                ret = true;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "", "数据库连接失败！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
            return ret;
        }

        //数据库断开连接 ZAM 2015-5-22
        public bool DisConnect()
        {
            bool ret = false;
            try
            {
                DataConnection.count--;
                CacheConnectionObject.Close();
                ret = true;
            }
            catch (Exception ex)
            {
                HygeiaComUtility.WriteClientLog(HygeiaEnum.LogType.ErrorLog, "", "数据库断开连接失败！ error information : " + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
            return ret;
        }

    }
}