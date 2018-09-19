angular.module('cms.pageTemplates').controller('PageTemplateDetailsController', [
    '$routeParams',
    '$location',
    'shared.LoadState',
    'shared.urlLibrary',
    'pageTemplates.pageTemplateService',
    'pageTemplates.modulePath',
function (
    $routeParams,
    $location,
    LoadState,
    urlLibrary,
    pageTemplateService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // Properties
        vm.urlLibrary = urlLibrary;
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* PRIVATE FUNCS */

    function initData() {
        var pageTemplateId = $routeParams.id;

        return pageTemplateService.getById(pageTemplateId)
            .then(load);

        function load(pageTemplate) {

            vm.pageTemplate = pageTemplate;
            vm.command = mapUpdateCommand(pageTemplate);
            vm.editMode = false;
        }
    }

    function mapUpdateCommand(pageTemplate) {

        return _.pick(pageTemplate,
            'pageTemplateId',
            'name',
            'description'
            );
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