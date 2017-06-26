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
            account : {
                files: [
                    '../Admin/Modules/Account/Js/**/*.js'
                ],
                tasks: ['concat:account', 'uglify:account']
            },
            accountHTML : {
                files: [
                    '../Admin/Modules/Account/Js/**/*.html'
                ],
                tasks: ['concat:accountHTML']
            },
            authChangePassword : {
                files: [
                    '../Admin/Modules/Auth/MVC/Views/ChangePassword.js'
                ],
                tasks: ['concat:authChangePassword', 'uglify:authChangePassword']
            },
            authForgotPassword : {
                files: [
                    '../Admin/Modules/Auth/MVC/Views/ForgotPassword.js'
                ],
                tasks: ['concat:authForgotPassword', 'uglify:authForgotPassword']
            },
            authLogin : {
                files: [
                    '../Admin/Modules/Auth/MVC/Views/Login.js'
                ],
                tasks: ['concat:authLogin', 'uglify:authLogin']
            },
            customEntities : {
                files: [
                    '../Admin/Modules/CustomEntities/Js/**/*.js'
                ],
                tasks: ['concat:customEntities', 'uglify:customEntities']
            },
            customEntitiesHTML : {
                files: [
                    '../Admin/Modules/CustomEntities/Js/**/*.html'
                ],
                tasks: ['concat:customEntitiesHTML']
            },
            dashboard : {
                files: [
                    '../Admin/Modules/Dashboard/Js/**/*.js'
                ],
                tasks: ['concat:dashboard', 'uglify:dashboard']
            },
            dashboardHTML : {
                files: [
                    '../Admin/Modules/Dashboard/Js/**/*.html'
                ],
                tasks: ['concat:dashboardHTML']
            },
            directories : {
                files: [
                    '../Admin/Modules/Directories/Js/**/*.js'
                ],
                tasks: ['concat:directories', 'uglify:directories']
            },
            directoriesHTML : {
                files: [
                    '../Admin/Modules/Directories/Js/**/*.html'
                ],
                tasks: ['concat:directoriesHTML']
            },
            documents : {
                files: [
                    '../Admin/Modules/Documents/Js/**/*.js'
                ],
                tasks: ['concat:documents', 'uglify:documents']
            },
            documentsHTML : {
                files: [
                    '../Admin/Modules/Documents/Js/**/*.html'
                ],
                tasks: ['concat:documents']
            },
            images : {
                files: [
                    '../Admin/Modules/Images/Js/**/*.js'
                ],
                tasks: ['concat:images', 'uglify:images']
            },
            imagesHTML : {
                files: [
                    '../Admin/Modules/Images/Js/**/*.html'
                ],
                tasks: ['concat:imagesHTML']
            },
            pages : {
                files: [
                    '../Admin/Modules/Pages/Js/**/*.js'
                ],
                tasks: ['concat:pages', 'uglify:pages']
            },
            pagesHTML : {
                files: [
                    '../Admin/Modules/Pages/Js/**/*.html'
                ],
                tasks: ['concat:pagesHTML']
            },
            pageTemplates : {
                files: [
                    '../Admin/Modules/PageTemplates/Js/**/*.js'
                ],
                tasks: ['concat:pageTemplates', 'uglify:pageTemplates']
            },
            pageTemplatesHTML : {
                files: [
                    '../Admin/Modules/PageTemplates/Js/**/*.html'
                ],
                tasks: ['concat:pageTemplatesHTML']
            },
            roles : {
                files: [
                    '../Admin/Modules/Roles/Js/**/*.js'
                ],
                tasks: ['concat:roles', 'uglify:roles']
            },
            rolesHTML : {
                files: [
                    '../Admin/Modules/Roles/Js/**/*.html'
                ],
                tasks: ['concat:rolesHTML']
            },
            settings : {
                files: [
                    '../Admin/Modules/Settings/Js/**/*.js'
                ],
                tasks: ['concat:settings', 'uglify:settings']
            },
            settingsHTML : {
                files: [
                    '../Admin/Modules/Settings/Js/**/*.html'
                ],
                tasks: ['concat:settingsHTML']
            },
            setup : {
                files: [
                    '../Admin/Modules/Setup/Js/**/*.js'
                ],
                tasks: ['concat:setup', 'uglify:setup']
            },
            setupHTML : {
                files: [
                    '../Admin/Modules/Setup/Js/**/*.html'
                ],
                tasks: ['concat:setupHTML']
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
            users : {
                files: [
                    '../Admin/Modules/Users/Js/**/*.js'
                ],
                tasks: ['concat:users', 'uglify:users']
            },
            usersHTML : {
                files: [
                    '../Admin/Modules/Users/Js/**/*.html'
                ],
                tasks: ['concat:usersHTML']
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

        cssUrlRewrite: {
            fontAwesome: {
                src: '../Admin/Modules/Shared/Content/css/third-party/font-awesome/font-awesome.css',
                dest: '../Admin/Modules/Shared/Content/css/font-awesome.css',
                options: {
                    skipExternal: true,
                    rewriteUrl: function(url, options, dataURI) {
                        var splitUrl = url.split('..');
                        return splitUrl[1];
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

        concat: {
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
            account: {
                src: [
                    '../Admin/Modules/Account/Js/Bootstrap/*.js',
                    '../Admin/Modules/Account/Js/DataServices/*.js',
                    '../Admin/Modules/Account/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Account/Content/js/account.js'
            },
            accountHTML: {
                options: {
                    banner: "angular.module('cms.account').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Account/Js/Routes/*.html',
                ],
                dest: '../Admin/Modules/Account/Content/js/account_templates.js'
            },
            authChangePassword: {
                src: ['../Admin/Modules/Auth/MVC/Views/ChangePassword.js'],
                dest: '../Admin/Modules/Auth/Content/js/ChangePassword.js'
            },
            authForgotPassword: {
                src: ['../Admin/Modules/Auth/MVC/Views/ForgotPassword.js'],
                dest: '../Admin/Modules/Auth/Content/js/ForgotPassword.js'
            },
            authLogin: {
                src: ['../Admin/Modules/Auth/MVC/Views/Login.js'],
                dest: '../Admin/Modules/Auth/Content/js/Login.js'
            },
            customEntities: {
                src: [
                    '../Admin/Modules/CustomEntities/Js/Bootstrap/*.js',
                    '../Admin/Modules/CustomEntities/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/CustomEntities/Content/js/customentities.js'
            },
            customEntitiesHTML: {
                options: {
                    banner: "angular.module('cms.customEntities').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/CustomEntities/Js/Routes/*.html',
                    '../Admin/Modules/CustomEntities/Js/Routes/Modals/*.html',
                ],
                dest: '../Admin/Modules/CustomEntities/Content/js/customentities_templates.js'
            },
            dashboard: {
                src: [
                    '../Admin/Modules/Dashboard/Js/Bootstrap/*.js',
                    '../Admin/Modules/Dashboard/Js/DataServices/*.js',
                    '../Admin/Modules/Dashboard/Js/UIComponents/*.js',
                    '../Admin/Modules/Dashboard/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Dashboard/Content/js/dashboard.js'
            },
            dashboardHTML: {
                options: {
                    banner: "angular.module('cms.dashboard').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Dashboard/Js/UIComponents/*.html',
                    '../Admin/Modules/Dashboard/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Dashboard/Content/js/dashboard_templates.js'
            },
            directories: {
                src: [
                    '../Admin/Modules/Directories/Js/Bootstrap/*.js',
                    '../Admin/Modules/Directories/Js/DataServices/*.js',
                    '../Admin/Modules/Directories/Js/UIComponents/*.js',
                    '../Admin/Modules/Directories/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Directories/Content/js/directories.js'
            },
            directoriesHTML: {
                options: {
                    banner: "angular.module('cms.directories').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Directories/Js/UIComponents/*.html',
                    '../Admin/Modules/Directories/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Directories/Content/js/directories_templates.js'
            },
            documents: {
                src: [
                    '../Admin/Modules/Documents/Js/Bootstrap/*.js',
                    '../Admin/Modules/Documents/Js/DataServices/*.js',
                    '../Admin/Modules/Documents/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Documents/Content/js/documents.js'
            },
            documentsHTML: {
                options: {
                    banner: "angular.module('cms.documents').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Documents/Js/Routes/*.html',
                ],
                dest: '../Admin/Modules/Documents/Content/js/documents_templates.js'
            },
            images: {
                src: [
                    '../Admin/Modules/Images/Js/Bootstrap/*.js',
                    '../Admin/Modules/Images/Js/DataServices/*.js',
                    '../Admin/Modules/Images/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Images/Content/js/images.js'
            },
            imagesHTML: {
                options: {
                    banner: "angular.module('cms.images').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Images/Js/Routes/*.html',
                ],
                dest: '../Admin/Modules/Images/Content/js/images_templates.js'
            },
            pages: {
                src: [
                    '../Admin/Modules/Pages/Js/Bootstrap/*.js',
                    '../Admin/Modules/Pages/Js/DataServices/*.js',
                    '../Admin/Modules/Pages/Js/Routes/*.js',
                    '../Admin/Modules/Pages/Js/Routes/Modals/*.js',
                ],
                dest: '../Admin/Modules/Pages/Content/js/pages.js'
            },
            pagesHTML: {
                options: {
                    banner: "angular.module('cms.pages').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Pages/Js/Routes/*.html',
                    '../Admin/Modules/Pages/Js/Routes/Modals/*.html',
                    '../Admin/Modules/Pages/Js/Routes/Partials/*.html'
                ],
                dest: '../Admin/Modules/Pages/Content/js/pages_templates.js'
            },
            pageTemplates: {
                src: [
                    '../Admin/Modules/PageTemplates/Js/Bootstrap/*.js',
                    '../Admin/Modules/PageTemplates/Js/DataServices/*.js',
                    '../Admin/Modules/PageTemplates/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/PageTemplates/Content/js/pagetemplates.js'
            },
            pageTemplatesHTML: {
                options: {
                    banner: "angular.module('cms.pageTemplates').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/PageTemplates/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/PageTemplates/Content/js/pagetemplates_templates.js'
            },
            roles: {
                src: [
                    '../Admin/Modules/Roles/Js/Bootstrap/*.js',
                    '../Admin/Modules/Roles/Js/DataServices/*.js',
                    '../Admin/Modules/Roles/Js/UIComponents/*.js',
                    '../Admin/Modules/Roles/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Roles/Content/js/roles.js'
            },
            rolesHTML: {
                options: {
                    banner: "angular.module('cms.roles').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Roles/Js/UIComponents/*.html',
                    '../Admin/Modules/Roles/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Roles/Content/js/roles_templates.js'
            },
            settings: {
                src: [
                    '../Admin/Modules/Settings/Js/Bootstrap/*.js',
                    '../Admin/Modules/Settings/Js/DataServices/*.js',
                    '../Admin/Modules/Settings/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Settings/Content/js/settings.js'
            },
            settingsHTML: {
                options: {
                    banner: "angular.module('cms.settings').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Settings/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Settings/Content/js/settings_templates.js'
            },
            setup: {
                src: [
                    '../Admin/Modules/Setup/Js/Bootstrap/*.js',
                    '../Admin/Modules/Setup/Js/DataServices/*.js',
                    '../Admin/Modules/Setup/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Setup/Content/js/setup.js'
            },
            setupHTML: {
                options: {
                    banner: "angular.module('cms.setup').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Setup/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Setup/Content/js/setup_templates.js'
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
            sharedHTML: {
                options: {
                    banner: "angular.module('cms.shared').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Shared/Js/UIComponents/**/*.html'
                ],
                dest: '../Admin/Modules/Shared/Content/js/shared_templates.js'
            },
            users: {
                src: [
                    '../Admin/Modules/Users/Js/Bootstrap/*.js',
                    '../Admin/Modules/Users/Js/DataServices/*.js',
                    '../Admin/Modules/Users/Js/UIComponents/*.js',
                    '../Admin/Modules/Users/Js/Routes/*.js',
                ],
                dest: '../Admin/Modules/Users/Content/js/users.js'
            },
            usersHTML: {
                options: {
                    banner: "angular.module('cms.users').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/Users/Js/Routes/*.html'
                ],
                dest: '../Admin/Modules/Users/Content/js/users_templates.js'
            },
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
            visualEditorHTML: {
                options: {
                    banner: "angular.module('cms.visualEditor').run(['$templateCache',function(t){",
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
                    '../Admin/Modules/VisualEditor/Js/UIComponents/*.html',
                    '../Admin/Modules/VisualEditor/Js/Routes/Modals/*.html'
                ],
                dest: '../Admin/Modules/VisualEditor/Content/js/visualeditor_templates.js'
            },
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
            
        },

        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
            },
            account: {
                src: '../Admin/Modules/Account/Content/js/account.js',
                dest: '../Admin/Modules/Account/Content/js/account_min.js'
            },
            authChangePassword: {
                src: '../Admin/Modules/Auth/Content/js/ChangePassword.js',
                dest: '../Admin/Modules/Auth/Content/js/ChangePassword_min.js'
            },
            authForgotPassword: {
                src: '../Admin/Modules/Auth/Content/js/ForgotPassword.js',
                dest: '../Admin/Modules/Auth/Content/js/ForgotPassword_min.js'
            },
            authLogin: {
                src: '../Admin/Modules/Auth/Content/js/Login.js',
                dest: '../Admin/Modules/Auth/Content/js/Login_min.js'
            },
            customEntities: {
                src: '../Admin/Modules/CustomEntities/Content/js/customentities.js',
                dest: '../Admin/Modules/CustomEntities/Content/js/customentities_min.js'
            },
            dashboard: {
                src: '../Admin/Modules/Dashboard/Content/js/dashboard.js',
                dest: '../Admin/Modules/Dashboard/Content/js/dashboard_min.js',
            },
            directories: {
                src: '../Admin/Modules/Directories/Content/js/directories.js',
                dest: '../Admin/Modules/Directories/Content/js/directories_min.js'
            },
            documents: {
                src: '../Admin/Modules/Documents/Content/js/documents.js',
                dest: '../Admin/Modules/Documents/Content/js/documents_min.js'
            },
            images: {
                src: '../Admin/Modules/Images/Content/js/images.js',
                dest: '../Admin/Modules/Images/Content/js/images_min.js'
            },
            pages: {
                src: '../Admin/Modules/Pages/Content/js/pages.js',
                dest: '../Admin/Modules/Pages/Content/js/pages_min.js'
            },
            pageTemplates: {
                src: '../Admin/Modules/PageTemplates/Content/js/pagetemplates.js',
                dest: '../Admin/Modules/PageTemplates/Content/js/pagetemplates_min.js'
            },
            roles: {
                src: '../Admin/Modules/Roles/Content/js/roles.js',
                dest: '../Admin/Modules/Roles/Content/js/roles_min.js'
            },
            settings: {
                src: '../Admin/Modules/Settings/Content/js/settings.js',
                dest: '../Admin/Modules/Settings/Content/js/settings_min.js'
            },
            setup: {
                src: '../Admin/Modules/Setup/Content/js/setup.js',
                dest: '../Admin/Modules/Setup/Content/js/setup_min.js'
            },
            shared: {
                src: '../Admin/Modules/Shared/Content/js/shared.js',
                dest: '../Admin/Modules/Shared/Content/js/shared_min.js'
            },
            users: {
                src: '../Admin/Modules/Users/Content/js/users.js',
                dest: '../Admin/Modules/Users/Content/js/users_min.js'
            },
            visualEditor: {
                src: '../Admin/Modules/VisualEditor/Content/js/visualeditor.js',
                dest: '../Admin/Modules/VisualEditor/Content/js/visualeditor_min.js'
            },
            contentPage: {
                src: '../Admin/Modules/VisualEditor/Content/js/contentpage.js',
                dest: '../Admin/Modules/VisualEditor/Content/js/contentpage_min.js'
            }
        }
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
    grunt.registerTask('bundleJS', [
        'concat:css',
        'concat:account',
        'concat:accountHTML',
        'concat:authChangePassword',
        'concat:authForgotPassword',
        'concat:authLogin',
        'concat:customEntities',
        'concat:customEntitiesHTML',
        'concat:dashboard',
        'concat:dashboardHTML',
        'concat:directories',
        'concat:directoriesHTML',
        'concat:documents',
        'concat:documentsHTML',
        'concat:images',
        'concat:imagesHTML',
        'concat:pages',
        'concat:pagesHTML',
        'concat:pageTemplates',
        'concat:pageTemplatesHTML',
        'concat:roles',
        'concat:rolesHTML',
        'concat:settings',
        'concat:settingsHTML',
        'concat:setup',
        'concat:setupHTML',
        'concat:shared',
        'concat:sharedHTML',
        'concat:users',
        'concat:usersHTML',
        'concat:visualEditor',
        'concat:visualEditorHTML',
        'concat:contentPage',
        'uglify',
        'concat:sharedAllMin'
    ]);
};