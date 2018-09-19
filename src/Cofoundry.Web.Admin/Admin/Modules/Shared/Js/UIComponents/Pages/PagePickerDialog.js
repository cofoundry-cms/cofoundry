angular.module('cms.shared').controller('PagePickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.pageService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'shared.urlLibrary',
    'options',
    'close',
function (
    $scope,
    LoadState,
    pageService,
    SearchQuery,
    modalDialogService,
    modulePath,
    urlLibrary,
    options,
    close) {

    /* VARS */

    var vm = $scope,
        PAGE_ID_PROP = 'pageId';

    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.selectedPage = vm.currentPage; // current page is null in single mode
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
        vm.urlLibrary = urlLibrary;

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

        return pageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the page
            vm.onSelected(vm.currentPage);
        }
        close();
    }

    function onSelect(page) {
        if (!vm.multiMode) {
            vm.selectedPage = page;
            return;
        }

        addOrRemove(page);
    }

    function onSelectAndClose(page) {
        if (!vm.multiMode) {
            vm.selectedPage = page;
            onOk();
            return;
        }

        addOrRemove(page);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedPage);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    /* PUBLIC HELPERS */

    function isSelected(page) {
        if (vm.selectedIds && page && vm.selectedIds.indexOf(page.pageId) > -1) return true;

        if (!page || !vm.selectedPage) return false;
        
        return page[PAGE_ID_PROP] === vm.selectedPage[PAGE_ID_PROP];
    }

    function addOrRemove(page) {
        if (!isSelected(page)) {
            vm.selectedIds.push(page[PAGE_ID_PROP]);
        } else {
            var index = vm.selectedIds.indexOf(page[PAGE_ID_PROP]);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);
