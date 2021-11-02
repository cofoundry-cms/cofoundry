angular.module('cms.shared').factory('shared.userAreaService', [
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

    service.getByCode = function (code) {
        return service
            .getAll()
            .then(function(userAreas) {
                return _.find(userAreas, function(userArea) {
                    return userArea.userAreaCode == code;
                })
            });
    }

    return service;
}]);