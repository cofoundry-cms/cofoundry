angular
    .module('cms.customEntities', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('customEntities.modulePath', '/Admin/Modules/CustomEntities/Js/');
angular.module('cms.customEntities').config([
    '$routeProvider',
    'shared.routingUtilities',
    'customEntities.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'CustomEntity');
}]);
angular.module('cms.customEntities').controller('AddCustomEntityController', [
    '$scope',
    '$location',
    '$q',
    '$window',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.urlLibrary',
    'customEntities.options',
function (
    $scope,
    $location,
    $q,
    $window,
    stringUtilities,
    LoadState,
    customEntityService,
    urlLibrary,
    moduleOptions) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();

        vm.formLoadState = new LoadState(true);
        vm.editMode = false;
        vm.options = moduleOptions;
        vm.saveButtonText = moduleOptions.autoPublish ? 'Save' : 'Save & Publish';
        vm.saveAndEditButtonText = moduleOptions.autoPublish ? 'Save & Edit Content' : 'Save Draft & Edit Content';

        vm.save = save.bind(null, false, redirectToDetails);
        vm.saveAndPublish = save.bind(null, true, redirectToDetails);
        vm.saveAndEdit = save.bind(null, false, redirectToVisualEditor);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;

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

        setLoadingOn(loadState);

        customEntityService
            .add(vm.command, moduleOptions.customEntityDefinitionCode)
            .then(redirectToCommand)
            .finally(setLoadingOff.bind(null, loadState));
    }

    function onNameChanged() {
        vm.command.urlSlug = stringUtilities.slugify(vm.command.title);
    }
    
    /* PRIVATE FUNCS */
    
    function cancel() {
        $location.path('/');
    }

    function redirectToDetails(id) {
        $location.path('/' + id);
    }

    function redirectToVisualEditor(id) {

        return customEntityService
            .getById(id)
            .then(redirect);

        function redirect(customEntity) {
            $window.location.href = urlLibrary.customEntityVisualEditor(customEntity, true);
        }
    }

    function initData() {

        var schemaDefferred = customEntityService
            .getDataModelSchema(moduleOptions.customEntityDefinitionCode)
            .then(loadModelSchema);

        var pageRoutesDefferred = customEntityService
            .getPageRoutes(moduleOptions.customEntityDefinitionCode)
            .then(loadPageRoutes);

        vm.formLoadState.offWhen(schemaDefferred, pageRoutesDefferred);

        vm.command = {};

        $scope.$watch('vm.command.localeId', function (localeId) {

            if (localeId) {
                vm.additionalParameters = {
                    localeId: localeId
                };
            } else {
                vm.additionalParameters = {};
            }
        });

        function loadModelSchema(modelMetaData) {
            vm.command.model = {};

            vm.formDataSource = {
                model: vm.command.model,
                modelMetaData: modelMetaData
            };
        }

        function loadPageRoutes(pageRoutes) {
            vm.pageRoutes = pageRoutes;
        }
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
angular.module('cms.customEntities').controller('CustomEntityDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.entityVersionModalDialogService',
    'shared.customEntityService',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'customEntities.modulePath',
    'customEntities.options',
function (
    $routeParams,
    $q,
    $location,
    _,
    LoadState,
    modalDialogService,
    entityVersionModalDialogService,
    customEntityService,
    urlLibrary,
    permissionValidationService,
    modulePath,
    moduleOptions
    ) {

    var vm = this,
        nameSingularLowerCase = moduleOptions.nameSingular.toLowerCase(),
        entityDialogConfig = {
            entityNameSingular: moduleOptions.nameSingular,
            isCustomEntity: true
        },
        modelMetaData;

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
        vm.deleteCustomEntity = deleteCustomEntity;
        vm.changeUrl = changeUrl;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.options = moduleOptions;
        vm.urlLibrary = urlLibrary;
        vm.saveButtonText = moduleOptions.autoPublish ? 'Save' : 'Save & Publish';
        vm.canChangeUrl = !moduleOptions.autoGenerateUrlSlug || moduleOptions.hasLocale;

        vm.canPublish = getPermission('CMEPUB');
        vm.canUpdateUrl = getPermission('UPDURL');
        vm.canDelete = getPermission('COMDEL');
        vm.canUpdate = getPermission('CMEPUB');

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
            vm.updateCommand.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        customEntityService.updateDraft(vm.updateCommand, moduleOptions.customEntityDefinitionCode)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, loadState));
    }

    function reset() {
        vm.editMode = false;
        vm.updateCommand = mapUpdateCommand(vm.customEntity);
        vm.mainForm.formStatus.clear();
    }

    function publish() {

        entityVersionModalDialogService
            .publish(vm.customEntity.customEntityId, setLoadingOn, entityDialogConfig)
            .then(onSuccess.bind(null, moduleOptions.nameSingular + ' published successfully.'))
            .catch(setLoadingOff);
    }

    function unpublish() {

        entityVersionModalDialogService
            .unpublish(vm.customEntity.customEntityId, setLoadingOn, entityDialogConfig)
            .then(onSuccess.bind(null, 'The ' + nameSingularLowerCase + ' has been unpublished and reverted to draft state.'))
            .catch(setLoadingOff);

    }

    function discardDraft() {
        var options = {
            title: 'Discard Version',
            message: 'Are you sure you want to discard this draft? This will discard all changes since it was last published.',
            okButtonTitle: 'Yes, discard it',
            onOk: onOk
        };

        modalDialogService
            .confirm(options)
            .then(onSuccess.bind(null, 'Draft discarded successfully'));

        function onOk() {
            setLoadingOn();
            return customEntityService.removeDraft(vm.customEntity.customEntityId);
        }
    }

    function copyToDraft(version) {
        var hasDraftVersion = !!getDraftVersion();

        entityVersionModalDialogService
            .copyToDraft(vm.customEntity.customEntityId, version.customEntityVersionId, hasDraftVersion, setLoadingOn, entityDialogConfig)
            .then(onOkSuccess)
            .catch(setLoadingOff);

        function onOkSuccess() {
            onSuccess('Draft created successfully.')
        }
    }

    function deleteCustomEntity() {
        var options = {
            title: 'Delete ' + moduleOptions.nameSingular,
            message: 'Are you sure you want to delete this ' + nameSingularLowerCase + '?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return customEntityService
                .remove(vm.customEntity.customEntityId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }
    
    function changeUrl() {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/ChangeUrl.html',
            controller: 'ChangeUrlController',
            options: {
                customEntity: vm.customEntity,
                onSave: onSuccess.bind(null, 'Url Changed')
            }
        });
    }

    /* PRIVATE FUNCS */

    function getPermission(code) {
        return permissionValidationService.hasPermission(moduleOptions.customEntityDefinitionCode + code);
    }

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
            .all([getCustomEntity(), getVersions(), getMetaData()])
            .then(onLoaded)
            .then(setLoadingOff.bind(null, loadStateToTurnOff));

        /* helpers */

        function onLoaded(results) {
            var customEntity = results[0],
                versions = results[1];

            modelMetaData = results[2];

            vm.customEntity = customEntity;
            vm.versions = versions;
            vm.updateCommand = mapUpdateCommand(customEntity);

            if (vm.customEntity.locale) {
                vm.additionalParameters = {
                    localeId: vm.customEntity.locale.localeId
                };
            } else {
                vm.additionalParameters = {};
            }

            vm.editMode = false;
        }

        function getMetaData() {
            return customEntityService.getDataModelSchema(moduleOptions.customEntityDefinitionCode);
        }

        function getCustomEntity() {
            return customEntityService.getById($routeParams.id);
        }

        function getVersions() {
            return customEntityService.getVersionsByCustomEntityId($routeParams.id);
        }
    }
    
    function mapUpdateCommand(customEntity) {

        var command = {
            customEntityId: customEntity.customEntityId,
            title: customEntity.latestVersion.title,
            model: angular.copy(customEntity.latestVersion.model)
        }

        vm.formDataSource = {
            model: command.model,
            modelMetaData: modelMetaData
        }

        return command;
    }

    function redirectToList() {
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
angular.module('cms.customEntities').controller('CustomEntityListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.customEntityService',
    'shared.permissionValidationService',
    'customEntities.modulePath',
    'customEntities.options',
function (
    _,
    LoadState,
    SearchQuery,
    modalDialogService,
    customEntityService,
    permissionValidationService,
    modulePath,
    options) {

    var vm = this;

    init();

    function init() {
        
        vm.options = options;
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.changeOrdering = changeOrdering;

        vm.permissions = permissionValidationService;
        vm.canUpdate = getPermission('COMUPD');
        vm.canCreate = getPermission('COMCRT');

        toggleFilter(false);

        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function changeOrdering() {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/ChangeOrdering.html',
            controller: 'ChangeOrderingController',
            options: {
                localeId: vm.filter.localeId,
                onSave: loadGrid
            }
        });
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* PRIVATE FUNCS */

    function getPermission(code) {
        return permissionValidationService.hasPermission(options.customEntityDefinitionCode + code);
    }
    
    function loadGrid() {
        vm.gridLoadState.on();

        return customEntityService.getAll(vm.query.getParameters(), options.customEntityDefinitionCode).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);
angular.module('cms.customEntities').controller('ChangeOrderingController', [
    '$scope',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.arrayUtilities',
    'shared.internalModulePath',
    'shared.modalDialogService',
    'shared.customEntityService',
    'customEntities.options',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    _,
    LoadState,
    arrayUtilities,
    sharedModulePath,
    modalDialogService,
    customEntityService,
    customEntityOptions,
    options,
    close) {

    var CUSTOM_ENTITY_ID_PROP = 'customEntityId';

    init();
    
    /* INIT */

    function init() {
        $scope.options = customEntityOptions;
        $scope.command = {
            localeId: options.localeId,
            customEntityDefinitionCode: customEntityOptions.customEntityDefinitionCode
        }
        $scope.isPartialOrdering = customEntityOptions.ordering === 'Partial';
        
        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);
        $scope.gridLoadState = new LoadState();

        $scope.save = save;
        $scope.close = close;
        $scope.setStep = setStep;
        $scope.onLocalesLoaded = onLocalesLoaded;
        $scope.onDrop = onDrop;
        $scope.remove = remove;
        $scope.showPicker = showPicker;

        initState();
    }

    /* EVENTS */

    function save() {
        $scope.command.orderedCustomEntityIds = getSelectedIds();

        $scope.submitLoadState.on();

        customEntityService
            .updateOrdering($scope.command)
            .then(options.onSave)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

    function onLocalesLoaded() {
        $scope.formLoadState.off();
    }

    function onDrop($index, droppedEntity) {

        arrayUtilities.moveObject($scope.gridData, droppedEntity, $index, CUSTOM_ENTITY_ID_PROP);
    }

    function remove(customEntity) {

        arrayUtilities.removeObject($scope.gridData, customEntity);
    }

    function showPicker() {
        modalDialogService.show({
            templateUrl: sharedModulePath + 'UIComponents/CustomEntities/CustomEntityPickerDialog.html',
            controller: 'CustomEntityPickerDialogController',
            options: {
                selectedIds: getSelectedIds(),
                customEntityDefinition: customEntityOptions,
                filter: {
                    localeId: $scope.command.localeId
                },
                onSelected: onSelected
            }
        });

        function onSelected(newEntityArr) {

            if (!newEntityArr || !newEntityArr.length) {
                $scope.gridData = [];
            }
            else {
                // Remove deselected
                var entitiesToRemove = _.filter($scope.gridData, function(entity) {
                    return !_.contains(newEntityArr, entity[CUSTOM_ENTITY_ID_PROP]);
                });

                $scope.gridData = _.difference($scope.gridData, entitiesToRemove);

                // Add new
                var newIds = _.difference(newEntityArr, getSelectedIds());

                if (newIds.length) {
                    $scope.gridLoadState.on();
                    
                    customEntityService.getByIdRange(newIds).then(function (items) {
                        $scope.gridData = _.union($scope.gridData, items);
                        $scope.gridLoadState.off();
                    });
                }
            }
        }
    }

    /* HELPERS */

    function getSelectedIds() {
        return _.pluck($scope.gridData, CUSTOM_ENTITY_ID_PROP);
    }

    function initState() {
        if (customEntityOptions.hasLocale) {
            $scope.allowStep1 = true;
            setStep(1);
        } else {
            setStep(2);
            $scope.formLoadState.off();
        }
    }

    function setStep(step) {
        $scope.currentStep = step;

        if (step === 2) {
            loadStep2();
        }
    }

    function loadStep2() {
        $scope.formLoadState.on();

        var query = {
            pageSize: 60,
            localeId: $scope.command.localeId,
            interpretNullLocaleAsNone: true
        }

        customEntityService
            .getAll(query, customEntityOptions.customEntityDefinitionCode)
            .then(onLoaded);

        function onLoaded(result) {

            if ($scope.isPartialOrdering) {
                $scope.gridData = _.filter(result.items, function (entity) {
                    return !!entity.ordering;
                });

            } else {
                $scope.gridData = result.items;
            }
            $scope.formLoadState.off();
        }
    }

}]);
angular.module('cms.customEntities').controller('ChangeUrlController', [
    '$scope',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.customEntityService',
    'customEntities.options',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    LoadState,
    customEntityService,
    customEntityOptions,
    options,
    close) {

    var loadLocalesDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {
        var customEntity = options.customEntity;

        $scope.options = customEntityOptions;
        $scope.customEntity = customEntity;
        $scope.command = {
            customEntityId: customEntity.customEntityId,
            localeId: customEntity.locale ? customEntity.locale.localeId : undefined,
            urlSlug: customEntity.urlSlug
        }

        $scope.submitLoadState = new LoadState();
        // Only the locales need loading at start, and only if the CE uses locales
        $scope.formLoadState = new LoadState(customEntityOptions.hasLocale);

        $scope.save = save;
        $scope.close = close;
        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred);
    }

    /* EVENTS */

    function save() {
        $scope.submitLoadState.on();

        customEntityService
            .updateUrl($scope.command)
            .then(options.onSave)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

}]);