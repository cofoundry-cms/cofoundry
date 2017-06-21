using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public interface IEmptyActionContextFactory
    {
        ActionContext Create();
    }
}