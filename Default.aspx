<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HotelManagement.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hotel Management Dashboard</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #f5f7fa;
            color: #333;
        }

        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            max-width: 1400px;
            margin: 0 auto;
        }

        .header h1 {
            font-size: 28px;
            font-weight: 600;
        }

        .user-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .user-info a {
            color: white;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 8px 15px;
            border-radius: 8px;
            transition: background 0.3s;
        }

        .user-info a:hover {
            background: rgba(255,255,255,0.2);
        }

        .user-info i {
            font-size: 24px;
        }

        .dashboard-container {
            max-width: 1400px;
            margin: 30px auto;
            padding: 0 40px;
        }

        .section-title {
            font-size: 20px;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .section-title i {
            color: #667eea;
        }

        /* Row Sections */
        .dashboard-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 25px;
            margin-bottom: 40px;
        }

        /* Card Styles */
        .stat-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            transition: transform 0.3s, box-shadow 0.3s;
            border-left: 4px solid;
            cursor: pointer;
        }

        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
        }

        .stat-card.guests { border-color: #667eea; }
        .stat-card.bookings { border-color: #f093fb; }
        .stat-card.checkins { border-color: #4facfe; }

        .stat-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }

        .stat-title {
            font-size: 14px;
            color: #718096;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .stat-icon {
            width: 50px;
            height: 50px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            color: white;
        }

        .stat-card.guests .stat-icon { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
        .stat-card.bookings .stat-icon { background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); }
        .stat-card.checkins .stat-icon { background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); }

        .stat-value {
            font-size: 36px;
            font-weight: 700;
            color: #2d3748;
            margin-bottom: 5px;
        }

        .stat-label {
            font-size: 13px;
            color: #a0aec0;
        }

        /* Room Cards */
        .room-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            transition: transform 0.3s;
            text-align: center;
        }

        .room-card:hover {
            transform: translateY(-5px);
        }

        .room-card.available { border-top: 4px solid #48bb78; }
        .room-card.occupied { border-top: 4px solid #f56565; }
        .room-card.reserved { border-top: 4px solid #ed8936; }

        .room-icon {
            width: 70px;
            height: 70px;
            border-radius: 50%;
            margin: 0 auto 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 32px;
            color: white;
        }

        .room-card.available .room-icon { background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); }
        .room-card.occupied .room-icon { background: linear-gradient(135deg, #f56565 0%, #e53e3e 100%); }
        .room-card.reserved .room-icon { background: linear-gradient(135deg, #ed8936 0%, #dd6b20 100%); }

        .room-value {
            font-size: 42px;
            font-weight: 700;
            color: #2d3748;
            margin-bottom: 5px;
        }

        .room-label {
            font-size: 16px;
            color: #718096;
            font-weight: 500;
        }

        /* Sales Card */
        .sales-card {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            border-radius: 12px;
            padding: 35px;
            color: white;
            box-shadow: 0 4px 15px rgba(245, 87, 108, 0.3);
            cursor: pointer;
            transition: transform 0.3s;
            grid-column: 1 / -1;
        }

        .sales-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(245, 87, 108, 0.4);
        }

        .sales-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .sales-info h3 {
            font-size: 18px;
            margin-bottom: 10px;
            opacity: 0.95;
        }

        .sales-amount {
            font-size: 48px;
            font-weight: 700;
            margin-bottom: 5px;
        }

        .sales-label {
            font-size: 14px;
            opacity: 0.9;
        }

        .sales-icon-large {
            font-size: 80px;
            opacity: 0.2;
        }

        .view-details {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            background: rgba(255,255,255,0.2);
            padding: 10px 20px;
            border-radius: 8px;
            margin-top: 15px;
            font-size: 14px;
            transition: background 0.3s;
        }

        .view-details:hover {
            background: rgba(255,255,255,0.3);
        }

        /* Table Styles */
        .data-table {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            overflow-x: auto;
        }

        .data-table table {
            width: 100%;
            border-collapse: collapse;
        }

        .data-table th {
            background: #f7fafc;
            padding: 12px;
            text-align: left;
            font-weight: 600;
            color: #4a5568;
            border-bottom: 2px solid #e2e8f0;
            font-size: 14px;
        }

        .data-table td {
            padding: 12px;
            border-bottom: 1px solid #e2e8f0;
            color: #2d3748;
            font-size: 14px;
        }

        .data-table tr:hover {
            background: #f7fafc;
        }

        .status-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
        }

        .status-badge.active { background: #c6f6d5; color: #22543d; }
        .status-badge.confirmed { background: #bee3f8; color: #2c5282; }
        .status-badge.checkedin { background: #fbd38d; color: #744210; }

        /* Quick Actions */
        .quick-actions {
            display: flex;
            gap: 15px;
            margin-bottom: 30px;
        }

        .action-btn {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px 24px;
            border-radius: 10px;
            text-decoration: none;
            font-weight: 600;
            transition: all 0.3s;
            box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
        }

        .action-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
        }

        .action-btn i {
            font-size: 18px;
        }

        /* Clickable card link wrapper */
        .card-link {
            text-decoration: none;
            color: inherit;
            display: block;
        }

        @media (max-width: 768px) {
            .dashboard-container {
                padding: 0 20px;
            }
            
            .dashboard-row {
                grid-template-columns: 1fr;
            }

            .sales-content {
                flex-direction: column;
                text-align: center;
            }

            .sales-icon-large {
                display: none;
            }

            .quick-actions {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <h1><i class="fas fa-hotel"></i> Hotel Management Dashboard</h1>
                <div class="user-info">
                    <a href="Profile.aspx">
                        <span><asp:Label ID="lblUsername" runat="server" Text="Admin User"></asp:Label></span>
                        <i class="fas fa-user-circle"></i>
                    </a>
                </div>
            </div>
        </div>

        <!-- Dashboard Container -->
        <div class="dashboard-container">
            
            <!-- Quick Actions -->
            <div class="quick-actions">
                <a href="Booking.aspx" class="action-btn" style="background: linear-gradient(135deg, #48bb78 0%, #38a169 100%);">
                    <i class="fas fa-walking"></i>
                    <span>Walk-in Check-In</span>
                </a>
                <a href="Booking.aspx" class="action-btn">
                    <i class="fas fa-calendar-check"></i>
                    <span>Create Booking</span>
                </a>
                <a href="GuestList.aspx" class="action-btn" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                    <i class="fas fa-address-book"></i>
                    <span>View All Guests</span>
                </a>
            </div>
            
            <!-- First Row: Check-ins and Bookings -->
            <div class="section-title">
                <i class="fas fa-chart-line"></i> Overview
            </div>
            <div class="dashboard-row">
                <!-- Check-ins Card - Clickable -->
                <a href="CheckInsList.aspx" class="card-link">
                    <div class="stat-card checkins">
                        <div class="stat-header">
                            <span class="stat-title">Check-ins</span>
                            <div class="stat-icon">
                                <i class="fas fa-sign-in-alt"></i>
                            </div>
                        </div>
                        <div class="stat-value">
                            <asp:Label ID="lblCheckIns" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-label">Expected today</div>
                    </div>
                </a>

                <!-- Bookings Card - Clickable -->
                <a href="BookingsList.aspx" class="card-link">
                    <div class="stat-card bookings">
                        <div class="stat-header">
                            <span class="stat-title">Overall Guest Data</span>
                            <div class="stat-icon">
                                <i class="fas fa-calendar-check"></i>
                            </div>
                        </div>
                        <div class="stat-value">
                            <asp:Label ID="lblBookings" runat="server" Text="0"></asp:Label>
                        </div>
                        <div class="stat-label">Total bookings</div>
                    </div>
                </a>
            </div>

            <!-- Second Row: Room Status -->
    <div class="section-title">
        <i class="fas fa-door-open"></i> Rooms
    </div>
    <div class="dashboard-row" style="cursor: pointer;" onclick="window.location.href='Rooms.aspx'">
        <div class="room-card available">
            <div class="room-icon">
                <i class="fas fa-bed"></i>
            </div>
            <div class="room-value">
                <asp:Label ID="lblAvailableRooms" runat="server" Text="0"></asp:Label>
            </div>
            <div class="room-label">Available</div>
        </div>
        <div class="room-card occupied">
            <div class="room-icon">
                <i class="fas fa-user-check"></i>
            </div>
            <div class="room-value">
                <asp:Label ID="lblOccupiedRooms" runat="server" Text="0"></asp:Label>
            </div>
            <div class="room-label">Occupied</div>
        </div>
        <div class="room-card reserved">
            <div class="room-icon">
                <i class="fas fa-bookmark"></i>
            </div>
            <div class="room-value">
                <asp:Label ID="lblReservedRooms" runat="server" Text="0"></asp:Label>
            </div>
            <div class="room-label">Reserved</div>
        </div>
    </div>
            <!-- Third Row: Sales & Revenue -->
            <div class="section-title">
                <i class="fas fa-chart-bar"></i> Financial Overview
            </div>
            <div class="dashboard-row">
                <asp:LinkButton ID="btnViewSales" runat="server" OnClick="btnViewSales_Click" CssClass="sales-card" style="text-decoration: none; color: white;">
                    <div class="sales-content">
                        <div class="sales-info">
                            <h3>Monthly Revenue</h3>
                            <div class="sales-amount">
                                ¥<asp:Label ID="lblMonthlyRevenue" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="sales-label">
                                <asp:Label ID="lblTransactionCount" runat="server" Text="0"></asp:Label> transactions this month
                            </div>
                            <div class="view-details">
                                <span>View Full Report</span>
                                <i class="fas fa-arrow-right"></i>
                            </div>
                        </div>
                        <div class="sales-icon-large">
                            <i class="fas fa-yen-sign"></i>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

            <!-- Recent Guests Table -->
            <div class="section-title">
                <i class="fas fa-list"></i> Current Guests
            </div>
            <div class="data-table">
                <asp:GridView ID="gvCurrentGuests" runat="server" AutoGenerateColumns="False" 
                    GridLines="None" CssClass="table" ShowHeaderWhenEmpty="True" EmptyDataText="No current guests">
                    <Columns>
                        <asp:BoundField DataField="GuestName" HeaderText="Guest Name" />
                        <asp:BoundField DataField="RoomNumber" HeaderText="Room" />
                        <asp:BoundField DataField="RoomType" HeaderText="Room Type" />
                        <asp:BoundField DataField="CheckInDate" HeaderText="Check In" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="CheckOutDate" HeaderText="Check Out" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="NightsStay" HeaderText="Nights" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>