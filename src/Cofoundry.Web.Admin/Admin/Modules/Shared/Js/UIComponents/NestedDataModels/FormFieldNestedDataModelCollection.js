angular.module('cms.shared').directive('cmsFormFieldNestedDataModelCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.nestedDataModelSchemaService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'baseFormFieldFactory',
    function (
        _,
        modulePath,
        LoadState,
        nestedDataModelSchemaService,
        modalDialogService,
        arrayUtilities,
        baseFormFieldFactory) {

        /* VARS */

        var baseConfig = baseFormFieldFactory.defaultConfig;

        /* CONFIG */

        var config = {
            templateUrl: modulePath + 'UIComponents/NestedDataModels/FormFieldNestedDataModelCollection.html',
            scope: _.extend(baseConfig.scope, {
                minItems: '@cmsMinItems',
                maxItems: '@cmsMaxItems',
                orderable: '=cmsOrderable',
                modelType: '@cmsModelType'
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
                definitionPromise;

            init();
            return baseConfig.link(scope, el, attributes, controllers);

            /* INIT */

            function init() {

                vm.add = add;
                vm.edit = edit;
                vm.remove = remove;
                vm.onDrop = onDrop;

                definitionPromise = nestedDataModelSchemaService
                    .getByName(vm.modelType)
                    .then(function (modelMetaData) {

                    vm.modelMetaData = modelMetaData;
                });
            }

            /* EVENTS */

            function remove(nestedModel) {

                arrayUtilities.removeObject(vm.model, nestedModel);
            }

            function edit(model) {

                showEditDialog({
                    model: model
                });
            }

            function add() {

                showEditDialog({
                    onSave: onSave
                });

                function onSave(newEntity) {
                    vm.model = vm.model || [];
                    vm.model.push(newEntity);
                }
            }

            function showEditDialog(options) {

                options.modelMetaData = vm.modelMetaData;

                modalDialogService.show({
                    templateUrl: modulePath + 'UIComponents/NestedDataModels/EditNestedDataModelDialog.html',
                    controller: 'EditNestedDataModelDialogController',
                    options: options
                });
            }

            function onDrop($index, droppedEntity) {

                arrayUtilities.moveObject(vm.model, droppedEntity, $index);
            }
        }
    }]);