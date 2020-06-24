using System.ServiceModel;
using System.ServiceModel.Web;
using SHPOT_ViewModel;
using System.IO;

namespace SHPOTApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAPI
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetUserDetails/{userid}", ResponseFormat = WebMessageFormat.Json)]
        Stream GetUserDetails(string userid);

        [OperationContract]
        [WebGet(UriTemplate = "GetAllUsers", ResponseFormat = WebMessageFormat.Json)]
        Stream GetAllUsers();

        [OperationContract]
        [WebInvoke(UriTemplate = "/SignUp", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Stream SignUp(UserLoginVM _userLogin);

        [OperationContract]
        [WebInvoke(UriTemplate = "/SignIn", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Stream SignIn(UserLoginVM _userLogin);

        [OperationContract]
        [WebInvoke(UriTemplate = "TestSign", ResponseFormat = WebMessageFormat.Json)]
        Stream TestSign();
    }    
}
