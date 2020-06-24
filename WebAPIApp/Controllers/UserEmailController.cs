using System;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System.IO;
using SHPOT_BLL;
using SHPOT_ViewModel;
using System.Configuration;
using System.Net.Mime;

namespace WebAPIApp.Controllers
{
    /// <summary>
    /// Controller For Sending Emails
    /// </summary>
    public class UserEmailController : Controller
    {
        #region [ Constructor and Private Members ]
        User_BLL _userBLL;

        public UserEmailController()
        {
            _userBLL = new User_BLL();
        }

        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region [ Forgot Password ]
        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="_userVM"></param>
        /// <returns></returns>
        // POST web/useremail/ForgotPassword
        [HttpPost]
        public void ForgotPassword(UserVM _userVM)
        {
            HttpContext.Response.ContentType = "application/json; charset=utf-8";
            try
            {
                //Check If User Exists

                _userVM = _userBLL.ForgotPassword(_userVM);

                if (_userVM != null)
                {
                    //Fetching Email Body Text from EmailTemplate File.  
                    string FilePath = Server.MapPath(@"~\EmailTemplates") + @"\ForgotPassword.html";
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    //Repalce EmailTemplate Parameters                        
                    MailText = MailText.Replace("[#SHPOT]", "SHPOT");
                    MailText = MailText.Replace("[#FirstName]", _userVM.FirstName);
                    MailText = MailText.Replace("[#FullName]", _userVM.FirstName + " " + _userVM.LastName);
                    MailText = MailText.Replace("[#ActionURL]", Common.APIURL + "/Web/UserEmail/ResetUserPassword/" + _userVM.ResetToken);
                    MailText = MailText.Replace("[#SupportURL]", Common.APIURL + "/Web/UserEmail/AddSupportQuery/");

                    String _fromEmail = Common.SMTPUserName;
                    String _fromName = "SHPOT Admin";
                    String _toEmail = _userVM.Email;
                    String _toName = _userVM.FirstName;

                    SendEmailNotifications(_fromEmail, _fromName, _toEmail, _toName, MailText, "Password Reset URL - SHPOT");

                    HttpContext.Response.Write("{\"status\": \"Success\",");
                    HttpContext.Response.Write("\"Message\": \"A link has been sent to your email.\"}");
                }
                else
                {
                    HttpContext.Response.Write("{\"status\": \"Failed\",");
                    HttpContext.Response.Write("\"Message\": \"User does not exists.\"}");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Response.Write("{\"status\": \"Failed\", ");
                HttpContext.Response.Write("\"Message\": \"" + ex.Message + "\"}");
            }
        }
        #endregion

        #region [ Reset User Password ]
        /// <summary>
        /// Reset User Password
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ResetUserPassword(String value)
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.ShowForm = false;
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                TempData["SuccessMessage"] = null;
                return View();
            }

            UserVM _userVM = _userBLL.CheckResetPwdValidity(value);
            ViewBag.UnSuccessMessage = null;
            ViewBag.SuccessMessage = null;

            if (_userVM != null)
            {
                ViewBag.InvalidURL = null;
                ViewBag.ShowForm = true;
                UserVM model = new UserVM() { ResetToken = value };
                return View(model);
            }
            else
            {
                ViewBag.ShowForm = false;
                ViewBag.UnSuccessMessage = "Sorry, Invalid URL. It has been expired now. Please try again for new password.";
                return View();
            }
        }

