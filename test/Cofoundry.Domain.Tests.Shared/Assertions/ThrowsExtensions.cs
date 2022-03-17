using Cofoundry.Core.Validation;
using FluentAssertions.Specialized;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain.Tests.Shared.Assertions;

/// <summary>
/// Extensions for the "Throws" family of FluentAssertsions methods.
/// </summary>
public static class ThrowsExtensions
{
    /// <summary>
    /// Asserts that a <see cref="ValidationException"/> references all members included in <paramref name="memberNames"/>; no more,no less.
    /// </summary>
    /// <typeparam name="TException">The expected type of the exception.</typeparam>
    /// <param name="parent">The <see cref="ExceptionAssertions{TException}"/> containing the thrown exception.</param>
    /// <param name="memberNames">
    /// The member (property) names to check for. Each member must
    /// be present in the validation exception for the assertion to pass.
    /// </param>
    /// <returns>
    /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
    /// </returns>
    public static ExceptionAssertions<TException> WithMemberNames<TException>(this ExceptionAssertions<TException> parent, params string[] memberNames)
        where TException : ValidationException
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

    /// <summary>
    /// Asserts that a <see cref="ValidationException"/> references all members included in <paramref name="memberNames"/>; no more,no less.
    /// </summary>
    /// <typeparam name="TException">The expected type of the exception.</typeparam>
    /// <param name="task">The <see cref="ExceptionAssertions{TException}"/> containing the thrown exception.</param>
    /// <param name="memberNames">
    /// The member (property) names to check for. Each member must
    /// be present in the validation exception for the assertion to pass.
    /// </param>
    /// <returns>
    /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
    /// </returns>
    public static async Task<ExceptionAssertions<TException>> WithMemberNames<TException>(this Task<ExceptionAssertions<TException>> task, params string[] memberNames)
        where TException : ValidationException
    {
        return (await task).WithMemberNames(memberNames);
    }

    /// <summary>
    /// Asserts that a <see cref="ValidationException"/> references all members included in <paramref name="memberNames"/>; no more,no less.
    /// </summary>
    /// <typeparam name="TException">The expected type of the exception.</typeparam>
    /// <param name="parent">The <see cref="ExceptionAssertions{TException}"/> containing the thrown exception.</param>
    /// <param name="memberNames">
    /// The member (property) names to check for. Each member must
    /// be present in the validation exception for the assertion to pass.
    /// </param>
    /// <returns>
    /// The modified <see cref="ExceptionAssertions{TException}"/> for continued chaining.
    /// </returns>
    public static ExceptionAssertions<EntityNotFoundException<TEntity>> WithId<TEntity>(this ExceptionAssertions<EntityNotFoundException<TEntity>> parent, object id)
        where TEntity : class
    {
        var exception = parent.Which;

        Execute.Assertion
            .ForCondition(id.Equals(exception.Id))
            .FailWith("Expected exception with id {0}{reason}, but found {1}.", id, exception.Id);

        return parent;
    }

    /// <summary>
    /// Asserts that a <see cref="EntityNotFoundException{TEntity}"/> references the expected <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the <see cref="EntityNotFoundException{TEntity}"/>.</typeparam>
    /// <param name="task">The assertion containing the thrown exception.</param>
    /// <param name="id">
    /// The expected entity id contained in the exception.
    /// </param>
    /// <returns>
    /// The modified assertion for continued chaining.
    /// </returns>
    public static async Task<ExceptionAssertions<EntityNotFoundException<TEntity>>> WithId<TEntity>(this Task<ExceptionAssertions<EntityNotFoundException<TEntity>>> task, object id)
        where TEntity : class
    {
        return (await task).WithId(id);
    }

    /// <summary>
    /// Asserts that a <see cref="ValidationErrorException"/> has the specified <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode">The expected error code contained in the exception.</param>
    /// <returns>
    /// The modified assertion for continued chaining.
    /// </returns>
    public static ExceptionAssertions<TException> WithErrorCode<TException>(this ExceptionAssertions<TException> parent, string errorCode)
        where TException : ValidationErrorException
    {
        var exception = parent.Which;

        Execute.Assertion
            .ForCondition(errorCode.Equals(exception.ErrorCode))
            .FailWith("Expected exception with ErrorCode {0}{reason}, but found {1}.", errorCode, exception.ErrorCode);

        return parent;
    }

    /// <summary>
    /// Asserts that a <see cref="ValidationErrorException"/> has the specified <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode">The expected error code contained in the exception.</param>
    /// <returns>
    /// The modified assertion for continued chaining.
    /// </returns>
    public static async Task<ExceptionAssertions<TException>> WithErrorCode<TException>(this Task<ExceptionAssertions<TException>> task, string errorCode)
        where TException : ValidationErrorException
    {
        return (await task).WithErrorCode(errorCode);
    }
}
