angular.module('cms.errors').config([
    '$routeProvider',
    'shared.routingUtilities',
    'errors.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .when('/:id', routingUtilities.mapOptions(modulePath, 'ErrorDetails'))
        .otherwise(routingUtilities.mapOptions(modulePath, 'ErrorList'));
}]);