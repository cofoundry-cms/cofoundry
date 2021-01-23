const gulp = require('gulp');
const sass = require('gulp-sass');
const postcss = require('gulp-postcss');
const cssnano = require('cssnano');
const sassGlob  = require('gulp-sass-glob');
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');

exports.CofoundryBuild = class  {

    /**
     * Attaches several gulp tasks to an object, which includes a 'build' and 
     * 'watch' task, as well as separate 'buildCss' and 'buildJs' tasks.
     * 
     * @param {string} basePath - The base path to your admin modules, relative to the gulp script.
     * @param {object} config - A configuration object containing properties for the js and sass modules to build
     */
    constructor(basePath, config) {
        this.basePath = basePath;
        this.jsModules = config.jsModules;
        this.sassModules = config.sassModules;
    }

    /**
     * Attaches several gulp tasks to an object, which includes a 'build' and 
     * 'watch' task, as well as separate 'buildCss' and 'buildJs' tasks.
     * 
     * @param {object} attachTo - An object to attach gulp tasks to
     */
    addGulpTasks = function(attachTo) {
        let basePath = this.basePath;
        attachTo.buildCss = gulp.parallel(createSassTasks(this.sassModules));
        attachTo.buildJs = gulp.parallel(createJsTasks(this.jsModules));
        attachTo.build = attachTo.default = gulp.parallel(
            createSassTasks(this.sassModules), 
            createJsTasks(this.jsModules)
            );
        
        attachTo.watch = watch.bind(this);

        /* PRIVATE */
        
        function createJsTasks(jsModules) {
            var tasks = jsModules.map(module => {
                return jsModuleTask.bind(null, module);
            });

            return gulp.parallel(...tasks);
        }

        function createSassTasks(sassTasks) {
            
            var tasks = sassTasks.map(module => {
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

        function jsModuleTask(config) {
            
            // If passing a string, it will be the module name (container
            // directory) e.g. "CustomEntities"
            if (typeof(config) === 'string') {
                config = {
                    moduleName: config
                };
            }

            // The standard module sources to scan for, which can be 
            // ignored with the ignoreDefaultSources flag if you want to load
            // custom scripts
            config.defaultSources = config.ignoreDefaultSources ? [] : [
                config.moduleName + '/Js/Bootstrap/**/*.js',
                config.moduleName + '/Js/DataServices/**/*.js',
                config.moduleName + '/Js/Utilities/**/*.js',
                config.moduleName + '/Js/Filters/**/*.js',
                config.moduleName + '/Js/Framework/**/*.js',
                config.moduleName + '/Js/UIComponents/**/*.js',
                config.moduleName + '/Js/UIComponents/**/*.js',
                config.moduleName + '/Js/Routes/**/*.js',
            ];

            // Additonal sources allows you to load any extra libs or custom files 
            // ahead of the standard modules.
            config.additionalSources = config.additionalSources || [];

            // By default the module name is lowercased to make the output file 
            // name e.g. "customentities.css", but you can provide a custom one
            config.outputFileName = config.outputFileName || config.moduleName.toLowerCase();
            
            return gulp
                .src([
                    ...config.additionalSources.map(sourcePath),
                    ...config.defaultSources.map(sourcePath)
                ])
                .pipe(concat(config.outputFileName + '.js'))
                .pipe(gulp.dest(sourcePath(config.moduleName + '/Content/js')))
                .pipe(uglify())
                .pipe(concat(config.outputFileName + '_min.js'))
                .pipe(gulp.dest(sourcePath(config.moduleName + '/Content/js')))
                ;
        }

        /**
         * Angular html templates are bundled into a single file per module
         * and loaded into the app ahead of time to speed up loading.
         * 
         * @param {string} moduleName The name of the module (container directory) e.g. "CustomEntities"
         */
        function htmlModuleTask(moduleName) {
            let outputFileName = moduleName.toLowerCase();
            let jsModuleName = camelize(moduleName);

            return gulp.src(sourcePath(moduleName + '/Js/**/*.html'))
                .pipe(concat(outputFileName + '_templates.js', {
                    process: prepareTemplate
                }))
                .pipe(concat.header("angular.module('cms." + jsModuleName + "').run(['$templateCache',function(t){"))
                .pipe(concat.footer('}]);'))
                .pipe(gulp.dest(sourcePath(moduleName + '/Content/js')))
                ;
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
            const EXTENSIONS = ['.scss', '.css'];
            moduleDirectory = moduleDirectory || moduleName;

            gulp.watch(
                globExtensions(moduleDirectory + '/Sass/**/*', EXTENSIONS), 
                sassModuleTask.bind(null, moduleName)
                );
        }

        function watchJsModule(module) {
            gulp.watch(
                sourcePath(module + '/Js/**/*.js'), 
                jsModuleTask.bind(null, module)
                );
        }

        function watchHtmlModule(module) {
            gulp.watch(
                sourcePath(module + '/Js/**/*.html'), 
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
            return basePath + path;
        }
    }


}


function camelize(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}
