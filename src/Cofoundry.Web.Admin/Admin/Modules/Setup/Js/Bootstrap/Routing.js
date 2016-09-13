angular.module('cms.setup').config([
    '$routeProvider',
    'shared.routingUtilities',
    'setup.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'SetupDetails'));

}]);