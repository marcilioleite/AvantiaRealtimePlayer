using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantiaRealtimePlayer.Readers
{
    public class FrameByFrameFactory : IVideoStreamReaderFactory
    {
        #region Singleton

        private static FrameByFrameFactory mInstance { get; set; }

        public static FrameByFrameFactory Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new FrameByFrameFactory();
                }
                return mInstance;
            }
        }

        private FrameByFrameFactory() { }

        #endregion

        public IVideoStreamReader Create(string input)
        {
            return new FrameByFrame(input);
        }
    }
}
