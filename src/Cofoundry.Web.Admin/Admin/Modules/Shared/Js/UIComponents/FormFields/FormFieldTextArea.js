angular.module('cms.shared').directive('cmsFormFieldTextArea', [
    'shared.internalModulePath', 
    'shared.stringUtilities',
    'baseFormFieldFactory', 
function (
    modulePath, 
    stringUtilities,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldTextArea.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'placeholder',
            'ngMinlength',
            'ngMaxlength',
            'ngPattern',
            'disabled',
            'rows',
            'cols',
            'wrap'
        ],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('textarea');
    }

}]);