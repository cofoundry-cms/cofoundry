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
        $scope.$watch("command.file", setFileName);

        setFilter(options.filter);

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

    function setFileName() {
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

        if (vm.filter.tags) {
            vm.command.tags = vm.filter.tags.split(',');
        }
    }

    function setFilter(filter) {
        var parts = [];

        if (filter) {
            addSize(filter.width, filter.height);
            addSize(filter.minWidth, filter.minHeight, 'min-');
        }

        vm.filterText = parts.join(', ');
        vm.isFilterSet = parts.length > 0;

        function addSize(width, height, prefix) {
            if (width && height) {
                parts.push(prefix + 'size ' + width + 'x' + height);
            }
            else {
                addIfSet(prefix + 'width', width);
                addIfSet(prefix + 'height', height);
            }
        }

        function addIfSet(name, value) {
            if (value) {
                parts.push(name + ' ' + value);
            }
        }
    }

    function cancel() {
        close();
    }

    function uploadComplete(imageAssetId) {
        options.onUploadComplete(imageAssetId);
        close();
    }

}]);
