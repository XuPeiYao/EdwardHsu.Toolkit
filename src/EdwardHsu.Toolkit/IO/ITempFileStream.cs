using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EdwardHsu.Toolkit.IO
{
    public interface ITempFileStream
    {
        public TempFileStreamStatus Status { get; }
        public bool AutoFlush { get; set; }
        public void OpenFile();
        public ValueTask OpenFileAsync();
        public void CloseFile();
        public ValueTask CloseFileAsync();
    }
}
