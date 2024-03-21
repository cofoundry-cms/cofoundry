﻿using System.Text;

namespace Cofoundry.Core.IO;

/// <summary>
/// TextWriter implementation to write to the debug log.
/// </summary>
/// <remarks>
/// See http://stackoverflow.com/a/637151/486434
/// </remarks>
public class DebugTextWriter : System.IO.TextWriter
{
    public override void Write(char[] buffer, int index, int count)
    {
        Write(new string(buffer, index, count));
    }

    public override void Write(string? value)
    {
        System.Diagnostics.Debug.Write(value);
    }

    public override void Write(char value)
    {
        System.Diagnostics.Debug.Write(value);
    }

    public override Encoding Encoding
    {
        get
        {
            return Encoding.GetEncoding(0); // default
        }
    }
}
