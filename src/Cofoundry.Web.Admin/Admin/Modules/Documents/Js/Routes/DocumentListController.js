angular.module('cms.documents').controller('DocumentListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'documents.documentService',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    permissionValidationService,
    documentService) {

    /* START */

    var vm = this;
    init();
    
    /* INIT */
    function init() {

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();

        vm.toggleFilter = toggleFilter;
        vm.getDocumentUrl = urlLibrary.getDocumentUrl;

        vm.canCreate = permissionValidationService.canCreate('COFDOC');
        vm.canUpdate = permissionValidationService.canUpdate('COFDOC');

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */
    
    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* HELPERS */

    function loadGrid() {
        vm.gridLoadState.on();

        return documentService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
}]);