<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckIn.aspx.cs" Inherits="HotelManagement.CheckIn" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Walk-in Check-In</title>
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
            background: linear-gradient(135deg, #48bb78 0%, #38a169 100%);
            color: white;
            padding: 20px 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            max-width: 1200px;
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
            max-width: 1000px;
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

        .option-cards {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 25px;
            margin-bottom: 40px;
        }

        .option-card {
            background: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            cursor: pointer;
            transition: all 0.3s;
            border: 3px solid transparent;
        }

        .option-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
        }

        .option-card.selected {
            border-color: #48bb78;
            background: #f0fff4;
        }

        .option-icon {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            background: linear-gradient(135deg, #48bb78 0%, #38a169 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            color: white;
            margin: 0 auto 15px;
        }

        .option-title {
            font-size: 20px;
            font-weight: 600;
            color: #2d3748;
            text-align: center;
            margin-bottom: 10px;
        }

        .option-desc {
            font-size: 14px;
            color: #718096;
            text-align: center;
        }

        .form-card {
            background: white;
            border-radius: 12px;
            padding: 40px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .section-title {
            font-size: 18px;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e2e8f0;
        }

        .section-title i {
            color: #48bb78;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 25px;
            margin-bottom: 25px;
        }

        .form-row.full {
            grid-template-columns: 1fr;
        }

        .form-group {
            margin-bottom: 25px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 8px;
        }

        .required {
            color: #f56565;
        }

        .input-wrapper {
            position: relative;
        }

        .input-icon {
            position: absolute;
            left: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: #a0aec0;
            font-size: 16px;
        }

        .form-input, .form-select {
            width: 100%;
            padding: 12px 15px 12px 45px;
            border: 2px solid #e2e8f0;
            border-radius: 10px;
            font-size: 14px;
            transition: all 0.3s;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .form-input:focus, .form-select:focus {
            outline: none;
            border-color: #48bb78;
            box-shadow: 0 0 0 3px rgba(72, 187, 120, 0.1);
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

        .validator {
            color: #f56565;
            font-size: 12px;
            margin-top: 5px;
            display: block;
        }

        .info-box {
            background: #f0fff4;
            border-left: 4px solid #48bb78;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
        }

        .info-box-title {
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 5px;
        }

        .info-box-value {
            font-size: 24px;
            font-weight: 700;
            color: #48bb78;
        }

        .form-actions {
            display: flex;
            gap: 15px;
            margin-top: 30px;
            padding-top: 30px;
            border-top: 1px solid #e2e8f0;
        }

        .btn {
            flex: 1;
            padding: 14px;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
        }

        .btn-primary {
            background: linear-gradient(135deg, #48bb78 0%, #38a169 100%);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(72, 187, 120, 0.3);
        }

        .btn-secondary {
            background: #e2e8f0;
            color: #4a5568;
        }

        .btn-secondary:hover {
            background: #cbd5e0;
        }

        .hidden {
            display: none;
        }

        @media (max-width: 768px) {
            .option-cards {
                grid-template-columns: 1fr;
            }

            .form-row {
                grid-template-columns: 1fr;
            }

            .form-card {
                padding: 30px 20px;
            }

            .form-actions {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <h1><i class="fas fa-walking"></i> Walk-in Check-In</h1>
                <a href="Default.aspx" class="back-btn">
                    <i class="fas fa-arrow-left"></i>
                    <span>Back to Dashboard</span>
                </a>
            </div>
        </div>

        <div class="container">
            <div class="page-intro">
                <h2>Walk-in Guest Check-In</h2>
                <p>Check in guests who arrive without prior booking</p>
            </div>

            <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                <i class="fas fa-check-circle"></i>
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                <i class="fas fa-exclamation-circle"></i>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Guest Selection Options -->
            <div class="option-cards">
                <asp:Panel ID="pnlNewGuest" runat="server" CssClass="option-card">
                    <asp:LinkButton ID="btnSelectNew" runat="server" OnClick="btnSelectNew_Click" CausesValidation="false" style="text-decoration: none; color: inherit; display: block;">
                        <div class="option-icon">
                            <i class="fas fa-user-plus"></i>
                        </div>
                        <div class="option-title">New Guest</div>
                        <div class="option-desc">Register and check in a new guest</div>
                    </asp:LinkButton>
                </asp:Panel>

                <asp:Panel ID="pnlExistingGuest" runat="server" CssClass="option-card">
                    <asp:LinkButton ID="btnSelectExisting" runat="server" OnClick="btnSelectExisting_Click" CausesValidation="false" style="text-decoration: none; color: inherit; display: block;">
                        <div class="option-icon">
                            <i class="fas fa-address-book"></i>
                        </div>
                        <div class="option-title">Existing Guest</div>
                        <div class="option-desc">Check in a previously registered guest</div>
                    </asp:LinkButton>
                </asp:Panel>
            </div>

            <!-- Form Card -->
            <div class="form-card">
                <!-- New Guest Form -->
                <asp:Panel ID="pnlNewGuestForm" runat="server" Visible="false">
                    <div class="section-title">
                        <i class="fas fa-user"></i> Guest Information
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">First Name <span class="required">*</span></label>
                            <div class="input-wrapper">
                                <i class="fas fa-user input-icon"></i>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" 
                                    placeholder="Enter first name" MaxLength="50"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                ControlToValidate="txtFirstName" Display="Dynamic" 
                                ErrorMessage="First name is required" CssClass="validator" 
                                ValidationGroup="CheckInGroup"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Last Name <span class="required">*</span></label>
                            <div class="input-wrapper">
                                <i class="fas fa-user input-icon"></i>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" 
                                    placeholder="Enter last name" MaxLength="50"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                ControlToValidate="txtLastName" Display="Dynamic" 
                                ErrorMessage="Last name is required" CssClass="validator" 
                                ValidationGroup="CheckInGroup"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Email</label>
                            <div class="input-wrapper">
                                <i class="fas fa-envelope input-icon"></i>
                                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" 
                                    CssClass="form-input" placeholder="guest@example.com"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Phone</label>
                            <div class="input-wrapper">
                                <i class="fas fa-phone input-icon"></i>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input" 
                                    placeholder="+1234567890"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">ID Number</label>
                            <div class="input-wrapper">
                                <i class="fas fa-id-card input-icon"></i>
                                <asp:TextBox ID="txtIDNumber" runat="server" CssClass="form-input" 
                                    placeholder="Passport or ID number"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Date of Birth</label>
                            <div class="input-wrapper">
                                <i class="fas fa-calendar input-icon"></i>
                                <asp:TextBox ID="txtDateOfBirth" runat="server" TextMode="Date" 
                                    CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Existing Guest Form -->
                <asp:Panel ID="pnlExistingGuestForm" runat="server" Visible="false">
                    <div class="section-title">
                        <i class="fas fa-user"></i> Select Guest
                    </div>

                    <div class="form-row full">
                        <div class="form-group">
                            <label class="form-label">Guest <span class="required">*</span></label>
                            <div class="input-wrapper">
                                <i class="fas fa-user-friends input-icon"></i>
                                <asp:DropDownList ID="ddlGuest" runat="server" CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvGuest" runat="server" 
                                ControlToValidate="ddlGuest" Display="Dynamic" InitialValue="0"
                                ErrorMessage="Please select a guest" CssClass="validator" 
                                ValidationGroup="CheckInGroup"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Room & Dates Section (Common for both) -->
                <asp:Panel ID="pnlRoomSection" runat="server" Visible="false">
                    <div class="section-title" style="margin-top: 30px;">
                        <i class="fas fa-bed"></i> Room & Stay Details
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Room <span class="required">*</span></label>
                            <div class="input-wrapper">
                                <i class="fas fa-door-open input-icon"></i>
                                <asp:DropDownList ID="ddlRoom" runat="server" CssClass="form-select" 
                                    AutoPostBack="True" OnSelectedIndexChanged="ddlRoom_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvRoom" runat="server" 
                                ControlToValidate="ddlRoom" Display="Dynamic" InitialValue="0"
                                ErrorMessage="Please select a room" CssClass="validator" 
                                ValidationGroup="CheckInGroup"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Number of Nights</label>
                            <div class="input-wrapper">
                                <i class="fas fa-moon input-icon"></i>
                                <asp:TextBox ID="txtNights" runat="server" TextMode="Number" 
                                    CssClass="form-input" Text="1" min="1" AutoPostBack="True" 
                                    OnTextChanged="CalculateTotal"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Check-Out Date</label>
                            <div class="input-wrapper">
                                <i class="fas fa-calendar-check input-icon"></i>
                                <asp:TextBox ID="txtCheckOut" runat="server" TextMode="Date" 
                                    CssClass="form-input" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Number of Guests</label>
                            <div class="input-wrapper">
                                <i class="fas fa-users input-icon"></i>
                                <asp:TextBox ID="txtNumberOfGuests" runat="server" TextMode="Number" 
                                    CssClass="form-input" Text="1" min="1"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="info-box">
                        <div class="info-box-title">Total Amount</div>
                        <div class="info-box-value">
                            $<asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" 
                            OnClick="btnCancel_Click" CausesValidation="false" />
                        <asp:Button ID="btnCheckIn" runat="server" Text="Check In Now" CssClass="btn btn-primary" 
                            OnClick="btnCheckIn_Click" ValidationGroup="CheckInGroup" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>