using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public class ViewNotFoundException : Exception
    {
        public ViewNotFoundException() { }

        public ViewNotFoundException(string viewName, IEnumerable<string> searchedLocations)
        {
            ViewName = viewName;
            SearchedLocations = searchedLocations;
        }

        public string ViewName { get; private set; }

        public IEnumerable<string> SearchedLocations { get; private set; }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(ViewName)) return "View not found - ViewName not specified.";

                var searchedLocations = String.Join(Environment.NewLine, EnumerableHelper.Enumerate(SearchedLocations));
                return $"View not found '{ViewName}'. Searched locations: { searchedLocations }";
            }
        }
    }
}