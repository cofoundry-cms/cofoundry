angular.module('cms.customEntities').controller('AddCustomEntityController', [
    '$scope',
    '$location',
    '$q',
    '$window',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.urlLibrary',
    'customEntities.options',
function (
    $scope,
    $location,
    $q,
    $window,
    stringUtilities,
    LoadState,
    customEntityService,
    urlLibrary,
    moduleOptions) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();

        vm.formLoadState = new LoadState(true);
        vm.editMode = false;
        vm.options = moduleOptions;
        vm.saveButtonText = moduleOptions.autoPublish ? 'Save' : 'Save & Publish';

        vm.save = save.bind(null, false, redirectToDetails);
        vm.saveAndPublish = save.bind(null, true, redirectToDetails);
        vm.saveAndEdit = save.bind(null, true, redirectToVisualEditor);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;

        initData();
    }

    /* EVENTS */

    function save(publish, redirectToCommand) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        customEntityService
            .add(vm.command, moduleOptions.customEntityDefinitionCode)
            .then(redirectToCommand)
            .finally(setLoadingOff.bind(null, loadState));
    }

    function onNameChanged() {
        vm.command.urlSlug = stringUtilities.slugify(vm.command.title);
    }
    
    /* PRIVATE FUNCS */
    
    function cancel() {
        $location.path('/');
    }

    function redirectToDetails(id) {
        $location.path('/' + id);
    }

    function redirectToVisualEditor(id) {

        return customEntityService
            .getById(id)
            .then(redirect);

        function redirect(customEntity) {
            $window.location.href = urlLibrary.customEntityVisualEditor(customEntity, true);
        }
    }

    function initData() {

        var schemaDefferred = customEntityService
            .getDataModelSchema(moduleOptions.customEntityDefinitionCode)
            .then(loadModelSchema);

        var pageRoutesDefferred = customEntityService
            .getPageRoutes(moduleOptions.customEntityDefinitionCode)
            .then(loadPageRoutes);

        vm.formLoadState.offWhen(schemaDefferred, pageRoutesDefferred);

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
            };
        }

        function loadPageRoutes(pageRoutes) {
            vm.pageRoutes = pageRoutes;
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