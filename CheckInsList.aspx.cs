using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace HotelManagement
{
    public partial class CheckInsList : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCheckIns();
                LoadStatistics();
            }
        }

        private void LoadCheckIns()
        {
            try
            {
                con.Open();

                // Get all bookings scheduled for today's check-in (exclude CheckedOut and Cancelled)
                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        b.BookingID,
                        b.GuestID,
                        g.FirstName + ' ' + g.LastName AS GuestName,
                        g.Phone,
                        r.RoomNumber,
                        rt.TypeName AS RoomType,
                        '12:00' AS ExpectedTime,
                        b.CheckOutDate,
                        b.NumberOfGuests,
                        b.TotalAmount,
                        b.Status
                    FROM Bookings b
                    INNER JOIN Guests g ON b.GuestID = g.GuestID
                    INNER JOIN Rooms r ON b.RoomID = r.RoomID
                    INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
                    WHERE CAST(b.CheckInDate AS DATE) = CAST(GETDATE() AS DATE)
                        AND b.Status NOT IN ('CheckedOut', 'Cancelled')
                    ORDER BY b.CheckInDate, r.RoomNumber", con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCheckIns.DataSource = dt;
                gvCheckIns.DataBind();

                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("Error loading check-ins: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void LoadStatistics()
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        COUNT(*) AS TotalExpected,
                        SUM(CASE WHEN Status = 'CheckedIn' THEN 1 ELSE 0 END) AS CheckedIn,
                        SUM(CASE WHEN Status NOT IN ('CheckedIn', 'CheckedOut', 'Cancelled') THEN 1 ELSE 0 END) AS Pending
                    FROM Bookings
                    WHERE CAST(CheckInDate AS DATE) = CAST(GETDATE() AS DATE)
                        AND Status NOT IN ('CheckedOut', 'Cancelled')", con);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTotalExpected.Text = reader["TotalExpected"].ToString();
                    lblAlreadyCheckedIn.Text = reader["CheckedIn"].ToString();
                    lblPending.Text = reader["Pending"].ToString();
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("Error loading statistics: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void gvCheckIns_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int bookingId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "CheckInGuest")
            {
                CheckInGuest(bookingId);
            }
            else if (e.CommandName == "CheckOutGuest")
            {
                CheckOutGuest(bookingId);
            }
            else if (e.CommandName == "CancelBooking")
            {
                CancelBooking(bookingId);
            }
        }

        private void CheckInGuest(int bookingId)
        {
            try
            {
                con.Open();

                // Get room ID first
                SqlCommand getRoomCmd = new SqlCommand("SELECT RoomID FROM Bookings WHERE BookingID = @BookingID", con);
                getRoomCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int roomId = Convert.ToInt32(getRoomCmd.ExecuteScalar());

                // Update booking status to CheckedIn
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Bookings 
                    SET Status = 'CheckedIn'
                    WHERE BookingID = @BookingID;
                    
                    UPDATE Rooms 
                    SET Status = 'Occupied' 
                    WHERE RoomID = @RoomID;", con);

                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);

                cmd.ExecuteNonQuery();
                con.Close();

                ShowSuccess("Guest checked in successfully!");
                LoadCheckIns();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError("Error checking in guest: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void CheckOutGuest(int bookingId)
        {
            try
            {
                con.Open();

                // Get room ID first
                SqlCommand getRoomCmd = new SqlCommand("SELECT RoomID FROM Bookings WHERE BookingID = @BookingID", con);
                getRoomCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int roomId = Convert.ToInt32(getRoomCmd.ExecuteScalar());

                // Update booking status to CheckedOut and room status to Available
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

                ShowSuccess("Guest checked out successfully!");
                LoadCheckIns();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError("Error checking out guest: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void CancelBooking(int bookingId)
        {
            try
            {
                con.Open();

                // Get room ID first
                SqlCommand getRoomCmd = new SqlCommand("SELECT RoomID FROM Bookings WHERE BookingID = @BookingID", con);
                getRoomCmd.Parameters.AddWithValue("@BookingID", bookingId);
                int roomId = Convert.ToInt32(getRoomCmd.ExecuteScalar());

                // Update booking status to Cancelled and room status to Available
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

                ShowSuccess("Booking cancelled successfully!");
                LoadCheckIns();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowError("Error cancelling booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
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