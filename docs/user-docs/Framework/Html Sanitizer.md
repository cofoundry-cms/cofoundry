Cofoundry includes a service for cleaning and sanitizing html which helps you prevent XSS attacks when rendering user input. 

To use this you can simply request `IHtmlSanitizer` from the DI container, or more commonly you might use the [Cofoundry View Helper](/content-management/cofoundry-view-helper) directly from a view:

```html
@using Cofoundry.Domain

@model MyTestViewModel
@inject ICofoundryHelper<MyTestViewModel> Cofoundry

<div>
     @Cofoundry.Sanitizer.Sanitize("<h1>My Heading</h1><script>alert('uh oh')</script>")
</div>

```

## Customizing the sanitization ruleset

The Cofoundry HtmlSanitizer relies on the excellent [mganss/HtmlSanitizer](https://github.com/mganss/HtmlSanitizer) package to perform sanitization and uses it's default ruleset with a couple of modifications – adding the `class` attribute and the `mailto` url scheme to the allow-list. This means the default ruleset is quite liberal, so you may wish to create your own ruleset or modify the default if you want to be more restrictive. 

Here's an example that shows how to create a ruleset, using the mganss sanitizer defaults.

```csharp
using Cofoundry.Core.Web;
using Ganss.Xss;
using System.Collections.Immutable;

private static HtmlSanitizationRuleSet CreateRuleSet()
{
    var ruleSet = new HtmlSanitizationRuleSet();

    ruleSet.PermittedAtRules = HtmlSanitizerDefaults
        .AllowedAtRules
        .ToImmutableHashSet();

    ruleSet.PermittedAttributes = HtmlSanitizerDefaults
        .AllowedAttributes
        .Append("class")
        .ToImmutableHashSet();

    ruleSet.PermittedCssClasses = HtmlSanitizerDefaults
        .AllowedClasses
        .ToImmutableHashSet();

    ruleSet.PermittedCssProperties = HtmlSanitizerDefaults
        .AllowedCssProperties
        .ToImmutableHashSet();

    ruleSet.PermittedSchemes = HtmlSanitizerDefaults
        .AllowedSchemes
        .Append("mailto")
        .ToImmutableHashSet();

    ruleSet.PermittedTags = HtmlSanitizerDefaults
        .AllowedTags
        .ToImmutableHashSet();

    ruleSet.PermittedUriAttributes = HtmlSanitizerDefaults
        .UriAttributes
        .ToImmutableHashSet();

    return ruleSet;
}
```

To use the ruleset you can pass it in as a parameter to the sanitize method:

```html
var html = "<h1>My Heading</h1><script>alert('uh oh')</script>";
_htmlSanitizer.Sanitize(html, ruleSet)

```

## Changing the default ruleset

If you need to change the default ruleset, you can do so by replacing the default `IDefaultHtmlSanitizationRuleSetFactory` implementation using the [DI override system](dependency-injection#overriding-registrations). 

Here's an example `IDefaultHtmlSanitizationRuleSetFactory` implementation.

```csharp
using Cofoundry.Core.Web;
using Ganss.Xss;
using System.Collections.Immutable;

public class ExampleHtmlSanitizationRuleSetFactory : IDefaultHtmlSanitizationRuleSetFactory
{
    private readonly Lazy<HtmlSanitizationRuleSet> _defaultRulset = new(Initizalize);

    public IHtmlSanitizationRuleSet Create()
    {
        return _defaultRulset.Value;
    }

    private static HtmlSanitizationRuleSet Initizalize()
    {
        var ruleSet = new HtmlSanitizationRuleSet();

        ruleSet.PermittedAtRules = HtmlSanitizerDefaults
            .AllowedAtRules
            .ToImmutableHashSet();

        ruleSet.PermittedAttributes = HtmlSanitizerDefaults
            .AllowedAttributes
            .Append("class")
            .ToImmutableHashSet();

        ruleSet.PermittedCssClasses = HtmlSanitizerDefaults
            .AllowedClasses
            .ToImmutableHashSet();

        ruleSet.PermittedCssProperties = HtmlSanitizerDefaults
            .AllowedCssProperties
            .ToImmutableHashSet();

        ruleSet.PermittedSchemes = HtmlSanitizerDefaults
            .AllowedSchemes
            .Append("mailto")
            .ToImmutableHashSet();

        ruleSet.PermittedTags = HtmlSanitizerDefaults
            .AllowedTags
            .ToImmutableHashSet();

        ruleSet.PermittedUriAttributes = HtmlSanitizerDefaults
            .UriAttributes
            .ToImmutableHashSet();

        return ruleSet;
    }
}
```
