angular.module('cms.shared').directive('cmsPageHeaderButtons', function () {
    return {
        restrict: 'E',
        template: '<div class="btn-group page-header-buttons" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});