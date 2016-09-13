angular.module('cms.documents').config([
    '$routeProvider',
    'shared.routingUtilities',
    'documents.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Document');

}]);