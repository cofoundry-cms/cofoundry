using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a Metadata attribute that can be parsed by 
    /// a MetadataProvider
    /// </summary>
    public interface IMetadataAttribute
    {
        /// <summary>
        /// Implement this to customize the model metadata, adding
        /// any items to the AdditionalValues. 
        /// </summary>
        void Process(ModelMetadata modelMetaData);
    }
}
