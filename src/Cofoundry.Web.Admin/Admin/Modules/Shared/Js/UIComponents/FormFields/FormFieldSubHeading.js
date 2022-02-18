angular.module('cms.shared').directive('cmsFormFieldSubHeading', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldSubHeading.html',
        require: ['^^cmsForm'],
        replace: true,
        transclude: true
    };
}]);