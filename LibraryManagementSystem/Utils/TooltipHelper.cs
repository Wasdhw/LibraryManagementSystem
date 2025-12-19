using System;
using System.Windows.Forms;

namespace LibraryManagementSystem.Utils
{
    public static class TooltipHelper
    {
        private static ToolTip tooltip = new ToolTip();

        static TooltipHelper()
        {
            tooltip.IsBalloon = true;
            tooltip.ToolTipIcon = ToolTipIcon.Info;
            tooltip.ToolTipTitle = "Help";
            tooltip.AutoPopDelay = 5000;
            tooltip.InitialDelay = 500;
            tooltip.ReshowDelay = 500;
        }

        public static void SetTooltip(Control control, string text)
        {
            tooltip.SetToolTip(control, text);
        }

        public static void SetTooltip(Control control, string text, string title)
        {
            tooltip.ToolTipTitle = title;
            tooltip.SetToolTip(control, text);
        }
    }
}

