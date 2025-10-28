using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HotelManagement
{
    public partial class Default : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 重要: ユーザーがログインしているか確認
            if (Session["AdminID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // セッションからユーザー名を設定
                if (Session["AdminName"] != null)
                {
                    lblUsername.Text = Session["AdminName"].ToString();
                }

                LoadDashboardData();
                CheckOverdueCheckouts(); // 遅延チェックアウトを確認
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ダッシュボード統計を取得
                    using (SqlCommand cmd = new SqlCommand("sp_GetDashboardStats", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // 結果セット1: 本日の予約
                            if (reader.Read())
                            {
                                lblBookings.Text = reader["TodayBookings"].ToString();
                            }

                            // 結果セット2: 本日のチェックイン
                            if (reader.NextResult() && reader.Read())
                            {
                                lblCheckIns.Text = reader["TodayCheckIns"].ToString();
                            }

                            // 結果セット3: 客室統計
                            if (reader.NextResult() && reader.Read())
                            {
                                lblAvailableRooms.Text = reader["AvailableRooms"].ToString();
                                lblOccupiedRooms.Text = reader["OccupiedRooms"].ToString();
                                lblReservedRooms.Text = reader["ReservedRooms"].ToString();
                            }

                            // 結果セット4: 月間売上
                            if (reader.NextResult() && reader.Read())
                            {
                                decimal revenue = Convert.ToDecimal(reader["MonthlyRevenue"]);
                                lblMonthlyRevenue.Text = revenue.ToString("N0");
                                lblTransactionCount.Text = reader["TotalTransactions"].ToString();
                            }
                        }
                    }

                    // 現在のゲストリストを取得
                    LoadCurrentGuests(con);
                }
            }
            catch (Exception ex)
            {
                // エラーをログに記録
                System.Diagnostics.Debug.WriteLine("ダッシュボードの読み込みエラー: " + ex.Message);
                // オプション: ユーザーフレンドリーなエラーメッセージを表示
            }
        }

        private void LoadCurrentGuests(SqlConnection con)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetCurrentGuests", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvCurrentGuests.DataSource = dt;
                        gvCurrentGuests.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("現在のゲストの読み込みエラー: " + ex.Message);
            }
        }

        // 遅延チェックアウトを確認（本日午後12時を過ぎた場合）
        private void CheckOverdueCheckouts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // 以下の条件に該当する予約の数を取得:
                    // 1. ステータスが 'CheckedIn'（現在客室を使用中）
                    // 2. チェックアウト日が本日以前
                    // 3. 現在時刻がチェックアウト時刻を過ぎている
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Bookings 
                        WHERE Status = 'CheckedIn' 
                        AND CheckOutDate <= GETDATE()";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int overdueCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (overdueCount > 0)
                        {
                            pnlOverdueWarning.Visible = true;
                            lblOverdueCount.Text = overdueCount.ToString();
                        }
                        else
                        {
                            pnlOverdueWarning.Visible = false;
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("遅延チェックアウトの確認エラー: " + ex.Message);
                pnlOverdueWarning.Visible = false;
            }
        }

        protected void btnViewSales_Click(object sender, EventArgs e)
        {
            Response.Redirect("SalesReport.aspx");
        }

        // 客室ページへのナビゲーションハンドラー
        protected void btnViewRooms_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rooms.aspx");
        }

        // ステータスフィルター付きで客室ページへナビゲートするハンドラー
        protected void btnViewAvailableRooms_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rooms.aspx?status=available");
        }

        protected void btnViewOccupiedRooms_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rooms.aspx?status=occupied");
        }

        protected void btnViewReservedRooms_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rooms.aspx?status=reserved");
        }
    }
}