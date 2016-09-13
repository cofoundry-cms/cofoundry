angular.module('cms.customEntities').controller('AddCustomEntityController', [
    '$scope',
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'customEntities.customEntityService',
    'customEntities.options',
function (
    $scope,
    $location,
    stringUtilities,
    LoadState,
    customEntityService,
    moduleOptions) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initData();

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();

        vm.formLoadState = new LoadState(true);
        vm.editMode = false;
        vm.options = moduleOptions;
        vm.saveButtonText = moduleOptions.autoPublish ? 'Save' : 'Save & Publish';

        vm.save = save.bind(null, false);
        vm.saveAndPublish = save.bind(null, true);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;
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
            .add(vm.command)
            .then(redirectToList)
            .finally(setLoadingOff.bind(null, loadState));
    }

    function onNameChanged() {
        vm.command.urlSlug = stringUtilities.slugify(vm.command.title);
    }


    /* PRIVATE FUNCS */
    
    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('/');
    }

    function initData() {

        customEntityService.getDataModelSchema().then(loadModelSchema);
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
            vm.command.model = {};

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