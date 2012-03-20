using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/* TODO
 * Make shapes more random (lines always come from the top left down)
 * Make values configurable in .ini file (pen thickness, sound directory, etc)
 * Add about page
 * Keep shapes from going off the screen - they should always be completely inside the window
 * Find icon for application
*/

namespace SmashKey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
            random = new Random();
            soundFiles = Directory.GetFiles(soundPath, "*.wav");
            soundPlayer = new System.Media.SoundPlayer();
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndIntertAfter, int X, int Y, int cx, int cy, int uFlags);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int Which);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private IntPtr HWND_TOP = IntPtr.Zero;
        private const int SWP_SHOWWINDOW = 64;
        private Graphics graphics;
        private Random random;
        private String soundPath = System.Environment.ExpandEnvironmentVariables("%WinDir%") + @"\media\";
        private String[] soundFiles;
        SoundPlayer soundPlayer;

        public int ScreenX
        {
            get
            {
                return GetSystemMetrics(SM_CXSCREEN);
            }
        }

        public int ScreenY
        {
            get
            {
                return GetSystemMetrics(SM_CYSCREEN);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            graphics = this.CreateGraphics();
            if (graphics != null)
            {
                graphics.Clear(Color.White);
            }
        }

        private void FullScreen()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            SetWindowPos(this.Handle, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        }

        private void Restore()
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.TopMost = false;
        }

        private void DrawShape()
        {
            // Get a random color
            int maxC = 255;
            Color color = Color.FromArgb(200,
                random.Next(0, maxC), random.Next(0, maxC), random.Next(0, maxC));

            // Choose random location and size
            int x = random.Next(0, this.Width);
            int y = random.Next(0, this.Height);
                        
            int width = random.Next(20, 200);
            int height = random.Next(20, 200);

            // Play random wav file
            if (soundFiles.Length > 0)
            {
                int i = random.Next(0, soundFiles.Length);
                soundPlayer.SoundLocation = soundFiles[i];
                soundPlayer.Play();
            }

            // Thickness of the pen
            int pWidth = 5;

            // Choose random shape
            int shape = random.Next(0, 4);
            switch (shape)
            {
                case 0:
                    graphics.DrawLine(new Pen(color, pWidth), x, y, width, height);
                    break;
                case 1:
                    graphics.DrawRectangle(new Pen(color, pWidth), x, y, width, height);
                    break;
                case 2:
                    graphics.FillRectangle(new SolidBrush(color), x, y, width, height);
                    break;
                case 3:
                    graphics.DrawEllipse(new Pen(color, pWidth), x, y, width, height);
                    break;
                case 4:
                    graphics.FillEllipse(new SolidBrush(color), x, y, width, height);
                    break;
                default:
                    break;
            }
        }

        private void cmdFullscreen_Click(Object sender, EventArgs e)
        {            
            FullScreen();
        }

        private void cmdRestore_Click(Object sender, EventArgs e)
        {
            Restore();
        }

        private void cmdExit_Click(Object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cmdDraw(Object sender, EventArgs e)
        {
            DrawShape();
        }

        private void cmdDraw(Object sender, KeyEventArgs e)
        {
            DrawShape();
        }
    }
}