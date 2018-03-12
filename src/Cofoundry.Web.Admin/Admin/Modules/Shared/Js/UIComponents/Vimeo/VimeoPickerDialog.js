angular.module('cms.shared').controller('VimeoPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.vimeoService',
    'options',
    'close',
function (
    $scope,
    LoadState,
    stringUtilities,
    vimeoService,
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
            vimeoService
                .getVideoInfo(options.currentVideo)
                .then(onVideoSelected)
                .finally(vm.loadState.off);
        } else {
            vm.model = options.currentVideo;
        }
    }

    /* ACTIONS */

    function onVideoSelected(model) {

        console.log('1', model);
        if (model) {
            vm.model = {
                id: model.video_id,
                title: model.title,
                description: stringUtilities.stripTags(model.description),
                width: model.width,
                height: model.height,
                uploadDate: model.upload_date,
                duration: model.duration,
                thumbnailUrl: model.thumbnail_url,
                thumbnailWidth: model.thumbnail_width,
                thumbnailHeight: model.thumbnail_height
            };
        } else {
            vm.model = null;
        }
        console.log('2',model, vm.model);
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
