angular.module('cms.shared').controller('DocumentAssetPickerDialogController', [
        '$scope',
        'shared.LoadState',
        'shared.documentService',
        'shared.SearchQuery',
        'shared.urlLibrary',
        'options',
        'close',
    function (
        $scope,
        LoadState,
        documentService,
        SearchQuery,
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
        vm.selectedAsset = vm.currentAsset;
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

        vm.isDocumentSelected = isDocumentSelected;
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
        vm.onSelected(vm.currentAsset);
        close();
    }

    function onSelect(document) {
        if (!isDocumentSelected(document)) {
            vm.selectedAsset = document;
        }
    }

    function onSelectAndClose(document) {
        vm.selectedAsset = document;
        onOk();
    }

    function onOk() {
        vm.onSelected(vm.selectedAsset);
        close();
    }

    /* PUBLIC HELPERS */

    function isDocumentSelected(document) {
        if (!document || !vm.selectedAsset) return false;

        return document.documentAssetId === vm.selectedAsset.documentAssetId;
    }
}]);
