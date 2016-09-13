using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets the locale associated with the current thread culture (uses ICultureContextService)
    /// </summary>
    public class GetCurrentActiveLocaleQuery : IQuery<ActiveLocale>
    {
    }
}
