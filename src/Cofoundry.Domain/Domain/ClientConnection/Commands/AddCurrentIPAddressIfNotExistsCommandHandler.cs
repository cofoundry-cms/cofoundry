using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data.Internal;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds the IP Address of the currently connected client in user to the 
    /// IPAddress table. The client IPAddress may not always be accessible
    /// (e.g due to privacy) and in this case no action is taken.
    /// </summary>
    public class AddCurrentIPAddressIfNotExistsCommandHandler
        : ICommandHandler<AddCurrentIPAddressIfNotExistsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IClientConnectionService _clientConnectionService;
        private readonly IIPAddressStoredProcedures _ipAddressStoredProcedures;

        public AddCurrentIPAddressIfNotExistsCommandHandler(
            IClientConnectionService clientConnectionService,
            IIPAddressStoredProcedures ipAddressStoredProcedures
            )
        {
            _clientConnectionService = clientConnectionService;
            _ipAddressStoredProcedures = ipAddressStoredProcedures;
        }

        public async Task ExecuteAsync(AddCurrentIPAddressIfNotExistsCommand command, IExecutionContext executionContext)
        {
            var connectionInfo = _clientConnectionService.GetConnectionInfo();
            EntityNotFoundException.ThrowIfNull(connectionInfo, null);

            if (string.IsNullOrWhiteSpace(connectionInfo.IPAddress)) return;

            command.OutputIPAddressId = await _ipAddressStoredProcedures.AddIfNotExistsAsync(connectionInfo.IPAddress, executionContext.ExecutionDate);
        }
    }
}
