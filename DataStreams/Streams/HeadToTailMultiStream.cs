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
using log4net;
using System;
using System.Collections.Generic;

namespace Fourspace.DataStreams.Streams
{
    /// <summary>
    /// Streams sequentially from multiple streams
    /// </summary>
    public class HeadToTailMultiStream<T> : IReadStream<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IList<IFactory<IReadStream<T>>> streamFactories;
        private readonly IEnumerator<IFactory<IReadStream<T>>> streamIterator;
        //
        private readonly IList<IReadStream<T>> createdStreams;
        private IReadStream<T> currentStream;
        private bool disposed;

        public HeadToTailMultiStream(IList<IFactory<IReadStream<T>>> streamFactories)
        {
            createdStreams = new List<IReadStream<T>>();
            this.streamFactories = streamFactories;
            this.streamIterator = streamFactories.GetEnumerator();
            MoveNext();
        }

        public bool IsDataAvailable()
        {
            // check streams
            bool available = false;
            do
            {
                available = currentStream != null && currentStream.IsDataAvailable();
            }
            while (!available && MoveNext());
            return available;
        }

        public T ReadFromStream()
        {
            return (IsDataAvailable()) ? currentStream.ReadFromStream() : default(T);
        }

        private bool MoveNext()
        {
            bool moved = streamIterator.MoveNext();
            if (moved)
            {
                currentStream = streamIterator.Current.Create();
                if (currentStream != null) createdStreams.Add(currentStream);
            }
            else
            {
                currentStream = null;
            }
            return moved;
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
                    foreach (var stream in createdStreams)
                    {
                        try
                        {
                            if (stream != null) stream.Dispose();
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Failed to dispose of stream", e);
                        }
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
