angular.module('cms.pages').factory('pages.pageTemplateService', [
    '$http',
    '$q',
    'shared.serviceBase',
function (
    $http,
    $q,
    serviceBase) {

    var service = {},
        pageTemplateServiceBase = serviceBase + 'page-templates';

    /* QUERIES */

    service.getAll = function () {
        var def = $q.defer();

        $http.get(pageTemplateServiceBase).then(function (pagedResult) {
            def.resolve(pagedResult.items);
        }, def.reject);

        return def.promise;
    }

    service.getExtensionDataModelSchemas = function (pageTemplateId) {
        return $http.get(pageTemplateServiceBase + '/' + pageTemplateId + '/extension-data-model-schemas');;
    }
    
    /* COMMANDS */

    return service;
}]);