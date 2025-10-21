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

        protected void btnViewSales_Click(object sender, EventArgs e)
        {
            Response.Redirect("SalesReport.aspx");
        }
    }
}