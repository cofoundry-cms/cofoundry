using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;
using FluentAssertions.Specialized;

namespace Cofoundry.Domain.Tests.Shared.Assertions;

/// <summary>
/// Extensions for the "Throws" family of FluentAssertsions methods.
/// </summary>
public static class ThrowsExtensions
{
    extension<TException>(ExceptionAssertions<TException> parent) where TException : ValidationException
    {
        /// <summary>
        /// Asserts that a <see cref="ValidationException"/> references all members included in <paramref name="memberNames"/>; no more,no less.
        /// </summary>
        /// <param name="memberNames">
        /// The member (property) names to check for. Each member must
        /// be present in the validation exception for the assertion to pass.
        /// </param>
        /// <returns>
        /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
        /// </returns>
        public ExceptionAssertions<TException> WithMemberNames(params string[] memberNames)
        {
            var exception = parent.Which;
            var numMatchingMembers = exception
                .ValidationResult
                .MemberNames
                .Where(m => memberNames.Contains(m))
                .Count();

            var isMatch = numMatchingMembers == memberNames.Length;
            var exceptionMemberNames = string.Join(", ", exception.ValidationResult.MemberNames);

            Execute.Assertion
                .ForCondition(isMatch)
                .FailWith("Expected exception with member names {0}{reason}, but found {1}.", string.Join(", ", memberNames), exceptionMemberNames);

            return parent;
        }
    }

    extension<TException>(Task<ExceptionAssertions<TException>> task) where TException : ValidationException
    {
        /// <summary>
        /// Asserts that a <see cref="ValidationException"/> references all members included in <paramref name="memberNames"/>; no more,no less.
        /// </summary>
        /// <param name="memberNames">
        /// The member (property) names to check for. Each member must
        /// be present in the validation exception for the assertion to pass.
        /// </param>
        /// <returns>
        /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
        /// </returns>
        public async Task<ExceptionAssertions<TException>> WithMemberNames(params string[] memberNames)
        {
            return (await task).WithMemberNames(memberNames);
        }
    }

    extension<TEntity>(ExceptionAssertions<EntityNotFoundException<TEntity>> parent) where TEntity : class
    {
        /// <summary>
        /// Asserts that a <see cref="EntityNotFoundException{TEntity}"/> references the expected <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The expected entity id contained in the exception.</param>
        /// <returns>
        /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
        /// </returns>
        public ExceptionAssertions<EntityNotFoundException<TEntity>> WithId(object id)
        {
            var exception = parent.Which;

            Execute.Assertion
                .ForCondition(id.Equals(exception.Id))
                .FailWith("Expected exception with id {0}{reason}, but found {1}.", id, exception.Id);

            return parent;
        }
    }

    extension<TEntity>(Task<ExceptionAssertions<EntityNotFoundException<TEntity>>> task) where TEntity : class
    {
        /// <summary>
        /// Asserts that a <see cref="EntityNotFoundException{TEntity}"/> references the expected <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The expected entity id contained in the exception.</param>
        /// <returns>The modified assertion for continued chaining.</returns>
        public async Task<ExceptionAssertions<EntityNotFoundException<TEntity>>> WithId(object id)
        {
            return (await task).WithId(id);
        }
    }

    extension<TException>(ExceptionAssertions<TException> parent) where TException : ValidationErrorException
    {
        /// <summary>
        /// Asserts that a <see cref="ValidationErrorException"/> has the specified <paramref name="errorCode"/>.
        /// </summary>
        /// <param name="errorCode">The expected error code contained in the exception.</param>
        /// <returns>The modified assertion for continued chaining.</returns>
        public ExceptionAssertions<TException> WithErrorCode(string errorCode)
        {
            var exception = parent.Which;

            Execute.Assertion
                .ForCondition(errorCode.Equals(exception.ErrorCode))
                .FailWith("Expected exception with ErrorCode {0}{reason}, but found {1}.", errorCode, exception.ErrorCode);

            return parent;
        }
    }

    extension<TException>(Task<ExceptionAssertions<TException>> task) where TException : ValidationErrorException
    {
        /// <summary>
        /// Asserts that a <see cref="ValidationErrorException"/> has the specified <paramref name="errorCode"/>.
        /// </summary>
        /// <param name="errorCode">The expected error code contained in the exception.</param>
        /// <returns>The modified assertion for continued chaining.</returns>
        public async Task<ExceptionAssertions<TException>> WithErrorCode(string errorCode)
        {
            return (await task).WithErrorCode(errorCode);
        }
    }
}
