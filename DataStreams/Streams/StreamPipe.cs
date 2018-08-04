/*
MIT License

Copyright (c) 2017 Richard Steward

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using log4net;
using System;

namespace Fourspace.DataStreams.Streams
{
    public class StreamPipe<T> : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IReadStream<T> readStream;
        private readonly IWriteStream<T> writeStream;
        //
        private bool disposed;

        public StreamPipe(IReadStream<T> readStream, IWriteStream<T> writeStream)
        {
            this.readStream = readStream;
            this.writeStream = writeStream;
        }

        public void Write()
        {
            while (readStream.IsDataAvailable())
            {
                T item = readStream.ReadFromStream();
                writeStream.WriteToStream(item);
            }
        }

        #region disposal
        /// <summary>
        /// This disposes the object. 
        /// It cannot be used again after this call.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    try
                    {
                        if (writeStream != null) writeStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to dispose of stream", e);
                    }
                    try
                    {
                        if (readStream != null) readStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to dispose of stream", e);
                    }
                }
                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
