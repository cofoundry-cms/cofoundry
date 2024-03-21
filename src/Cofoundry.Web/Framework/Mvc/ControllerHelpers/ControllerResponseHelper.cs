﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

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

    private static void AddValidationExceptionToModelState(Controller controller, ValidationException ex)
    {
        var propName = string.Empty;
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
