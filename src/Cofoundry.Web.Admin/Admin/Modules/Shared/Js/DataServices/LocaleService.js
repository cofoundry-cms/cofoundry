angular.module('cms.shared').factory('shared.localeService', ['$http', 'shared.serviceBase', function ($http, serviceBase) {
    var service = {},
        localeServiceBase = serviceBase + 'locales';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(localeServiceBase);
    }

    return service;
}]);