angular.module('cms.pageTemplates').controller('PageTemplateDetailsController', [
    '$routeParams',
    '$location',
    'shared.LoadState',
    'shared.modalDialogService',
    'pageTemplates.pageTemplateService',
    'pageTemplates.modulePath',
function (
    $routeParams,
    $location,
    LoadState,
    modalDialogService,
    pageTemplateService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.deleteLayout = deleteLayout;
        vm.editModuleTypes = editModuleTypes;
        vm.rescanFile = rescanFile;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        pageTemplateService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand(vm.pageTemplate);
        vm.mainForm.formStatus.clear();
    }

    function deleteLayout() {
        var options = {
            title: 'Delete Page Template',
            message: 'Are you sure you want to delete this page template?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return pageTemplateService
                .remove(vm.pageTemplate.pageTemplateId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    function rescanFile() {
        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/ReScanTemplateSections.html',
            controller: 'ReScanTemplateSectionsController',
            options: {
                pageTemplate: vm.pageTemplate
            }
        }).then(function (modal) {
            modal.close.then(initData);
        });
    }

    function editModuleTypes(section) {

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/editmoduletypes.html',
            controller: 'EditModuleTypesController',
            options: {
                section: section
            }
        }).then(function(modal) {
            modal.close.then(initData);
        });
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData() {
        var pageTemplateId = $routeParams.id;

        return pageTemplateService.getById(pageTemplateId)
            .then(load);

        function load(pageTemplate) {

            vm.pageTemplate = pageTemplate;
            vm.command = mapUpdateCommand(pageTemplate);
            vm.editMode = false;

            if (pageTemplate.numPages) {
                vm.deleteTitle = "This page template cannot be deleted because it is in use.";
            }
        }
    }

    function mapUpdateCommand(pageTemplate) {

        return _.pick(pageTemplate,
            'pageTemplateId',
            'name',
            'description'
            );
    }

    function redirectToList() {
        $location.path('');
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
        if (loadState && _.isFunction(loadState.on)) loadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
        if (loadState && _.isFunction(loadState.off)) loadState.off();
    }
}]);