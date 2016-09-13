using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cofoundry.Core.IO
{
    /**
     * The default implementation of StringWriter doesn't allow you to specify the
     * encoding and uses UTF-16 by default
     */
    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter() : base()
        {
        }

        public Utf8StringWriter(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        public Utf8StringWriter(StringBuilder sb)
            : base(sb)
        {
        }


        public Utf8StringWriter(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
        }

        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
