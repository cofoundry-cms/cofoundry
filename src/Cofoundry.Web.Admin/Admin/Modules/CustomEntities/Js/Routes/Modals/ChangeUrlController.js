angular.module('cms.customEntities').controller('ChangeUrlController', [
    '$scope',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.customEntityService',
    'customEntities.options',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    LoadState,
    customEntityService,
    customEntityOptions,
    options,
    close) {

    var loadLocalesDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {
        var customEntity = options.customEntity;

        $scope.options = customEntityOptions;
        $scope.customEntity = customEntity;
        $scope.command = {
            customEntityId: customEntity.customEntityId,
            localeId: customEntity.locale ? customEntity.locale.localeId : undefined,
            urlSlug: customEntity.urlSlug
        }

        $scope.submitLoadState = new LoadState();
        // Only the locales need loading at start, and only if the CE uses locales
        $scope.formLoadState = new LoadState(customEntityOptions.hasLocale);

        $scope.save = save;
        $scope.close = close;
        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred);
    }

    /* EVENTS */

    function save() {
        $scope.submitLoadState.on();

        customEntityService
            .updateUrl($scope.command)
            .then(options.onSave)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

}]);