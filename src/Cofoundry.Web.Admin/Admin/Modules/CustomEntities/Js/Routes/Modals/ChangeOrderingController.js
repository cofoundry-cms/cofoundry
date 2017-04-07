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