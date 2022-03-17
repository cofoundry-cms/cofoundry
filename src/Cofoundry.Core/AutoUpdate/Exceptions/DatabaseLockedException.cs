namespace Cofoundry.Core.AutoUpdate;

public class DatabaseLockedException : Exception
{
    public DatabaseLockedException()
        : base("The database has been locked to prevent accidental automatic updates. Please unlock it before pushing updates.")
    {
    }
}
