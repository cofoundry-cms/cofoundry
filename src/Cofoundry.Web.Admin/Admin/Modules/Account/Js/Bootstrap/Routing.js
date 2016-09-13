angular.module('cms.account').config([
    '$routeProvider',
    'shared.routingUtilities',
    'account.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .when('/change-password', routingUtilities.mapOptions(modulePath, 'ChangePassword'))
        .otherwise(routingUtilities.mapOptions(modulePath, 'AccountDetails'));
}]);