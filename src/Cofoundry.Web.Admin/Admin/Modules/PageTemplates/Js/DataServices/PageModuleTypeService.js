angular.module('cms.pageTemplates').factory('pageTemplates.pageModuleTypeService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        moduleTypesServiceBase = serviceBase + 'page-module-types';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(moduleTypesServiceBase);
    }

    return service;
}]);