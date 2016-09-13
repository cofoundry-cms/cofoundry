angular.module('cms.dashboard').controller('DashboardController', [
    '_',
    'shared.modalDialogService',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'dashboard.dashboardService',
function (
    _,
    modalDialogService,
    LoadState,
    SearchQuery,
    urlLibrary,
    dashboardService) {

    var vm = this;

    init();

    function init() {

        vm.urlLibrary = urlLibrary;

        loadGrid(dashboardService.getPages, 'pages');
        loadGrid(dashboardService.getPageTemplates, 'pageTemplates');
        loadGrid(dashboardService.getDraftPages, 'draftPages');
        loadGrid(dashboardService.getUsers, 'users');
    }

    /* PRIVATE FUNCS */

    function loadGrid(queryExecutor, resultProperty) {
        var loadState = new LoadState(true);
        vm[resultProperty + 'LoadState'] = loadState;

        return queryExecutor().then(function (result) {

            vm[resultProperty] = result.items;
            loadState.off();
        });
    }

}]);