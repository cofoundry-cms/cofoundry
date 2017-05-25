using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Sandbox.Controllers
{
    [Route("api")]
    public class TestApiController : Controller
    {
        // http://www.dotnet-programming.com/post/2017/02/22/Custom-Model-Binding-in-Aspnet-Core-2-Getting-Time-2b-Client-Time-Zone-Offset.aspx
        // https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding
        //[HttpPost("person")]
        //public IActionResult AddPerson([ModelBinder(BinderType = typeof(AddPersonCommandPersonModelBinder))] AddPersonCommand command)
        //{

        //    return Ok(command);
        //}

        [HttpPost("person")]
        public IActionResult AddPerson([FromBody] AddPersonCommand command)
        {

            return Ok(command);
        }

        
    }
}
