angular
    .module('cms.settings', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('settings.modulePath', '/Admin/Modules/Settings/Js/');
angular.module('cms.settings').config([
    '$routeProvider',
    'shared.routingUtilities',
    'settings.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    $routeProvider
        .otherwise(routingUtilities.mapOptions(modulePath, 'SettingsDetails'));

}]);
angular.module('cms.settings').factory('settings.settingsService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        settingsServiceBase = serviceBase + 'settings',
        generalSettingsRoute = settingsServiceBase + '/generalsite',
        seoSettingsRoute = settingsServiceBase + '/seo';

    /* QUERIES */

    service.getGeneralSiteSettings = function () {

        return $http.get(generalSettingsRoute);
    }

    service.getSeoSettings = function () {

        return $http.get(seoSettingsRoute);
    }
     
    /* COMMANDS */

    service.updateGeneralSiteSettings = function (command) {

        return $http.patch(generalSettingsRoute, command);
    }

    service.updateSeoSettings = function (command) {

        return $http.patch(seoSettingsRoute, command);
    }

    service.clearCache = function (command) {

        return $http.delete(serviceBase + 'cache');
    }

    return service;
}]);
angular.module('cms.settings').controller('SettingsDetailsController', [
    '_',
    '$q',
    'shared.LoadState',
    'shared.permissionValidationService',
    'settings.settingsService',
    'settings.modulePath',
function (
    _,
    $q,
    LoadState,
    permissionValidationService,
    settingsService,
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
        vm.clearCache = clearCache;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.canUpdateSettings = permissionValidationService.hasPermission('COFSETGENUPD');

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

        settingsService.updateGeneralSiteSettings(vm.generalSettingsCommand)
            .then(settingsService.updateSeoSettings.bind(null, vm.seoSettingsCommand))
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        resetData();
        vm.mainForm.formStatus.clear();
    }

    function clearCache() {
        vm.globalLoadState.on();

        settingsService.clearCache()
            .then(onSuccess.bind(null, 'Cache cleared'))
            .finally(vm.globalLoadState.off);
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData() {

        var generalPromise = settingsService
            .getGeneralSiteSettings()
            .then(load.bind(null, 'generalSettings'));

        var seoPromise = settingsService
            .getSeoSettings()
            .then(load.bind(null, 'seoSettings'));

        return $q.all([
            generalPromise,
            seoPromise
        ]).then(function () {
            resetData();
            vm.editMode = false;
        });

        function load(prop, result) {
            vm[prop] = result;
        }
    }

    function resetData() {
        vm.seoSettingsCommand = _.clone(vm.seoSettings);
        vm.generalSettingsCommand = _.clone(vm.generalSettings);
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