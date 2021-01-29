angular.module('cms.shared').controller('EditNestedDataModelDialogController', [
    '$scope',
    'shared.nestedDataModelSchemaService',
    'shared.LoadState',
    'options',
    'close',
function (
    $scope,
    nestedDataModelSchemaService,
    LoadState,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    function init() {
        angular.extend($scope, options);

        vm.save = onSave;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.isNew = !options.model;
        vm.title = vm.isNew ? 'Add Item' : 'Edit Item';
        vm.saveLoadState = new LoadState();
        initFormDataSource(options);
    }

    /* EVENTS */

    function initFormDataSource(options) {
        var model = options.model || {},
            modelMetaData = options.modelMetaData;

        if (vm.isNew && modelMetaData.defaultValue && modelMetaData.defaultValue.value) {
            model = angular.copy(modelMetaData.defaultValue.value);
        }

        vm.formDataSource = {
            model: model,
            modelMetaData: modelMetaData
        };
    }

    function onSave() {
        vm.saveLoadState.on();

        nestedDataModelSchemaService
            .validate(vm.formDataSource.modelMetaData.typeName, vm.formDataSource.model)
            .then(onValidationSuccess)
            .finally(vm.saveLoadState.off);

        function onValidationSuccess() {
            if (options.onSave) options.onSave(vm.formDataSource.model);
            close();
        }
    }

    function onCancel() {
        close();
    }

    /* PUBLIC HELPERS */

    function initData() {
        vm.command = {};
    }

    function cancel() {
        close();
    }
}]);
