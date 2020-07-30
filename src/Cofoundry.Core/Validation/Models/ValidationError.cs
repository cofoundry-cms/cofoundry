using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    public class ValidationError
    {
        public ValidationError()
        {
        }

        public ValidationError(string message, string property = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message");
            }

            Message = message;
            if (string.IsNullOrWhiteSpace(property))
            {
                Properties = new string[1] { property };
            }
        }

        public ICollection<string> Properties { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }
    }
}
