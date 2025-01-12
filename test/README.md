# Cofoundry: Tests

## Running Tests

Many of the tests projects run integrated with local services such as databases provided via [test containers](https://dotnet.testcontainers.org/). As long as you have a local docker daemon running you will be able to run the tests e.g. via [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Rancher Desktop](https://rancherdesktop.io/).

### Using long-lived containers

To avoid the long startup times associated with running test containers on each local test run you can instead use docker compose with the `docker-compose.test.yml` file to run long-lived container instances:

```
docker compose -f docker-compose.test.yml up
```

Once running you just need to configure the connection strings for each project to point to the docker resources. Do this by creating your own `appsettings.local.json` file in the following projects:

**Cofoundry.Domain.Tests.Integration:**

```json
{
  "Cofoundry": {
    "Database:ConnectionString": "Data Source=127.0.0.1,57681;Initial Catalog=master;User ID=sa;Password=Integrati0nTest$;TrustServerCertificate=True"
  }
}
```

**Cofoundry.Web.Tests.Integration:**

```json
{
  "Cofoundry": {
    "Database:ConnectionString": "Data Source=127.0.0.1,57682;Initial Catalog=master;User ID=sa;Password=Integrati0nTest$;TrustServerCertificate=True"
  }
}
```

**Cofoundry.Plugins.Azure.Tests:**

```json
{
  "Cofoundry": {
    "Plugins:Azure": {
      "BlobStorageConnectionString": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:57683/devstoreaccount1;QueueEndpoint=http://127.0.0.1:57683/devstoreaccount1;"
    }
  }
}
```

### Parallel test execution

To run test projects in parallel you need to configure your IDE to use the [dev.runsettings](dev.runsettings) file. See the [Microsoft docs for more info](https://learn.microsoft.com/en-gb/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?branch=release-16.4&view=vs-2019#manually-select-the-run-settings-file).



