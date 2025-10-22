using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace HotelManagement
{
    public partial class Booking : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGuests();
                LoadRooms();
                // Set today's date as default check-in
                txtCheckIn.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtCheckOut.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }
        }

        protected void btnSelectNewGuest_Click(object sender, EventArgs e)
        {
            // Show new guest form and booking details
            pnlNewGuestForm.CssClass = "form-card";
            pnlExistingGuestForm.CssClass = "form-card hidden";
            pnlBookingDetails.CssClass = "form-card";

            // Set active state
            pnlNewGuestCard.CssClass = "option-card active";
            pnlExistingGuestCard.CssClass = "option-card";

            // Set the validation group for the create booking button
            btnCreateBooking.ValidationGroup = "NewGuestGroup";
        }

        protected void btnSelectExistingGuest_Click(object sender, EventArgs e)
        {
            // Show existing guest form and booking details
            pnlNewGuestForm.CssClass = "form-card hidden";
            pnlExistingGuestForm.CssClass = "form-card";
            pnlBookingDetails.CssClass = "form-card";

            // Set active state
            pnlNewGuestCard.CssClass = "option-card";
            pnlExistingGuestCard.CssClass = "option-card active";

            // Set the validation group for the create booking button
            btnCreateBooking.ValidationGroup = "ExistingGuestGroup";
        }

        private void LoadGuests()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT GuestID, FirstName + ' ' + LastName AS FullName 
            FROM Guests 
            WHERE IsActive = 1 OR IsActive IS NULL
            ORDER BY FirstName, LastName", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlGuest.DataSource = dt;
                ddlGuest.DataTextField = "FullName";
                ddlGuest.DataValueField = "GuestID";
                ddlGuest.DataBind();

                ddlGuest.Items.Insert(0, new ListItem("-- Select Guest --", "0"));
            }
            catch (Exception ex)
            {
                ShowError("Error loading guests: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadRooms()
        {
            try
            {
                con.Open();
                // Load available rooms with floor information
                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        r.RoomID, 
                        r.RoomNumber, 
                        r.Floor,
                        rt.TypeName, 
                        rt.BasePrice, 
                        rt.Capacity,
                        r.Status
                    FROM Rooms r
                    INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
                    WHERE r.Status = 'Available' OR r.Status IS NULL
                    ORDER BY r.Floor, r.RoomNumber", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string roomNum = row["RoomNumber"].ToString();
                    string floor = row["Floor"] == DBNull.Value ? "" : "F" + row["Floor"].ToString() + " - ";
                    string roomType = row["TypeName"].ToString();
                    decimal price = Convert.ToDecimal(row["BasePrice"]);
                    int capacity = Convert.ToInt32(row["Capacity"]);

                    // Format: F2 - 201 - Double (¥8,000/night, 2 guests)
                    row["RoomNumber"] = floor + roomNum + " - " + roomType +
                                       " (¥" + price.ToString("N0") + "/night, " +
                                       capacity + " guest" + (capacity > 1 ? "s" : "") + ")";
                }

                ddlRoom.DataSource = dt;
                ddlRoom.DataTextField = "RoomNumber";
                ddlRoom.DataValueField = "RoomID";
                ddlRoom.DataBind();

                ddlRoom.Items.Insert(0, new ListItem("-- Select Room --", "0"));

                // Show message if no rooms available
                if (dt.Rows.Count == 0)
                {
                    ShowError("No available rooms at the moment. All rooms are occupied or reserved.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading rooms: " + ex.Message);
            }
            finally
            {
                con.Close();
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
                if (ddlRoom.SelectedValue == "0" || string.IsNullOrEmpty(txtCheckIn.Text) || string.IsNullOrEmpty(txtCheckOut.Text))
                    return;

                DateTime checkIn = DateTime.Parse(txtCheckIn.Text);
                DateTime checkOut = DateTime.Parse(txtCheckOut.Text);

                if (checkOut <= checkIn)
                {
                    ShowError("Check-out date must be after check-in date.");
                    lblTotalAmount.Text = "0";
                    return;
                }

                int nights = (checkOut - checkIn).Days;

                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT rt.BasePrice 
                    FROM Rooms r 
                    INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID 
                    WHERE r.RoomID = @RoomID", con);
                cmd.Parameters.AddWithValue("@RoomID", ddlRoom.SelectedValue);

                object result = cmd.ExecuteScalar();
                decimal pricePerNight = 0;

                if (result != null)
                    pricePerNight = Convert.ToDecimal(result);

                con.Close();

                decimal total = pricePerNight * nights;
                lblTotalAmount.Text = total.ToString("0.00");
                pnlError.Visible = false;
            }
            catch (Exception ex)
            {
                ShowError("Error calculating total: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void btnCreateBooking_Click(object sender, EventArgs e)
        {
            try
            {
                int guestId = 0;

                // Determine if this is a new guest or existing guest
                if (btnCreateBooking.ValidationGroup == "NewGuestGroup")
                {
                    // Validate new guest fields
                    if (!Page.IsValid)
                        return;

                    // Create new guest first
                    guestId = CreateNewGuest();
                    if (guestId == 0)
                    {
                        ShowError("Failed to create guest. Please try again.");
                        return;
                    }
                }
                else if (btnCreateBooking.ValidationGroup == "ExistingGuestGroup")
                {
                    // Validate existing guest selection
                    if (!Page.IsValid)
                        return;

                    guestId = int.Parse(ddlGuest.SelectedValue);
                }
                else
                {
                    ShowError("Please select New Guest or Existing Guest option first.");
                    return;
                }

                // Now create the booking
                int roomId = int.Parse(ddlRoom.SelectedValue);
                DateTime checkIn = DateTime.Parse(txtCheckIn.Text);
                DateTime checkOut = DateTime.Parse(txtCheckOut.Text);
                int numberOfGuests = string.IsNullOrEmpty(txtNumberOfGuests.Text) ? 1 : int.Parse(txtNumberOfGuests.Text);
                string specialRequests = txtSpecialRequests.Text.Trim();
                decimal totalAmount = decimal.Parse(lblTotalAmount.Text);

                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Bookings (GuestID, RoomID, CheckInDate, CheckOutDate, BookingDate, Status, TotalAmount, AmountPaid, NumberOfGuests, SpecialRequests)
                    VALUES (@GuestID, @RoomID, @CheckInDate, @CheckOutDate, GETDATE(), 'Confirmed', @TotalAmount, 0, @NumberOfGuests, @SpecialRequests);
                    
                    UPDATE Rooms SET Status = 'Reserved' WHERE RoomID = @RoomID;", con);

                cmd.Parameters.AddWithValue("@GuestID", guestId);
                cmd.Parameters.AddWithValue("@RoomID", roomId);
                cmd.Parameters.AddWithValue("@CheckInDate", checkIn);
                cmd.Parameters.AddWithValue("@CheckOutDate", checkOut);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@NumberOfGuests", numberOfGuests);
                cmd.Parameters.AddWithValue("@SpecialRequests", string.IsNullOrEmpty(specialRequests) ? (object)DBNull.Value : specialRequests);

                cmd.ExecuteNonQuery();
                con.Close();

                ShowSuccess("Booking completed successfully! Total amount: $" + totalAmount.ToString("0.00"));
                ClearForm();
                LoadRooms();
                LoadGuests();
            }
            catch (Exception ex)
            {
                ShowError("Error creating booking: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private int CreateNewGuest()
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Guests (FirstName, LastName, Email, Phone, Address, IDNumber, DateOfBirth, CreatedDate)
                    VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @IDNumber, @DateOfBirth, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", con);

                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@IDNumber", string.IsNullOrEmpty(txtIDNumber.Text) ? (object)DBNull.Value : txtIDNumber.Text.Trim());

                if (string.IsNullOrEmpty(txtDateOfBirth.Text))
                    cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(txtDateOfBirth.Text));

                int newGuestId = (int)cmd.ExecuteScalar();
                con.Close();

                return newGuestId;
            }
            catch (Exception ex)
            {
                ShowError("Error creating guest: " + ex.Message);
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        private void ClearForm()
        {
            // Clear new guest form
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtIDNumber.Text = "";
            txtDateOfBirth.Text = "";

            // Clear existing guest selection
            ddlGuest.SelectedIndex = 0;

            // Clear booking details
            ddlRoom.SelectedIndex = 0;
            txtCheckIn.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtCheckOut.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            txtNumberOfGuests.Text = "1";
            txtSpecialRequests.Text = "";
            lblTotalAmount.Text = "0";

            // Hide all forms
            pnlNewGuestForm.CssClass = "form-card hidden";
            pnlExistingGuestForm.CssClass = "form-card hidden";
            pnlBookingDetails.CssClass = "form-card hidden";

            // Reset option cards
            pnlNewGuestCard.CssClass = "option-card";
            pnlExistingGuestCard.CssClass = "option-card";
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