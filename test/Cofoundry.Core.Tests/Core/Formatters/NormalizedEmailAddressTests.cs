namespace Cofoundry.Core.Tests.Core.Formatters;

public class NormalizedEmailAddressTests
{
    private const string EXAMPLE_LOCAL = "bill.ben";
    private static readonly EmailDomainName EXAMPLE_DOMAIN = EmailDomainName.Parse("example.com");
    private static readonly NormalizedEmailAddress ORIGINAL_EMAIL = new NormalizedEmailAddress(EXAMPLE_LOCAL, EXAMPLE_DOMAIN);

    [Fact]
    public void AlterDomain_CanAlter()
    {
        var result = ORIGINAL_EMAIL.AlterDomain(domain => "pottingshed." + domain);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(EXAMPLE_LOCAL);
            result.Domain.Name.Should().Be("pottingshed.example.com");
        }
    }

    [Fact]
    public void AlterDomainIfTrue_IfTrue_Alters()
    {
        var result = ORIGINAL_EMAIL.AlterDomainIf(email => email.Local == EXAMPLE_LOCAL, domain => "pottingshed." + domain);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(ORIGINAL_EMAIL.Local);
            result.Domain.Name.Should().Be("pottingshed.example.com");
        }
    }

    [Fact]
    public void AlterDomainIfTrue_IfFalse_DoesNotAlter()
    {
        var result = ORIGINAL_EMAIL.AlterDomainIf(email => email.Local != EXAMPLE_LOCAL, domain => "pottingshed." + domain);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(ORIGINAL_EMAIL.Local);
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }

    [Fact]
    public void AlterLocal_CanAlter()
    {
        var result = ORIGINAL_EMAIL.AlterLocal(local => local.Replace(".", "-and-"));

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be("bill-and-ben");
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }

    [Fact]
    public void AlterLocalIfTrue_IfTrue_Alters()
    {
        var result = ORIGINAL_EMAIL.AlterLocalIf(email => email.Local == EXAMPLE_LOCAL, local => local.Replace(".", "-and-"));

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be("bill-and-ben");
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }

    [Fact]
    public void AlterLocalIfTrue_IfFalse_DoesNotAlter()
    {
        var result = ORIGINAL_EMAIL.AlterLocalIf(email => email.Local != EXAMPLE_LOCAL, local => local.Replace(".", "-and-"));

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(ORIGINAL_EMAIL.Local);
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }

    [Theory]
    [InlineData("a.example.com")]
    [InlineData("b.example.com")]
    [InlineData("c.example.com")]
    public void HasDomain_IfMatch_ReturnsTrue(string domain)
    {
        var email = ORIGINAL_EMAIL.AlterDomain(m => domain);
        var result = email.HasDomain("a.example.com", "b.example.com", "c.example.com");

        result.Should().BeTrue();
    }

    [Fact]
    public void HasDomain_IfNotMatch_ReturnsFalse()
    {
        var result = ORIGINAL_EMAIL.HasDomain("a.example.com", "b.example.com", "c.example.com");

        result.Should().BeFalse();
    }

    [Fact]
    public void MergeDomains_IMatch_Alters()
    {
        var result = ORIGINAL_EMAIL.MergeDomains("pottingshed.example.com", "example.com", "b.example.com");

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(EXAMPLE_LOCAL);
            result.Domain.Name.Should().Be("pottingshed.example.com");
        }
    }

    [Fact]
    public void MergeDomains_IfNotMatch_DoesNotAlter()
    {
        var result = ORIGINAL_EMAIL.MergeDomains("primary.example.com", "b.example.com", "c.example.com");

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(EXAMPLE_LOCAL);
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }

    [Theory]
    [InlineData("bill", "bill")]
    [InlineData("ben+flobadob", "ben")]
    [InlineData("bill+ben+little-weed", "bill")]
    [InlineData("+bill+ben", "+bill+ben")]
    public void WithoutPlusAddressing_MatchesExpected(string testLocal, string expectedLocal)
    {
        var email = ORIGINAL_EMAIL.AlterLocal(m => testLocal);
        var result = email.WithoutPlusAddressing();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Local.Should().Be(expectedLocal);
            result.Domain.Should().Be(EXAMPLE_DOMAIN);
        }
    }
}
