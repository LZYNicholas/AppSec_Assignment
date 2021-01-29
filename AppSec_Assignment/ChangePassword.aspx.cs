using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSec_Assignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }

        string MYDBConnectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        static string finalHash;
        protected void submit_btn_Click(object sender, EventArgs e)
        {
            string pwd = HttpUtility.HtmlEncode(tb_currentpass.Text.ToString().Trim());
            string userid = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
            {
                string pwdWithSalt = pwd + dbSalt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                string userHash = Convert.ToBase64String(hashWithSalt);
                if (userHash.Equals(dbHash))
                {
                    if (tb_password.Text == tb_confirmpassword.Text)
                    {
                        if (checkPassword(tb_password.Text) == 5)
                        {
                            if (Convert.ToDateTime(getMinPass(userid)) < DateTime.Now)
                            {
                                string newpwd = HttpUtility.HtmlEncode(tb_password.Text.ToString().Trim());
                                string pwdWithSalt2 = newpwd + dbSalt;
                                byte[] hashWithSalt2 = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt2));
                                string userHash2 = Convert.ToBase64String(hashWithSalt2);
                                if (userHash2.Equals(getUsedPassHash1(userid)) || userHash2.Equals(getUsedPassHash2(userid)))
                                {
                                    emailcheck.Text = "You cannot change password to your previous 2 passwords!";
                                }
                                else if (userHash2.Equals(dbHash))
                                {
                                    emailcheck.Text = "Cannot change password to current password!";
                                }
                                else
                                {
                                    //string pwd = get value from your Textbox
                                    string passwd = tb_password.Text.ToString().Trim();
                                    //Generate random "salt"
                                    SHA512Managed hash = new SHA512Managed();
                                    string passwdWithSalt = passwd + dbSalt;
                                    byte[] plainHash = hash.ComputeHash(Encoding.UTF8.GetBytes(passwd));
                                    byte[] hashSalt = hash.ComputeHash(Encoding.UTF8.GetBytes(passwdWithSalt));
                                    finalHash = Convert.ToBase64String(hashSalt);
                                    DateTime minpass = DateTime.Now.AddMinutes(5);
                                    DateTime maxpass = DateTime.Now.AddMinutes(15);
                                    UpdateAccountPassword(userid, finalHash, minpass, maxpass);
                                    if (getUsedPassHash1(userid) == null)
                                    {
                                        UpdateUsedPassHash1(userid, userHash);
                                    }
                                    else if (getUsedPassHash1(userid) != null && getUsedPassHash2(userid) == null)
                                    {
                                        UpdateUsedPassHash2(userid, userHash);
                                    }
                                    else
                                    {
                                        UpdateUsedPassHash1(userid, getUsedPassHash2(userid));
                                        UpdateUsedPassHash2(userid, userHash);
                                    }
                                    emailcheck.Text = "Password Changed!";
                                }
                            }
                            else
                            {
                                emailcheck.Text = "Cannot change password too quickly!";
                            }
                        }
                        else
                        {
                            emailcheck.Text = "New password is not strong enough!";
                        }
                    }
                    else
                    {
                        emailcheck.Text = "New password and confirm password does not match!";
                    }
                }
                else
                {
                    emailcheck.Text = "Invalid email or password";
                }
            }
            else
            {
                emailcheck.Text = "Invalid email or password";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
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
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getUsedPassHash1(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select UsedPass1 FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["UsedPass1"] != null)
                        {
                            if (reader["UsedPass1"] != DBNull.Value)
                            {
                                h = reader["UsedPass1"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getUsedPassHash2(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select UsedPass2 FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["UsedPass2"] != null)
                        {
                            if (reader["UsedPass2"] != DBNull.Value)
                            {
                                h = reader["UsedPass2"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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

        protected string getMinPass(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select MinPassAge FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["MinPassAge"] != null)
                        {
                            if (reader["MinPassAge"] != DBNull.Value)
                            {
                                h = reader["MinPassAge"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return h;
        }

        public int UpdateAccountPassword(string id, string finalhash, DateTime minpass, DateTime maxpass)
        {
            string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            SqlConnection myConn = new SqlConnection(DBConnect);

            string sqlStmt = "UPDATE Account SET PasswordHash=@paraPasswordHash,MinPassAge=@paraMinPass,MaxPassAge=@paraMaxPass WHERE Email=@paraEmail";

            SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

            sqlCmd.Parameters.AddWithValue("@paraEmail", id);
            sqlCmd.Parameters.AddWithValue("@paraPasswordHash", finalhash);
            sqlCmd.Parameters.AddWithValue("@paraMinPass", minpass);
            sqlCmd.Parameters.AddWithValue("@paraMaxPass", maxpass);

            myConn.Open();
            int result = sqlCmd.ExecuteNonQuery();
            myConn.Close();

            return result;
        }

        public int UpdateUsedPassHash1(string id, string hash)
        {
            string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            SqlConnection myConn = new SqlConnection(DBConnect);

            string sqlStmt = "UPDATE Account SET UsedPass1=@paraUsedPass WHERE Email=@paraEmail";

            SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

            sqlCmd.Parameters.AddWithValue("@paraEmail", id);
            sqlCmd.Parameters.AddWithValue("@paraUsedPass", hash);


            myConn.Open();
            int result = sqlCmd.ExecuteNonQuery();
            myConn.Close();

            return result;
        }

        public int UpdateUsedPassHash2(string id, string hash)
        {
            string DBConnect = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            SqlConnection myConn = new SqlConnection(DBConnect);

            string sqlStmt = "UPDATE Account SET UsedPass2=@paraUsedPass WHERE Email=@paraEmail";

            SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

            sqlCmd.Parameters.AddWithValue("@paraEmail", id);
            sqlCmd.Parameters.AddWithValue("@paraUsedPass", hash);

            myConn.Open();
            int result = sqlCmd.ExecuteNonQuery();
            myConn.Close();

            return result;
        }

        public bool EmailExist(string id)
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
            int rec_cnt = ds.Tables[0].Rows.Count;
            if (rec_cnt == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}