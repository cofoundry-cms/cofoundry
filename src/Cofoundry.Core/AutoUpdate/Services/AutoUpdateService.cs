using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Service to update applications and modules.
    /// </summary>
    public class AutoUpdateService : IAutoUpdateService
    {
        #region private variables

        private static readonly MethodInfo _runMethod = typeof(AutoUpdateService).GetMethod("ExecuteGenericCommand", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IUpdatePackageFactory[] _updatePackageFactories;
        private readonly IUpdateCommandHandlerFactory _commandHandlerFactory;
        private readonly IDatabase _db;

        #endregion

        #region constructor

        public AutoUpdateService(
            IUpdatePackageFactory[] updatePackageFactories,
            IUpdateCommandHandlerFactory commandHandlerFactory,
            IDatabase db
            )
        {
            _updatePackageFactories = updatePackageFactories;
            _commandHandlerFactory = commandHandlerFactory;
            _db = db;
        }

        #endregion
        
        #region update

        /// <summary>
        /// Updates an application and referenced modules by scanning for implementations
        /// of IUpdatePackageFactory and executing any packages found.
        /// </summary>
        /// <remarks>
        /// Async because sometimes UpdateCommands need to be async to save having to create
        /// sync versions of methods that would not normally require them. E.g. when calling into
        /// shared command handlers. I don't really think there's much benefit in making any other
        /// part asyn because nothing else useful should be happening while the db update is going on anyway.
        /// </remarks>
        public async Task UpdateAsync()
        {
            var versions = GetModuleVersions();

            // Make sure Cofoundry packages are installed first, even if someone has forgotten to mark it as a dependency
            var packages = _updatePackageFactories
                .SelectMany(f => f.Create(versions))
                .Where(p => p.Commands.Any())
                .OrderByDescending(p => p.ModuleIdentifier == CofoundryModuleInfo.ModuleIdentifier)
                .ThenBy(p => p)
                .ToList();

            if (packages.Any() && IsLocked())
            {
                throw new DatabaseLockedException();
            }

            foreach (var package in packages)
            {
                foreach (var command in package.Commands)
                {
                    try
                    {
                        await ExecuteCommandAsync(command);
                    }
                    catch (Exception ex)
                    {
                        LogUpdateError(package.ModuleIdentifier, command.Version, command.Description, ex);
                        throw;
                    }

                    LogUpdateSuccess(package.ModuleIdentifier, command.Version, command.Description);
                }
            }
        }

        private void LogUpdateSuccess(string module, int version, string description)
        {
            var sql = @"
	                insert into Cofoundry.ModuleUpdate (Module, [Version], [Description], ExecutionDate) 
	                values (@Module, @Version, @Description, @ExecutionDate)";

            _db.Execute(sql,
                new SqlParameter("Module", module),
                new SqlParameter("Version", version),
                new SqlParameter("Description", description),
                new SqlParameter("ExecutionDate", DateTime.UtcNow)
                );
        }

        private void LogUpdateError(string module, int version, string description, Exception ex)
        {
            try
            {
                var sql = @"
                    insert into Cofoundry.ModuleUpdateError (Module, [Version], [Description], ExecutionDate, ExceptionMessage) 
	                values (@Module, @Version, @Description, @ExecutionDate, @ExceptionMessage)";

                _db.Execute(sql,
                    new SqlParameter("Module", module),
                    new SqlParameter("Version", version),
                    new SqlParameter("Description", description),
                    new SqlParameter("ExecutionDate", DateTime.UtcNow),
                    new SqlParameter("ExceptionMessage", ex.ToString())
                    );
            }
            catch (Exception loggingException)
            {
                throw new AutoUpdateErrorLoggingException(ex, loggingException);
            }
        }

        private async Task ExecuteCommandAsync(IUpdateCommand command)
        {
            var task = (Task)_runMethod
                .MakeGenericMethod(command.GetType())
                .Invoke(this, new object[] { command });

            await task;
        }

        private async Task ExecuteGenericCommand<TCommand>(TCommand command) where TCommand : IUpdateCommand
        {
            var runner = _commandHandlerFactory.Create<TCommand>();

            if (runner is IAsyncUpdateCommandHandler<TCommand>)
            {
                await ((IAsyncUpdateCommandHandler<TCommand>)runner).ExecuteAsync(command);
            }
            else
            {
                ((ISyncUpdateCommandHandler<TCommand>)runner).Execute(command);
            }
        }

        private IEnumerable<ModuleVersion> GetModuleVersions()
        {
            // check both schemas since this was removed in v2
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

            var moduleVersions = _db.Read(query, (r) =>
            {
                var moduleVersion = new ModuleVersion();
                moduleVersion.Module = (string)r["Module"];
                moduleVersion.Version = (int)r["Version"];

                return moduleVersion;
            });

            return moduleVersions;
        }

        #endregion

        #region locking

        /// <summary>
        /// Runs a query to work out whether the database is locked for 
        /// schema updates.
        /// </summary>
        public bool IsLocked()
        {
            var query = @"
                if (exists (select * 
                                 from information_schema.tables 
                                 where table_schema = 'Cofoundry' 
                                 and  table_name = 'AutoUpdateLock'))
                begin
                    select IsLocked from Cofoundry.AutoUpdateLock;
                end";

            var isLocked = _db.Read(query, (r) =>
            {
                return (bool)r["IsLocked"];
            });

            return isLocked.FirstOrDefault();
        }

        /// <summary>
        /// Sets a flag in the database to enable/disable database updates.
        /// </summary>
        /// <param name="isLocked">True to lock the database and prevent schema updates</param>
        public void SetLocked(bool isLocked)
        {
            var cmd = "update Cofoundry.AutoUpdateLock set IsLocked = @IsLocked";
            _db.Execute(cmd, new SqlParameter("@IsLocked", isLocked));
        }

        #endregion
    }
}
