using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Sandbox
{
    public class CatClass : IExampleClass
    {
        public string Title { get; set; }
    }

    public class DogClass : IExampleClass
    {
        public string Title { get; set; }
    }

    public interface IExampleClass
    {
        string Title { get; set; }
    }
}
