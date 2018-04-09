angular.module('cms.shared').directive('cmsFormFieldCustomEntityCollection', [
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
            metaDataPromise,
            dynamicFormFieldController = _.last(controllers),
            lastDragToIndex;

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;
            vm.onDropSuccess = onDropSuccess;

            definitionPromise = customEntityService.getDefinition(vm.customEntityDefinitionCode).then(function (customEntityDefinition) {
                vm.customEntityDefinition = customEntityDefinition;
            });

            metaDataPromise = customEntityService
                .getDataModelSchema(vm.customEntityDefinitionCode)
                .then(loadMetaData);

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(customEntity, $index) {

            arrayUtilities.removeObject(vm.gridData, customEntity);
            arrayUtilities.removeObject(vm.model, customEntity[CUSTOM_ENTITY_ID_PROP]);
            vm.gridImages.remove($index);
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
                vm.gridImages = new ImagePreviewFieldCollection();
                return vm.gridImages.load(vm.gridData, vm.previewFields);
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

        function loadMetaData(modelMetaData) {
            vm.previewFields = new ModelPreviewFieldset(modelMetaData);
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