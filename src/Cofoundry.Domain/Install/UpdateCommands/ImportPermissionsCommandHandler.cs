using System.Text;
using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.Data;
using Cofoundry.Core.Data.SimpleDatabase;

namespace Cofoundry.Domain.Installation;

/// <summary>
/// This import job is to add all the permissions defined in code in
/// a single batch job during first startup, reducing the burden of 
/// doing this individually as roles are created
/// </summary>
public class ImportPermissionsCommandHandler : IAsyncVersionedUpdateCommandHandler<ImportPermissionsCommand>
{
    private readonly ICofoundryDatabase _db;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IPermissionRepository _permissionRepository;

    public ImportPermissionsCommandHandler(
        ICofoundryDatabase db,
        IPermissionRepository permissionRepository,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _db = db;
        _permissionRepository = permissionRepository;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(ImportPermissionsCommand command)
    {
        var sb = new StringBuilder();

        foreach (var permission in _permissionRepository.GetAll())
        {
            if (permission is IEntityPermission entityPermission)
            {
                var entityDefinition = entityPermission.EntityDefinition;

                sb.AppendLine(CultureInfo.InvariantCulture, $"if not exists (select * from Cofoundry.[EntityDefinition] where EntityDefinitionCode = '{entityDefinition.EntityDefinitionCode}')");
                sb.AppendLine("begin");
                sb.AppendLine(CultureInfo.InvariantCulture, $"insert into Cofoundry.[EntityDefinition] (EntityDefinitionCode, Name) values ('{entityDefinition.EntityDefinitionCode}', '{entityDefinition.Name}')");
                sb.AppendLine("end");
                sb.AppendLine(CultureInfo.InvariantCulture, $"insert into Cofoundry.[Permission] (EntityDefinitionCode, PermissionCode) values ('{entityDefinition.EntityDefinitionCode}', '{permission.PermissionType.Code}')");
            }
            else
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"insert into Cofoundry.[Permission] (PermissionCode) values ('{permission.PermissionType.Code}')");
            }
        }

        var sql = sb.ToString();
        using (var scope = _transactionScopeManager.Create(_db))
        {
            await _db.ExecuteAsync(sql);
            await scope.CompleteAsync();
        }
    }
}
