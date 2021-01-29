<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="AppSec_Assignment.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="CSS/general.css"/>
</head>
<body>
    <div class="context">
        <div class="centered">
            <div class="padding">
                <p class="styleheading">Your Profile</p>
                <div class="centered">
    <form id="form1" runat="server">
        <div class="tablestyle">
            Success!<br />
            <br />
            <p id="counter"></p>
            <br/>
            <br/>
            Email:
            <asp:Label ID="lbl_email" runat="server"></asp:Label>
            <br />
            <br />
            First Name:
            <asp:Label ID="lbl_fn" runat="server"></asp:Label>
            <br />
            <br />
            Last Name:
            <asp:Label ID="lbl_ln" runat="server"></asp:Label>
            <br />
            <br />
            Date of Birth:
            <asp:Label ID="lbl_dob" runat="server"></asp:Label>
            <br />
            <br />
            <div class="btn-div">
            <asp:Button ID="btn_Logout" CssClass="btn-oval" runat="server" OnClick="btn_Logout_Click" Text="Log out" />
            </div>
        </div>
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
<script type="text/javascript">
function countdown(minutes) {
    var seconds = 60;
    var mins = minutes
    function tick() {
        //This script expects an element with an ID = "counter". You can change that to what ever you want. 
        var counter = document.getElementById("counter");
        var current_minutes = mins - 1
        seconds--;
        counter.innerHTML = current_minutes.toString() + ":" + (seconds < 10 ? "0" : "") + String(seconds);
        if (seconds > 0) {
            setTimeout(tick, 1000);
        } else {
            if (mins > 1) {
                countdown(mins - 1);
            }
            else {
                alert("Session expired");
                location.reload()
            }
        }
    }
    tick();
}
//You can use this script with a call to onclick, onblur or any other attribute you would like to use. 
countdown(1);//where n is the number of minutes required. 
</script>
</html>
