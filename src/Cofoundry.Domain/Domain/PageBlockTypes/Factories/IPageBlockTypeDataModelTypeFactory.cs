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
    public interface IPageBlockTypeDataModelTypeFactory
    {
        /// <summary>
        /// Creates a data model type from the file name
        /// string i.e. 'PlainText' not 'PlainTextDataModel'. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="pageBlockTypeFileName">The unique name of the page block type</param>
        Type CreateByPageBlockTypeFileName(string pageBlockTypeFileName);

        /// <summary>
        /// Creates a data model type from the database id. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="pageBlockTypeId">Id of the page block type in the database</param>
        Task<Type> CreateByPageBlockTypeIdAsync(int pageBlockTypeId);
    }
}
