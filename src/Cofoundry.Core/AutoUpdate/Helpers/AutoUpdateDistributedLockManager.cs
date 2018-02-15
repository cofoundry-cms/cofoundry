using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Manages locking of the auto-update process to prevent concurrent update
    /// processes being run in multi-instance deployments.
    /// </summary>
    /// <remarks>
    /// A generic DistributedLock table is used here and could be potentially opened
    /// up to other processes. Implementation here is auto-update specific until we
    /// find more use-cases.
    /// </remarks>
    public class AutoUpdateDistributedLockManager : IAutoUpdateDistributedLockManager
    {
        const string DISTRIBUTED_LOCK_ID = "COFUPD";

        private readonly IDatabase _db;
        private readonly AutoUpdateSettings _autoUpdateSettings;

        public AutoUpdateDistributedLockManager(
            IDatabase db,
            AutoUpdateSettings autoUpdateSettings
            )
        {
            _db = db;
            _autoUpdateSettings = autoUpdateSettings;
        }

        public void Lock(Guid lockingId)
        {
            EnsureDistributedLockInfrastructureExists();

            var query = @" 
                declare @DateNow datetime2(7) = GetUtcDate()

                update Cofoundry.DistributedLock 
                set LockingId = @LockingId, LockDate = @DateNow, ExpiryDate = dateadd(second, @TimeoutInSeconds, @DateNow)
                where DistributedLockId = @DistributedLockId
                and (LockingId is null or ExpiryDate < @DateNow)

                select DistributedLockId, LockingId, LockDate, ExpiryDate 
                from Cofoundry.DistributedLock
                where DistributedLockId = @DistributedLockId
                ";

            var distributedLock = _db.Read(query,
                MapDistributedLock,
                new SqlParameter("DistributedLockId", DISTRIBUTED_LOCK_ID),
                new SqlParameter("LockingId", lockingId),
                new SqlParameter("TimeoutInSeconds", _autoUpdateSettings.ProcessLockTimeoutInSeconds)
                )
                .SingleOrDefault();

            if (distributedLock == null)
            {
                throw new Exception($"Distributed lock record missing for the auto update process. DistributedLockId: {DISTRIBUTED_LOCK_ID}");
            }

            if (distributedLock.LockingId != lockingId)
            {
                throw new AutoUpdateProcessLockedException(distributedLock);
            }
        }

        public void Unlock(Guid lockingId)
        {
            var sql = @"
                update Cofoundry.DistributedLock 
                set LockingId = null, LockDate = null, ExpiryDate = null
                where LockingId = @LockingId
                ";

            _db.Execute(sql, new SqlParameter("LockingId", lockingId));
        }

        private AutoUpdateDistributedLock MapDistributedLock(SqlDataReader reader)
        {
            var result = new AutoUpdateDistributedLock();
            if (reader[nameof(result.DistributedLockId)] == null) return null;

            result.DistributedLockId = (string)reader[nameof(result.DistributedLockId)];
            result.LockingId = (Guid)reader[nameof(result.LockingId)];
            result.LockDate = (DateTime)reader[nameof(result.LockDate)];
            result.ExpiryDate = (DateTime)reader[nameof(result.ExpiryDate)];

            return result;
        }

        private void EnsureDistributedLockInfrastructureExists()
        {
            _db.Execute(@"
                if not exists (select schema_name from information_schema.schemata where schema_name = 'Cofoundry')
                begin
	                exec sp_executesql N'create schema Cofoundry'
                end");

            _db.Execute(@"
                if (not exists (select * 
                    from information_schema.tables 
                    where table_schema = 'Cofoundry' 
                    and  table_name = 'DistributedLock'))
                begin
	                create table Cofoundry.DistributedLock (
		                DistributedLockId char(6) not null,
		                [Name] varchar(100) not null,
		                LockingId uniqueidentifier null,
		                LockDate datetime2(7) null,
		                ExpiryDate datetime2(7) null,

		                constraint PK_DistributedLock primary key (DistributedLockId)
	                )
                end");

            _db.Execute(@"
                with data as (select '" + DISTRIBUTED_LOCK_ID + @"' as DistributedLockId, 'Cofoundry auto-update process' as [Name])
                merge Cofoundry.DistributedLock t
                using data s on s.DistributedLockId = t.DistributedLockId
                when not matched by target
                then insert (DistributedLockId, [Name]) 
                values (s.DistributedLockId, s.[Name]);
                ");
        }

    }
}
