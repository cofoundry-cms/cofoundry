angular.module('cms.shared').directive('cmsTableGroupHeading', function () {
    return {
        restrict: 'E',
        template: '<h5 class="table-group-heading" ng-transclude></h5>',
        replace: true,
        transclude: true
    }
});