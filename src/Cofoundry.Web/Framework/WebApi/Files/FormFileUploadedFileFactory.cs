using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using Cofoundry.Core.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory for creating and mapping asp.net IFormFile instances 
    /// to Cofoundry IUploadedFile instances.
    /// </summary>
    public class FormFileUploadedFileFactory : IFormFileUploadedFileFactory
    {
        private readonly IMimeTypeService _mimeTypeService;
            
        public FormFileUploadedFileFactory(
            IMimeTypeService mimeTypeService
            )
        {
            _mimeTypeService = mimeTypeService;
        }

        /// <summary>
        /// Creates and maps  a Cofoundry FormFileUploadedFile from an
        /// asp.net IFormFile instance. If the form file is null then a
        /// null is returned.
        /// </summary>
        /// <param name="formFile">Instance to map. If the form file is null then
        /// null is returned.</param>
        /// <returns>New FormFileUploadedFile instance or null if the input form file is null.</returns>
        public IUploadedFile Create(IFormFile formFile)
        {
            if (formFile == null) return null;

            return new FormFileUploadedFile(formFile, _mimeTypeService);
        }
    }
}