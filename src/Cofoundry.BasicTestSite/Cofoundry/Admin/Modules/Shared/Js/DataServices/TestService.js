angular.module('cms.shared').factory('shared.testService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        testServiceBase = serviceBase + 'test';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(testServiceBase);
    }

    return service;

}]);