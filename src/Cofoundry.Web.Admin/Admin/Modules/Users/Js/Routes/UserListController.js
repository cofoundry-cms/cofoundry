angular.module('cms.users').controller('UserListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'users.userService',
    'users.options',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    permissionValidationService,
    userService,
    options) {

    var vm = this;

    init();

    function init() {
        
        vm.userArea = options;
        vm.urlLibrary = urlLibrary;
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        var entityDefinitionCode = options.userAreaCode === 'COF' ? 'COFUSR' : 'COFUSN';
        vm.canRead = permissionValidationService.canRead(entityDefinitionCode);
        vm.canUpdate = permissionValidationService.canUpdate(entityDefinitionCode);
        vm.canCreate = permissionValidationService.canCreate(entityDefinitionCode);

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

        return userService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);