using AvantiaRealtimePlayer.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvantiaRealtimePlayer
{
    public partial class Form1 : Form
    {
        private FrameByFrame mFrameByFrameReader;

        public Form1()
        {
            InitializeComponent();

            var input = "rtsp://admin:4321@10.20.50.26/profile2/media.smp";

            mFrameByFrameReader = (FrameByFrame) FrameByFrameFactory
                .Instance.Create(input);
            
            mFrameByFrameReader.GrabFrame += OnGrabFrame;
        }

        private void OnGrabFrame(object sender, GrabFrameEventArgs e)
        {
            try
            {
                pictureBox1.Image = e.Frame;
            }
            catch (InvalidOperationException)
            { }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!mFrameByFrameReader.FFmpegProcess.HasExited)
            {
                mFrameByFrameReader.FFmpegProcess.Kill();
            }
        }
    }
}
