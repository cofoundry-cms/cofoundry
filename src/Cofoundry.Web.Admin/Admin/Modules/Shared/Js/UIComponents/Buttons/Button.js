angular.module('cms.shared').directive('cmsButton', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/Button.html',
        scope: {
            text: '@cmsText'
        }
    };
}]);