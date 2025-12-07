using Cofoundry.SamplesGenerator;
using Microsoft.Extensions.Configuration;

var settings = ParseConfigSettings();
var logger = Console.Out;
var docGenerator = new SamplesGenerator(settings, logger);

await docGenerator.Run();

static SamplesGeneratorSettings ParseConfigSettings()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.local.json", true);

    var configuration = builder.Build();

    var settings = configuration.Get<SamplesGeneratorSettings>();

    if (settings == null)
    {
        throw new Exception($"Could not bind {nameof(SamplesGeneratorSettings)}");
    }

    return settings;
}
