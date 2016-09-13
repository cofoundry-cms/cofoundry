angular.module('cms.shared').directive('cmsFormFieldDropdown', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDropdown.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            options: '=cmsOptions',
            optionValue: '@cmsOptionValue',
            optionName: '@cmsOptionName',
            defaultItemText: '@cmsDefaultItemText',
            required: '=cmsRequired',
            disabled: '=cmsDisabled'
        }),
        passThroughAttributes: [
            'placeholder',
            'disabled',
            'cmsMatch'
        ],
        getInputEl: getInputEl,
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* PRIVATE */

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm;
        init();

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        /* Init */

        function init() {
            vm.isRequiredAttributeDefined = angular.isDefined(attrs.required);

            var optionsWatch = scope.$watch('vm.options', function () {
                bindDisplayValue();
                // remove watch
                optionsWatch();
            });

            scope.$watch('vm.model', bindDisplayValue);
        }

        /* Helpers */

        function bindDisplayValue() {
             var selectedOption = _.find(vm.options, function (option) {
                return option[vm.optionValue] == vm.model;
             });

             vm.displayValue = selectedOption ? selectedOption[vm.optionName] : vm.defaultItemText;
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('select');
    }

}]);