angular.module('cms.shared').directive('cmsFormFieldUrl', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldUrl.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'placeholder',
            'pattern',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);

}]);