using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared.Mocks
{
    public class MockClientConnectionService : IClientConnectionService
    {
        public MockClientConnectionService() { }

        public MockClientConnectionService(Action<ClientConnectionInfo> configure)
        {
            configure(ClientConnectionInfo);
        }

        public ClientConnectionInfo ClientConnectionInfo { get; set; } = new ClientConnectionInfo();

        public ClientConnectionInfo GetConnectionInfo()
        {
            return ClientConnectionInfo;
        }
    }
}
