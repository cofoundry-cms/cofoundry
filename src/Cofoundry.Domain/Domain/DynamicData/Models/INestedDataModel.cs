using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marker interface that indicates a model can be nested inside
    /// another data model.
    /// </summary>
    /// <remarks>
    /// We need to use marker here because we let our api query on model type
    /// and so we must limit type creation to known safe types.
    /// </remarks>
    public interface INestedDataModel
    {
    }
}
