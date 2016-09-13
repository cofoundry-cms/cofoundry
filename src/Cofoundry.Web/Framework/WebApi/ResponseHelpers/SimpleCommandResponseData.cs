using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;

namespace Cofoundry.Web.WebApi
{
    public class SimpleCommandResponseData<T>
    {
        public bool IsValid { get; set; }
        public IEnumerable<ValidationError> Errors { get; set; }
        public T Data { get; set; }
    }

    public class SimpleCommandResponseData
    {
        public bool IsValid { get; set; }
        public IEnumerable<ValidationError> Errors { get; set; }
    }
}
