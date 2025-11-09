using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Spritesheet2Gif
{
    public partial class Form1 : Form
    {
        Graphics canvasGraphics;
        IntPtr canvasHdc;
        bool autoplay;
        bool dbgAutoOpen;

        public Form1()
        {
            InitializeComponent();

            canvasGraphics = canvas.CreateGraphics();
            canvasHdc = canvasGraphics.GetHdc();
            if (Native.Initialize(canvas.Width, canvas.Height, this.Handle, canvasHdc) == 0)
            {
                this.Load += ErrorOnLoad;
            }

            autoplay = false;

            saveToolStripMenuItem.Enabled = false;
            zoomInToolStripMenuItem.Enabled = false;
            zoomOutToolStripMenuItem.Enabled = false;
            resetZoomToolStripMenuItem.Enabled = false;
            refreshToolStripMenuItem.Enabled = false;

            dbgAutoOpen = true;

#if DEBUG
            if (dbgAutoOpen)
            {
                if (Native.AutoOpenDocument(this.Handle) != 0)
                {
                    AfterOpen_EnableUI();
                }
                else
                {
                    MessageBox.Show("An error occurred when opening the document.");
                }
            }
#endif
        }
        private void AfterOpen_EnableUI()
        {
            int targetWidth = Native.GetTargetWidth();
            if (targetWidth == 0)
                return;

            int targetHeight = Native.GetTargetHeight();
            if (targetHeight == 0)
                return;

            this.targetWidth.Text = targetWidth.ToString();
            this.targetWidth.Enabled = false;

            this.targetHeight.Text = targetHeight.ToString();
            this.targetHeight.Enabled = false;

            previousBtn.Enabled = true;
            nextBtn.Enabled = true;
            autoplayBtn.Enabled = true;
            gifSpeed.Enabled = true;

            gifSpeed.Minimum = 10;
            gifSpeed.Maximum = 5000;
            gifSpeed.Value = 100;
            gifSpeed.Increment = 10;

            saveToolStripMenuItem.Enabled = true;
            zoomInToolStripMenuItem.Enabled = true;
            zoomOutToolStripMenuItem.Enabled = true;
            resetZoomToolStripMenuItem.Enabled = true;
            refreshToolStripMenuItem.Enabled = true;

            loopForever.Enabled = true;
            loopNTimes.Enabled = true;
            loopCount.Enabled = true;
            loopCount.Minimum = 1;
            loopCount.Maximum = 999999;
            loopCount.Value = 1;
            loopForever.Select();

            Native.Paint();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Native.OpenDocument(this.Handle) != 0)
            {
                AfterOpen_EnableUI();
            }
            else
            {
                MessageBox.Show("An error occurred when opening the document.");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void ErrorOnLoad(object sender, EventArgs e)
        {
            MessageBox.Show("An error occurred while initializing Direct2D.", "Error");

            Close();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Native.Paint();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Native.Uninitialize();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Native.OnResize(canvas.Width, canvas.Height, canvasHdc);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                Native.PreviousFrame(this.Handle);
                Native.Paint();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Native.NextFrame(this.Handle);
                Native.Paint();
            }
        }

        void ForceStopAutoplay()
        {
            if (autoplay)
            {
                autoplayBtn.Text = "Autoplay: off";
                Native.SetAutoplay(canvas.Handle, 0, 0);
                autoplay = false;
            }
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            ForceStopAutoplay();
            Native.PreviousFrame(this.Handle);
            Native.Paint();
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            ForceStopAutoplay();
            Native.NextFrame(this.Handle);
            Native.Paint();
        }

        private void AutoplayBtn_Click(object sender, EventArgs e)
        {
            autoplay = !autoplay;
            if (autoplay)
            {
                autoplayBtn.Text = "Autoplay: on";
            }
            else
            {
                autoplayBtn.Text = "Autoplay: off";
            }

            Native.SetAutoplay(canvas.Handle, autoplay ? 1 : 0, (int)gifSpeed.Value);
        }

        private void AutoplaySpeed_ValueChanged(object sender, EventArgs e)
        {
            Native.SetAutoplaySpeed(canvas.Handle, (int)gifSpeed.Value);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int loopCountLowLevel = 0;
            if (loopNTimes.Checked)
            {
                loopCountLowLevel = (int)loopCount.Value;
            }

            int scaleFactor = (int)(scaleFactorNumericUpDown.Value);

            Native.SaveGif(this.Handle, (int)gifSpeed.Value, loopCountLowLevel, scaleFactor);
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Native.ZoomIn(this.Handle);
            Native.Paint();
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Native.ZoomOut(this.Handle);
            Native.Paint();
        }

        private void ResetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Native.ResetZoom(this.Handle);
            Native.Paint();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Native.ReloadImage();
            Native.Paint();
        }
    }
}

class Native
{
    [DllImport("Native.dll")]
    public static extern Byte Initialize(int clientWidth, int clientHeight, IntPtr parentDialog, IntPtr handle);
    [DllImport("Native.dll")]
    public static extern void OnResize(int clientWidth, int clientHeight, IntPtr handle);

    [DllImport("Native.dll")]
    public static extern void Paint();

    [DllImport("Native.dll")]
    public static extern Byte OpenDocument(IntPtr parentDialog);
    [DllImport("Native.dll")]
    public static extern Byte AutoOpenDocument(IntPtr parentDialog);
    [DllImport("Native.dll")]
    public static extern int GetTargetWidth();
    [DllImport("Native.dll")]
    public static extern int GetTargetHeight();

    [DllImport("Native.dll")]
    public static extern void PreviousFrame(IntPtr parentDialog);

    [DllImport("Native.dll")]
    public static extern void NextFrame(IntPtr parentDialog);

    [DllImport("Native.dll")]
    public static extern void SetAutoplay(IntPtr parentDialog, int value, int gifSpeed);

    [DllImport("Native.dll")]
    public static extern void SetAutoplaySpeed(IntPtr parentDialog, int gifSpeed);

    [DllImport("Native.dll")]
    public static extern Byte SaveGif(IntPtr parentDialog, int animationSpeed, int loopCount, int scaleFactor);

    [DllImport("Native.dll")]
    public static extern void ZoomIn(IntPtr parentDialog);

    [DllImport("Native.dll")]
    public static extern void ZoomOut(IntPtr parentDialog);

    [DllImport("Native.dll")]
    public static extern void ResetZoom(IntPtr parentDialog);

    [DllImport("Native.dll")]
    public static extern void ReloadImage();

    [DllImport("Native.dll")]
    public static extern void Uninitialize();
}