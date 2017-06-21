using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents a partial update of a model, intended for use in
    /// a web api http patch operation.
    /// </summary>
    /// <typeparam name="TModel">The type of model to be updated.</typeparam>
    public interface IDelta<in TModel>
    {
        /// <summary>
        /// Updates an existing model with the data from the delta.
        /// </summary>
        /// <param name="model">Model to update.</param>
        void Patch(TModel model);
    }
}