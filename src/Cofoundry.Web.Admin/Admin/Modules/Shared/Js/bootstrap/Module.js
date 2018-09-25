angular
    .module('cms.shared', [
        'ngRoute',
        'ngSanitize',
        'angularModalService',
        'ngFileUpload',
        'ui.tinymce',
        'ang-drag-drop',
        'ui.select'
    ])
    .constant('shared.internalModulePath', '/Admin/Modules/Shared/Js/')
    .constant('shared.pluginModulePath', '/Plugins/Admin/Modules/Shared/Js/')
    .constant('shared.modulePath', '/Cofoundry/Admin/Modules/Shared/Js/')
    ;
