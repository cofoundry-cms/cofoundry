const { CofoundryBuild } = require('./CofoundryBuild');

// The JavaScript modules to include in the build. 
// Standard modules just need to provide the module name (container directory)
let jsModules = [{ 
	moduleName: 'Shared',
	outputFileName: 'youtube'
}];

// The Sass modules to include in the build. 
let sassModules = [];

var cofoundryBuild = new CofoundryBuild('../Modules/', {
	urlBase: '/Plugins/Admin',
    jsModules: jsModules,
    sassModules: sassModules
})

// Adds tasks: buildCss, buildJs, build, watch
cofoundryBuild.addGulpTasks(exports);