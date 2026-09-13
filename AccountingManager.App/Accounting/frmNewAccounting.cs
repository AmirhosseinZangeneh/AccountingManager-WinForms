using AccountingManager.App.UI;
using AccountingManager.Business;
using AccountingManager.DataLayer.Context;
using AccountingManager.ViewModels.Customers;
using System;
using System.Windows.Forms;
using ValidationComponents;

namespace AccountingManager.App
{
    public partial class frmNewAccounting : Form
    {
        private int? _selectedCustomerId;

        public int AccountId { get; set; }
        public frmNewAccounting()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            AcceptButton = btnSave;
            btnSave.Size = new System.Drawing.Size(276, 36);
        }

        private void frmNewAccounting_Load(object sender, EventArgs e)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                dgvCustomers.AutoGenerateColumns = false;
                dgvCustomers.DataSource = unitOfWork.CustomerRepository.GetNameCustomers();

                if (AccountId != 0)
                {
                    var account = unitOfWork.AccountingRepository.GetById(AccountId);
                    if (account == null)
                    {
                        UserMessages.ShowInformation("The selected transaction no longer exists.");
                        Close();
                        return;
                    }

                    _selectedCustomerId = account.CustomerId;
                    txtAmount.Value = account.Amount;
                    txtDescription.Text = account.Description;
                    txtName.Text = unitOfWork.CustomerRepository.GetCustomerNameById(account.CustomerId);
                    rbReceive.Checked = account.TypeId == (int)TransactionType.Receipt;
                    rbPay.Checked = account.TypeId == (int)TransactionType.Payment;
                    Text = "Edit transaction";
                    btnSave.Text = "Save changes";
                }
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                dgvCustomers.AutoGenerateColumns = false;
                dgvCustomers.DataSource = unitOfWork.CustomerRepository.GetNameCustomers(txtFilter.Text);
            }
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvCustomers.CurrentRow == null)
            {
                return;
            }

            ListCustomerViewModel customer = dgvCustomers.CurrentRow.DataBoundItem as ListCustomerViewModel;
            if (customer != null)
            {
                _selectedCustomerId = customer.CustomerId;
                txtName.Text = customer.FullName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (BaseValidator.IsFormValid(this.components))
            {
                if (rbPay.Checked || rbReceive.Checked)
                {
                    if (!_selectedCustomerId.HasValue)
                    {
                        UserMessages.ShowInformation("Please select a customer.");
                        return;
                    }

                    DataLayer.Accounting accounting = new DataLayer.Accounting()
                    {
                        Amount = decimal.ToInt32(txtAmount.Value),
                        CustomerId = _selectedCustomerId.Value,
                        TypeId = rbReceive.Checked
                            ? (int)TransactionType.Receipt
                            : (int)TransactionType.Payment,
                        DateTitle = DateTime.Now,
                        Description = txtDescription.Text.Trim(),
                    };
                    try
                    {
                        using (UnitOfWork unitOfWork = new UnitOfWork())
                        {
                            if (AccountId == 0)
                            {
                                unitOfWork.AccountingRepository.Insert(accounting);
                            }
                            else
                            {
                                accounting.Id = AccountId;
                                unitOfWork.AccountingRepository.Update(accounting);
                            }

                            unitOfWork.Save();
                        }

                        DialogResult = DialogResult.OK;
                    }
                    catch (Exception exception)
                    {
                        UserMessages.ShowError(exception, "Save transaction");
                    }
                }
                else
                {
                    UserMessages.ShowInformation("Please select a transaction type.");
                }
            }
        }
    }
}
