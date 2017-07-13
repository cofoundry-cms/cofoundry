angular.module('cms.shared').directive('cmsFormFieldCustomEntityCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    customEntityService,
    modalDialogService,
    arrayUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var CUSTOM_ENTITY_ID_PROP = 'customEntityId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntityCollection.html',
        scope: _.extend(baseConfig.scope, {
            customEntityDefinitionCode: '@cmsCustomEntityDefinitionCode',
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable'
        }),
        require: _.union(baseConfig.require, ['?^^cmsFormDynamicFieldSet']),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            definitionPromise,
            dynamicFormFieldController = _.last(controllers);

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            definitionPromise = customEntityService.getDefinition(vm.customEntityDefinitionCode).then(function (customEntityDefinition) {
                vm.customEntityDefinition = customEntityDefinition;
            });

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(customEntity) {

            arrayUtilities.removeObject(vm.gridData, customEntity);
            arrayUtilities.removeObject(vm.model, customEntity[CUSTOM_ENTITY_ID_PROP]);
        }

        function showPicker() {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityPickerDialog.html',
                controller: 'CustomEntityPickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    customEntityDefinition: vm.customEntityDefinition,
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newEntityArr) {
                vm.model = newEntityArr
                setGridItems(newEntityArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, CUSTOM_ENTITY_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, 'title');
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, CUSTOM_ENTITY_ID_PROP);
        }

        /* HELPERS */

        function getPermission(code) {
            return permissionValidationService.hasPermission(options.customEntityDefinitionCode + code);
        }

        function getFilter() {
            var filter = {},
                localeId;

            if (vm.localeId) {
                localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            if (localeId) {
                filter.localeId = localeId;
            }

            return filter;
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, CUSTOM_ENTITY_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                var promise = customEntityService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    orderGridItemsAndSetModel();
                });

                vm.gridLoadState.offWhen(definitionPromise, promise);
            }
        }
    }
}]);