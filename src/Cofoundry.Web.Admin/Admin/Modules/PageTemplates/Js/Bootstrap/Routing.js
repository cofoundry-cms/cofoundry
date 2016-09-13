angular.module('cms.pageTemplates').config([
    '$routeProvider',
    'shared.routingUtilities',
    'pageTemplates.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'PageTemplate');

}]);