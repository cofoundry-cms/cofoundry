using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing multiple lines of simple text 
    /// </summary>
    public class PlainTextDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
    {
        [MultiLineText]
        public string PlainText { get; set; }
    }
}