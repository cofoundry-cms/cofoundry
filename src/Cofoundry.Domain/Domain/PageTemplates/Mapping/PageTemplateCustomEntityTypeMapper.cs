using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateCustomEntityTypeMapper
    {
        private readonly ICustomEntityDisplayModel[] _customEntityDisplayModels;

        public PageTemplateCustomEntityTypeMapper(
            ICustomEntityDisplayModel[] customEntityDisplayModels
            )
        {
            _customEntityDisplayModels = customEntityDisplayModels;
        }

        public Type Map(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            var displayModels = _customEntityDisplayModels.Where(m => m.GetType().Name == typeName);

            Debug.Assert(displayModels.Count() == 1, "Incorrect number of ICustomEntityDisplayModels registered with the name '" + typeName + "'. Expected 1, got " + displayModels.Count());

            Type result = null;

            if (displayModels.Any())
            {
                result = displayModels.First().GetType();
            }


            return result;
        }
    }
}
