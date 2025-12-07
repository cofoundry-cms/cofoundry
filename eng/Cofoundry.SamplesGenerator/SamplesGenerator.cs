using Cofoundry.Build;

namespace Cofoundry.SamplesGenerator;

public class SamplesGenerator
{
    private static readonly string[] _ignoreDirectories = ["bin", "obj", ".vs"];

    private readonly SamplesGeneratorSettings _samplesGeneratorSettings;
    private readonly TextWriter _logger;

    public SamplesGenerator(
        SamplesGeneratorSettings samplesGeneratorSettings,
        TextWriter logger)
    {
        _samplesGeneratorSettings = samplesGeneratorSettings;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.WriteLine("Clearing directories");
        await CleanDestinationDirectory("src");
        await CleanDestinationDirectory("docker");

        string[] samples = [
            "Mail",
            "Menus",
            "PageBlockTypes",
            "SimpleSite",
            "SPASite",
            "UserAreas"
        ];

        _logger.WriteLine("Copying samples");
        foreach (var sample in samples)
        {
            var sourcePath = Path.Combine(_samplesGeneratorSettings.SourceDirectory, "src", sample);
            var destinationPath = Path.Combine(_samplesGeneratorSettings.DestinationDirectory, "src", sample);
            _logger.WriteLine($"Copying sample {sample}");

            await CopyDirectory(sourcePath, destinationPath, filePostProcessor: SampleFilesPostProcessor);
        }

        string[] additionalDirectories = [
            "local-env"
        ];

        _logger.WriteLine("Copying additional directories");
        foreach (var additionalDirectory in additionalDirectories)
        {
            var sourcePath = Path.Combine(_samplesGeneratorSettings.SourceDirectory, additionalDirectory);
            var destinationPath = Path.Combine(_samplesGeneratorSettings.DestinationDirectory, additionalDirectory);
            _logger.WriteLine($"Copying directory {additionalDirectory}");

            await CopyDirectory(sourcePath, destinationPath, filePreProcessor: AdditionalFilesPreProcessor);
        }

        string[] rootFiles = [
            Path.Combine(_samplesGeneratorSettings.SourceDirectory, "../.gitattributes"),
            Path.Combine(_samplesGeneratorSettings.SourceDirectory, "../.gitignore"),
            Path.Combine(_samplesGeneratorSettings.SourceDirectory, "../.editorconfig"),
            Path.Combine(_samplesGeneratorSettings.SourceDirectory, "../LICENSE")
        ];

        foreach (var rootFilePath in rootFiles)
        {
            var destinationPath = Path.Combine(_samplesGeneratorSettings.DestinationDirectory, Path.GetFileName(rootFilePath));
            var sourceFile = new FileInfo(rootFilePath);
            _logger.WriteLine($"Copying file '{sourceFile.Name}'");

            await CopyFile(sourceFile, destinationPath);
        }

        _logger.WriteLine("SUCCESS: Sample generator finished");
    }

    private void SampleFilesPostProcessor(FileInfo fileInfo)
    {
        if (fileInfo.Extension == ".csproj")
        {
            var projectFile = new ProjectFile(fileInfo.FullName);
            _logger.WriteLine($"Patching project file with Cofoundry NuGet version {_samplesGeneratorSettings.CofoundryVersion}");

            projectFile.ConvertProjectReferencesToNuGet(_samplesGeneratorSettings.CofoundryVersion, _logger);
            projectFile.Save();
        }
    }

    private bool AdditionalFilesPreProcessor(FileInfo fileInfo)
    {
        var shouldIgnoreFile = fileInfo.Extension == ".bacpac"
            && (fileInfo.Name.StartsWith("Cofoundry.Plugins.") || fileInfo.Name.StartsWith("Cofoundry.Dev"));

        if (shouldIgnoreFile)
        {
            _logger.WriteLine($"Ignoring file {fileInfo.Name}");
        }

        return !shouldIgnoreFile;
    }

    private async Task CleanDestinationDirectory(string directory)
    {
        var path = Path.Combine(_samplesGeneratorSettings.DestinationDirectory, directory);

        FileHelper.DeleteContents(path);
    }

    private static async Task CopyDirectory(
        string sourcePath,
        string destinationPath,
        Func<FileInfo, bool>? filePreProcessor = null,
        Action<FileInfo>? filePostProcessor = null)
    {
        var sourceDirectory = new DirectoryInfo(sourcePath);
        var directoriesToCopy = sourceDirectory.GetDirectories();

        var destinationDirectory = new DirectoryInfo(destinationPath);
        destinationDirectory.Create();

        foreach (var sourceFile in sourceDirectory.EnumerateFiles())
        {
            var canCopyFile = filePreProcessor?.Invoke(sourceFile) ?? true;

            if (canCopyFile)
            {
                var destinationFilePath = Path.Combine(destinationDirectory.FullName, sourceFile.Name);
                await CopyFile(sourceFile, destinationFilePath);
                filePostProcessor?.Invoke(new FileInfo(destinationFilePath));
            }
        }

        foreach (var subDir in directoriesToCopy.Where(d => !_ignoreDirectories.Contains(d.Name)))
        {
            var subDirectory = Path.Combine(destinationDirectory.FullName, subDir.Name);
            await CopyDirectory(subDir.FullName, subDirectory, filePreProcessor, filePostProcessor);
        }
    }

    private static async Task CopyFile(FileInfo sourceFile, string destinationPath)
    {
        using var sourceStream = sourceFile.Open(FileMode.Open);
        using var destinationStream = File.Create(destinationPath);

        await sourceStream.CopyToAsync(destinationStream);
    }
}
