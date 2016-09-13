angular.module('cms.pages').factory('pages.directoryService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
        directoryServiceBase = serviceBase + 'webdirectories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    return service;
}]);