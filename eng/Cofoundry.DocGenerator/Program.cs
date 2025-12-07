using Cofoundry.DocGenerator;
using Microsoft.Extensions.Configuration;

var settings = ParseConfigSettings();
var logger = Console.Out;
var docGenerator = new DocGenerator(settings, logger);

try
{
    await docGenerator.GenerateAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}

static DocGeneratorSettings ParseConfigSettings()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.local.json", true);

    var configuration = builder.Build();

    var settings = configuration.Get<DocGeneratorSettings>();

    if (settings == null)
    {
        throw new Exception($"Could not bind {nameof(DocGeneratorSettings)}");
    }
    return settings;
}
