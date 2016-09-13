angular.module('cms.roles').factory('roles.permissionService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        permissionServiceBase = serviceBase + 'permissions';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(permissionServiceBase);
    }

    return service;
}]);