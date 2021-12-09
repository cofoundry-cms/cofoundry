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