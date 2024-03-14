namespace Cofoundry.Web;

public class ViewNotFoundException : Exception
{
    public ViewNotFoundException()
        : this(null, null)
    {
    }

    public ViewNotFoundException(string? message)
        : base(message)
    {

    }

    public ViewNotFoundException(string? viewName, IEnumerable<string>? searchedLocations)
        : base(FormatMessage(viewName, searchedLocations))
    {
        ViewName = viewName;
        SearchedLocations = searchedLocations;
    }

    public string? ViewName { get; private set; }

    public IEnumerable<string>? SearchedLocations { get; private set; }

    private static string FormatMessage(string? viewName, IEnumerable<string>? searchedLocations)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            return "View not found - ViewName not specified.";
        }

        var formattedSearchedLocations = string.Join(Environment.NewLine, EnumerableHelper.Enumerate(searchedLocations));

        return $"View not found '{viewName}'. Searched locations: {formattedSearchedLocations}";
    }
}
