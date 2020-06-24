using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using SHPOT_ViewModel;
using SHPOT_BLL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestClient
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            #region Uploading JOSN Data
            UserLoginVM _userLogin = new UserLoginVM
            {
                UserName = txtEmail.Text,
                Email = txtEmail.Text,
                Password = txtPassword.Text,
                DeviceToken = txtDeviceToken.Text,
                DeviceType = txtDeviceType.Text
            };

            WebClient Proxy1 = new WebClient();
            Proxy1.Headers["Content-type"] = "application/json";
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializerToUplaod = new DataContractJsonSerializer(typeof(UserLoginVM));
            serializerToUplaod.WriteObject(ms, _userLogin);
            //byte[] data = Proxy1.UploadData("http://shpot2018-001-site1.btempurl.com/api/values/signin", "POST", ms.ToArray());
            //byte[] data = Proxy1.UploadData("http://shpot2018-001-site1.btempurl.com/api/API.svc/signin", "POST", ms.ToArray());
            //byte[] data = Proxy1.UploadData("http://localhost:64003/api.svc/signin", "POST", ms.ToArray());
            byte[] data = Proxy1.UploadData("http://localhost:61630/api/values/signin", "POST", ms.ToArray());
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

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}