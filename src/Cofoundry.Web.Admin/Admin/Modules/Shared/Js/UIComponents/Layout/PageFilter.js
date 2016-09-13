angular.module('cms.shared').directive('cmsPageFilter', function () {
    return {
        restrict: 'E',
        template: '<div class="page-filter" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});