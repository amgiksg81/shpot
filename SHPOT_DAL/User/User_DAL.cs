using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SHPOT_DAL
{
    public class User_DAL
    {
        #region [ Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        private DBContext _dbContext = null;

        public User_DAL()
        {
            _dbContext = new DBContext();
        }
        #endregion

        #region [ Get All Users ]
        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllUsers()
        {
            return _dbContext.Users.AsNoTracking().ToList(); ;
        }
        #endregion

        #region [ Get User Details By UserId ]
        /// <summary>
        /// Get User Details By UserId
        /// </summary>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public User GetUserDetails(int _userID)
        {
            return _dbContext.Users.AsNoTracking().Where(d => d.UserID == _userID).FirstOrDefault();
        }
        #endregion        

        #region [ New User Sign Registration ]
        /// <summary>
        /// New User Sign Registration
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User SignUp(User _user)
        {
            User _userExists = CheckUserExists(_user);
            if (_userExists != null)
            {
                if (_userExists.UserName == _user.UserName)
                    _user.UserName = "UserInvalid";
                else if (_userExists.Email == _user.Email)
                    _user.Email = "EmailInvalid";
                return _user;
            }
            else
            {
                _dbContext.Users.Add(_user);
                _dbContext.SaveChanges();
                return _user;
            }
        }
        #endregion

        #region [ Social User Sign Up / Sign In ]
        /// <summary>
        /// Social User Sign Up / Sign In
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User SocialSignIn(User _user)
        {
            User _userExists = _dbContext.Users.AsNoTracking().Where(d => (d.Email == _user.Email || d.SocialID == _user.SocialID)).FirstOrDefault();
            if (_userExists != null)
            {
                _user.UserID = _userExists.UserID;
                _dbContext.Users.Attach(_user);
                _dbContext.Entry(_user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _user;
            }
            else
            {
                _dbContext.Users.Add(_user);
                _dbContext.SaveChanges();
                return _user;
            }
        }
        #endregion

        #region [ Edit / Update User Details ]
        /// <summary>
        /// Edit / Update User Details
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_newPassword"></param>
        /// <returns></returns>
        public User EditUserProfile(User _user, String _newPassword)
        {
            User _userExists;
            if (!string.IsNullOrEmpty(_user.Password))
                _userExists = _dbContext.Users.Where(d => d.UserID == _user.UserID && d.Password == _user.Password).FirstOrDefault();
            else
                _userExists = _dbContext.Users.Where(d => d.UserID == _user.UserID).FirstOrDefault();

            if (_userExists != null)
            {
                _userExists.FirstName = _user.FirstName;
                _userExists.LastName = _user.LastName;

                if (!string.IsNullOrEmpty(_newPassword))
                    _userExists.Password = _newPassword;

                _userExists.ProfileImageUrl = _user.ProfileImageUrl;
                _dbContext.Entry(_userExists).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _userExists;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region [ Update BrainTree Details Of User ]
        /// <summary>
        /// Update BrainTree Details Of User
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User UpdateBrainTreeDetails(User _user)
        {
            User _userExists;
            _userExists = _dbContext.Users.Where(d => d.UserID == _user.UserID).FirstOrDefault();

            if (_userExists != null)
            {
                _userExists.BrainTreeClientToken = _user.BrainTreeClientToken;
                _userExists.BrainTreeCustomerID = _user.BrainTreeCustomerID;

                _dbContext.Entry(_userExists).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _userExists;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region [ Update Brain Tree Payment ]
        /// <summary>
        /// Update Brain Tree Payment
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public UserPremium UpdateBrainTreePayment(UserPremium _userPremium)
        {
            UserPremium _userPremiumExists;
            _userPremiumExists = _dbContext.UserPremiums.Where(d => d.UserID == _userPremium.UserID && d.BusinessID == _userPremium.BusinessID).FirstOrDefault();

            if (_userPremiumExists != null)
            {
                _userPremiumExists.BrainTreeClientToken = _userPremium.BrainTreeClientToken;
                _userPremiumExists.BrainTreeCustomerID = _userPremium.BrainTreeCustomerID;

                _userPremiumExists.Amount = _userPremium.Amount;
                _userPremiumExists.PaymentMode = _userPremium.PaymentMode;
                _userPremiumExists.TransactionID = _userPremium.TransactionID;
                _userPremiumExists.PaymentDate = DateTime.Now;
                _userPremiumExists.IsActive = true;
                _userPremiumExists.BrainTreeClientToken = _userPremium.BrainTreeClientToken;
                _userPremiumExists.BrainTreeCustomerID = _userPremium.BrainTreeCustomerID;

                _dbContext.Entry(_userPremiumExists).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _userPremiumExists;
            }
            else
            {
                _dbContext.UserPremiums.Add(_userPremium);
                _dbContext.SaveChanges();
                return _userPremium;
            }
        }
        #endregion

        #region [ User Sign In ]
        /// <summary>
        /// User Sign In
        /// </summary>
        /// <param name="_userLogin"></param>
        /// <returns></returns>
        public UserLogin SignIn(UserLogin _userLogin, string _newHeaderToken)
        {
            UserLogin _UserLoginDetails = _dbContext.UserLogins.Where(d => d.UserID == _userLogin.UserID).FirstOrDefault();

            User _userDetails = _dbContext.Users.Where(d => d.UserID == _userLogin.UserID).FirstOrDefault();
            _userDetails.HeaderToken = _newHeaderToken;
            _dbContext.SaveChanges();

            if (_UserLoginDetails == null)
            {

                _dbContext.UserLogins.Add(_userLogin);
                _dbContext.SaveChanges();
                return _userLogin;
            }
            else
            {
                _UserLoginDetails.DeviceToken = _userLogin.DeviceToken;
                _UserLoginDetails.DeviceType = _userLogin.DeviceType;

                _dbContext.Entry(_UserLoginDetails).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _UserLoginDetails;
            }
        }
        #endregion

        #region [ User Sign Out ]
        /// <summary>
        /// User Sign Out
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User SignOut(User _user, string _newHeaderToken)
        {
            User _userDetails = _dbContext.Users.Where(d => d.UserID == _user.UserID && d.HeaderToken == _user.HeaderToken).FirstOrDefault();
            if (_userDetails != null)
            {
                _userDetails.HeaderToken = _newHeaderToken;
                _dbContext.SaveChanges();

                return _userDetails;
            }
            else
                return null;
        }
        #endregion

        #region [ Check if User Already Exists ]
        /// <summary>
        /// Check if User Already Exists
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User CheckUserExists(User _user)
        {
            return _dbContext.Users.AsNoTracking().Where(d => (d.Email == _user.Email || d.UserName == _user.UserName)).FirstOrDefault();
        }
        #endregion

        #region [ Forgot Password + Update Reset Token ]
        /// <summary>
        /// Forgot Password + Update Reset Token
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User ForgotPassword(User _user)
        {
            User _checkUser = _dbContext.Users.Where(d => (d.Email == _user.Email)).FirstOrDefault();
            if (_checkUser != null)
            {
                _checkUser.ResetToken = _user.ResetToken;
                _checkUser.ResetTokenExpiryDate = _user.ResetTokenExpiryDate;
                _checkUser.IsResetTokenActive = _user.IsResetTokenActive;
                _dbContext.Entry(_checkUser).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _checkUser;
            }
            else
                return null;
        }
        #endregion

        #region [ Check Reset Password Validity ]
        /// <summary>
        /// Check Reset Password Validity
        /// </summary>
        /// <param name="_resetToken"></param>
        /// <returns></returns>
        public User CheckResetPwdValidity(string _resetToken)
        {
            User _checkUser = _dbContext.Users.Where(d => d.ResetToken == _resetToken && d.IsResetTokenActive == true).FirstOrDefault();
            if (_checkUser != null && _checkUser.ResetTokenExpiryDate >= DateTime.Now)
            {
                _checkUser.IsResetTokenActive = false;
                _dbContext.Entry(_checkUser).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _checkUser;
            }
            else
                return null;
        }
        #endregion

        #region [ Reset Password + Update Reset Token ]
        /// <summary>
        /// Reset Password + Update Reset Token
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public User ResetPassword(User _user)
        {
            User _checkUser = _dbContext.Users.Where(d => (d.ResetToken == _user.ResetToken)).FirstOrDefault();
            if (_checkUser != null)
            {
                _checkUser.Password = _user.Password;
                _checkUser.ResetToken = null;
                _checkUser.ResetTokenExpiryDate = _user.ResetTokenExpiryDate;
                _checkUser.IsResetTokenActive = false;
                _dbContext.Entry(_checkUser).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return _checkUser;
            }
            else
                return null;
        }
        #endregion
    }
}
