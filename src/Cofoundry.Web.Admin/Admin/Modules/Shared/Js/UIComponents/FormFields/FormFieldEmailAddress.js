angular.module('cms.shared').directive('cmsFormFieldEmailAddress', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldEmailAddress.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'placeholder',
            'disabled',
            'cmsMatch'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        // add custom error for email since its not attribute based like other validation messages
        vm.validators.push({
            name: 'email',
            message: 'Please enter a valid email address'
        });
    }
}]);