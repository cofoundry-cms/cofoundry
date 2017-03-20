angular.module('cms.pages').controller('DuplicatePageController', [
    '$scope',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.pageService',
    'pages.customEntityService',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    LoadState,
    pageService,
    customEntityService,
    options,
    close) {

    var loadLocalesDeferred = $q.defer(),
        loadWebDirectoryDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {
       
        initData();

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = close;
        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.webDirectoriesLoaded = loadWebDirectoryDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred, loadWebDirectoryDeferred, loadRoutingRules());
    }

    /* EVENTS */

    function initData() {
        var page = options.page,
           pageRoute = page.pageRoute;

        $scope.isCustomEntityRoute = pageRoute.pageType === 'CustomEntityDetails';

        $scope.page = page;
        $scope.command = {
            pageToDuplicateId: page.pageId,
            localeId: pageRoute.locale ? pageRoute.locale.localeId : undefined,
            webDirectoryId: pageRoute.webDirectory.webDirectoryId,
            title: 'Copy of ' + page.latestVersion.title
        }
        if ($scope.isCustomEntityRoute) {
            $scope.command.customEntityRoutingRule = pageRoute.urlPath;
        } else {
            $scope.command.urlPath = pageRoute.urlPath + '-copy';
        }
    }

    function loadRoutingRules() {

        if ($scope.isCustomEntityRoute) {
            return customEntityService
                .getAllRoutingRules()
                .then(function (routingRules) {
                    $scope.routingRules = routingRules;
                });
        }

        var def = $q.defer();
        def.resolve();
        return def;
    }

    function save() {
        $scope.submitLoadState.on();

        pageService
            .duplicate($scope.command)
            .then(redirectoToPage)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

    /* PRIVATE FUNCS */

    function redirectoToPage(pageId) {
        $location.path('/' + pageId);
    }
}]);