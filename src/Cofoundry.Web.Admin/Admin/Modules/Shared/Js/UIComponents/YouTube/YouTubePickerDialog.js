angular.module('cms.shared').controller('YoutubePickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.youtubeService',
    'options',
    'close',
function (
    $scope,
    LoadState,
    stringUtilities,
    YouTubeService,
    options,
    close) {

    var vm = $scope;
    init();

    /* INIT */

    function init() {

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.onVideoSelected = onVideoSelected;
        vm.isModelId = options.modelType === 'id';
        vm.loadState = new LoadState();

        if (vm.isModelId && options.currentVideo) {
            vm.loadState.on();
            youTubeService
                .getVideoInfo(options.currentVideo)
                .then(onVideoSelected)
                .finally(vm.loadState.off);
        } else {
            vm.model = options.currentVideo;
        }
    }

    /* ACTIONS */

    function onVideoSelected(model) {

        if (model) {
            vm.model = {
                id: model.id,
                title: model.title,
                description: stringUtilities.stripTags(model.description),
                width: model.width,
                height: model.height,
                uploadDate: model.upload_date,
                duration: model.duration,
            };
        } else {
            vm.model = null;
        }
    }

    function onCancel() {
        close();
    }

    function onOk() {
        if (vm.model && vm.isModelId) {
            options.onSelected(vm.model.id);
        } else {
            options.onSelected(vm.model);
        }
        close();
    }
}]);
