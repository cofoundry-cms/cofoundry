module.exports = function(grunt) {

    "use strict";
    
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        watch: {
            shared : {
                files: [
                    '../Admin/Modules/Shared/Sass/**/*.scss', 
                    '../Admin/Modules/Shared/Js/UIComponents/**/*.scss',
                ],
                tasks: ['compass:shared']
            },
            siteViewer : {
                files: [
                    '../Admin/Modules/VisualEditor/Sass/**/*.scss', 
                    '../Admin/Modules/VisualEditor/Js/UIComponents/**/*.scss',
                ],
                tasks: ['compass:visualEditor']
            },
            options: {
                spawn: false
            }
        },

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

        svgmin: {
             prod: {
                 files: [{
                     expand: true,
                     cwd: '../Admin/Modules/Shared/Content/Css/svg',
                     src: ['*.svg'],
                     dest: '../Admin/Modules/Shared/Content/Css/svg-dist'
                 }]
             }
        },

        svgcombine: {
            all: {
                files: {
                    '../Admin/Modules/VisualEditor/Content/svg-cache.html': ['../Admin/Modules/VisualEditor/Content/svg/*.svg'],
                }
            }
        },

         //grunticon: {
         //    prod: {
         //        files: [{
         //            expand: true,
         //            cwd: '../Content/images/grunticon-source',
         //            src: ['*.svg', '*.png'],
         //            dest: '../Content/images/grunticon-output'
         //        }],
         //        options: {
         //        }
         //    }
         //}
    });
    
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-compass');
    grunt.loadNpmTasks('grunt-contrib-imagemin');
    grunt.loadNpmTasks('grunt-svg-combine');

    grunt.registerTask('svgcombine', ['svgcombine:all']);
    grunt.registerTask('default', ['compass:shared', 'svgcombine']);
};