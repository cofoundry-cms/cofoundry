using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// A query handler always requires a query definition, even if there are 
    /// no parameters. As a shortcut you can instead make use of the generic 
    /// GetAllQuery so that you only need to define a handler.
    /// 
    /// See https://www.cofoundry.org/docs/framework/cqs#query-helpers for
    /// more information on Query Helpers
    /// </summary>
    public class GetAllBreedsQuery : IQuery<IEnumerable<Breed>>
    {
    }
}
