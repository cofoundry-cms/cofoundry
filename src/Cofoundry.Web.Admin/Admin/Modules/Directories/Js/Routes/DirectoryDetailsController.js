angular.module('cms.directories').controller('DirectoryDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'shared.userAreaService',
    'shared.internalModulePath',
    'directories.directoryService',
function (
    $routeParams,
    $q,
    $location,
    _,
    stringUtilities,
    LoadState,
    modalDialogService,
    permissionValidationService,
    userAreaService,
    sharedModulePath,
    directoryService
    ) {

    var vm = this,
        ENTITY_DEFINITION_CODE = 'COFDIR';

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.deleteDirectory = deleteDirectory;
        vm.viewAccessRules = viewAccessRules;

        // Events
        vm.onNameChanged = onNameChanged;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.canUpdate = permissionValidationService.canUpdate(ENTITY_DEFINITION_CODE);
        vm.canDelete = permissionValidationService.canDelete(ENTITY_DEFINITION_CODE);

        // Init
        initData(vm.formLoadState);
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        directoryService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand(vm.pageDirectory);
        vm.mainForm.formStatus.clear();
    }

    function deleteDirectory() {
        var options = {
            title: 'Delete Directory',
            message: 'Deleting this directory will delete ALL sub-directories and pages linked to this directory. Are you sure you want to continue?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return directoryService
                .remove(vm.pageDirectory.pageDirectoryId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    function viewAccessRules() {

        modalDialogService.show({
            templateUrl: sharedModulePath + 'UIComponents/EntityAccess/EntityAccessEditor.html',
            controller: 'EntityAccessEditorController',
            options: {
                entityDefinitionCode: ENTITY_DEFINITION_CODE,
                entityIdPrefix: 'pageDirectory',
                entityDefinitionName: 'Directory',
                entityDescription: vm.pageDirectory.fullUrlPath,
                entityAccessLoader: directoryAccessLoader,
                saveAccess: directoryService.updateAccessRules
            }
        });

        function directoryAccessLoader() {
            return directoryService
                .getAccessRulesByPageDirectoryId(vm.pageDirectory.pageDirectoryId);
        }
    }

    /* EVENTS */

    function onNameChanged() {
        if (!vm.hasChildContent) {
            vm.command.urlPath = stringUtilities.slugify(vm.command.name);
        }
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData(loadStateToTurnOff) {
        var pageDirectoryId = $routeParams.id;

        return $q
            .all([getDirectory(), getUserAreas()])
            .then(setLoadingOff.bind(null, loadStateToTurnOff));

        function getDirectory() {
            return directoryService
                .getTree()
                .then(function loadDirectory(tree) {
                    var pageDirectory = tree.findNodeById(pageDirectoryId),
                        parentDirectories = tree.flatten(pageDirectoryId);
        
                    vm.pageDirectory = pageDirectory;
                    vm.parentDirectories = parentDirectories;
                    vm.command = mapUpdateCommand(pageDirectory);
                    vm.editMode = false;
                    vm.hasChildContent = pageDirectory.numPages || pageDirectory.childPageDirectories.length;
                });
        }

        function getUserAreas() {
            return userAreaService.getAll().then(function (userAreas) {
                vm.accessRulesEnabled = userAreas.length > 1;
            });
        }
    }

    function mapUpdateCommand(pageDirectory) {

        return _.pick(pageDirectory,
            'pageDirectoryId',
            'name',
            'urlPath',
            'parentPageDirectoryId'
            );
    }

    function redirectToList() {
        $location.path('');
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