using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace SHPOT_DAL
{
    public class Business_DAL
    {
        #region [ Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        private DBContext _dbContext = null;

        public Business_DAL()
        {
            _dbContext = new DBContext();
        }
        #endregion

        #region [ Get All Businesses ]
        /// <summary>
        /// Get Business Details By BusinessID
        /// </summary>
        /// <returns></returns>
        public List<Business> GetAllBusinesses()
        {
            return _dbContext.Businesses.AsNoTracking().ToList();
        }
        #endregion

        #region [ Get All Near By Businesses]
        /// <summary>
        /// Get All Near By Businesses
        /// </summary>
        /// <returns></returns>
        public List<Business> GetAllNearByBusinesses(String latitude, String longitude, int _radius)
        {

            var latitudeParam = new SqlParameter("@Latitude", latitude);
            var longitudeParam = new SqlParameter("@Longitude", longitude);
            var radiusParam = new SqlParameter("@Radius", _radius);

            List<Business> empDetails = _dbContext.Businesses.SqlQuery("exec FindPlacesNearLatLong @Latitude, @Longitude, @Radius", latitudeParam, longitudeParam, radiusParam).ToList();

            return empDetails;
        }
        #endregion

        #region [ Get Business Details By BusinessId ]
        /// <summary>
        /// Get Business Details By BusinessId
        /// </summary>
        /// <param name="_businessID"></param>
        /// <returns></returns>
        public Business GetBusinessDetails(int _businessID)
        {
            return _dbContext.Businesses.AsNoTracking().Where(d => d.BusinessID == _businessID).FirstOrDefault();
        }
        #endregion

        #region [ Add New Business ]
        /// <summary>
        /// Add New Business
        /// </summary>
        /// <returns></returns>
        public Business AddNewBusiness(Business _business)
        {
            _dbContext.Businesses.Add(_business);
            _dbContext.SaveChanges();
            return _business;
        }
        #endregion

        #region [ PromoteMyBusiness ]
        /// <summary>
        /// Promote My Business
        /// </summary>
        /// <returns></returns>
        public Business PromoteMyBusiness(Business _business)
        {
            Business _businessExists = _dbContext.Businesses.Where(b => b.BusinessID == _business.BusinessID && b.UserID == _business.UserID).FirstOrDefault();

            if (_businessExists != null)
            {
                _businessExists.PremiumImageContent = _business.PremiumImageContent;
                _dbContext.Entry(_businessExists).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _businessExists;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region [ Add Business Images ]
        /// <summary>
        /// Add Business Images
        /// </summary>
        /// <returns></returns>
        public BusinessImage AddBusinessImages(BusinessImage _businessImage)
        {
            _dbContext.BusinessImages.Add(_businessImage);
            _dbContext.SaveChanges();
            return _businessImage;
        }
        #endregion

        #region [ Add Business Ratings ]
        /// <summary>
        /// Add Business Ratings
        /// </summary>
        /// <returns></returns>
        public BusinessRating AddBusinessRating(BusinessRating _businessRating)
        {
            if (_businessRating.BusinessID != null && _businessRating.BusinessID != 0)
            {
                Business _business = GetBusinessDetails((int)_businessRating.BusinessID);
                if (_business == null)
                    return null;
            }

            BusinessRating _objBusinessRating = _dbContext.BusinessRatings.Where(b => ((b.BusinessID == _businessRating.BusinessID && b.PlaceID == _businessRating.PlaceID) && b.UserID == _businessRating.UserID)).FirstOrDefault();

            // Check If Already in Business Rating
            if (_objBusinessRating != null)
            {
                _objBusinessRating.Rating = _businessRating.Rating;
                _objBusinessRating.Review = _businessRating.Review;

                if (!string.IsNullOrEmpty(_businessRating.PlaceID))
                    _objBusinessRating.BusinessID = null;

                _dbContext.Entry(_objBusinessRating).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
                return _objBusinessRating;
            }
            else
            {
                if (!string.IsNullOrEmpty(_businessRating.PlaceID))
                    _businessRating.BusinessID = null;

                _dbContext.BusinessRatings.Add(_businessRating);
                _dbContext.SaveChanges();
                return _businessRating;
            }
        }
        #endregion

        #region [ Get Reviews By PlaceID ]
        /// <summary>
        /// Get Reviews By PlaceID
        /// </summary>
        /// <param name="_placeID"></param>
        /// <returns></returns>
        public List<BusinessRating> GetReviewsByPlaceID(String _placeID)
        {
            return _dbContext.BusinessRatings.AsNoTracking().Where(f => f.PlaceID == _placeID).ToList();
        }
        #endregion

        #region [ Is Place Favourite By PlaceID ]
        /// <summary>
        /// Is Place Favourite By PlaceID
        /// </summary>
        /// <param name="_placeID"></param>
        /// <returns></returns>
        public int IsPlaceFavouriteByPlaceID(String _placeID, Int32 _userID)
        {
            if (_dbContext.FavouritePlaces.AsNoTracking().Any(f => f.PlaceID == _placeID && f.UserID == _userID))
                return 1;
            else
                return 0;
        }
        #endregion

        #region [ Get Favourite Places ]
        /// <summary>
        /// Get Favourite Places
        /// </summary>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public List<FavouritePlace> GetFavouritePlaces(Int32 _userID)
        {
            return _dbContext.FavouritePlaces.AsNoTracking().Where(f => f.UserID == _userID).ToList();
        }
        #endregion

        #region [ Add To Favourite ]
        /// <summary>
        /// Add To Favourite
        /// </summary>
        /// <returns></returns>
        public FavouritePlace AddToFavourite(FavouritePlace _favouritePlace, Boolean IsFavourite)
        {
            if (_favouritePlace.BusinessID != null && _favouritePlace.BusinessID!=0)
            {
                Business _business = GetBusinessDetails((int)_favouritePlace.BusinessID);
                if (_business == null)
                    return null;
            }

            FavouritePlace _objFavouritePlace = _dbContext.FavouritePlaces.Where(b => ((b.BusinessID == _favouritePlace.BusinessID && b.PlaceID == _favouritePlace.PlaceID) && b.UserID == _favouritePlace.UserID)).FirstOrDefault();

            if (IsFavourite) // Add To Favourite
            {
                // Check If Already in Favourite List
                if (_objFavouritePlace != null)
                    return _objFavouritePlace;
                else
                {
                    // Add To Favourite Place
                    if (!string.IsNullOrEmpty(_favouritePlace.PlaceID))
                        _favouritePlace.BusinessID = null;

                    _dbContext.FavouritePlaces.Add(_favouritePlace);
                    _dbContext.SaveChanges();
                    return _favouritePlace;
                }
            }
            else // Revemove From Favourite
            {
                if (_objFavouritePlace != null)
                {
                    if (!string.IsNullOrEmpty(_favouritePlace.PlaceID))
                        _objFavouritePlace.BusinessID = null;

                    _dbContext.Entry(_objFavouritePlace).State = System.Data.Entity.EntityState.Deleted;
                    _dbContext.SaveChanges();
                    return _favouritePlace;
                }
                else
                    return null;
            }            
        }
        #endregion

        #region [ Add Support Query ]
        /// <summary>
        /// Add Support Query
        /// </summary>
        /// <param name="_supportQuery"></param>
        /// <returns></returns>
        public SupportQuery AddSupportQuery(SupportQuery _supportQuery)
        {
            _dbContext.SupportQueries.Add(_supportQuery);
            _dbContext.SaveChanges();
            return _supportQuery;
        }
        #endregion
    }
}
