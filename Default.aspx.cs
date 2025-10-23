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
            // IMPORTANT: Check if user is logged in
            if (Session["AdminID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Set username from session
                if (Session["AdminName"] != null)
                {
                    lblUsername.Text = Session["AdminName"].ToString();
                }

                LoadDashboardData();
                CheckOverdueCheckouts(); // NEW: Check for overdue checkouts
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get Dashboard Statistics
                    using (SqlCommand cmd = new SqlCommand("sp_GetDashboardStats", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Result Set 1: Today's Bookings
                            if (reader.Read())
                            {
                                lblBookings.Text = reader["TodayBookings"].ToString();
                            }

                            // Result Set 2: Today's Check-ins
                            if (reader.NextResult() && reader.Read())
                            {
                                lblCheckIns.Text = reader["TodayCheckIns"].ToString();
                            }

                            // Result Set 3: Room Statistics
                            if (reader.NextResult() && reader.Read())
                            {
                                lblAvailableRooms.Text = reader["AvailableRooms"].ToString();
                                lblOccupiedRooms.Text = reader["OccupiedRooms"].ToString();
                                lblReservedRooms.Text = reader["ReservedRooms"].ToString();
                            }

                            // Result Set 4: Monthly Sales
                            if (reader.NextResult() && reader.Read())
                            {
                                decimal revenue = Convert.ToDecimal(reader["MonthlyRevenue"]);
                                lblMonthlyRevenue.Text = revenue.ToString("N0");
                                lblTransactionCount.Text = reader["TotalTransactions"].ToString();
                            }
                        }
                    }

                    // Get Current Guests List
                    LoadCurrentGuests(con);
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading dashboard: " + ex.Message);
                // Optionally show user-friendly error message
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
                System.Diagnostics.Debug.WriteLine("Error loading current guests: " + ex.Message);
            }
        }

        // NEW: Check for overdue checkouts (past 12:00 PM today)
        private void CheckOverdueCheckouts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get count of bookings where:
                    // 1. Status is 'CheckedIn' (currently occupying room)
                    // 2. CheckOutDate is today or earlier
                    // 3. Current time is past the checkout time
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
                System.Diagnostics.Debug.WriteLine("Error checking overdue checkouts: " + ex.Message);
                pnlOverdueWarning.Visible = false;
            }
        }

        protected void btnViewSales_Click(object sender, EventArgs e)
        {
            Response.Redirect("SalesReport.aspx");
        }

        // NEW: Handler for navigating to Rooms page
        protected void btnViewRooms_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rooms.aspx");
        }

        // NEW: Handlers for navigating to Rooms page with status filter
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