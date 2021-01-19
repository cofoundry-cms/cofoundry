angular.module('cms.shared').directive('cmsFormFieldFilteredDropdown', [
    '$q',
    '_',
    'shared.internalModulePath',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    $q,
    _,
    modulePath,
    stringUtilities,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldFilteredDropdown.html',
        passThroughAttributes: [
            'required',
            'disabled'
        ],
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            defaultItemText: '@cmsDefaultItemText',
            href: '@cmsHref',
            searchFunction: '&cmsSearchFunction',
            initialItemFunction: '&cmsInitialItemFunction',
            optionName: '@cmsOptionName',
            optionValue: '@cmsOptionValue',
            required: '=cmsRequired'
        }),
        require: _.union(baseFormFieldFactory.defaultConfig.require, ['?^^cmsFormDynamicFieldSet']),
        link: link,
        transclude: true
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm,
            dynamicFormFieldController = _.last(controllers);

        init();

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        /* Init */

        function init() {
            vm.refreshDataSource = refreshDataSource;
            vm.dataSource = [];
            vm.hasRequiredAttribute = _.has(attrs, 'required');
            vm.placeholder = attrs['placeholder'];
            vm.clearSelected = clearSelected;

            scope.$watch("vm.model", setSelectedText);
        }

        function setSelectedText(id) {
            vm.selectedText = '';

            if (id && vm.dataSource && vm.dataSource.length) {
                var item = _.find(vm.dataSource, function (item) {
                    return id == item[vm.optionValue];
                });

                if (item) vm.selectedText = item[vm.optionName];
            }

            if (!vm.selectedText && id && vm.initialItemFunction) {
                $q.when(vm.initialItemFunction({ id: id })).then(setSelectedItem);
            }

            function setSelectedItem(item) {
                if (item) {
                    vm.selectedText = item[vm.optionName];
                    refreshDataSource(vm.selectedText);
                }
            }
        }

        function clearSelected() {
            vm.selectedText = '';

            if (vm.model) {
                vm.model = null;
            }
        }

        function refreshDataSource(search) {
            var query = {
                text: search,
                pageSize: 20
            }

            if (vm.localeId) {
                query.localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                query.localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            return vm.searchFunction({ $query: query }).then(loadResults);

            function loadResults(results) {
                vm.dataSource = results.items;
            }
        }
    }

}]);