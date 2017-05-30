angular.module('cms.shared').directive('cmsButtonSubmit', [
    'shared.internalModulePath',
function (
    modulePath
) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/ButtonSubmit.html',
        scope: {
            text: '@cmsText'
        }
    };
}]);