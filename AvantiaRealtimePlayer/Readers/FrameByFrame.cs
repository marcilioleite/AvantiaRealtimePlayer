using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvantiaRealtimePlayer.Readers
{
    /// <summary>
    /// Responsável por iniciar um processo do
    /// FFmpeg com argumentos para salvar cada 
    /// frame de um streaming RTSP como um arquivo
    /// do diretório temporário do sistema.
    /// </summary>
    public class FrameByFrame : IVideoStreamReader
    {
        #region Public Properties
        
        public string Input { get; set; }
        public string Output { get; set; }
        public bool DiscardBadFrames { get; set; }
        public Bitmap CurrentFrame { get; set; }
        public Process FFmpegProcess { get; set; }
        
        #endregion

        #region Private Member Variables
        
        private Task mTask;
        private FileSystemWatcher mFileWatcher;
        
        #endregion

        #region Event Handlers

        public EventHandler<GrabFrameEventArgs> GrabFrame;
        public EventHandler Error; 

        #endregion

        public FrameByFrame(string input)
        {
            Input = input;

            var tempFolder = Path.GetTempPath();
            var fileName = Path.GetRandomFileName();
            Output = string.Format("{0}\\{1}.png", tempFolder, fileName);

            mFileWatcher = new FileSystemWatcher();
            mFileWatcher.Path = Path.GetDirectoryName(Output);
            mFileWatcher.Filter = Path.GetFileName(Output);
            mFileWatcher.Changed += (sender, args) =>
            {
                var fileStream = new FileStream(Output, FileMode.Open, 
                    FileAccess.Read, FileShare.ReadWrite);

                try
                {
                    CurrentFrame = new Bitmap(fileStream);
                    EventHandler<GrabFrameEventArgs> handler = GrabFrame;
                    if (handler != null)
                    {
                        GrabFrameEventArgs eventArgs = new GrabFrameEventArgs
                        {
                            Frame = CurrentFrame
                        };
                        handler(this, eventArgs);
                    }
                }
                catch (ArgumentException)
                {
                    EventHandler handler = Error;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            };

            mFileWatcher.EnableRaisingEvents = true;
            mTask = RunFFmpeg();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public Task RunFFmpeg()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var arguments = "-i {0} -f image2 -update 1 -y {1}";

            var processStartInfo = new ProcessStartInfo("ffmpeg", 
                string.Format(arguments, Input, Output))
            {
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = true,
            };

            FFmpegProcess = new Process { StartInfo = processStartInfo, 
                EnableRaisingEvents = true };

            FFmpegProcess.Exited += (sender, args) =>
            {
                EventHandler handler = Error;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                taskCompletionSource.SetResult(true);
                FFmpegProcess.Dispose();
            };

            FFmpegProcess.Start();
            
            return taskCompletionSource.Task;
        }

        public void Stop()
        {
            FFmpegProcess.Kill();
        }
    }

    /// <summary>
    /// Argumentos de Evento GragFrame
    /// 
    /// Carrega o frame atual do Reader.
    /// </summary>
    public class GrabFrameEventArgs : EventArgs
    {
        public Bitmap Frame { get; set; }
    }
}
