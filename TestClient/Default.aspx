<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestClient.Default" %>

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
                    <td>UserName</td>
                    <td><asp:TextBox ID="txtUserName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Email</td>
                    <td><asp:TextBox ID="txtEmail" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td><asp:TextBox ID="txtPassword" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>First Name</td>
                    <td><asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Last Name</td>
                    <td><asp:TextBox ID="txtLastName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>User Type</td>
                    <td>
                        <asp:RadioButtonList ID="rdoUserType" runat="server">
                            <asp:ListItem Selected="True" Text="Business" Value="Business"></asp:ListItem>
                            <asp:ListItem Text="User" Value="User"></asp:ListItem>
                        </asp:RadioButtonList>

                    </td>
                </tr>                            
                <tr>
                    <td>IP Address</td>
                    <td><asp:TextBox ID="txtIPAddress" runat="server"></asp:TextBox></td>
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
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
                        - 
                        <asp:Button ID="btnNew" runat="server" Text="Clear Data" OnClick="btnNew_Click" />
                        - 
                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                    </td>
                </tr>
            </table>
            <hr />
            <asp:Label ID="lblResponse" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
