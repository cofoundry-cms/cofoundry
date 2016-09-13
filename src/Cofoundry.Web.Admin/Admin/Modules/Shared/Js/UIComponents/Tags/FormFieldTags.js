/**
 * Formfield wrapper around a TagInput
 */
angular.module('cms.shared').directive('cmsFormFieldTags', [
    '_',
    'shared.internalModulePath',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    stringUtilities,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/Tags/FormFieldTags.html',
        passThroughAttributes: [
            'required'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);