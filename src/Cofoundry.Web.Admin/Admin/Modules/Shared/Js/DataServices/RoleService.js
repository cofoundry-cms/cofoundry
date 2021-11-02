angular.module('cms.shared').factory('shared.roleService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        roleServiceBase = serviceBase + 'roles';

    /* QUERIES */

    service.search = function (query) {

        return $http.get(roleServiceBase, {
            params: query
        });
    }

    service.getSelectionList = function (userAreaCode) {
        return service.search({
            userAreaCode: userAreaCode,
            excludeAnonymous: true
        });
    }

    service.getById = function (roleId) {

        return $http.get(getIdRoute(roleId));
    }

    /* PRIVATES */

    function getIdRoute(roleId) {
        return roleServiceBase + '/' + roleId;
    }

    return service;
}]);