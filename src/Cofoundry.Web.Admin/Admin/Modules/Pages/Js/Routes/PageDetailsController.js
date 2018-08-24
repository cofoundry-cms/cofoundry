angular.module('cms.pages').controller('PageDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.entityVersionModalDialogService',
    'shared.urlLibrary',
    'shared.pageService',
    'shared.permissionValidationService',
    'pages.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    LoadState,
    SearchQuery,
    modalDialogService,
    entityVersionModalDialogService,
    urlLibrary,
    pageService,
    permissionValidationService,
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

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.versionsLoadState = new LoadState();

        vm.urlLibrary = urlLibrary;

        vm.versionsQuery = new SearchQuery({
            onChanged: loadVersions,
            useHistory: false,
            defaultParams: {
                pageSize: 6
            }
        });

        vm.canCreate = permissionValidationService.canCreate('COFPGE');
        vm.canUpdate = permissionValidationService.canUpdate('COFPGE');
        vm.canDelete = permissionValidationService.canDelete('COFPGE');
        vm.canPublishPage = permissionValidationService.hasPermission('COFPGEPAGPUB');
        vm.canUpdatePageUrl =  permissionValidationService.hasPermission('COFPGEUPDURL');

        // Init
        initData(vm.formLoadState);
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

        entityVersionModalDialogService
            .copyToDraft(vm.page.pageId, version.pageVersionId, vm.page.pageRoute.hasDraftVersion, setLoadingOn)
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
            templateUrl: modulePath + 'Routes/Modals/DuplicatePage.html',
            controller: 'DuplicatePageController',
            options: {
                page: vm.page
            }
        });
    }

    function changeUrl() {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/ChangePageUrl.html',
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

    function initData(loadStateToTurnOff) {

        return $q
            .all([getPage(), getVersions()])
            .then(function (results) {
                mapVersions(results[1]);
            })
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
           
        /* helpers */

        function getPage() {
            return pageService.getById($routeParams.id).then(function (page) {
                vm.page = page;
                vm.updatePageCommand = mapUpdatePageCommand(page);
                vm.updateDraftCommand = mapUpdateDraftCommand(page);
                vm.editMode = false;
                vm.isMarkedPublished = vm.page.pageRoute.publishStatus == 'Published';
                vm.publishStatusLabel = getPublishStatusLabel(page.pageRoute);

                return page;
            });
        }
    }

    function loadVersions() {
        vm.versionsLoadState.on();

        return getVersions()
            .then(mapVersions)
            .then(setLoadingOff.bind(null, vm.versionsLoadState));
    }

    function getVersions() {

        return pageService.getVersionsByPageId($routeParams.id, vm.versionsQuery.getParameters());
    }

    function mapVersions(pagedVersions) {
        var page = vm.page,
            isPublished = page.pageRoute.isPublished();

        _.each(pagedVersions.items, function (version) {

            version.versionLabel = getVersionLabel(version, page.pageRoute);
            version.browseUrl = vm.urlLibrary.visualEditorForVersion(page.pageRoute, version, false, isPublished);
        });

        vm.versions = pagedVersions;

        function getVersionLabel(version, entityRoute) {

            if (version.workFlowStatus == 'Draft') return version.workFlowStatus;

            var versionNumber = 'V' + version.displayVersion;

            if (!version.isLatestPublishedVersion) return versionNumber;

            return versionNumber + ' (' + getPublishStatusLabel(entityRoute) + ')';
        }
    }

    function getPublishStatusLabel(entityRoute) {
        if (entityRoute.publishStatus == 'Published' && entityRoute.publishDate < Date.now()) {
            return 'Pending Publish';
        }

        return entityRoute.publishStatus;
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