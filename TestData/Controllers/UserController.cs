using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using SHPOT_BLL;
using SHPOT_ViewModel;
using System;
using System.Web.Script.Serialization;
using System.Text;

namespace WebAPIApp.Controllers
{
    public class UserController: ApiController
    {
        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        User_BLL _userBLL;
        StringBuilder _strJSONContent;
        Boolean _success;

        public UserController()
        {
            _userBLL = new User_BLL();
            _strJSONContent = new StringBuilder();
            _success = true;
        }
        #endregion

        #region [ Get All User Details ]
        /// <summary>
        /// Get All User Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAllUsers()
        {
            List<UserLoginVM> _UserLoginVMs = _userBLL.GetAllUsers();

            if (_UserLoginVMs != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVMs);
                String _SuccessCode = "101";
                String _Message = "";
                _strJSONContent = User_BLL.GenerateReturnJSONData(_UserLoginVMs[0].HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            else
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"No Record(s) Found.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            return ResponseOutput();
        }
        
        #endregion

        #region [ Get User Details By ID ]
        /// <summary>
        /// Get User Details BY ID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetUserDetails(int id)
        {
            UserLoginVM _UserLoginVM = _userBLL.GetUserDetails(id);
            
            if (_UserLoginVM != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVM);
                String _SuccessCode = "102";
                String _Message = "";
                _strJSONContent = User_BLL.GenerateReturnJSONData(_UserLoginVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            else
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"User does not exists.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            return ResponseOutput();
        }
        #endregion

        #region [ New User Registration ]
        /// <summary>
        /// New User Registration
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        // POST api/values
        [HttpPost]
        public HttpResponseMessage SignUp(UserLoginVM _userLogin)
        {
            UserLoginVM _UserVM = _userBLL.SignUp(_userLogin);
            if (_UserVM != null && _UserVM.UserName == "UserInvalid")
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"Username already exists.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            else if (_UserVM != null && _UserVM.Email == "EmailInvalid")
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"Email Address already exists.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            else
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserVM);
                String _SuccessCode = "103";
                String _Message = "\"message\":\"Successfully Registered.\",";
                _strJSONContent = User_BLL.GenerateReturnJSONData(_UserVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            return ResponseOutput();
        }
        #endregion

        #region [ User Sign In ]
        /// <summary>
        /// User Sign In
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        // POST api/values
        [HttpPost]
        public HttpResponseMessage SignIn(UserLoginVM _userLogin)
        {
            UserLoginVM _UserLoginVM = _userBLL.SignIn(_userLogin);
            if (_UserLoginVM == null)
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"Invalid UserName/Email Address.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            else if (_UserLoginVM != null && _UserLoginVM.Password == "InvalidPassword")
            {
                String _FailureCode = "";
                String _Message = "\"message\":\"Invalid Password.\",";
                _success = false;
                _strJSONContent = User_BLL.FailureResponseRequest(_FailureCode, _Message);
            }
            else
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserLoginVM);
                String _SuccessCode = "104";
                String _Message = "\"message\":\"Successfully Logged In.\",";
                _strJSONContent = User_BLL.GenerateReturnJSONData(_UserLoginVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            return ResponseOutput();
        }
        #endregion

        #region [ Social Sign In ]
        /// <summary>
        /// Social Sign In
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        // POST api/values/socialsignin
        [HttpPost]
        public HttpResponseMessage SocialSignIn(UserLoginVM _userLogin)
        {
            _userLogin.IsSocialUser = true;

            UserLoginVM _UserVM = _userBLL.SignUp(_userLogin);
            if (_UserVM != null)
            {
                String _JSONData = new JavaScriptSerializer().Serialize(_UserVM);
                String _SuccessCode = "103";
                String _Message = "\"message\":\"Successfully Registered.\",";
                _strJSONContent = User_BLL.GenerateReturnJSONData(_UserVM.HeaderToken, _JSONData, _SuccessCode, _Message);
            }
            return ResponseOutput();
        }
        #endregion

        #region [ API's Final Response Output ]
        /// <summary>
        /// API's Final Response Output
        /// </summary>
        /// <returns></returns>
        private HttpResponseMessage ResponseOutput()
        {
            var resp = new HttpResponseMessage() { Content = new StringContent(_strJSONContent.ToString()) };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            resp.Content.Headers.Add("Success", _success.ToString());
            return resp;
        }
        #endregion

        #region [ --- SAMPLE METHODS ---]
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/showvalues
        [HttpGet]
        public IEnumerable<string> ShowValues()
        {
            return new string[] { "value3", "value4" };
        }

        // GET api/values/showvalues
        [HttpGet]
        public IEnumerable<string> DoNotDo()
        {
            return new string[] { "value5", "value6" };
        }

        // GET api/values/5
        [HttpGet]
        public HttpResponseMessage SampleGet(int id)
        {
            var resp = new HttpResponseMessage() { Content = new StringContent("[{\"Name\":\"ABC\"},[{\"A\":\"1\"},{\"B\":\"2\"},{\"C\":\"3\"}]]") };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            resp.Content.Headers.Add("Success", "true");

            //Another Format for JSON Return
            //var resp = new HttpResponseMessage { Content = new StringContent("[{\"Name\":\"ABC\"},[{\"A\":\"1\"},{\"B\":\"2\"},{\"C\":\"3\"}]]", System.Text.Encoding.UTF8, "application/json") };
            return resp;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/values/5
        public void Delete(int id)
        {

        }
        #endregion        
    }
}