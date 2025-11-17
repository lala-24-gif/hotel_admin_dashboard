using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace HotelManagement
{
    public partial class Register : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if already logged in
                if (Session["AdminID"] != null)
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string fullName = txtFullName.Text.Trim();
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                // Check if username or email already exists
                if (UserExists(username, email))
                {
                    // CHANGED: Username or email already exists
                    ShowError("ユーザー名またはメールアドレスは既に使用されています。別の情報を入力してください。");
                    return;
                }

                // Register the user
                if (RegisterUser(fullName, username, email, password))
                {
                    // CHANGED: Account created successfully
                    ShowSuccess("アカウントが正常に作成されました！ログインページにリダイレクトしています...");

                    // Redirect to login page after 2 seconds
                    Response.AddHeader("REFRESH", "2;URL=Login.aspx");
                }
                else
                {
                    // CHANGED: Registration failed
                    ShowError("登録に失敗しました。もう一度お試しください。");
                }
            }
        }

        private bool UserExists(string username, string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM AdminUser WHERE Username = @Username OR Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ユーザー存在確認エラー: " + ex.Message);
                return true; // Return true to prevent registration on error
            }
        }

        private bool RegisterUser(string fullName, string username, string email, string password)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO AdminUser (Username, Password, Email, FullName, Role, IsActive, CreatedDate) 
                                   VALUES (@Username, @Password, @Email, @FullName, 'Admin', 1, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", HashPassword(password));
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@FullName", fullName);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("登録エラー: " + ex.Message);
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
        }
    }
}