using System.Drawing;
using System.Windows.Forms;

namespace AccountingManager.App.UI
{
    internal static class AppTheme
    {
        public static readonly Color Background = Color.FromArgb(244, 247, 251);
        public static readonly Color Surface = Color.White;
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);
        public static readonly Color PrimaryDark = Color.FromArgb(29, 78, 216);
        public static readonly Color Success = Color.FromArgb(5, 150, 105);
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);
        public static readonly Color Text = Color.FromArgb(15, 23, 42);
        public static readonly Color MutedText = Color.FromArgb(100, 116, 139);
        public static readonly Color Border = Color.FromArgb(226, 232, 240);

        private static readonly Font DefaultFont = new Font("Segoe UI", 9F, FontStyle.Regular);

        public static void Apply(Form form)
        {
            form.Font = DefaultFont;
            form.BackColor = Background;
            form.ForeColor = Text;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.AutoScaleMode = AutoScaleMode.Dpi;

            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button)
                {
                    StyleButton((Button)control);
                }
                else if (control is DataGridView)
                {
                    StyleGrid((DataGridView)control);
                }
                else if (control is GroupBox)
                {
                    StyleGroupBox((GroupBox)control);
                }
                else if (control is TextBoxBase || control is ComboBox || control is NumericUpDown)
                {
                    control.BackColor = Surface;
                    control.ForeColor = Text;
                }
                else if (control is StatusStrip)
                {
                    StyleStatusStrip((StatusStrip)control);
                }
                else if (control is ToolStrip)
                {
                    StyleToolStrip((ToolStrip)control);
                }

                if (control.HasChildren)
                {
                    ApplyToControls(control.Controls);
                }
            }
        }

        private static void StyleButton(Button button)
        {
            bool isDanger = ContainsAny(button.Name, "delete", "remove");
            bool isSecondary = ContainsAny(button.Name, "select", "refresh", "cancel");

            button.AutoSize = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = isSecondary ? 1 : 0;
            button.FlatAppearance.BorderColor = Border;
            button.Cursor = Cursors.Hand;
            button.Height = button.Height < 34 ? 34 : button.Height;
            button.Padding = new Padding(10, 0, 10, 0);

            if (isSecondary)
            {
                button.BackColor = Surface;
                button.ForeColor = Text;
            }
            else
            {
                button.BackColor = isDanger ? Danger : Primary;
                button.ForeColor = Color.White;
            }
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 41, 59);
            grid.ColumnHeadersHeight = 38;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 34;
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
        }

        private static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.BackColor = Surface;
            groupBox.ForeColor = Text;
            groupBox.Padding = new Padding(12);
        }

        private static void StyleToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = Surface;
            toolStrip.ForeColor = Text;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.RenderMode = ToolStripRenderMode.System;
            toolStrip.Padding = new Padding(8, 4, 8, 4);
            toolStrip.Font = DefaultFont;

            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.Margin = new Padding(2);
                item.ForeColor = Text;
                item.ImageScaling = ToolStripItemImageScaling.None;
                ApplyModernIcon(item);
            }
        }

        private static void ApplyModernIcon(ToolStripItem item)
        {
            string name = (item.Name ?? string.Empty).ToLowerInvariant();
            AppIcon icon;
            Color color = Primary;

            if (name.Contains("customers") || name.Contains("customer") && name.Contains("add"))
            {
                icon = name.Contains("add") ? AppIcon.AddCustomer : AppIcon.Customers;
            }
            else if (name.Contains("newaccounting"))
            {
                icon = AppIcon.AddTransaction;
            }
            else if (name.Contains("reportreceive"))
            {
                icon = AppIcon.Receipt;
                color = Success;
            }
            else if (name.Contains("reportpay"))
            {
                icon = AppIcon.Payment;
                color = Danger;
            }
            else if (name.Contains("edit"))
            {
                icon = AppIcon.Edit;
            }
            else if (name.Contains("delete"))
            {
                icon = AppIcon.Delete;
                color = Danger;
            }
            else if (name.Contains("refresh"))
            {
                icon = AppIcon.Refresh;
                color = MutedText;
            }
            else if (name.Contains("print"))
            {
                icon = AppIcon.Print;
            }
            else
            {
                return;
            }

            item.Image = IconFactory.Create(icon, color);
        }

        private static void StyleStatusStrip(StatusStrip statusStrip)
        {
            statusStrip.BackColor = Color.FromArgb(30, 41, 59);
            statusStrip.ForeColor = Color.White;
            statusStrip.SizingGrip = false;
        }

        private static bool ContainsAny(string value, params string[] candidates)
        {
            string normalized = (value ?? string.Empty).ToLowerInvariant();
            foreach (string candidate in candidates)
            {
                if (normalized.Contains(candidate))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
