angular
    .module('cms.errors', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('errors.modulePath', '/Plugins/Admin/Modules/Errors/Js/');
angular.module('cms.errors').config([
    '$routeProvider',
    'shared.routingUtilities',
    'errors.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .when('/:id', routingUtilities.mapOptions(modulePath, 'ErrorDetails'))
        .otherwise(routingUtilities.mapOptions(modulePath, 'ErrorList'));
}]);
angular.module('cms.errors').factory('errors.errorService', [
    '$http',
    'shared.pluginServiceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        errorServiceBase = serviceBase + 'errors';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(errorServiceBase, {
            params: query
        });
    }

    service.getById = function (userId) {

        return $http.get(getIdRoute(userId));
    }

    /* PRIVATES */

    function getIdRoute(id) {
        return errorServiceBase + '/' + id;
    }

    return service;
}]);
angular.module('cms.errors').controller('ErrorDetailsController', [
    '$routeParams',
    'shared.LoadState',
    'errors.errorService',
function (
    $routeParams,
    LoadState,
    errorService
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // Properties
        vm.editMode = false;
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(vm.formLoadState.off);
    }

    /* UI ACTIONS */

    function reset() {
        vm.command = mapUpdateCommand(vm.error);
        vm.mainForm.formStatus.clear();
    }

    /* PRIVATE FUNCS */

    function initData() {
        var errorId = $routeParams.id;

        return errorService.getById(errorId)
            .then(load);

        function load(error) {

            vm.error = error;
        }
    }
}]);
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