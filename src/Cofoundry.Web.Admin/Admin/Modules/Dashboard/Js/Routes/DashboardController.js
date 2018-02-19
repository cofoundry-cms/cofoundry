angular.module('cms.dashboard').controller('DashboardController', [
    '_',
    'shared.modalDialogService',
    'shared.urlLibrary',
    'dashboard.dashboardService',
    function (
    _,
    modalDialogService,
    urlLibrary,
    dashboardService
    ) {

    var vm = this;

    init();

    function init() {
        vm.urlLibrary = urlLibrary;

        dashboardService
            .getContent()
            .then(function (content) {
                vm.content = content;
            });
    }

}]);