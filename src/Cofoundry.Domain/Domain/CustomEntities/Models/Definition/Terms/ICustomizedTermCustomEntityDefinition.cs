using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Implement this interface to define custom terminology to use in the UI for a
    /// custom entity. Otherwise default terms will be used.
    /// </summary>
    public interface ICustomizedTermCustomEntityDefinition<TDataModel> 
        : ICustomizedTermCustomEntityDefinition, ICustomEntityDefinition<TDataModel>
        where TDataModel : ICustomEntityDataModel
    {
    }

    /// <summary>
    /// Implement this interface to define custom terminology to use in the UI for a
    /// custom entity. Otherwise default terms will be used.
    /// </summary>
    public interface ICustomizedTermCustomEntityDefinition : ICustomEntityDefinition
    {
        /// <summary>
        /// A dictionary of any custom terminology to use when displaying the custom 
        /// entity, e.g. Title, Url Slug. You can use the values in CustomizableCustomEntityTermKeys
        /// constants class for the keys.
        /// </summary>
        Dictionary<string, string> CustomTerms { get; }
    }
}
