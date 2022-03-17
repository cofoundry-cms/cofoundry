using Cofoundry.Core.Data.SimpleDatabase;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.AutoUpdate.Internal;

/// <inheritdoc/>
public class AutoUpdateStore : IAutoUpdateStore
{
    private readonly ICofoundryDatabase _db;

    public AutoUpdateStore(
        ICofoundryDatabase db
        )
    {
        _db = db;
    }

    public async Task<ICollection<ModuleVersion>> GetVersionHistoryAsync()
    {
        var query = @"
                if (exists (select * 
                    from information_schema.tables 
                    where table_schema = 'Cofoundry' 
                    and  table_name = 'ModuleUpdate'))
                begin
                    select Module, MAX([Version]) as Version
	                from  Cofoundry.ModuleUpdate
	                group by Module
	                order by Module
                end";

        var moduleVersions = await _db.ReadAsync(query, r =>
        {
            var moduleVersion = new ModuleVersion();
            moduleVersion.Module = (string)r["Module"];
            moduleVersion.Version = (int)r["Version"];

            return moduleVersion;
        });

        return moduleVersions;
    }

    public async Task<bool> IsDatabaseLockedAsync()
    {
        // else this option can also be set in the db
        var query = @"
                if (exists (select * 
                    from information_schema.tables 
                    where table_schema = 'Cofoundry' 
                    and  table_name = 'AutoUpdateLock'))
                begin
                    select IsLocked from Cofoundry.AutoUpdateLock;
                end";

        var isLocked = await _db.ReadAsync(query, (r) =>
        {
            return (bool)r["IsLocked"];
        });

        return isLocked.FirstOrDefault();
    }

    public Task SetDatabaseLockedAsync(bool isLocked)
    {
        var cmd = "update Cofoundry.AutoUpdateLock set IsLocked = @IsLocked";
        return _db.ExecuteAsync(cmd, new SqlParameter("@IsLocked", isLocked));
    }

    public Task LogSuccessAsync(string module, int version, string description)
    {
        var sql = @"
	                insert into Cofoundry.ModuleUpdate (Module, [Version], [Description], ExecutionDate) 
	                values (@Module, @Version, @Description, @ExecutionDate)";

        return _db.ExecuteAsync(sql,
            new SqlParameter("Module", module),
            new SqlParameter("Version", version),
            new SqlParameter("Description", description),
            new SqlParameter("ExecutionDate", DateTime.UtcNow)
            );
    }

    public Task LogErrorAsync(string module, int version, string description, Exception ex)
    {
        var sql = @"
                insert into Cofoundry.ModuleUpdateError (Module, [Version], [Description], ExecutionDate, ExceptionMessage) 
	            values (@Module, @Version, @Description, @ExecutionDate, @ExceptionMessage)";

        return _db.ExecuteAsync(sql,
            new SqlParameter("Module", module),
            new SqlParameter("Version", version),
            new SqlParameter("Description", description),
            new SqlParameter("ExecutionDate", DateTime.UtcNow),
            new SqlParameter("ExceptionMessage", ex.ToString())
            );
    }
}
