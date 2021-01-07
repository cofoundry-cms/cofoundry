angular.module('cms.shared').directive('cmsFormFieldDateTime', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDateTime.html',
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
                var minDate = attrs.min.replace('T', ' ');
                vm.addOrUpdateValidator({
                    name: 'min',
                    message: "This date cannot be before " + minDate
                });
            }
            if (attrs.max) {
                var maxDate = attrs.min.replace('T', ' ');
                vm.addOrUpdateValidator({
                    name: 'max',
                    message: "This date cannot be after " + maxDate
                });
            }
        }
    }
}]);