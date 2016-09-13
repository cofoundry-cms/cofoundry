angular.module('cms.shared').directive('cmsFormFieldCheckbox', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldCheckbox.html',
        passThroughAttributes: [
            'disabled'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);