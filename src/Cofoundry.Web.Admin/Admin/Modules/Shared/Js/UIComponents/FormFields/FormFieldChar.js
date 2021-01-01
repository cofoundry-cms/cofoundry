angular.module('cms.shared').directive('cmsFormFieldChar', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldChar.html',
        passThroughAttributes: [
            'required',
            'placeholder',
            'pattern',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);