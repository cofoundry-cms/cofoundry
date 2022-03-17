using Cofoundry.Domain.Extendable;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cofoundry.Web.Internal;

/// <summary>
/// An <see cref="IDomainRepositoryExecutor"/> implementation that wraps
/// query and command execution with error handling code that pipes validation
/// exceptions and validation query results into model state. Additionally if the
/// model state is invalid prior to execution, then execution will be skipped.
/// <inheritdoc/>
public class DomainRepositoryExecutorWithModelState : IDomainRepositoryExecutor
{
    private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;
    private readonly ModelStateDictionary _modelState;
    private readonly TemplateInfo _templateInfo;

    public DomainRepositoryExecutorWithModelState(
        IDomainRepositoryExecutor innerDomainRepositoryExecutor,
        ModelStateDictionary modelState,
        TemplateInfo templateInfo
        )
    {
        _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
        _modelState = modelState;
        _templateInfo = templateInfo;
    }

    public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
    {
        if (!_modelState.IsValid) return;

        try
        {
            await _innerDomainRepositoryExecutor.ExecuteAsync(command, executionContext);
        }
        catch (ValidationException validationException)
        {
            AddValidationExceptionToModelState(validationException);
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
    {
        if (!_modelState.IsValid) return default(TResult);

        var result = default(TResult);

        try
        {
            result = await _innerDomainRepositoryExecutor.ExecuteAsync(query, executionContext);
        }
        catch (ValidationException validationException)
        {
            AddValidationExceptionToModelState(validationException);
        }

        if (result is IValidationQueryResult validationQueryResult && validationQueryResult != null)
        {
            foreach (var error in validationQueryResult.GetErrors())
            {
                AddValidationErrorToModelState(error);
            }
        }

        return result;
    }

    private void AddValidationExceptionToModelState(ValidationException ex)
    {
        var propName = GetPropertyName(ex.ValidationResult?.MemberNames);

        _modelState.AddModelError(propName, ex.Message);
    }

    private void AddValidationErrorToModelState(ValidationError error)
    {
        var propName = GetPropertyName(error.Properties);

        _modelState.AddModelError(propName, error.Message);
    }

    private string GetPropertyName(IEnumerable<string> memberNames)
    {
        // Note: we condense multi-property errors into a global error
        if (EnumerableHelper.IsNullOrEmpty(memberNames) || memberNames.Count() != 1) return string.Empty;

        var prefix = _templateInfo?.HtmlFieldPrefix;

        if (!string.IsNullOrEmpty(prefix))
        {
            prefix += ".";
        }

        return prefix + memberNames.Single();
    }
}
