using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public class TemplateModelParserException : Exception
    {
        const string MESSAGE = "Unable to parse mail template data";

        public TemplateModelParserException()
            : base(MESSAGE)
        {
        }

        public TemplateModelParserException(int mailQueueId, string templateData, string templateDataType, Exception innerEx)
            : base(MESSAGE, innerEx)
        {
            MailQueueId = mailQueueId;
            TemplateData = templateData;
            TemplateDataType = templateDataType;
        }

        public int MailQueueId { get; set; }

        public string TemplateData { get; set; }

        public string TemplateDataType { get; set; }
    }
}
