angular.module('cms.shared').directive('cmsFormFieldNestedDataModelCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.nestedDataModelSchemaService',
    'shared.modalDialogService',
    'shared.imageService',
    'shared.arrayUtilities',
    'shared.stringUtilities',
    'baseFormFieldFactory',
    function (
        _,
        modulePath,
        LoadState,
        nestedDataModelSchemaService,
        modalDialogService,
        imageService,
        arrayUtilities,
        stringUtilities,
        baseFormFieldFactory) {

        /* VARS */

        var PREVIEW_TITLE_FIELD_NAME = 'previewTitle',
            PREVIEW_IMAGE_FIELD_NAME = 'previewImage',
            baseConfig = baseFormFieldFactory.defaultConfig;

        /* CONFIG */

        var config = {
            templateUrl: modulePath + 'UIComponents/NestedDataModels/FormFieldNestedDataModelCollection.html',
            scope: _.extend(baseConfig.scope, {
                minItems: '@cmsMinItems',
                maxItems: '@cmsMaxItems',
                modelType: '@cmsModelType',
                orderable: '=cmsOrderable'
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
                definitionPromise,
                dynamicFormFieldController = _.last(controllers),
                lastDragToIndex;

            init();
            return baseConfig.link(scope, el, attributes, controllers);

            /* INIT */

            function init() {

                vm.add = add;
                vm.edit = edit;
                vm.remove = remove;
                vm.onDrop = onDrop;
                vm.onDropSuccess = onDropSuccess;
                vm.getTitle = getTitle;

                definitionPromise = nestedDataModelSchemaService
                    .getByName(vm.modelType)
                    .then(function (modelMetaData) {
                        var gridFields = {};

                        setGridField(gridFields, modelMetaData.dataModelProperties, PREVIEW_TITLE_FIELD_NAME);
                        setGridField(gridFields, modelMetaData.dataModelProperties, 'previewDescription');
                        setGridField(gridFields, modelMetaData.dataModelProperties, PREVIEW_IMAGE_FIELD_NAME);
                        vm.showTitleColumn = gridFields[PREVIEW_TITLE_FIELD_NAME] || !gridFields.hasFields;
                        vm.gridFields = gridFields;
                        vm.modelMetaData = modelMetaData;

                        if (gridFields[PREVIEW_TITLE_FIELD_NAME]) {
                            vm.gridTitleTerm = gridFields[PREVIEW_TITLE_FIELD_NAME].displayName;
                        } else {
                            vm.gridTitleTerm = "Title";
                        }

                        loadImageFields();
                    });

                function setGridField(gridFields, dataModelProperties, fieldName) {

                    var field = _.find(dataModelProperties, function (property) {

                        return property.additionalAttributes[fieldName];
                    });

                    if (field) {
                        field.lowerName = stringUtilities.lowerCaseFirstLetter(field.name);
                        gridFields[fieldName] = field;
                        gridFields.hasFields = true;
                    }
                }
            }

            function updateImageField(itemToUpdate, index, isNew) {
                var field = vm.gridFields[PREVIEW_IMAGE_FIELD_NAME];
                if (!field) return;

                var newImageId = itemToUpdate[field.lowerName];

                if (!isNew) {
                    var existingImage = vm.modelImages[index],
                        existingId;

                    if (existingImage) {
                        existingId = existingImage['imageAssetId'];
                    }

                    if (newImageId == existingId) return;

                    if (!newImageId) {
                        vm.modelImages[index] = undefined;
                        return;
                    }
                }

                imageService
                    .getById(newImageId)
                    .then(loadImage);

                function loadImage (image) {
                    vm.modelImages[index] = image;
                }
            }

            function loadImageFields() {
                if (!vm.model || !vm.gridFields || !vm.gridFields[PREVIEW_IMAGE_FIELD_NAME]) return;

                var field = vm.gridFields[PREVIEW_IMAGE_FIELD_NAME];

                var allImageIds = _.chain(vm.model)
                    .map(function (model) {
                        return model[field.lowerName];
                    })
                    .filter(function (id) {
                        return id;
                    })
                    .uniq()
                    .value();

                imageService.getByIdRange(allImageIds).then(function (images) {
                    vm.modelImages = [];

                    _.each(vm.model, function (item) {
                        var id = item[field.lowerName],
                            image;

                        if (id) {
                            image = _.find(images, { imageAssetId: id })
                        }

                        vm.modelImages.push(image);
                    });
                });
            }

            /* EVENTS */

            function remove(nestedModel, $index) {

                arrayUtilities.removeObject(vm.model, nestedModel);
                arrayUtilities.remove(vm.modelImages, $index);
            }

            function edit(model, $index) {

                showEditDialog({
                    model: model,
                    onSave: onSave
                });

                function onSave() {
                    updateImageField(model, $index);
                }
            }

            function add() {

                showEditDialog({
                    onSave: onSave
                });

                function onSave(newEntity) {
                    vm.model = vm.model || [];
                    vm.model.push(newEntity);

                    updateImageField(newEntity, vm.model.length -1, true);
                }
            }

            function showEditDialog(options) {

                options.modelMetaData = vm.modelMetaData;

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
                arrayUtilities.move(vm.modelImages, $index, lastDragToIndex);
            }

            /* FORMATTERS */

            function getTitle(entity, index) {
                var field = vm.gridFields[PREVIEW_TITLE_FIELD_NAME];
                if (field) {
                    return entity[field.lowerName];
                }

                if (entity.title) return entity.title;

                return 'Item ' + (index + 1);
            }

        }
    }]);