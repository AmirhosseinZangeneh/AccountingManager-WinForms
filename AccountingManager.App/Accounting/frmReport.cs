using AccountingManager.App.UI;
using AccountingManager.Business;
using AccountingManager.DataLayer;
using AccountingManager.DataLayer.Context;
using AccountingManager.ViewModels.Customers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace AccountingManager.App
{
    public partial class frmReport : Form
    {
        private int _printRowIndex;

        public int TypeId { get; set; }
        public frmReport()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            ClientSize = new Size(840, 560);
            MinimumSize = new Size(760, 540);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            groupBox1.Location = new Point(16, 82);
            groupBox1.Size = new Size(808, 64);
            cbCustomer.Size = new Size(180, 23);
            label2.Location = new Point(292, 25);
            txtFromDate.Location = new Point(362, 22);
            label3.Location = new Point(476, 25);
            txtToDate.Location = new Point(534, 22);
            btnFilter.Location = new Point(680, 17);
            btnFilter.Size = new Size(110, 34);

            dgReport.Location = new Point(16, 162);
            dgReport.Size = new Size(808, 382);
            dgReport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                List<ListCustomerViewModel> list = new List<ListCustomerViewModel>();
                list.Add(new ListCustomerViewModel()
                {
                    CustomerId = 0,
                    FullName = "Select"
                });
                list.AddRange(unitOfWork.CustomerRepository.GetNameCustomers());
                cbCustomer.DataSource = list;
                cbCustomer.DisplayMember = "FullName";
                cbCustomer.ValueMember = "CustomerId";
            }
            if (TypeId == (int)TransactionType.Receipt)
            {
                this.Text = "Receipts Report";
            }
            else
            {
                this.Text = "Payments Report";
            }

            Filter();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            DateTime? startDate;
            DateTime? endDate;
            if (!TryReadDate(txtFromDate, "from", out startDate)
                || !TryReadDate(txtToDate, "to", out endDate))
            {
                return;
            }

            if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
            {
                UserMessages.ShowInformation("The from date cannot be later than the to date.");
                return;
            }

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                int customerId = Convert.ToInt32(cbCustomer.SelectedValue);
                IEnumerable<Accounting> result = unitOfWork.AccountingRepository.Get(
                    transaction => transaction.TypeId == TypeId
                        && (customerId == 0 || transaction.CustomerId == customerId));

                if (startDate.HasValue)
                {
                    result = result.Where(transaction => transaction.DateTitle >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    DateTime exclusiveEndDate = endDate.Value.Date.AddDays(1);
                    result = result.Where(transaction => transaction.DateTitle < exclusiveEndDate);
                }

                Dictionary<int, string> customerNames = unitOfWork.CustomerRepository
                    .GetAllCustomers()
                    .ToDictionary(customer => customer.CustomerId, customer => customer.FullName);

                dgReport.Rows.Clear();
                foreach (Accounting accounting in result.OrderByDescending(item => item.DateTitle))
                {
                    string customerName;
                    if (!customerNames.TryGetValue(accounting.CustomerId, out customerName))
                    {
                        customerName = "Unknown customer";
                    }

                    dgReport.Rows.Add(accounting.Id, customerName, accounting.Amount, accounting.DateTitle, accounting.Description);
                }
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgReport.CurrentRow != null)
            {
                int id = int.Parse(dgReport.CurrentRow.Cells[0].Value.ToString());
                if (UserMessages.ConfirmDelete("the selected transaction"))
                {
                    using (UnitOfWork unitOfWork = new UnitOfWork())
                    {
                        unitOfWork.AccountingRepository.Delete(id);
                        unitOfWork.Save();
                        Filter();
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (dgReport.CurrentRow != null)
            {
                int id = int.Parse(dgReport.CurrentRow.Cells[0].Value.ToString());
                using (frmNewAccounting transactionForm = new frmNewAccounting())
                {
                    transactionForm.AccountId = id;
                    if (transactionForm.ShowDialog(this) == DialogResult.OK)
                    {
                        Filter();
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgReport.Rows.Count == 0)
            {
                UserMessages.ShowInformation("There are no rows to print.");
                return;
            }

            _printRowIndex = 0;
            using (PrintDocument document = new PrintDocument())
            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                document.DocumentName = TypeId == (int)TransactionType.Receipt ? "Receipts report" : "Payments report";
                document.PrintPage += PrintDocument_PrintPage;
                preview.Document = document;
                preview.Width = 1000;
                preview.Height = 700;
                preview.ShowDialog(this);
            }

            _printRowIndex = 0;
        }

        private static bool TryReadDate(MaskedTextBox input, string label, out DateTime? value)
        {
            value = null;
            if (!input.MaskCompleted)
            {
                if (!string.IsNullOrWhiteSpace(input.Text.Replace("/", string.Empty)))
                {
                    UserMessages.ShowInformation("Enter a valid " + label + " date.");
                    return false;
                }

                return true;
            }

            DateTime parsedDate;
            if (!DateTime.TryParse(input.Text, out parsedDate))
            {
                UserMessages.ShowInformation("Enter a valid " + label + " date.");
                return false;
            }

            value = parsedDate;
            return true;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            const int rowHeight = 28;
            int left = e.MarginBounds.Left;
            int top = e.MarginBounds.Top;
            int width = e.MarginBounds.Width;

            using (Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Font headerFont = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Font rowFont = new Font("Segoe UI", 9))
            using (Pen separator = new Pen(Color.LightGray))
            {
                string title = TypeId == (int)TransactionType.Receipt ? "Receipts report" : "Payments report";
                e.Graphics.DrawString(title, titleFont, Brushes.Black, left, top);
                top += 40;

                DrawPrintRow(e.Graphics, headerFont, left, top, width, "Customer", "Amount", "Date", "Description");
                top += rowHeight;

                while (_printRowIndex < dgReport.Rows.Count)
                {
                    if (top + rowHeight > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    DataGridViewRow row = dgReport.Rows[_printRowIndex];
                    DrawPrintRow(
                        e.Graphics,
                        rowFont,
                        left,
                        top,
                        width,
                        CellText(row, 1),
                        CellText(row, 2),
                        CellText(row, 3),
                        CellText(row, 4));

                    e.Graphics.DrawLine(separator, left, top + rowHeight - 3, left + width, top + rowHeight - 3);
                    top += rowHeight;
                    _printRowIndex++;
                }
            }

            e.HasMorePages = false;
        }

        private static void DrawPrintRow(Graphics graphics, Font font, int left, int top, int width, string customer, string amount, string date, string description)
        {
            int customerWidth = width * 28 / 100;
            int amountWidth = width * 18 / 100;
            int dateWidth = width * 22 / 100;
            int descriptionWidth = width - customerWidth - amountWidth - dateWidth;

            graphics.DrawString(customer, font, Brushes.Black, new RectangleF(left, top, customerWidth, 24));
            graphics.DrawString(amount, font, Brushes.Black, new RectangleF(left + customerWidth, top, amountWidth, 24));
            graphics.DrawString(date, font, Brushes.Black, new RectangleF(left + customerWidth + amountWidth, top, dateWidth, 24));
            graphics.DrawString(description, font, Brushes.Black, new RectangleF(left + customerWidth + amountWidth + dateWidth, top, descriptionWidth, 24));
        }

        private static string CellText(DataGridViewRow row, int cellIndex)
        {
            object value = row.Cells[cellIndex].Value;
            return value == null ? string.Empty : Convert.ToString(value);
        }
    }
}
