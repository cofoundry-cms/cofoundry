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
        /// Gets a specific INestedDataModel type by it's name. The "DataModel"
        /// suffix is options e.g. "CarouselItemDataModel" and "CarouselItem"
        /// both match the same type.
        /// </summary>
        /// <param name="name">
        /// The name of the model to get. The "DataModel" suffix is options e.g. 
        /// "CarouselItemDataModel" and "CarouselItem" both match the same type.
        /// </param>
        Type GetByName(string name);
    }
}
