angular.module('cms.shared').factory('shared.localStorage', ['shared.serviceBase', function (serviceBase) {
    var service = {},
        localStorageServiceBase = serviceBase + 'localStorage';

    service.setValue = function (key, value) {
        localStorage.setItem(key, value);
    }

    service.getValue = function (key) {
        var value = localStorage.getItem(key);
        return value;
    }

    return service;
}]);