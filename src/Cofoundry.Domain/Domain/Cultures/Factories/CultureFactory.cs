using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A culture factory that allows the creation and registration of custom cultures.
    /// </summary>
    /// <remarks>
    /// This functionality has been copied across from an older part of Cofoundry and is not known to be in use in recent (2014) projects
    /// and therefore may need some work when it comes to be required again.
    /// </remarks>
    public class CultureFactory : ICultureFactory
    {
        private static readonly CultureInfo[] _customCultures = new CultureInfo[] { new EnglishUAECulture() };
 
        public CultureInfo Create(string languageTag)
        {
            var culture = _customCultures.SingleOrDefault(c => c.Name.Equals(languageTag, StringComparison.OrdinalIgnoreCase));

            if (culture != null) return culture;

            return new CultureInfo(languageTag);
        }

        // ******************************************************************************************************************
        // JM: Not sure if this code is doing anything? It is supposed to register the custom culture in the machine on startup
        // so that you can create resx files with the culture, but because the cultureAndRegionInfoBuilder.Register() code
        // is commented out I can't see that it is doing anything? 
        // I'm going to leave it out for now and then we can re-implement it when a use-case comes available. I imagine a better idea would be
        // to use DI to request an array of custom CultureInfo objects and register them on the fly, or maybe it would be better to provide the facilities
        // to hook this stuff up but let the website implementer decide when they need the custom cultures rather than boostrap them for every website.
        // ******************************************************************************************************************

        ///// <summary>
        ///// Registers any custom cultures
        ///// </summary>
        ///// <see cref="http://stackoverflow.com/a/1304569/486434"/>
        //public static void RegisterCustomCultures()
        //{
        //    RegisterCustomCulture("en-GB", "en-AE", "English - United Arab Emirates");
        //}

        //private static void RegisterCustomCulture(string cultureToDuplicate, string newCultureCode, string newCultureName)
        //{
        //    CultureInfo cultureInfo = new CultureInfo(cultureToDuplicate);
        //    RegionInfo regionInfo = new RegionInfo(cultureInfo.Name);
        //    var cultureAndRegionInfoBuilder = new CultureAndRegionInfoBuilder(newCultureCode, CultureAndRegionModifiers.Neutral);

        //    cultureAndRegionInfoBuilder.LoadDataFromCultureInfo(cultureInfo);
        //    cultureAndRegionInfoBuilder.LoadDataFromRegionInfo(regionInfo);

        //    // Custom Changes
        //    cultureAndRegionInfoBuilder.CultureEnglishName = newCultureName;
        //    cultureAndRegionInfoBuilder.CultureNativeName = newCultureName;

        //    //cultureAndRegionInfoBuilder.Register();
        //}
    }
}
