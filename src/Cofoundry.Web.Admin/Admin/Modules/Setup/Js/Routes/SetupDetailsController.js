
angular.module('cms.setup').controller('SetupDetailsController', [
    '_',
    'shared.LoadState',
    'shared.urlLibrary',
    'shared.userAreaService',
    'setup.setupService',
function (
    _,
    LoadState,
    urlLibrary,
    userAreaService,
    settingsService
    ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        vm.save = save;

        vm.urlLibrary = urlLibrary;
        vm.saveLoadState = new LoadState();

        initData();
    }

    /* UI ACTIONS */

    function save() {

        vm.saveLoadState.on();

        settingsService.run(vm.command)
            .then(onSuccess)
            .finally(vm.saveLoadState.off);

        function onSuccess() {
            vm.isSetupComplete = true;
        }
    }
    
    /* PRIVATE FUNCS */

    function initData() {
        vm.command = {};

        return userAreaService
            .getPasswordPolicy('COF')
            .then(function(passwordPolicy) {
                vm.passwordPolicy = passwordPolicy;
            });
    }

}]);