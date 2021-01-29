angular
    .module('cms.visualEditor', ['cms.shared'])
    .constant('_', window._)
    .constant('visualEditor.modulePath', '/Admin/Modules/VisualEditor/Js/');
/**
 * Service for managing page block, which can either be attached to a page or a custom entity.
 * Pass in the isCustomEntityRoute to switch between either route endpoint.
 */
angular.module('cms.visualEditor').factory('visualEditor.pageBlockService', [
    '$http',
    'shared.serviceBase',
    'visualEditor.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        pageBlocksServiceBase = serviceBase + 'page-version-region-blocks',
        customEntityBlocksServiceBase = serviceBase + 'custom-entity-version-page-blocks';

    /* QUERIES */

    service.getAllBlockTypes = function () {
        return $http.get(serviceBase + 'page-block-types/');
    }

    service.getPageVersionBlockById = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.get(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '?datatype=updatecommand');
    }   

    service.getRegion = function (pageRegionId) {
        return $http.get(serviceBase + 'page-templates/0/regions/' + pageRegionId);
    }

    service.getBlockTypeSchema = function (pageBlockTypeId) {
        return $http.get(serviceBase + 'page-block-types/' + pageBlockTypeId);
    }

    /* COMMANDS */

    service.add = function (isCustomEntityRoute, command) {
        var entityName = isCustomEntityRoute ? 'customEntity' : 'page';
        command[entityName + 'VersionId'] = options.versionId;

        return $http.post(getServiceBase(isCustomEntityRoute), command);
    }

    service.update = function (isCustomEntityRoute, pageVersionBlockId, command) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId), command);
    }

    service.remove = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.delete(getIdRoute(isCustomEntityRoute, pageVersionBlockId));
    }

    service.moveUp = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-up');
    }

    service.moveDown = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-down');
    }

    /* HELPERS */

    function getIdRoute(isCustomEntityRoute, pageVersionBlockId) {
        return getServiceBase(isCustomEntityRoute) + '/' + pageVersionBlockId;
    }

    function getServiceBase(isCustomEntityRoute) {
        return isCustomEntityRoute ? customEntityBlocksServiceBase : pageBlocksServiceBase;
    }

    return service;
}]);
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
angular.module('cms.visualEditor').controller('VisualEditorController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'shared.modalDialogService',
    'shared.localStorage',
    'visualEditor.pageBlockService',
    'visualEditor.modulePath',
    'shared.urlLibrary',
    'visualEditor.options',
function (
    $window,
    $scope,
    _,
    LoadState,
    entityVersionModalDialogService,
    modalDialogService,
    localStorageService,
    pageBlockService,
    modulePath,
    urlLibrary,
    options
    ) {

    var vm = this,
        document = $window.document,
        entityDialogServiceConfig,
        globalLoadState = new LoadState();

    init();

    /* INIT */

    function init() {
        // Create IE + others compatible event handler
        var eventMethod = $window.addEventListener ? "addEventListener" : "attachEvent",
            postMessageListener = window[eventMethod],
            messageEvent = eventMethod === "attachEvent" ? "onmessage" : "message";

        postMessageListener(messageEvent, handleMessage);

        vm.globalLoadState = globalLoadState;
        vm.config = config;
        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.copyToDraft = copyToDraft;
        vm.addRegionBlock = addRegionBlock;
        vm.addBlock = addBlock;
        vm.addBlockAbove = addBlock;
        vm.addBlockBelow = addBlock;
        vm.editBlock = editBlock;
        vm.moveBlockUp = moveBlock;
        vm.moveBlockDown = moveBlock;
        vm.deleteBlock = deleteBlock;
    }

    /* UI ACTIONS */

    function handleMessage(e) {
        if (e.data.action && vm[e.data.action]) {
            vm[e.data.action].apply(this, e.data.args);
        }
    }

    function config() {
        entityDialogServiceConfig = {
            entityNameSingular: options.entityNameSingular,
            isCustomEntity: options.isCustomEntityRoute
        };
    }

    function publish(args) {
        entityVersionModalDialogService
            .publish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reloadDefaultMode)
            .catch(setLoadingOff);
    }

    function unpublish(args) {
        entityVersionModalDialogService
            .unpublish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reloadDefaultMode)
            .catch(setLoadingOff);
    }

    function copyToDraft(args) {
        entityVersionModalDialogService
            .copyToDraft(args.entityId, args.versionId, args.hasDraftVersion, setLoadingOn, entityDialogServiceConfig)
            .then(reloadDefaultMode)
            .catch(setLoadingOff);
    }

    function addRegionBlock(args) {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
            controller: 'AddBlockController',
            options: {
                insertMode: args.insertMode,
                pageTemplateRegionId: args.pageTemplateRegionId,
                adjacentVersionBlockId: args.versionBlockId,
                permittedBlockTypes: args.permittedBlockTypes,
                isCustomEntity: args.isCustomEntity,
                regionName: args.regionName,
                pageId: args.pageId,
                onClose: onClose,
                refreshContent: refreshRegion
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function addBlock(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
            controller: 'AddBlockController',
            options: {
                pageTemplateRegionId: args.pageTemplateRegionId,
                adjacentVersionBlockId: args.versionBlockId,
                permittedBlockTypes: args.permittedBlockTypes,
                insertMode: args.insertMode,
                refreshContent: refreshRegion,
                isCustomEntity: args.isCustomEntity,
                pageId: args.pageId,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function editBlock(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/EditBlock.html',
            controller: 'EditBlockController',
            options: {
                versionBlockId: args.versionBlockId,
                pageBlockTypeId: args.pageBlockTypeId,
                isCustomEntity: args.isCustomEntity,
                refreshContent: refreshRegion,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function moveBlock(args) {
        var fn = args.isUp ? pageBlockService.moveUp : pageBlockService.moveDown;

        if (globalLoadState.isLoading) return;

        globalLoadState.on();

        fn(args.isCustomEntity, args.versionBlockId)
            .then(refreshRegion)
            .finally(globalLoadState.off);
    }

    function deleteBlock(args) {
        var isCustomEntity = args.isCustomEntity,
            options = {
                title: 'Delete Block',
                message: 'Are you sure you want to delete this content block?',
                okButtonTitle: 'Yes, delete it',
                onOk: onOk,
                onCancel: onCancel
            };

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.confirm(options);

        function onOk() {
            return pageBlockService
                .remove(isCustomEntity, args.versionBlockId)
                .then(refreshRegion)
                .finally(globalLoadState.off);
        }

        function onCancel() {
            globalLoadState.off();
        }
    }

    /* PRIVATE FUNCS */

    function refreshRegion() {
        reload();
    }

    function preview() {
        var location = $window.parent.location.href;
        if (location.indexOf('mode=edit') > -1) {
            location = location.replace('mode=edit', 'mode=preview');
        }
        $window.parent.location = location;
    }

    function reload() {
        $window.parent.location = $window.parent.location;
    }

    function reloadDefaultMode() {
        var location = filterQueryParamsFromUrl($window.parent.location.href, ['version', 'mode']);

        $window.parent.location = location;
    }

    /**
     * Removes the specified querystring parameters from a url
     */
    function filterQueryParamsFromUrl(url, paramsToFilter) {
        var queryMatches = url.match(/(.+)(?:\?)([^#\s]*)(#.*|)/i),
            validParams = [],
            validParamsResult = '';

        if (!queryMatches) return url;

        // parse and strip out any unwanted qs parameters
        _.each(queryMatches[2].split('&'), function (queryParam) {
            if (!queryParam) return;

            var canAdd = _.every(paramsToFilter, function (paramToFilter) {
                return queryParam.indexOf(paramToFilter + '=') === -1;
            });

            if (canAdd) {
                validParams.push(queryParam);
            }
        });

        if (validParams.length) {
            validParamsResult = '?' + validParams.join('&');
        }

        // re-build the url without the parameters
        url = queryMatches[1] + validParamsResult + queryMatches[3];

        return url;
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);