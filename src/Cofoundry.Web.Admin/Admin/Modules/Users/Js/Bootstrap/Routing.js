angular.module('cms.users').config([
    '$routeProvider',
    'shared.routingUtilities',
    'users.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'User');
}]);