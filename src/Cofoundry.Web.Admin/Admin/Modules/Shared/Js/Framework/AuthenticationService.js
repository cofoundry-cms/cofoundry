angular.module('cms.shared').factory('authenticationService', ['$window', function ($window) {
    var service = {};

    /* PUBLIC */

    service.redirectToLogin = function () {
        var loc = $window.location;
        var path = '/admin/auth/login?returnUrl=' + encodeURIComponent(loc.pathname + loc.hash);
        $window.location = path;
    }

    return service;
}]);