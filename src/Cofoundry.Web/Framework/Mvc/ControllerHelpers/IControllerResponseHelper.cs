using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    public interface IControllerResponseHelper
    {
        void ExecuteIfValid<TCommand>(Controller controller, TCommand command) where TCommand : ICommand;
        Task ExecuteIfValidAsync<TCommand>(Controller controller, TCommand command) where TCommand : ICommand;
    }
}