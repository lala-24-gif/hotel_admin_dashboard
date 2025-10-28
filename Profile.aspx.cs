using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HotelManagement
{
    public partial class Profile : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ユーザーがログインしているかチェック
            if (Session["AdminID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        // ユーザープロフィールを読み込む
        private void LoadUserProfile()
        {
            try
            {
                int adminID = Convert.ToInt32(Session["AdminID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT Username, Email, FullName, Role, CreatedDate, LastLogin 
                                   FROM AdminUser 
                                   WHERE AdminID = @AdminID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AdminID", adminID);

                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblFullName.Text = reader["FullName"].ToString();
                                lblUsername.Text = reader["Username"].ToString();
                                lblEmail.Text = reader["Email"].ToString();

                                // ロールを日本語に変換
                                string role = reader["Role"].ToString();
                                lblRole.Text = ConvertRoleToJapanese(role);

                                DateTime createdDate = Convert.ToDateTime(reader["CreatedDate"]);
                                lblCreatedDate.Text = createdDate.ToString("yyyy年MM月dd日");

                                if (reader["LastLogin"] != DBNull.Value)
                                {
                                    DateTime lastLogin = Convert.ToDateTime(reader["LastLogin"]);
                                    lblLastLogin.Text = lastLogin.ToString("yyyy年MM月dd日 HH:mm");
                                }
                                else
                                {
                                    lblLastLogin.Text = "なし";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("プロフィールの読み込みエラー: " + ex.Message);
                ShowError("プロフィール情報の読み込みに失敗しました。");
            }
        }

        // ログアウトボタンクリック
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // すべてのセッションデータをクリア
            Session.Clear();
            Session.Abandon();

            // ログインページにリダイレクト
            Response.Redirect("Login.aspx");
        }

        // アカウント削除ボタンクリック
        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            try
            {
                int adminID = Convert.ToInt32(Session["AdminID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // すべての削除が一緒に行われるようにトランザクションを開始
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // ユーザーの売上記録を削除（ある場合）
                            string deleteSalesQuery = "DELETE FROM Sales WHERE CreatedBy = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteSalesQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                cmd.ExecuteNonQuery();
                            }

                            // ユーザーの予約記録を削除（ある場合）
                            string deleteBookingsQuery = "DELETE FROM Bookings WHERE CreatedBy = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteBookingsQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                cmd.ExecuteNonQuery();
                            }

                            // ユーザーアカウントを削除
                            string deleteUserQuery = "DELETE FROM AdminUser WHERE AdminID = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteUserQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // トランザクションをコミット
                                    transaction.Commit();

                                    // セッションをクリア
                                    Session.Clear();
                                    Session.Abandon();

                                    // 成功メッセージと共にログインにリダイレクト
                                    Response.Redirect("Login.aspx?deleted=1");
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowError("アカウントの削除に失敗しました。もう一度お試しください。");
                                }
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("アカウント削除エラー: " + ex.Message);
                ShowError("アカウントの削除中にエラーが発生しました。もう一度お試しください。");
            }
        }

        // ロールを日本語に変換するヘルパーメソッド
        private string ConvertRoleToJapanese(string role)
        {
            switch (role?.ToLower())
            {
                case "administrator":
                case "admin":
                    return "管理者";
                case "manager":
                    return "マネージャー";
                case "receptionist":
                    return "受付係";
                case "staff":
                    return "スタッフ";
                default:
                    return role;
            }
        }

        // エラーメッセージを表示
        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblError.Text = message;
        }

        // 成功メッセージを表示
        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lblSuccess.Text = message;
        }
    }
}