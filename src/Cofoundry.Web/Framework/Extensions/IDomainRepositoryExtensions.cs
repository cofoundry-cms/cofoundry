using Cofoundry.Domain.Extendable;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cofoundry.Web;

/// <summary>
/// Extension methods for <see cref="IDomainRepository"/>.
/// </summary>
public static class IDomainRepositoryExtensions
{
    extension<TRepository>(TRepository repository) where TRepository : IDomainRepository
    {
        /// <summary>
        /// Wraps query and command execution with error handling code that pipes validation
        /// exceptions and validation query results into model state. Additionally if the
        /// model state is invalid prior to execution, then execution will be skipped.
        /// If execution is skipped then the default result type value will be returned and 
        /// should be ignored.
        /// </summary>
        /// <param name="controller">The controller instance containing the model state to pipe error to.</param>
        public TRepository WithModelState(ControllerBase controller)
        {
            ArgumentNullException.ThrowIfNull(controller);

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, controller.ModelState, null));
        }

        /// <summary>
        /// Wraps query and command execution with error handling code that pipes validation
        /// exceptions and validation query results into model state. Additionally if the
        /// model state is invalid prior to execution, then execution will be skipped.
        /// If execution is skipped then the default result type value will be returned and 
        /// should be ignored.
        /// </summary>
        /// <param name="controller">The controller instance containing the model state to pipe error to.</param>
        public TRepository WithModelState(Controller controller)
        {
            ArgumentNullException.ThrowIfNull(controller);

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, controller.ModelState, controller.ViewData.TemplateInfo));
        }

        /// <summary>
        /// Wraps query and command execution with error handling code that pipes validation
        /// exceptions and validation query results into model state. Additionally if the
        /// model state is invalid prior to execution, then execution will be skipped.
        /// If execution is skipped then the default result type value will be returned and 
        /// should be ignored.
        /// </summary>
        /// <param name="pageModel">The Razor Pages model instance containing the model state to pipe error to.</param>
        public TRepository WithModelState(PageModel pageModel)
        {
            ArgumentNullException.ThrowIfNull(pageModel);

            var extendedContentRepositry = repository.AsExtendableContentRepository();
            return (TRepository)extendedContentRepositry.WithExecutor(executor => new DomainRepositoryExecutorWithModelState(executor, pageModel.ModelState, pageModel.ViewData.TemplateInfo));
        }
    }
}
