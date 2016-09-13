using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;
using AutoMapper;

namespace Cofoundry.Web
{
    public class QuotationDisplayModelMapper : AutoMapperPageModuleDisplayModelMapper<QuotationDataModel, QuotationDisplayModel>
    {
    }
}