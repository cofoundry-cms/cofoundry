angular.module('cms.shared').directive('cmsFormFieldColor', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldColor.html',
        passThroughAttributes: [
            'required',
            'disabled'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        // add custom error for value since its not attribute based like other validation messages
        vm.validators.push({
            name: 'pattern',
            message: vm.title + " must be a hexadecimal colour value e.g. '#EFEFEF' or '#fff'"
        });
    }
}]);