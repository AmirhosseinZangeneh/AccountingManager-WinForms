using AccountingManager.App.UI;
using AccountingManager.Business;
using AccountingManager.ViewModels.Accounting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountingManager.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            ConfigureDashboard();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            using (frmCustomers customersForm = new frmCustomers())
            {
                customersForm.ShowDialog(this);
            }
        }

        private void btnNewAccounting_Click(object sender, EventArgs e)
        {
            using (frmNewAccounting accountingForm = new frmNewAccounting())
            {
                if (accountingForm.ShowDialog(this) == DialogResult.OK)
                {
                    Report();
                }
            }
        }

        private void btnReportPay_Click(object sender, EventArgs e)
        {
            using (frmReport reportForm = new frmReport())
            {
                reportForm.TypeId = (int)TransactionType.Payment;
                reportForm.ShowDialog(this);
            }

            Report();
        }

        private void btnReportReceive_Click(object sender, EventArgs e)
        {
            using (frmReport reportForm = new frmReport())
            {
                reportForm.TypeId = (int)TransactionType.Receipt;
                reportForm.ShowDialog(this);
            }

            Report();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            lblDate.Text = DateTime.Now.ToShortDateString();
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
            Report();
        }

        private void Report()
        {
            ReportViewModel report = DashboardSummaryService.GetCurrentMonthSummary();
            lblPay.Text = report.Pay.ToString("#,0.##");
            lblReceive.Text = report.Receive.ToString("#,0.##");
            lblAccountBalance.Text = report.AccountBalance.ToString("#,0.##");
            lblAccountBalance.ForeColor = report.AccountBalance < 0 ? AppTheme.Danger : AppTheme.Primary;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void ConfigureDashboard()
        {
            Text = "Accounting Manager — Dashboard";
            ClientSize = new Size(900, 600);
            MinimumSize = new Size(820, 560);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            toolStrip1.BackColor = Color.FromArgb(30, 41, 59);
            toolStrip1.ForeColor = Color.White;
            toolStripDropDownButton1.ForeColor = Color.White;
            toolStripDropDownButton1.Text = "Settings";
            btnEditLogin.Text = "Account settings";

            groupBox1.Visible = false;

            Label pageTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 20F),
                ForeColor = AppTheme.Text,
                Location = new Point(24, 66),
                Text = "Overview"
            };
            Label pageSubtitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = AppTheme.MutedText,
                Location = new Point(27, 105),
                Text = "Your financial activity for the current month"
            };

            TableLayoutPanel summaryLayout = new TableLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = AppTheme.Background,
                ColumnCount = 3,
                Location = new Point(20, 145),
                RowCount = 1,
                Size = new Size(ClientSize.Width - 40, 136)
            };
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            summaryLayout.Controls.Add(CreateSummaryCard("Receipts", lblReceive, AppTheme.Success), 0, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Payments", lblPay, AppTheme.Danger), 1, 0);
            summaryLayout.Controls.Add(CreateSummaryCard("Balance", lblAccountBalance, AppTheme.Primary), 2, 0);

            Panel quickActions = CreateQuickActionsPanel();
            quickActions.Location = new Point(24, 315);
            quickActions.Size = new Size(ClientSize.Width - 48, 168);
            quickActions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Controls.Add(pageTitle);
            Controls.Add(pageSubtitle);
            Controls.Add(summaryLayout);
            Controls.Add(quickActions);
            pageTitle.BringToFront();
            pageSubtitle.BringToFront();
            summaryLayout.BringToFront();
            quickActions.BringToFront();

            lblDate.Spring = true;
            lblDate.TextAlign = ContentAlignment.MiddleLeft;
        }

        private Panel CreateSummaryCard(string title, Label valueLabel, Color accentColor)
        {
            Panel card = new Panel
            {
                BackColor = AppTheme.Surface,
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            Panel accent = new Panel
            {
                BackColor = accentColor,
                Dock = DockStyle.Top,
                Height = 4
            };
            Label titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 9.5F),
                ForeColor = AppTheme.MutedText,
                Height = 42,
                Padding = new Padding(18, 16, 0, 0),
                Text = title
            };

            valueLabel.AutoSize = false;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI Semibold", 18F);
            valueLabel.ForeColor = accentColor;
            valueLabel.Padding = new Padding(16, 0, 16, 10);
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(accent);
            return card;
        }

        private Panel CreateQuickActionsPanel()
        {
            Panel panel = new Panel
            {
                BackColor = AppTheme.Surface,
                Padding = new Padding(18)
            };
            Label title = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = AppTheme.Text,
                Height = 34,
                Text = "Quick actions"
            };
            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                BackColor = AppTheme.Surface,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 8, 0, 0),
                WrapContents = false
            };

            actions.Controls.Add(CreateActionButton("Customers", AppIcon.Customers, AppTheme.Primary, btnCustomers_Click));
            actions.Controls.Add(CreateActionButton("New transaction", AppIcon.AddTransaction, AppTheme.Primary, btnNewAccounting_Click));
            actions.Controls.Add(CreateActionButton("Receipts", AppIcon.Receipt, AppTheme.Success, btnReportReceive_Click));
            actions.Controls.Add(CreateActionButton("Payments", AppIcon.Payment, AppTheme.Danger, btnReportPay_Click));

            panel.Controls.Add(actions);
            panel.Controls.Add(title);
            return panel;
        }

        private static Button CreateActionButton(string text, AppIcon icon, Color color, EventHandler clickHandler)
        {
            Button button = new Button
            {
                BackColor = AppTheme.Surface,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F),
                ForeColor = AppTheme.Text,
                Height = 72,
                Image = IconFactory.Create(icon, color, 30),
                ImageAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 12, 0),
                Padding = new Padding(14, 0, 10, 0),
                Size = new Size(174, 72),
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderColor = AppTheme.Border;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);
            button.Click += clickHandler;
            return button;
        }

        private void btnEditLogin_Click(object sender, EventArgs e)
        {
            using (frmLogin loginForm = new frmLogin())
            {
                loginForm.IsEditMode = true;
                loginForm.ShowDialog(this);
            }
        }
    }
}
