angular.module('cms.shared').controller('CustomEntityPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.SearchQuery',
    'options',
    'close',
function (
    $scope,
    LoadState,
    customEntityService,
    SearchQuery,
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

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return customEntityService.getAll(options.customEntityDefinition.customEntityDefinitionCode, vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
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

    /* PUBLIC HELPERS */

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
