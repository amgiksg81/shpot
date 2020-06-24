using System;
using System.Collections.Generic;
using System.Text;
using SHPOT_DAL;
using SHPOT_ViewModel;
using System.Linq;

namespace SHPOT_BLL
{
    public class User_BLL
    {
        #region [ Default Constructor and Private Members ]
        /// <summary>
        /// Default Constructor and Private Members
        /// </summary>
        User_DAL _objUserDAL;
        UserLoginVM _objUserLoginVM;

        public User_BLL()
        {
            _objUserDAL = new User_DAL();
            _objUserLoginVM = null;
        }
        #endregion

        #region [ Check if Token is Authenticated With User ID ]
        /// <summary>
        /// Check if Token is Authenticated With User ID
        /// </summary>
        /// <param name="_headerToken"></param>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public bool IsTokenAuthenticated(string _headerToken, int _userID)
        {
            User _user = _objUserDAL.GetUserDetails(_userID);
            if (_user != null && _user.HeaderToken == _headerToken)
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region [ Get All User Details ]
        /// <summary>
        /// Get All User Details
        /// </summary>
        /// <returns></returns>
        public List<UserLoginVM> GetAllUsers()
        {
            List<User> _users = _objUserDAL.GetAllUsers();

            if (_users.Count > 0)
            {
                List<UserLoginVM> _listUsersLoginVM = new List<UserLoginVM>();
                foreach (User _user in _users)
                {
                    UserLoginVM _userVM = MakeUserVM(_user);
                    _listUsersLoginVM.Add(_userVM);
                }
                return _listUsersLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Get User Details By ID ]
        /// <summary>
        /// Get User Details BY ID
        /// </summary>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public UserLoginVM GetUserDetails(int _userID)
        {
            User _user = _objUserDAL.GetUserDetails(_userID);
            if (_user != null)
            {
                _objUserLoginVM = MakeUserVM(_user);
                return _objUserLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Check If User Exists ]
        /// <summary>
        /// Check If User Exists
        /// </summary>
        /// <param name="_userVM"></param>
        /// <returns></returns>
        public UserLoginVM CheckUserExists(UserVM _userVM)
        {
            User _user = new User { Email = _userVM.Email, UserName = _userVM.UserName };
            _user = _objUserDAL.CheckUserExists(_user);
            if (_user != null)
            {
                _objUserLoginVM = MakeUserVM(_user);
                return _objUserLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Forgot Password + Update Reset Token ]
        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="_userVM"></param>
        /// <returns></returns>
        public UserLoginVM ForgotPassword(UserVM _userVM)
        {
            User _user = new User { Email = _userVM.Email, ResetToken = RandomString(), IsResetTokenActive = true, ResetTokenExpiryDate = DateTime.Now.AddHours(24)};
            _user = _objUserDAL.ForgotPassword(_user);
            if (_user != null)
            {
                _objUserLoginVM = MakeUserVM(_user);

                // SET REST TOKEN VALUES
                _objUserLoginVM.ResetToken = _user.ResetToken;
                _objUserLoginVM.IsResetTokenActive = _user.IsResetTokenActive;
                _objUserLoginVM.ResetTokenExpiryDate = _user.ResetTokenExpiryDate;

                return _objUserLoginVM;
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
        public UserLoginVM CheckResetPwdValidity(String _resetToken)
        {
            User _user = _objUserDAL.CheckResetPwdValidity(_resetToken);
            if (_user != null)
            {
                _objUserLoginVM = MakeUserVM(_user);
                return _objUserLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Register New User + Social User Login ]
        /// <summary>
        /// Register New User -- 2 Database calls
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <returns></returns>
        public UserLoginVM SignUp(UserLoginVM _userLoginVM)
        {
            User _user = new User
            {
                UserName = _userLoginVM.UserName,
                Email = _userLoginVM.Email,
                Password = _userLoginVM.Password,
                FirstName = _userLoginVM.FirstName,
                LastName = _userLoginVM.LastName,
                UserType = _userLoginVM.UserType,
                CreatedDate = DateTime.Now,
                IPAddress = _userLoginVM.IPAddress,
                IsActive = true,
                ProfileImageUrl = _userLoginVM.ProfileImageUrl,
                SocialID = _userLoginVM.SocialID,
                IsSocialUser = _userLoginVM.IsSocialUser
            };

            if (_userLoginVM.IsSocialUser)
            {
                _user = _objUserDAL.SocialSignIn(_user);
                return UserSignIn(_userLoginVM, _user);
            }
            else
            {
                _user = _objUserDAL.SignUp(_user);
                if (_user.UserName == "UserInvalid")
                {
                    _userLoginVM.UserName = _user.UserName;
                    return _userLoginVM;
                }
                else if (_user.Email == "EmailInvalid")
                {
                    _userLoginVM.Email = _user.Email;
                    return _userLoginVM;
                }
                else
                    return UserSignIn(_userLoginVM, _user);
            }
        }
        #endregion

        #region [ Edit / Update User Details ]
        /// <summary>
        /// Edit / Update User Details
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <returns></returns>
        public UserLoginVM EditUserProfile(UserLoginVM _userLoginVM)
        {
            User _user = new User
            {
                UserID = _userLoginVM.UserID,
                Password = _userLoginVM.Password,
                FirstName = _userLoginVM.FirstName,
                LastName = _userLoginVM.LastName,
                ProfileImageUrl = _userLoginVM.ProfileImageUrl
            };

            _user = _objUserDAL.EditUserProfile(_user, _userLoginVM.NewPassword);
            if (_user != null)
            {
                _userLoginVM = MakeUserVM(_user);
                return _userLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Login User ]
        /// <summary>
        /// Login User  -- 2 Database calls
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <returns></returns>
        public UserLoginVM SignIn(UserLoginVM _userLoginVM)
        {
            User _user = new User { UserName = _userLoginVM.UserName, Email = _userLoginVM.Email, Password = _userLoginVM.Password };
            _user = _objUserDAL.CheckUserExists(_user);    // Sign In -- 1st Hit to Database

            if (_user != null)
            {
                if (_user.Password == _userLoginVM.Password)
                    return UserSignIn(_userLoginVM, _user);
                else
                {
                    _userLoginVM.Password = "InvalidPassword";
                    return _userLoginVM;
                }
            }
            else
                return null;
        }
        #endregion

        #region [ User Sign In [Used in SignUP + SignIn ] 
        /// <summary>
        /// User Sign In [Used in SignUP + SignIn]
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <param name="_user"></param>
        /// <returns></returns>
        private UserLoginVM UserSignIn(UserLoginVM _userLoginVM, User _user)
        {
            if (_user != null)
            {
                UserLogin _userLogin = new UserLogin { UserID = _user.UserID, DeviceToken = _userLoginVM.DeviceToken, DeviceType = _userLoginVM.DeviceType };
                _user.HeaderToken = RandomString();
                _userLogin = _objUserDAL.SignIn(_userLogin, _user.HeaderToken);

                if (_userLogin != null)
                {
                    _userLoginVM.UserID = _user.UserID;
                    _userLoginVM.FirstName = _user.FirstName;
                    _userLoginVM.LastName = _user.LastName;
                    _userLoginVM.UserName = _user.UserName;
                    _userLoginVM.Email = _user.Email;
                    _userLoginVM.UserType = _user.UserType;
                    _userLoginVM.IPAddress = _user.IPAddress;
                    _userLoginVM.HeaderToken = _user.HeaderToken;
                    _userLoginVM.ProfileImageUrl = _user.ProfileImageUrl;
                    _userLoginVM.SocialID = _user.SocialID;
                    _userLoginVM.IsSocialUser = _user.IsSocialUser;

                    _userLoginVM.DeviceToken = _userLogin.DeviceToken;
                    _userLoginVM.DeviceType = _userLogin.DeviceType;

                    if (_user.UserPremiums != null && _user.UserPremiums.Count > 0)
                    {
                        if (_user.UserPremiums.Where(u => u.IsActive == true).Count() > 0)
                        {
                            _userLoginVM.IsUserPremium = true;
                        }
                    }


                    return _userLoginVM;
                }
                else
                    return null;
            }
            else
                return null;
        }
        #endregion

        #region [ Sign Out ]
        /// <summary>
        /// Sign Out 
        /// </summary>
        /// <param name="_userVM"></param>
        /// <returns></returns>
        public bool SignOut(UserVM _userVM)
        {
            User _user = new User { UserID = _userVM.UserID, HeaderToken = _userVM.HeaderToken};
            _user = _objUserDAL.SignOut(_user, RandomString());

            if (_user != null)
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region [ Reset Password ]
        /// <summary>
        /// Add Business Rating
        /// </summary>
        /// <returns></returns>
        public UserVM ResetPassword(UserVM _userVM)
        {
            User _user = _objUserDAL.ResetPassword(new User { ResetToken = _userVM.ResetToken, Password = _userVM.Password });
            return _userVM;
        }
        #endregion

        #region [ Update BrainTree Details Of User ]
        /// <summary>
        /// Update BrainTree Details Of User
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <returns></returns>
        public UserLoginVM UpdateBrainTreeDetails(UserLoginVM _userLoginVM)
        {
            User _user = new User
            {
                UserID = _userLoginVM.UserID,
                BrainTreeClientToken = _userLoginVM.BrainTreeClientToken,
                BrainTreeCustomerID = _userLoginVM.BrainTreeCustomerID
            };

            _user = _objUserDAL.UpdateBrainTreeDetails(_user);
            if (_user != null)
            {
                _userLoginVM = MakeUserVM(_user);
                return _userLoginVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Update BrainTree Payment ]
        /// <summary>
        /// Update BrainTree Payment
        /// </summary>
        /// <param name="_userLoginVM"></param>
        /// <returns></returns>
        public UserPremiumVM UpdateBrainTreePayment(UserPremiumVM _userPremiumVM)
        {
            UserPremium _userPremium = new UserPremium
            {
                UserID = _userPremiumVM.UserID,
                BusinessID = _userPremiumVM.BusinessID,
                Amount = _userPremiumVM.Amount,
                PaymentMode = _userPremiumVM.PaymentMode,
                TransactionID = _userPremiumVM.TransactionID,
                PaymentDate = DateTime.Now,
                IsActive=true,
                BrainTreeClientToken = _userPremiumVM.ClientToken,
                BrainTreeCustomerID = _userPremiumVM.CustomerID
            };

            _userPremium = _objUserDAL.UpdateBrainTreePayment(_userPremium);
            if (_userPremium != null)
            {
                return _userPremiumVM;
            }
            else
                return null;
        }
        #endregion

        #region [ Convert User to User View Model Object ]
        private UserLoginVM MakeUserVM(User item)
        {
            _objUserLoginVM = new UserLoginVM()
            {
                UserID = item.UserID,
                UserName = item.UserName,
                Email = item.Email,
                Password = item.Password,
                FirstName = item.FirstName,
                LastName = item.LastName,
                UserType = item.UserType,
                HeaderToken = item.HeaderToken,
                IPAddress = item.IPAddress,
                IsActive = item.IsActive,
                ProfileImageUrl = item.ProfileImageUrl,
                SocialID = item.SocialID,
                IsSocialUser = item.IsSocialUser,
                IsUserPremium = false
            };

            if (item.UserPremiums != null && item.UserPremiums.Count > 0)
            {
                if (item.UserPremiums.Where(u => u.IsActive == true).Count()>0)
                {
                    _objUserLoginVM.IsUserPremium = true;
                }
            }

            if (item.UserLogins != null && item.UserLogins.Count > 0)
            {
                var lastItem = item.UserLogins.LastOrDefault();

                _objUserLoginVM.DeviceToken = lastItem.DeviceToken;
                _objUserLoginVM.DeviceType = lastItem.DeviceType;
            }            

            return _objUserLoginVM;
        }
        #endregion

        #region [ Generate a random string with a given size ]
        /// <summary>
        /// Generate a random string with a given size
        /// </summary>
        /// <returns></returns>
        private string RandomString()
        {
            int size = 50;
            bool lowerCase = true;
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        #endregion
    }
}