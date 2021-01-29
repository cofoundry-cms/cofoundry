using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to define custom data properties to save against 
    /// a custom entity. Data annotations can be used to describe 
    /// the model which influences how the fields are displayed in the
    /// editor in the admin panel.
    /// </summary>
    public interface ICustomEntityDataModel : IEntityDataModel
    {
    }
}
