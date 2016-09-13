
angular.module('cms.setup').controller('SetupDetailsController', [
    '_',
    '$q',
    'shared.LoadState',
    'shared.urlLibrary',
    'setup.setupService',
    'setup.modulePath',
function (
    _,
    $q,
    LoadState,
    urlLibrary,
    settingsService,
    modulePath
    ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        // UI actions
        vm.save = save;

        // helpers
        vm.doesPasswordMatch = doesPasswordMatch;
        vm.urlLibrary = urlLibrary;

        // Properties
        vm.saveLoadState = new LoadState();
        vm.command = {};
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

    function doesPasswordMatch(value) {
        if (!vm.command) return false;

        return vm.command.userPassword === value;
    }

}]);