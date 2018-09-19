angular.module('cms.pageTemplates').controller('PageTemplateListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'pageTemplates.pageTemplateService',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    pageTemplateService
) {

    var vm = this;

    init();

    function init() {

        vm.urlLibrary = urlLibrary;
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

        return pageTemplateService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);