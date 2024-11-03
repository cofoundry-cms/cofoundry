Cofoundry comes with a number of features included as standard which makes it easier to get started building content managed websites. Although this is a popular use-case, Cofoundry can be used to build many other types of applications where some of these traditional features are not needed. 

## Disabling Content Features

The standard set of content management features can be disabled using the following settings:

- **Cofoundry:DocumentAssets:Disabled** Disables document asset functionality, removing it from the admin panel and skipping registration of document asset routes. Access to document is still possible from code if you choose to use those APIs from a user account with permissions.
- **Cofoundry:ImageAssets:Disabled** Disables image asset functionality, removing it from the admin panel and skipping registration of image asset routes. Access to images is still possible from code if you choose to use those APIs from a user account with permissions.
- **Cofoundry:Pages:Disabled** Disables the pages functionality, removing page, directories and page templates from the admin panel and skipping registration of the dynamic page route and visual editor. Access to pages is still possible from code if you choose to use those APIs from a user account with permissions.

## Disabling the admin panel

The admin panel is included in a separate NuGet package, so one way to remove the admin panel is simply to not install it in the first place. However you can also disable this via a config setting:

- **Cofoundry:Admin:Disabled** Disables the admin panel, removing all routes from the routing table and disabling sign in.

Note that you can also change the admin directory name via config too:

- **Cofoundry:Admin:DirectoryName** The path to the admin panel. Defaults to "admin". Can only contain letters, numbers and dashes.

## AutoUpdateSettings

When running the auto-update process Cofoundry will automatically lock the update process to prevent it from running concurrently across multiple application instances, however you may wish to disable the auto-update process for a specific environment via config, which you can do using this setting:

- **Cofoundry:AutoUpdate:Disabled** Disables the auto-update process entirely.

## Plugins

For plugins you should check the documentation in the plugin repository. Typically they will follow the convention of *Cofoundry:Plugins:PluginName:Disabled* 

## Other Features

Check out the [Common Config Settings](/references/common-config-settings) documentation for an up-to-date list of all configuration settings. If you need a setting that isn't listed do let us know by submitting [an issue](https://github.com/cofoundry-cms/cofoundry/issues).

