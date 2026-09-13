using System.Drawing;
using System.Drawing.Drawing2D;

namespace AccountingManager.App.UI
{
    internal enum AppIcon
    {
        Customers,
        AddCustomer,
        AddTransaction,
        Receipt,
        Payment,
        Edit,
        Delete,
        Refresh,
        Print
    }

    internal static class IconFactory
    {
        public static Bitmap Create(AppIcon icon, Color color, int size = 28)
        {
            Bitmap bitmap = new Bitmap(size, size);
            bitmap.SetResolution(96F, 96F);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(color, 2.2F))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                switch (icon)
                {
                    case AppIcon.Customers:
                        DrawCustomers(graphics, pen);
                        break;
                    case AppIcon.AddCustomer:
                        DrawAddCustomer(graphics, pen);
                        break;
                    case AppIcon.AddTransaction:
                        DrawAddTransaction(graphics, pen);
                        break;
                    case AppIcon.Receipt:
                        DrawTransfer(graphics, pen, false);
                        break;
                    case AppIcon.Payment:
                        DrawTransfer(graphics, pen, true);
                        break;
                    case AppIcon.Edit:
                        DrawEdit(graphics, pen);
                        break;
                    case AppIcon.Delete:
                        DrawDelete(graphics, pen);
                        break;
                    case AppIcon.Refresh:
                        DrawRefresh(graphics, pen);
                        break;
                    case AppIcon.Print:
                        DrawPrint(graphics, pen);
                        break;
                }
            }

            return bitmap;
        }

        public static Bitmap CreateAvatarPlaceholder(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetResolution(96F, 96F);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (SolidBrush background = new SolidBrush(Color.FromArgb(241, 245, 249)))
            using (SolidBrush silhouette = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.FillRectangle(background, 0, 0, width, height);

                float headSize = width * 0.32F;
                float headX = (width - headSize) / 2F;
                float headY = height * 0.20F;
                graphics.FillEllipse(silhouette, headX, headY, headSize, headSize);

                float bodyWidth = width * 0.68F;
                float bodyHeight = height * 0.40F;
                float bodyX = (width - bodyWidth) / 2F;
                float bodyY = headY + headSize + height * 0.08F;
                graphics.FillEllipse(silhouette, bodyX, bodyY, bodyWidth, bodyHeight);
            }

            return bitmap;
        }

        private static void DrawCustomers(Graphics graphics, Pen pen)
        {
            graphics.DrawEllipse(pen, 9, 4, 10, 10);
            graphics.DrawArc(pen, 5, 15, 18, 12, 190, 160);
            graphics.DrawEllipse(pen, 3, 8, 6, 6);
            graphics.DrawArc(pen, 1, 15, 9, 8, 190, 140);
        }

        private static void DrawAddCustomer(Graphics graphics, Pen pen)
        {
            graphics.DrawEllipse(pen, 5, 4, 9, 9);
            graphics.DrawArc(pen, 2, 14, 16, 11, 190, 160);
            graphics.DrawEllipse(pen, 16, 14, 10, 10);
            graphics.DrawLine(pen, 21, 17, 21, 21);
            graphics.DrawLine(pen, 19, 19, 23, 19);
        }

        private static void DrawAddTransaction(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, 5, 3, 15, 21);
            graphics.DrawLine(pen, 9, 8, 16, 8);
            graphics.DrawLine(pen, 9, 12, 16, 12);
            graphics.DrawEllipse(pen, 16, 15, 10, 10);
            graphics.DrawLine(pen, 21, 18, 21, 22);
            graphics.DrawLine(pen, 19, 20, 23, 20);
        }

        private static void DrawTransfer(Graphics graphics, Pen pen, bool upward)
        {
            graphics.DrawLine(pen, 5, 22, 23, 22);
            graphics.DrawLine(pen, 8, 18, 20, 18);

            if (upward)
            {
                graphics.DrawLine(pen, 14, 17, 14, 5);
                graphics.DrawLine(pen, 14, 5, 9, 10);
                graphics.DrawLine(pen, 14, 5, 19, 10);
            }
            else
            {
                graphics.DrawLine(pen, 14, 4, 14, 16);
                graphics.DrawLine(pen, 14, 16, 9, 11);
                graphics.DrawLine(pen, 14, 16, 19, 11);
            }
        }

        private static void DrawEdit(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, 4, 5, 16, 19);
            graphics.DrawLine(pen, 10, 18, 21, 7);
            graphics.DrawLine(pen, 18, 5, 23, 10);
            graphics.DrawLine(pen, 9, 19, 8, 23);
        }

        private static void DrawDelete(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, 6, 8, 22, 8);
            graphics.DrawLine(pen, 10, 5, 18, 5);
            graphics.DrawRectangle(pen, 8, 9, 12, 15);
            graphics.DrawLine(pen, 12, 13, 12, 20);
            graphics.DrawLine(pen, 16, 13, 16, 20);
        }

        private static void DrawRefresh(Graphics graphics, Pen pen)
        {
            graphics.DrawArc(pen, 5, 5, 18, 18, 35, 235);
            graphics.DrawLine(pen, 5, 8, 5, 15);
            graphics.DrawLine(pen, 5, 8, 12, 8);
            graphics.DrawArc(pen, 5, 5, 18, 18, 215, 80);
            graphics.DrawLine(pen, 23, 20, 16, 20);
            graphics.DrawLine(pen, 23, 20, 23, 13);
        }

        private static void DrawPrint(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, 8, 3, 12, 7);
            graphics.DrawRectangle(pen, 6, 16, 16, 9);
            graphics.DrawRectangle(pen, 3, 9, 22, 11);
            graphics.DrawEllipse(pen, 19, 12, 1, 1);
        }
    }
}
