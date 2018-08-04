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
using Fourspace.DataStreams.Streams.Text;
using Fourspace.Toolbox.Service;
using Fourspace.Toolbox.Service.Adapters;
using System.Collections.Generic;

namespace Fourspace.DataStreams.Streams.Xsv
{
    /// <summary>
    /// XSV style adapter, using adapters to parsed the header and body lines into a dictionary.
    /// This dictionary is then used to create an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XsvToObjectAdapterFactory<T> : IFactory<IAdapter<string, T>, string>
    {
        private readonly IAdapter<string, IReadOnlyList<string>> headerAdapter;
        private readonly IAdapter<string, IReadOnlyList<string>> bodyAdapter;
        private readonly IAdapter<IReadOnlyDictionary<string, string>, T> objectAdapter;

        public XsvToObjectAdapterFactory(IAdapter<string, IReadOnlyList<string>> headerAdapter, IAdapter<string, IReadOnlyList<string>> bodyAdapter, IAdapter<IReadOnlyDictionary<string, string>, T> objectAdapter)
        {
            this.headerAdapter = headerAdapter;
            this.bodyAdapter = bodyAdapter;
            this.objectAdapter = objectAdapter;
        }

        public IAdapter<string, T> Create(string headerString)
        {
            var headers = headerAdapter.Adapt(headerString);
            var stringToDictionaryAdapter = AdapterChain.Create(bodyAdapter, new StringZipAdapter(headers));
            return AdapterChain.Create(stringToDictionaryAdapter, objectAdapter);
        }
    }
}
