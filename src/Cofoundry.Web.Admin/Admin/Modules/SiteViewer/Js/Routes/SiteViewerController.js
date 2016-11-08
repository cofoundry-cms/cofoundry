angular.module('cms.siteViewer').controller('SiteViewerController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'siteViewer.modulePath',
    'siteViewer.options',
function (
    $window,
    $scope,
    _,
    LoadState,
    entityVersionModalDialogService,
    modulePath,
    options
    ) {

    var vm = this,
        document = $window.document,
        entityDialogServiceConfig = {
            entityNameSingular: options.entityNameSingular,
            isCustomEntity: options.isCustomEntityRoute
        };

    init();

    /* INIT */

    function init() {
        vm.globalLoadState = new LoadState();

        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.copyToDraft = copyToDraft;
        vm.toggleOpen = toggleOpen;
        vm.toggleVersionMenu = toggleVersionMenu;
        vm.toggleViewOptionsMenu = toggleViewOptionsMenu;
        vm.siteViewerActive = true;
        vm.siteViewerText = 'Hide';
        vm.versionMenuActive = false;
        vm.viewOptionsMenuActive = false;
    }

    /* UI ACTIONS */

    function toggleVersionMenu() {
        vm.versionMenuActive = !vm.versionMenuActive;
        vm.viewOptionsMenuActive = false;
    }

    function toggleViewOptionsMenu() {
        vm.viewOptionsMenuActive = !vm.viewOptionsMenuActive;
        vm.versionMenuActive = false;
    }

    function publish() {
        entityVersionModalDialogService
            .publish(options.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function unpublish() {
        entityVersionModalDialogService
            .unpublish(options.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function copyToDraft() {
        entityVersionModalDialogService
            .copyToDraft(options.entityId, options.versionId, options.hasDraftVersion, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function toggleOpen() {
        vm.siteViewerActive = !vm.siteViewerActive;
    }

    function toggleButtonText() {
        vm.siteViewerText = vm.siteViewerActive === true ? 'Hide' : 'Show';
    }

    /* PRIVATE FUNCS */
    
    function getCustomEntityConfig() {
        if (!options.isCustomEntityRoute) return;

        return {
            name: options.entityNameSingular
        }
    }

    function reload() {
        $window.location = $window.location.pathname;
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);