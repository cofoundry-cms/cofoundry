using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Core.AutoMapper
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds HtmlString type converters to the configuration
        /// </summary>
        public static IMapperConfigurationExpression AddHtmlStringConverters(this IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<string, ProjectableHtmlString>().ProjectUsing(s => new ProjectableHtmlString() { Value = s });
            cfg.CreateMap<string, HtmlString>().ConvertUsing(new StringToHtmlStringConverter());
            cfg.CreateMap<HtmlString, string>().ConvertUsing(new HtmlStringToStringConverter());

            return cfg;
        }
    }
}
