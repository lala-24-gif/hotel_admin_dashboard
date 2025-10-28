using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace HotelManagement
{
    // BookingsList.aspx のコードビハインド（ホテル予約を管理）
    public partial class BookingsList : System.Web.UI.Page
    {
        // データベース接続文字列
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString);

        // ページ読み込み時の処理
        protected void Page_Load(object sender, EventArgs e)
        {
            // 初回ページ読み込み時に予約と統計を読み込む（ポストバックでない場合）
            if (!IsPostBack)
            {
                LoadBookings();
                LoadStatistics();
            }
        }

        // フィルターを適用してデータを再読み込み
        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadBookings(); // 選択されたフィルターに基づいて予約を再読み込み
            LoadStatistics(); // 選択されたフィルターに基づいて統計を再読み込み
        }

        // 適用されたフィルターでGridViewに予約を読み込む
        private void LoadBookings()
        {
            try
            {
                // データベース接続を開く
                con.Open();

                string statusFilter = ddlStatusFilter.SelectedValue; // 選択されたステータスフィルターを取得
                string dateFilter = ddlDateFilter.SelectedValue; // 選択された日付フィルターを取得

                string query = @"
                    SELECT 
                        b.BookingID,
                        g.FirstName + ' ' + g.LastName AS GuestName,
                        r.RoomNumber,
                        b.CheckInDate,
                        b.CheckOutDate,
                        b.NumberOfGuests,
                        b.TotalAmount,
                        b.Status
                    FROM Bookings b
                    INNER JOIN Guests g ON b.GuestID = g.GuestID
                    INNER JOIN Rooms r ON b.RoomID = r.RoomID
                    WHERE 1=1"; // フィルターしないが、動的に条件を追加しやすくする

                // ステータスフィルターを適用
                if (statusFilter != "All")
                {
                    query += " AND b.Status = @Status"; // @Status はパラメータプレースホルダー（SQLインジェクションを防ぐ）
                }

                // 日付フィルターを適用
                switch (dateFilter)
                {
                    case "Today":
                        query += " AND CAST(b.CheckInDate AS DATE) = CAST(GETDATE() AS DATE)";
                        break;
                    case "Week":
                        // 現在の週（月曜日から日曜日）の予約を取得
                        query += @" AND b.CheckInDate >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
                                 AND b.CheckInDate < DATEADD(DAY, 8 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))";
                        break;
                    case "Month":
                        query += " AND MONTH(b.CheckInDate) = MONTH(GETDATE()) AND YEAR(b.CheckInDate) = YEAR(GETDATE())";
                        break;
                    case "Future":
                        query += " AND b.CheckInDate > CAST(GETDATE() AS DATE)";
                        break;
                }

                query += " ORDER BY b.CheckInDate DESC, b.BookingID DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                if (statusFilter != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", statusFilter);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBookings.DataSource = dt;
                gvBookings.DataBind();

                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("予約の読み込みエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // 統計情報を読み込む
        private void LoadStatistics()
        {
            try
            {
                con.Open();

                string statusFilter = ddlStatusFilter.SelectedValue;
                string dateFilter = ddlDateFilter.SelectedValue;

                string query = @"
                    SELECT 
                        COUNT(*) AS TotalBookings,
                        SUM(CASE WHEN Status = 'Confirmed' THEN 1 ELSE 0 END) AS Confirmed,
                        SUM(CASE WHEN Status = 'CheckedIn' THEN 1 ELSE 0 END) AS CheckedIn,
                        SUM(CASE WHEN Status = 'CheckedOut' THEN 1 ELSE 0 END) AS CheckedOut
                       
                    FROM Bookings
                    WHERE 1=1"; // 条件を簡単に追加できる

                // メイングリッドと同じフィルターを適用
                if (statusFilter != "All")
                {
                    query += " AND Status = @Status";
                }

                switch (dateFilter)
                {
                    case "Today":
                        query += " AND CAST(CheckInDate AS DATE) = CAST(GETDATE() AS DATE)";
                        break;
                    case "Week":
                        query += " AND CheckInDate >= CAST(GETDATE() AS DATE) AND CheckInDate < DATEADD(DAY, 7, CAST(GETDATE() AS DATE))";
                        break;
                    case "Month":
                        query += " AND MONTH(CheckInDate) = MONTH(GETDATE()) AND YEAR(CheckInDate) = YEAR(GETDATE())";
                        break;
                    case "Future":
                        query += " AND CheckInDate > CAST(GETDATE() AS DATE)";
                        break;
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (statusFilter != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", statusFilter);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTotalBookings.Text = reader["TotalBookings"].ToString();
                    lblConfirmed.Text = reader["Confirmed"].ToString();
                    lblCheckedIn.Text = reader["CheckedIn"].ToString();
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("統計の読み込みエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // GridViewの行コマンドを処理
        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int bookingId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "CheckOut")
            {
                CheckOutGuest(bookingId);
            }
            else if (e.CommandName == "CancelBooking")
            {
                CancelBooking(bookingId);
            }
        }

        // ゲストをチェックアウト
        private void CheckOutGuest(int bookingId)
        {
            try
            {
                con.Open();

                // 客室IDを取得
                SqlCommand getRoomCmd = new SqlCommand("SELECT RoomID FROM Bookings WHERE BookingID = @BookingID", con);
                getRoomCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int roomId = Convert.ToInt32(getRoomCmd.ExecuteScalar());

                // 予約と客室を更新
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Bookings 
                    SET Status = 'CheckedOut'
                    WHERE BookingID = @BookingID;
                    
                    UPDATE Rooms 
                    SET Status = 'Available' 
                    WHERE RoomID = @RoomID;", con);

                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);

                cmd.ExecuteNonQuery();
                con.Close();

                ShowSuccess("ゲストのチェックアウトが完了しました！客室は現在利用可能です。");
                LoadBookings();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError("チェックアウトエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // 予約をキャンセル
        private void CancelBooking(int bookingId)
        {
            try
            {
                con.Open();

                // 客室IDを取得
                SqlCommand getRoomCmd = new SqlCommand("SELECT RoomID FROM Bookings WHERE BookingID = @BookingID", con);
                getRoomCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int roomId = Convert.ToInt32(getRoomCmd.ExecuteScalar());

                // 予約をキャンセルして客室を解放
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Bookings 
                    SET Status = 'Cancelled'
                    WHERE BookingID = @BookingID;
                    
                    UPDATE Rooms 
                    SET Status = 'Available' 
                    WHERE RoomID = @RoomID;", con);

                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);

                cmd.ExecuteNonQuery();
                con.Close();

                ShowSuccess("予約がキャンセルされました！客室は現在利用可能です。");
                LoadBookings();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError("予約キャンセルエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ステータスを日本語に変換するヘルパーメソッド
        protected string GetStatusText(string status)
        {
            switch (status)
            {
                case "Confirmed":
                    return "確認済み";
                case "CheckedIn":
                    return "チェックイン済み";
                case "CheckedOut":
                    return "チェックアウト済み";
                case "Cancelled":
                    return "キャンセル済み";
                default:
                    return status;
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