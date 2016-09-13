angular.module('cms.pages').controller('AddPageController', [
    '_',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.stringUtilities',
    'pages.pageService',
    'pages.pageTemplateService',
    'pages.customEntityService',
function (
    _,
    $q,
    $location,
    LoadState,
    stringUtilities,
    pageService,
    pageTemplateService,
    customEntityService
    ) {

    var vm = this,
        loadLocalesDeferred = $q.defer(),
        loadWebDirectoryDeferred = $q.defer();

    init();

    /* INIT */

    function init() {

        vm.save = save.bind(null, false);
        vm.saveAndPublish = save.bind(null, true);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;
        vm.onPageTypeChanged = onPageTypeChanged;

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.onLocalesLoaded = loadLocalesDeferred.resolve;
        vm.onWebDirectoriesLoaded = loadWebDirectoryDeferred.resolve;

        initData();
    }

    /* EVENTS */

    function save(publish) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        pageService
            .add(vm.command)
            .then(redirectToDetails)
            .finally(setLoadingOff.bind(null, loadState));

        function redirectToDetails(id) {
            $location.path('/' + id);
        }
    }

    function onNameChanged() {
        vm.command.urlPath = stringUtilities.slugify(vm.command.title);
    }

    function onPageTypeChanged() {
        var selectedPageType = vm.command.pageType,
            filterBy = selectedPageType  == 'CustomEntityDetails' ? selectedPageType : 'Generic'; 
       
        vm.pageTemplates = _.where(vm.allPageTemplates, { pageType: filterBy });
    }

    /* PRIVATE FUNCS */

    function cancel() {
        $location.path('/');
    }

    function initData() {
        vm.pageTypes = pageService.getPageTypes();

        vm.command = {
            showInSiteMap: true,
            pageType: vm.pageTypes[0].value
        };

        var loadTemplatesDeferred = pageTemplateService
            .getAll()
            .then(function (pageTemplates) {
                vm.allPageTemplates = pageTemplates;
            });

        var loadRoutingRulesDeferred = customEntityService
            .getAllRoutingRules()
            .then(function (routingRules) {
                vm.routingRules = routingRules;
            });

        vm.formLoadState
            .offWhen(
                loadLocalesDeferred,
                loadWebDirectoryDeferred,
                loadTemplatesDeferred,
                loadRoutingRulesDeferred
                )
            .then(onPageTypeChanged);
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