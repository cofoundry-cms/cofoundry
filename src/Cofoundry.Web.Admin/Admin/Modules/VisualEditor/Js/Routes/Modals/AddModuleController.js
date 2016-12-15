angular.module('cms.visualEditor').controller('AddModuleController', [
    '$scope',
    '$q',
    '_',
    'shared.LoadState',
    'visualEditor.pageModuleService',
    'visualEditor.options',
    'options',
    'close',
function (
    $scope,
    $q,
    _,
    LoadState,
    pageModuleService,
    visualEditorOptions,
    options,
    close) {

    init();
    
    /* INIT */

    function init() {
        var anchorElement = options.anchorElement;

        $scope.command = { 
            dataModel: {},
            pageTemplateSectionId: options.pageTemplateSectionId,
            pageVersionId: visualEditorOptions.pageVerisonId,
            adjacentVersionModuleId: options.adjacentVersionModuleId,
            insertMode: options.insertMode || 'Last'
        };

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = onClose;
        $scope.selectModuleType = selectModuleType;
        $scope.selectModuleTypeAndNext = selectModuleTypeAndNext;
        $scope.isModuleTypeSelected = isModuleTypeSelected;
        $scope.setStep = setStep;

        initData();
    }

    /* EVENTS */

    function initData() {
        setStep(1);

        pageModuleService
            .getAllModuleTypes()
            .then(onLoaded);

        function onLoaded(allModuleTypes) {;

            $scope.title = options.sectionName;

            if (options.permittedModuleTypes.length) {
                // Filter the permitted modules list to those specified
                $scope.moduleTypes = _.filter(allModuleTypes, function (moduleType) {
                    return _.contains(options.permittedModuleTypes, moduleType.fileName);
                });
            } else {
                // Empty means 'all' module types
                $scope.moduleTypes = allModuleTypes;
            }

            if ($scope.moduleTypes.length === 1) {
                $scope.command.pageModuleTypeId = $scope.moduleTypes[0].pageModuleTypeId;
                setStep(2);
            } else {
                $scope.allowStep1 = true;
            }

            $scope.formLoadState.off();
        }
    }

    function save() {

        $scope.submitLoadState.on();

        pageModuleService
            .add(options.isCustomEntity, $scope.command)
            .then(options.refreshContent)
            .then(onClose)
            .finally($scope.submitLoadState.off);
    }

    function onClose() {
        close();
        options.onClose();
    }

    function setStep(step) {
        $scope.currentStep = step;

        if (step === 2) {
            loadStep2();
        }
    }

    function loadStep2() {
        $scope.formLoadState.on();

        pageModuleService
            .getModuleTypeSchema($scope.command.pageModuleTypeId)
            .then(onLoaded);

        function onLoaded(modelMetaData) {

            $scope.formDataSource = {
                modelMetaData: modelMetaData,
                model: $scope.command.dataModel
            };

            $scope.templates = modelMetaData.templates;

            $scope.formLoadState.off();
        }
    }

    function selectModuleType(moduleType) {
        $scope.command.pageModuleTypeId = moduleType && moduleType.pageModuleTypeId;
    }

    function selectModuleTypeAndNext(moduleType) {
        selectModuleType(moduleType);
        setStep(2);
    }

    /* PUBLIC HELPERS */

    function isModuleTypeSelected(moduleType) {
        return moduleType && moduleType.pageModuleTypeId === $scope.command.pageModuleTypeId;
    }

}]);