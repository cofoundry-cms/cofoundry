const { CofoundryBuild } = require('./CofoundryBuild');

// The JavaScript modules to include in the build. 
let jsModules = [

    // Standard modules just need to provide the module name (container directory)
    'Account', 
    'CustomEntities', 
    'Dashboard', 
    'Directories', 
    'Documents', 
    'Images', 
    'Pages', 
    'PageTemplates', 
    'Roles',
    'Settings',
    'Setup',
    'Users', 
    'VisualEditor', 
    // More complex modules can use a config object
    {
        // Shared loads in the core framework libs ahead of the standard files
        moduleName:'Shared',
        additionalSources: [
            'Shared/Js/lib/underscore.min.js',
            'Shared/Js/lib/angular.min.js',
            'Shared/Js/lib/angular-sanitize.min.js',
            'Shared/Js/lib/AngularModules/TinyMce/tinymce.min.js',
            'Shared/Js/lib/AngularModules/TinyMce/ui-tinymce.min.js',
            'Shared/Js/lib/AngularModules/*.js',
        ]
    },{
        // The visual editor module has a bespoke vanilla js file for the host "content page"
        // This ignores most of the standard config and bundles a custom set of files 
        // into contentpage.js
        moduleName:'VisualEditor',
        outputFileName:'contentpage',
        ignoreDefaultSources: true,
        additionalSources: [
            'VisualEditor/Js/ContentPage/Index.js',
            'VisualEditor/Js/ContentPage/EventAggregator.js',
            'VisualEditor/Js/ContentPage/GuiController.js',
        ]
    },{
        // The auth module isn't angular and instead has a single file that needs bundling
        moduleName:'Auth',
        outputFileName:'Shared',
        ignoreDefaultSources: true,
        additionalSources: [
            'Auth/MVC/Views/Shared.js',
        ]
    }];

// The Sass modules to include in the build. 
let sassModules = [{
    // Shared module contains css for components in all the modules, so the directory needs globbing
    moduleName: 'Shared',
    directory: '*'
},{
    // Visual editor has a separate css file as it is loaded independently
    moduleName: 'VisualEditor',
}];

var cofoundryBuild = new CofoundryBuild('../Admin/Modules/', {
    jsModules: jsModules,
    sassModules: sassModules
})

// Adds tasks: buildCss, buildJs, build, watch
cofoundryBuild.addGulpTasks(exports);