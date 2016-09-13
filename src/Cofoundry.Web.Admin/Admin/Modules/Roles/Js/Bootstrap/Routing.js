angular.module('cms.roles').config([
    '$routeProvider',
    'shared.routingUtilities',
    'roles.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Role');
}]);