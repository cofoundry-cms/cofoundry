angular.module('cms.shared').directive('cmsFormFieldContainer', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldContainer.html',
        require: ['^^cmsForm'],
        replace: true,
        transclude: true,
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription'
        }
    };
}]);