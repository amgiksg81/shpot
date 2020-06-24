using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using SHPOT_ViewModel;
using SHPOT_BLL;

namespace TestClient
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            User_BLL _userBLL = new User_BLL();
            UserLoginVM _userLogin = new UserLoginVM
            {
                UserName = txtUserName.Text,
                Email = txtEmail.Text,
                Password = txtPassword.Text,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                UserType = rdoUserType.SelectedValue,
                IPAddress = txtIPAddress.Text,
                DeviceToken = txtDeviceToken.Text,
                DeviceType = txtDeviceType.Text
            };

            #region Uploading JOSN Data
            WebClient Proxy1 = new WebClient();
            Proxy1.Headers["Content-type"] = "application/json";
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializerToUplaod = new DataContractJsonSerializer(typeof(UserLoginVM));
            serializerToUplaod.WriteObject(ms, _userLogin);
            //byte[] data = Proxy1.UploadData("http://shpot2018-001-site1.btempurl.com/api/values/signup", "POST", ms.ToArray());
            //byte[] data = Proxy1.UploadData("http://shpot2018-001-site1.btempurl.com/api/API.svc/signup", "POST", ms.ToArray());
            //byte[] data = Proxy1.UploadData("http://localhost:64003/api.svc/signup", "POST", ms.ToArray());
            byte[] data = Proxy1.UploadData("http://localhost:61630/api/values/signup", "POST", ms.ToArray());

            Stream stream = new MemoryStream(data);

            string response = "";

            using (Stream webStream = new MemoryStream(data))
            {
                if (webStream != null)
                {
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        response = responseReader.ReadToEnd();
                    }
                }
            }

            lblResponse.Text = response;
            #endregion
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}