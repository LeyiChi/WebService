using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataMethod
{
    public class MethodConst
    {
        #region <" Const ">
        //SetDataType
        public const int SetDataSuccess = 1;
        public const int SetDataFailed = 0;
        //DeleteDataType
        public const int DataNotExist = 2;
        public const int DeleteDataSuccess = 1;
        public const int DeleteDataFailed = 0;
        //ChangePasswordType
        public const int ChangePasswordSuccess = 1;
        public const int NewPasswordFail = 2;
        public const int OldPasswordInvalid = 3;
        public const int PasswordExpired = 4;
        //CheckPhoneNoType
        public const int CheckFail = 0;
        public const int CheckSuccess = 1;
        //InvalidFlagType
        public const int Valid = 0;
        public const int Invalid = 1;
        #endregion
    }
}