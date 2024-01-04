using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain.Tests.Domain.AccessRules;

public class UpdateAccessRuleSetCommandBaseTests
{
    [Fact]
    public void Validate_WhenAccessRuleIsCofoundryUserArea_ReturnsError()
    {
        var command = new UpdatePageAccessRuleSetCommand
        {
            PageId = 1,
            AccessRules = [
                new()
                {
                    UserAreaCode = "TST"
                },
                new()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code
                }]
        };

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
        var command = new UpdatePageAccessRuleSetCommand
        {
            PageId = 1,
            UserAreaCodeForSignInRedirect = CofoundryAdminUserArea.Code,
            AccessRules = [
                new()
                {
                    UserAreaCode = "TST"
                },
                new()
                {
                    UserAreaCode = CofoundryAdminUserArea.Code
                }]
        };

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
        var command = new UpdatePageAccessRuleSetCommand
        {
            PageId = 1,
            UserAreaCodeForSignInRedirect = "NON",
            AccessRules = [
                new()
                {
                    UserAreaCode = "TST"
                }]
        };

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
        var command = new UpdatePageAccessRuleSetCommand
        {
            PageId = 1,
            AccessRules = [
                new()
                {
                    UserAreaCode = "TST"
                },
                new()
                {
                    UserAreaCode = "TST"
                },
                new()
                {
                    UserAreaCode = "TST",
                    RoleId = 9
                },
                new()
                {
                    UserAreaCode = "BLH"
                },
                new()
                {
                    UserAreaCode = "BLH",
                    RoleId = 6
                },
                new()
                {
                    UserAreaCode = "BLH",
                    RoleId = 6
                }]
        };

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
        var command = new UpdatePageAccessRuleSetCommand
        {
            PageId = 1,
            ViolationAction = (AccessRuleViolationAction)Int32.MinValue,
            AccessRules = [new() { UserAreaCode = "TST" }]
        };

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
