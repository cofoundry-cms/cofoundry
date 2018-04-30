using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An option in a list component in the admin panel e.g. select list
    /// or radiobutton list.
    /// </summary>
    public class ListOption
    {
        public ListOption() { }

        public ListOption(string text, object value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        /// The display text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The option value.
        /// </summary>
        public object Value { get; set; }
    }
}
