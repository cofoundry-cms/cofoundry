angular.module('cms.visualEditor').controller('EditBlockController', [
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
            pageId: options.pageId
        };

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = onClose;

        initData();
    }

    /* EVENTS */

    function initData() {
        var formDataSource = {},
            blockTypeSchemaDeferred, blockDataDeferred;

        $scope.formLoadState.on();

        blockTypeSchemaDeferred = pageBlockService
            .getBlockTypeSchema(options.pageBlockTypeId)
            .then(onMetaDataLoaded);

        blockDataDeferred = pageBlockService
            .getPageVersionBlockById(options.isCustomEntity, options.versionBlockId)
            .then(onModelLoaded);

        var q = $q.all([blockTypeSchemaDeferred, blockDataDeferred]).then(onLoadComplete);

        function onMetaDataLoaded(modelMetaData) {
            $scope.templates = modelMetaData.templates;
            formDataSource.modelMetaData = modelMetaData;
        }

        function onModelLoaded(model) {
            $scope.command = model;
            formDataSource.model = model.dataModel;
        }

        function onLoadComplete() {

            $scope.formDataSource = formDataSource;
            $scope.formLoadState.off();
        }
    }

    function save() {

        $scope.submitLoadState.on();

        pageBlockService
            .update(options.isCustomEntity, options.versionBlockId, $scope.command)
            .then(options.refreshContent)
            .then(onClose)
            .finally($scope.submitLoadState.off);
    }

    function onClose() {
        close();
        options.onClose();
    }
    
}]);