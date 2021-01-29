angular.module('cms.shared').directive('cmsFormFieldCustomEntityMultiTypeCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.ModelPreviewFieldset',
    'shared.ImagePreviewFieldCollection',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    customEntityService,
    modalDialogService,
    arrayUtilities,
    ModelPreviewFieldset,
    ImagePreviewFieldCollection,
    baseFormFieldFactory) {

    /* VARS */

    var CUSTOM_ENTITY_ID_PROP = 'customEntityId',
        CUSTOM_ENTITY_DEFINITION_CODE_PROP = 'customEntityDefinitionCode',
        PREVIEW_DESCRIPTION_FIELD_NAME = 'previewDescription',
        PREVIEW_IMAGE_FIELD_NAME = 'previewImage',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntityMultiTypeCollection.html',
        scope: _.extend(baseConfig.scope, {
            customEntityDefinitionCodes: '@cmsCustomEntityDefinitionCodes',
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable',
            titleColumnHeader: '@cmsTitleColumnHeader',
            descriptionColumnHeader: '@cmsDescriptionColumnHeader',
            imageColumnHeader: '@cmsImageColumnHeader',
            typeColumnHeader: '@cmsTypeColumnHeader'
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
            metaDataPromise,
            dynamicFormFieldController = _.last(controllers),
            lastDragToIndex;

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            var allDefinitionCodes = getDefinitionCodesAsArray();

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;
            vm.onDropSuccess = onDropSuccess;

            definitionPromise = customEntityService.getDefinitionsByIdRange(allDefinitionCodes).then(function (customEntityDefinitions) {
                vm.customEntityDefinitions = {};

                _.each(customEntityDefinitions, function (customEntityDefinition) {
                    vm.customEntityDefinitions[customEntityDefinition.customEntityDefinitionCode] = customEntityDefinition;

                    // If any are publishable, show the publish column
                    if (!customEntityDefinition.autoPublish) {
                        vm.showPublishColumn = true;
                    }
                });
            });

            metaDataPromise = customEntityService
                .getDataModelSchemasByCodeRange(allDefinitionCodes)
                .then(loadMetaData);

            scope.$watch("vm.model", setGridItems);
            initDisplayFields();
        }

        function initDisplayFields() {
            if (vm.titleColumnHeader === undefined) {
                vm.titleColumnHeader = "Title";
            }

            if (vm.descriptionColumnHeader === undefined) {
                vm.descriptionColumnHeader = "Description";
            }

            if (vm.typeColumnHeader === undefined) {
                vm.typeColumnHeader = "Type";
            }
        }

        /* EVENTS */

        function remove(customEntity, $index) {

            arrayUtilities.removeObject(vm.gridData, customEntity);
            arrayUtilities.remove(vm.model, $index);
            vm.gridImages.remove($index);
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
                for (i = 0; i < newEntityArr.length; i++) {
                    vm.model.push({
                        customEntityId: newEntityArr[i],
                        customEntityDefinitionCode: definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP]
                    });
                }

                setGridItems(vm.model);
            }
        }

        function onDrop($index) {

            // drag drop doesnt give us the to/from index data in the same event, and 
            // we can't use property tracking here, so stuff the index in a variable
            lastDragToIndex = $index;
        }

        function onDropSuccess($index) {

            arrayUtilities.move(vm.gridData, $index, lastDragToIndex);
            vm.gridImages.move($index, lastDragToIndex);

            // Update model with new ordering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, 'title');
                setModelFromGridData();
            }

            // once sorted, load images
            metaDataPromise.then(loadImages);

            function loadImages() {
                vm.gridImages = new ImagePreviewFieldCollection('customEntityDefinitionCode');

                return vm.gridImages.load(vm.gridData, vm.previewFields);
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

        function loadMetaData(modelMetaData) {
            vm.previewFields = {};

            _.each(modelMetaData, function (data) {
                var previewData = new ModelPreviewFieldset(data);

                if (previewData.fields[PREVIEW_DESCRIPTION_FIELD_NAME]) {
                    vm.previewFields.showDescription = true;
                }

                if (previewData.fields[PREVIEW_IMAGE_FIELD_NAME]) {
                    vm.previewFields.showImage = true;
                }
                vm.previewFields[data.customEntityDefinitionCode] = previewData;
            });
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