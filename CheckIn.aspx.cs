using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HotelManagement
{
    public partial class CheckIn : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Remove session check if you don't have authentication yet
            // if (Session["AdminID"] == null)
            // {
            //     Response.Redirect("Login.aspx");
            //     return;
            // }

            if (!IsPostBack)
            {
                LoadAvailableRooms();
                LoadGuests();
            }
        }

        protected void btnSelectNew_Click(object sender, EventArgs e)
        {
            pnlNewGuest.CssClass = "option-card selected";
            pnlExistingGuest.CssClass = "option-card";

            pnlNewGuestForm.Visible = true;
            pnlExistingGuestForm.Visible = false;
            pnlRoomSection.Visible = true;
        }

        protected void btnSelectExisting_Click(object sender, EventArgs e)
        {
            pnlExistingGuest.CssClass = "option-card selected";
            pnlNewGuest.CssClass = "option-card";

            pnlExistingGuestForm.Visible = true;
            pnlNewGuestForm.Visible = false;
            pnlRoomSection.Visible = true;
        }

        private void LoadGuests()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT GuestID, FirstName + ' ' + LastName AS GuestName 
                           FROM Guests 
                           WHERE IsActive = 1 OR IsActive IS NULL
                           ORDER BY FirstName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlGuest.Items.Clear();
                        ddlGuest.Items.Add(new ListItem("-- Select Guest --", "0"));

                        while (reader.Read())
                        {
                            ddlGuest.Items.Add(new ListItem(reader["GuestName"].ToString(), reader["GuestID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading guests: " + ex.Message);
            }
        }

        private void LoadAvailableRooms()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.RoomID, r.RoomNumber, r.Floor, rt.TypeName, rt.BasePrice, rt.Capacity
                                   FROM Rooms r
                                   INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
                                   WHERE r.Status = 'Available'
                                   ORDER BY r.Floor, r.RoomNumber";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlRoom.Items.Clear();
                        ddlRoom.Items.Add(new ListItem("-- Select Room --", "0"));

                        while (reader.Read())
                        {
                            string floor = reader["Floor"] == DBNull.Value ? "" : "F" + reader["Floor"].ToString() + " - ";
                            string roomNumber = reader["RoomNumber"].ToString();
                            string roomType = reader["TypeName"].ToString();
                            decimal price = Convert.ToDecimal(reader["BasePrice"]);
                            int capacity = Convert.ToInt32(reader["Capacity"]);

                            string roomText = $"{floor}{roomNumber} - {roomType} (¥{price:N0}/night, {capacity} guest{(capacity > 1 ? "s" : "")})";
                            ddlRoom.Items.Add(new ListItem(roomText, reader["RoomID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading rooms: " + ex.Message);
            }
        }

        protected void ddlRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTotal(sender, e);
        }

        protected void CalculateTotal(object sender, EventArgs e)
        {
            try
            {
                if (ddlRoom.SelectedValue != "0" && !string.IsNullOrEmpty(txtNights.Text))
                {
                    int nights = Convert.ToInt32(txtNights.Text);
                    decimal roomPrice = GetRoomPrice(Convert.ToInt32(ddlRoom.SelectedValue));
                    decimal totalAmount = roomPrice * nights;

                    lblTotalAmount.Text = totalAmount.ToString("N0");

                    // Calculate checkout date
                    DateTime checkOut = DateTime.Today.AddDays(nights);
                    txtCheckOut.Text = checkOut.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                ShowError("Error calculating total: " + ex.Message);
            }
        }

        private decimal GetRoomPrice(int roomID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT rt.BasePrice 
                                   FROM Rooms r
                                   INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
                                   WHERE r.RoomID = @RoomID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@RoomID", roomID);
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        protected void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int guestID = 0;

                    // Check if new guest or existing
                    if (pnlNewGuestForm.Visible)
                    {
                        // Create new guest first
                        guestID = CreateNewGuest();
                        if (guestID == 0)
                        {
                            ShowError("Failed to create guest. Please try again.");
                            return;
                        }
                    }
                    else
                    {
                        guestID = Convert.ToInt32(ddlGuest.SelectedValue);
                    }

                    // Create booking and check in
                    int roomID = Convert.ToInt32(ddlRoom.SelectedValue);
                    DateTime checkIn = DateTime.Today; // Use Today instead of Now for consistency
                    int nights = Convert.ToInt32(txtNights.Text);
                    DateTime checkOut = checkIn.AddDays(nights);
                    int numberOfGuests = Convert.ToInt32(txtNumberOfGuests.Text);
                    decimal totalAmount = decimal.Parse(lblTotalAmount.Text);

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        // Create booking with CheckedIn status
                        string bookingQuery = @"INSERT INTO Bookings 
                            (GuestID, RoomID, CheckInDate, CheckOutDate, BookingDate, Status, TotalAmount, AmountPaid, NumberOfGuests, SpecialRequests)
                            VALUES (@GuestID, @RoomID, @CheckIn, @CheckOut, GETDATE(), 'CheckedIn', @Amount, 0, @NumGuests, NULL)";

                        using (SqlCommand cmd = new SqlCommand(bookingQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@GuestID", guestID);
                            cmd.Parameters.AddWithValue("@RoomID", roomID);
                            cmd.Parameters.AddWithValue("@CheckIn", checkIn);
                            cmd.Parameters.AddWithValue("@CheckOut", checkOut);
                            cmd.Parameters.AddWithValue("@Amount", totalAmount);
                            cmd.Parameters.AddWithValue("@NumGuests", numberOfGuests);

                            cmd.ExecuteNonQuery();
                        }

                        // Update room status to Occupied
                        string updateRoomQuery = "UPDATE Rooms SET Status = 'Occupied' WHERE RoomID = @RoomID";
                        using (SqlCommand cmd = new SqlCommand(updateRoomQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@RoomID", roomID);
                            cmd.ExecuteNonQuery();
                        }

                        con.Close();

                        string guestName = pnlNewGuestForm.Visible ?
                            $"{txtFirstName.Text} {txtLastName.Text}" :
                            ddlGuest.SelectedItem.Text;

                        ShowSuccess($"Guest {guestName} has been checked in successfully! Room is now occupied.");
                        ClearForm();
                        LoadAvailableRooms();
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Error checking in: " + ex.Message);
                }
            }
        }

        private int CreateNewGuest()
        {
            try
            {
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string idNumber = txtIDNumber.Text.Trim();

                DateTime? dateOfBirth = null;
                if (!string.IsNullOrEmpty(txtDateOfBirth.Text))
                {
                    dateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text);
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Guests (FirstName, LastName, Email, Phone, IDNumber, DateOfBirth, CreatedDate) 
                                   OUTPUT INSERTED.GuestID
                                   VALUES (@FirstName, @LastName, @Email, @Phone, @IDNumber, @DateOfBirth, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone);
                        cmd.Parameters.AddWithValue("@IDNumber", string.IsNullOrEmpty(idNumber) ? (object)DBNull.Value : idNumber);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth.HasValue ? (object)dateOfBirth.Value : DBNull.Value);

                        con.Open();
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error creating guest: " + ex.Message);
                return 0;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        private void ClearForm()
        {
            // Clear new guest fields
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtIDNumber.Text = string.Empty;
            txtDateOfBirth.Text = string.Empty;

            // Clear existing guest
            ddlGuest.SelectedIndex = 0;

            // Clear room fields
            ddlRoom.SelectedIndex = 0;
            txtNights.Text = "1";
            txtNumberOfGuests.Text = "1";
            txtCheckOut.Text = string.Empty;
            lblTotalAmount.Text = "0";

            // Reset visibility
            pnlNewGuestForm.Visible = false;
            pnlExistingGuestForm.Visible = false;
            pnlRoomSection.Visible = false;
            pnlNewGuest.CssClass = "option-card";
            pnlExistingGuest.CssClass = "option-card";
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