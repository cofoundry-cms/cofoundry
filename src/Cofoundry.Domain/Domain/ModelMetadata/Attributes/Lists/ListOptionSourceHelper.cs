using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Shared code helper for metadata attributes that can have list option
    /// sources such as SelectListAttribute.
    /// </summary>
    public static class ListOptionSourceMetadataHelper
    {
        public static void AddToMetadata(DisplayMetadata modelMetaData, Type optionSourceType)
        {
            if (optionSourceType == null) throw new ArgumentNullException(nameof(optionSourceType));

            ICollection<ListOption> options = null;

            if (optionSourceType.IsEnum)
            {
                options = Enum
                    .GetNames(optionSourceType)
                    .Select(e => new ListOption(TextFormatter.PascalCaseToSentence(e), e))
                    .ToList();
            }
            else
            {
                if (optionSourceType.GetConstructor(Type.EmptyTypes) == null)
                {
                    var msg = "OptionSource type does not have a public parameterless constructor.";
                    throw new InvalidOperationException(msg);
                }

                var source = Activator.CreateInstance(optionSourceType);

                if (source is IListOptionApiSource apiSource)
                {
                    ValidateApiSource(optionSourceType, apiSource);

                    modelMetaData.AdditionalValues.Add("OptionsApi", apiSource.Path);
                    modelMetaData.AdditionalValues.Add("OptionName", TextFormatter.Camelize(apiSource.NameField));
                    modelMetaData.AdditionalValues.Add("OptionValue", TextFormatter.Camelize(apiSource.ValueField));
                }
                else if (source is IListOptionSource listOptionSource)
                {
                    options = listOptionSource.Create();
                }
                else
                {
                    var msg = "OptionSource type is not a valid type. Valid types are ";
                    throw new InvalidOperationException(msg);
                }
            }

            if (options != null)
            {
                modelMetaData.AdditionalValues.Add("OptionName", "text");
                modelMetaData.AdditionalValues.Add("OptionValue", "value");
                modelMetaData.AdditionalValues.Add("Options", options);
            }
        }

        private static void ValidateApiSource(Type optionSourceType, IListOptionApiSource apiSource)
        {
            if (string.IsNullOrWhiteSpace(apiSource.Path))
            {
                var msg = $"Source type {optionSourceType.FullName} does not define a Path.";
                throw new Exception(msg);
            }

            if (!Uri.IsWellFormedUriString(apiSource.Path, UriKind.Relative))
            {
                var msg = $"The path for source type {optionSourceType.FullName} is not a relative path. Only relative path types are supported.";
                throw new Exception(msg);
            }
        }
    }
}
