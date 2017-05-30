angular
    .module('cms.setup', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('setup.modulePath', '/Admin/Modules/Setup/Js/');
angular.module('cms.setup').config([
    '$routeProvider',
    'shared.routingUtilities',
    'setup.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'SetupDetails'));

}]);
angular.module('cms.setup').factory('setup.setupService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        setupServiceBase = serviceBase + 'setup';

    /* COMMANDS */

    service.run = function (command) {

        return $http.post(setupServiceBase, command);
    }

    return service;
}]);

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