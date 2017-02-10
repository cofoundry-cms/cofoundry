angular.module('cms.dashboard').controller('DashboardController', [
    '_',
    'shared.modalDialogService',
    'shared.urlLibrary',
function (
    _,
    modalDialogService,
    urlLibrary
    ) {

    var vm = this;

    init();

    function init() {
        vm.urlLibrary = urlLibrary;
    }

}]);