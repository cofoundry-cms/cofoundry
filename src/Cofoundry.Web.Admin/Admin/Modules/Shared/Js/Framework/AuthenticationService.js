angular.module('cms.shared').factory('authenticationService', [
    '$window',
    'shared.urlLibrary',
function (
    $window,
    urlLibrary
) {

    var service = {};

    /* PUBLIC */

    service.redirectToLogin = function () {
        var loc = $window.location;
        var path = urlLibrary.login(loc.pathname + loc.hash);

        $window.location = path;
    }

    return service;
}]);