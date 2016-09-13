angular.module('cms.settings').config([
    '$routeProvider',
    'shared.routingUtilities',
    'settings.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'SettingsDetails'));

}]);