        /// <summary>
        /// Reset Password POST Mode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetUserPassword(UserVM model)
        {
            ViewBag.UnSuccessMessage = null;
            ViewBag.SuccessMessage = null;
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.UnSuccessMessage = "Please enter password.";
                    return View("AddSupportQuery", model);
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        model.Password = Common.EncryptedPassword(model.Password);
                        UserVM _userVM = _userBLL.ResetPassword(model);
                        TempData["SuccessMessage"] = "Password has been updated successfuly.";
                        return RedirectToAction("ResetUserPassword");
                    }
                    else
                        return View("ResetUserPassword", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.UnSuccessMessage = "Please goto forgot password process again. Some error has occurred. Error is: " + ex.Message;
            }
            return View("ResetUserPassword");
        }
        #endregion

        #region [ Add Support Query ]
        /// <summary>
        /// Add Support Query
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSupportQuery()
        {
            return View();
        }

        /// <summary>
        /// Add Support Query
        /// </summary>
        /// <param name="_Email"></param>
        /// <param name="_Query"></param>
        /// <returns></returns>
        // POST web/useremail/addsupportquery
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddSupportQuery(SupportQueryVM _supportQuery)
        {
            try
            {
                ViewBag.UnSuccessMessage = null;
                ViewBag.SuccessMessage = null;

                if (!ModelState.IsValid)
                {
                    ViewBag.UnSuccessMessage = "Please enter your Email Address/Query.";
                    return View("AddSupportQuery");
                }
                else
                {
                    _supportQuery.IPAddress = Request.UserHostAddress;

                    if (_supportQuery.EmailAddress != string.Empty && _supportQuery.Query != string.Empty)
                    {
                        _supportQuery = new Business_BLL().AddSupportQuery(_supportQuery);

                        if (_supportQuery != null)
                        {
                            ViewBag.SuccessMessage = "Your query has been submitted successfully. Support person will contact you shortly to help you with this.";

                            //Send Email to Admin
                            //Fetching Email Body Text from EmailTemplate File.  
                            string FilePath = Server.MapPath(@"~\EmailTemplates") + @"\SupportQuery.html";
                            StreamReader str = new StreamReader(FilePath);
                            string MailText = str.ReadToEnd();
                            str.Close();

                            //Repalce EmailTemplate Parameters                        
                            MailText = MailText.Replace("[#SHPOT]", "SHPOT");
                            MailText = MailText.Replace("[#FULLNAME]", _supportQuery.FullName);
                            MailText = MailText.Replace("[#IPADDRESS]", _supportQuery.IPAddress);
                            MailText = MailText.Replace("[#QUERYDATE]", _supportQuery.QueryDateTime.ToString());
                            MailText = MailText.Replace("[#EMAILID]", _supportQuery.EmailAddress);
                            MailText = MailText.Replace("[#QUERY]", _supportQuery.Query);

                            String _fromEmail = _supportQuery.EmailAddress;
                            String _fromName = _supportQuery.FullName;
                            String _toEmail = Common.SMTPUserName;
                            String _toName = "SHPOT Admin";
                            SendEmailNotifications(_supportQuery.EmailAddress, _supportQuery.FullName, _toEmail, _toName, MailText, "Support Query By: " + _supportQuery.FullName);
                        }
                        else
                            ViewBag.UnSuccessMessage = "Please submit your query again.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.UnSuccessMessage = "Please submit your query again. Some error has occurred. Error is: " + ex.Message;
            }
            return View("AddSupportQuery");
        }
        #endregion

        #region [ Send Email Notifications ]
        /// <summary>
        /// Send Email Notifications
        /// </summary>
        /// <param name="_fromEmail"></param>
        /// <param name="_fromName"></param>
        /// <param name="_toEmail"></param>
        /// <param name="_toName"></param>
        /// <param name="MailText"></param>
        /// <param name="_Subject"></param>
        private static void SendEmailNotifications(String _fromEmail, String _fromName, String _toEmail, String _toName, String MailText, String _Subject)
        {
            SmtpClient client = new SmtpClient(Common.SMTPHostName, Common.SMTPPort);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Timeout = 50000;
            client.Credentials = new NetworkCredential(Common.SMTPUserName, Common.SMTPUserPassword);

            MailMessage mailMessage = new MailMessage(new MailAddress(_fromEmail, _fromName), new MailAddress(_toEmail, _toName));
            mailMessage.Subject = _Subject;
            mailMessage.Body = MailText;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;

            string PlainText = "plain text";
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(PlainText, new ContentType("text/plain")));
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(MailText, new ContentType("text/html")));
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

            client.Send(mailMessage);
        }
        #endregion
    }
}