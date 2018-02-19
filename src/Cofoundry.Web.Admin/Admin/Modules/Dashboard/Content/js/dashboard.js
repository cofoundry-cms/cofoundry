angular
    .module('cms.dashboard', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('dashboard.modulePath', '/Admin/Modules/Dashboard/Js/');
angular.module('cms.dashboard').config([
    '$routeProvider',
    'shared.routingUtilities',
    'dashboard.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'Dashboard'));

}]);
angular.module('cms.dashboard').factory('dashboard.dashboardService', [
    '$http', 
    '_',
    'shared.serviceBase'
, function (
    $http,
    _,
    serviceBase
    ) {

    var service = {};

    /* QUERIES */
    
    service.getContent = function () {
        return $http.get(serviceBase + 'dashboard');
    }

    return service;
}]);
angular.module('cms.dashboard').directive('cmsDashboardComponent', [
    'dashboard.modulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DashboardComponent.html',
        scope: {
            heading: '@cmsHeading',
            listUrl: '@cmsListUrl',
            createUrl: '@cmsCreateUrl',
            entityName: '@cmsEntityName',
            entityNamePlural: '@cmsEntityNamePlural',
            numItems: '=cmsNumItems',
            loader: '=cmsLoader'
        },
        replace: true,
        controller: controller,
        controllerAs: 'vm',
        bindToController: true,
        transclude: true
    };

    /* CONTROLLER */

    function controller() {
    }
}]);
angular.module('cms.dashboard').controller('DashboardController', [
    '_',
    'shared.modalDialogService',
    'shared.urlLibrary',
    'dashboard.dashboardService',
    function (
    _,
    modalDialogService,
    urlLibrary,
    dashboardService
    ) {

    var vm = this;

    init();

    function init() {
        vm.urlLibrary = urlLibrary;

        dashboardService
            .getContent()
            .then(function (content) {
                vm.content = content;
            });
    }

}]);