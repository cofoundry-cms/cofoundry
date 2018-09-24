angular.module('cms.shared').controller('ImageAssetPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'shared.permissionValidationService',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
    SearchQuery,
    modalDialogService,
    modulePath,
    permissionValidationService,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.onUpload = onUpload;
        vm.selectedAsset = vm.currentAsset; // currentAsset is null in single mode
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: options.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        vm.isSelected = isSelected;
        vm.multiMode = vm.selectedIds ? true : false;
        vm.okText = vm.multiMode ? 'Ok' : 'Select';

        vm.canCreate = permissionValidationService.canCreate('COFIMG');

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return imageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the currentAsset
            vm.onSelected(vm.currentAsset);
        }
        close();
    }

    function onSelect(image) {
        if (!vm.multiMode) {
            vm.selectedAsset = image;
            return;
        }

        addOrRemove(image);
    }

    function onSelectAndClose(image) {
        if (!vm.multiMode) {
            vm.selectedAsset = image;
            onOk();
            return;
        }

        addOrRemove(image);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedAsset);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    function onUpload() {
        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/ImageAssets/UploadImageAssetDialog.html',
            controller: 'UploadImageAssetDialogController',
            options: {
                filter: options.filter,
                onUploadComplete: onUploadComplete
            }
        });

        function onUploadComplete(imageAssetId) {

            imageService
                .getById(imageAssetId)
                .then(onSelectAndClose);
        }
    }

    /* PUBLIC HELPERS */

    function isSelected(image) {
        if (vm.selectedIds && image && vm.selectedIds.indexOf(image.imageAssetId) > -1) return true;

        if (!image || !vm.selectedAsset) return false;

        return image.imageAssetId === vm.selectedAsset.imageAssetId;
    }

    function addOrRemove(image) {
        if (!isSelected(image)) {
            vm.selectedIds.push(image.imageAssetId);
        } else {
            var index = vm.selectedIds.indexOf(image.imageAssetId);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);
