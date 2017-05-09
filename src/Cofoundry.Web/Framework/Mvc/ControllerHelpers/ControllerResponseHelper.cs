using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper for providing responses back from controller actions
    /// </summary>
    public class ControllerResponseHelper : IControllerResponseHelper
    {
        private readonly ICommandExecutor _commandExecutor;

        public ControllerResponseHelper(
            ICommandExecutor commandExecutor
            )
        {
            _commandExecutor = commandExecutor;
        }

        public async Task ExecuteIfValidAsync<TCommand>(Controller controller, TCommand command) where TCommand : ICommand
        {
            if (controller.ModelState.IsValid)
            {
                try
                {
                    await _commandExecutor.ExecuteAsync(command);
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToModelState(controller, ex);
                }
            }
        }

        private void AddValidationExceptionToModelState(Controller controller, ValidationException ex)
        {
            string propName = string.Empty;
            var prefix = controller.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix += ".";
            }
            if (ex.ValidationResult != null && ex.ValidationResult.MemberNames.Count() == 1)
            {
                propName = prefix + ex.ValidationResult.MemberNames.First();
            }
            controller.ModelState.AddModelError(propName, ex.Message);
        }
    }
}