angular.module('cms.pages').controller('PageDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.entityVersionModalDialogService',
    'shared.urlLibrary',
    'shared.pageService',
    'pages.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    LoadState,
    modalDialogService,
    entityVersionModalDialogService,
    urlLibrary,
    pageService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save.bind(null, false);
        vm.saveAndPublish = save.bind(null, true);
        vm.cancel = reset;
        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.discardDraft = discardDraft;
        vm.copyToDraft = copyToDraft;
        vm.deletePage = deletePage;
        vm.duplicatePage = duplicatePage;
        vm.changeUrl = changeUrl;

        // Helper Functions
        vm.getPartialUrl = getPartialUrl;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.urlLibrary = urlLibrary;

        // Init
        initData(vm.formLoadState);
    }

    /* PUBLIC FUNCS */

    function getPartialUrl(file) {
        return modulePath + 'routes/partials/' + file + '.html';
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save(publish) {

        var loadState;

        if (publish) {
            vm.updateDraftCommand.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        pageService.update(vm.updatePageCommand)
            .then(pageService.updateDraft.bind(this, vm.updateDraftCommand))
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, loadState));
    }

    function reset() {
        vm.editMode = false;
        vm.updatePageCommand = mapUpdatePageCommand(vm.page);
        vm.updateDraftCommand = mapUpdateDraftCommand(vm.page);
        vm.mainForm.formStatus.clear();
    }

    function publish() {

        entityVersionModalDialogService
            .publish(vm.page.pageId, setLoadingOn)
            .then(onSuccess.bind(null, 'Page published successfully.'))
            .catch(setLoadingOff);
    }

    function unpublish() {
        entityVersionModalDialogService
            .unpublish(vm.page.pageId, setLoadingOn)
            .then(onSuccess.bind(null, 'The page has been unpublished and reverted to draft state.'))
            .catch(setLoadingOff);
    }

    function discardDraft() {
        var options = {
            title: 'Discard Version',
            message: 'Are you sure you want to discard this draft? This will discard all changes since the page was last published.',
            okButtonTitle: 'Yes, discard it',
            onOk: onOk
        };

        modalDialogService
            .confirm(options)
            .then(onSuccess.bind(null, 'Draft discarded successfully'));

        function onOk() {
            setLoadingOn();
            return pageService.removeDraft(vm.page.pageId);
        }
    }

    function copyToDraft(version) {
        var hasDraftVersion = !!getDraftVersion();

        entityVersionModalDialogService
            .copyToDraft(vm.page.pageId, version.pageVersionId, hasDraftVersion, setLoadingOn)
            .then(onOkSuccess)
            .catch(setLoadingOff);

        function onOkSuccess() {
            onSuccess('Draft created successfully.')
        }
    }

    function deletePage() {
        var options = {
            title: 'Delete Page',
            message: 'Are you sure you want to delete this page?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService
            .confirm(options);

        function onOk() {
            setLoadingOn();
            return pageService
                .remove(vm.page.pageId)
                .then(redirectToPageList)
                .catch(setLoadingOff);
        }
    }

    function duplicatePage() {

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/duplicatepage.html',
            controller: 'DuplicatePageController',
            options: {
                page: vm.page
            }
        });
    }

    function changeUrl() {

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/changepageurl.html',
            controller: 'ChangePageUrlController',
            options: {
                page: vm.page,
                onSave: onSuccess.bind(null, 'Url Changed')
            }
        });
    }

    /* PRIVATE FUNCS */
    
    function onSuccess(message, loadStateToTurnOff) {
        return initData(loadStateToTurnOff)
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function getDraftVersion() {
        return _.find(vm.versions, function (version) {
            return version.workFlowStatus === 'Draft';
        });
    }

    function initData(loadStateToTurnOff) {

        return $q
            .all([getPage(), getVersions()])
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
           
        /* helpers */

        function getPage() {
            return pageService.getById($routeParams.id).then(function (page) {
                vm.page = page;
                vm.updatePageCommand = mapUpdatePageCommand(page);
                vm.updateDraftCommand = mapUpdateDraftCommand(page);
                vm.editMode = false;
            });
        }

        function getVersions() {
            return pageService.getVersionsByPageId($routeParams.id).then(function (versions) {
                vm.versions = versions;
            });
        }
    }

    function mapUpdatePageCommand(page) {

        return {
            pageId: page.pageId,
            tags: page.tags
        }
    }

    function mapUpdateDraftCommand(page) {
        var version = page.latestVersion,
            ogData = version.openGraph;

        return {
            pageId: page.pageId,
            title: version.title,
            metaDescription: version.metaDescription,
            openGraphTitle: ogData.title,
            openGraphDescription: ogData.description,
            openGraphImageId: ogData.image ? ogData.image.ImageAssetId : undefined,
            showInSiteMap: version.showInSiteMap
        }
    }

    function redirectToPageList() {
        $location.path('');
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