using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to create data model types and validate they exist
    /// </summary>
    public interface IPageModuleDataModelTypeFactory
    {
        /// <summary>
        /// Creates a data model type from the file name
        /// string i.e. 'PlainText' not 'PlainTextDataModel'. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="moduleTypeFileName">The unique name of the module type</param>
        Type CreateByPageModuleTypeFileName(string moduleTypeFileName);

        /// <summary>
        /// Creates a data model type from the database id. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="pageModuleTypeId">Id of the page module type in the database</param>
        Type CreateByPageModuleTypeId(int pageModuleTypeId);
    }
}
