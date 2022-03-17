using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Tests.Shared.Mocks;

public class MockImageAssetFileService : IImageAssetFileService
{
    private const string ASSET_FILE_CONTAINER_NAME = "Images";

    private readonly CofoundryDbContext _dbContext;
    private readonly IFileStoreService _fileStoreService;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public MockImageAssetFileService(
        CofoundryDbContext dbContext,
        IFileStoreService fileStoreService,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _fileStoreService = fileStoreService;
        _transactionScopeManager = transactionScopeManager;
    }

    public bool SaveFile { get; set; } = true;

    public int? WidthInPixels { get; set; }

    public int? HeightInPixels { get; set; }

    public async Task SaveAsync(IFileSource fileToSave, ImageAsset imageAsset, string validationErrorPropertyName)
    {
        var fileExtension = Path.GetExtension(fileToSave.FileName);

        if (WidthInPixels.HasValue)
        {
            imageAsset.WidthInPixels = WidthInPixels.Value;
        }

        if (HeightInPixels.HasValue)
        {
            imageAsset.HeightInPixels = HeightInPixels.Value;
        }

        imageAsset.FileExtension = fileExtension;

        using var inputSteam = await fileToSave.OpenReadStreamAsync();
        imageAsset.FileSizeInBytes = inputSteam.Length;
        var fileName = Path.ChangeExtension(imageAsset.FileNameOnDisk, imageAsset.FileExtension);

        using (var scope = _transactionScopeManager.Create(_dbContext))
        {
            if (SaveFile)
            {
                await _fileStoreService.CreateAsync(ASSET_FILE_CONTAINER_NAME, fileName, inputSteam);
            }

            await _dbContext.SaveChangesAsync();
            await scope.CompleteAsync();
        }
    }
}
