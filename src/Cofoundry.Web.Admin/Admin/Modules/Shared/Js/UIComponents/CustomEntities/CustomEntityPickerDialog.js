angular.module('cms.shared').controller('CustomEntityPickerDialogController', [
    '$scope',
    '$q',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'shared.permissionValidationService',
    'shared.ModelPreviewFieldset',
    'shared.ImagePreviewFieldCollection',
    'options',
    'close',
function (
    $scope,
    $q,
    LoadState,
    customEntityService,
    SearchQuery,
    modalDialogService,
    modulePath,
    permissionValidationService,
    ModelPreviewFieldset,
    ImagePreviewFieldCollection,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.onCreate = onCreate;
        vm.selectedEntity = vm.currentEntity; // current entity is null in single mode
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: options.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.isSelected = isSelected;
        vm.customEntityDefinition = options.customEntityDefinition;
        vm.multiMode = vm.selectedIds ? true : false;

        vm.canCreate = getPermission('COMCRT');

        toggleFilter(false);
        reloadData();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        reloadData();
    }

    function reloadData() {

        var metaDataDef,
            definitionCode = options.customEntityDefinition.customEntityDefinitionCode,
            gridDef = loadGrid();

        if (vm.previewFields) {
            metaDataDef = $q.defer();
            metaDataDef.resolve();
        } else {
            metaDataDef = getMetaData().then(loadMetaData);
        }

        return $q
            .all([metaDataDef, gridDef])
            .then(loadImages);

        function loadGrid() {
            vm.gridLoadState.on();

            return customEntityService.getAll(vm.query.getParameters(), definitionCode).then(function (result) {
                vm.result = result;
                vm.gridLoadState.off();
            });
        }

        function getMetaData() {
            return customEntityService.getDataModelSchema(definitionCode);
        }

        function loadMetaData(modelMetaData) {
            vm.previewFields = new ModelPreviewFieldset(modelMetaData);
        }

        function loadImages() {
            vm.gridImages = new ImagePreviewFieldCollection();
            return vm.gridImages.load(vm.result.items, vm.previewFields);
        }
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the entity
            vm.onSelected(vm.currentEntity);
        }
        close();
    }

    function onSelect(entity) {
        if (!vm.multiMode) {
            vm.selectedEntity = entity;
            return;
        }

        addOrRemove(entity);
    }

    function onSelectAndClose(entity) {
        if (!vm.multiMode) {
            vm.selectedEntity = entity;
            onOk();
            return;
        }

        addOrRemove(entity);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedEntity);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    function onCreate() {
        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/CustomEntities/AddCustomEntityDialog.html',
            controller: 'AddCustomEntityDialogController',
            options: {
                customEntityDefinition: options.customEntityDefinition,
                onComplete: onComplete
            }
        });

        function onComplete(customEntityId) {
            if (vm.multiMode) {
                onSelect({ customEntityId: customEntityId });
                reloadData();
            } else {
                onSelectAndClose({ customEntityId: customEntityId });
            }
        }
    }

    /* PUBLIC HELPERS */

    function getPermission(code) {
        return permissionValidationService.hasPermission(options.customEntityDefinition.customEntityDefinitionCode + code);
    }

    function isSelected(entity) {
        if (vm.selectedIds && entity && vm.selectedIds.indexOf(entity.customEntityId) > -1) return true;

        if (!entity || !vm.selectedEntity) return false;
        
        return entity.customEntityId === vm.selectedEntity.customEntityId;
    }

    function addOrRemove(entity) {
        if (!isSelected(entity)) {
            vm.selectedIds.push(entity.customEntityId);
        } else {
            var index = vm.selectedIds.indexOf(entity.customEntityId);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);
