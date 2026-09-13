using AccountingManager.App.UI;
using AccountingManager.DataLayer.Context;
using System;
using System.Windows.Forms;

namespace AccountingManager.App
{
    public partial class frmCustomers : Form
    {
        public frmCustomers()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            Text = "Customers";
            ClientSize = new System.Drawing.Size(760, 480);
            MinimumSize = new System.Drawing.Size(720, 480);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            txtFilter.AutoSize = false;
            txtFilter.Height = 28;
            dgvCustomers.CellDoubleClick += (sender, args) =>
            {
                if (args.RowIndex >= 0)
                {
                    btnEditCustomer_Click(sender, EventArgs.Empty);
                }
            };
        }

        private void frmCustomers_Load(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                dgvCustomers.AutoGenerateColumns = false;
                dgvCustomers.DataSource = unitOfWork.CustomerRepository.GetAllCustomers();
            }
        }

        private void btnRefreshCustomer_Click(object sender, EventArgs e)
        {
            txtFilter.Text = "";
            BindGrid();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                dgvCustomers.DataSource = unitOfWork.CustomerRepository.GetCustomersByFilter(txtFilter.Text);
            }
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    string name = Convert.ToString(dgvCustomers.CurrentRow.Cells[1].Value);
                    if (UserMessages.ConfirmDelete(name))
                    {
                        int customerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells[0].Value);
                        if (unitOfWork.CustomerRepository.HasTransactions(customerId))
                        {
                            UserMessages.ShowInformation("This customer has financial transactions and cannot be deleted.");
                            return;
                        }

                        if (!unitOfWork.CustomerRepository.DeleteCustomer(customerId))
                        {
                            UserMessages.ShowInformation("The selected customer no longer exists.");
                            return;
                        }

                        unitOfWork.Save();
                        BindGrid();
                    }
                }
            }
            else
            {
                UserMessages.ShowInformation("Please select a customer first.");
            }
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            using (frmAddOrEditCustomer customerForm = new frmAddOrEditCustomer())
            {
                if (customerForm.ShowDialog(this) == DialogResult.OK)
                {
                    BindGrid();
                }
            }
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                int customerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells[0].Value);
                using (frmAddOrEditCustomer customerForm = new frmAddOrEditCustomer())
                {
                    customerForm.CustomerId = customerId;
                    if (customerForm.ShowDialog(this) == DialogResult.OK)
                    {
                        BindGrid();
                    }
                }
            }
        }
    }
}
