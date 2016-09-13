angular.module('cms.pageTemplates').controller('AddPageTemplateController', [
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'pageTemplates.pageTemplateService',
    'pageTemplates.modulePath',
function (
    $location,
    stringUtilities,
    LoadState,
    SearchQuery,
    modalDialogService,
    pageTemplateService,
    modulePath) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        vm.globalLoadState = new LoadState();
        vm.gridLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        
        vm.step2LoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.editMode = false;

        vm.save = save;
        vm.cancel = cancel;
        vm.setStep = setStep;
        vm.onFileClick = onFileClick;
        vm.onFileDblClick = onFileDblClick;
        vm.editModuleTypes = editModuleTypes;

        vm.isFileSelected = isFileSelected;

        loadFileGrid();
        setStep(1);
    }

    /* EVENTS */

    function save() {
        setLoadingOn(vm.saveLoadState);
        addSectionsToCommand();

        pageTemplateService
            .add(vm.command)
            .then(redirectToList)
            .finally(setLoadingOn.bind(vm.saveLoadState, null));
    }

    function cancel() {
        redirectToList();
    }

    function onQueryChanged() {
        loadFileGrid();
    }

    function setStep(step) {
        vm.currentStep = step;

        if (step === 2) {
            loadStep2();
        }
    }

    function onFileClick(file) {
        if (!isFileSelected(file)) {
            vm.selectedFile = file;
        }
    }

    function onFileDblClick(file) {
        vm.selectedFile = file;
        setStep(2);
    }

    function editModuleTypes(section) {

        modalDialogService.show({
            templateUrl: modulePath + 'routes/modals/editmoduletypes.html',
            controller: 'EditModuleTypesController',
            options: {
                section: section
            }
        });
    }

    /* PUBLIC HELPERS */

    function isFileSelected(file) {
        if (!file || !vm.selectedFile) return false;

        return file.fullPath === vm.selectedFile.fullPath;
    }

    /* PRIVATE FUNCS */

    function loadStep2() {

        vm.step2LoadState.on();

        return pageTemplateService.parseFile(vm.selectedFile.fullPath).then(function (fileInfo) {
            mapCommand(fileInfo);

            _.each(fileInfo.sections, function (section) {
                section.hasAllModuleTypes = true;
            });

            vm.fileInfo = fileInfo;
            vm.step2LoadState.off();
        });
    }

    function loadFileGrid() {

        vm.gridLoadState.on();

        return pageTemplateService.getFiles(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

    function redirectToList() {
        $location.path('/');
    }

    function mapCommand(fileInfo) {

        vm.command = _.pick(vm.selectedFile || {},
            'name',
            'fullPath'
            );

        vm.command.pageType = fileInfo.pageType;
        vm.command.CustomEntityModelType = fileInfo.customEntityModelType;
    }

    /**
     * Parse the sections collection into commands to send to the server
     * and add them to the main command.
     */
    function addSectionsToCommand() {
        vm.command.sections = _.map(vm.fileInfo.sections, function (section) {
            var addSectionCommand = {
                name: section.name,
                permitAllModuleTypes: section.hasAllModuleTypes
            };

            if (!addSectionCommand.hasAllModuleTypes) {
                addSectionCommand.permittedModuleTypeIds = _.map(section.moduleTypes, function (moduleType) {
                    return moduleType.pageModuleTypeId
                });
            }
            
            return addSectionCommand;
        });
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