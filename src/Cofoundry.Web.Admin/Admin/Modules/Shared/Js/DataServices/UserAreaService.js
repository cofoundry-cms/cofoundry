angular.module('cms.shared').factory('shared.userAreaService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
    userAreaServiceBase = serviceBase + 'user-areas/';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(userAreaServiceBase);
    }

    service.getByCode = function (code) {
        return service
            .getAll()
            .then(function(userAreas) {
                return _.find(userAreas, function(userArea) {
                    return userArea.userAreaCode == code;
                })
            });
    }

    service.getPasswordPolicy = function (code) {
        return $http.get(userAreaServiceBase + code + '/password-policy');
    }

    return service;
}]);