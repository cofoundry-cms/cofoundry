﻿namespace Cofoundry.Domain.Tests.Shared.Mocks;

public class MockClientConnectionService : IClientConnectionService
{
    public MockClientConnectionService()
    {
        ClientConnectionInfo.IPAddress = TestIPAddresses.Localhost;
    }

    public MockClientConnectionService(Action<ClientConnectionInfo> configure)
    {
        configure(ClientConnectionInfo);
    }

    public ClientConnectionInfo ClientConnectionInfo { get; set; } = new()
    {
        IPAddress = IPAddressConstants.Default
    };

    public ClientConnectionInfo GetConnectionInfo()
    {
        return ClientConnectionInfo;
    }
}
