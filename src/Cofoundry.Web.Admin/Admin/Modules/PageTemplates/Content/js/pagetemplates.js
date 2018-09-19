angular
    .module('cms.pageTemplates', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('pageTemplates.modulePath', '/Admin/Modules/PageTemplates/Js/');
angular.module('cms.pageTemplates').config([
    '$routeProvider',
    'shared.routingUtilities',
    'pageTemplates.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'PageTemplate');

}]);
angular.module('cms.pageTemplates').factory('pageTemplates.pageTemplateService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pageTemplateServiceBase = serviceBase + 'page-templates';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pageTemplateServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(getIdRoute(imageId));
    }

    /* PRIVATES */

    function getIdRoute(id) {
        return pageTemplateServiceBase + '/' + id;
    }

    return service;
}]);
angular.module('cms.pageTemplates').controller('PageTemplateDetailsController', [
    '$routeParams',
    '$location',
    'shared.LoadState',
    'shared.urlLibrary',
    'pageTemplates.pageTemplateService',
    'pageTemplates.modulePath',
function (
    $routeParams,
    $location,
    LoadState,
    urlLibrary,
    pageTemplateService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // Properties
        vm.urlLibrary = urlLibrary;
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* PRIVATE FUNCS */

    function initData() {
        var pageTemplateId = $routeParams.id;

        return pageTemplateService.getById(pageTemplateId)
            .then(load);

        function load(pageTemplate) {

            vm.pageTemplate = pageTemplate;
            vm.command = mapUpdateCommand(pageTemplate);
            vm.editMode = false;
        }
    }

    function mapUpdateCommand(pageTemplate) {

        return _.pick(pageTemplate,
            'pageTemplateId',
            'name',
            'description'
            );
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
        if (loadState && _.isFunction(loadState.on)) loadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
        if (loadState && _.isFunction(loadState.off)) loadState.off();
    }
}]);
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