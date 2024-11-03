angular.module('cms.errors').factory('errors.errorService', [
    '$http',
    'shared.pluginServiceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        errorServiceBase = serviceBase + 'errors';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(errorServiceBase, {
            params: query
        });
    }

    service.getById = function (userId) {

        return $http.get(getIdRoute(userId));
    }

    /* PRIVATES */

    function getIdRoute(id) {
        return errorServiceBase + '/' + id;
    }

    return service;
}]);