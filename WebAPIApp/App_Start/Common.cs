using SHPOT_BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace WebAPIApp
{
    public class Common
    {
        #region [ Static Variables from Configuration File ]
        /// <summary>
        /// Static Variables from Configuration File
        /// </summary>
        public static string APIURL = ConfigurationManager.AppSettings["APIURL"];
        public static string GooglePlaceAPIURL = ConfigurationManager.AppSettings["GooglePlaceAPIURL"];
        public static string GooglePlaceAPIReviewURL = ConfigurationManager.AppSettings["GooglePlaceAPIReviewURL"];
        public static string GooglePlaceAPIKey = ConfigurationManager.AppSettings["GooglePlaceAPIKey"];
        public static string SMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
        public static string SMTPUserPassword = ConfigurationManager.AppSettings["SMTPUserPassword"];
        public static string SMTPHostName = ConfigurationManager.AppSettings["SMTPHostName"];
        public static int SMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
        #endregion

        #region [ Check if Token is Authenticated With User ID ]
        /// <summary>
        /// Check if Token is Authenticated With User ID
        /// </summary>
        /// <returns></returns>
        public static bool IsTokenAuthenticated(HttpRequestHeaders _refHeaders, ref int _refUserId, ref StringBuilder _refJSONContent)
        {
            string _headerToken = string.Empty;
            try
            {
                if (_refHeaders.Contains("header_token"))
                {
                    _headerToken = _refHeaders.GetValues("header_token").First();
                }
                if (_refHeaders.Contains("user_id"))
                {
                    _refUserId = Convert.ToInt32(_refHeaders.GetValues("user_id").First().Replace("\"", ""));
                }

                if (_headerToken == string.Empty || _refUserId == 0)
                {
                    _refJSONContent.Append("{\"status\":\"UnAuthorized\"}");
                    return false;
                }
                else if (!(new User_BLL().IsTokenAuthenticated(_headerToken, _refUserId)))
                {
                    _refJSONContent.Append("{\"status\":\"UnAuthorized\"}");
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                _refJSONContent.Append("{\"status\":\"UnAuthorized - " + ex.Message + "\"}");
                return false;
            }
        }
        #endregion

        #region [ API's Final Response Output ]
        /// <summary>
        /// API's Final Response Output
        /// </summary>
        /// <returns></returns>
        public static HttpResponseMessage ResponseOutput(StringBuilder _strJSONContent)
        {
            var resp = new HttpResponseMessage() { Content = new StringContent(_strJSONContent.ToString()) };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }
        #endregion

        #region [ Generate Header and JSON Data for ResponseRequest ]
        /// <summary>
        /// Generate Header and JSON Data for ResponseRequest
        /// </summary>
        /// <param name="_UserVM"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static StringBuilder GenerateReturnJSONData(string _headerToken, string _SuccessCode, string _JSONData, string _Message, string _Type)
        {
            StringBuilder strJSONContent = new StringBuilder();
            strJSONContent.Append("{");

            if (!string.IsNullOrWhiteSpace(_headerToken))
                strJSONContent.Append("\"headers\":\"" + _headerToken + "\",");

            if (!string.IsNullOrWhiteSpace(_Message))
                strJSONContent.Append(_Message);

            strJSONContent.Append("\"success\":\"true\",");

            if (!string.IsNullOrWhiteSpace(_SuccessCode))
                strJSONContent.Append("\"SuccessCode\":\"" + _SuccessCode + "\",");

            strJSONContent.Append("\"" + _Type + "\":");
            strJSONContent.Append(_JSONData);

            strJSONContent.Append("}");

            return strJSONContent.Replace("\\", "");
        }
        #endregion

        #region [ Failed Response for Request ]
        /// <summary>
        /// Failed Response for Request
        /// </summary>
        /// <returns></returns>
        public static StringBuilder FailureResponseRequest(string _FailureCode, string _Message)
        {
            StringBuilder strJSONContent = new StringBuilder();
            strJSONContent.Append("{");

            if (!string.IsNullOrWhiteSpace(_FailureCode))
                strJSONContent.Append("\"SuccessCode\":\"" + _FailureCode + "\",");
            if (_Message != "")
                strJSONContent.Append(_Message);
            strJSONContent.Append("\"success\":\"false\"");

            strJSONContent.Append("}");

            return strJSONContent;
        }
        #endregion

        #region [Password Encryption/Decryption]
        public static string _encryptionPrivateKey = ConfigurationManager.AppSettings["EncryptionPrivateKey"].ToString();

        /// <summary>
        /// Password Encryption
        /// </summary>
        public static string EncryptedPassword(string _password)
        {
            return CryptorEngine.Encrypt(_password, true, _encryptionPrivateKey);
        }

        /// <summary>
        /// Password Decryption
        /// </summary>
        public static string DecryptedPassword(string _password)
        {
            return CryptorEngine.Decrypt(_password, true, _encryptionPrivateKey);
        }
        #endregion
    }

    public struct UserTypes
    {
        public static string User = "User";
        public static string Business = "Business";
    }
}