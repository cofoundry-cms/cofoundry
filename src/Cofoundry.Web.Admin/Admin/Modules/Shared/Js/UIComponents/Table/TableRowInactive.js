angular.module('cms.shared').directive('cmsTableRowInactive', function () {
    return {
        restrict: 'A',
        scope: {
            tableRowInactive: '=cmsTableRowInactive',
        },
        link: function (scope, element, attrs, ctrls) {

            scope.$watch('tableRowInactive', function (isTableRowInactive) {
                element.toggleClass("inactive", !!isTableRowInactive);
            });
        }
    }
});