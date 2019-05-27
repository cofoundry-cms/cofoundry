using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a unique collection of all files types currently 
    /// stored in the system.
    /// </summary>
    public class GetAllDocumentAssetFileTypesQuery : IQuery<ICollection<DocumentAssetFileType>>
    {
    }
}
