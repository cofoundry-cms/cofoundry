angular.module('cms.visualEditor').controller('EditModuleController', [
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
            dataModel: {}
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
            moduleTypeSchemaDeferred, moduleDataDeferred;

        $scope.formLoadState.on();

        moduleTypeSchemaDeferred = pageModuleService
            .getModuleTypeSchema(options.pageModuleTypeId)
            .then(onMetaDataLoaded);

        moduleDataDeferred = pageModuleService
            .getPageVersionModuleById(options.isCustomEntity, options.versionModuleId)
            .then(onModelLoaded);

        var q = $q.all([moduleTypeSchemaDeferred, moduleDataDeferred]).then(onLoadComplete);

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

        pageModuleService
            .update(options.isCustomEntity, options.versionModuleId, $scope.command)
            .then(options.refreshContent)
            .then(onClose)
            .finally($scope.submitLoadState.off);
    }

    function onClose() {
        close();
        options.onClose();
    }
    
}]);