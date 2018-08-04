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
using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using Fourspace.DataStreams.Streams.Text;
using Fourspace.Toolbox.Util;
using Fourspace.Toolbox.Service.Adapters;
using Fourspace.DataStreams.Streams;
using System.IO;
using Fourspace.Toolbox.Service;
using Fourspace.DataStreams.Streams.Xsv;
using System.Linq;
using Fourspace.DataStreams.Streams.Files;
using Fourspace.Toolbox.Service.Factories;

namespace Fourspace.DataStreams.Test.Streams
{
    /// <summary>
    /// Summary description for XsvReadTest
    /// </summary>
    [TestFixture]
    public class XsvReadTest
    {
        private const char split = '|';
        private static readonly IReadOnlyList<string> Headers = Immutable.ReadOnlyList("A", "B", "C");
        private static readonly IReadOnlyDictionary<string, string> ExpectedFirst = new Dictionary<string, string>() { { "A", "Hello" }, { "B", "And" }, { "C", "Goodbye" } };
        private static readonly IReadOnlyDictionary<string, string> ExpectedSecond = new Dictionary<string, string>() { { "A", "Merhaba" }, { "B", "Ve" }, { "C", "Hos Cakal" } };
        private const string TestFirstString = "Hello|And|Goodbye";
        private const string TestSecondString = "Merhaba|Ve|Hos Cakal";

        [Test]
        public void AdaptStringTest() {

            var adapter = AdapterChain.Create(new CharSplitAdapter(split), new StringZipAdapter(Headers));
            var result = adapter.Adapt(TestFirstString);
            Assert.AreEqual(ExpectedFirst, result);
        }

        [Test]
        public void ParseStreamTest()
        {
            // arrange
            string TestString = TestFirstString + Environment.NewLine + TestSecondString;
            var adapter = AdapterChain.Create(new CharSplitAdapter(split), new StringZipAdapter(Headers));
            List<IReadOnlyDictionary<string, string>> parsed = new List<IReadOnlyDictionary<string, string>>();
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes(TestString)))
            using (StreamReader sr = new StreamReader(s))
            using (var stream = new LineReadStream(sr))
            using (var adapterStream = new AdapterReadStream<string, IReadOnlyDictionary<string, string>>(adapter, stream))
            {
                // act
                while (adapterStream.IsDataAvailable())
                {
                    parsed.Add(adapterStream.ReadFromStream());
                }
            }
            // assert
            Assert.AreEqual(ExpectedFirst, parsed[0]);
            Assert.AreEqual(ExpectedSecond, parsed[1]);
        }


        public class JoinedXsvStreamContext : IFileStreamMonitor
        {
            public string CurrentFilePath
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public IReadOnlyList<string> FilePaths { get; }
        }

        public class ParsedObject { }

        public class XsvToObjectStreamFactory<T>: IFactory<IReadStream<T>, IReadStream<string>>
        {
            public IReadStream<T> Create(IReadStream<string> stringStream)
            {
                try
                {
                    IAdapter<IReadOnlyDictionary<string, string>, T> objectAdapter = null;
                    return new HeaderBasedAdapterStream<string, T>(stringStream, XsvAdapterFactory.CreateXsvToObjectFactory(split, objectAdapter));
                } catch (Exception)
                {
                    stringStream.Dispose();
                    throw;
                }
            }
        }

        public interface IFileStreamMonitor {
            string CurrentFilePath { get; set; }
        }
        public interface ItemStreamMonitor
        {
            string CurrentItem { get; set; }
        }

        public interface MultiFileStreamMonitor : IFileStreamMonitor
        {
            string CurrentFileIndex { get; set; }
        }

        public class StreamMonitor
        {

        }

        public class FileStreamXsvFactory<T> : IFactory<IReadStream<T>, JoinedXsvStreamContext>
        {
            public IReadStream<T> Create(JoinedXsvStreamContext context)
            {
                XsvToObjectStreamFactory<T> x = null;
                var lineReadFactories = context.FilePaths.Select(i => FactoryChain.Create(FactoryWatcher.Create(new FileLineReadStreamFactory(i), (o) => context.CurrentFilePath = i), x));//.ToList<IFactory<IReadStream<string>>>();
                return new HeadToTailMultiStream<T>(lineReadFactories.ToList());
            }
        }

        //public void doit()
        //{
        //    JoinedXsvStreamContext context = null;
        //    IFactory<IReadStream<ParsedObject>, JoinedXsvStreamContext> streamFactory = new FileStreamXsvFactory<ParsedObject>();
        //    using (IReadStream<ParsedObject> stream = streamFactory.Create(context))
        //    {

        //    }

        //    AdapterChain.Create(new InsertPreExtensionFilePathAdapter("test"), new InsertPreExtensionFilePathAdapter("test"), new InsertPreExtensionFilePathAdapter("test"));

        //    try
        //    {
        //        // create a xsv reader example
        //        var stringStream = streamFactory.Create(context);

        //        IAdapter<IReadOnlyDictionary<string, string>, ParsedObject> objectAdapter = null;
        //        var objectStream = new HeaderBasedAdapterStream<string, ParsedObject>(stringStream, XsvAdapterFactory.CreateXsvToObjectFactory(split, objectAdapter));

        //        IReadStream<string> inputStream;

        //    } catch (Exception)
        //    {

        //    }

        //}
    }
}
