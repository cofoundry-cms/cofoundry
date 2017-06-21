using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Core.Json;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public interface IVisualEditorActionResultFactory
    {
        IActionResult Create(IActionResult wrappedActionResult);
    }
}