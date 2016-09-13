using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageModuleDisplayModelMapperInput<TDataModel> where TDataModel : IPageModuleDataModel
    {
        public int VersionModuleId { get; set; }
        public TDataModel DataModel { get; set; }

        public PageModuleDisplayModelMapperOutput CreateOutput(IPageModuleDisplayModel displayModel)
        {
            Condition.Requires(displayModel, "displayModel").IsNotNull();
            Condition.Requires(VersionModuleId, "VersionModuleId").IsGreaterThan(0);

            return new PageModuleDisplayModelMapperOutput()
            {
                DisplayModel = displayModel,
                VersionModuleId = VersionModuleId
            };
        }
    }
}
