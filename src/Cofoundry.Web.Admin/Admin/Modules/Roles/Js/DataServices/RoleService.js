angular.module('cms.roles').factory('roles.roleService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
        COFOUNDRY_USER_AREA_CODE = 'COF',
        roleServiceBase = serviceBase + 'roles';

    /* QUERIES */

    service.getAll = function (query) {

        return $http.get(roleServiceBase, {
            params: query
        });
    }

    service.getById = function (roleId) {

        return $http.get(getIdRoute(roleId));
    }

    /* COMMANDS */

    service.add = function (command) {
        return $http.post(roleServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.roleId), command);
    }

    service.remove = function (roleId) {

        return $http.delete(getIdRoute(roleId));
    }

    /* PRIVATES */

    function getIdRoute(roleId) {
        return roleServiceBase + '/' + roleId;
    }


    /* PRIVATES */

    return service;
}]);