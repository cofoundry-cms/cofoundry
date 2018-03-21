angular.module('cms.shared').directive('cmsFormFieldCustomEntityMultiTypeCollection', [
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
        CUSTOM_ENTITY_DEFINITION_CODE_PROP = 'customEntityDefinitionCode',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntityMultiTypeCollection.html',
        scope: _.extend(baseConfig.scope, {
            customEntityDefinitionCodes: '@cmsCustomEntityDefinitionCodes',
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

            definitionPromise = customEntityService.getDefinitionsByIdRange(getDefinitionCodesAsArray()).then(function (customEntityDefinitions) {
                vm.customEntityDefinitions = _.indexBy(customEntityDefinitions, 'customEntityDefinitionCode');
            });

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(customEntity) {

            arrayUtilities.removeObject(vm.gridData, customEntity);
            arrayUtilities.removeObject(vm.model, customEntity, CUSTOM_ENTITY_ID_PROP);
        }

        function showPicker(definition) {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityPickerDialog.html',
                controller: 'CustomEntityPickerDialogController',
                options: {
                    selectedIds: getSelectedIds(),
                    customEntityDefinition: definition,
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function getSelectedIds() {
                return _.chain(vm.model)
                        .where({ customEntityDefinitionCode: definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP] })
                        .map(function (value) {
                            return value[CUSTOM_ENTITY_ID_PROP];
                        })
                        .value();
            }

            function onSelected(newEntityArr) {
                if (!vm.model) vm.model = [];

                // Iterate through existing items - remove items of the type just edited if does not exist in new array
                if (vm.model.length > 0) {
                    for (var i = 0; i < vm.model.length; i++) {
                        if (vm.model[i].customEntityDefinitionCode === definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP]) {

                            // See if this item already exists so we dont add again or lose ordering
                            var index = newEntityArr.indexOf(vm.model[i].customEntityId);
                            if (index > -1) {
                                newEntityArr.splice(index, 1);
                                continue;
                            }

                            vm.model.splice(i, 1);
                        }
                    }
                }

                // Add new items to the end of the list
                for (var i = 0; i < newEntityArr.length; i++) {
                    vm.model.push({
                        customEntityId: newEntityArr[i],
                        customEntityDefinitionCode: definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP]
                    });
                }

                setGridItems(vm.model);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, CUSTOM_ENTITY_ID_PROP);

            // Update model with new ordering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, 'title');
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.map(vm.gridData, function(entity) {
                return _.pick(entity, CUSTOM_ENTITY_ID_PROP, CUSTOM_ENTITY_DEFINITION_CODE_PROP);
            });
        }

        /* HELPERS */

        function getDefinitionCodesAsArray() {
            if (!vm.customEntityDefinitionCodes) return [];
            return vm.customEntityDefinitionCodes.split(',');
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
         * Load the grid data if it is inconsistent with the values collection.
         */
        function setGridItems(values) {
            var ids = values ? _.pluck(values, CUSTOM_ENTITY_ID_PROP) : [];

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