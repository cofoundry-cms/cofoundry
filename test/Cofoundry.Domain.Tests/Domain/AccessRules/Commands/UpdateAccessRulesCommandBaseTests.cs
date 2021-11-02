using Cofoundry.Core.Validation.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Linq;
using Xunit;

namespace Cofoundry.Domain.Tests.Domain.AccessRules
{
    public class UpdateAccessRulesCommandBaseTests
    {
        [Fact]
        public void Validate_WhenAccessRuleIsCofoundryUserArea_ReturnsError()
        {
            var command = new UpdatePageAccessRulesCommand();
            command.PageId = 1;
            command.AccessRules
                .AddNew("TST")
                .AddNew(CofoundryAdminUserArea.AreaCode);

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
            var command = new UpdatePageAccessRulesCommand();
            command.PageId = 1;
            command.UserAreaCodeForLoginRedirect = CofoundryAdminUserArea.AreaCode;
            command.AccessRules
                .AddNew("TST")
                .AddNew(CofoundryAdminUserArea.AreaCode);

            var validationService = new ModelValidationService();
            var errors = validationService.GetErrors(command);

            using (new AssertionScope())
            {
                errors.Should().HaveCount(2);
                var error = errors.Single(e => e.Properties.Contains(nameof(command.UserAreaCodeForLoginRedirect)));
                error.Should().NotBeNull();
                error.Properties.Should().HaveCount(1);
                error.Message.Should().Match("*redirect*admin user area*");
            }
        }

        [Fact]
        public void Validate_WhenRedirectAreaIsNotInAccessRules_ReturnsError()
        {
            var command = new UpdatePageAccessRulesCommand();
            command.PageId = 1;
            command.UserAreaCodeForLoginRedirect = "NON";
            command.AccessRules.AddNew("TST");

            var validationService = new ModelValidationService();
            var errors = validationService.GetErrors(command);

            using (new AssertionScope())
            {
                errors.Should().HaveCount(1);
                var error = errors.Single(e => e.Properties.Contains(nameof(command.UserAreaCodeForLoginRedirect)));
                error.Should().NotBeNull();
                error.Properties.Should().HaveCount(1);
                error.Message.Should().Match("*redirect*login*access rules*");
            }
        }

        [Fact]
        public void Validate_WhenDuplicates_ReturnsError()
        {
            var command = new UpdatePageAccessRulesCommand();
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
            var command = new UpdatePageAccessRulesCommand();
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
}
