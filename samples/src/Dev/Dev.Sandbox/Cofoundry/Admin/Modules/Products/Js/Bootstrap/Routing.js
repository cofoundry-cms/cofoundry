angular.module('cms.products').config([
    '$routeProvider',
    'shared.routingUtilities',
    'products.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'ProductDetails'));

}]);