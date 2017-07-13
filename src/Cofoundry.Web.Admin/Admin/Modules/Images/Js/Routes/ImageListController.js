angular.module('cms.images').controller('ImageListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.permissionValidationService',
    'images.imageService',
function (
    _,
    LoadState,
    SearchQuery,
    permissionValidationService,
    imageService) {

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

        vm.canCreate = permissionValidationService.canCreate('COFIMG');
        vm.canUpdate = permissionValidationService.canUpdate('COFIMG');

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

        return imageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
}]);