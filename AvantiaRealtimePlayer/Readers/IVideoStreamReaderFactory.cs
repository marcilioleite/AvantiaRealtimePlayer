using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvantiaRealtimePlayer.Readers
{
    public interface IVideoStreamReaderFactory
    {
        IVideoStreamReader Create(string input);
    }
}
