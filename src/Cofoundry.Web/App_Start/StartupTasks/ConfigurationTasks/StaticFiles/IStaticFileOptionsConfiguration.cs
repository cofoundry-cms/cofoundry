using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Web;

public interface IStaticFileOptionsConfiguration
{
    void Configure(StaticFileOptions options);
}
