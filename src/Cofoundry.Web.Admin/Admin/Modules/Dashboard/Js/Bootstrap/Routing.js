angular.module('cms.dashboard').config([
    '$routeProvider',
    'shared.routingUtilities',
    'dashboard.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'Dashboard'));

}]);