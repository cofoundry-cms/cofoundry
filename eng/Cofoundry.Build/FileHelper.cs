namespace Cofoundry.Build;

public static class FileHelper
{
    public static void EnsureDirectoryExists(string path)
    {
        var directory = new DirectoryInfo(path);
        directory.Create();
    }

    public static void DeleteContents(string path)
    {
        var directory = new DirectoryInfo(path);

        if (!directory.Exists)
        {
            return;
        }

        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in directory.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}
