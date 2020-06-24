using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using SHPOT_BLL;
using SHPOT_ViewModel;
using System;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Braintree;

namespace WebAPIApp.Controllers
{
    public class BusinessController : ApiController
    {
        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        Business_BLL _businessBLL;
        private StringBuilder _strJSONContent;
        private String _Message;
        private Int32 _userId;

        public BusinessController()
        {
            _businessBLL = new Business_BLL();
            _strJSONContent = new StringBuilder();
            _Message = string.Empty;
        }
        #endregion

        #region [ Get All Business Details ]
        /// <summary>
        /// Get All Business Details
        /// </summary>
        /// <returns></returns>
        /// GET api/business/GetAllBusinesses
        [HttpGet]
        public HttpResponseMessage GetAllBusinesses()
        {
            if (IsTokenAuthenticated())
            {
                List<BusinessVM> _businessVMs = _businessBLL.GetAllBusinesses();
                if (_businessVMs != null)
                {
                    JSONSuccessResult(_businessVMs);
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Get Near By Business Details ]
        /// <summary>
        /// Get All Business Details
        /// </summary>
        /// <returns></returns>
        /// GET api/business/GetNearByBusinesses
        [HttpGet]
        public HttpResponseMessage GetNearByBusinesses(String latitude, String longitude, int radius)
        {
            if (IsTokenAuthenticated())
            {
                String googlePlaceURL = Common.GooglePlaceAPIURL + latitude + "," + longitude + "&keyword=shisha,hookah&radius=" + radius + "&key=" + Common.GooglePlaceAPIKey;

                StringBuilder strResponse = new StringBuilder();
                List<BusinessVM> _businessVMs = new List<BusinessVM>();

                using (var client = new WebClient())
                using (var stream = client.OpenRead(googlePlaceURL))
                using (var reader = new StreamReader(stream))
                using (var jsonData = new JsonTextReader(reader))
                {
                    var jObject = JObject.Load(jsonData);
                    foreach (var data in jObject["results"])
                    {
                        BusinessVM _businessVM = _businessBLL.BusinessPlaceDetails(data, _userId);
                        Double distance = _businessBLL.GetDistance(Convert.ToDouble(latitude), Convert.ToDouble(longitude), Convert.ToDouble(_businessVM.Latitude), Convert.ToDouble(_businessVM.Longitude));
                        _businessVM.Distance = distance.ToString();
                        if (_businessVM != null)
                            _businessVMs.Add(_businessVM);
                    }
                }

                List<BusinessVM> _businessNearByVMs = _businessBLL.GetAllNearByBusinesses(latitude, longitude, radius / 1000);
                if (_businessNearByVMs != null)
                    _businessVMs.AddRange(_businessNearByVMs);

                if (_businessVMs != null)
                {
                    JSONSuccessResult(_businessVMs.OrderBy(b => b.Distance).ToList());
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                }

                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Get NearBy Place By ID ]
        /// <summary>
        /// Get NearBy Place By ID
        /// </summary>
        /// <returns></returns>
        /// GET api/business/GetNearByPlaceByID
        [HttpGet]
        public HttpResponseMessage GetNearByPlaceByID(string id)
        {
            try
            {
                if (IsTokenAuthenticated())
                {
                    String googlePlaceURL = Common.GooglePlaceAPIReviewURL + id + "&key=" + Common.GooglePlaceAPIKey;

                    BusinessVM _businessVM = new BusinessVM();

                    using (var client = new WebClient())
                    using (var stream = client.OpenRead(googlePlaceURL))
                    using (var reader = new StreamReader(stream))
                    using (var jsonData = new JsonTextReader(reader))
                    {
                        _businessVM = _businessBLL.BusinessPlaceDetails(JObject.Load(jsonData)["result"], _userId);
                    }

                    if (_businessVM != null)
                    {
                        JSONSuccessResult(_businessVM);
                    }
                    else
                    {
                        _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
            }
            return Common.ResponseOutput(_strJSONContent);
        }
        #endregion        

        #region [ Get Business Details By ID ]
        /// <summary>
        /// Get Business Details BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// GET api/business/GetBusinessDetails/[id]
        [HttpGet]
        public HttpResponseMessage GetBusinessDetails(int id)
        {
            if (IsTokenAuthenticated())
            {
                BusinessVM _businessVM = _businessBLL.GetBusinessDetails(id);
                if (_businessVM != null)
                {
                    JSONSuccessResult(_businessVM);                    
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"Business does not exists.\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Add New Business ]
        /// <summary>
        /// Add New Business
        /// </summary>
        /// <returns></returns>
        /// POST api/business/AddNewBusiness
        [HttpPost]
        public async Task<HttpResponseMessage> AddNewBusiness()
        {
            if (IsTokenAuthenticated())
            {
                try
                {
                    string root = HttpContext.Current.Server.MapPath("~/App_Data");
                    //var provider = new MultipartFormDataStreamProvider(root);

                    var provider = await Request.Content.ReadAsMultipartAsync<InMemMultiFDSProvider>(new InMemMultiFDSProvider());
                    //access form data
                    BusinessVM _businessVM = new BusinessVM();
                    NameValueCollection formData = provider.FormData;

                    if (formData["BusinessName"] != null)
                        _businessVM.BusinessName = formData["BusinessName"];

                    if (formData["Latitude"] != null)
                        _businessVM.Latitude = formData["Latitude"];

                    if (formData["Longitude"] != null)
                        _businessVM.Longitude = formData["Longitude"];

                    if (formData["Location"] != null)
                        _businessVM.Location = formData["Location"];

                    if (formData["ContactNo"] != null)
                        _businessVM.ContactNo = formData["ContactNo"];

                    if (formData["Website"] != null)
                        _businessVM.Website = formData["Website"];

                    if (formData["StartTime"] != null)
                        _businessVM.StartTime = formData["StartTime"];

                    if (formData["EndTime"] != null)
                        _businessVM.EndTime = formData["EndTime"];

                    if (formData["Flavours"] != null)
                        _businessVM.Flavours = formData["Flavours"];

                    _businessVM.UserID = _userId;

                    _businessVM = _businessBLL.AddNewBusiness(_businessVM);

                    _businessVM.Reviews = "[]";

                    if (_businessVM != null)
                    {
                        if (provider.Files.Count > 0)
                        {
                            List<BusinessImageVM> lstBusinessImage = new List<BusinessImageVM>();

                            IList<HttpContent> files = provider.Files;

                            foreach (HttpContent fileHttpContent in provider.Files)
                            {
                                var thisFileName = fileHttpContent.Headers.ContentDisposition.FileName.Trim('\"');

                                string filename = String.Empty;
                                Stream input = await fileHttpContent.ReadAsStreamAsync();

                                string directoryName = String.Empty;
                                string URL = String.Empty;
                                string tempDocUrl = ConfigurationManager.AppSettings["APIURL"];
                                string fileExtension = thisFileName.Substring(thisFileName.LastIndexOf(".") + 1);

                                var path = HttpRuntime.AppDomainAppPath;
                                directoryName = Path.Combine(path, "Uploads\\" + UserTypes.Business + "\\" + _businessVM.BusinessID);
                                filename = Path.Combine(directoryName, thisFileName);

                                if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

                                if (File.Exists(filename)) { File.Delete(filename); }

                                using (Stream file = File.OpenWrite(filename))
                                {
                                    input.CopyTo(file);
                                    file.Close();
                                }

                                BusinessImageVM _businessImageVM = new BusinessImageVM { BusinessID = _businessVM.BusinessID, ImageName = thisFileName, ImageType = fileExtension, IsPremiumImage = false };
                                _businessImageVM = _businessBLL.AddBusinessImages(_businessImageVM);
                                _businessImageVM.ImagePath = tempDocUrl + "/Uploads/" + UserTypes.Business + "/" + _businessVM.BusinessID + "/" + thisFileName;

                                // Add To List
                                lstBusinessImage.Add(_businessImageVM);
                            }

                            _businessVM.Images = new JavaScriptSerializer().Serialize(lstBusinessImage);
                        }
                        else
                        {
                            _businessVM.Images = "[]";
                        }
                        JSONSuccessResult(_businessVM);
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
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion        

        #region [ Promote My Business ]
        /// <summary>
        /// Add New Business
        /// </summary>
        /// <returns></returns>
        /// POST api/business/PromoteMyBusiness
        [HttpPost]
        public async Task<HttpResponseMessage> PromoteMyBusiness()
        {
            if (IsTokenAuthenticated())
            {
                try
                {
                    var provider = await Request.Content.ReadAsMultipartAsync<InMemMultiFDSProvider>(new InMemMultiFDSProvider());
                    //access form data
                    BusinessVM _businessVM = new BusinessVM();
                    NameValueCollection formData = provider.FormData;

                    if (formData["BusinessID"] != null)
                        _businessVM.BusinessID = Convert.ToInt32(formData["BusinessID"]);

                    if (formData["PremiumImageContent"] != null)
                        _businessVM.PremiumImageContent = formData["PremiumImageContent"];

                    _businessVM.UserID = _userId;

                    _businessVM = _businessBLL.PromoteMyBusiness(_businessVM);

                    if (_businessVM != null)
                    {
                        if (provider.Files.Count > 0)
                        {
                            List<BusinessImageVM> lstBusinessImage = new List<BusinessImageVM>();

                            IList<HttpContent> files = provider.Files;
                            foreach (HttpContent fileHttpContent in files)
                            {
                                var thisFileName = fileHttpContent.Headers.ContentDisposition.FileName.Trim('\"');

                                string filename = String.Empty;
                                Stream input = await fileHttpContent.ReadAsStreamAsync();
                                string directoryName = String.Empty;
                                string URL = String.Empty;
                                string tempDocUrl = ConfigurationManager.AppSettings["APIURL"];
                                string fileExtension = thisFileName.Substring(thisFileName.LastIndexOf(".") + 1);

                                var path = HttpRuntime.AppDomainAppPath;
                                directoryName = Path.Combine(path, "Uploads\\" + UserTypes.Business + "\\" + _businessVM.BusinessID);
                                filename = Path.Combine(directoryName, thisFileName);

                                if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

                                if (File.Exists(filename)) { File.Delete(filename); }

                                using (Stream file = File.OpenWrite(filename))
                                {
                                    input.CopyTo(file);
                                    file.Close();
                                }

                                BusinessImageVM _businessImageVM = new BusinessImageVM { BusinessID = _businessVM.BusinessID, ImageName = thisFileName, ImageType = fileExtension, IsPremiumImage = true };
                                _businessImageVM = _businessBLL.AddBusinessImages(_businessImageVM);
                            }                            
                        }
                        _strJSONContent.Append("{\"status\":\"Success\"}");

                        ///* IOS push example */
                        //var path = HttpContext.Current.Server.MapPath("~/Certificates21Nov.p12");
                        ////IOSPush iosPush = new IOSPush(ApnsServerEnvironment.Production/*ApnsServerEnvironment.Sandbox in case of development*/, path, "Password of p12 file");
                        //IOSPush iosPush = new IOSPush(ApnsServerEnvironment.Production, path, "webastral99");

                        ////var json = "{\"ID\":\"121\",\"Name\":\"WebAstral\",\"notificationActions\":[{\"NotificationName\":\"Add to reminder\",\"NotificationActionEntityValues\":[{\"NotificationEntityName\":\"Title\",\"NotificationEntityValue\":\"Test\"},{\"NotificationEntityName\":\"DateTime\",\"NotificationEntityValue\":\"Pqr-28-2016 8:00:00\"}]},{\"NotificationName\":\"Add to calendar\",\"NotificationActionEntityValues\":[{\"NotificationEntityName\":\"Title\",\"NotificationEntityValue\":\"Title\"},{\"NotificationEntityName\":\"Location\",\"NotificationEntityValue\":\"Location\"},{\"NotificationEntityName\":\"All day\",\"NotificationEntityValue\":\"N\"},{\"NotificationEntityName\":\"Start DateTime\",\"NotificationEntityValue\":\"Oct-30-2016 8:00:00\"},{\"NotificationEntityName\":\"End DateTime\",\"NotificationEntityValue\":\"Oct-30-2016 17:00:00\"},{\"NotificationEntityName\":\"Alert\",\"NotificationEntityValue\":\"None\"},{\"NotificationEntityName\":\"Show As\",\"NotificationEntityValue\":\"None\"},{\"NotificationEntityName\":\"Note\",\"NotificationEntityValue\":\"Some notes here\"}]}]}";
                        //////Regular push notification: 4KB (4096 bytes) based on HTTP/2-based APNs provider API
                        ////iosPush.SendWithJSon("0eff51dbe4cfa72ae23b2a99f8a57baf1c13c065b0f2d3ebd49056b7ce881547", "Test push send by WebAstral", json, new Action<Result>(x =>
                        ////{
                        ////    Console.WriteLine(x.status);
                        ////}));

                        //iosPush.Send("0eff51dbe4cfa72ae23b2a99f8a57baf1c13c065b0f2d3ebd49056b7ce881547", "Test push send by WebAstral", "ID=121;Name=WebAstral");
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
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion        

        #region [ Get Promoted Business Details By ID ]
        /// <summary>
        /// Get Promoted Business Details By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// GET api/business/GetPromotedBusinessDetails/[id]
        [HttpGet]
        public HttpResponseMessage GetPromotedBusinessDetails(int id)
        {
            if (IsTokenAuthenticated())
            {
                BusinessVM _businessVM = _businessBLL.GetPromotedBusinessDetails(id);
                if (_businessVM != null)
                {
                    JSONSuccessResult(_businessVM);
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"Business does not exists.\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Get Favourite Places ]
        /// <summary>
        /// Get Favourite Places
        /// </summary>
        /// <returns></returns>
        /// GET api/business/GetFavouritePlaces
        [HttpGet]
        public HttpResponseMessage GetFavouritePlaces()
        {
            if (IsTokenAuthenticated())
            {
                List<FavouritePlaceVM> _favouritePlaceVMs = _businessBLL.GetFavouritePlaces(_userId);

                if (_favouritePlaceVMs != null)
                {
                    JSONSuccessResult(_favouritePlaceVMs);
                }
                else
                {
                    _strJSONContent.Append("{\"message\":\"No Business record(s) exists.\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }

        #endregion

        #region [ Add To Favourite ]
        /// <summary>
        /// Add To Favourite
        /// </summary>
        /// <param name="_favouritePlaceVM"></param>
        /// <returns></returns>
        /// POST api/business/AddToFavourite
        [HttpPost]
        public HttpResponseMessage AddToFavourite(FavouritePlaceVM _favouritePlaceVM)
        {
            if (IsTokenAuthenticated())
            {
                try
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
                }
                catch (Exception ex)
                {
                    _Message = ex.Message;
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ Add Business Ratings ]
        /// <summary>
        /// Add Business Ratings
        /// </summary>
        /// <param name="_businessRatingVM"></param>
        /// <returns></returns>
        /// POST api/business/AddBusinessRatings
        [HttpPost]
        public HttpResponseMessage AddBusinessRatings(BusinessRatingVM _businessRatingVM)
        {
            if (IsTokenAuthenticated())
            {
                try
                {
                    _businessRatingVM.UserID = _userId;
                    _businessRatingVM.Rating = _businessRatingVM.Rating.Replace("\"", "");
                    _businessRatingVM.Rating = _businessRatingVM.Rating != "" ? _businessRatingVM.Rating : "0";

                    BusinessVM _businessVM = _businessBLL.AddBusinessRating(_businessRatingVM);

                    if (_businessVM != null)
                    {
                        JSONSuccessResult(_businessVM);
                    }
                    else
                    {
                        _strJSONContent.Append("{\"message\":\"User does not exists.\"}");
                    }
                    return Common.ResponseOutput(_strJSONContent);
                }
                catch (Exception ex)
                {
                    _Message = ex.Message;
                    _strJSONContent.Append("{\"status\":\"Failed\"}");
                }
                return Common.ResponseOutput(_strJSONContent);
            }
            else
            {
                return Common.ResponseOutput(_strJSONContent);
            }
        }
        #endregion

        #region [ JSON Success Result Data To Return To API ]
        /// <summary>
        /// JSON SUCCESS RESULT DATA TO RETURN TO API
        /// </summary>
        /// <param name="_businessVMs"></param>
        private void JSONSuccessResult(Object _object)
        {
            String _JSONData = new JavaScriptSerializer().Serialize(_object);
            _JSONData = JSONContentReplaceEscapeSeq(_JSONData);

            String _Message = "\"status\":\"Success\",";
            _strJSONContent = Common.GenerateReturnJSONData(string.Empty, string.Empty, _JSONData, _Message, UserTypes.Business);
        }
        #endregion

        #region [ Check Token Authentication ]
        /// <summary>
        /// Check Token Authentication
        /// </summary>
        /// <returns></returns>
        private bool IsTokenAuthenticated()
        {
            return Common.IsTokenAuthenticated(Request.Headers, ref _userId, ref _strJSONContent);
        }
        #endregion

        #region [ JSON Content Replace Escape Sequence ]
        /// <summary>
        /// JSON Content Replace Escape Sequence
        /// </summary>
        /// <param name="_JSONData"></param>
        /// <returns></returns>
        private static string JSONContentReplaceEscapeSeq(string _JSONData)
        {
            _JSONData = _JSONData.Replace("\"Images\":\"", "\"Images\":");
            _JSONData = _JSONData.Replace("\"}]\"}", "\"}]}");

            _JSONData = _JSONData.Replace("\"}]\",", "\"}],");
            _JSONData = _JSONData.Replace(":\"[", ":[");
            _JSONData = _JSONData.Replace("]\"}", "]}");

            _JSONData = _JSONData.Replace("\"Reviews\":\"", "\"Reviews\":");
            _JSONData = _JSONData.Replace("}\"}", "}}");
            _JSONData = _JSONData.Replace(":[]\",", ":[],");

            _JSONData = _JSONData.Replace("}\",\"Images", "},\"Images");
            _JSONData = _JSONData.Replace("]\",\"Images", "],\"Images");

            return _JSONData;
        }
        #endregion
    }
}