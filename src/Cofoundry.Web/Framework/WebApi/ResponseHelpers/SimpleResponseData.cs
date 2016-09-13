using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.WebApi
{
    [Serializable]
    public class SimpleResponseData<T>
    {
        /// <summary>
        /// Any additional data to send back to the response.
        /// </summary>
        public T Data { get; set; }
    }
}
