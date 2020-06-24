<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TestClient.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>UserName/Email</td>
                    <td><asp:TextBox ID="txtEmail" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td><asp:TextBox ID="txtPassword" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Device Token</td>
                    <td><asp:TextBox ID="txtDeviceToken" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Device Type</td>
                    <td><asp:TextBox ID="txtDeviceType" runat="server"></asp:TextBox></td>
                </tr>                
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                        - 
                        <asp:Button ID="btnRegister" runat="server" Text="Register New User" OnClick="btnRegister_Click" />

                    </td>
                </tr>
            </table>
            <hr />
            <asp:Label ID="lblResponse" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
