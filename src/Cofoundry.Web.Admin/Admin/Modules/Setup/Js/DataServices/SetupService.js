angular.module('cms.setup').factory('setup.setupService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        setupServiceBase = serviceBase + 'setup';

    /* COMMANDS */

    service.run = function (command) {

        return $http.post(setupServiceBase, command);
    }

    return service;
}]);