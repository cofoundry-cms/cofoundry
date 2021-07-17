using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class UserAuthenticationHelperTests
    {
        [Fact]
        public void VerifyPassword_WhenUserNull_ReturnsFailure()
        {
            var helper = CreateUserAuthenticationHelper();

            var result = helper.VerifyPassword(null, "not required");

            Assert.Equal(PasswordVerificationResult.Failed, result);
        }

        [Fact]
        public void VerifyPassword_WhenUserAreaDoesNotAllowPasswordLogin_Throws()
        {
            var userAreaCode = "T01";
            var userArea = CreateUserAreaDefinition(userAreaCode, false);
            var helper = CreateUserAuthenticationHelper(userArea);
            var user = new User()
            {
                UserAreaCode = userAreaCode
            };

            Assert.Throws<InvalidOperationException>(() => helper.VerifyPassword(user, "not required"));
        }

        [Fact]
        public void VerifyPassword_WhenUserIsMissingPassword_Throws()
        {
            var userAreaCode = "T01";
            var userArea = CreateUserAreaDefinition(userAreaCode, true);
            var helper = CreateUserAuthenticationHelper(userArea);
            var user = new User()
            {
                UserAreaCode = userAreaCode,
                PasswordHashVersion = (int)PasswordHashVersion.V3
            };

            Assert.Throws<InvalidOperationException>(() => helper.VerifyPassword(user, "not required"));
        }

        [Fact]
        public void VerifyPassword_WhenUserIsMissingPasswordHasVersion_Throws()
        {
            var userAreaCode = "T01";
            var userArea = CreateUserAreaDefinition(userAreaCode, true);
            var helper = CreateUserAuthenticationHelper(userArea);
            var user = new User()
            {
                UserAreaCode = userAreaCode,
                Password = "test"
            };

            Assert.Throws<InvalidOperationException>(() => helper.VerifyPassword(user, "not required"));
        }

        private IUserAreaDefinition CreateUserAreaDefinition(string code, bool allowPasswordLogin)
        {
            var userAreaDefinition = new Mock<IUserAreaDefinition>();
            userAreaDefinition.SetupGet(o => o.AllowPasswordLogin).Returns(allowPasswordLogin);
            userAreaDefinition.SetupGet(o => o.UserAreaCode).Returns(code);
            userAreaDefinition.SetupGet(o => o.Name).Returns(code);

            return userAreaDefinition.Object;
        }

        public UserAuthenticationHelper CreateUserAuthenticationHelper(params IUserAreaDefinition[] userAreaDefinitions)
        {
            var userAreaRepository = new UserAreaDefinitionRepository(userAreaDefinitions);

            var passwordCryptographyServiceMock = new Mock<IPasswordCryptographyService>();
            passwordCryptographyServiceMock
                .Setup(c => c.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(PasswordVerificationResult.Success);

            var helper = new UserAuthenticationHelper(passwordCryptographyServiceMock.Object, userAreaRepository);

            return helper;
        }
    }
}
