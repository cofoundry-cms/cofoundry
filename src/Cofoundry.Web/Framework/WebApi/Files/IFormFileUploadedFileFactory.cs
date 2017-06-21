using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory for creating and mapping asp.net IFormFile instances 
    /// to Cofoundry IUploadedFile instances.
    /// </summary>
    public interface IFormFileUploadedFileFactory
    {
        /// <summary>
        /// Creates and maps a Cofoundry IUploadedFile from an
        /// asp.net IFormFile instance. If the form file is null then
        /// null is returned.
        /// </summary>
        /// <param name="formFile">Instance to map. If the form file is null then a
        /// null is returned.</param>
        /// <returns>New IUploadedFile instance or null if the input form file is null.</returns>
        IUploadedFile Create(IFormFile formFile);
    }
}