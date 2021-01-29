angular.module('cms.shared').directive('cmsFormFieldText', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldText.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'placeholder',
            'pattern',
            'disabled',
            'readonly',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);