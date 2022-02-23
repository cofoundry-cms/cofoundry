using Cofoundry.Domain.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Roles
{
    public class PermissionSetBuilderTests
    {
        [Fact]
        public void Build_WhenNoMethodCalled_ReturnsEmpty()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);

            var result = builder.Build();

            using (new AssertionScope())
            {
                allPermissions.Should().NotBeNullOrEmpty();
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [Fact]
        public void IncludeAll_AddAll()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);

            var result = builder
                .IncludeAll()
                .Build();

            result.Should().HaveCount(allPermissions.Count());
        }

        [Fact]
        public void IncludeAll_WhenInvokedTwice_ReturnsNoDuplicates()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);

            var result = builder
                .IncludeAll()
                .IncludeAll()
                .Build();

            var duplicates = result
                .GroupBy(p => p.GetUniqueIdentifier())
                .Where(g => g.Count() > 1);

            using (new AssertionScope())
            {
                result.Should().HaveSameCount(allPermissions);
                duplicates.Should().BeEmpty();
            }
        }

        [Fact]
        public void CanIncludeSinglePermission()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);
            var numCreatePermissions = allPermissions.Count(p => p.PermissionType.Code == CommonPermissionTypes.CreatePermissionCode);

            var result = builder
                .Include<CofoundryUserReadPermission>()
                .Build();

            using (new AssertionScope())
            {
                numCreatePermissions.Should().BePositive();
                result.Should().HaveCount(1);
                result.First().Should().BeOfType<CofoundryUserReadPermission>();
            }
        }

        [Fact]
        public void CanExludeByPermissionType()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);
            var numCreatePermissions = allPermissions.Count(p => p.PermissionType.Code == CommonPermissionTypes.CreatePermissionCode);

            var result = builder
                .IncludeAll()
                .ExcludeAllCreate()
                .Build();

            var numCreatesInResult = result.Count(p => p.PermissionType.Code == CommonPermissionTypes.CreatePermissionCode);

            using (new AssertionScope())
            {
                numCreatePermissions.Should().BePositive();
                result.Should().HaveCount(allPermissions.Count() - numCreatePermissions);
            }
        }

        [Fact]
        public void CanApplyRoleConfiguration()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);
            var anonymousRolePermissions = allPermissions.FilterToAnonymousRoleDefaults();

            var result = builder
                .ApplyAnonymousRoleConfiguration()
                .Build();

            using (new AssertionScope())
            {
                result.Should().HaveSameCount(anonymousRolePermissions);
                result.Should().Contain(anonymousRolePermissions);
                result.Should().Contain(p => p is PageReadPermission);
            }
        }

        [Fact]
        public void CanUseEntityBuilders()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = GetPermissions(serviceProvider);
            var builder = CreateBuilder(serviceProvider, allPermissions);

            var result = builder
                .IncludePage()
                .ExcludePage(c => c.Special().AdminModule())
                .IncludeImageAsset(c => c.Read().Delete())
                .IncludeCustomEntity<TestCustomEntityDefinition>(c => c.Read())
                .Build();

            using (new AssertionScope())
            {
                result.Should().HaveCount(7);
                result.Should().Contain(p => p is PageReadPermission);
                result.Should().Contain(p => p is PageCreatePermission);
                result.Should().Contain(p => p is PageUpdatePermission);
                result.Should().Contain(p => p is PageDeletePermission);
                result.Should().Contain(p => p is ImageAssetReadPermission);
                result.Should().Contain(p => p is ImageAssetDeletePermission);
                result.Should().Contain(p => p.GetUniqueIdentifier() == TestCustomEntityDefinition.Code + CommonPermissionTypes.ReadPermissionCode);
            }
        }

        [Fact]
        public void WhenNotInAvailablePermissions_DoesNotAdd()
        {
            using var serviceProvider = DbDependentTestApplicationServiceProviderFactory.CreateTestHostProvider();
            var allPermissions = new IPermission[] 
            { 
                new PageDirectoryReadPermission(),
                new PageDirectoryDeletePermission()
            };
            var builder = CreateBuilder(serviceProvider, allPermissions);

            var result = builder
                .IncludePageDirectory()
                .IncludeRole()
                .IncludeAllUpdate()
                .Build();

            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                result.Should().Contain(p => p is PageDirectoryReadPermission);
                result.Should().Contain(p => p is PageDirectoryDeletePermission);
            }
        }

        private IEnumerable<IPermission> GetPermissions(IServiceProvider serviceProvider)
        {
            var permissionsRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
            var permissions = permissionsRepository.GetAll();

            return permissions;
        }

        private IPermissionSetBuilder CreateBuilder(IServiceProvider serviceProvider, IEnumerable<IPermission> permissions)
        {
            var factory = serviceProvider.GetRequiredService<IPermissionSetBuilderFactory>();
            var builder = factory.Create(permissions);

            return builder;
        }
    }
}
