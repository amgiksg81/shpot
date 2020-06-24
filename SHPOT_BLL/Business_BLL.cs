using System;
using System.Collections.Generic;
using System.Text;
using SHPOT_DAL;
using SHPOT_ViewModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SHPOT_BLL
{
    public class Business_BLL
    {
        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        Business_DAL _objBusinessDAL;
        BusinessVM _objBusinessVM;
        FavouritePlaceVM _objFavouritePlaceVM;
        Double _averageRating = 0;

        public Business_BLL()
        {
            _objBusinessDAL = new Business_DAL();
            _objBusinessVM = null;
            _objFavouritePlaceVM = null;
        }
        #endregion

        #region [ Get All Businesses ]
        /// <summary>
        /// Get All Businesses
        /// </summary>
        /// <returns></returns>
        public List<BusinessVM> GetAllBusinesses()
        {
            List<Business> _businesses = _objBusinessDAL.GetAllBusinesses();

            if (_businesses.Count > 0)
            {
                List<BusinessVM> _listBusinessVM = new List<BusinessVM>();
                foreach (Business _business in _businesses)
                {
                    BusinessVM _businessVM = MakeBusinessVM(_business, false);
                    _listBusinessVM.Add(_businessVM);
                }
                return _listBusinessVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Get All Near By Businesses ]
        /// <summary>
        /// Get All Near By Businesses
        /// </summary>
        /// <returns></returns>
        public List<BusinessVM> GetAllNearByBusinesses(String latitude, String longitude, int _radius)
        {
            List<Business> _businesses = _objBusinessDAL.GetAllNearByBusinesses(latitude, longitude, _radius);

            if (_businesses.Count > 0)
            {
                List<BusinessVM> _listBusinessVM = new List<BusinessVM>();
                foreach (Business _business in _businesses)
                {
                    BusinessVM _businessVM = MakeBusinessVM(_business, false);
                    Double distance = GetDistance(Convert.ToDouble(latitude), Convert.ToDouble(longitude), Convert.ToDouble(_businessVM.Latitude), Convert.ToDouble(_businessVM.Longitude));
                    _businessVM.Distance = distance.ToString();
                    _listBusinessVM.Add(_businessVM);
                }
                return _listBusinessVM;
            }
            else
                return null;
        }
        #endregion        

        #region [ Get Business Details By ID ]
        /// <summary>
        /// Get Business Details BY ID
        /// </summary>
        /// <param name="_businessID"></param>
        /// <returns></returns>
        public BusinessVM GetBusinessDetails(int _businessID)
        {
            Business _business = _objBusinessDAL.GetBusinessDetails(_businessID);
            if (_business != null)
            {
                return MakeBusinessVM(_business, false);
            }
            else
                return null;
        }
        #endregion

        #region [ Get Promoted Business Details By ID ]
        /// <summary>
        /// Get Promoted Business Details By ID
        /// </summary>
        /// <param name="_businessID"></param>
        /// <returns></returns>
        public BusinessVM GetPromotedBusinessDetails(int _businessID)
        {
            Business _business = _objBusinessDAL.GetBusinessDetails(_businessID);
            if (_business != null)
            {
                return MakeBusinessVM(_business, true);
            }
            else
                return null;
        }
        #endregion

        #region [ Add New Business ]
        /// <summary>
        /// Add New Business
        /// </summary>
        /// <param name="_businessVM"></param>
        /// <returns></returns>
        public BusinessVM AddNewBusiness(BusinessVM _businessVM)
        {
            Business _business = new Business
            {
                BusinessName = _businessVM.BusinessName,
                Latitude = _businessVM.Latitude,
                Longitude = _businessVM.Longitude,
                Location = _businessVM.Location,
                ContactNo = _businessVM.ContactNo,
                Website = _businessVM.Website,
                StartTime = _businessVM.StartTime,
                EndTime = _businessVM.EndTime,
                Flavours = _businessVM.Flavours,
                PremiumImageContent = _businessVM.PremiumImageContent,
                UserID = _businessVM.UserID,
                CreatedDate = DateTime.Now
            };

            _business = _objBusinessDAL.AddNewBusiness(_business);
            if (_business != null)
            {
                _businessVM.BusinessID = _business.BusinessID;
                return _businessVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Promote My Business ]
        /// <summary>
        /// Promote My Business
        /// </summary>
        /// <param name="_businessVM"></param>
        /// <returns></returns>
        public BusinessVM PromoteMyBusiness(BusinessVM _businessVM)
        {
            Business _business = new Business
            {
                BusinessID = _businessVM.BusinessID,
                UserID = _businessVM.UserID,
                PremiumImageContent = _businessVM.PremiumImageContent
            };

            _business = _objBusinessDAL.PromoteMyBusiness(_business);
            if (_business != null)
                return _businessVM;
            else
                return null;
        }
        #endregion

        #region [ Add Business Images ]
        /// <summary>
        /// Add Business Images
        /// </summary>
        /// <returns></returns>
        public BusinessImageVM AddBusinessImages(BusinessImageVM _businessImageVM)
        {
            BusinessImage _businessImage = new BusinessImage { BusinessID = _businessImageVM.BusinessID, ImageName = _businessImageVM.ImageName, ImageType = _businessImageVM.ImageType, IsPremiumImage = _businessImageVM.IsPremiumImage };
            _businessImage = _objBusinessDAL.AddBusinessImages(_businessImage);
            _businessImageVM.BusinessImageID = _businessImage.BusinessImageID;
            return _businessImageVM;
        }
        #endregion

        #region [ Add Business Rating ]
        /// <summary>
        /// Add Business Rating
        /// </summary>
        /// <returns></returns>
        public BusinessVM AddBusinessRating(BusinessRatingVM _businessRatingVM)
        {
            BusinessRating _businessRating = new BusinessRating
            {
                UserID = _businessRatingVM.UserID,
                Rating = Convert.ToDouble(_businessRatingVM.Rating),
                Review = _businessRatingVM.Review
            };

            if (string.IsNullOrEmpty(_businessRatingVM.PlaceID))
            {
                _businessRating.BusinessID = _businessRatingVM.BusinessID;
                _businessRating.PlaceID = null;
            }
            else
            {
                _businessRating.BusinessID = null;
                _businessRating.PlaceID = _businessRatingVM.PlaceID;
            }

            _objBusinessDAL.AddBusinessRating(_businessRating);

            if (!string.IsNullOrEmpty(_businessRatingVM.PlaceID))
            {
                String googlePlaceURL = ConfigurationManager.AppSettings["GooglePlaceAPIReviewURL"] + _businessRatingVM.PlaceID + "&key=" + ConfigurationManager.AppSettings["GooglePlaceAPIKey"];

                using (var client = new WebClient())
                using (var stream = client.OpenRead(googlePlaceURL))
                using (var reader = new StreamReader(stream))
                using (var jsonData = new JsonTextReader(reader))
                {
                    return BusinessPlaceDetails(JObject.Load(jsonData)["result"], _businessRatingVM.UserID);
                }
            }
            else
                return GetBusinessDetails(_businessRatingVM.BusinessID);
        }
        #endregion

        #region [ Get Favourite Places ]
        /// <summary>
        /// Get Favourite Places
        /// </summary>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public List<FavouritePlaceVM> GetFavouritePlaces(Int32 _userID)
        {
            List<FavouritePlace> _favouritePlaces = _objBusinessDAL.GetFavouritePlaces(_userID);

            if (_favouritePlaces.Count > 0)
            {
                List<FavouritePlaceVM> _listFavouritePlaceVM = new List<FavouritePlaceVM>();
                foreach (FavouritePlace _favouritePlace in _favouritePlaces)
                {
                    FavouritePlaceVM _favouritePlaceVM = null;

                    if (_favouritePlace.BusinessID != null)
                        _favouritePlaceVM = MakeFavouritePlaceVM(_favouritePlace);
                    else
                    {
                        String googlePlaceURL = ConfigurationManager.AppSettings["GooglePlaceAPIReviewURL"] + _favouritePlace.PlaceID + "&key=" + ConfigurationManager.AppSettings["GooglePlaceAPIKey"];

                        using (var client = new WebClient())
                        using (var stream = client.OpenRead(googlePlaceURL))
                        using (var reader = new StreamReader(stream))
                        using (var jsonData = new JsonTextReader(reader))
                        {
                            _favouritePlaceVM = FavouritePlaceDetailsFromAPI(JObject.Load(jsonData)["result"], _userID);
                        }
                    }
                    _listFavouritePlaceVM.Add(_favouritePlaceVM);
                }
                return _listFavouritePlaceVM.OrderBy(b => b.Latitude).ThenBy(b => b.Longitude).ToList();
            }
            else
                return null;
        }
        #endregion

        #region [ Get Favourite Place Business Place Details from Google API ]
        /// <summary>
        /// Business Place Details from Google API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private FavouritePlaceVM FavouritePlaceDetailsFromAPI(JToken data, Int32 _userID)
        {
            FavouritePlaceVM _favouritePlaceVM = new FavouritePlaceVM();
            BusinessVM _businessVM = BusinessPlaceDetails(data, _userID);

            _favouritePlaceVM.Latitude = _businessVM.Latitude;
            _favouritePlaceVM.Longitude = _businessVM.Longitude;
            _favouritePlaceVM.BusinessName = _businessVM.BusinessName;
            _favouritePlaceVM.Icon = _businessVM.Icon;
            _favouritePlaceVM.StartTime = _businessVM.StartTime;
            _favouritePlaceVM.Images = _businessVM.Images;
            _favouritePlaceVM.PlaceID = _businessVM.PlaceID;
            _favouritePlaceVM.Reviews = _businessVM.Reviews;
            _favouritePlaceVM.ContactNo = _businessVM.ContactNo;
            _favouritePlaceVM.AverageRating = _businessVM.AverageRating;
            _favouritePlaceVM.Location = _businessVM.Location;
            _favouritePlaceVM.IsFavourite = Convert.ToBoolean(_businessVM.IsFavouritePlace);
            _favouritePlaceVM.IsFavouritePlace = _businessVM.IsFavouritePlace;

            return _favouritePlaceVM;
        }
        #endregion

        #region [ Business Place Details from Google API ]
        /// <summary>
        /// Business Place Details from Google API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BusinessVM BusinessPlaceDetails(JToken data, Int32 _userID)
        {
            BusinessVM _businessVM = new BusinessVM();

            if (data["geometry"]["location"]["lat"] != null)
                _businessVM.Latitude = data["geometry"]["location"]["lat"].ToString();

            if (data["geometry"]["location"]["lng"] != null)
                _businessVM.Longitude = data["geometry"]["location"]["lng"].ToString();

            if (data["name"] != null)
                _businessVM.BusinessName = data["name"].ToString().Replace("\"", "'");

            if (data["icon"] != null)
                _businessVM.Icon = data["icon"].ToString().Replace("\"", "'");

            if (data["photos"] != null)
            {
                List<BusinessImageVM> _businessImageVMs = new List<BusinessImageVM>();
                foreach (var images in data["photos"])
                {
                    _businessImageVMs.Add(new BusinessImageVM { ImagePath = images["photo_reference"].ToString() });
                }
                _businessVM.Images = new JavaScriptSerializer().Serialize(_businessImageVMs);
            }
            else
                _businessVM.Images = "[]";

            if (data["place_id"] != null)
            {
                _businessVM.PlaceID = data["place_id"].ToString();
                String _contactNumber = "";
                String _startTime = "";
                String _endTime = "";
                _businessVM.Reviews = GetReviewsByPlaceID(data["place_id"].ToString(), ref _contactNumber, ref _startTime, ref _endTime);
                _businessVM.ContactNo = _contactNumber;
                _businessVM.StartTime = _startTime;
                _businessVM.EndTime = _endTime;
            }

            //if (data["rating"] != null)
            //    _businessVM.AverageRating = Convert.ToDouble(data["rating"].ToString());
            _businessVM.AverageRating = Math.Round(_averageRating, 2);

            if (data["vicinity"] != null)
                _businessVM.Location = data["vicinity"].ToString().Replace("\"", "'");

            _businessVM.IsFavouritePlace = _objBusinessDAL.IsPlaceFavouriteByPlaceID(_businessVM.PlaceID, _userID);
            return _businessVM;
        }
        #endregion

        #region [ Get Reviews By Place By ID ]
        /// <summary>
        /// Get Reviews By Place By ID
        /// </summary>
        /// <param name="_placeID"></param>
        public String GetReviewsByPlaceID(String _placeID, ref String _contactNumber, ref String _startTime, ref String _endTime)
        {
            try
            {
                _averageRating = 0;
                String googlePlaceURL = ConfigurationManager.AppSettings["GooglePlaceAPIReviewURL"] + _placeID + "&fields=review,opening_hours,formatted_phone_number&key=" + ConfigurationManager.AppSettings["GooglePlaceAPIKey"];

                StringBuilder strResponse = new StringBuilder();
                List<BusinessRatingVM> _businessRatingVMs = new List<BusinessRatingVM>();
                Int32 _ratingRecordsCount = 0;

                using (var client = new WebClient())
                using (var stream = client.OpenRead(googlePlaceURL))
                using (var reader = new StreamReader(stream))
                using (var jsonData = new JsonTextReader(reader))
                {
                    var jObject = JObject.Load(jsonData)["result"];

                    if (jObject["formatted_phone_number"] != null)
                        _contactNumber = jObject["formatted_phone_number"].ToString().Replace("\"", "'");

                    if (jObject["opening_hours"] != null)
                    {
                        try
                        {
                            _startTime = jObject["opening_hours"]["periods"][((int)DateTime.Now.DayOfWeek + 6) % 7]["open"]["time"].ToString().Replace("\"", "'");
                            _startTime = DateTime.Parse(_startTime.Insert(2, ":")).ToString("h:mm tt");
                        }
                        catch (Exception ex) { _startTime = ""; }
                        try
                        {
                            _endTime = jObject["opening_hours"]["periods"][((int)DateTime.Now.DayOfWeek + 6) % 7]["close"]["time"].ToString().Replace("\"", "'");
                            _endTime = DateTime.Parse(_endTime.Insert(2, ":")).ToString("h:mm tt");
                        }
                        catch (Exception ex) { _endTime = ""; }
                    }

                    if (jObject["reviews"] != null)
                    {
                        foreach (var data in jObject["reviews"])
                        {
                            BusinessRatingVM _businessRatingVM = new BusinessRatingVM();

                            if (data["author_name"] != null)
                                _businessRatingVM.UserName = data["author_name"].ToString().Replace("\"", "'");

                            if (data["profile_photo_url"] != null)
                                _businessRatingVM.ProfileImageUrl = data["profile_photo_url"].ToString().Replace("\"", "'");

                            if (data["rating"] != null)
                                _businessRatingVM.Rating = data["rating"].ToString();

                            if (data["text"] != null)
                                _businessRatingVM.Review = data["text"].ToString().Replace("\"", "'");

                            _businessRatingVMs.Add(_businessRatingVM);
                            _averageRating = _averageRating + Double.Parse(_businessRatingVM.Rating);
                            _ratingRecordsCount++;
                        }
                    }
                }

                // Add Business Rating
                List<BusinessRating> lstBusinessRating = _objBusinessDAL.GetReviewsByPlaceID(_placeID);

                if (lstBusinessRating != null)
                {

                    List<BusinessRatingVM> lstBusinessRatingVM = new List<BusinessRatingVM>();
                    foreach (var rating in lstBusinessRating)
                    {
                        BusinessRatingVM _businessRatingVM = new BusinessRatingVM
                        {
                            BusinessRatingID = rating.BusinessRatingID,
                            Rating = rating.Rating.ToString(),
                            Review = rating.Review,
                            UserID = rating.UserID,
                            UserName = rating.User.UserName,
                            ProfileImageUrl = rating.User.ProfileImageUrl
                        };
                        lstBusinessRatingVM.Add(_businessRatingVM);
                        _averageRating = _averageRating + rating.Rating;
                        _ratingRecordsCount++;
                    }

                    if (_ratingRecordsCount != 0)
                        _averageRating = _averageRating / _ratingRecordsCount;

                    if (lstBusinessRatingVM.Count > 0)
                        _businessRatingVMs.AddRange(lstBusinessRatingVM);
                }
                return new JavaScriptSerializer().Serialize(_businessRatingVMs);
            }
            catch (Exception ex)
            {
                return "[]";
            }
        }
        #endregion

        #region [ Add To Favourite ]
        /// <summary>
        /// AddToFavourite
        /// </summary>
        /// <param name="_businessVM"></param>
        /// <returns></returns>
        public FavouritePlaceVM AddToFavourite(FavouritePlaceVM _favouritePlaceVM)
        {
            FavouritePlace _favouritePlace = new FavouritePlace { UserID = _favouritePlaceVM.UserID };

            if (string.IsNullOrEmpty(_favouritePlaceVM.PlaceID))
            {
                _favouritePlace.BusinessID = _favouritePlaceVM.BusinessID;
                _favouritePlace.PlaceID = null;
            }
            else
            {
                _favouritePlace.BusinessID = null;
                _favouritePlace.PlaceID = _favouritePlaceVM.PlaceID;
            }

            _favouritePlace = _objBusinessDAL.AddToFavourite(_favouritePlace, _favouritePlaceVM.IsFavourite);
            if (_favouritePlace != null)
            {
                _favouritePlaceVM.FavouritePlaceID = _favouritePlace.FavouritePlaceID;
                return _favouritePlaceVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Add Support Query ]
        /// <summary>
        /// Add Support Query
        /// </summary>
        /// <param name="_supportQueryVM"></param>
        /// <returns></returns>
        public SupportQueryVM AddSupportQuery(SupportQueryVM _supportQueryVM)
        {
            SupportQuery _supportQuery = new SupportQuery { FullName = _supportQueryVM.FullName, EmailAddress = _supportQueryVM.EmailAddress, Query = _supportQueryVM.Query, IPAddress = _supportQueryVM.IPAddress };
            _supportQuery = _objBusinessDAL.AddSupportQuery(_supportQuery);
            if (_supportQuery != null)
            {
                _supportQueryVM.SupportQueryID = _supportQuery.SupportQueryID;
                _supportQueryVM.QueryDateTime = _supportQuery.QueryDateTime;
                return _supportQueryVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Convert Business to Business View Model Object ]
        /// <summary>
        /// Convert Business to Business View Model Object 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private BusinessVM MakeBusinessVM(Business item, Boolean _isPremiumBusiness)
        {
            _objBusinessVM = new BusinessVM()
            {
                BusinessID = item.BusinessID,
                BusinessName = item.BusinessName,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Location = item.Location,
                ContactNo = item.ContactNo,
                Website = item.Website,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Flavours = item.Flavours,                
                UserID = item.UserID,
                IsFavouritePlace = item.FavouritePlaces.Where(f => (f.BusinessID == item.BusinessID && f.UserID == item.UserID)).Count() > 0 ? 1 : 0
            };

            if (_isPremiumBusiness)
                _objBusinessVM.PremiumImageContent = item.PremiumImageContent;


            // Add Business Rating
            Double _averageRating = 0;
            if (item.BusinessRatings.Count > 0)
            {
                List<BusinessRatingVM> lstBusinessRating = new List<BusinessRatingVM>();
                foreach (var rating in item.BusinessRatings)
                {
                    BusinessRatingVM _businessRatingVM = new BusinessRatingVM
                    {
                        BusinessRatingID = rating.BusinessRatingID,
                        BusinessID = (int)rating.BusinessID,
                        Rating = rating.Rating.ToString(),
                        Review = rating.Review,
                        UserID = rating.UserID,
                        UserName = rating.User.UserName,
                        ProfileImageUrl = rating.User.ProfileImageUrl
                    };
                    lstBusinessRating.Add(_businessRatingVM);
                    _averageRating = _averageRating + rating.Rating;
                }

                _objBusinessVM.Reviews = new JavaScriptSerializer().Serialize(lstBusinessRating);
                _averageRating = _averageRating / item.BusinessRatings.Count;
            }
            else
                _objBusinessVM.Reviews = "[]";

            _objBusinessVM.AverageRating = _averageRating;

            // Add Business Images
            if (item.BusinessImages.Where(d => d.IsPremiumImage == _isPremiumBusiness).Count() > 0)
            {
                List<BusinessImageVM> lstBusinessImage = new List<BusinessImageVM>();
                string tempDocUrl = ConfigurationManager.AppSettings["APIURL"];
                foreach (var image in item.BusinessImages.Where(d => d.IsPremiumImage == _isPremiumBusiness))
                {
                    BusinessImageVM _businessImageVM = new BusinessImageVM
                    {
                        BusinessImageID = image.BusinessImageID,
                        BusinessID = image.BusinessID,
                        ImageName = image.ImageName,
                        ImageType = image.ImageType,
                        ImagePath = tempDocUrl + "/Uploads/Business/" + image.BusinessID + "/" + image.ImageName
                    };
                    lstBusinessImage.Add(_businessImageVM);
                }
                _objBusinessVM.Images = new JavaScriptSerializer().Serialize(lstBusinessImage);
            }
            else
                _objBusinessVM.Images = "[]";

            return _objBusinessVM;
        }
        #endregion

        #region [ Convert FavouritePlace to FavouritePlace View Model Object ]
        /// <summary>
        /// Convert FavouritePlace to FavouritePlace View Model Object 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private FavouritePlaceVM MakeFavouritePlaceVM(FavouritePlace item)
        {
            _objFavouritePlaceVM = new FavouritePlaceVM()
            {
                FavouritePlaceID = item.FavouritePlaceID,
                UserID = item.Business.UserID,
                BusinessID = (int)item.BusinessID,
                BusinessName = item.Business.BusinessName,
                Latitude = item.Business.Latitude,
                Longitude = item.Business.Longitude,
                Location = item.Business.Location,
                ContactNo = item.Business.ContactNo,
                Website = item.Business.Website,
                StartTime = item.Business.StartTime,
                EndTime = item.Business.EndTime,
                Flavours = item.Business.Flavours,
                IsFavouritePlace = 1
            };

            // Add Business Rating
            Double _averageRating = 0;
            if (item.Business.BusinessRatings.Count > 0)
            {
                List<BusinessRatingVM> lstBusinessRating = new List<BusinessRatingVM>();
                foreach (var rating in item.Business.BusinessRatings)
                {
                    BusinessRatingVM _businessRatingVM = new BusinessRatingVM
                    {
                        BusinessRatingID = rating.BusinessRatingID,
                        BusinessID = (int)rating.BusinessID,
                        Rating = rating.Rating.ToString(),
                        Review = rating.Review,
                        UserID = rating.UserID,
                        UserName = rating.User.UserName,
                        ProfileImageUrl = rating.User.ProfileImageUrl
                    };
                    lstBusinessRating.Add(_businessRatingVM);
                    _averageRating = _averageRating + rating.Rating;
                }

                _objFavouritePlaceVM.Reviews = new JavaScriptSerializer().Serialize(lstBusinessRating);
                _averageRating = _averageRating / item.Business.BusinessRatings.Count;
            }
            else
                _objFavouritePlaceVM.Reviews = "[]";

            _objFavouritePlaceVM.AverageRating = _averageRating;

            // Add Business Images
            if (item.Business.BusinessImages.Where(d => d.IsPremiumImage == false).Count() > 0)
            {
                List<BusinessImageVM> lstBusinessImage = new List<BusinessImageVM>();
                string tempDocUrl = ConfigurationManager.AppSettings["APIURL"];
                foreach (var image in item.Business.BusinessImages.Where(d => d.IsPremiumImage == false))
                {
                    BusinessImageVM _businessImageVM = new BusinessImageVM
                    {
                        BusinessImageID = image.BusinessImageID,
                        BusinessID = image.BusinessID,
                        ImageName = image.ImageName,
                        ImageType = image.ImageType,
                        ImagePath = tempDocUrl + "/Uploads/Business/" + image.BusinessID + "/" + image.ImageName
                    };
                    lstBusinessImage.Add(_businessImageVM);
                }
                _objFavouritePlaceVM.Images = new JavaScriptSerializer().Serialize(lstBusinessImage);
            }
            else
                _objFavouritePlaceVM.Images = "[]";

            return _objFavouritePlaceVM;
        }
        #endregion

        #region [ Calculate Distance Between Google Locations by Latitude and Longitude ]
        /// <summary>
        /// Calculate Distance Between Google Locations by Latitude and Longitude
        /// </summary>
        /// <param name="Latitude1"></param>
        /// <param name="Longitude1"></param>
        /// <param name="Latitude2"></param>
        /// <param name="Longitude2"></param>
        /// <returns></returns>
        public double GetDistance(double Latitude1, double Longitude1, double Latitude2, double Longitude2)
        {
            double R = 6371.0;          // R is earth radius.
            double dLat = this.toRadian(Latitude2 - Latitude1);
            double dLon = this.toRadian(Longitude2 - Longitude1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(this.toRadian(Latitude1)) * Math.Cos(this.toRadian(Latitude2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;

            return d;
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
        #endregion
    }
}
