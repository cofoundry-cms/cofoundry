angular.module('cms.visualEditor').controller('VisualEditorController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
<<<<<<< HEAD
    'shared.modalDialogService',
    'shared.localStorage',
    'visualEditor.pageModuleService',
=======
    'shared.localStorage',
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07
    'visualEditor.modulePath',
    'visualEditor.options',
function (
    $window,
    $scope,
    _,
    LoadState,
    entityVersionModalDialogService,
<<<<<<< HEAD
    modalDialogService,
    localStorageService,
    pageModuleService,
=======
    localStorageService,
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07
    modulePath,
    options
    ) {

    var vm = this,
        document = $window.document,
<<<<<<< HEAD
        entityDialogServiceConfig,
        globalLoadState = new LoadState();
=======
        entityDialogServiceConfig;
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07

    init();

    /* INIT */

    function init() {
        // Create IE + others compatible event handler
        var eventMethod = $window.addEventListener ? "addEventListener" : "attachEvent",
            postMessageListener = window[eventMethod],
            messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

        postMessageListener(messageEvent, handleMessage);

<<<<<<< HEAD
        vm.globalLoadState = globalLoadState;
=======
        vm.globalLoadState = new LoadState();
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07
        vm.config = config;
        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.copyToDraft = copyToDraft;
<<<<<<< HEAD
        vm.addSectionModule = addSectionModule;
        vm.addModule = addModule;
        vm.addModuleAbove = addModule;
        vm.addModuleBelow = addModule;
        vm.editModule = editModule;
        vm.moveModuleUp = moveModule;
        vm.moveModuleDown = moveModule;
        vm.deleteModule = deleteModule;
=======
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07
    }

    /* UI ACTIONS */

    function handleMessage(e) {
        vm[e.data.action].apply(this, e.data.args);
    }

    function config(isCustomEntityRoute, entityNameSingular) {
        entityDialogServiceConfig = {
            entityNameSingular: entityNameSingular,
            isCustomEntity: isCustomEntityRoute
        };
    }

    function publish(args) {
        entityVersionModalDialogService
            .publish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
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
            .then(reload)
            .catch(setLoadingOff);
    }

<<<<<<< HEAD
    function addSectionModule(args) {
        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/addmodule.html',
            controller: 'AddModuleController',
            options: {
                pageTemplateSectionId: args.pageTemplateSectionId,
                onClose: onClose,
                refreshContent: refreshSection,
                isCustomEntity: args.isCustomEntity,
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function addModule(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        //scope.isPopupActive = true;

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/addmodule.html',
            controller: 'AddModuleController',
            options: {
                pageTemplateSectionId: args.pageTemplateSectionId,
                adjacentVersionModuleId: args.versionModuleId,
                insertMode: args.insertMode,
                refreshContent: refreshSection,
                isCustomEntity: args.isCustomEntity,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function editModule(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();
        //scope.isPopupActive = true;

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/editmodule.html',
            controller: 'EditModuleController',
            options: {
                versionModuleId: args.versionModuleId,
                pageModuleTypeId: args.pageModuleTypeId,
                isCustomEntity: args.isCustomEntity,
                refreshContent: refreshSection,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function moveModule(args) {
        var fn = args.isUp ? pageModuleService.moveUp : pageModuleService.moveDown;

        if (globalLoadState.isLoading) return;

        globalLoadState.on();

        fn(args.isCustomEntity, args.versionModuleId)
            .then(refreshSection)
            .finally(globalLoadState.off);
    }

    function deleteModule(args) {
        var isCustomEntity = args.isCustomEntity,
            options = {
                title: 'Delete Module',
                message: 'Are you sure you want to delete this module?',
                okButtonTitle: 'Yes, delete it',
                onOk: onOk
            };

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.confirm(options);

        function onOk() {
            return pageModuleService
                .remove(isCustomEntity, args.versionModuleId)
                .then(refreshSection)
                .finally(globalLoadState.off);
        }
    }

    /* PRIVATE FUNCS */

    function refreshSection() {
        reload();
    }

    function reload() {
        $window.parent.location = $window.parent.location;
=======
    /* PRIVATE FUNCS */

    function reload() {
        $window.location = $window.location.pathname;
>>>>>>> 933c7fc0845629bb31aaffb54e147e5ca5b8bc07
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);