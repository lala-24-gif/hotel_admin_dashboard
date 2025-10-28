<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesReport.aspx.cs" Inherits="HotelManagement.SalesReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>売上・収益レポート</title>
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
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 25px;
            margin-bottom: 40px;
        }

        .summary-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            border-left: 4px solid;
        }

        .summary-card.revenue { border-color: #48bb78; }
        .summary-card.transactions { border-color: #667eea; }
        .summary-card.average { border-color: #ed8936; }
        .summary-card.growth { border-color: #4299e1; }

        .summary-label {
            font-size: 14px;
            color: #718096;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .summary-value {
            font-size: 36px;
            font-weight: 700;
            color: #2d3748;
        }

        .summary-change {
            font-size: 13px;
            margin-top: 8px;
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

        @media (max-width: 768px) {
            .container {
                padding: 0 20px;
            }

            .summary-cards {
                grid-template-columns: 1fr;
            }

            .chart-wrapper {
                height: 300px;
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

            <!-- Summary Cards -->
            <div class="summary-cards">
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

                <div class="summary-card average">
                    <div class="summary-label">平均取引額</div>
                    <div class="summary-value">¥<asp:Label ID="lblAverageTransaction" runat="server" Text="0"></asp:Label></div>
                    <div class="summary-change">予約あたりの平均</div>
                </div>

                <div class="summary-card growth">
                    <div class="summary-label">最高月</div>
                    <div class="summary-value"><asp:Label ID="lblBestMonth" runat="server" Text="-"></asp:Label></div>
                    <div class="summary-change">最高収益月</div>
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
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Chart Script -->
        <asp:HiddenField ID="hfChartData" runat="server" />
        <script type="text/javascript">
            window.onload = function () {
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
            };
        </script>
    </form>
</body>
</html>
