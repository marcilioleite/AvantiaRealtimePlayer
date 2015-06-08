using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvantiaRealtimePlayer
{
    public class FFmpegFrameReader
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public Bitmap Frame { get; set; }
        public Process Process { get; set; }

        public EventHandler FrameChange;
        private Task task; // Task que indica o status de completo do process
        private FileSystemWatcher fileWatcher; // Observer do arquivo do frame
        
        public FFmpegFrameReader(string input, string output)
        {
            Input = input;
            Output = output;
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(output);
            fileWatcher.Filter = Path.GetFileName(output);
            fileWatcher.Changed += (sender, args) =>
            {
                var fs = new FileStream(output, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                try
                {
                    Frame = new Bitmap(fs);
                    EventHandler handler = FrameChange;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
                catch (ArgumentException)
                {
                }
            };
            fileWatcher.EnableRaisingEvents = true;
            task = RunAsync();
        }

        public Task RunAsync(string program = "ffmpeg")
        {
            var tcs = new TaskCompletionSource<bool>();

            var arguments = "-i {0} -f image2 -update 1 -y {1}";

            var processStartInfo = new ProcessStartInfo(program, string.Format(arguments, Input, Output))
            {
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = false,
            };

            // Configura o process
            Process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };

            // Registra evento
            Process.Exited += (sender, args) =>
            {
                MessageBox.Show("Processo finalizado inesperadamente");
                tcs.SetResult(true);
                Process.Dispose();
            };

            Process.Start();
            
            return tcs.Task;
        }
    }
}
