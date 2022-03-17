namespace Cofoundry.Domain.Tests.Shared;

public class TestAuthorizedTaskType1 : IAuthorizedTaskTypeDefinition
{
    public const string Code = "TST001";

    public string AuthorizedTaskTypeCode => Code;

    public string Name => "Test 1";
}
