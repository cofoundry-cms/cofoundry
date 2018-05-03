angular.module('cms.shared').controller('DocumentAssetPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.documentService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'shared.permissionValidationService',
    'shared.urlLibrary',
    'options',
    'close',
function (
    $scope,
    LoadState,
    documentService,
    SearchQuery,
    modalDialogService,
    modulePath,
    permissionValidationService,
    urlLibrary,
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
            defaultParams: vm.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        vm.isSelected = isSelected;
        vm.multiMode = vm.selectedIds ? true : false;
        vm.okText = vm.multiMode ? 'Ok' : 'Select';

        vm.canCreate = permissionValidationService.canCreate('COFDOC');

        vm.getDocumentUrl = urlLibrary.getDocumentUrl;

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

        return documentService.getAll(vm.query.getParameters()).then(function (result) {
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

    function onSelect(document) {
        if (!vm.multiMode) {
            vm.selectedAsset = document;
            return;
        }

        addOrRemove(document);
    }

    function onSelectAndClose(document) {
        if (!vm.multiMode) {
            vm.selectedAsset = document;
            onOk();
            return;
        }

        addOrRemove(document);
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
            templateUrl: modulePath + 'UIComponents/DocumentAssets/UploadDocumentAssetDialog.html',
            controller: 'UploadDocumentAssetDialogController',
            options: {
                filter: options.filter,
                onUploadComplete: onUploadComplete
            }
        });

        function onUploadComplete(documentAssetId) {
            onSelectAndClose({ documentAssetId: documentAssetId });
        }
    }

    /* PUBLIC HELPERS */

    function isSelected(document) {
        if (vm.selectedIds && document && vm.selectedIds.indexOf(document.documentAssetId) > -1) return true;

        if (!document || !vm.selectedAsset) return false;

        return document.documentAssetId === vm.selectedAsset.documentAssetId;
    }

    function addOrRemove(document) {
        if (!isSelected(document)) {
            vm.selectedIds.push(document.documentAssetId);
        } else {
            var index = vm.selectedIds.indexOf(document.documentAssetId);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);
