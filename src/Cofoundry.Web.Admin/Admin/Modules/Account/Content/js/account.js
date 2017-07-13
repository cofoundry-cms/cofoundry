angular
    .module('cms.account', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('account.modulePath', '/Admin/Modules/Account/Js/');
angular.module('cms.account').config([
    '$routeProvider',
    'shared.routingUtilities',
    'account.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .when('/change-password', routingUtilities.mapOptions(modulePath, 'ChangePassword'))
        .otherwise(routingUtilities.mapOptions(modulePath, 'AccountDetails'));
}]);
angular.module('cms.account').factory('account.accountService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        accountServiceBase = serviceBase + 'account';

    /* QUERIES */
        
    service.getAccountDetails = function () {

        return $http.get(accountServiceBase);
    }

    /* COMMANDS */

    service.update = function (command) {

        return $http.patch(accountServiceBase, command);
    }

    service.updatePassword = function (command) {

        return $http.put(accountServiceBase + "/password", command);
    }

    return service;
}]);
angular.module('cms.account').controller('AccountDetailsController', [
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'account.accountService',
    'account.modulePath',
function (
    LoadState,
    modalDialogService,
    permissionValidationService,
    accountService,
    modulePath
    ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.canUpdate = permissionValidationService.canUpdate('COFCUR');

        // Init
        initData()
            .then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        accountService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand(vm.user);
        vm.mainForm.formStatus.clear();
    }

    function deleteUser() {
        var options = {
            title: 'Delete User',
            message: 'Are you sure you want to delete this user?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return accountService
                .remove(vm.user.userId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData() {

        return accountService
            .getAccountDetails()
            .then(load);

        function load(user) {

            vm.user = user;
            vm.command = mapUpdateCommand(user);
            vm.editMode = false;
        }
    }

    function mapUpdateCommand(user) {

        return _.pick(user,
            'firstName',
            'lastName',
            'email'
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