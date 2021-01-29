<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="AppSec_Assignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="CSS/General.css"/>
    <script type="text/javascript">
    function validate() {
        var str = document.getElementById('<%=tb_password.ClientID %>').value;
        if (str.length < 8) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password length must be at least 8 characters";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("too_short");
        }
        else if (str.search(/[0-9]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_number");
        }
        else if (!/[A-Z]/.test(str)) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 Uppercase";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_uppercase");
        }
        else if (!/[a-z]/.test(str)) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 Lowercase";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_lowercase");
        }
        else if (str.search(/[^A-Za-z0-9]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 special character";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_specialcharacter");
        }
        document.getElementById("lbl_pwdchecker").innerHTML = "Excellent!"
        document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }

    function confirmpassword() {
        if (document.getElementById('<%= tb_password.ClientID %>').value != document.getElementById('<%= tb_confirmpassword.ClientID %>').value) {
            document.getElementById("lbl_cfmpwdchecker").innerHTML = "Password does not match!"
            document.getElementById("lbl_cfmpwdchecker").style.color = "red";
            return ("no_match");
        }
        else {
            document.getElementById("lbl_cfmpwdchecker").innerHTML = "";
        }
    }
    </script>
</head>
<body>
    <div class="context">
        <div class="centered">
            <div class="padding3">
                <p class="styleheading">Change Password</p>
                <div class="centered">
                    <div class="tablestyle">
                        <form id="form1" runat="server">
                            <div>
                                <p>Email:</p>
                                <asp:TextBox ID="tb_email" CssClass="text-input" runat="server"></asp:TextBox>
                                <p>Current Password:</p>
                                <asp:TextBox ID="tb_currentpass" CssClass="text-input" runat="server"></asp:TextBox>
                                <p>New Password:</p>
                                <asp:TextBox ID="tb_password" runat="server" CssClass="text-input" onkeyup="javascript:validate()"></asp:TextBox>
                                <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>
                                <p>Confirm Password:</p>
                                <asp:TextBox ID="tb_confirmpassword" runat="server" CssClass="text-input" onkeyup="javascript:confirmpassword()"></asp:TextBox>
                                <asp:Label ID="lbl_cfmpwdchecker" runat="server"></asp:Label>
                                <div class="btn-div">
                                <p>
                                    <asp:Button ID="submit_btn" CssClass="btn-oval" runat="server" Text="Submit" OnClick="submit_btn_Click" /></p>
                                <p>
                                    <br/>
                                    <asp:Button ID="Button1" runat="server" CssClass="btn-oval" Text="Return to login page" OnClick="Button1_Click" /></p>
                                </div>
                                <asp:Label ID="emailcheck" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                        </form>
                    </div>
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
