using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to format the 'FileName' property for a PageBlockType base
    /// on set postfix conventions.
    /// </summary>
    public interface IPageBlockTypeFileNameFormatter
    {
        /// <summary>
        /// Formats the clean block type file name from the name of the
        /// specified data model type . E.g. for 'SingleLineTextDataModel' 
        /// this returns 'SingleLineText'. This should be fairly forgiving, e.g. if 
        /// the postfix isn't in the model name it should just pass back the model name.
        /// </summary>
        /// <param name="dataModelType">Data model type to get the formatted name for</param>
        string FormatFromDataModelType(Type dataModelType);

        /// <summary>
        /// Formats the clean block type file name from the data model
        /// type name. E.g. for 'SingleLineTextDataModel' this returns
        /// 'SingleLineText'. This should be fairly forgiving, e.g. if 
        /// I pass in 'SingleLineText' I should receive 'SingleLineText' back. 
        /// </summary>
        /// <param name="dataModelName">Type name without namespace of the block type data model</param>
        string FormatFromDataModelName(string dataModelName);
    }
}
