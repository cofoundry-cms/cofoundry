angular.module('cms.settings').controller('SettingsDetailsController', [
    '_',
    '$q',
    'shared.LoadState',
    'settings.settingsService',
    'settings.modulePath',
function (
    _,
    $q,
    LoadState,
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

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

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