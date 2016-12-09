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
        }

        // svgmin: {
        //     prod: {
        //         files: [{
        //             expand: true,
        //             cwd: '../Content/images/svg',
        //             src: ['*.svg'],
        //             dest: '../Content/images/grunticon-source'
        //         }]
        //     }
        // },
        // grunticon: {
        //     prod: {
        //         files: [{
        //             expand: true,
        //             cwd: '../Content/images/grunticon-source',
        //             src: ['*.svg', '*.png'],
        //             dest: '../Content/images/grunticon-output'
        //         }],
        //         options: {
        //         }
        //     }
        // }
    });
    
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-compass');
    grunt.loadNpmTasks('grunt-contrib-imagemin');
    // grunt.loadNpmTasks('grunt-grunticon');
    // grunt.loadNpmTasks('grunt-svgmin');

    grunt.registerTask('all_sass', ['compass:shared', 'compass:pages']);

    grunt.registerTask('default', ['all_sass']);
    // grunt.registerTask("svg", ["svgmin:prod", "grunticon:prod"]);
    
};