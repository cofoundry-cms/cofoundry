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
        vm.switchDisplaySize = switchDisplaySize;
        vm.displayClass = 'full';
        vm.siteViewerActive = true;
        vm.versionMenuActive = false;
        vm.viewOptionsMenuActive = false;

        checkLocalStorage();
    }

    function checkLocalStorage() {
        var displayMode = localStorageService.getValue('displayMode');

        if (displayMode == null || displayMode == 'null') return;

        switchDisplaySize(null, displayMode);
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
        var wrapper = document.getElementById('wrapper');
        vm.siteViewerActive = !vm.siteViewerActive;

        if (vm.siteViewerActive === false) {
            wrapper.classList.add('down');
        }
        else {
            wrapper.classList.remove('down');
        }
    }

    function switchDisplaySize(e, displayClass) {
        var list = document.getElementById('display-list'),
            links = list.getElementsByTagName('li');

        if (e !== null && e.target.tagName.toLowerCase() == 'span') {
            targetLink = e.target.parentElement;
        }
        else {
            targetLink = document.getElementById('display-' + displayClass);
        }

        for (var i = 0; i < links.length; i++) {
            links[i].classList.remove('active');
        }

        targetLink.classList.add('active');
        vm.displayClass = displayClass;

        localStorageService.setValue('displayMode', vm.displayClass);
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