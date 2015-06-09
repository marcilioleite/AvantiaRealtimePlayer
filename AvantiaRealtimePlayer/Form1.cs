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

        PoolOfVideoStreamReaders pool = PoolOfVideoStreamReaders.Instance;

        public Form1()
        {
            InitializeComponent();

            var input = "rtsp://admin:4321@10.20.50.26/profile2/media.smp";
            var input2 = "rtsp://admin:4321@10.20.50.73/profile4/media.smp";

            pool.Dive(input);
            pool.Dive(input2);

            var fbfReader = (FrameByFrame)pool.Joined[input];
            fbfReader.GrabFrame += OnGrabFrame;

            var fbfReader2 = (FrameByFrame)pool.Joined[input2];
            fbfReader2.GrabFrame += OnGrabFrame2;
        }

        private void OnGrabFrame2(object sender, GrabFrameEventArgs e)
        {
            try
            {
                pictureBox2.Image = e.Frame;
            }
            catch (InvalidOperationException)
            { }
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
            pool.Close();
        }
    }
}
