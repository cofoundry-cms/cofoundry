namespace Cofoundry.Domain.Tests.Shared;

public class TestAuthorizedTaskType2 : IAuthorizedTaskTypeDefinition
{
    public const string Code = "TST002";

    public string AuthorizedTaskTypeCode => Code;

    public string Name => "Test 2";
}
