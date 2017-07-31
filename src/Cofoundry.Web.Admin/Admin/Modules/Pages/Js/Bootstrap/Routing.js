angular.module('cms.pages').config([
    '$routeProvider',
    'shared.routingUtilities',
    'pages.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    var mapOptions = routingUtilities.mapOptions.bind(null, modulePath);

    $routeProvider
        .when('/new', mapOptions('AddPage'))
        .when('/:id', mapOptions('PageDetails'))
        .otherwise(mapOptions('PageList'));

}]);