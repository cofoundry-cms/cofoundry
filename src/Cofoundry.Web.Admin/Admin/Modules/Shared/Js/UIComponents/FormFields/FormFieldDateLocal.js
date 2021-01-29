angular.module('cms.shared').directive('cmsFormFieldDateLocal', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDateLocal.html',
        passThroughAttributes: [
            'required',
            'min',
            'max',
            'disabled',
            'cmsMatch'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        init();

        /* Init */

        function init() {
            if (attrs.min) {
                vm.addOrUpdateValidator({
                    name: 'min',
                    message: "This date cannot be before " + attrs.min
                });
            }
            if (attrs.max) {
                vm.addOrUpdateValidator({
                    name: 'max',
                    message: "This date cannot be after " + attrs.max
                });
            }
        }
    }
}]);