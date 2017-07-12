angular.module('cms.customEntities').controller('CustomEntityListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.customEntityService',
    'shared.permissionValidationService',
    'customEntities.modulePath',
    'customEntities.options',
function (
    _,
    LoadState,
    SearchQuery,
    modalDialogService,
    customEntityService,
    permissionValidationService,
    modulePath,
    options) {

    var vm = this;

    init();

    function init() {
        
        vm.options = options;
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.changeOrdering = changeOrdering;

        vm.permissions = permissionValidationService;

        toggleFilter(false);

        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function changeOrdering() {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/ChangeOrdering.html',
            controller: 'ChangeOrderingController',
            options: {
                localeId: vm.filter.localeId,
                onSave: loadGrid
            }
        });
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* PRIVATE FUNCS */
    
    function loadGrid() {
        vm.gridLoadState.on();

        return customEntityService.getAll(vm.query.getParameters(), options.customEntityDefinitionCode).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);