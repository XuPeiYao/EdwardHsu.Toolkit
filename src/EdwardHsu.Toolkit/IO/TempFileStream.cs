using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EdwardHsu.Toolkit.IO
{
    public class TempFileStream : Stream, ITempFileStream
    {
        public const int DEFAULT_BUFFER_SIZE = 4 * ByteSize.KiB;
        private string _tempFilePath = string.Empty;
        private FileStream _tempFileStream;
        
        


        public TempFileStream()
        {
            _tempFilePath = PathHelper.GetTempFileName();
            Status        = TempFileStreamStatus.Close;
        }



        public string FilePath => _tempFilePath;
        public override bool CanRead => Status == TempFileStreamStatus.Open && (_tempFileStream?.CanRead ?? false);
        public override bool CanSeek => Status == TempFileStreamStatus.Open && (_tempFileStream?.CanSeek ?? false);
        public override bool CanWrite => Status == TempFileStreamStatus.Open && (_tempFileStream?.CanWrite ?? false);
        public override long Length => Status == TempFileStreamStatus.Open ? _tempFileStream.Length: 0;
        public override long Position
        {
            get => Status == TempFileStreamStatus.Open ? _tempFileStream.Position : 0;
            set
            {
                if (_tempFileStream != null)
                {
                    _tempFileStream.Position = value;
                }
            } 
        }
        public TempFileStreamStatus Status { get; private set; }
        public bool AutoFlush { get; set; } = true;

        public override void Flush()
        {
            _tempFileStream.Flush();
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            OpenFile();

            return _tempFileStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            OpenFile();

            return _tempFileStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            OpenFile();

            _tempFileStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            OpenFile();

            _tempFileStream.Write(buffer, offset, count);

            if (AutoFlush)
            {
                _tempFileStream.Flush(true);
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (AutoFlush)
            {
                _tempFileStream.Flush(true);
            }

            await CloseFileAsync();
            File.Delete(_tempFilePath);

            Status = TempFileStreamStatus.Released;
        }
        
        public void OpenFile()
        {
            if (Status == TempFileStreamStatus.Open)
            {
                return;
            }

            if (Status == TempFileStreamStatus.Released)
            {
                throw new TempFileAlreadyDeletedException();
            }

            _tempFileStream = new FileStream(
                _tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, DEFAULT_BUFFER_SIZE,
                FileOptions.WriteThrough);
            
            Status = TempFileStreamStatus.Open;
        }

        public async ValueTask OpenFileAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

#pragma warning disable CS4014
            Task.Run(
#pragma warning restore CS4014
                () =>
                {
                    try
                    {
                        OpenFile();
                        tcs.SetResult(true);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            await tcs.Task;
        }

        public void CloseFile()
        {
            if (Status != TempFileStreamStatus.Open)
            {
                return;
            }

            _tempFileStream.Close();
            _tempFileStream.Dispose();
            _tempFileStream = null;
            Status          = TempFileStreamStatus.Close;
        }

        public async ValueTask CloseFileAsync()
        {
            if (Status != TempFileStreamStatus.Open)
            {
                return;
            }

            _tempFileStream.Close();
            await _tempFileStream.DisposeAsync();
            _tempFileStream = null;
            Status          = TempFileStreamStatus.Close;
        }
    }
}
