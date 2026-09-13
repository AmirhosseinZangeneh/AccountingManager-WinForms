using AccountingManager.App.Infrastructure;
using System;
using System.Windows.Forms;

namespace AccountingManager.App.UI
{
    internal static class UserMessages
    {
        public static void ShowError(Exception exception, string context)
        {
            AppLogger.Error(exception, context);
            MessageBox.Show(
                "The operation could not be completed. Please try again. Technical details were saved to the application log.",
                "Accounting Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void ShowInformation(string message)
        {
            MessageBox.Show(message, "Accounting Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool ConfirmDelete(string itemName)
        {
            string message = string.IsNullOrWhiteSpace(itemName)
                ? "Are you sure you want to delete the selected item?"
                : "Are you sure you want to delete " + itemName + "?";

            return MessageBox.Show(
                message,
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }
    }
}
