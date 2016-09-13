using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cofoundry.Core.IO
{
    /// <summary>
    /// A stream that replaces white space in the string output, apart from inside pre tags.
    /// Usually used for reducing the size of rendered HTML documents.
    /// Also, usually used from inside action filter attributes in MVC.
    /// Probably not the best solution because of the performance hit: "the hit is as large as 88ms on a high end i7 trimming a 100KB document using RegEx-based code found on Stack Overflow." and "78KB HTML file takes 250ms to process".
    /// Make sure you use GZIP/deflate instead/aswell.
    /// </summary>
    /// <see cref="http://stackoverflow.com/a/8771084/486434"/>
    /// <see cref="http://stackoverflow.com/questions/8762993/remove-white-space-from-entire-html-but-inside-pre-with-regular-expressions"/>
    public class WhitespaceStream : MemoryStream
    {
        private readonly StringBuilder outputString = new StringBuilder();
        private readonly Stream outputStream = null;

        public WhitespaceStream(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            outputString.Append(Encoding.UTF8.GetString(buffer));
        }

        public override void Close()
        {
            // http://stackoverflow.com/questions/8762993/remove-white-space-from-entire-html-but-inside-pre-with-regular-expressions
            Regex reg = new Regex(@"(?<=\s)\s+(?![^<>]*</pre>)");
            string result = reg.Replace(outputString.ToString(), string.Empty);

            byte[] rawResult = Encoding.UTF8.GetBytes(result);
            outputStream.Write(rawResult, 0, rawResult.Length);

            base.Close();
            outputStream.Close();
        }
    }
}
