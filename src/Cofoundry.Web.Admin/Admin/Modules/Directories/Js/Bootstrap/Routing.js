angular.module('cms.directories').config([
    '$routeProvider',
    'shared.routingUtilities',
    'directories.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Directory');
}]);