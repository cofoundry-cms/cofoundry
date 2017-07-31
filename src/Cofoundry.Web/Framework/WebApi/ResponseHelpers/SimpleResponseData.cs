using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// A simple container for wrapping data output via a rest api. Used
    /// to provide a consistent response from a rest api and to prevent
    /// potential JSON hijacking vulnerability.
    /// </summary>
    /// <typeparam name="T">Type of the data being returned</typeparam>
    public class SimpleResponseData<T>
    {
        /// <summary>
        /// Any additional data to send back to the response.
        /// </summary>
        public T Data { get; set; }
    }
}
