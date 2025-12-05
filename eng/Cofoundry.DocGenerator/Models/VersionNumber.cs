namespace Cofoundry.DocGenerator;

public class VersionNumber : IComparable<VersionNumber>
{
    public int Major { get; set; }

    public int Minor { get; set; }

    public int Patch { get; set; }

    public int CompareTo(VersionNumber? other)
    {
        if (other == null)
        {
            return -1;
        }

        return (Major, Minor, Patch).CompareTo((other.Major, other.Minor, other.Patch));
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}
