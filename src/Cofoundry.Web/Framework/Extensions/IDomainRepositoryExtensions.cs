using Cofoundry.Domain.Extendable;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cofoundry.Web;

public static class IDomainRepositoryExtensions
{
    /// <summary>
    /// Wraps query and command execution with error handling code that pipes validation
    /// exceptions and validation query results into model state. Additionally if the
    /// model state is invalid prior to execution, then execution will be skipped.
    /// </summary>
    /// <param name="controller">The controller instance containing the model state to pipe error to.</param>
    public static TRepository WithModelState<TRepository>(this TRepository repository, ControllerBase controller)
        where TRepository : IDomainRepository
    {
        ArgumentNullException.ThrowIfNull(controller);

        var extendedContentRepositry = repository.AsExtendableContentRepository();
        return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, controller.ModelState, null));
    }

    /// <summary>
    /// Wraps query and command execution with error handling code that pipes validation
    /// exceptions and validation query results into model state. Additionally if the
    /// model state is invalid prior to execution, then execution will be skipped.
    /// </summary>
    /// <param name="controller">The controller instance containing the model state to pipe error to.</param>
    public static TRepository WithModelState<TRepository>(this TRepository repository, Controller controller)
        where TRepository : IDomainRepository
    {
        ArgumentNullException.ThrowIfNull(controller);

        var extendedContentRepositry = repository.AsExtendableContentRepository();
        return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, controller.ModelState, controller.ViewData.TemplateInfo));
    }

    /// <summary>
    /// Wraps query and command execution with error handling code that pipes validation
    /// exceptions and validation query results into model state. Additionally if the
    /// model state is invalid prior to execution, then execution will be skipped.
    /// </summary>
    /// <param name="controller">The controller instance containing the model state to pipe error to.</param>
    public static TRepository WithModelState<TRepository>(this TRepository repository, PageModel pageModel)
        where TRepository : IDomainRepository
    {
        ArgumentNullException.ThrowIfNull(pageModel);

        var extendedContentRepositry = repository.AsExtendableContentRepository();
        return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, pageModel.ModelState, pageModel.ViewData.TemplateInfo));
    }
}
