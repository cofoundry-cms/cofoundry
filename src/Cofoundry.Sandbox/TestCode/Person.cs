using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Sandbox
{
    public class AddPersonCommand
    {
        public string Title { get; set; }

        public string TypeCode { get; set; }

        public IPersonData Data { get; set; }
    }

    public class ManagerData : IPersonData
    {
        public string Test2 { get; set; }
    }

    public class WorkerData : IPersonData
    {
        public string Test1 { get; set; }
    }

    public interface IPersonData
    {

    }
}
