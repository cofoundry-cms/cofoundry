angular.module('cms.customEntities').controller('CustomEntityListController', [
    '$q',
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.customEntityService',
    'shared.permissionValidationService',
    'shared.ModelPreviewFieldset',
    'shared.ImagePreviewFieldCollection',
    'customEntities.modulePath',
    'customEntities.options',
function (
    $q,
    _,
    LoadState,
    SearchQuery,
    modalDialogService,
    customEntityService,
    permissionValidationService,
    ModelPreviewFieldset,
    ImagePreviewFieldCollection,
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

        reloadData();
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
                onSave: reloadData
            }
        });
        
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        reloadData();
    }

    /* PRIVATE FUNCS */

    function getPermission(code) {
        return permissionValidationService.hasPermission(options.customEntityDefinitionCode + code);
    }

    function reloadData() {

        var metaDataDef,
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

            return customEntityService
                .getAll(vm.query.getParameters(), options.customEntityDefinitionCode)
                .then(function (result) {

                    vm.result = result;
                    vm.gridLoadState.off();
                });
        }

        function getMetaData() {
            return customEntityService.getDataModelSchema(options.customEntityDefinitionCode);
        }

        function loadMetaData(modelMetaData) {
            vm.previewFields = new ModelPreviewFieldset(modelMetaData);
        }

        function loadImages() {
            vm.gridImages = new ImagePreviewFieldCollection();
            return vm.gridImages.load(vm.result.items, vm.previewFields);
        }
    }

}]);