angular.module('cms.shared').directive('cmsFormFieldDropdown', [
    '_',
    '$http',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    $http,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDropdown.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            options: '=cmsOptions',
            optionValue: '@cmsOptionValue',
            optionName: '@cmsOptionName',
            optionsApi: '@cmsOptionsApi',
            defaultItemText: '@cmsDefaultItemText',
            href: '@cmsHref',
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

            if (vm.optionsApi) {
                // options bound by api call
                getOptions(vm.optionsApi);

            } else {

                // options provided as a model
                var optionsWatch = scope.$watch('vm.options', function () {

                    // need to copy b/c of assignment issue with bound attributes
                    vm.listOptions = vm.options;

                    bindDisplayValue();
                });
            }

            scope.$watch('vm.model', bindDisplayValue);
        }

        /* Helpers */

        function getOptions(apiPath) {
            return $http.get(apiPath).then(loadOptions);

            function loadOptions(options) {
                vm.listOptions = options;
                bindDisplayValue();
            }
        }

        function bindDisplayValue() {

            var selectedOption = _.find(vm.listOptions, function (option) {
                return option[vm.optionValue] == vm.model;
            });

            vm.displayValue = selectedOption ? selectedOption[vm.optionName] : vm.defaultItemText;

            // if the options and model are bound, and the option does not appear in the 
            // list, remove the value from the model.
            if (!selectedOption && vm.model != undefined && vm.listOptions) {
                vm.model = undefined;
            }
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('select');
    }

}]);