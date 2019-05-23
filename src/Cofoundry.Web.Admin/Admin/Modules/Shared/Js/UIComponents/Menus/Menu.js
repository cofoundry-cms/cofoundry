angular.module('cms.shared').directive('cmsMenu', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        replace: true,
        transclude: true,
        templateUrl: modulePath + 'UIComponents/Menus/Menu.html',
        scope: {
            icon: '@cmsIcon'
        }
    };
}]);