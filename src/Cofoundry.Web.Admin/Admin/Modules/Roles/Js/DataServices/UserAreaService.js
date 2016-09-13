angular.module('cms.roles').factory('roles.userAreaService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {};

    /* QUERIES */

    service.getAll = function () {
        return $http.get(serviceBase + 'user-areas');
    }

    return service;
}]);