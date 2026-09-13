using AccountingManager.App.UI;
using AccountingManager.Business;
using System;
using System.Drawing;
using System.Windows.Forms;
using ValidationComponents;

namespace AccountingManager.App
{
    public partial class frmLogin : Form
    {
        private readonly AuthenticationService _authenticationService = new AuthenticationService();

        public bool IsEditMode { get; set; }
        public frmLogin()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            ConfigureLayout();
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            if (BaseValidator.IsFormValid(this.components))
            {
                try
                {
                    if (IsEditMode)
                    {
                        _authenticationService.UpdateCredentials(txtUserName.Text, txtPassword.Text);
                        Application.Restart();
                    }
                    else if (_authenticationService.Authenticate(txtUserName.Text, txtPassword.Text))
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception exception)
                {
                    UserMessages.ShowError(exception, "Sign in or credential update");
                }
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            if (IsEditMode)
            {
                this.Text = "Login Options";
                btnlogin.Text = "Save Changes";
                txtUserName.Text = _authenticationService.GetUserName();
                txtPassword.Clear();
            }
        }

        private void ConfigureLayout()
        {
            Text = "Sign in — Accounting Manager";
            ClientSize = new Size(420, 250);
            AcceptButton = btnlogin;

            groupBox1.Text = "Account credentials";
            groupBox1.Location = new Point(24, 24);
            groupBox1.Size = new Size(372, 142);

            label1.Location = new Point(22, 38);
            label2.Location = new Point(22, 84);
            txtUserName.Location = new Point(118, 34);
            txtPassword.Location = new Point(118, 80);
            txtUserName.Size = new Size(224, 23);
            txtPassword.Size = new Size(224, 23);
            txtPassword.UseSystemPasswordChar = true;

            btnlogin.Location = new Point(24, 184);
            btnlogin.Size = new Size(372, 40);
        }
    }
}
