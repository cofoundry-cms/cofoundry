using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core;
using Microsoft.Extensions.Logging;
using Cofoundry.Core.Data;
using Cofoundry.Core.Data.SimpleDatabase;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.BasicTestSite
{
    public class TestViewModel
    {
        public int TestID { get; set; }
    }

    public class TestController : Controller
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly Test2DbContext _testDbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly DatabaseSettings _databaseSettings;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICofoundryDatabase _cofoundryDatabase;

        public TestController(
            CofoundryDbContext dbContext,
            Test2DbContext testDbContext,
            ICommandExecutor commandExecutor,
            ITransactionScopeManager transactionScopeFactory,
            ILoggerFactory loggerFactory,
            DatabaseSettings databaseSettings,
            ICofoundryDatabase cofoundryDatabase
            )
        {
            _dbContext = dbContext;
            _testDbContext = testDbContext;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
            _databaseSettings = databaseSettings;
            _loggerFactory = loggerFactory;
            _cofoundryDatabase = cofoundryDatabase;
        }

        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            await _cofoundryDatabase.ExecuteAsync("insert into Cofoundry.RewriteRule (WriteFrom,WriteTo,CreateDate,CreatorId) values ('out/db-pre', '/in/db-pre',GetUtcDate(),1)");

            using (var transaction = _transactionScopeFactory.Create(_cofoundryDatabase))
            {
                await _cofoundryDatabase.ExecuteAsync("insert into Cofoundry.RewriteRule (WriteFrom,WriteTo,CreateDate,CreatorId) values ('out/dbtran', '/in/dbtran',GetUtcDate(),1)");
                await transaction.CompleteAsync();
            }

            await _cofoundryDatabase.ExecuteAsync("insert into Cofoundry.RewriteRule (WriteFrom,WriteTo,CreateDate,CreatorId) values ('out/db1-post', '/in/db1-post',GetUtcDate(),1)");


            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                var command = new AddRedirectRuleCommand()
                {
                    WriteTo = "in/blahblablah",
                    WriteFrom = "out/blahblablah"
                };

                await _commandExecutor.ExecuteAsync(command);

                var command2 = new AddRedirectRuleCommand()
                {
                    WriteTo = "in2/blahblablah",
                    WriteFrom = "out2/blahblablah"
                };

                await _cofoundryDatabase.ExecuteAsync("insert into Cofoundry.RewriteRule (WriteFrom,WriteTo,CreateDate,CreatorId) values ('out/db1', '/in/db1',GetUtcDate(),1)");

                await _commandExecutor.ExecuteAsync(command2);
                // TODO: find a way to test by passing a connection into TestDb and see if we can get it working.
                //using (var scope2 = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                using (var scope2 = _transactionScopeFactory.Create(_testDbContext))
                {

                    var cat = new Cat()
                    {
                        Name = "Carrot",
                        CreatorId = 1
                    };

                    await _cofoundryDatabase.ExecuteAsync("insert into Cofoundry.RewriteRule (WriteFrom,WriteTo,CreateDate,CreatorId) values ('out/db2', '/in/db2',GetUtcDate(),1)");

                    var pages = await _testDbContext
                        .PageBlockTypes
                        .FilterActive()
                        .ToListAsync();

                    var cats = await _testDbContext
                        .Cats
                        .Include(c => c.Creator)
                        .ToListAsync();

                    _testDbContext.Cats.Add(cat);
                    await _testDbContext.SaveChangesAsync();

                    await scope2.CompleteAsync();
                }

                //scope.Complete();
                await scope.CompleteAsync();
            }

            return View(new TestViewModel());
        }
    }
}
