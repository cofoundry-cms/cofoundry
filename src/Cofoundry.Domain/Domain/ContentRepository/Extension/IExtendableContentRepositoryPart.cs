using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Extendable
{
    /// <summary>
    /// Used to mark up nested content repository parts such
    /// as query builders which allows for extension without 
    /// poluting the public api surface.
    /// </summary>
    public interface IExtendableContentRepositoryPart
    {
        /// <summary>
        /// The base content repository that can be leveraged in
        /// custom extensions. Intended only to be used internally
        /// by Cofoundry or by plugins that need to extend IContentRepository.
        /// </summary>
        IExtendableContentRepository ExtendableContentRepository { get; }
    }
}
