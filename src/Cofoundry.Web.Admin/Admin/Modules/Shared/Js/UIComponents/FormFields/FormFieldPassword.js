angular.module('cms.shared').directive('cmsFormFieldPassword', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldPassword.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);