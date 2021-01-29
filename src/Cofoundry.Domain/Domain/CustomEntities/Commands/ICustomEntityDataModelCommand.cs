using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a Custom Entity command that requires the
    /// dynamic data model.
    /// </summary>
    public interface ICustomEntityDataModelCommand
    {
        /// <summary>
        /// The custom entity data model data to be used in the
        /// command.
        /// </summary>
        ICustomEntityDataModel Model { get; set; }

        /// <summary>
        /// Unique 6 character definition code of the custom entity type
        /// that the data model related to.
        /// </summary>
        string CustomEntityDefinitionCode { get; set; }
    }
}
