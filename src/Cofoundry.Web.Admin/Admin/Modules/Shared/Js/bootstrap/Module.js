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
    .constant('shared.internalModulePath', '/admin/modules/shared/js/')
    .constant('shared.pluginModulePath', '/plugins/admin/modules/shared/js/')
    .constant('shared.modulePath', '/cofoundry/admin/modules/shared/js/')
    .constant('shared.serviceBase', '/admin/api/')
    .constant('shared.pluginServiceBase', '/admin/api/plugins/')
    .constant('shared.urlBaseBase', '/admin/')
    .constant('shared.contentPath', '/admin/modules/shared/content/');
