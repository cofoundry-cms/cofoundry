module.exports = function(grunt) {

    "use strict";
    
	var standardModules = [
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
		'Users'
		];
	
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        watch: getWatchConfig(),

        compass: {
            options: {
                sassDir: 'Sass',
                cssDir: 'Content/css',
                outputStyle: 'expanded',
                sourcemap : true,
            },
            shared : {
                config : '../Admin/Modules/Shared/config.rb',
                options: {
                    basePath : '../Admin/Modules/Shared',
                },
            },
            visualEditor: {
                config: '../Admin/Modules/VisualEditor/config.rb',
                options: {
                    basePath: '../Admin/Modules/VisualEditor',
                },
            }
        },
        
        // See https://github.com/gruntjs/grunt-contrib-imagemin
        imagemin: {
            static: {
                options: {
                    optimizationLevel: 3
                },
                files: {
                    '../Admin/Modules/Shared/Content/Css/img/icons/normal.png': '../Admin/Modules/Shared/Content/Css/img/icons/normal.png',
                    '../Admin/Modules/Shared/Content/Css/img/icons/retina.png': '../Admin/Modules/Shared/Content/Css/img/icons/retina.png'
                }
            }
        },

        cssUrlRewrite: {
            fontAwesome: {
                src: '../Admin/Modules/Shared/Content/css/third-party/font-awesome/font-awesome.css',
                dest: '../Admin/Modules/Shared/Content/css/font-awesome.css',
                options: {
                    skipExternal: true,
                    rewriteUrl: function(url, options, dataURI) {
                        var splitUrl = url.split('..');
                        return splitUrl[1].replace('/Admin/Modules/Shared/Content/css/', '');
                    }
                }
            },
            shared: {
                src: '../Admin/Modules/Shared/Content/css/modules.css',
                dest: '../Admin/Modules/Shared/Content/css/modules.css',
                options: {
                    skipExternal: true,
					baseDir: '..',
                    rewriteUrl: function(url, options, dataURI) {
                        var splitUrl = url.split('..');
						grunt.log.writeln(url);
						grunt.log.writeln(splitUrl[1].replace('/Admin/Modules/Shared/Content/css/', ''));
                        return splitUrl[1].replace('/Admin/Modules/Shared/Content/css/', '');
                    }
                }
            }
        },

        cssmin: {
            target: {
                src: '../Admin/Modules/Shared/Content/css/shared.css',
                dest: '../Admin/Modules/Shared/Content/css/shared.min.css'
            }
        },

        concat: getConcactConfig(),

        uglify: getUglifyConfig(),
    });
    
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-compass');
    grunt.loadNpmTasks('grunt-contrib-imagemin');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks("grunt-css-url-rewrite");
    grunt.loadNpmTasks('grunt-contrib-cssmin');

    grunt.registerTask('default', ['compass:shared', 'cssUrlRewrite', 'bundleJS', 'cssmin']);
    grunt.registerTask('buildCSS', ['compass:shared', 'cssUrlRewrite', 'concat:css', 'cssmin']);
    grunt.registerTask('bundleJS', getBundleJSTaskConfig());
	
	// Helpers
	
	function getWatchConfig() {
		var config = {
            shared : {
                files: [
                    '../Admin/Modules/Shared/Sass/**/*.scss', 
                    '../Admin/Modules/Shared/Js/UIComponents/**/*.scss',
                ],
                tasks: ['compass:shared', 'cssUrlRewrite', 'concat:css', 'cssmin']
            },
            siteViewer : {
                files: [
                    '../Admin/Modules/VisualEditor/Sass/**/*.scss', 
                    '../Admin/Modules/VisualEditor/Js/UIComponents/**/*.scss',
                ],
                tasks: ['compass:visualEditor']
            },
            auth : {
                files: [
                    '../Admin/Modules/Auth/MVC/Views/Shared.js'
                ],
                tasks: ['concat:auth', 'uglify:auth']
            },
            sharedJS : {
                files: [
                    '../Admin/Modules/Shared/Js/**/*.js'
                ],
                tasks: ['concat:shared', 'uglify:shared', 'concat:sharedAll', 'concat:sharedAllMin']
            },
            sharedHTML : {
                files: [
                    '../Admin/Modules/Shared/Js/**/*.html'
                ],
                tasks: ['concat:sharedHTML']
            },
            visualEditor : {
                files: [
                    '../Admin/Modules/VisualEditor/Js/**/*.js'
                ],
                tasks: ['concat:visualEditor', 'concat:contentPage', 'uglify:visualEditor', 'uglify:contentPage']
            },
            visualEditorHTML : {
                files: [
                    '../Admin/Modules/VisualEditor/Js/**/*.html'
                ],
                tasks: ['concat:visualEditorHTML']
            },
            options: {
                spawn: false
            }
        }
		
		standardModules.forEach(function(moduleName) {
			var camelCaseName = camelize(moduleName);
			var htmlTaskName = camelCaseName + 'HTML';
			
			config[camelCaseName] = {
                files: [
                    '../Admin/Modules/' + moduleName + '/Js/**/*.js'
                ],
                tasks: ['concat:' + camelCaseName, 'uglify:' + camelCaseName]
			};
			
			config[htmlTaskName] = {
                files: [
                    '../Admin/Modules/' + moduleName + '/Js/**/*.html'
                ],
                tasks: ['concat:' + htmlTaskName ]
            };
		});
		
		return config;
	}
	
	function getConcactConfig() {
		var config = {
            css: {
                src: [
                    '../Admin/Modules/Shared/Content/css/font-awesome.css',
                    '../Admin/Modules/Shared/Content/css/third-party/tinymce/skin.min.css',
                    '../Admin/Modules/Shared/Content/css/third-party/ui-select.css',
                    '../Admin/Modules/Shared/Content/css/third-party/selectize.default.css',
                    '../Admin/Modules/Shared/Content/css/modules.css',
                ],
                dest: '../Admin/Modules/Shared/Content/css/shared.css'
            },
            auth: {
                src: ['../Admin/Modules/Auth/MVC/Views/Shared.js'],
                dest: '../Admin/Modules/Auth/Content/js/Shared.js'
            },
            shared: {
                src: [
                    '../Admin/Modules/Shared/Js/Bootstrap/*.js',
                    '../Admin/Modules/Shared/Js/DataServices/*.js',
                    '../Admin/Modules/Shared/Js/Utilities/*.js',
                    '../Admin/Modules/Shared/Js/Filters/*.js',
                    '../Admin/Modules/Shared/Js/Framework/*.js',
                    '../Admin/Modules/Shared/Js/UIComponents/*.js',
                    '../Admin/Modules/Shared/Js/UIComponents/**/*.js',
                ],
                dest: '../Admin/Modules/Shared/Content/js/shared.js'
            },
            sharedHTML: getHtmlTemplateConfig('Shared'),
            visualEditor: {
                src: [
                    '../Admin/Modules/VisualEditor/Js/Bootstrap/*.js',
                    '../Admin/Modules/VisualEditor/Js/DataServices/*.js',
                    '../Admin/Modules/VisualEditor/Js/UIComponents/*.js',
                    '../Admin/Modules/VisualEditor/Js/Routes/*.js',
                    '../Admin/Modules/VisualEditor/Js/Routes/Modals/*.js',
                ],
                dest: '../Admin/Modules/VisualEditor/Content/js/visualeditor.js'
            },
            visualEditorHTML: getHtmlTemplateConfig('VisualEditor'),
            contentPage: {
                src: [
                    '../Admin/Modules/VisualEditor/Js/ContentPage/Index.js',
                    '../Admin/Modules/VisualEditor/Js/ContentPage/EventAggregator.js',
                    '../Admin/Modules/VisualEditor/Js/ContentPage/GuiController.js',
                ],
                dest: '../Admin/Modules/VisualEditor/Content/js/contentpage.js'
            },
            sharedAll: {
                src: [
                    '../Admin/Modules/Shared/Js/lib/underscore.min.js',
                    '../Admin/Modules/Shared/Js/lib/angular.min.js',
                    '../Admin/Modules/Shared/Js/lib/angular-sanitize.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/TinyMce/tinymce.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/TinyMce/ui-tinymce.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/*.js',
                    '../Admin/Modules/Shared/Content/js/shared.js',
                ],
                dest: '../Admin/Modules/Shared/Content/js/shared.js'
            },
            sharedAllMin: {
                src: [
                    '../Admin/Modules/Shared/Js/lib/underscore.min.js',
                    '../Admin/Modules/Shared/Js/lib/angular.min.js',
                    '../Admin/Modules/Shared/Js/lib/angular-sanitize.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/TinyMce/tinymce.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/TinyMce/ui-tinymce.min.js',
                    '../Admin/Modules/Shared/Js/lib/AngularModules/*.js',
                    '../Admin/Modules/Shared/Content/js/shared_min.js',
                ],
                dest: '../Admin/Modules/Shared/Content/js/shared_min.js'
            }
            
        };
				
		standardModules.forEach(function(moduleName) {
			var camelCaseName = camelize(moduleName);
			var htmlTaskName = camelCaseName + 'HTML';
			
			config[camelCaseName] = {
                src: [
                    '../Admin/Modules/' + moduleName + '/Js/Bootstrap/*.js',
                    '../Admin/Modules/' + moduleName + '/Js/DataServices/*.js',
                    '../Admin/Modules/' + moduleName + '/Js/UIComponents/*.js',
                    '../Admin/Modules/' + moduleName + '/Js/Routes/*.js',
                    '../Admin/Modules/' + moduleName + '/Js/Routes/Modals/*.js',
                ],
                dest: '../Admin/Modules/' + moduleName + '/Content/js/' + moduleName.toLowerCase() + '.js'
            };
			
			config[htmlTaskName] = getHtmlTemplateConfig(moduleName);
		});
		
		return config;
	}
	
	function getUglifyConfig() {
		var config = {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
            },
            auth: {
                src: '../Admin/Modules/Auth/Content/js/Shared.js',
                dest: '../Admin/Modules/Auth/Content/js/Shared.js'
            },
            contentPage: {
                src: '../Admin/Modules/VisualEditor/Content/js/contentpage.js',
                dest: '../Admin/Modules/VisualEditor/Content/js/contentpage_min.js'
            }
        };
		
		['Shared', 'VisualEditor'].forEach(addConfig);
		standardModules.forEach(addConfig);
		
		function addConfig(moduleName) {
			var camelCaseName = camelize(moduleName);
			
			config[camelCaseName] = {
                src: '../Admin/Modules/' + moduleName + '/Content/js/' + moduleName.toLowerCase() + '.js',
                dest: '../Admin/Modules/' + moduleName + '/Content/js/' + moduleName.toLowerCase() + '_min.js'
            };
		}
		
		return config;
	
	}
	
	function getBundleJSTaskConfig() {
		var config = [
			'concat:css',
			'concat:auth',
			'concat:shared',
			'concat:sharedHTML',
			'concat:visualEditor',
			'concat:visualEditorHTML',
			'concat:contentPage'
		];
		
		standardModules.forEach(addConfig);
		
		function addConfig(moduleName) {
			var camelCaseName = camelize(moduleName);
			
			config.push('concat:' + camelCaseName);
			config.push('concat:' + camelCaseName + 'HTML');
		}
		
		// Run these tasks last
		config.push('uglify', 'concat:sharedAll', 'concat:sharedAllMin');
		
		return config;
	}
	
	function getHtmlTemplateConfig(moduleName) {
		return {
			options: {
				banner: "angular.module('cms." + camelize(moduleName) + "').run(['$templateCache',function(t){",
				footer: "}]);",
				process: function(src, filepath) {
					var removeSpaces = src.replace(/[\t\n\r]/gm, "");
					var escapeQuotes = removeSpaces.replace(/'/g, "\\'");
					var splitPath = filepath.split('..');
					var formattedSrc = "t.put('" + splitPath[1] + "','" + escapeQuotes + "');";
					
					return formattedSrc;
				}
			},
			src: [
				'../Admin/Modules/' + moduleName + '/Js/Routes/*.html',
				'../Admin/Modules/' + moduleName + '/Js/Routes/**/*.html',
				'../Admin/Modules/' + moduleName + '/Js/UIComponents/*.html',
				'../Admin/Modules/' + moduleName + '/Js/UIComponents/**/*.html'
			],
			dest: '../Admin/Modules/' + moduleName + '/Content/js/' + moduleName.toLowerCase() + '_templates.js'
		}
	}
	function camelize(string) {
		return string.charAt(0).toLowerCase() + string.slice(1);
	}
};