using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain.Tests.Domain.AccessRules;

public class UpdateAccessRuleSetCommandBaseTests
{
    [Fact]
    public void Validate_WhenAccessRuleIsCofoundryUserArea_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand();
        command.PageId = 1;
        command.AccessRules
            .AddNew("TST")
            .AddNew(CofoundryAdminUserArea.Code);

        var validationService = new ModelValidationService();
        var errors = validationService.GetErrors(command);

        using (new AssertionScope())
        {
            errors.Should().HaveCount(1);
            var error = errors.Single();
            error.Properties.Should().HaveCount(1);
            error.Properties.First().Should().Be("UserAreaCode");
            error.Message.Should().Match("*added*admin user area*");
        }
    }

    [Fact(Skip = "Multiple errors aren't provided with the current ModelValidationService, so other errors are reported ahead of this one. A new implementation of ModelValidationService might fix this.")]
    public void Validate_WhenRedirectAreaIsCofoundryUserArea_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand();
        command.PageId = 1;
        command.UserAreaCodeForSignInRedirect = CofoundryAdminUserArea.Code;
        command.AccessRules
            .AddNew("TST")
            .AddNew(CofoundryAdminUserArea.Code);

        var validationService = new ModelValidationService();
        var errors = validationService.GetErrors(command);

        using (new AssertionScope())
        {
            errors.Should().HaveCount(2);
            var error = errors.Single(e => e.Properties.Contains(nameof(command.UserAreaCodeForSignInRedirect)));
            error.Should().NotBeNull();
            error.Properties.Should().HaveCount(1);
            error.Message.Should().Match("*redirect*admin user area*");
        }
    }

    [Fact]
    public void Validate_WhenRedirectAreaIsNotInAccessRules_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand();
        command.PageId = 1;
        command.UserAreaCodeForSignInRedirect = "NON";
        command.AccessRules.AddNew("TST");

        var validationService = new ModelValidationService();
        var errors = validationService.GetErrors(command);

        using (new AssertionScope())
        {
            errors.Should().HaveCount(1);
            var error = errors.Single(e => e.Properties.Contains(nameof(command.UserAreaCodeForSignInRedirect)));
            error.Should().NotBeNull();
            error.Properties.Should().HaveCount(1);
            error.Message.Should().Match("*redirect*sign in*access rules*");
        }
    }

    [Fact]
    public void Validate_WhenDuplicates_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand();
        command.PageId = 1;
        command.AccessRules.AddNew("TST");
        command.AccessRules.AddNew("TST");
        command.AccessRules.AddNew("TST", 9);
        command.AccessRules.AddNew("BLH");
        command.AccessRules.AddNew("BLH", 6);
        command.AccessRules.AddNew("BLH", 6);

        var validationService = new ModelValidationService();
        var errors = validationService.GetErrors(command);

        using (new AssertionScope())
        {
            errors.Should().HaveCount(2);

            var roleError = errors.Single(e => e.Message.Contains("role"));
            roleError.Should().NotBeNull();
            roleError.Properties.Single().Should().Be(nameof(command.AccessRules));
            roleError.Message.Should().Match("Duplicate*role*");

            var userAreaError = errors.Single(e => e.Message.Contains("user area"));
            userAreaError.Should().NotBeNull();
            userAreaError.Properties.Single().Should().Be(nameof(command.AccessRules));
            userAreaError.Message.Should().Match("Duplicate*user area*");
        }
    }

    [Fact]
    public void Validate_WhenBadEnum_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand();
        command.PageId = 1;
        command.AccessRules.AddNew("TST");
        command.ViolationAction = (AccessRuleViolationAction)Int32.MinValue;

        var validationService = new ModelValidationService();
        var errors = validationService.GetErrors(command);

        using (new AssertionScope())
        {
            errors.Should().HaveCount(1);
            var error = errors.Single(e => e.Properties.Contains(nameof(command.ViolationAction)));
            error.Should().NotBeNull();
            error.Properties.Should().HaveCount(1);
        }
    }
}
