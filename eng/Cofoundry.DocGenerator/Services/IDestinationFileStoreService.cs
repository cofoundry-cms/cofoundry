namespace Cofoundry.DocGenerator;

public interface IDestinationFileStoreService
{
    Task ClearDirectoryAsync(string folderName);
    Task<string[]> GetDirectoryNamesAsync(string path);
    Task EnsureDirectoryExistsAsync(string relativePath);
    Task CopyFile(string source, string destination);
    Task WriteText(string text, string destination);
}
