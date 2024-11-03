*While Cofoundry does technically support localized sites, this area has been a little neglected and not given a thorough update since the .NET Core conversion.*

*The work for this is tracked in issue #43. There will almost certainly be breaking changes when we do this work as localization changed significantly in .NET Core.*

*If you can't wait for #43 to be completed and want to use the existing locale system then please bear in mind that it has not been extensively tested with .NET Core and there will be future breaking changes.*

## Active Locales

To make use of the locale system, you'll first need to set the `IsActive` flag to true for the locales you want to use in the `Cofoundry.Locale` database table.

Active locales are cached so you'll need to restart the application or break the cache to see the changes.

## Localization startup

Please refer to issue #43 for .NET Core localization startup workarounds.

## Assigning Locales

The locale system only comes into play when you have more than one active locale. With more than one active you'll find that when creating pages and custom entities you'll be able to select a locale.




