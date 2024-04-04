﻿using FluentAssertions.Primitives;

namespace Cofoundry.Domain.Tests.Shared.Assertions;

public static class HttpResponseMessageAssertionsExtensions
{
    public static Task<AndConstraint<HttpResponseMessageAssertions>> BeDeveloperPageExceptionAsync<TException>(
        this HttpResponseMessageAssertions messageAssertions,
        string because = "",
        params object[] becauseArgs
        )
    {
        return messageAssertions.BeDeveloperPageExceptionAsync($"*{typeof(TException).Name}*", because, becauseArgs);
    }

    /// <summary>
    /// Asserts that the response is an error 500 and contains the specified <paramref name="errorMessageWildcardPattern"/>.
    /// You can use this to assert that an exception has been thrown, but in the client setup you'll
    /// first need to ensure the developer exception page is enabled. Do this during client creation by
    /// altering the service collection, calling services.TurnOnDeveloperExceptionPage().
    /// </summary>
    /// <param name="errorMessageWildcardPattern">The exception message to check for; supports wildcard matching.</param>
    /// <param name="because">
    /// A formatted phrase as is supported by System.String.Format(System.String,System.Object[])
    /// explaining why the assertion is needed. If the phrase does not start with the
    /// word because, it is prepended automatically.
    ///</param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in because.
    /// </param>
    public static async Task<AndConstraint<HttpResponseMessageAssertions>> BeDeveloperPageExceptionAsync(
        this HttpResponseMessageAssertions messageAssertions,
        string errorMessageWildcardPattern,
        string because = "",
        params object[] becauseArgs
        )
    {
        var success = Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => messageAssertions.Subject.StatusCode)
            .ForCondition(s => s == HttpStatusCode.InternalServerError)
            .FailWith("Expected {context:response} status to be 500{reason}, but found {0}", messageAssertions.Subject.StatusCode);

        if (success)
        {
            var content = await messageAssertions.Subject.Content.ReadAsStringAsync();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(ContainsMatch(content, errorMessageWildcardPattern))
                .FailWith("Expected {context:response} to contain {0}{reason}, but found {1}.",
                    errorMessageWildcardPattern, content);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(messageAssertions);
    }

    private static bool ContainsMatch(string content, string wildcardPattern)
    {
        using var scope = new AssertionScope();

        content.Should().Match(wildcardPattern);
        return scope.Discard().Length == 0;
    }
}
