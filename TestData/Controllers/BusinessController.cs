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

namespace WebAPIApp.Controllers
{
    public class BusinessController : ApiController
    {
        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        Business_BLL _businessBLL;
        StringBuilder _strJSONContent;
        Boolean _success;
        int _userId = 0;

        public BusinessController()
        {
            _businessBLL = new Business_BLL();
            _strJSONContent = new StringBuilder();
            _success = true;
        }
        #endregion

        #region [ Get All Business Details ]
        /// <summary>
        /// Get All Business Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAllBusinesses()
        {
            if (!IsTokenAuthenticated())
            {
                return ResponseOutput();
            }
            else
            {
                List<BusinessVM> _businessVMs = _businessBLL.GetAllBusinesses();

                if (_businessVMs != null)
                {
                    String _JSONData = new JavaScriptSerializer().Serialize(_businessVMs);
                    _strJSONContent = Business_BLL.GenerateReturnJSONData(_JSONData, "");
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                }
                return ResponseOutput();
            }
        }

        #endregion

        #region [ Get Business Details By ID ]
        /// <summary>
        /// Get Business Details BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBusinessDetails(int id)
        {
            if (!IsTokenAuthenticated())
            {
                return ResponseOutput();
            }
            else
            {
                BusinessVM _businessVM = _businessBLL.GetBusinessDetails(id);

                if (_businessVM != null)
                {
                    String _JSONData = new JavaScriptSerializer().Serialize(_businessVM);
                    String _Message = "\"status\":\"Success\",";
                    _success = true;
                    _strJSONContent = Business_BLL.GenerateReturnJSONData(_JSONData, _Message);
                }
                else
                {
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                    _strJSONContent.Append("{\"message\":\"User does not exists.\"}");
                }
                return ResponseOutput();
            }
        }
        #endregion

        #region [ Add New Business ]
        /// <summary>
        /// Add New Business
        /// </summary>
        /// <param name="_businessVM"></param>
        /// <returns></returns>
        // POST api/values
        [HttpPost]
        public HttpResponseMessage AddNewBusiness(BusinessVM _businessVM)
        {
            if (!IsTokenAuthenticated())
            {
                return ResponseOutput();
            }
            else
            {
                _businessVM = _businessBLL.AddNewBusiness(_businessVM);

                if (_businessVM != null)
                {
                    String _JSONData = new JavaScriptSerializer().Serialize(_businessVM);
                    String _Message = "\"status\":\"Success\",";
                    _success = true;
                    _strJSONContent = Business_BLL.GenerateReturnJSONData(_JSONData, _Message);
                }
                else
                {
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
                return ResponseOutput();
            }
        }
        #endregion

        #region [ Get Favourite Places ]
        /// <summary>
        /// Get Favourite Places
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetFavouritePlaces()
        {
            if (!IsTokenAuthenticated())
            {
                return ResponseOutput();
            }
            else
            {
                List<FavouritePlaceVM> _favouritePlaceVMs = _businessBLL.GetFavouritePlaces(_userId);

                if (_favouritePlaceVMs != null)
                {
                    String _JSONData = new JavaScriptSerializer().Serialize(_favouritePlaceVMs);
                    _strJSONContent = Business_BLL.GenerateReturnJSONData(_JSONData, "");
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                }
                return ResponseOutput();
            }
        }

        #endregion

        #region [ Add To Favourite ]
        /// <summary>
        /// Add To Favourite
        /// </summary>
        /// <param name="place_id"></param>
        /// <param name="is_favourite"></param>
        /// <returns></returns>
        // POST api/values
        [HttpPost]
        public HttpResponseMessage AddToFavourite(FavouritePlaceVM _favouritePlaceVM)
        {
            if (!IsTokenAuthenticated())
            {
                return ResponseOutput();
            }
            else
            {
                _favouritePlaceVM.UserID = _userId;
                _favouritePlaceVM = _businessBLL.AddToFavourite(_favouritePlaceVM);

                if (_favouritePlaceVM != null)
                {
                    _strJSONContent.Append("{");
                    _strJSONContent.Append("\"status\":\"Success\",");
                    _strJSONContent.Append("\"isFavourite\":\"" + _favouritePlaceVM.IsFavourite + "\"");
                    _strJSONContent.Append("}");
                }
                else
                {
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
                return ResponseOutput();
            }
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

        #region [ Check if Token is Authenticated With User ID ]
        /// <summary>
        /// Check if Token is Authenticated With User ID
        /// </summary>
        /// <returns></returns>
        private bool IsTokenAuthenticated()
        {
            HttpRequestHeaders headers = this.Request.Headers;
            string _headerToken = string.Empty;
            if (headers.Contains("header_token"))
            {
                _headerToken = headers.GetValues("header_token").First();
            }
            if (headers.Contains("user_id"))
            {
                _userId = Convert.ToInt32(headers.GetValues("user_id").First());
            }

            User_BLL _objUserBll = new User_BLL();
            if (_headerToken == string.Empty || _userId == 0)
            {
                _strJSONContent.Append("{\"status\":\"UnAuthorized\"}");
                return false;
            }
            else if (!(_objUserBll.IsTokenAuthenticated(_headerToken, _userId)))
            {
                _strJSONContent.Append("{\"status\":\"UnAuthorized\"}");
                return false;
            }
            else
                return true;
        }
        #endregion
    }
}