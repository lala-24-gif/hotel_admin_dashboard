<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesReport.aspx.cs" Inherits="HotelManagement.SalesReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>売上・収益レポート</title>
    
    <!-- Viewport for responsive design -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0, viewport-fit=cover" />
    
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" /> 
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.9.1/chart.min.js"></script> 
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
            margin: 30px auto;
            padding: 0 40px;
        }

        .page-title {
            font-size: 32px;
            font-weight: 700;
            color: #2d3748;
            margin-bottom: 10px;
        }

        .page-subtitle {
            font-size: 16px;
            color: #718096;
            margin-bottom: 30px;
        }

        .summary-cards {
           display: grid;
           grid-template-columns: repeat(2, 1fr);
           gap: 25px;
           margin-bottom: 40px;
           max-width: 900px;
           margin-left: auto;  /* NEW: Center horizontally */
           margin-right: auto; /* NEW: Center horizontally */
}

        .summary-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            border-left: 4px solid;
            transition: transform 0.3s;
            min-height: 140px; /* Consistent card height */
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .summary-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.12);
        }

        .summary-card.revenue { border-color: #48bb78; }
        .summary-card.transactions { border-color: #667eea; }
        .summary-card.average { border-color: #ed8936; }
        .summary-card.growth { border-color: #4299e1; }
        .summary-card.guests { border-color: #f093fb; }
        .summary-card.avg-guests { border-color: #4facfe; }

        .summary-label {
            font-size: 14px;
            color: #718096;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            font-weight: 600;
        }

        .summary-value {
            font-size: 36px;
            font-weight: 700;
            color: #2d3748;
            line-height: 1.2;
        }

        .summary-change {
            font-size: 13px;
            margin-top: 8px;
            color: #718096;
        }

        .summary-change.positive {
            color: #48bb78;
        }

        .summary-change.negative {
            color: #f56565;
        }

        .chart-container {
            background: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            margin-bottom: 30px;
        }

        .chart-title {
            font-size: 20px;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .chart-wrapper {
            position: relative;
            height: 400px;
        }

        .monthly-breakdown {
            background: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .breakdown-table {
            width: 100%;
            border-collapse: collapse;
        }

        .breakdown-table th {
            background: #f7fafc;
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #4a5568;
            border-bottom: 2px solid #e2e8f0;
            font-size: 14px;
        }

        .breakdown-table td {
            padding: 15px;
            border-bottom: 1px solid #e2e8f0;
            color: #2d3748;
        }

        .breakdown-table tr:hover {
            background: #f7fafc;
        }

        .month-name {
            font-weight: 600;
            color: #2d3748;
        }

        .amount {
            font-weight: 600;
            color: #48bb78;
        }

        /* Tablet: Keep 2 columns */
        @media (max-width: 1024px) {
            .summary-cards {
                grid-template-columns: repeat(2, 1fr);
                max-width: 100%;
            }
        }

        /* Mobile: 1 column */
        @media (max-width: 768px) {
            .container {
                padding: 0 20px;
            }

            .summary-cards {
                grid-template-columns: 1fr;
                max-width: 100%;
            }
            
            .summary-card {
                min-height: auto;
            }

            .chart-wrapper {
                height: 300px;
            }
            
            .page-title {
                font-size: 24px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <h1><i class="fas fa-chart-line"></i> 売上・収益レポート</h1>
                <a href="Default.aspx" class="back-btn">
                    <i class="fas fa-arrow-left"></i>
                    <span>ダッシュボードに戻る</span>
                </a>
            </div>
        </div>

        <!-- Main Container -->
        <div class="container">
            <div class="page-title">年間収益レポート</div>
            <div class="page-subtitle"><asp:Label ID="lblCurrentYear" runat="server"></asp:Label>年の財務概要</div>

            <!-- Summary Cards - 2-2-2 BALANCED LAYOUT -->
            <div class="summary-cards">
                <!-- Row 1: Revenue & Transactions -->
                <div class="summary-card revenue">
                    <div class="summary-label">総収益</div>
                    <div class="summary-value">¥<asp:Label ID="lblTotalRevenue" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change positive">
                        <i class="fas fa-arrow-up"></i> 前年比 <asp:Label ID="lblRevenueChange" runat="server" Text="0"></asp:Label>%
                    </div>
                </div>

                <div class="summary-card transactions">
                    <div class="summary-label">総取引数</div>
                    <div class="summary-value"><asp:Label ID="lblTotalTransactions" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change positive">
                        <i class="fas fa-arrow-up"></i> 前年比 <asp:Label ID="lblTransactionChange" runat="server" Text="0"></asp:Label>%
                    </div>
                </div>

                <!-- Row 2: Average Transaction & Best Month -->
                <div class="summary-card average">
                    <div class="summary-label">平均取引額</div>
                    <div class="summary-value">¥<asp:Label ID="lblAverageTransaction" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change">予約あたりの平均</div>
                </div>

                <div class="summary-card growth">
                    <div class="summary-label">最高収益月</div>
                    <div class="summary-value"><asp:Label ID="lblBestMonth" runat="server" Text="-"></asp:Label></div>
                    <div class="summary-change">最高収益月</div>
                </div>

                <!-- Row 3: Guest Statistics -->
                <div class="summary-card guests">
                    <div class="summary-label">年間ゲスト数</div>
                    <div class="summary-value"><asp:Label ID="lblTotalGuests" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change">総ゲスト数</div>
                </div>

                <div class="summary-card avg-guests">
                    <div class="summary-label">平均ゲスト数</div>
                    <div class="summary-value"><asp:Label ID="lblAvgGuests" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change">予約あたり</div>
                </div>
            </div>

            <!-- Revenue Chart -->
            <div class="chart-container">
                <div class="chart-title">
                    <i class="fas fa-chart-bar"></i> 月次収益推移
                </div>
                <div class="chart-wrapper">
                    <canvas id="revenueChart"></canvas>
                </div>
            </div>

            <!-- Guest Count Chart -->
            <div class="chart-container">
                <div class="chart-title">
                    <i class="fas fa-users"></i> 月次ゲスト数推移
                </div>
                <div class="chart-wrapper">
                    <canvas id="guestChart"></canvas>
                </div>
            </div>

            <!-- Monthly Breakdown Table -->
            <div class="monthly-breakdown">
                <div class="chart-title">
                    <i class="fas fa-table"></i> 月次内訳
                </div>
                <asp:GridView ID="gvMonthlyBreakdown" runat="server" AutoGenerateColumns="False" 
                    CssClass="breakdown-table" GridLines="None" ShowHeaderWhenEmpty="True">
                    <Columns>
                        <asp:BoundField DataField="Month" HeaderText="月" ItemStyle-CssClass="month-name" />
                        <asp:BoundField DataField="Revenue" HeaderText="収益" DataFormatString="¥{0:N0}" ItemStyle-CssClass="amount" />
                        <asp:BoundField DataField="Transactions" HeaderText="取引数" />
                        <asp:BoundField DataField="AverageTransaction" HeaderText="平均取引額" DataFormatString="¥{0:N0}" />
                        <asp:BoundField DataField="Bookings" HeaderText="予約数" />
                        <asp:BoundField DataField="Guests" HeaderText="ゲスト数" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Chart Scripts -->
        <asp:HiddenField ID="hfChartData" runat="server" />
        <asp:HiddenField ID="hfGuestData" runat="server" />
        
        <script type="text/javascript">
            window.onload = function () {
                // Revenue Chart
                var chartData = JSON.parse(document.getElementById('<%= hfChartData.ClientID %>').value);

                var ctx = document.getElementById('revenueChart').getContext('2d');
                var revenueChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: chartData.labels,
                        datasets: [{
                            label: '収益 (¥)',
                            data: chartData.data,
                            backgroundColor: 'rgba(102, 126, 234, 0.1)',
                            borderColor: 'rgba(102, 126, 234, 1)',
                            borderWidth: 3,
                            fill: true,
                            tension: 0.4,
                            pointBackgroundColor: 'rgba(102, 126, 234, 1)',
                            pointBorderColor: '#fff',
                            pointBorderWidth: 2,
                            pointRadius: 5,
                            pointHoverRadius: 7
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'top'
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        return '収益: ¥' + context.parsed.y.toLocaleString();
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function (value) {
                                        return '¥' + value.toLocaleString();
                                    }
                                }
                            }
                        }
                    }
                });

                // Guest Count Chart
                var guestData = JSON.parse(document.getElementById('<%= hfGuestData.ClientID %>').value);

                var guestCtx = document.getElementById('guestChart').getContext('2d');
                var guestChart = new Chart(guestCtx, {
                    type: 'bar',
                    data: {
                        labels: guestData.labels,
                        datasets: [{
                            label: 'ゲスト数 (人)',
                            data: guestData.data,
                            backgroundColor: 'rgba(240, 147, 251, 0.7)',
                            borderColor: 'rgba(240, 147, 251, 1)',
                            borderWidth: 2,
                            borderRadius: 8
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'top',
                                labels: {
                                    font: {
                                        size: 13,
                                        weight: '600'
                                    }
                                }
                            },
                            tooltip: {
                                backgroundColor: 'rgba(0, 0, 0, 0.8)',
                                padding: 12,
                                titleFont: {
                                    size: 14,
                                    weight: 'bold'
                                },
                                bodyFont: {
                                    size: 13
                                },
                                callbacks: {
                                    label: function (context) {
                                        return 'ゲスト数: ' + context.parsed.y + ' 人';
                                    }
                                }
                            }
                        },
                        scales: {
                            x: {
                                grid: {
                                    display: false
                                }
                            },
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function (value) {
                                        return value + ' 人';
                                    }
                                }
                            }
                        }
                    }
                });
            };
        </script>
    </form>
</body>
</html>