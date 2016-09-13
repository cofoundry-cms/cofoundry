angular.module('cms.account').controller('ChangePasswordController', [
    '$location',
    'shared.LoadState',
    'shared.modalDialogService',
    'account.accountService',
function (
    $location,
    LoadState,
    modalDialogService,
    accountService
    ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        // UI actions
        vm.save = save;
        vm.cancel = cancel;

        // helpers
        vm.doesPasswordMatch = doesPasswordMatch;

        // Properties
        vm.globalLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(vm.formLoadState.off);
    }

    /* UI ACTIONS */
    
    function save() {
        vm.globalLoadState.on();

        accountService
            .updatePassword(vm.command)
            .then(redirectToDetails)
            .finally(vm.globalLoadState.off);
    }

    function cancel() {
        redirectToDetails();
    }

    /* PRIVATE FUNCS */

    function doesPasswordMatch(value) {
        if (!vm.command) return false;

        return vm.command.newPassword === value;
    }

    function initData() {

        return accountService
            .getAccountDetails()
            .then(load);

        function load(user) {

            vm.user = user;
            vm.command = {};
        }
    }

    function redirectToDetails() {
        $location.path('/');
    }
}]);