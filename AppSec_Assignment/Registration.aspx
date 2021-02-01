<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="AppSec_Assignment.Registration" ValidateRequest="true"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="CSS/general.css"/>
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
    </script>
</head>
<body>
    <div class="context">
    <div class="centered">
        <div class="padding2">
            <p class="styleheading">Registration</p>
            <div class="centered fix">
    <form id="form1" runat="server">
        <div class="tablestyle">
    <table class="auto-style1">
        <tr>
            <td class="auto-style2">First Name:</td>
            <td>
                <asp:TextBox ID="tb_firstname" runat="server" CssClass="text-input"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="First Name Required" ControlToValidate="tb_firstname" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">Last Name:</td>
            <td>
                <asp:TextBox ID="tb_lastname" runat="server" CssClass="text-input"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Last Name Required" ControlToValidate="tb_lastname" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">Credit Card Number (VISA Only):</td>
            <td>
                <asp:TextBox ID="tb_cardno" runat="server" CssClass="text-input"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Credit Card Required" ControlToValidate="tb_cardno" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="Invalid card number" ControlToValidate="tb_cardno" ValidationExpression="^4[0-9]{12}(?:[0-9]{3})?$" ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">Email address:</td>
            <td>
                <asp:TextBox ID="tb_email" runat="server" CssClass="text-input"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        ControlToValidate="tb_email" ErrorMessage="Email is required"
        SetFocusOnError="True" ForeColor="Red" ></asp:RequiredFieldValidator>

            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
             ErrorMessage="Invalid Email" ControlToValidate="tb_email"
             SetFocusOnError="True"
             ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">Password:</td>
            <td>
                <asp:TextBox ID="tb_password" runat="server" CssClass="text-input" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Password is required" ForeColor="Red" 
                    ControlToValidate="tb_password">
                </asp:RequiredFieldValidator>
                <br />
                <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">Date of Birth (YYYY-MM-DD):</td>
            <td>
                <asp:TextBox ID="tb_dob" runat="server" CssClass="auto-style3 text-input"></asp:TextBox>
                                <br />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
        ControlToValidate="tb_dob" ErrorMessage="Date of Birth is required"
        SetFocusOnError="True" ForeColor="Red" ></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">&nbsp;</td>
            <td>
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
             ErrorMessage="Invalid Date of Birth" ControlToValidate="tb_dob"
             SetFocusOnError="True"
             ValidationExpression="20\d{2}(-|\/)((0[1-9])|(1[0-2]))(-|\/)((0[1-9])|([1-2][0-9])|(3[0-1]))" ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">&nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    </div>
        <div class="btn-div">
        <p>
&nbsp;<asp:Button ID="btn_Submit" CssClass="btn-oval" runat="server" Text="Register Account" OnClick="btn_Submit_Click" />
        </p>
            <br/>
            <asp:Button ID="Return" CssClass="btn-oval" runat="server" CausesValidation="false" Text="Return to login page" OnClick="Return_Click" />
            </div>
        <asp:Label ID="Label1" ForeColor="Red" runat="server"></asp:Label>
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
