angular.module('cms.shared').directive('cmsPageBody', function () {
    return {
        restrict: 'E',
        template: '<div class="page-body {{ contentType }} {{ subHeader }}"><div class="form-wrap" ng-transclude></div></div>',
        scope: {
            contentType: '@cmsContentType',
            subHeader: '@cmsSubHeader'
        },
        replace: true,
        transclude: true
    }
});