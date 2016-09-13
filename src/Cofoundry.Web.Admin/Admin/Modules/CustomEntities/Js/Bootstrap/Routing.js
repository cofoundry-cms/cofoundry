angular.module('cms.customEntities').config([
    '$routeProvider',
    'shared.routingUtilities',
    'customEntities.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'CustomEntity');
}]);