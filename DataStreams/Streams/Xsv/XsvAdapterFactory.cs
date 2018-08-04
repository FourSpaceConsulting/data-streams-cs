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
using Fourspace.Toolbox.Service.Adapters;
using System.Collections.Generic;

namespace Fourspace.DataStreams.Streams.Xsv
{

    public static class XsvAdapterFactory {

        /// <summary>
        /// Creates a standard header based xsv to dictionary adapter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator">XSV separator</param>
        /// <param name="objectAdapter">Adapter from dictionary to object</param>
        /// <returns></returns>
        public static IFactory<IAdapter<string, IReadOnlyDictionary<string, string>>, string> CreateXsvToDictionaryFactory<T>(char separator)
        {
            var xsvSplitter = new CharSplitAdapter(separator);
            return new XsvToDictionaryAdapterFactory<T>(xsvSplitter, xsvSplitter);
        }

        /// <summary>
        /// Creates a standard header based dictionary parser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator">XSV separator</param>
        /// <param name="objectAdapter">Adapter from dictionary to object</param>
        /// <returns></returns>
        public static IFactory<IAdapter<string, T>, string> CreateXsvToObjectFactory<T>(char separator, IAdapter<IReadOnlyDictionary<string,string>,T> objectAdapter)
        {
            var xsvSplitter = new CharSplitAdapter(separator);
            return new XsvToObjectAdapterFactory<T>(xsvSplitter, xsvSplitter, objectAdapter);
        }
    }

}
