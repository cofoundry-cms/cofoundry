angular.module('cms.images').controller('AddImageController', [
    '$location',
    '$scope',
    '_',
    'shared.focusService',
    'shared.stringUtilities',
    'shared.LoadState',
    'images.imageService',
function (
    $location,
    $scope,
    _,
    focusService,
    stringUtilities,
    LoadState,
    imageService
) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initData();

        vm.save = save;
        vm.cancel = cancel;
        $scope.$watch("vm.command.file", setFileName);

        vm.editMode = false;
        vm.saveLoadState = new LoadState();
    }

    /* EVENTS */

    function save() {
        vm.saveLoadState.on();

        imageService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(redirectToList);
    }

    function setFileName() {

        var command = vm.command;
        if (command.file && command.file.name) {

            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            focusService.focusById('title');
        }
    }

    /* PRIVATE FUNCS */

    function initData() {
        vm.command = {};
    }

    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('');
    }
}]);