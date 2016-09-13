angular.module('cms.shared').directive('cmsTableContainer', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Table/TableContainer.html',
        replace: true,
        transclude: true,
        link: function (scope, el, attrs, controllers, transclude) {

            el.find('table').addClass('table');

            //transclude(scope, function (clone) {
            //    clone.find('table').addClass('table');
            //    el.append(clone);
            //});
        }
    };
}]);