using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace HotelManagement
{
    public partial class GuestList : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

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
                LoadStatistics();
            }
        }

        private void LoadGuests(string searchTerm = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                        g.GuestID,
                                        g.FirstName + ' ' + g.LastName AS GuestName,
                                        g.Email,
                                        g.Phone,
                                        g.IDNumber,
                                        g.CreatedDate,
                                        COUNT(b.BookingID) AS TotalBookings
                                    FROM Guests g
                                    LEFT JOIN Bookings b ON g.GuestID = b.GuestID
                                    WHERE (@SearchTerm = '' OR 
                                           g.FirstName LIKE '%' + @SearchTerm + '%' OR 
                                           g.LastName LIKE '%' + @SearchTerm + '%' OR 
                                           g.Email LIKE '%' + @SearchTerm + '%' OR 
                                           g.Phone LIKE '%' + @SearchTerm + '%')
                                    GROUP BY g.GuestID, g.FirstName, g.LastName, g.Email, g.Phone, g.IDNumber, g.CreatedDate
                                    ORDER BY g.FirstName, g.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gvGuests.DataSource = dt;
                            gvGuests.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading guests: " + ex.Message);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Guests";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        int totalGuests = (int)cmd.ExecuteScalar();
                        lblTotalGuests.Text = totalGuests.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading statistics: " + ex.Message);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadGuests(txtSearch.Text.Trim());
        }
    }
}