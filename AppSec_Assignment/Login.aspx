<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSec_Assignment.Login" ValidateRequest="true"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Le0sB0aAAAAABE1tTszfPMnMaDiCFlxu0WXxD6o"></script>
    <link rel="stylesheet" type="text/css" href="CSS/general.css"/>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 78px;
        }
        .auto-style3 {
            width: 78px;
            height: 23px;
        }
        .auto-style4 {
            height: 23px;
        }
    </style>
</head>
<body>
    <div class="context">
    <div class="centered">
     <div class="padding">
         <p class="styleheading">LOGIN</p>
         <div class="centered">
    <form id="form1" runat="server">
        <div class="tablestyle">
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Email:</td>
                    <td>
                        <asp:TextBox ID="tb_email" CssClass="text-input" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Password:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tb_password" CssClass="text-input" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
        <br/>
        <div class="btn-div">
            <asp:Button ID="btn_Submit" cssClass="btn-oval" runat="server" OnClick="Button1_Click" Text="Sign in" />
            <br/>
            <br/>
            <asp:Button ID="Register" cssClass="btn-oval" runat="server" OnClick="Register_Click" Text="Register" />
            <br/>
            <br/>
            <asp:Button ID="changepwd" cssClass="btn-oval" runat="server" OnClick="changepwd_Click" Text="Change password" />
            <p>
                <asp:Label ID="errorMsg" runat="server" ForeColor="Red"></asp:Label>
            </p>
        </div>
    <script>
         grecaptcha.ready(function () {
             grecaptcha.execute('6Le0sB0aAAAAABE1tTszfPMnMaDiCFlxu0WXxD6o', { action: 'Login' }).then(function (token) {
             document.getElementById("g-recaptcha-response").value = token;
         });
         });
    </script>
        <asp:Label ID="lbl_gScore" runat="server"></asp:Label>
    </form>
             </div>
         </div>
        </div>
        </div>

    <div class="area" >
            <ul class="circles">
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
                    <li></li>
            </ul>
    </div >
    </body>
</html>
