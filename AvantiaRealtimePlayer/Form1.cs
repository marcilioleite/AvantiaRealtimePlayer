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
        private FFmpegFrameReader FFmpegFrameReader;

        public Form1()
        {
            InitializeComponent();

            var input = "rtsp://admin:4321@10.20.50.26/profile2/media.smp";
            var output = "C:/Users/marcilio.leite/Desktop/tmp/frame.png";
            FFmpegFrameReader = new FFmpegFrameReader(input, output);
            FFmpegFrameReader.FrameChange += OnFrameChange;
        }

        private void OnFrameChange(object sender, EventArgs e)
        {
            pictureBox1.Image = FFmpegFrameReader.Frame;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!FFmpegFrameReader.Process.HasExited)
            {
                FFmpegFrameReader.Process.Kill();
            }
        }
    }
}
