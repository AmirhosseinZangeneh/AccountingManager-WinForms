using AccountingManager.App.UI;
using AccountingManager.DataLayer;
using AccountingManager.DataLayer.Context;
using System;
using System.IO;
using System.Windows.Forms;
using ValidationComponents;

namespace AccountingManager.App
{
    public partial class frmAddOrEditCustomer : Form
    {
        private string _selectedImagePath;
        private string _existingImageName;
        private readonly System.Drawing.Image _placeholderImage;

        public int CustomerId { get; set; }
        public frmAddOrEditCustomer()
        {
            InitializeComponent();
            AppTheme.Apply(this);
            ConfigureLayout();
            AcceptButton = btnSave;
            pcCustomer.SizeMode = PictureBoxSizeMode.Zoom;
            _placeholderImage = IconFactory.CreateAvatarPlaceholder(pcCustomer.Width, pcCustomer.Height);
            pcCustomer.Image = _placeholderImage;
            FormClosed += (sender, args) => _placeholderImage.Dispose();
        }

        private void ConfigureLayout()
        {
            ClientSize = new System.Drawing.Size(700, 480);
            MinimumSize = new System.Drawing.Size(716, 519);

            groupBox2.Location = new System.Drawing.Point(20, 20);
            groupBox2.Size = new System.Drawing.Size(420, 376);
            groupBox2.Text = "Personal information";

            label1.Location = new System.Drawing.Point(20, 32);
            label1.Text = "Name";
            txtName.Location = new System.Drawing.Point(20, 55);
            txtName.Size = new System.Drawing.Size(380, 25);

            label2.Location = new System.Drawing.Point(20, 96);
            label2.Text = "Mobile";
            txtMobile.Location = new System.Drawing.Point(20, 119);
            txtMobile.Size = new System.Drawing.Size(380, 25);

            label3.Location = new System.Drawing.Point(20, 160);
            label3.Text = "Email";
            txtEmail.Location = new System.Drawing.Point(20, 183);
            txtEmail.Size = new System.Drawing.Size(380, 25);

            label4.Location = new System.Drawing.Point(20, 224);
            label4.Text = "Address";
            txtAddress.Location = new System.Drawing.Point(20, 247);
            txtAddress.Size = new System.Drawing.Size(380, 95);

            groupBox1.Location = new System.Drawing.Point(460, 20);
            groupBox1.Size = new System.Drawing.Size(220, 376);
            groupBox1.Text = "Profile image";

            pcCustomer.Location = new System.Drawing.Point(15, 32);
            pcCustomer.Size = new System.Drawing.Size(190, 260);
            btnSelectPhoto.Location = new System.Drawing.Point(15, 310);
            btnSelectPhoto.Size = new System.Drawing.Size(190, 42);
            btnSelectPhoto.Text = "Choose image";

            btnSave.Location = new System.Drawing.Point(20, 416);
            btnSave.Size = new System.Drawing.Size(660, 42);
            btnSave.Text = "Save customer";
            btnSave.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        }

        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "Image files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    _selectedImagePath = openFile.FileName;
                    pcCustomer.Image = null;
                    pcCustomer.ImageLocation = openFile.FileName;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (BaseValidator.IsFormValid(this.components))
            {
                try
                {
                    string imageName = SaveSelectedImage();
                    Customers customer = new Customers()
                    {
                        Address = txtAddress.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        FullName = txtName.Text.Trim(),
                        Mobile = txtMobile.Text.Trim(),
                        CustomerImage = imageName
                    };
                    using (UnitOfWork unitOfWork = new UnitOfWork())
                    {
                        if (CustomerId == 0)
                        {
                            unitOfWork.CustomerRepository.InsertCustomer(customer);
                        }
                        else
                        {
                            customer.CustomerId = CustomerId;
                            unitOfWork.CustomerRepository.UpdateCustomer(customer);
                        }

                        unitOfWork.Save();
                    }

                    DialogResult = DialogResult.OK;
                }
                catch (Exception exception)
                {
                    UserMessages.ShowError(exception, "Save customer");
                }
            }
        }

        private void frmAddOrEditCustomer_Load(object sender, EventArgs e)
        {
            if (CustomerId != 0)
            {
                this.Text = "Edit Person";
                btnSave.Text = "Save changes";
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    var customer = unitOfWork.CustomerRepository.GetCustomerById(CustomerId);
                    if (customer == null)
                    {
                        UserMessages.ShowInformation("The selected customer no longer exists.");
                        Close();
                        return;
                    }

                    txtEmail.Text = customer.Email;
                    txtAddress.Text = customer.Address;
                    txtMobile.Text = customer.Mobile;
                    txtName.Text = customer.FullName;
                    _existingImageName = customer.CustomerImage;

                    string imagePath = Path.Combine(Application.StartupPath, "Images", _existingImageName ?? string.Empty);
                    if (File.Exists(imagePath))
                    {
                        pcCustomer.Image = null;
                        pcCustomer.ImageLocation = imagePath;
                    }
                }
            }
        }

        private string SaveSelectedImage()
        {
            if (string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                return string.IsNullOrWhiteSpace(_existingImageName)
                    ? "no-profile-image.gif"
                    : _existingImageName;
            }

            string imagesDirectory = Path.Combine(Application.StartupPath, "Images");
            Directory.CreateDirectory(imagesDirectory);

            FileInfo selectedImage = new FileInfo(_selectedImagePath);
            if (selectedImage.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("The selected image must be smaller than 5 MB.");
            }

            string imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(_selectedImagePath).ToLowerInvariant();
            File.Copy(_selectedImagePath, Path.Combine(imagesDirectory, imageName), true);

            return imageName;
        }
    }
}
