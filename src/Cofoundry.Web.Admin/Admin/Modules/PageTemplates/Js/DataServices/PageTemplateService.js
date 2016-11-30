angular.module('cms.pageTemplates').factory('pageTemplates.pageTemplateService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pageTemplateServiceBase = serviceBase + 'page-templates';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pageTemplateServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(getIdRoute(imageId));
    }

    /* PRIVATES */

    function getIdRoute(id) {
        return pageTemplateServiceBase + '/' + id;
    }

    return service;
}]);