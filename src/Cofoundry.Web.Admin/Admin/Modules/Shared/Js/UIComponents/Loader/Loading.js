angular.module('cms.shared').directive('cmsLoading', function () {
    return {
        restrict: 'A',
        link: function (scope, el, attributes) {

            scope.$watch(attributes.cmsLoading, function (isLoading) {
                el.toggleClass('loading', isLoading);
            });
        }
    };
});