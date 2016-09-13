angular.module('cms.shared').directive('cmsTableColumnActions', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs, ctrls) {
            element.addClass("actions");
        }
    }
});