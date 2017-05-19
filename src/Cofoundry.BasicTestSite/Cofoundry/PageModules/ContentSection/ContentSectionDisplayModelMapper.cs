using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.BasicTestSite
{
    public class ContentSectionDisplayModelMapper : IPageModuleDisplayModelMapper<ContentSectionDataModel>
    {
        /// <summary>
        /// A IPageModuleDisplayModelMapper class handles the mapping from
        /// a display model to a data model.
        /// 
        /// The mapper supports DI which gives you flexibility in what data
        /// you want to include in the display model and how you want to 
        /// map it. Mapping is done in batch to improve performance when 
        /// the same module type is used multiple times on a page
        /// </summary>
        public Task<IEnumerable<PageModuleDisplayModelMapperOutput>> MapAsync(
            IEnumerable<PageModuleDisplayModelMapperInput<ContentSectionDataModel>> inputs, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            var results = new List<PageModuleDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = new ContentSectionDisplayModel();
                output.HtmlText = new HtmlString(input.DataModel.HtmlText);
                output.Title = input.DataModel.Title;

                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}