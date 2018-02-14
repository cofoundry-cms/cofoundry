using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    public class SettingsViewHelper : ISettingsViewHelper
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public SettingsViewHelper(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        public Task<TSettings> GetAsync<TSettings>() where TSettings : ICofoundrySettings
        {
            var query = new GetSettingsQuery<TSettings>();
            return _queryExecutor.ExecuteAsync(query);
        }

        #endregion
    }
}
