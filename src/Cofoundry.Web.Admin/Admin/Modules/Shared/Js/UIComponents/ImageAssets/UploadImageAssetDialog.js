angular.module('cms.shared').controller('UploadImageAssetDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.focusService',
    'shared.stringUtilities',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
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

        imageService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(uploadComplete);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
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
        return options.filter.minWidth ||
            options.filter.minHeight ||
            options.filter.width ||
            options.filter.height;
    }

    function cancel() {
        close();
    }

    function uploadComplete(imageAssetId) {
        options.onUploadComplete(imageAssetId);
        close();
    }

}]);
