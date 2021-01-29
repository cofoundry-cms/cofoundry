angular.module('cms.shared').controller('AddCustomEntityDialogController', [
    '$scope',
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.customEntityService',
    'options',
    'close',
function (
    $scope,
    $location,
    stringUtilities,
    LoadState,
    customEntityService,
    options,
    close) {

    var vm = $scope;

    init();

    /* INIT */

    function init() {
        angular.extend($scope, options.customEntityDefinition);

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();

        vm.formLoadState = new LoadState(true);
        vm.editMode = false;
        vm.options = options.customEntityDefinition;
        vm.saveButtonText = options.customEntityDefinition.autoPublish ? 'Save' : 'Save & Publish';

        vm.save = save.bind(null, false);
        vm.saveAndPublish = save.bind(null, true);
        vm.cancel = onCancel;
        vm.close = onCancel;
        vm.onNameChanged = onNameChanged;

        initData();
    }

    /* EVENTS */

    function save(publish) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        customEntityService
            .add(vm.command, options.customEntityDefinition.customEntityDefinitionCode)
            .then(complete)
            .finally(setLoadingOff.bind(null, loadState))
        ;
    }

    function onNameChanged() {
        vm.command.urlSlug = stringUtilities.slugify(vm.command.title);
    }

    function onCancel() {
        close();
    }

    function complete(entityId) {
        options.onComplete(entityId);
        close();
    }

    /* PRIVATE FUNCS */

    function initData() {
        customEntityService.getDataModelSchema(options.customEntityDefinition.customEntityDefinitionCode).then(loadModelSchema);
        vm.command = {};

        $scope.$watch('vm.command.localeId', function (localeId) {

            if (localeId) {
                vm.additionalParameters = {
                    localeId: localeId
                };
            } else {
                vm.additionalParameters = {};
            }
        });

        function loadModelSchema(modelMetaData) {
            if (modelMetaData.defaultValue && modelMetaData.defaultValue.value) {
                vm.command.model = angular.copy(modelMetaData.defaultValue.value);
            } else {
                vm.command.model = {};
            }

            vm.formDataSource = {
                model: vm.command.model,
                modelMetaData: modelMetaData
            }

            vm.formLoadState.off();
        }
    }
    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
        if (loadState && _.isFunction(loadState.on)) loadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
        if (loadState && _.isFunction(loadState.off)) loadState.off();
    }
}]);