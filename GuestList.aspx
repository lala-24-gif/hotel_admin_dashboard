<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GuestList.aspx.cs" Inherits="HotelManagement.GuestList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ゲストディレクトリ</title>
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
            border-left: 4px solid #f56565;
        }


        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            overflow: auto;
        }

        .modal-content {
            background-color: white;
            margin: 5% auto;
            width: 90%;
            max-width: 600px;
            border-radius: 12px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            overflow: hidden;
            display: flex;
            flex-direction: column;
            max-height: 90vh;
        }

        .modal-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 25px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-shrink: 0;
        }

        .modal-header h2 {
            margin: 0;
            font-size: 24px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .close {
            color: white;
            font-size: 32px;
            font-weight: bold;
            cursor: pointer;
            background: none;
            border: none;
            padding: 0;
            width: 32px;
            height: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: opacity 0.2s;
        }

        .close:hover {
            opacity: 0.7;
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
                <h1><i class="fas fa-address-book"></i> ゲストディレクトリ</h1>
                <a href="Default.aspx" class="back-btn">
                    <i class="fas fa-arrow-left"></i>
                    <span>ダッシュボードに戻る</span>
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
                    <div class="page-title">全ゲスト</div>
                    <div class="page-subtitle">登録済みゲストの完全リスト</div>
                </div>
                <a href="AddGuest.aspx" class="action-btn">
                    <i class="fas fa-user-plus"></i>
                    <span>新規ゲスト追加</span>
                </a>
            </div>

            <div class="stats-row">
                <div class="stat-card">
                    <div class="stat-label">総ゲスト数</div>
                    <div class="stat-value">
                        <asp:Label ID="lblTotalGuests" runat="server" Text="0"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="search-box">
                <div class="search-wrapper">
                    <i class="fas fa-search search-icon"></i>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                        placeholder="名前、メール、または電話番号で検索..." AutoPostBack="True" 
                        OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                </div>
            </div>

            <div class="guest-table">
                <asp:GridView ID="gvGuests" runat="server" AutoGenerateColumns="False" 
                    GridLines="None" ShowHeaderWhenEmpty="True" EmptyDataText="ゲストが見つかりません"
                    OnRowCommand="gvGuests_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="GuestName" HeaderText="ゲスト名" />
                        <asp:BoundField DataField="Email" HeaderText="メールアドレス" />
                        <asp:BoundField DataField="Phone" HeaderText="電話番号" />
                        <asp:BoundField DataField="IDNumber" HeaderText="ID番号" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="登録日" DataFormatString="{0:yyyy年MM月dd日}" />
                        <asp:BoundField DataField="TotalBookings" HeaderText="予約総数" />
                        
                        <asp:TemplateField HeaderText="操作">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" 
                                        CommandName="EditGuest" 
                                        CommandArgument='<%# Eval("GuestID") %>'
                                        CssClass="btn btn-edit">
                                        <i class="fas fa-edit"></i> 編集
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                        CommandName="DeleteGuest" 
                                        CommandArgument='<%# Eval("GuestID") %>'
                                        CssClass="btn btn-delete"
                                        OnClientClick="return confirm('このゲストを削除してもよろしいですか？この操作は元に戻せません。');">
                                        <i class="fas fa-trash"></i> 削除
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
                    <h2><i class="fas fa-user-edit"></i> ゲスト情報編集</h2>
                    <button type="button" class="close" onclick="hideEditModal()">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfEditGuestID" runat="server" />
                    
                    <div class="form-group">
                        <label>名</label>
                        <asp:TextBox ID="txtEditFirstName" runat="server" placeholder="名を入力してください" />
                    </div>

                    <div class="form-group">
                        <label>姓</label>
                        <asp:TextBox ID="txtEditLastName" runat="server" placeholder="姓を入力してください" />
                    </div>

                    <div class="form-group">
                        <label>メールアドレス</label>
                        <asp:TextBox ID="txtEditEmail" runat="server" placeholder="メールアドレスを入力してください" TextMode="Email" />
                    </div>

                    <div class="form-group">
                        <label>電話番号</label>
                        <asp:TextBox ID="txtEditPhone" runat="server" placeholder="電話番号を入力してください" />
                    </div>

                    <div class="form-group">
                        <label>ID番号</label>
                        <asp:TextBox ID="txtEditIDNumber" runat="server" placeholder="ID番号を入力してください" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-cancel" onclick="hideEditModal()">キャンセル</button>
                    <asp:Button ID="btnSaveEdit" runat="server" Text="変更を保存" 
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

            // モーダルの外側をクリックしたら閉じる
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
