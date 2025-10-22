<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GuestList.aspx.cs" Inherits="HotelManagement.GuestList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Guest Directory</title>
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
            padding: 0 40px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .page-title {
            font-size: 32px;
            font-weight: 700;
            color: #2d3748;
        }

        .page-subtitle {
            font-size: 16px;
            color: #718096;
            margin-top: 5px;
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
            border: none;
            cursor: pointer;
        }

        .action-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
        }

        .search-box {
            background: white;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .search-input {
            width: 100%;
            padding: 12px 15px 12px 45px;
            border: 2px solid #e2e8f0;
            border-radius: 10px;
            font-size: 14px;
            transition: all 0.3s;
        }

        .search-input:focus {
            outline: none;
            border-color: #667eea;
        }

        .search-wrapper {
            position: relative;
        }

        .search-icon {
            position: absolute;
            left: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: #a0aec0;
        }

        .guest-table {
            background: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            overflow-x: auto;
        }

        .guest-table table {
            width: 100%;
            border-collapse: collapse;
        }

        .guest-table th {
            background: #f7fafc;
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #4a5568;
            border-bottom: 2px solid #e2e8f0;
            font-size: 14px;
        }

        .guest-table td {
            padding: 15px;
            border-bottom: 1px solid #e2e8f0;
            color: #2d3748;
            font-size: 14px;
        }

        .guest-table tr:hover {
            background: #f7fafc;
        }

        .stats-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: white;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            border-left: 4px solid #667eea;
        }

        .stat-label {
            font-size: 14px;
            color: #718096;
            margin-bottom: 10px;
        }

        .stat-value {
            font-size: 32px;
            font-weight: 700;
            color: #2d3748;
        }

        /* Action Buttons */
        .btn {
            padding: 6px 12px;
            border-radius: 6px;
            text-decoration: none;
            font-size: 13px;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            border: none;
            cursor: pointer;
            transition: all 0.2s;
        }

        .btn-edit {
            background: #4299e1;
            color: white;
        }

        .btn-edit:hover {
            background: #3182ce;
        }

        .btn-delete {
            background: #f56565;
            color: white;
        }

        .btn-delete:hover {
            background: #e53e3e;
        }

        .action-buttons {
            display: flex;
            gap: 8px;
        }

        /* Alert Messages */
        .alert {
            padding: 15px 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .alert-success {
            background: #c6f6d5;
            color: #22543d;
            border-left: 4px solid #38a169;
        }

        .alert-error {
            background: #fed7d7;
            color: #742a2a;
            border-left: 4px solid #e53e3e;
        }

        /* Modal Styles */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            overflow-y: auto;
        }

        .modal-content {
            background-color: white;
            margin: 50px auto;
            padding: 0;
            border-radius: 12px;
            width: 90%;
            max-width: 600px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            animation: slideDown 0.3s ease;
            position: relative;
            max-height: calc(100vh - 100px);
            display: flex;
            flex-direction: column;
        }

        @keyframes slideDown {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .modal-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px 30px;
            border-radius: 12px 12px 0 0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-shrink: 0;
        }

        .modal-header h2 {
            margin: 0;
            font-size: 22px;
        }

        .close {
            color: white;
            font-size: 28px;
            font-weight: bold;
            cursor: pointer;
            border: none;
            background: none;
            line-height: 1;
        }

        .close:hover {
            opacity: 0.8;
        }

        .modal-body {
            padding: 30px;
            overflow-y: auto;
            flex: 1;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #2d3748;
            font-size: 14px;
        }

        .form-group input {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            font-size: 14px;
            transition: all 0.3s;
        }

        .form-group input:focus {
            outline: none;
            border-color: #667eea;
        }

        .modal-footer {
            padding: 20px 30px;
            border-top: 1px solid #e2e8f0;
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            flex-shrink: 0;
            background: white;
            border-radius: 0 0 12px 12px;
        }

        .btn-cancel {
            background: #e2e8f0;
            color: #4a5568;
            padding: 10px 20px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-cancel:hover {
            background: #cbd5e0;
        }

        .btn-save {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            border: none;
            cursor: pointer;
            font-weight: 600;
        }

        .btn-save:hover {
            opacity: 0.9;
        }

        @media (max-width: 768px) {
            .container {
                padding: 0 20px;
            }

            .page-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 15px;
            }

            .modal-content {
                width: 95%;
                margin: 20px auto;
                max-height: calc(100vh - 40px);
            }

            .modal-body {
                padding: 20px;
            }

            .modal-footer {
                padding: 15px 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <h1><i class="fas fa-address-book"></i> Guest Directory</h1>
                <a href="Default.aspx" class="back-btn">
                    <i class="fas fa-arrow-left"></i>
                    <span>Back to Dashboard</span>
                </a>
            </div>
        </div>

        <div class="container">
            <!-- Alert Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                <i class="fas fa-check-circle"></i>
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-error">
                <i class="fas fa-exclamation-circle"></i>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <div class="page-header">
                <div>
                    <div class="page-title">All Guests</div>
                    <div class="page-subtitle">Complete list of registered guests</div>
                </div>
                <a href="AddGuest.aspx" class="action-btn">
                    <i class="fas fa-user-plus"></i>
                    <span>Add New Guest</span>
                </a>
            </div>

            <div class="stats-row">
                <div class="stat-card">
                    <div class="stat-label">Total Guests</div>
                    <div class="stat-value">
                        <asp:Label ID="lblTotalGuests" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="search-box">
                <div class="search-wrapper">
                    <i class="fas fa-search search-icon"></i>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                        placeholder="Search by name, email, or phone..." AutoPostBack="True" 
                        OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="guest-table">
                <asp:GridView ID="gvGuests" runat="server" AutoGenerateColumns="False" 
                    GridLines="None" ShowHeaderWhenEmpty="True" EmptyDataText="No guests found"
                    OnRowCommand="gvGuests_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="GuestName" HeaderText="Guest Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Phone" HeaderText="Phone" />
                        <asp:BoundField DataField="IDNumber" HeaderText="ID Number" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Registered On" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="TotalBookings" HeaderText="Total Bookings" />
                        
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                        CommandName="EditGuest" 
                                        CommandArgument='<%# Eval("GuestID") %>'
                                        CssClass="btn btn-edit">
                                        <i class="fas fa-edit"></i> Edit
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                        CommandName="DeleteGuest" 
                                        CommandArgument='<%# Eval("GuestID") %>'
                                        CssClass="btn btn-delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this guest? This action cannot be undone.');">
                                        <i class="fas fa-trash"></i> Delete
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Edit Guest Modal -->
        <div id="editModal" class="modal">
            <div class="modal-content">
                <div class="modal-header">
                    <h2><i class="fas fa-user-edit"></i> Edit Guest Information</h2>
                    <button type="button" class="close" onclick="hideEditModal()">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfEditGuestID" runat="server" />
                    
                    <div class="form-group">
                        <label>First Name</label>
                        <asp:TextBox ID="txtEditFirstName" runat="server" placeholder="Enter first name" />
                    </div>

                    <div class="form-group">
                        <label>Last Name</label>
                        <asp:TextBox ID="txtEditLastName" runat="server" placeholder="Enter last name" />
                    </div>

                    <div class="form-group">
                        <label>Email</label>
                        <asp:TextBox ID="txtEditEmail" runat="server" placeholder="Enter email address" TextMode="Email" />
                    </div>

                    <div class="form-group">
                        <label>Phone</label>
                        <asp:TextBox ID="txtEditPhone" runat="server" placeholder="Enter phone number" />
                    </div>

                    <div class="form-group">
                        <label>ID Number</label>
                        <asp:TextBox ID="txtEditIDNumber" runat="server" placeholder="Enter ID number" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-cancel" onclick="hideEditModal()">Cancel</button>
                    <asp:Button ID="btnSaveEdit" runat="server" Text="Save Changes" 
                        CssClass="btn-save" OnClick="btnSaveEdit_Click" />
                </div>
            </div>
        </div>

        <script>
            function showEditModal() {
                document.getElementById('editModal').style.display = 'block';
            }

            function hideEditModal() {
                document.getElementById('editModal').style.display = 'none';
            }

            // Close modal when clicking outside
            window.onclick = function (event) {
                var modal = document.getElementById('editModal');
                if (event.target == modal) {
                    hideEditModal();
                }
            }
        </script>
    </form>
</body>
</html>