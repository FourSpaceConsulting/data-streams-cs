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
using Fourspace.Toolbox.Service;
using System;

namespace Fourspace.DataStreams.Streams
{
    public class AdapterReadStream<I, O> : IReadStream<O>
    {
        private readonly IAdapter<I, O> adapter;
        private readonly IReadStream<I> dataStream;
        private bool disposed;

        public AdapterReadStream(IAdapter<I, O> adapter, IReadStream<I> dataStream)
        {
            this.adapter = adapter;
            this.dataStream = dataStream;
        }

        public bool IsDataAvailable()
        {
            return dataStream.IsDataAvailable();
        }

        public O ReadFromStream()
        {
            return adapter.Adapt(dataStream.ReadFromStream());
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
                    if (dataStream != null)
                        dataStream.Dispose();
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
