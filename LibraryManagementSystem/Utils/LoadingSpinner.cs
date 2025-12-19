using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryManagementSystem.Utils
{
    public partial class LoadingSpinner : UserControl
    {
        private Timer timer;
        private int angle = 0;
        private Color spinnerColor = Color.FromArgb(14, 128, 87);

        public LoadingSpinner()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.Size = new Size(40, 40);
            
            timer = new Timer();
            timer.Interval = 50; // 50ms for smooth animation
            timer.Tick += Timer_Tick;
        }

        public void Start()
        {
            this.Visible = true;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            this.Visible = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            angle += 15;
            if (angle >= 360) angle = 0;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = Math.Min(this.Width, this.Height) / 2 - 5;
            
            for (int i = 0; i < 8; i++)
            {
                int currentAngle = angle + (i * 45);
                float alpha = 1.0f - (i * 0.125f);
                Color color = Color.FromArgb((int)(alpha * 255), spinnerColor);
                
                float x = centerX + (float)(radius * Math.Cos(currentAngle * Math.PI / 180));
                float y = centerY + (float)(radius * Math.Sin(currentAngle * Math.PI / 180));
                
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, x - 3, y - 3, 6, 6);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

