using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Data;
using Cofoundry.Core.Data.SimpleDatabase;

namespace Cofoundry.Core.AutoUpdate
{
    public class UpdateDbCommandHandler : IAsyncVersionedUpdateCommandHandler<UpdateDbCommand>
    {
        private readonly ICofoundryDatabase _db;
        private readonly ITransactionScopeManager _transactionScopeManager;

        public UpdateDbCommandHandler(
            ICofoundryDatabase db,
            ITransactionScopeManager transactionScopeManager
            )
        {
            _db = db;
            _transactionScopeManager = transactionScopeManager;
        }

        public async Task ExecuteAsync(UpdateDbCommand command)
        {
            using (var transaction = _transactionScopeManager.Create(_db))
            {
                await RunPreScriptAsync(command);

                var sqlBatches = SqlHelper.SplitIntoBatches(command.Sql);

                foreach (var sqlBatch in sqlBatches)
                {
                    await _db.ExecuteAsync(sqlBatch);
                }

                await transaction.CompleteAsync();
            }
        }

        /// <summary>
        /// By convention we will delete out existing objects if they already exist and re-create them.
        /// </summary>
        private Task RunPreScriptAsync(UpdateDbCommand command)
        {
            var sql = new SqlStringBuilder();

            switch (command.ScriptType)
            {
                case DbScriptType.Functions:
                    sql.IfExists("select * from sysobjects where id = object_id(N'{0}') and type in (N'FN', N'IF', N'TF', N'FS', N'FT')", command.FileName);
                    sql.AppendLine("drop function {0}", command.FileName);
                    break;
                case DbScriptType.StoredProcedures:
                    sql.IfExists("select * from sysobjects where id = object_id(N'{0}') and ObjectProperty(id, N'IsProcedure') = 1", command.FileName);
                    sql.AppendLine("drop procedure {0}", command.FileName);
                    break;
                case DbScriptType.Views:
                    sql.AppendLine("if (object_id('{0}', 'V') is not null) ", command.FileName);
                    sql.AppendLine("drop view {0}", command.FileName);
                    break;
                case DbScriptType.Triggers:
                    sql.AppendLine("if (ObjectProperty(object_id('{0}'), 'IsTrigger') = 1)", command.FileName);
                    sql.AppendLine("drop trigger {0}", command.FileName);
                    break;
            }

            if (!sql.IsEmpty())
            {
                return _db.ExecuteAsync(sql.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
