using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// IResourceLocator wrapper around the file providers assigned to
    /// the razor view engine.
    /// </summary>
    public class WebsiteResourceLocator : IResourceLocator
    {
        private readonly IList<IFileProvider> _fileProviders;

        public WebsiteResourceLocator(
            IOptions<MvcRazorRuntimeCompilationOptions> mvcRazorRuntimeCompilationOptions
            )
        {
            _fileProviders = mvcRazorRuntimeCompilationOptions.Value.FileProviders;
        }

        public bool DirectoryExists(string virtualDir)
        {
            foreach (var fileProvider in _fileProviders)
            {
                if (fileProvider.GetDirectoryContents(virtualDir).Exists) return true;
            }

            return false;
        }

        public bool FileExists(string virtualPath)
        {
            foreach (var fileProvider in _fileProviders)
            {
                if (fileProvider.GetFileInfo(virtualPath).Exists) return true;
            }

            return false;
        }

        public IDirectoryContents GetDirectory(string virtualDir)
        {
            var directories = _fileProviders
                .Where(d => d.GetDirectoryContents(virtualDir).Exists)
                .ToList();

            // The directory might appear in multiple file providers, but
            // each may contain different files, so we need to return all matches
            if (directories.Any())
            {
                return new CompositeDirectoryContents(directories, virtualDir);
            }

            return new NotFoundDirectoryContents();
        }

        public IFileInfo GetFile(string virtualPath)
        {
            IFileInfo file = null;

            foreach (var fileProvider in _fileProviders)
            {
                file = fileProvider.GetFileInfo(virtualPath);
                if (file.Exists) return file;
            }

            return new NotFoundFileInfo(virtualPath);
        }
    }
}