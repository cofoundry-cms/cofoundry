# Cofoundry.Build

The build project must be run from the repository root. 

To run the build locally:

```bash
# default target (tests)
dotnet run --project eng/Cofoundry.Build

# run target: tests
dotnet run --project eng/Cofoundry.Build -- test

# run target: publish
dotnet run --project eng/Cofoundry.Build -- publish
```

To publish packages you will need to supply the relevant NuGet feed API keys:

| Environment | Feed  | Environment Variable |
|-------------|-------|----------------------|
| Prerelease  | MyGet | MYGET_API_KEY        |
| Release     | NuGet | NUGET_API_KEY        |

