angular.module('cms.shared').constant('shared.routingUtilities', new RoutingUtilitites());

function RoutingUtilitites() {
    var routingUtilities = {};

    /**
        * Maps a standard set of options for the call to $routeProvider.when() or 
        * $routeProvider.otherwise(), setting the controller, template and controllerAs property.
        */
    routingUtilities.mapOptions = function (modulePath, controllerName) {
        return {
            controller: controllerName + 'Controller',
            controllerAs: 'vm',
            templateUrl: modulePath + 'Routes/' + controllerName + '.html',
            reloadOnSearch: false
        }
    },

    /** 
        * Maps the standard list/new/details routes for the specified entity. Controllers
        * must match the standard naming 'AddEntityController', 'EntityListController', 'EntityDetailsController'
        */
    routingUtilities.registerCrudRoutes = function ($routeProvider, modulePath, entityName) {

        $routeProvider
            .when('/new', routingUtilities.mapOptions(modulePath, 'Add' + entityName))
            .when('/:id', routingUtilities.mapOptions(modulePath, entityName + 'Details'))
            .otherwise(routingUtilities.mapOptions(modulePath, entityName + 'List'));
    }

    return routingUtilities;
};