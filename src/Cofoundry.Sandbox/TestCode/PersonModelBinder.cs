using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Sandbox
{
    public class AddPersonCommandPersonModelBinder : IModelBinder
    {
        public AddPersonCommandPersonModelBinder()
        {

        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string body;
            using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }
            

        }
    }
}
