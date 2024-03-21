using System.Globalization;
using System.Text;

namespace Cofoundry.Core.Data.SimpleDatabase;

/// <summary>
/// Used to build up a block of MS SqlServer DML script.
/// </summary>
internal class SqlStringBuilder
{
    private readonly StringBuilder _sql = new();

    public void AppendLine(string text, params object[] args)
    {
        _sql.AppendFormat(CultureInfo.InvariantCulture, text, args);
    }

    public void Go()
    {
        _sql.AppendLine("go");
        _sql.AppendLine();
    }

    public void Begin()
    {
        _sql.AppendLine("begin");
    }

    public void End()
    {
        _sql.AppendLine("end");
    }

    public void Use(string dbName)
    {
        AppendLine("use [{0}]", dbName);
        Go();
    }

    public void IfNotExists(string query, params object[] args)
    {
        var formattedQuery = string.Format(CultureInfo.InvariantCulture, query, args);
        AppendLine("if not exists({0})", formattedQuery);
    }

    public void IfExists(string query, params object[] args)
    {
        var formattedQuery = string.Format(CultureInfo.InvariantCulture, query, args);
        AppendLine("if exists({0}) ", formattedQuery);
    }

    public bool IsEmpty()
    {
        return _sql.Length < 1;
    }

    public override string ToString()
    {
        return _sql.ToString();
    }
}
