angular.module('cms.shared').controller('UploadDocumentAssetDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.documentService',
    'shared.SearchQuery',
    'shared.focusService',
    'shared.stringUtilities',
    'options',
    'close',
function (
    $scope,
    LoadState,
    documentService,
    SearchQuery,
    focusService,
    stringUtilities,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    function init() {
        angular.extend($scope, options);

        initData();

        vm.onUpload = onUpload;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.filter = options.filter;
        vm.onFileChanged = onFileChanged;
        vm.hasFilterRestrictions = hasFilterRestrictions;

        vm.saveLoadState = new LoadState();
    }

    /* EVENTS */
    function onUpload() {
        vm.saveLoadState.on();

        documentService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(uploadComplete);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            command.fileName = stringUtilities.slugify(command.title);
            focusService.focusById('title');
        }
    }

    function onCancel() {
        close();
    }

    /* PUBLIC HELPERS */
    function initData() {
        vm.command = {};
    }

    function hasFilterRestrictions() {
        return options.filter.fileExtension ||
            options.filter.fileExtensions;
    }

    function cancel() {
        close();
    }

    function uploadComplete(documentAssetId) {
        options.onUploadComplete(documentAssetId);
        close();
    }

}]);
