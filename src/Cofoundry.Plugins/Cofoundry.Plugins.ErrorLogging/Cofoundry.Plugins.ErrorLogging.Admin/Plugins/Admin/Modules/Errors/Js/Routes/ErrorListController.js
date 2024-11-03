angular.module('cms.errors').controller('ErrorListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'errors.errorService',
function (
    _,
    LoadState,
    SearchQuery,
    errorService) {

    var vm = this;

    init();

    function init() {
        
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

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

    /* PRIVATE FUNCS */
    
    function loadGrid() {
        vm.gridLoadState.on();

        return errorService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);