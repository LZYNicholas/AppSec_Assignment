using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSec_Assignment
{
    public partial class Login : System.Web.UI.Page
    {
        public class MyObject
        {
            public string success { get; set; }
            public List<String> ErrorMessage { get; set; }
        }
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string pwd = HttpUtility.HtmlEncode(tb_password.Text.ToString().Trim());
            string userid = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (ValidateCaptcha())
                    {
                        int loginAttempt = Convert.ToInt32(getLoginAttempt(userid));
                        if (loginAttempt == 3)
                        {
                            DateTime dateAllowed = Convert.ToDateTime(getDateAllowed(userid));
                            if (DateTime.Now < dateAllowed)
                            {
                                errorMsg.Text = "Your Account Is Locked! Please Try Again Later.";
                            }
                            else if (userHash.Equals(dbHash))
                            {
                                if (DateTime.Now > Convert.ToDateTime(getMaxPass(userid)))
                                {
                                    errorMsg.Text = "You are required to reset your password";
                                }
                                else
                                {
                                    UpdateAccountLoginAttempt(userid, 0);
                                    Session["UserID"] = userid;
                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                    Response.Redirect("Success.aspx", false);
                                }
                            }
                            else
                            {
                                UpdateAccountLoginAttempt(userid, 1);
                                UpdateAccountDate(userid, DateTime.Now, DateTime.Now);
                                errorMsg.Text = "Userid or password is not valid. Please try again.";
                                tb_email.Text = "";
                                tb_password.Text = "";
                            }
                        }
                        else if (userHash.Equals(dbHash))
                        {
                            if (DateTime.Now > Convert.ToDateTime(getMaxPass(userid)))
                            {
                                errorMsg.Text = "You are required to reset your password";
                            }
                            else
                            {
                                UpdateAccountLoginAttempt(userid, 0);
                                Session["UserID"] = userid;
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                Response.Redirect("Success.aspx", false);
                            }
                        }

                        else if (loginAttempt == 2)
                        {
                            int updatedAttempt = loginAttempt + 1;
                            DateTime dateattempted = DateTime.Now;
                            DateTime Allowed = DateTime.Now.AddMinutes(15);
                            UpdateAccountDate(userid, dateattempted, Allowed);
                            UpdateAccountLoginAttempt(userid, updatedAttempt);
                            errorMsg.Text = "Userid or password is not valid. Please try again.";
                            tb_email.Text = "";
                            tb_password.Text = "";
                        }
                        else if (loginAttempt < 2)
                        {
                            int updatedAttempt = loginAttempt + 1;
                            UpdateAccountLoginAttempt(userid, updatedAttempt);
                            errorMsg.Text = "Userid or password is not valid. Please try again.";
                            tb_email.Text = "";
                            tb_password.Text = "";
                        }
                    }
                }
                else
                {
                    errorMsg.Text = "Userid or password is not valid. Please try again.";
                    tb_email.Text = "";
                    tb_password.Text = "";
                }
                //Response.Redirect("Login.aspx", false);
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally { }
        }

        protected void changepwd_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx");
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        public bool EmailExist(string id)
        {
            try
            {
                //Step 1 -  Define a connection to the database by getting
                //          the connection string from App.config
                string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                //Step 2 -  Create a DataAdapter object to retrieve data from the database table
                string sqlStmt = "Select * from Account where Email = @paraId";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraId", id);

                //Step 3 -  Create a DataSet to store the data to be retrieved
                DataSet ds = new DataSet();

                //Step 4 -  Use the DataAdapter to fill the DataSet with data retrieved
                da.Fill(ds);

                // Read data from DataSet.
                int count = ds.Tables[0].Rows.Count;
                if (count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getMaxPass(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select MaxPassAge FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["MaxPassAge"] != null)
                        {
                            if (reader["MaxPassAge"] != DBNull.Value)
                            {
                                h = reader["MaxPassAge"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally
            {
                connection.Close();
            }

            return h;
        }

        protected string getLoginAttempt(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginAttempt FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["LoginAttempt"] != null)
                        {
                            if (reader["LoginAttempt"] != DBNull.Value)
                            {
                                h = reader["LoginAttempt"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally { connection.Close(); }
            return h;
        }
        protected string getDateAllowed(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select DateAllowed FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["DateAllowed"] != null)
                        {
                            if (reader["DateAllowed"] != DBNull.Value)
                            {
                                h = reader["DateAllowed"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally
            {
                connection.Close();
            }

            return h;
        }

        public int UpdateAccountLoginAttempt(string id, int attempt)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "UPDATE Account SET LoginAttempt=@paraLoginAttempt WHERE Email=@paraEmail";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@paraEmail", id);
                sqlCmd.Parameters.AddWithValue("@paraLoginAttempt", attempt);

                myConn.Open();
                int result = sqlCmd.ExecuteNonQuery();
                myConn.Close();
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public int UpdateAccountDate(string id, DateTime dateattempted, DateTime dateallowed)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "UPDATE Account SET DateAttempted=@paraDateAttempted, DateAllowed=@paraDateAllowed WHERE Email=@paraEmail";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@paraEmail", id);
                sqlCmd.Parameters.AddWithValue("@paraDateAttempted", dateattempted);
                sqlCmd.Parameters.AddWithValue("@paraDateAllowed", dateallowed);

                myConn.Open();
                int result = sqlCmd.ExecuteNonQuery();
                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("HelpLink = {0}" + ex.HelpLink + "<br>");
                Response.Write("Message = {0}" + ex.Message + "<br>");
                Response.Write("Source = {0}" + ex.Source + "<br>");
                Response.Write("StackTrace = {0}" + ex.StackTrace + "<br>");
                Response.Write("TargetSite = {0}" + ex.TargetSite + "<br>");
            }
            finally { connection.Close(); }
            return s;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6Le0sB0aAAAAAJUWOrHUobhl1-u0btmJHnaPcVjZ &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        lbl_gScore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}