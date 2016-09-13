angular.module('cms.users').factory('users.roleService', [
    '$http',
    'shared.serviceBase',
    'users.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        roleServiceBase = serviceBase + 'roles';

    /* QUERIES */

    service.getSelectionList = function () {
        return $http.get(roleServiceBase, {
            params: {
                userAreaCode: options.userAreaCode,
                excludeAnonymous: true
            }
        });
    }

    /* PRIVATES */

    return service;
}]);