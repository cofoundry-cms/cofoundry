angular.module('cms.visualEditor').controller('AddBlockController', [
    '$scope',
    '$q',
    '_',
    'shared.LoadState',
    'visualEditor.pageBlockService',
    'visualEditor.options',
    'options',
    'close',
function (
    $scope,
    $q,
    _,
    LoadState,
    pageBlockService,
    visualEditorOptions,
    options,
    close) {

    init();
    
    /* INIT */

    function init() {
        var anchorElement = options.anchorElement;

        $scope.command = { 
            dataModel: {},
            pageId: options.pageId,
            pageTemplateRegionId: options.pageTemplateRegionId,
            pageVersionId: visualEditorOptions.pageVerisonId,
            adjacentVersionBlockId: options.adjacentVersionBlockId,
            insertMode: options.insertMode || 'Last'
        };

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = onClose;
        $scope.selectBlockType = selectBlockType;
        $scope.selectBlockTypeAndNext = selectBlockTypeAndNext;
        $scope.isBlockTypeSelected = isBlockTypeSelected;
        $scope.setStep = setStep;

        initData();
    }

    /* EVENTS */

    function initData() {
        setStep(1);

        pageBlockService
            .getAllBlockTypes()
            .then(onLoaded);

        function onLoaded(allBlockTypes) {;
            $scope.title = options.regionName;

            if (options.permittedBlockTypes.length) {
                // Filter the permitted blocks list to those specified
                $scope.blockTypes = _.filter(allBlockTypes, function (blockType) {
                    return _.contains(options.permittedBlockTypes, blockType.fileName);
                });
            } else {
                // Empty means 'all' block types
                $scope.blockTypes = allBlockTypes;
            }

            if ($scope.blockTypes.length === 1) {
                $scope.command.pageBlockTypeId = $scope.blockTypes[0].pageBlockTypeId;
                setStep(2);
            } else {
                $scope.allowStep1 = true;
            }

            $scope.formLoadState.off();
        }
    }

    function save() {

        $scope.submitLoadState.on();

        pageBlockService
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

        pageBlockService
            .getBlockTypeSchema($scope.command.pageBlockTypeId)
            .then(onLoaded);

        function onLoaded(modelMetaData) {

            if (modelMetaData.defaultValue && modelMetaData.defaultValue.value) {
                _.defaults($scope.command.dataModel, angular.copy(modelMetaData.defaultValue.value));
            }

            $scope.formDataSource = {
                modelMetaData: modelMetaData,
                model: $scope.command.dataModel
            };

            $scope.templates = modelMetaData.templates;

            $scope.formLoadState.off();
        }
    }

    function selectBlockType(blockType) {
        $scope.command.pageBlockTypeId = blockType && blockType.pageBlockTypeId;
    }

    function selectBlockTypeAndNext(blockType) {
        selectBlockType(blockType);
        setStep(2);
    }

    /* PUBLIC HELPERS */

    function isBlockTypeSelected(blockType) {
        return blockType && blockType.pageBlockTypeId === $scope.command.pageBlockTypeId;
    }

}]);