angular
    .module('cms.shared', [
        'ngRoute',
        'ngSanitize',
        'angularModalService',
        'angularFileUpload',
        'ui.tinymce',
        'ang-drag-drop',
        'ui.select'
    ])
    .constant('shared.internalModulePath', '/Admin/Modules/Shared/Js/')
    .constant('shared.pluginModulePath', '/Plugins/Admin/Modules/Shared/Js/')
    .constant('shared.modulePath', '/Cofoundry/Admin/Modules/Shared/Js/')
    .constant('shared.serviceBase', '/Admin/Api/')
    .constant('shared.pluginServiceBase', '/Admin/Api/Plugins/')
    .constant('shared.urlBaseBase', '/Admin/')
    .constant('shared.internalContentPath', '/Admin/Modules/Shared/Content/')
    .constant('shared.contentPath', '/Cofoundry/Admin/Modules/Shared/Content/')
    .constant('shared.pluginContentPath', '/Plugins/Admin/Modules/Shared/Content/')
    ;
