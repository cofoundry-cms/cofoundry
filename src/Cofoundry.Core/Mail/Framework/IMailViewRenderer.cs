using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public interface IMailViewRenderer
    {
        string Render(string viewPath);
        string Render<T>(string viewPath, T model);
    }
}
