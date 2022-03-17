namespace Cofoundry.Core.AutoUpdate;

public class CreateDirectoriesUpdateCommandHandler : ISyncVersionedUpdateCommandHandler<CreateDirectoriesUpdateCommand>
{
    IPathResolver _pathResolver;

    public CreateDirectoriesUpdateCommandHandler(
        IPathResolver pathResolver
        )
    {
        _pathResolver = pathResolver;
    }

    public void Execute(CreateDirectoriesUpdateCommand command)
    {
        foreach (var path in command.Directories)
        {
            CreateDirectory(path);
        }
    }

    private void CreateDirectory(string directory)
    {
        var path = _pathResolver.MapPath(directory);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
