angular.module('cms.siteViewer').controller('SiteViewerController', [
    '$window',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'siteViewer.modulePath',
    'siteViewer.options',
function (
    $window,
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
    }

    /* UI ACTIONS */

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