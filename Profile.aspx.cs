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
            // Check if user is logged in
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
                                lblRole.Text = reader["Role"].ToString();

                                DateTime createdDate = Convert.ToDateTime(reader["CreatedDate"]);
                                lblCreatedDate.Text = createdDate.ToString("MMMM dd, yyyy");

                                if (reader["LastLogin"] != DBNull.Value)
                                {
                                    DateTime lastLogin = Convert.ToDateTime(reader["LastLogin"]);
                                    lblLastLogin.Text = lastLogin.ToString("MMMM dd, yyyy hh:mm tt");
                                }
                                else
                                {
                                    lblLastLogin.Text = "Never";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
                ShowError("Failed to load profile information.");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear all session data
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("Login.aspx");
        }

        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            try
            {
                int adminID = Convert.ToInt32(Session["AdminID"]);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Start transaction to ensure all deletions happen together
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Delete user's sales records (if any)
                            string deleteSalesQuery = "DELETE FROM Sales WHERE CreatedBy = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteSalesQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete user's booking records (if any)
                            string deleteBookingsQuery = "DELETE FROM Bookings WHERE CreatedBy = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteBookingsQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete the user account
                            string deleteUserQuery = "DELETE FROM AdminUser WHERE AdminID = @AdminID";
                            using (SqlCommand cmd = new SqlCommand(deleteUserQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AdminID", adminID);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // Commit the transaction
                                    transaction.Commit();

                                    // Clear session
                                    Session.Clear();
                                    Session.Abandon();

                                    // Redirect to login with success message
                                    Response.Redirect("Login.aspx?deleted=1");
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowError("Failed to delete account. Please try again.");
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
                System.Diagnostics.Debug.WriteLine("Error deleting account: " + ex.Message);
                ShowError("An error occurred while deleting your account. Please try again.");
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