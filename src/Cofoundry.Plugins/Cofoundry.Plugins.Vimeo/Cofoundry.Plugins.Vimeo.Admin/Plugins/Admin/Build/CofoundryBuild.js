const gulp = require('gulp');
const sass = require('gulp-sass');
const postcss = require('gulp-postcss');
const cssnano = require('cssnano');
const sassGlob  = require('gulp-sass-glob');
const concat = require('gulp-concat-util');
const uglify = require('gulp-uglify');
const path = require('path');
const gulpif = require('gulp-if');
const branch = require('branch-pipe')

exports.CofoundryBuild = class  {

    /**
     * Attaches several gulp tasks to an object, which includes a 'build' and 
     * 'watch' task, as well as separate 'buildCss' and 'buildJs' tasks.
     * 
     * @param {string} filePath - The base path to your admin modules, relative to the gulp script.
     * @param {object} config - A configuration object containing properties for the js and sass modules to build
     */
    constructor(filePath, config) {
        this.filePath = filePath;
		this.urlBase = config.urlBase;
        this.jsModules = mapJsModules(config.jsModules);
        this.sassModules = config.sassModules || [];
    }

    /**
     * Attaches several gulp tasks to an object, which includes a 'build' and 
     * 'watch' task, as well as separate 'buildCss' and 'buildJs' tasks.
     * 
     * @param {object} attachTo - An object to attach gulp tasks to
     */
    addGulpTasks = function(attachTo) {
        let filePath = this.filePath;
		let urlBase = this.urlBase;
		let cssTasks = createSassTasks(this.sassModules);
		let jsTasks = createJsTasks(this.jsModules);
		let htmlTasks = createHtmlTasks(this.jsModules);

        attachTo.buildCss = cssTasks ? cssTasks : noTasks;
		
		var jsBuildTasks = [jsTasks, htmlTasks].filter(t => t != null);
        attachTo.buildJs = jsTasks.length ? gulp.parallel(jsBuildTasks) : noTasks;
				
		var allBuildTasks = [cssTasks, jsTasks, htmlTasks].filter(t => t != null);
        attachTo.build = attachTo.default = allBuildTasks.length ? gulp.parallel(...allBuildTasks) : noTasks;
        
        attachTo.watch = watch.bind(this);

        /* PRIVATE */
		
		function noTasks() {
			console.log('No modules found to build.');
		}
        
        function createJsTasks(jsModules) {
			
			if (!jsModules || !jsModules.length) return null;
			
            var tasks = jsModules.map(module => {
                return jsModuleTask.bind(null, module);
            });

            return gulp.parallel(...tasks);
        }
        
        function createHtmlTasks(jsModules) {
			
			if (!jsModules || !jsModules.length) return null;
			
            var tasks = jsModules.filter(module => {
                // if we're ignoring defaults, assume there's no html templates
                return !module.ignoreDefaultSources;
            }).map(module => {
                return htmlModuleTask.bind(null, module);
            });

            return gulp.parallel(...tasks);
        }

        function createSassTasks(sassModules) {
            
			if (!sassModules || !sassModules.length) return null;
			
            var tasks = sassModules.map(module => {
                return sassModuleTask.bind(null, module.moduleName);
            });

            return gulp.parallel(...tasks);
        }

        function sassModuleTask(moduleName) {
            // moduleName is the directory e.g. "CustomEntities", which is lowercased 
            // to make the output file name e.g. "customentities.css"
            let outputFileName = moduleName.toLowerCase();

            return gulp.src(sourcePath(moduleName + '/Sass/' + outputFileName + '.scss'))
                .pipe(sassGlob())
                .pipe(sass().on('error', sass.logError))
                .pipe(concat(outputFileName + '.css'))
                .pipe(gulp.dest(sourcePath(moduleName + '/Content/css')))
                .pipe(postcss([cssnano()]))
                .pipe(concat(outputFileName + '_min.css'))
                .pipe(gulp.dest(sourcePath(moduleName + '/Content/css')))
                ;
        }

        function jsModuleTask(module) {

            return gulp
                .src([
                    ...module.additionalSources.map(sourcePath),
                    ...module.defaultSources.map(sourcePath)
                ])
                .pipe(branch.obj(src => [
                    src.pipe(concat(module.outputFileName + '.js'))
                       .pipe(gulp.dest(sourcePath(module.moduleName + '/Content/js'))),
                    src.pipe(gulpif('!**/*.min.js', uglify()))
                       .pipe(concat(module.outputFileName + '_min.js'))
                       .pipe(gulp.dest(sourcePath(module.moduleName + '/Content/js')))
                  ]));
        }

        /**
         * Angular html templates are bundled into a single file per module
         * and loaded into the app ahead of time to speed up loading.
         * 
         * @param {string} moduleName The name of the module (container directory) e.g. "CustomEntities"
         */
        function htmlModuleTask(module) {
            let jsModuleName = camelize(module.moduleName);

            return gulp.src(sourcePath(module.moduleName + '/Js/**/*.html'))
                .pipe(concat(module.outputFileName + '_templates.js', {
                    process: prepareTemplate
                }))
                .pipe(concat.header("angular.module('cms." + jsModuleName + "').run(['$templateCache',function(t){"))
                .pipe(concat.footer('}]);'))
                .pipe(gulp.dest(sourcePath(module.moduleName + '/Content/js')))
                ;
                        
            function prepareTemplate(src, filePath) {
                
                var removeSpaces = src.replace(/[\t\n\r]/gm, "");
                var escapeQuotes = removeSpaces.replace(/'/g, "\\'");		
                var releativePath = path.relative(__dirname, filePath).replace(/\\/g, '/');
                var splitPath = releativePath.split('..');
                var formattedSrc = "t.put('" + (urlBase || '') + splitPath[1] + "','" + escapeQuotes + "');";
                
                return formattedSrc;
            }
        }

        function watch() {

            this.sassModules.forEach(module => {
                watchSassModule(module.moduleName, module.directory);
            });
            
            this.jsModules.forEach(module => {
                watchJsModule(module);
                watchHtmlModule(module);
            });
        }

        function watchSassModule(moduleName, moduleDirectory) {
            // Currently we watch for sass and css files, but we
            // might also need to add images in here if they form
            // part of the build
            const EXTENSIONS = ['.css'];
            moduleDirectory = moduleDirectory || moduleName;

            gulp.watch(
                [
                    ...globExtensions(moduleDirectory + '/Sass/**/*', EXTENSIONS), 
                    sourcePath(moduleName + '/**/*.scss')
                ], 
                sassModuleTask.bind(null, moduleName)
                );
        }

        function watchJsModule(module) {
            gulp.watch(
                sourcePath(module.moduleName + '/Js/**/*.js'), 
                jsModuleTask.bind(null, module)
                );
        }

        function watchHtmlModule(module) {
            gulp.watch(
                sourcePath(module.moduleName + '/Js/**/*.html'), 
                htmlModuleTask.bind(null, module)
                );
        }

        function globExtensions(path, extensions) {
            var paths = [];
            extensions.forEach(e => {
                paths.push(sourcePath(path + e));
            });

            return paths;
        }

        function sourcePath(path) {
            return filePath + path;
        }
    }
}

function mapJsModules(jsModules) {
            
    return jsModules.map(mapJsModule || []);
    
    function mapJsModule(module) {
        // If passing a string, it will be the module name (container
        // directory) e.g. "CustomEntities"
        if (typeof(module) === 'string') {
            module = {
                moduleName: module
            };
        }

        // The standard module sources to scan for, which can be 
        // ignored with the ignoreDefaultSources flag if you want to load
        // custom scripts
        module.defaultSources = module.ignoreDefaultSources ? [] : [
            module.moduleName + '/Js/Bootstrap/**/*.js',
            module.moduleName + '/Js/DataServices/**/*.js',
            module.moduleName + '/Js/Utilities/**/*.js',
            module.moduleName + '/Js/Filters/**/*.js',
            module.moduleName + '/Js/Framework/**/*.js',
            module.moduleName + '/Js/UIComponents/**/*.js',
            module.moduleName + '/Js/UIComponents/**/*.js',
            module.moduleName + '/Js/Routes/**/*.js',
        ];

        // Additonal sources allows you to load any extra libs or custom files 
        // ahead of the standard modules.
        module.additionalSources = module.additionalSources || [];

        // By default the module name is lowercased to make the output file 
        // name e.g. "customentities.css", but you can provide a custom one
        module.outputFileName = module.outputFileName || module.moduleName.toLowerCase();

        return module;
    }
}

function camelize(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}
