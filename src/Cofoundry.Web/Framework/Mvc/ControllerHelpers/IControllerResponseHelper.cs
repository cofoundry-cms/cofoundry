using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    public interface IControllerResponseHelper
    {
        Task ExecuteIfValidAsync<TCommand>(Controller controller, TCommand command) where TCommand : ICommand;
    }
}