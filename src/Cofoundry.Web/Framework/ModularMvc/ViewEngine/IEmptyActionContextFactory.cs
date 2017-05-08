using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public interface IEmptyActionContextFactory
    {
        ActionContext Create();
    }
}