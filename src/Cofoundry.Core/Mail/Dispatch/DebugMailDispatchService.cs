using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail.Internal
{
    /// <summary>
    /// This is a simple debug implementation that writes out mail
    /// to a text file to make debugging templates easier. This is 
    /// also the default Cofoundry mail dispatch service and needs
    /// to be overidden by a plugin in order to actually dispatch 
    /// email.
    /// </summary>
    public class DebugMailDispatchService :  IMailDispatchService
    {
        #region constructor

        private readonly MailSettings _mailSettings;
        private readonly IPathResolver _pathResolver;
        
        public DebugMailDispatchService(
            IPathResolver pathResolver,
            MailSettings mailSettings
            )
        {
            _mailSettings = mailSettings;
            _pathResolver = pathResolver;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public async Task DispatchAsync(MailMessage message)
        {
            using (var session = CreateSession())
            {
                session.Add(message);
                await session.FlushAsync();
            }
        }

        /// <summary>
        /// Creates a new mail session that can be used to send batches of mail.
        /// </summary>
        /// <returns>New instance of an IMailDispatchSession</returns>
        public IMailDispatchSession CreateSession()
        {
            return new DebugMailDispatchSession(_mailSettings, _pathResolver);
        }

        #endregion
    }
}
