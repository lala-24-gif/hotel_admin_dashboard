using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Text;
using Newtonsoft.Json;

namespace HotelManagement
{
    public partial class SalesReport : System.Web.UI.Page
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
                lblCurrentYear.Text = DateTime.Now.Year.ToString();
                LoadSalesReport();
            }
        }

       
        private void LoadSalesReport()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                
                    LoadAnnualSummary(con);

                   
                    LoadMonthlyBreakdown(con);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("売上レポートの読み込みエラー: " + ex.Message);
            }
        }

       
        private void LoadAnnualSummary(SqlConnection con)
        {
            string query = @"
                SELECT 
                    ISNULL(SUM(TotalAmount), 0) AS TotalRevenue,
                    COUNT(*) AS TotalTransactions,
                    ISNULL(AVG(TotalAmount), 0) AS AvgTransaction
                FROM Bookings
                WHERE YEAR(CheckOutDate) = @Year
                    AND Status = 'CheckedOut'";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal totalRevenue = Convert.ToDecimal(reader["TotalRevenue"]);
                        lblTotalRevenue.Text = totalRevenue.ToString("N0");
                        lblTotalTransactions.Text = reader["TotalTransactions"].ToString();

                        decimal avgTransaction = Convert.ToDecimal(reader["AvgTransaction"]);
                        lblAverageTransaction.Text = avgTransaction.ToString("N0");
                    }
                }
            }

            CalculateGrowth(con);

         
            FindBestMonth(con);
        }

      
        private void CalculateGrowth(SqlConnection con)
        {
            string query = @"
                SELECT 
                    ISNULL(SUM(CASE WHEN YEAR(CheckOutDate) = @CurrentYear THEN TotalAmount ELSE 0 END), 0) AS CurrentYearRevenue,
                    ISNULL(SUM(CASE WHEN YEAR(CheckOutDate) = @LastYear THEN TotalAmount ELSE 0 END), 0) AS LastYearRevenue,
                    ISNULL(COUNT(CASE WHEN YEAR(CheckOutDate) = @CurrentYear THEN 1 END), 0) AS CurrentYearTransactions,
                    ISNULL(COUNT(CASE WHEN YEAR(CheckOutDate) = @LastYear THEN 1 END), 0) AS LastYearTransactions
                FROM Bookings
                WHERE Status = 'CheckedOut'";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@CurrentYear", DateTime.Now.Year);
                cmd.Parameters.AddWithValue("@LastYear", DateTime.Now.Year - 1);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal currentRevenue = Convert.ToDecimal(reader["CurrentYearRevenue"]);
                        decimal lastRevenue = Convert.ToDecimal(reader["LastYearRevenue"]);

                        if (lastRevenue > 0)
                        {
                            decimal revenueGrowth = ((currentRevenue - lastRevenue) / lastRevenue) * 100;
                            lblRevenueChange.Text = Math.Abs(revenueGrowth).ToString("N1");
                        }
                        else
                        {
                            lblRevenueChange.Text = "0";
                        }

                        int currentTransactions = Convert.ToInt32(reader["CurrentYearTransactions"]);
                        int lastTransactions = Convert.ToInt32(reader["LastYearTransactions"]);

                        if (lastTransactions > 0)
                        {
                            decimal transactionGrowth = ((decimal)(currentTransactions - lastTransactions) / lastTransactions) * 100;
                            lblTransactionChange.Text = Math.Abs(transactionGrowth).ToString("N1");
                        }
                        else
                        {
                            lblTransactionChange.Text = "0";
                        }
                    }
                }
            }
        }

        private void FindBestMonth(SqlConnection con)
        {
            string query = @"
                SELECT TOP 1 
                    DATENAME(MONTH, CheckOutDate) AS BestMonth,
                    SUM(TotalAmount) AS MonthRevenue
                FROM Bookings
                WHERE YEAR(CheckOutDate) = @Year
                    AND Status = 'CheckedOut'
                GROUP BY MONTH(CheckOutDate), DATENAME(MONTH, CheckOutDate)
                ORDER BY MonthRevenue DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                   
                        string monthName = reader["BestMonth"].ToString();
                        lblBestMonth.Text = ConvertMonthToJapanese(monthName);
                    }
                    else
                    {
                        lblBestMonth.Text = "N/A";
                    }
                }
            }
        }

        private void LoadMonthlyBreakdown(SqlConnection con)
        {
            string query = @"
                WITH Months AS (
                    SELECT 1 AS MonthNum, '1月' AS MonthName UNION ALL
                    SELECT 2, '2月' UNION ALL
                    SELECT 3, '3月' UNION ALL
                    SELECT 4, '4月' UNION ALL
                    SELECT 5, '5月' UNION ALL
                    SELECT 6, '6月' UNION ALL
                    SELECT 7, '7月' UNION ALL
                    SELECT 8, '8月' UNION ALL
                    SELECT 9, '9月' UNION ALL
                    SELECT 10, '10月' UNION ALL
                    SELECT 11, '11月' UNION ALL
                    SELECT 12, '12月'
                )
                SELECT 
                    m.MonthName AS Month,
                    ISNULL(SUM(b.TotalAmount), 0) AS Revenue,
                    ISNULL(COUNT(b.BookingID), 0) AS Transactions,
                    ISNULL(AVG(b.TotalAmount), 0) AS AverageTransaction,
                    ISNULL(COUNT(DISTINCT b.BookingID), 0) AS Bookings
                FROM Months m
                LEFT JOIN Bookings b ON MONTH(b.CheckOutDate) = m.MonthNum 
                    AND YEAR(b.CheckOutDate) = @Year
                    AND b.Status = 'CheckedOut'
                GROUP BY m.MonthNum, m.MonthName
                ORDER BY m.MonthNum";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                   
                    gvMonthlyBreakdown.DataSource = dt;
                    gvMonthlyBreakdown.DataBind();

                   
                    PrepareChartData(dt);
                }
            }
        }

        private void PrepareChartData(DataTable dt)
        {
            var labels = new System.Collections.Generic.List<string>();
            var data = new System.Collections.Generic.List<decimal>();

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(row["Month"].ToString());
                data.Add(Convert.ToDecimal(row["Revenue"]));
            }

            var chartData = new
            {
                labels = labels,
                data = data
            };

            hfChartData.Value = JsonConvert.SerializeObject(chartData);
        }

        
        private string ConvertMonthToJapanese(string englishMonth)
        {
            switch (englishMonth.ToLower())
            {
                case "january": return "1月";
                case "february": return "2月";
                case "march": return "3月";
                case "april": return "4月";
                case "may": return "5月";
                case "june": return "6月";
                case "july": return "7月";
                case "august": return "8月";
                case "september": return "9月";
                case "october": return "10月";
                case "november": return "11月";
                case "december": return "12月";
                default: return englishMonth;
            }
        }
    }
}