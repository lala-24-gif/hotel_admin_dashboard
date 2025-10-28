using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;

namespace HotelManagement
{
    public partial class Rooms : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoomStatistics();
                LoadRoomsByFloor();

                // URLにステータスフィルターがあるかチェック
                string statusFilter = Request.QueryString["status"];
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    // アクティブなフィルターの視覚的インジケーターを追加
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "HighlightStatus",
                        $"setTimeout(function(){{ highlightStatus('{statusFilter}'); }}, 100);", true);
                }
            }
        }

        // 客室統計を読み込む
        private void LoadRoomStatistics()
        {
            try
            {
                con.Open();

                // ステータス別の客室数を取得
                string query = @"
                    SELECT 
                        Status,
                        COUNT(*) as Count
                    FROM Rooms
                    GROUP BY Status";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                int available = 0;
                int occupied = 0;
                int reserved = 0;

                while (reader.Read())
                {
                    string status = reader["Status"]?.ToString() ?? "Available";
                    int count = Convert.ToInt32(reader["Count"]);

                    switch (status.ToLower())
                    {
                        case "available":
                            available = count;
                            break;
                        case "occupied":
                            occupied = count;
                            break;
                        case "reserved":
                            reserved = count;
                            break;
                    }
                }

                reader.Close();

                lblAvailable.Text = available.ToString();
                lblOccupied.Text = occupied.ToString();
                lblReserved.Text = reserved.ToString();

                con.Close();
            }
            catch (Exception ex)
            {
                // エラー処理
                con.Close();
            }
        }

        // フロア別に客室を読み込む
        private void LoadRoomsByFloor()
        {
            try
            {
                con.Open();

                string query = @"
                    SELECT 
                        r.RoomID,
                        r.RoomNumber,
                        r.Floor,
                        r.Status,
                        rt.TypeName,
                        rt.BasePrice,
                        rt.Capacity
                    FROM Rooms r
                    INNER JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
                    ORDER BY r.Floor, r.RoomNumber";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                con.Close();

                // フロア別に客室をグループ化
                StringBuilder html = new StringBuilder();

                for (int floor = 1; floor <= 5; floor++)
                {
                    DataRow[] floorRooms = dt.Select($"Floor = {floor}");

                    html.Append("<div class='floor-section'>");
                    html.Append($"<div class='floor-header'>");
                    html.Append($"<div class='floor-title'>{floor}階</div>");

                    // フロア統計を計算（メンテナンスを除く）
                    int floorAvailable = floorRooms.Count(r => (r["Status"]?.ToString() ?? "Available").Equals("Available", StringComparison.OrdinalIgnoreCase));
                    int floorOccupied = floorRooms.Count(r => (r["Status"]?.ToString() ?? "").Equals("Occupied", StringComparison.OrdinalIgnoreCase));
                    int floorReserved = floorRooms.Count(r => (r["Status"]?.ToString() ?? "").Equals("Reserved", StringComparison.OrdinalIgnoreCase));

                    html.Append("<div class='floor-stats'>");
                    if (floorAvailable > 0)
                        html.Append($"<div class='floor-stat'><div class='dot available'></div><span>{floorAvailable} 利用可能</span></div>");
                    if (floorOccupied > 0)
                        html.Append($"<div class='floor-stat'><div class='dot occupied'></div><span>{floorOccupied} 使用中</span></div>");
                    if (floorReserved > 0)
                        html.Append($"<div class='floor-stat'><div class='dot reserved'></div><span>{floorReserved} 予約済み</span></div>");
                    html.Append("</div>");
                    html.Append("</div>");

                    if (floorRooms.Length > 0)
                    {
                        html.Append("<div class='rooms-grid'>");

                        foreach (DataRow room in floorRooms)
                        {
                            string roomId = room["RoomID"].ToString();
                            string roomNumber = room["RoomNumber"].ToString();
                            string status = room["Status"]?.ToString() ?? "Available";
                            string roomType = room["TypeName"].ToString();
                            string price = Convert.ToDecimal(room["BasePrice"]).ToString("N0");
                            string capacity = room["Capacity"].ToString();

                            // ステータスを日本語に変換
                            string statusJp = status;
                            if (status.Equals("Available", StringComparison.OrdinalIgnoreCase))
                                statusJp = "利用可能";
                            else if (status.Equals("Occupied", StringComparison.OrdinalIgnoreCase))
                                statusJp = "使用中";
                            else if (status.Equals("Reserved", StringComparison.OrdinalIgnoreCase))
                                statusJp = "予約済み";

                            string statusClass = status.ToLower();

                            html.Append($@"
                                <div class='room-card {statusClass}' onclick=""showRoomDetails({roomId}, '{roomNumber}', {floor}, '{roomType}', {capacity}, {room["BasePrice"]}, '{status}')"">
                                    <div class='room-number'>{roomNumber}</div>
                                    <div class='room-type'>{roomType}</div>
                                    <div class='room-price'>¥{price}/泊</div>
                                    <span class='room-status {statusClass}'>{statusJp}</span>
                                </div>
                            ");
                        }

                        html.Append("</div>");
                    }
                    else
                    {
                        html.Append("<div class='empty-floor'>このフロアには客室がありません</div>");
                    }

                    html.Append("</div>");
                }

                litFloors.Text = html.ToString();
            }
            catch (Exception ex)
            {
                litFloors.Text = $"<div class='floor-section'><p style='color:red;'>客室の読み込みエラー: {ex.Message}</p></div>";
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ダッシュボードに戻る
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}