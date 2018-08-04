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
using System.Collections.Generic;

namespace Fourspace.DataStreams.Streams.Text
{
    /// <summary>
    /// Creates a dictionary based on position with a key array
    /// </summary>
    public class StringZipAdapter : IAdapter<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>
    {
        private readonly IReadOnlyList<string> keys;

        public StringZipAdapter(IReadOnlyList<string> keys)
        {
            if (null == keys) throw new ArgumentNullException(nameof(keys));
            this.keys = keys;
        }

        public IReadOnlyDictionary<string, string> Adapt(IReadOnlyList<string> input)
        {
            Dictionary<string, string> dict = null;
            if (input != null)
            {
                if (keys.Count != input.Count)
                {
                    throw new Exception("Data contained " + input.Count + " values but expected " + keys.Count);
                }
                dict = new Dictionary<string, string>();
                for (int i = 0; i < keys.Count; ++i)
                {
                    dict[keys[i]] = input[i];
                }
            }
            return dict;
        }
    }
}
