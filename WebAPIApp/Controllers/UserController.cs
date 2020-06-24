using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using SHPOT_BLL;
using SHPOT_ViewModel;
using System;
using System.Web.Script.Serialization;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Configuration;
using System.Web;
using System.Threading.Tasks;
using Braintree;

namespace WebAPIApp.Controllers
{
    public class UserController : ApiController
    {
        public IBraintreeConfiguration config = new BraintreeConfiguration();

        public static readonly 
            TransactionStatus[] transactionSuccessStatuses = {
                                                                TransactionStatus.AUTHORIZED,
                                                                TransactionStatus.AUTHORIZING,
                                                                TransactionStatus.SETTLED,
                                                                TransactionStatus.SETTLING,
                                                                TransactionStatus.SETTLEMENT_CONFIRMED,
                                                                TransactionStatus.SETTLEMENT_PENDING,
                                                                TransactionStatus.SUBMITTED_FOR_SETTLEMENT
                                                            };

        #region [ Braintree Register Client ]
        /// <summary>
        /// Braintree Register Client
        /// </summary>
        /// <returns></returns>
        /// POST api/user/BraintreeRegisterClient
        [HttpPost]
        public HttpResponseMessage BraintreeRegisterClient()
        {
            if (Common.IsTokenAuthenticated(Request.Headers, ref _userId, ref _strJSONContent))
            {
                var gateway = config.GetGateway();
                var clientToken = gateway.ClientToken.Generate();

                UserLoginVM _UserLoginVM = _userBLL.GetUserDetails(_userId);


                var request = new CustomerRequest
                {
                    FirstName = _UserLoginVM.FirstName,
                    LastName = _UserLoginVM.LastName,
                    Company = "SHPOT",
                    Email = _UserLoginVM.Email,
                    Phone = _UserLoginVM.HeaderToken,
                    Website = ConfigurationManager.AppSettings["APIURL"]
                };

                Result<Customer> result = gateway.Customer.Create(request);

                bool success = result.IsSuccess();
                if (success)
                {
                    string customerId = result.Target.Id;

                    _strJSONContent.Append("{\"ClientToken\":\"" + clientToken + "\",");
                    _strJSONContent.Append("\"CustomerId\":\"" + customerId + "\"}");

                    _UserLoginVM.BrainTreeClientToken = clientToken;
                    _UserLoginVM.BrainTreeCustomerID = customerId;

                    _UserLoginVM = _userBLL.UpdateBrainTreeDetails(_UserLoginVM);
                }
                else
                    _strJSONContent.Append("{\"status\":\"Failed\"}");

                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Braintree Make Payment ]
        /// <summary>
        /// Braintree Make Payment
        /// </summary>
        /// <returns></returns>
        /// POST api/user/BraintreeMakePayment
        [HttpPost]
        public HttpResponseMessage BraintreeMakePayment(UserPremiumVM _userPremiumVM)
        {
            if (Common.IsTokenAuthenticated(Request.Headers, ref _userId, ref _strJSONContent))
            {
                var gateway = config.GetGateway();
                Decimal amount;

                try
                {
                    amount = Convert.ToDecimal(_userPremiumVM.Amount);

                    var nonce = _userPremiumVM.PaymentNonce;
                    var request = new TransactionRequest
                    {
                        Amount = amount,
                        CustomerId = _userPremiumVM.CustomerID,                        
                        PaymentMethodNonce = nonce,
                        Options = new TransactionOptionsRequest
                        {
                            SubmitForSettlement = true
                        }
                    };

                    Result<Transaction> result = gateway.Transaction.Sale(request);
                    if (result.IsSuccess())
                    {
                        Transaction transaction = result.Target;
                        _strJSONContent.Append("{\"status\":\"Success\",");
                        _strJSONContent.Append("\"TransactionID\":\"" + transaction.Id + "\"}");

                        _userPremiumVM.TransactionID = transaction.Id;
                        _userPremiumVM.PaymentMode = "Monthly";

                        _userPremiumVM = _userBLL.UpdateBrainTreePayment(_userPremiumVM);
                    }
                    else
                    {
                        string errorMessages = "";
                        foreach (ValidationError error in result.Errors.DeepAll())
                        {
                            errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                        }
                        _strJSONContent.Append("{\"status\":\"Failed\",");
                        _strJSONContent.Append("\"Error\":\"" + errorMessages + "\"}");
                    }
                }
                catch (FormatException e)
                {
                    _strJSONContent.Append("{\"status\":\"Amount is an invalid format.\"}");
                    _strJSONContent.Append("{\"status\":\"Amount is an invalid format.\"}");
                }                
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        User_BLL _userBLL;
        private StringBuilder _strJSONContent;
        private String _Message;
        private Int32 _userId;

        public UserController()
        {
            _userBLL = new User_BLL();
            _strJSONContent = new StringBuilder();
            _Message = string.Empty;
        }
        #endregion

        #region [ Get All User Details ]
        /// <summary>
        /// Get All User Details
        /// </summary>
        /// <returns></returns>
        /// GET api/user/GetAllUsers
        [HttpGet]
        public HttpResponseMessage GetAllUsers()
        {
            List<UserLoginVM> _UserLoginVMs = _userBLL.GetAllUsers();

            if (_UserLoginVMs != null)
            {
                String _SuccessCode = "101";
                JSONSuccessResult(_UserLoginVMs, _SuccessCode, _UserLoginVMs[0].HeaderToken);
            }
            else
            {
                String _FailureCode = String.Empty;
                _Message = "\"message\":\"No Record(s) Found.\",";
                _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
            }
            return Common.ResponseOutput(_strJSONContent);
        }

        #endregion

        #region [ Get User Details By ID ]
        /// <summary>
        /// Get User Details BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// GET api/user/GetUserDetails/[id]
        [HttpGet]
        public HttpResponseMessage GetUserDetails(int id)
        {
            UserLoginVM _UserLoginVM = _userBLL.GetUserDetails(id);

            if (_UserLoginVM != null)
            {
                String _SuccessCode = "102";
                JSONSuccessResult(_UserLoginVM, _SuccessCode, _UserLoginVM.HeaderToken);
            }
            else
            {
                String _FailureCode = String.Empty;
                _Message = "\"message\":\"User does not exists.\",";
                _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ New User Registration ]
        /// <summary>
        /// New User Registration
        /// </summary>
        /// <param name="_userLogin"></param>
        /// <returns></returns>
        /// POST api/user/signup
        [HttpPost]
        public HttpResponseMessage SignUp(UserLoginVM _userLogin)
        {
            try
            {
                _userLogin.Password = Common.EncryptedPassword(_userLogin.Password);
                UserLoginVM _UserLoginVM = _userBLL.SignUp(_userLogin);
                if (_UserLoginVM != null && _UserLoginVM.UserName == "UserInvalid")
                {
                    String _FailureCode = String.Empty;
                    _Message = "\"message\":\"Username already exists.\",";
                    _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
                }
                else if (_UserLoginVM != null && _UserLoginVM.Email == "EmailInvalid")
                {
                    String _FailureCode = String.Empty;
                    _Message = "\"message\":\"Email Address already exists.\",";
                    _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
                }
                else
                {
                    String _SuccessCode = "103";
                    _Message = "\"message\":\"Successfully Registered.\",";
                    JSONSuccessResult(_UserLoginVM, _SuccessCode, _UserLoginVM.HeaderToken);
                }
            }
            catch (Exception ex)
            {
                _Message = ex.Message;
                _strJSONContent.Append("{\"status\":\"Failed\"}");
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ User Sign In ]
        /// <summary>
        /// User Sign In
        /// </summary>
        /// <param name="_userLogin"></param>
        /// <returns></returns>
        /// POST api/user/SignIn
        [HttpPost]
        public HttpResponseMessage SignIn(UserLoginVM _userLogin)
        {
            try
            {
                _userLogin.Password = Common.EncryptedPassword(_userLogin.Password);
                UserLoginVM _UserLoginVM = _userBLL.SignIn(_userLogin);
                if (_UserLoginVM == null)
                {
                    String _FailureCode = String.Empty;
                    _Message = "\"message\":\"Invalid UserName/Email Address.\",";
                    _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
                }
                else if (_UserLoginVM != null && _UserLoginVM.Password == "InvalidPassword")
                {
                    String _FailureCode = String.Empty;
                    _Message = "\"message\":\"Invalid Password.\",";
                    _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
                }
                else
                {
                    String _SuccessCode = "104";
                    _Message = "\"message\":\"Successfully Logged In.\",";
                    JSONSuccessResult(_UserLoginVM, _SuccessCode, _UserLoginVM.HeaderToken);
                }
            }
            catch (Exception ex)
            {
                _Message = ex.Message;
                _strJSONContent.Append("{\"status\":\"Failed\"}");
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ Social Sign In ]
        /// <summary>
        /// Social Sign In
        /// </summary>
        /// <param name="_userLogin"></param>
        /// <returns></returns>
        /// POST api/user/socialsignin
        [HttpPost]
        public HttpResponseMessage SocialSignIn(UserLoginVM _userLogin)
        {
            try
            {
                _userLogin.IsSocialUser = true;
                UserLoginVM _UserLoginVM = _userBLL.SignUp(_userLogin);
                if (_UserLoginVM != null)
                {
                    String _SuccessCode = "103";
                    _Message = "\"message\":\"Successfully Registered.\",";
                    JSONSuccessResult(_UserLoginVM, _SuccessCode, _UserLoginVM.HeaderToken);
                }
            }
            catch (Exception ex)
            {
                _Message = ex.Message;
                _strJSONContent.Append("{\"status\":\"Failed\"}");
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ Edit / Update User Details ]
        /// <summary>
        /// Edit / Update User Details
        /// </summary>
        /// <param name="_userLogin"></param>
        /// <returns></returns>
        /// POST api/user/EditUserProfile
        [HttpPost]
        public async Task<HttpResponseMessage> EditUserProfile()
        {
            var provider = await Request.Content.ReadAsMultipartAsync<InMemMultiFDSProvider>(new InMemMultiFDSProvider());
            //access form data
            UserLoginVM _UserLoginVM = new UserLoginVM();
            NameValueCollection formData = provider.FormData;

            if (formData["FirstName"] != null)
                _UserLoginVM.FirstName = formData["FirstName"].Trim();

            if (formData["LastName"] != null)
                _UserLoginVM.LastName = formData["LastName"].Trim();

            if (!string.IsNullOrWhiteSpace(formData["Password"]))
            {
                _UserLoginVM.Password = Common.EncryptedPassword(formData["Password"].Trim());
            }
            if (!string.IsNullOrWhiteSpace(formData["NewPassword"]))
            {
                _UserLoginVM.NewPassword = Common.EncryptedPassword(formData["NewPassword"].Trim());
            }
            if (!string.IsNullOrWhiteSpace(_UserLoginVM.Password) && string.IsNullOrWhiteSpace(_UserLoginVM.NewPassword))
            {
                String _FailureCode = String.Empty;
                _Message = "\"message\":\"Invalid New Password\",";
                _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
            }
            else if (Common.IsTokenAuthenticated(Request.Headers, ref _userId, ref _strJSONContent))
            {
                try
                {
                    string filename = String.Empty;
                    string directoryName = String.Empty;
                    Stream input = null;
                    _UserLoginVM.UserID = _userId;

                    if (provider.Files.Count > 0)
                    {
                        List<BusinessImageVM> lstBusinessImage = new List<BusinessImageVM>();
                        HttpContent file1 = provider.Files[0];

                        var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"');
                        input = await file1.ReadAsStreamAsync();
                        string URL = String.Empty;
                        string tempDocUrl = ConfigurationManager.AppSettings["APIURL"];
                        string fileExtension = thisFileName.Substring(thisFileName.LastIndexOf(".") + 1);

                        var path = HttpRuntime.AppDomainAppPath;
                        directoryName = Path.Combine(path, "Uploads\\" + UserTypes.User + "\\" + _UserLoginVM.UserID);
                        filename = Path.Combine(directoryName, thisFileName);
                        _UserLoginVM.ProfileImageUrl = tempDocUrl + "/Uploads/" + UserTypes.User + "/" + _UserLoginVM.UserID + "/" + thisFileName;
                    }
                    else
                        _UserLoginVM.ProfileImageUrl = String.Empty;

                    _UserLoginVM = _userBLL.EditUserProfile(_UserLoginVM);

                    if (_UserLoginVM != null)
                    {
                        if (!string.IsNullOrWhiteSpace(_UserLoginVM.ProfileImageUrl))
                        {
                            if (!Directory.Exists(directoryName)) { Directory.CreateDirectory(directoryName); }
                            else
                            {
                                string[] filePaths = Directory.GetFiles(directoryName);
                                foreach (string filePath in filePaths)
                                    File.Delete(filePath);
                            }

                            if (File.Exists(filename)) { File.Delete(filename); }
                            using (Stream file = File.OpenWrite(filename))
                            {
                                input.CopyTo(file);
                                file.Close();
                            }
                        }
                        String _SuccessCode = String.Empty;
                        _Message = "\"message\":\"Successfully Updated\",";
                        JSONSuccessResult(_UserLoginVM, _SuccessCode, _UserLoginVM.HeaderToken);
                    }
                    else
                    {
                        String _FailureCode = String.Empty;
                        _Message = "\"message\":\"Invalid Password\",";
                        _strJSONContent = Common.FailureResponseRequest(_FailureCode, _Message);
                    }
                }
                catch (Exception ex)
                {
                    _Message = ex.Message;
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ User Sign Out ]
        /// <summary>
        /// User Sign Out
        /// </summary>
        /// <returns></returns>
        /// POST api/user/signout
        [HttpPost]
        public HttpResponseMessage SignOut()
        {
            HttpRequestHeaders headers = this.Request.Headers;
            UserVM _userVM = new UserVM();

            try
            {
                if (headers.Contains("header_token"))
                {
                    _userVM.HeaderToken = headers.GetValues("header_token").First();
                }
                if (headers.Contains("user_id"))
                {
                    _userVM.UserID = Convert.ToInt32(headers.GetValues("user_id").First().Replace("\"", ""));
                }

                if (_userVM.HeaderToken != string.Empty || _userVM.UserID != 0)
                {
                    if (_userBLL.SignOut(_userVM))
                        _strJSONContent.Append("{\"status\":\"Success\"}");
                    else
                        _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
                else
                {
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }

            }
            catch (Exception ex)
            {
                _Message = ex.Message;
                _strJSONContent.Append("{\"status\":\"Failed\"}");
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion

        #region [ JSON Success Result Data To Return To API ]
        /// <summary>
        /// JSON SUCCESS RESULT DATA TO RETURN TO API
        /// </summary>
        /// <param name="_businessVMs"></param>
        private void JSONSuccessResult(Object _object, string _SuccessCode, string _HeaderToken)
        {
            String _JSONData = new JavaScriptSerializer().Serialize(_object);
            _strJSONContent = Common.GenerateReturnJSONData(_HeaderToken, _SuccessCode, _JSONData, _Message, UserTypes.User);
        }
        #endregion

        #region [ --- SAMPLE METHODS ---]
        // GET api/user
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/user/showvalues
        [HttpGet]
        public IEnumerable<string> ShowValues()
        {
            return new string[] { "value3", "value4" };
        }

        // GET api/user/showvalues
        [HttpGet]
        public IEnumerable<string> DoNotDo()
        {
            return new string[] { "value5", "value6" };
        }

        // GET api/user/5
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

        // PUT api/user/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/user/5
        public void Delete(int id)
        {

        }
        #endregion                
    }
}