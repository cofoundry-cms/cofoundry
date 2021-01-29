angular.module('cms.shared').directive('cmsFormFieldCheckboxList', [
    '_',
    '$http',
    'shared.internalModulePath',
    'shared.optionSourceService',
    'baseFormFieldFactory',
function (
    _,
    $http,
    modulePath,
    optionSourceService,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldCheckboxList.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            options: '=cmsOptions',
            optionValue: '@cmsOptionValue',
            optionName: '@cmsOptionName',
            optionsApi: '@cmsOptionsApi',
            noValueText: '@cmsNoValueText',
            required: '=cmsRequired',
            disabled: '=cmsDisabled'
        }),
        passThroughAttributes: [
            'placeholder',
            'disabled',
            'cmsMatch',
            'required'
        ],
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
            vm.onCheckChanged = updateModel;

            if (vm.optionsApi) {
                // options bound by api call
                getOptions(vm.optionsApi);

            } else {

                // options provided as a model
                var optionsWatch = scope.$watch('vm.options', function () {

                    bindOptions(vm.options);
                    updateDisplayValues();
                });
            }

            scope.$watch('vm.model', onModelChanged);
        }

        /* Events */

        function onModelChanged() {
            updateOptionSelectionFromModel();
            updateDisplayValues();
        }

        /* Helpers */

        function getOptions(apiPath) {
            return optionSourceService.getFromApi(apiPath).then(loadOptions);

            function loadOptions(options) {
                bindOptions(options);
                updateDisplayValues();
            }
        }

        /**
         * Copies over the option collection so the original
         * is not modified with selcted properties.
         */
        function bindOptions(options) {
            vm.listOptions = _.map(options, mapOption);

            updateOptionSelectionFromModel();

            function mapOption(option) {
                return {
                    text: option[vm.optionName],
                    value: option[vm.optionValue]
                };
            }
        }

        /**
         * Updates the selected values in the listOptions
         * collection from the model,
         */
        function updateOptionSelectionFromModel() {
            var isModelEmpty = !vm.model || !vm.model.length;
            vm.displayValues = [];

            _.each(vm.listOptions, function (option) {

                option.selected = !isModelEmpty && !!_.find(vm.model, function (m) {
                    return m === option.value;
                });
            });
        }

        function updateDisplayValues() {

            vm.displayValues = _.chain(vm.listOptions)
                .filter(function (o) { return o.selected })
                .pluck('text')
                .value();
        }

        function updateModel() {

            vm.model = _.chain(vm.listOptions)
                .filter(function (o) { return o.selected })
                .pluck('value')
                .value();
        }
    }
}]);