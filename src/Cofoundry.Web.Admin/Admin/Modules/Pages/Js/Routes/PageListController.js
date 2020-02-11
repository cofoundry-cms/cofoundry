angular.module('cms.pages').controller('PageListController', [
    '_',
    'shared.entityVersionModalDialogService',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.pageService',
    'shared.permissionValidationService',
    'pages.pageTemplateService',
function (
    _,
    entityVersionModalDialogService,
    LoadState,
    SearchQuery,
    pageService,
    permissionValidationService,
    pageTemplateService) {

    var vm = this;

    init();

    function init() {

        loadFilterData();

        vm.gridLoadState = new LoadState();
        vm.globalLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        toggleFilter(false);
        vm.publish = publish;

        vm.canCreate = permissionValidationService.canCreate('COFPGE');
        vm.canUpdate = permissionValidationService.canUpdate('COFPGE');

        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function publish(pageId) {

        entityVersionModalDialogService
            .publish(pageId, vm.globalLoadState.on)
            .then(loadGrid)
            .then(vm.globalLoadState.off)
            .catch(vm.globalLoadState.off);
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* PRIVATE FUNCS */

    function loadFilterData() {
        vm.publishStatus = [{
            name: 'Unpublished'
        }, {
            name: 'Published'
        }];

        pageTemplateService.getAll().then(function (pageTemplates) {
            vm.pageTemplates = pageTemplates;
        });
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return pageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);