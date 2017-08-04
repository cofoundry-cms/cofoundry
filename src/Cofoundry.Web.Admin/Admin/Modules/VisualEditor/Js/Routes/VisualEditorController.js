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
        vm[e.data.action].apply(this, e.data.args);
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
            .then(preview)
            .catch(setLoadingOff);
    }

    function unpublish(args) {
        entityVersionModalDialogService
            .unpublish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function copyToDraft(args) {
        entityVersionModalDialogService
            .copyToDraft(args.entityId, args.versionId, args.hasDraftVersion, setLoadingOn, entityDialogServiceConfig)
            .then(reloadWithoutVersion)
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
                onClose: onClose,
                refreshContent: refreshRegion,
                isCustomEntity: args.isCustomEntity,
                regionName: args.regionName
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

    function reloadWithoutVersion() {
        var location = $window.parent.location.href;
        // remove the version query parameter
        location = location.replace(/(.+\?(?:.+&|))(version=\d+&?)(.*)/i, '$1$3');
        $window.parent.location = location
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);