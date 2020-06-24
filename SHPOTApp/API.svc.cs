using System;
using SHPOT_BLL;
using SHPOT_ViewModel;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ServiceModel.Web;

namespace SHPOTApp
{
    public class API : IAPI
    {
        User_BLL _userBLL;
        public API()
        {
            _userBLL = new User_BLL();
        }

        #region [ Get All User Details ]
        /// <summary>
        /// Get All User Details
        /// </summary>
        /// <returns></returns>
        public Stream GetAllUsers()
        {
            List<UserLoginVM> _UserLoginVMs = _userBLL.GetAllUsers();

            if (_UserLoginVMs != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVMs);
                String _SuccessCode = "101";
                String _Message = "";
                return GenerateReturnJSONData(_UserLoginVMs[0].HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            else
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"No Record(s) Found.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
        }
        #endregion

        #region [ Get User Details By ID]
        /// <summary>
        /// Get User Details BY ID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Stream GetUserDetails(string userid)
        {
            UserLoginVM _UserLoginVM = _userBLL.GetUserDetails(Convert.ToInt32(userid));
            if (_UserLoginVM != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVM);
                String _SuccessCode = "102";
                String _Message = "";
                return GenerateReturnJSONData(_UserLoginVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            else
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"User does not exists.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
        }
        #endregion

        #region [ New User Registration ]
        /// <summary>
        /// New User Registration
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public Stream SignUp(UserLoginVM _userLogin)
        {
            UserLoginVM _UserVM = _userBLL.SignUp(_userLogin);
            if (_UserVM != null && _UserVM.UserName == "UserInvalid")
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"Username already exists.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
            else if (_UserVM != null && _UserVM.Email == "EmailInvalid")
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"Email Address already exists.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
            else
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserVM);
                String _SuccessCode = "103";
                String _Message = "\"message\":\"Successfully Registered.\",";
                return GenerateReturnJSONData(_UserVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
        }
        #endregion

        #region [ User Login]
        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public Stream SignIn(UserLoginVM _userLogin)
        {
            UserLoginVM _UserLoginVM = _userBLL.SignIn(_userLogin);
            if (_UserLoginVM == null)
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"Invalid UserName/Email Address.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
            else if (_UserLoginVM != null && _UserLoginVM.Password== "InvalidPassword")
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"Invalid Password.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
            else 
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVM);
                String _SuccessCode = "104";
                String _Message = "\"message\":\"Successfully Logged In.\",";
                return GenerateReturnJSONData(_UserLoginVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
        }
        #endregion

        #region [ User Test Login]
        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public Stream TestSign()
        {
            UserLoginVM _userLogin = new UserLoginVM { UserName="admin", Email = "admin@gmail.com", Password= "Test123@" };
            UserLoginVM _UserLoginVM = _userBLL.SignIn(_userLogin);
            if (_UserLoginVM != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVM);
                String _SuccessCode = "104";
                String _Message = "\"message\":\"Successfully Registered.\",";
                return GenerateReturnJSONData(_UserLoginVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            else
            {
                String _FailureCode = "";
                String _Message = "{\"message\":\"Incorrect Username or Password.\",";
                return FailureResponseRequest(_FailureCode, _Message);
            }
        }
        #endregion

        #region [ Generate Header and JSON Data for ResponseRequest ]
        /// <summary>
        /// Generate Header and JSON Data for ResponseRequest
        /// </summary>
        /// <param name="_UserVM"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static Stream GenerateReturnJSONData(string _headerToken, string _JSONData, string _SuccessCode, string _Message)
        {
            StringBuilder strJSONContent = User_BLL.GenerateReturnJSONData(_headerToken, _JSONData, _SuccessCode, _Message);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(strJSONContent.ToString()));
        }
        #endregion

        #region [ Failed Response for Request ]
        /// <summary>
        /// Failed Response for Request
        /// </summary>
        /// <returns></returns>
        public static Stream FailureResponseRequest(string _FailureCode, string _Message)
        {
            StringBuilder strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(strJSONContent.ToString()));
        }
        #endregion
    }
}
