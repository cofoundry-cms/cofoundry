angular.module('cms.shared').directive('cmsFormFieldNumber', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldNumber.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'min',
            'max',
            'step',
            'disabled',
            'placeholder',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);