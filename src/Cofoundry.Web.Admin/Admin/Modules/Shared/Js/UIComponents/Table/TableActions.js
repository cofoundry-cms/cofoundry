angular.module('cms.shared').directive('cmsTableActions', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Table/TableActions.html',
        replace: true,
        transclude: true,
        link: function (scope, el, attrs, controllers, transclude) {

        }
    };
}]);