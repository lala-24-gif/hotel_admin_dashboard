using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace HotelManagement
{
    public partial class GuestList : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadGuests();
                LoadTotalGuests();
            }
        }

        // ゲストを読み込む
        private void LoadGuests()
        {
            try
            {
                con.Open();

                string searchTerm = txtSearch.Text.Trim();
                string query = @"
            SELECT 
                g.GuestID,
                g.FirstName + ' ' + g.LastName AS GuestName,
                g.FirstName,
                g.LastName,
                g.Email,
                g.Phone,
                g.IDNumber,
                g.CreatedDate,
                COUNT(b.BookingID) AS TotalBookings
            FROM Guests g
            LEFT JOIN Bookings b ON g.GuestID = b.GuestID
            WHERE g.IsActive = 1"; // アクティブなゲストのみを表示

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += @" AND (g.FirstName LIKE @Search 
                        OR g.LastName LIKE @Search 
                        OR g.Email LIKE @Search 
                        OR g.Phone LIKE @Search)";
                }

                query += " GROUP BY g.GuestID, g.FirstName, g.LastName, g.Email, g.Phone, g.IDNumber, g.CreatedDate";
                query += " ORDER BY g.CreatedDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvGuests.DataSource = dt;
                gvGuests.DataBind();

                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("ゲストの読み込みエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ゲスト総数を読み込む
        private void LoadTotalGuests()
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Guests WHERE IsActive = 1", con);
                int totalGuests = Convert.ToInt32(cmd.ExecuteScalar());
                lblTotalGuests.Text = totalGuests.ToString();

                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("ゲスト数の読み込みエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // 検索テキストが変更されたときの処理
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadGuests();
        }

        // GridViewの行コマンドを処理
        protected void gvGuests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int guestId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditGuest")
            {
                LoadGuestForEdit(guestId);
            }
            else if (e.CommandName == "DeleteGuest")
            {
                DeleteGuest(guestId);
            }
        }

        // 編集用にゲスト情報を読み込む
        private void LoadGuestForEdit(int guestId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT GuestID, FirstName, LastName, Email, Phone, IDNumber 
                    FROM Guests 
                    WHERE GuestID = @GuestID", con);
                cmd.Parameters.AddWithValue("@GuestID", guestId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    hfEditGuestID.Value = reader["GuestID"].ToString();
                    txtEditFirstName.Text = reader["FirstName"].ToString();
                    txtEditLastName.Text = reader["LastName"].ToString();
                    txtEditEmail.Text = reader["Email"].ToString();
                    txtEditPhone.Text = reader["Phone"].ToString();
                    txtEditIDNumber.Text = reader["IDNumber"].ToString();

                    // 編集モーダルを表示
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        "showEditModal();", true);
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("ゲスト詳細の読み込みエラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // 編集を保存
        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int guestId = Convert.ToInt32(hfEditGuestID.Value);

                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Guests 
                    SET FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        Phone = @Phone,
                        IDNumber = @IDNumber
                    WHERE GuestID = @GuestID", con);

                cmd.Parameters.AddWithValue("@GuestID", guestId);
                cmd.Parameters.AddWithValue("@FirstName", txtEditFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastName", txtEditLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEditEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtEditPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@IDNumber", txtEditIDNumber.Text.Trim());

                cmd.ExecuteNonQuery();
                con.Close();

                ShowSuccess("ゲスト情報が正常に更新されました！");
                LoadGuests();

                // モーダルを非表示
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "HideEditModal",
                    "hideEditModal();", true);
            }
            catch (Exception ex)
            {
                ShowError("ゲスト更新エラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ゲストを削除
        private void DeleteGuest(int guestId)
        {
            try
            {
                con.Open();

                // アクティブな予約（チェックアウトまたはキャンセルされていない）があるかチェック
                SqlCommand checkCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Bookings 
            WHERE GuestID = @GuestID 
                AND Status IN ('Confirmed', 'CheckedIn')", con);
                checkCmd.Parameters.AddWithValue("@GuestID", guestId);

                int activeBookings = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (activeBookings > 0)
                {
                    ShowError("アクティブまたは確認済みの予約があるゲストは削除できません。まずアクティブな予約をチェックアウトまたはキャンセルしてください。");
                    con.Close();
                    return;
                }

                // 確認メッセージ用に予約数を取得
                SqlCommand countCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Bookings 
            WHERE GuestID = @GuestID", con);
                countCmd.Parameters.AddWithValue("@GuestID", guestId);
                int totalBookings = Convert.ToInt32(countCmd.ExecuteScalar());

                // オプション1: ゲストデータを匿名化してソフト削除（推奨）
                SqlCommand anonymizeCmd = new SqlCommand(@"
            UPDATE Guests 
            SET FirstName = 'Deleted',
                LastName = 'Guest',
                Email = 'deleted_' + CAST(@GuestID AS VARCHAR) + '@removed.com',
                Phone = 'N/A',
                IDNumber = 'DELETED',
                IsActive = 0
            WHERE GuestID = @GuestID", con);
                anonymizeCmd.Parameters.AddWithValue("@GuestID", guestId);
                anonymizeCmd.ExecuteNonQuery();

                con.Close();

                if (totalBookings > 0)
                {
                    ShowSuccess($"ゲスト情報は匿名化されました。会計目的のために{totalBookings}件の過去の予約記録は保存されています。");
                }
                else
                {
                    ShowSuccess("ゲストが正常に削除されました！");
                }

                LoadGuests();
                LoadTotalGuests();
            }
            catch (Exception ex)
            {
                ShowError("ゲスト削除エラー: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
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