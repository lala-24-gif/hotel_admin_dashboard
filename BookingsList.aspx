<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookingsList.aspx.cs" Inherits="HotelManagement.BookingsList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Overall Guest Data</title>
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
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
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

        .back-btn {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            background: rgba(255,255,255,0.2);
            padding: 10px 20px;
            border-radius: 8px;
            color: white;
            text-decoration: none;
            transition: background 0.3s;
        }

        .back-btn:hover {
            background: rgba(255,255,255,0.3);
        }

        .container {
            max-width: 1400px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .page-intro {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-intro h2 {
            font-size: 32px;
            font-weight: 700;
            color: #2d3748;
            margin-bottom: 10px;
        }

        .page-intro p {
            font-size: 16px;
            color: #718096;
        }

        .filter-section {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            margin-bottom: 30px;
            display: flex;
            gap: 15px;
            align-items: end;
            flex-wrap: wrap;
        }

        .filter-group {
            flex: 1;
            min-width: 200px;
        }

        .filter-label {
            display: block;
            font-size: 14px;
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 8px;
        }

        .filter-select {
            width: 100%;
            padding: 10px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            font-size: 14px;
        }

        .btn-filter {
            padding: 10px 20px;
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-filter:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(240, 147, 251, 0.4);
        }

        .stats-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 40px;
        }

        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            border-left: 4px solid #f093fb;
        }

        .stat-label {
            font-size: 14px;
            color: #718096;
            margin-bottom: 8px;
        }

        .stat-value {
            font-size: 32px;
            font-weight: 700;
            color: #f093fb;
        }

        .alert {
            padding: 12px 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .alert-success {
            background: #c6f6d5;
            color: #22543d;
            border-left: 4px solid #48bb78;
        }

        .alert-danger {
            background: #fed7d7;
            color: #742a2a;
            border-left: 4px solid #f56565;
        }

        .bookings-grid {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            overflow: hidden;
        }

        .grid-header {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            color: white;
            padding: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .grid-header h3 {
            font-size: 20px;
            font-weight: 600;
        }

        .gridview-wrapper {
            overflow-x: auto;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
        }

        .gridview th {
            background: #f7fafc;
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #4a5568;
            border-bottom: 2px solid #e2e8f0;
            white-space: nowrap;
        }

        .gridview td {
            padding: 15px;
            border-bottom: 1px solid #e2e8f0;
        }

        .gridview tr:hover {
            background: #f7fafc;
        }

        .status-badge {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            display: inline-block;
        }

        .status-confirmed {
            background: #bee3f8;
            color: #2c5282;
        }

        .status-checkedin {
            background: #c6f6d5;
            color: #22543d;
        }

        .status-checkedout {
            background: #e2e8f0;
            color: #4a5568;
        }

        .status-cancelled {
            background: #fed7d7;
            color: #742a2a;
        }

        .btn-action {
            padding: 6px 12px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 13px;
            font-weight: 600;
            transition: all 0.3s;
            margin-right: 5px;
        }

        .btn-checkout {
            background: #48bb78;
            color: white;
        }

        .btn-checkout:hover {
            background: #38a169;
        }

        .btn-cancel {
            background: #f56565;
            color: white;
        }

        .btn-cancel:hover {
            background: #e53e3e;
        }

        .btn-view {
            background: #e2e8f0;
            color: #4a5568;
        }

        .btn-view:hover {
            background: #cbd5e0;
        }

        .empty-state {
            text-align: center;
            padding: 60px 20px;
        }

        .empty-icon {
            font-size: 64px;
            color: #cbd5e0;
            margin-bottom: 20px;
        }

        .empty-title {
            font-size: 24px;
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 10px;
        }

        .empty-desc {
            font-size: 16px;
            color: #718096;
        }

        @media (max-width: 768px) {
            .filter-section {
                flex-direction: column;
            }

            .filter-group {
                width: 100%;
            }

            .stats-cards {
                grid-template-columns: 1fr;
            }

            .gridview th,
            .gridview td {
                padding: 10px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <h1><i class="fas fa-users-cog"></i> Overall Guest Data</h1>
                <a href="Default.aspx" class="back-btn">
                    <i class="fas fa-arrow-left"></i>
                    <span>Back to Dashboard</span>
                </a>
            </div>
        </div>

        <div class="container">
            <div class="page-intro">
                <h2>Manage Guest Data</h2>
                <p>View and manage all guest bookings and stays</p>
            </div>

            <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                <i class="fas fa-check-circle"></i>
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                <i class="fas fa-exclamation-circle"></i>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <div class="filter-section">
                <div class="filter-group">
                    <label class="filter-label">Filter by Status</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="filter-select">
                        <asp:ListItem Value="All" Selected="True">All Guests</asp:ListItem>
                        <asp:ListItem Value="Confirmed">Confirmed</asp:ListItem>
                        <asp:ListItem Value="CheckedIn">Checked In</asp:ListItem>
                        <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label class="filter-label">Date Range</label>
                    <asp:DropDownList ID="ddlDateFilter" runat="server" CssClass="filter-select">
                        <asp:ListItem Value="All" Selected="True">All Dates</asp:ListItem>
                        <asp:ListItem Value="Today">Today</asp:ListItem>
                        <asp:ListItem Value="Week">This Week</asp:ListItem>
                        <asp:ListItem Value="Month">This Month</asp:ListItem>
                        <asp:ListItem Value="Future">Future Guests</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter" CssClass="btn-filter" OnClick="btnApplyFilter_Click" />
                </div>
            </div>

            <div class="stats-cards">
                <div class="stat-card">
                    <div class="stat-label">Total Records</div>
                    <div class="stat-value">
                        <asp:Label ID="lblTotalBookings" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-label">Confirmed</div>
                    <div class="stat-value">
                        <asp:Label ID="lblConfirmed" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-label">Checked In</div>
                    <div class="stat-value">
                        <asp:Label ID="lblCheckedIn" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
                <div class="stat-card">
                    <div class="stat-label">Total Revenue</div>
                    <div class="stat-value">
                        ¥<asp:Label ID="lblTotalRevenue" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="bookings-grid">
                <div class="grid-header">
                    <h3><i class="fas fa-database"></i> Guest Data List</h3>
                </div>
                <div class="gridview-wrapper">
                    <asp:GridView ID="gvBookings" runat="server" 
                        CssClass="gridview" 
                        AutoGenerateColumns="False"
                        OnRowCommand="gvBookings_RowCommand"
                        EmptyDataText="No bookings found">
                        <Columns>
                            <asp:BoundField DataField="BookingID" HeaderText="ID" />
                            <asp:BoundField DataField="GuestName" HeaderText="Guest Name" />
                            <asp:BoundField DataField="RoomNumber" HeaderText="Room" />
                            <asp:BoundField DataField="CheckInDate" HeaderText="Check-In" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="CheckOutDate" HeaderText="Check-Out" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="NumberOfGuests" HeaderText="Guests" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="Amount" DataFormatString="¥{0:N0}" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                        <%# Eval("Status") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnCheckOut" runat="server" 
                                        Text="Check Out" 
                                        CommandName="CheckOut" 
                                        CommandArgument='<%# Eval("BookingID") %>'
                                        CssClass="btn-action btn-checkout"
                                        Visible='<%# Eval("Status").ToString() == "CheckedIn" %>'
                                        OnClientClick="return confirm('Check out this guest?');" />
                                    <asp:Button ID="btnCancel" runat="server" 
                                        Text="Cancel" 
                                        CommandName="Cancel" 
                                        CommandArgument='<%# Eval("BookingID") %>'
                                        CssClass="btn-action btn-cancel"
                                        Visible='<%# Eval("Status").ToString() != "CheckedOut" && Eval("Status").ToString() != "Cancelled" %>'
                                        OnClientClick="return confirm('Cancel this booking?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-state">
                                <div class="empty-icon">
                                    <i class="fas fa-calendar-times"></i>
                                </div>
                                <div class="empty-title">No Guest Data Found</div>
                                <div class="empty-desc">Try adjusting your filters or create a new booking</div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>