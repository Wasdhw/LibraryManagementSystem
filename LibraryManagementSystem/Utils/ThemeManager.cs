using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagementSystem.Utils
{
    public static class ThemeManager
    {
        // Color scheme
        public static readonly Color PrimaryColor = Color.FromArgb(14, 128, 87);
        public static readonly Color PrimaryHoverColor = Color.FromArgb(20, 150, 100);
        public static readonly Color SecondaryColor = Color.FromArgb(220, 53, 69);
        public static readonly Color BackgroundColor = SystemColors.Control;
        public static readonly Color CardBackgroundColor = SystemColors.ButtonHighlight;

        // Typography
        public static readonly Font HeadingFont = new Font("Tahoma", 15.75F, FontStyle.Bold);
        public static readonly Font SubheadingFont = new Font("Tahoma", 12F, FontStyle.Regular);
        public static readonly Font BodyFont = new Font("Tahoma", 9.75F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Arial Narrow", 9.75F, FontStyle.Regular);

        // Spacing (8px grid)
        public static readonly int SpacingXS = 4;
        public static readonly int SpacingS = 8;
        public static readonly int SpacingM = 16;
        public static readonly int SpacingL = 24;
        public static readonly int SpacingXL = 32;

        /// <summary>
        /// Applies standard button styling
        /// </summary>
        public static void StyleButton(Button button, bool isPrimary = true)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = isPrimary ? PrimaryColor : Color.Gray;
            button.ForeColor = Color.White;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.MouseOverBackColor = PrimaryHoverColor;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 100, 70);
        }

        /// <summary>
        /// Applies standard panel styling
        /// </summary>
        public static void StylePanel(Panel panel, bool hasBorder = true)
        {
            panel.BackColor = CardBackgroundColor;
            if (hasBorder)
            {
                panel.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary>
        /// Applies standard DataGridView styling
        /// </summary>
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = BackgroundColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            
            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial Rounded MT Bold", 11.25F, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            // Row style
            dgv.DefaultCellStyle.BackColor = CardBackgroundColor;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = PrimaryHoverColor;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = BodyFont;
            
            // Alternating rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = BackgroundColor;
            
            // Grid lines
            dgv.GridColor = Color.LightGray;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }
    }
}

