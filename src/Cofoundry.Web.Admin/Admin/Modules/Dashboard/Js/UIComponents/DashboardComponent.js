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