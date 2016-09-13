angular.module('cms.shared').directive('cmsFormFieldValidationSummary', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldValidationSummary.html',
        replace: true
    };
}]);