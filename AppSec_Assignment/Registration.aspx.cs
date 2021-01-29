using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace AppSec_Assignment
{
    public partial class Registration : System.Web.UI.Page
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
        static string salt;
        byte[] Key;
        byte[] IV;
        int loginattempt;
        DateTime dateattempted;
        DateTime minpass;
        DateTime maxpass;
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (EmailExist(tb_email.Text))
            {
                Label1.Text = "Email already exists!";
            }
            else if (checkPassword(tb_password.Text) != 5)
            {
                Label1.Text = "Password is does not meet the requirements!";
            }
            else
            {
                //string pwd = get value from your Textbox
                string pwd = tb_password.Text.ToString().Trim(); ;
                //Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                loginattempt = 0;
                dateattempted = DateTime.Now;
                minpass = DateTime.Now.AddMinutes(5);
                maxpass = DateTime.Now.AddMinutes(15);
                createAccount();
                Response.Redirect("Login.aspx");
            }
        }

        protected void Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName,@LastName, @CardNo,@Email, @PasswordHash,@PasswordSalt, @IV, @Key, @DOB,@paraLoginAttempt,@paraDateAttempted,@paraDateAllowed,@paraMinPass,@paraMaxPass,@paraUsedPass1,@paraUsedPass2)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(tb_firstname.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(tb_lastname.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CardNo", Convert.ToBase64String(encryptData(tb_cardno.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tb_dob.Text.Trim()));
                            cmd.Parameters.AddWithValue("@paraLoginAttempt", loginattempt);
                            cmd.Parameters.AddWithValue("@paraDateAttempted", dateattempted);
                            cmd.Parameters.AddWithValue("@paraDateAllowed", dateattempted);
                            cmd.Parameters.AddWithValue("@paraMinPass", minpass);
                            cmd.Parameters.AddWithValue("@paraMaxPass", maxpass);
                            cmd.Parameters.AddWithValue("@paraUsedPass1", DBNull.Value);
                            cmd.Parameters.AddWithValue("@paraUsedPass2", DBNull.Value);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
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

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }
    }
}