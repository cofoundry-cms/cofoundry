angular.module('cms.siteViewer').controller('SiteViewerController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'shared.localStorage',
    'siteViewer.modulePath',
    'siteViewer.options',
function (
    $window,
    $scope,
    _,
    LoadState,
    entityVersionModalDialogService,
    localStorageService,
    modulePath,
    options
    ) {

    var vm = this,
        document = $window.document,
        entityDialogServiceConfig;

    init();

    /* INIT */

    function init() {
        // Create IE + others compatible event handler
        var eventMethod = $window.addEventListener ? "addEventListener" : "attachEvent",
            postMessageListener = window[eventMethod],
            messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

        postMessageListener(messageEvent, handleMessage);

        vm.globalLoadState = new LoadState();
        vm.config = config;
        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.copyToDraft = copyToDraft;
    }

    /* UI ACTIONS */

    function handleMessage(e) {
        vm[e.data.action].apply(this, e.data.args);
    }

    function config(isCustomEntityRoute, entityNameSingular) {
        entityDialogServiceConfig = {
            entityNameSingular: entityNameSingular,
            isCustomEntity: isCustomEntityRoute
        };
    }

    function publish(args) {
        entityVersionModalDialogService
            .publish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function unpublish(args) {
        entityVersionModalDialogService
            .unpublish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function copyToDraft(args) {
        entityVersionModalDialogService
            .copyToDraft(args.entityId, args.versionId, args.hasDraftVersion, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    /* PRIVATE FUNCS */

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