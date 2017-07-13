angular.module('cms.directories').controller('DirectoryDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'directories.directoryService',
    'directories.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    stringUtilities,
    LoadState,
    modalDialogService,
    permissionValidationService,
    directoryService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.deleteDirectory = deleteDirectory;

        // Events
        vm.onNameChanged = onNameChanged;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.canUpdate = permissionValidationService.canUpdate('COFDIR');
        vm.canDelete = permissionValidationService.canDelete('COFDIR');

        // Init
        initData()
            .then(setLoadingOff.bind(null, vm.formLoadState));
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
        vm.command = mapUpdateCommand(vm.webDirectory);
        vm.mainForm.formStatus.clear();
    }

    function deleteDirectory() {
        var options = {
            title: 'Delete Directory',
            message: 'Are you sure you want to delete this directory?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return directoryService
                .remove(vm.webDirectory.webDirectoryId)
                .then(redirectToList)
                .catch(setLoadingOff);
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

    function initData() {
        var webDirectoryId = $routeParams.id;

        return directoryService.getTree()
            .then(loadDirectory);

        function loadDirectory(tree) {
            var webDirectory = tree.findNodeById(webDirectoryId),
                parentDirectories = tree.flatten(webDirectoryId);

            vm.webDirectory = webDirectory;
            vm.parentDirectories = parentDirectories;
            vm.command = mapUpdateCommand(webDirectory);
            vm.editMode = false;
            vm.hasChildContent = webDirectory.numPages || webDirectory.childWebDirectories.length;
        }
    }

    function mapUpdateCommand(webDirectory) {

        return _.pick(webDirectory,
            'webDirectoryId',
            'name',
            'urlPath',
            'parentWebDirectoryId'
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