angular.module('cms.shared').directive('cmsFormFieldDate', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDate.html',
        passThroughAttributes: [
            'required',
            'min',
            'max',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);