using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A wrapper file provider that gives access to a single
    /// file provider that can access all registered static resource 
    /// locations.
    /// </summary>
    public interface IStaticResourceFileProvider : IFileProvider
    {
    }
}