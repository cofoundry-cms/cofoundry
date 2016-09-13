angular.module('cms.images').config([
    '$routeProvider',
    'shared.routingUtilities',
    'images.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Image');

}]);