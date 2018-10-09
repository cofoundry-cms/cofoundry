using Cofoundry.Core.Data;
using Cofoundry.Core.Data.SimpleDatabase;
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
    /// Service to update applications and modules. Typically this is
    /// run at application startup.
    /// </summary>
    public class AutoUpdateService : IAutoUpdateService
    {
        #region private variables

        private static readonly MethodInfo _runVersionedCommandMethod = typeof(AutoUpdateService).GetMethod("ExecuteGenericVersionedCommand", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _runAlwaysRunCommandMethod = typeof(AutoUpdateService).GetMethod("ExecuteGenericAlwaysRunCommand", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IEnumerable<IUpdatePackageFactory> _updatePackageFactories;
        private readonly IUpdateCommandHandlerFactory _commandHandlerFactory;
        private readonly ICofoundryDatabase _db;
        private readonly IUpdatePackageOrderer _updatePackageOrderer;
        private readonly AutoUpdateSettings _autoUpdateSettings;
        private readonly IAutoUpdateDistributedLockManager _autoUpdateDistributedLockManager;

        #endregion

        #region constructor

        public AutoUpdateService(
            IEnumerable<IUpdatePackageFactory> updatePackageFactories,
            IUpdateCommandHandlerFactory commandHandlerFactory,
            ICofoundryDatabase db,
            IUpdatePackageOrderer updatePackageOrderer,
            AutoUpdateSettings autoUpdateSettings,
            IAutoUpdateDistributedLockManager autoUpdateDistributedLockManager
            )
        {
            _updatePackageFactories = updatePackageFactories;
            _commandHandlerFactory = commandHandlerFactory;
            _db = db;
            _updatePackageOrderer = updatePackageOrderer;
            _autoUpdateSettings = autoUpdateSettings;
            _autoUpdateDistributedLockManager = autoUpdateDistributedLockManager;
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
        /// part async because nothing else useful should be happening while the db update is going on anyway.
        /// </remarks>
        public async Task UpdateAsync()
        {
            var previouslyAppliedVersions = await GetUpdateVersionHistoryAsync();

            var filteredPackages = _updatePackageFactories
                .SelectMany(f => f.Create(previouslyAppliedVersions))
                .ToList();

            var packages = _updatePackageOrderer.Order(filteredPackages);

            if (!packages.Any()) return;

            if (await IsLockedAsync())
            {
                throw new DatabaseLockedException();
            }

            // Lock the process to prevent concurrent updates
            var lockingId = Guid.NewGuid();
            await _autoUpdateDistributedLockManager.LockAsync(lockingId);

            try
            {
                foreach (var package in packages)
                {
                    await ExecutePackage(package);
                }
            }
            finally
            {
                await _autoUpdateDistributedLockManager.UnlockAsync(lockingId);
            }
        }

        private async Task ExecutePackage(UpdatePackage package)
        {
            // Versioned Commands
            foreach (var command in EnumerableHelper.Enumerate(package.VersionedCommands))
            {
                try
                {
                    await ExecuteCommandAsync(command);
                }
                catch (Exception ex)
                {
                    await LogUpdateErrorAsync(package.ModuleIdentifier, command.Version, command.Description, ex);
                    throw;
                }

                await LogUpdateSuccessAsync(package.ModuleIdentifier, command.Version, command.Description);
            }

            // Always Run Commands
            foreach (var command in EnumerableHelper.Enumerate(package.AlwaysUpdateCommands))
            {
                await ExecuteCommandAsync(command);
            }
        }

        private Task LogUpdateSuccessAsync(string module, int version, string description)
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

        private Task LogUpdateErrorAsync(string module, int version, string description, Exception ex)
        {
            try
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
            catch (Exception loggingException)
            {
                throw new AutoUpdateErrorLoggingException(ex, loggingException);
            }
        }

        private Task ExecuteCommandAsync(IVersionedUpdateCommand command)
        {
            var task = (Task)_runVersionedCommandMethod
                .MakeGenericMethod(command.GetType())
                .Invoke(this, new object[] { command });

            return task;
        }

        private Task ExecuteCommandAsync(IAlwaysRunUpdateCommand command)
        {
            var task = (Task)_runAlwaysRunCommandMethod
                .MakeGenericMethod(command.GetType())
                .Invoke(this, new object[] { command });

            return task;
        }

        private Task ExecuteGenericVersionedCommand<TCommand>(TCommand command) where TCommand : IVersionedUpdateCommand
        {
            var runner = _commandHandlerFactory.CreateVersionedCommand<TCommand>();

            if (runner is IAsyncVersionedUpdateCommandHandler<TCommand>)
            {
                return ((IAsyncVersionedUpdateCommandHandler<TCommand>)runner).ExecuteAsync(command);
            }
            else
            {
                ((ISyncVersionedUpdateCommandHandler<TCommand>)runner).Execute(command);
                return Task.CompletedTask;
            }
        }

        private Task ExecuteGenericAlwaysRunCommand<TCommand>(TCommand command) where TCommand : IAlwaysRunUpdateCommand
        {
            var runner = _commandHandlerFactory.CreateAlwaysRunCommand<TCommand>();

            if (runner is IAsyncAlwaysRunUpdateCommandHandler<TCommand>)
            {
                return ((IAsyncAlwaysRunUpdateCommandHandler<TCommand>)runner).ExecuteAsync(command);
            }
            else
            {
                ((ISyncAlwaysRunUpdateCommandHandler<TCommand>)runner).Execute(command);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Gets a collections of module updates that have already been applied
        /// to the system.
        /// </summary>
        private async Task<ICollection<ModuleVersion>> GetUpdateVersionHistoryAsync()
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

        #endregion

        #region locking

        /// <summary>
        /// Works out whether the database is locked for 
        /// schema updates. This is different to distributed locking which 
        /// is intended to prevent multile update instances running.
        /// </summary>
        public async Task<bool> IsLockedAsync()
        {
            // First check config
            if (_autoUpdateSettings.Disabled) return true;

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

        /// <summary>
        /// Sets a flag in the database to enable/disable database updates.
        /// </summary>
        /// <param name="isLocked">True to lock the database and prevent schema updates</param>
        public Task SetLockedAsync(bool isLocked)
        {
            var cmd = "update Cofoundry.AutoUpdateLock set IsLocked = @IsLocked";
            return _db.ExecuteAsync(cmd, new SqlParameter("@IsLocked", isLocked));
        }

        #endregion
    }
}
