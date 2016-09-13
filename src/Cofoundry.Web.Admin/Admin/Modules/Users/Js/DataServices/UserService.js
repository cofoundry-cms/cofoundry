angular.module('cms.users').factory('users.userService', [
    '$http',
    'shared.serviceBase',
    'users.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        userServiceBase = serviceBase + 'users';

    /* QUERIES */

    service.getAll = function (query) {
        query = addUserArea(query);

        return $http.get(userServiceBase, {
            params: query
        });
    }

    service.getById = function (userId) {

        return $http.get(getIdRoute(userId));
    }

    /* COMMANDS */

    service.add = function (command) {
        command = addUserArea(command);
        command.generatePassword = true;

        return $http.post(userServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.userId), command);
    }

    service.remove = function (id) {

        return $http.delete(getIdRoute(id));
    }

    /* PRIVATES */

    function getIdRoute(userId) {
        return userServiceBase + '/' + userId;
    }

    function addUserArea(o) {
        o = o || {};
        o.userAreaCode = options.userAreaCode;

        return o;
    }

    return service;
}]);