angular.module('cms.settings').factory('settings.settingsService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        settingsServiceBase = serviceBase + 'settings',
        generalSettingsRoute = settingsServiceBase + '/generalsite',
        seoSettingsRoute = settingsServiceBase + '/seo';

    /* QUERIES */

    service.getGeneralSiteSettings = function () {

        return $http.get(generalSettingsRoute);
    }

    service.getSeoSettings = function () {

        return $http.get(seoSettingsRoute);
    }
     
    /* COMMANDS */

    service.updateGeneralSiteSettings = function (command) {

        return $http.patch(generalSettingsRoute, command);
    }

    service.updateSeoSettings = function (command) {

        return $http.patch(seoSettingsRoute, command);
    }

    service.clearCache = function (command) {

        return $http.delete(serviceBase + 'cache');
    }

    return service;
}]);