angular.module('cms.shared').directive('cmsPageActions', function () {
    return {
        restrict: 'E',
        template: '<div class="page-actions" ng-transclude></div>',
        replace: true,
        transclude: true,
    }
});