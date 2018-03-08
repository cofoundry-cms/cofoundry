using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    ///  This repository is used for looking up INestedDataModel types. An instance
    ///  of each is registered with the DI container which is intended to beused for looking
    ///  up definitions of model properties. This repository checks for duplicate type definitions
    ///  and will throw an exception on startup if duplicates are defined.
    /// </summary>
    public interface INestedDataModelTypeRepository
    {
        /// <summary>
        /// Gets a specific INestedDataModel type by it's name. The
        /// name is the type name with the 'DataModel' suffix removed e.g. for the 
        /// data model type "CarouselItemDataModel", the name would be "CarouselItem".
        /// </summary>
        /// <param name="name">
        /// The name of the model to get. The
        /// name is the type name with the 'DataModel' suffix removed e.g. for the 
        /// data model type "CarouselItemDataModel", the name would be "CarouselItem".
        /// </param>
        Type GetByName(string name);
    }
}
