angular.module('cms.directories').controller('ChangeDirectoryUrlController', [
    '$scope',
    '$q',
    'shared.LoadState',
    'directories.directoryService',
    'options',
    'close',
function (
    $scope,
    $q,
    LoadState,
    directoryService,
    options,
    close) {

    var vm = $scope;

    init();
    
    /* INIT */

    function init() {

        initData();

        vm.submitLoadState = new LoadState();

        vm.save = save;
        vm.close = close;
    }

    function initData() {
        var pageDirectory = options.pageDirectory;

        vm.pageDirectory = pageDirectory;
        vm.selectableParentDirectories = options.selectableParentDirectories;
        vm.hasChildContent = options.hasChildContent;
        vm.command = {
            pageDirectoryId: pageDirectory.pageDirectoryId,
            urlPath: pageDirectory.urlPath,
            parentPageDirectoryId: pageDirectory.parentPageDirectoryId
        };
    }

    /* EVENTS */

    function save() {
        vm.submitLoadState.on();

        directoryService
            .updateUrl(vm.command)
            .then(options.onSave)
            .then(close)
            .finally(vm.submitLoadState.off);
    }
}]);