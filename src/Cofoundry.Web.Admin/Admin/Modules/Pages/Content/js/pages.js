angular
    .module('cms.pages', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('pages.modulePath', '/Admin/Modules/Pages/Js/');
angular.module('cms.pages').config([
    '$routeProvider',
    'shared.routingUtilities',
    'pages.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    var mapOptions = routingUtilities.mapOptions.bind(null, modulePath);

    $routeProvider
        .when('/new', mapOptions('AddPage'))
        .when('/:id', mapOptions('PageDetails'))
        .otherwise(mapOptions('PageList'));

}]);
angular.module('cms.pages').factory('pages.customEntityService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {};

    /* QUERIES */

    service.getAllRoutingRules = function () {
        return $http.get(serviceBase + 'custom-entity-routing-rules/');
    }
    
    return service;
}]);
angular.module('cms.pages').factory('pages.directoryService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
        directoryServiceBase = serviceBase + 'page-directories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    return service;
}]);
angular.module('cms.pages').factory('pages.pageTemplateService', [
    '$http',
    '$q',
    'shared.serviceBase',
function (
    $http,
    $q,
    serviceBase) {

    var service = {},
        pageTemplateServiceBase = serviceBase + 'page-templates';

    /* QUERIES */

    service.getAll = function () {
        var def = $q.defer();

        $http.get(pageTemplateServiceBase).then(function (pagedResult) {
            def.resolve(pagedResult.items);
        }, def.reject);

        return def.promise;
    }

    service.getExtensionDataModelSchemas = function (pageTemplateId) {
        return $http.get(pageTemplateServiceBase + '/' + pageTemplateId + '/extension-data-model-schemas');;
    }
    
    /* COMMANDS */

    return service;
}]);
angular.module('cms.pages').controller('ChangePageUrlController', [
    '$scope',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.pageService',
    'pages.customEntityService',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    LoadState,
    pageService,
    customEntityService,
    options,
    close) {

    var loadLocalesDeferred = $q.defer(),
        loadPageDirectoryDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {

        initData();

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = close;
        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.pageDirectoriesLoaded = loadPageDirectoryDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred, loadPageDirectoryDeferred, loadRoutingRules());
    }

    function initData() {
        var page = options.page,
            pageRoute = page.pageRoute;

        $scope.isCustomEntityRoute = pageRoute.pageType === 'CustomEntityDetails';

        $scope.page = page;
        $scope.command = {
            pageId: page.pageId,
            localeId: pageRoute.locale ? pageRoute.locale.localeId : undefined,
            pageDirectoryId: pageRoute.pageDirectory.pageDirectoryId
        };

        if ($scope.isCustomEntityRoute) {
            $scope.command.customEntityRoutingRule = pageRoute.urlPath;
        } else {
            $scope.command.urlPath = pageRoute.urlPath;
        }
    }

    function loadRoutingRules() {

        if ($scope.isCustomEntityRoute) {
            return customEntityService
                .getAllRoutingRules()
                .then(function (routingRules) {
                    $scope.routingRules = routingRules;
                });
        } 
        
        var def = $q.defer();
        def.resolve();
        return def;
    }

    /* EVENTS */

    function save() {
        $scope.submitLoadState.on();

        pageService
            .updateUrl($scope.command)
            .then(options.onSave)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

}]);
angular.module('cms.pages').controller('DuplicatePageController', [
    '$scope',
    '$q',
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.pageService',
    'pages.customEntityService',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    stringUtilities,
    LoadState,
    pageService,
    customEntityService,
    options,
    close) {

    var loadLocalesDeferred = $q.defer(),
        loadPageDirectoryDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {
       
        initData();

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = close;
        $scope.onTitleChanged = onTitleChanged;

        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.pageDirectoriesLoaded = loadPageDirectoryDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred, loadPageDirectoryDeferred, loadRoutingRules());
    }

    /* EVENTS */

    function initData() {
        var page = options.page,
           pageRoute = page.pageRoute;

        $scope.isCustomEntityRoute = pageRoute.pageType === 'CustomEntityDetails';

        $scope.page = page;
        $scope.command = {
            pageToDuplicateId: page.pageId,
            localeId: pageRoute.locale ? pageRoute.locale.localeId : undefined,
            pageDirectoryId: pageRoute.pageDirectory.pageDirectoryId,
            title: 'Copy of ' + page.latestVersion.title
        }
        if ($scope.isCustomEntityRoute) {
            $scope.command.customEntityRoutingRule = pageRoute.urlPath;
        } else {
            onTitleChanged();
        }
    }

    function onTitleChanged() {
        if (!$scope.isCustomEntityRoute) {
            $scope.command.urlPath = stringUtilities.slugify($scope.command.title, 200);
        }
    }

    function loadRoutingRules() {

        if ($scope.isCustomEntityRoute) {
            return customEntityService
                .getAllRoutingRules()
                .then(function (routingRules) {
                    $scope.routingRules = routingRules;
                });
        }

        var def = $q.defer();
        def.resolve();
        return def;
    }

    function save() {
        $scope.submitLoadState.on();

        pageService
            .duplicate($scope.command)
            .then(redirectoToPage)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

    /* PRIVATE FUNCS */

    function redirectoToPage(pageId) {
        $location.path('/' + pageId);
    }
}]);
angular.module('cms.pages').controller('AddPageController', [
    '_',
    '$scope',
    '$q',
    '$location',
    '$window',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.urlLibrary',
    'shared.pageService',
    'pages.pageTemplateService',
    'pages.customEntityService',
function (
    _,
    $scope,
    $q,
    $location,
    $window,
    LoadState,
    stringUtilities,
    urlLibrary,
    pageService,
    pageTemplateService,
    customEntityService
    ) {

    var vm = this,
        loadLocalesDeferred = $q.defer(),
        loadPageDirectoryDeferred = $q.defer();

    init();

    /* INIT */

    function init() {

        vm.save = save.bind(null, false, redirectToDetails);
        vm.saveAndPublish = save.bind(null, true, redirectToDetails);
        vm.saveAndEdit = save.bind(null, false, redirectToVisualEditor);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;
        vm.onPageTypeChanged = onPageTypeChanged;
        vm.onPageTemplateChanged = onPageTemplateChanged;

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.onLocalesLoaded = loadLocalesDeferred.resolve;
        vm.onPageDirectoriesLoaded = loadPageDirectoryDeferred.resolve;

        initData();
    }

    /* EVENTS */

    function save(publish, redirectToCommand) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        if (vm.command.pageType == 'CustomEntityDetails') {
            vm.command.urlPath = undefined;
        } else {
            vm.command.customEntityRoutingRule = undefined;
        }

        setLoadingOn(loadState);

        pageService
            .add(vm.command)
            .then(redirectToCommand)
            .finally(setLoadingOff.bind(null, loadState));

    }

    function redirectToDetails(id) {
        $location.path('/' + id);
    }

    function redirectToVisualEditor(id) {

        return pageService
            .getById(id)
            .then(redirect);

        function redirect(page) {
            $window.location.href = urlLibrary.visualEditorForPage(page.pageRoute, true);
        }
    }

    function onNameChanged() {
        vm.command.urlPath = stringUtilities.slugify(vm.command.title, 200);
    }

    function onPageTypeChanged() {
        var selectedPageType = vm.command.pageType,
            filterBy = selectedPageType  == 'CustomEntityDetails' ? selectedPageType : 'Generic'; 
       
        vm.pageTemplates = _.where(vm.allPageTemplates, { pageType: filterBy });
    }

    function onPageTemplateChanged() {
       pageTemplateService
            .getExtensionDataModelSchemas(vm.command.pageTemplateId)
            .then(function (schemas) {
                vm.command.extensionData = vm.command.extensionData || {};
                vm.extensionDataSources = [];
                _.each(schemas, mapSchema);
            });
            
        function mapSchema(modelMetaData) {
            var dataModel = vm.command.extensionData[modelMetaData.name] || {};
            if (modelMetaData.defaultValue && modelMetaData.defaultValue.value) {
                vm.command.extensionData[modelMetaData.name] = _.extend(angular.copy(modelMetaData.defaultValue.value), dataModel);
            } else {
                vm.command.extensionData[modelMetaData.name]  = dataModel;
            }

            vm.extensionDataSources.push({
                model: vm.command.extensionData[modelMetaData.name],
                modelMetaData: modelMetaData
            });
        }
    }

    /* PRIVATE FUNCS */

    function cancel() {
        $location.path('/');
    }

    function initData() {
        vm.pageTypes = pageService.getPageTypes();

        vm.command = {
            showInSiteMap: true,
            pageType: vm.pageTypes[0].value
        };

        var loadTemplatesDeferred = pageTemplateService
            .getAll()
            .then(function (pageTemplates) {
                vm.allPageTemplates = pageTemplates;
            });

        var loadRoutingRulesDeferred = customEntityService
            .getAllRoutingRules()
            .then(function (routingRules) {
                vm.routingRules = routingRules;
            });

        vm.formLoadState
            .offWhen(
                loadLocalesDeferred,
                loadPageDirectoryDeferred,
                loadTemplatesDeferred,
                loadRoutingRulesDeferred
                )
            .then(onPageTypeChanged);
        
        $scope.$watch('vm.command.localeId', function (localeId) {

            if (localeId) {
                vm.additionalParameters = {
                    localeId: localeId
                };
            } else {
                vm.additionalParameters = {};
            }
        });
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
    'shared.userAreaService',
    'shared.internalModulePath',
    'pages.pageTemplateService',
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
    userAreaService,
    sharedModulePath,
    pageTemplateService,
    modulePath
    ) {

    var vm = this,
        ENTITY_DEFINITION_CODE = 'COFPGE';

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
        vm.viewAccessRules = viewAccessRules;

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

        vm.canCreate = permissionValidationService.canCreate(ENTITY_DEFINITION_CODE);
        vm.canUpdate = permissionValidationService.canUpdate(ENTITY_DEFINITION_CODE);
        vm.canDelete = permissionValidationService.canDelete(ENTITY_DEFINITION_CODE);
        vm.canPublishPage = permissionValidationService.hasPermission(ENTITY_DEFINITION_CODE + 'PAGPUB');
        vm.canUpdatePageUrl =  permissionValidationService.hasPermission(ENTITY_DEFINITION_CODE + 'UPDURL');

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

    function viewAccessRules() {

        modalDialogService.show({
            templateUrl: sharedModulePath + 'UIComponents/EntityAccess/EntityAccessEditor.html',
            controller: 'EntityAccessEditorController',
            options: {
                entityDefinitionCode: ENTITY_DEFINITION_CODE,
                entityIdPrefix: 'page',
                entityDefinitionName: 'Page',
                entityDescription: vm.page.pageRoute.fullUrlPath,
                entityAccessLoader: pageAccessLoader,
                saveAccess: pageService.updateAccessRules
            }
        });

        function pageAccessLoader() {
            return pageService
                .getAccessRulesByPageId(vm.page.pageId);
        }
    }

    /* PRIVATE FUNCS */
    
    function onSuccess(message, loadStateToTurnOff) {

        return initData(loadStateToTurnOff)
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData(loadStateToTurnOff) {

        return $q
            .all([getPage(), getVersions(), getUserAreas()])
            .then(function (results) {
                mapVersions(results[1]);
                return getExtensionMetaData(results[0]);
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

                if (vm.page.pageRoute.locale) {
                    vm.additionalParameters = {
                        localeId: vm.page.pageRoute.locale.localeId
                    };
                } else {
                    vm.additionalParameters = {};
                }
                return page;
            });
        }

        function getUserAreas() {
            return userAreaService.getAll().then(function (userAreas) {
                vm.accessRulesEnabled = userAreas.length > 1;
            });
        }
        
        function getExtensionMetaData(page) {
            return pageTemplateService
                .getExtensionDataModelSchemas(page.latestVersion.template.pageTemplateId)
                .then(function (schemas) {
                    vm.extensionDataSources = [];
                    _.each(schemas, mapSchema);
                });
                
            function mapSchema(modelMetaData) {
                vm.extensionDataSources.push({
                    model: vm.updateDraftCommand.extensionData[modelMetaData.name],
                    modelMetaData: modelMetaData
                });
            }
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
            showInSiteMap: version.showInSiteMap,
            extensionData: version.extensionData
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
angular.module('cms.pages').controller('PageListController', [
    '_',
    'shared.entityVersionModalDialogService',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.pageService',
    'shared.permissionValidationService',
    'pages.pageTemplateService',
function (
    _,
    entityVersionModalDialogService,
    LoadState,
    SearchQuery,
    pageService,
    permissionValidationService,
    pageTemplateService) {

    var vm = this;

    init();

    function init() {

        loadFilterData();

        vm.gridLoadState = new LoadState();
        vm.globalLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        toggleFilter(false);
        vm.publish = publish;

        vm.canCreate = permissionValidationService.canCreate('COFPGE');
        vm.canUpdate = permissionValidationService.canUpdate('COFPGE');

        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function publish(pageId) {

        entityVersionModalDialogService
            .publish(pageId, vm.globalLoadState.on)
            .then(loadGrid)
            .then(vm.globalLoadState.off)
            .catch(vm.globalLoadState.off);
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* PRIVATE FUNCS */

    function loadFilterData() {
        vm.publishStatus = [{
            name: 'Unpublished'
        }, {
            name: 'Published'
        }];

        pageTemplateService.getAll().then(function (pageTemplates) {
            vm.pageTemplates = pageTemplates;
        });
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return pageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);