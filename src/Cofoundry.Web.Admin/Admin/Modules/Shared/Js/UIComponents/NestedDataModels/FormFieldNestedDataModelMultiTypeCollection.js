angular.module('cms.shared').directive('cmsFormFieldNestedDataModelMultiTypeCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.nestedDataModelSchemaService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.ModelPreviewFieldset',
    'shared.ImagePreviewFieldCollection',
    'baseFormFieldFactory',
    function (
        _,
        modulePath,
        LoadState,
        nestedDataModelSchemaService,
        modalDialogService,
        arrayUtilities,
        ModelPreviewFieldset,
        ImagePreviewFieldCollection,
        baseFormFieldFactory) {

        /* VARS */

        var baseConfig = baseFormFieldFactory.defaultConfig,
            PREVIEW_TITLE_FIELD_NAME = 'previewTitle';

        /* CONFIG */

        var config = {
            templateUrl: modulePath + 'UIComponents/NestedDataModels/FormFieldNestedDataModelMultiTypeCollection.html',
            scope: _.extend(baseConfig.scope, {
                minItems: '@cmsMinItems',
                maxItems: '@cmsMaxItems',
                modelTypes: '@cmsModelTypes',
                orderable: '=cmsOrderable',
                titleColumnHeader: '@cmsTitleColumnHeader',
                descriptionColumnHeader: '@cmsDescriptionColumnHeader',
                imageColumnHeader: '@cmsImageColumnHeader',
                typeColumnHeader: '@cmsTypeColumnHeader'
            }),
            passThroughAttributes: [
                'required'
            ],
            link: link
        };

        return baseFormFieldFactory.create(config);

        /* LINK */

        function link(scope, el, attributes, controllers) {
            var vm = scope.vm,
                dynamicFormFieldController = _.last(controllers),
                lastDragToIndex,
                PREVIEW_TITLE_FIELD_NAME = 'previewTitle',
                PREVIEW_DESCRIPTION_FIELD_NAME = 'previewDescription',
                PREVIEW_IMAGE_FIELD_NAME = 'previewImage';

            init();
            return baseConfig.link(scope, el, attributes, controllers);

            /* INIT */

            function init() {

                var allModelTypes = getModelTypesAsArray();

                vm.add = add;
                vm.edit = edit;
                vm.remove = remove;
                vm.onDrop = onDrop;
                vm.onDropSuccess = onDropSuccess;
                vm.getTitle = getTitle;

                nestedDataModelSchemaService
                    .getByNames(allModelTypes)
                    .then(loadModelMetaData);

                initDisplayFields();
            }

            function triggerModelChange() {
                // Because the model is an array, angular can't track the changes
                // which prevents validation from being updated
                vm.model = vm.model.slice(0);
            }

            function getModelTypesAsArray() {
                if (!vm.modelTypes) return [];
                return vm.modelTypes.split(',');
            }

            function loadModelMetaData(modelMetaDataCollection) {
                var hasCustomTitleField = false,
                    hasNonTitleField = false;

                vm.modelMetaDataLookup = {};
                vm.previewFields = {};

                _.chain(modelMetaDataCollection)
                    .sortBy('displayName')
                    .each(function (modelMetaData) {
                        vm.modelMetaDataLookup[modelMetaData.typeName] = modelMetaData;

                        var previewData = new ModelPreviewFieldset(modelMetaData);

                        if (previewData.fields[PREVIEW_TITLE_FIELD_NAME]) {
                            hasCustomTitleField = true;
                        }

                        if (previewData.fields[PREVIEW_DESCRIPTION_FIELD_NAME]) {
                            vm.previewFields.showDescription = true;
                            hasNonTitleField = true;
                        }

                        if (previewData.fields[PREVIEW_IMAGE_FIELD_NAME]) {
                            vm.previewFields.showImage = true;
                            hasNonTitleField = true;
                        }

                        vm.previewFields[modelMetaData.typeName] = previewData;
                });

                if (hasCustomTitleField || !hasNonTitleField) {
                    vm.previewFields.showTitle = true;
                }

                vm.gridImages = new ImagePreviewFieldCollection('typeName');
                vm.gridImages.load(vm.model, vm.previewFields);
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

            function remove(nestedModel, $index) {

                arrayUtilities.removeObject(vm.model, nestedModel);
                vm.gridImages.remove($index);
            }

            function edit(item, $index) {

                showEditDialog({
                    model: item.model,
                    onSave: onSave,
                    modelMetaData: vm.modelMetaDataLookup[item.typeName]
                });

                function onSave() {
                    vm.gridImages.update(item, $index);
                    triggerModelChange();
                }
            }

            function add(modelMetaData) {

                showEditDialog({
                    onSave: onSave,
                    modelMetaData: modelMetaData
                });

                function onSave(dataModel) {
                    vm.model = vm.model || [];

                    var newItem = {
                        model: dataModel,
                        typeName: modelMetaData.typeName
                    };

                    vm.model.push(newItem);

                    vm.gridImages.add(newItem, vm.model.length - 1, true);
                    triggerModelChange();
                }
            }

            function showEditDialog(options) {

                if (dynamicFormFieldController) {
                    options.additionalParameters = dynamicFormFieldController.additionalParameters;
                }

                modalDialogService.show({
                    templateUrl: modulePath + 'UIComponents/NestedDataModels/EditNestedDataModelDialog.html',
                    controller: 'EditNestedDataModelDialogController',
                    options: options
                });
            }

            function onDrop($index, droppedEntity) {

                // drag drop doesnt give us the to/from index data in the same event, and 
                // we can't use property tracking here, so stuff the index in a variable
                lastDragToIndex = $index;
            }

            function onDropSuccess($index) {
                arrayUtilities.move(vm.model, $index, lastDragToIndex);
                vm.gridImages.move($index, lastDragToIndex);
            }

            /* FORMATTERS */

            function getTitle(item, index) {
                var field = vm.previewFields[item.typeName].fields[PREVIEW_TITLE_FIELD_NAME];

                if (field) {
                    return item.model[field.lowerName];
                }

                if (item.model.title) return item.model.title;

                return 'Item ' + (index + 1);
            }

        }
    }]);