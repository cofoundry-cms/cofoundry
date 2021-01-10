angular.module('cms.shared').directive('cmsFormFieldNestedDataModelCollection', [
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
                        vm.modelMetaData = modelMetaData;
                        vm.previewFields = new ModelPreviewFieldset(modelMetaData);

                        vm.gridImages = new ImagePreviewFieldCollection();

                        vm.gridImages.load(vm.model, vm.previewFields);
                    });

            }

            function triggerModelChange() {
                // Because the model is an array, angular can't track the changes
                // which prevents validation from being updated
                vm.model = vm.model.slice(0);
            }

            /* EVENTS */

            function remove(nestedModel, $index) {

                arrayUtilities.removeObject(vm.model, nestedModel);
                vm.gridImages.remove($index);
            }

            function edit(model, $index) {

                showEditDialog({
                    model: model,
                    onSave: onSave
                });

                function onSave() {
                    vm.gridImages.update(model, $index);
                    triggerModelChange();
                }
            }

            function add() {

                showEditDialog({
                    onSave: onSave
                });

                function onSave(newEntity, test) {
                    vm.model = vm.model || [];
                    vm.model.push(newEntity);

                    vm.gridImages.add(newEntity, vm.model.length - 1);
                    triggerModelChange();
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
                vm.gridImages.move($index, lastDragToIndex);
            }

            /* FORMATTERS */

            function getTitle(entity, index) {
                var field = vm.previewFields.fields[PREVIEW_TITLE_FIELD_NAME];
                if (field) {
                    return entity[field.lowerName];
                }

                if (entity.title) return entity.title;

                return 'Item ' + (index + 1);
            }

        }
    }]);