angular.module('cms.shared').directive('cmsPageSubHeader', function () {
    return {
        restrict: 'E',
        template: '<div class="page-sub-header" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});