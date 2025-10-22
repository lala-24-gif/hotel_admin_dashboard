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
            WHERE g.IsActive = 1"; // Only show active guests

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
                ShowError("Error loading guests: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

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
                ShowError("Error loading guest count: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadGuests();
        }

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

                    // Show the edit modal
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
                        "showEditModal();", true);
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                ShowError("Error loading guest details: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

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

                ShowSuccess("Guest information updated successfully!");
                LoadGuests();

                // Hide the modal
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "HideEditModal",
                    "hideEditModal();", true);
            }
            catch (Exception ex)
            {
                ShowError("Error updating guest: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void DeleteGuest(int guestId)
        {
            try
            {
                con.Open();

                // Check if guest has any ACTIVE bookings (not checked out or cancelled)
                SqlCommand checkCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Bookings 
            WHERE GuestID = @GuestID 
                AND Status IN ('Confirmed', 'CheckedIn')", con);
                checkCmd.Parameters.AddWithValue("@GuestID", guestId);

                int activeBookings = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (activeBookings > 0)
                {
                    ShowError("Cannot delete guest with active or confirmed bookings. Please check out or cancel the active bookings first.");
                    con.Close();
                    return;
                }

                // Get booking count for confirmation message
                SqlCommand countCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Bookings 
            WHERE GuestID = @GuestID", con);
                countCmd.Parameters.AddWithValue("@GuestID", guestId);
                int totalBookings = Convert.ToInt32(countCmd.ExecuteScalar());

                // Option 1: Soft delete by anonymizing the guest data (RECOMMENDED)
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
                    ShowSuccess($"Guest information has been anonymized. Their {totalBookings} past booking record(s) are preserved for accounting purposes.");
                }
                else
                {
                    ShowSuccess("Guest deleted successfully!");
                }

                LoadGuests();
                LoadTotalGuests();
            }
            catch (Exception ex)
            {
                ShowError("Error deleting guest: " + ex.Message);
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