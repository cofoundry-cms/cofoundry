angular.module('cms.documents').controller('AddDocumentController', [
            '$location',
            '_',
            'shared.focusService',
            'shared.stringUtilities',
            'shared.LoadState',
            'documents.documentService',
        function (
            $location,
            _,
            focusService,
            stringUtilities,
            LoadState,
            documentService
        ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initData();

        vm.save = save;
        vm.cancel = cancel;
        vm.onFileChanged = onFileChanged;

        vm.editMode = false;
        vm.saveLoadState = new LoadState();
    }

    /* EVENTS */

    function save() {
        vm.saveLoadState.on();

        documentService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(redirectToList)
            .finally(vm.saveLoadState.off);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            command.fileName = stringUtilities.slugify(command.title);
            focusService.focusById('title');
        }
    }

    /* PRIVATE FUNCS */

    function initData() {
        vm.command = {};
    }

    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('');
    }
}]);