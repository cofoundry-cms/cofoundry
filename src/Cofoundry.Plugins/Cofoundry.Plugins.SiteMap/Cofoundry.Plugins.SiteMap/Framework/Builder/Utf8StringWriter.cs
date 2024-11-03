using System.Text;

namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// The default implementation of StringWriter doesn't allow you to specify the
/// encoding and uses UTF-16 by default
/// </summary>
internal class Utf8StringWriter : StringWriter
{
    public Utf8StringWriter(StringBuilder sb, IFormatProvider formatProvider)
        : base(sb, formatProvider)
    {
    }

    public override Encoding Encoding => Encoding.UTF8;
}
