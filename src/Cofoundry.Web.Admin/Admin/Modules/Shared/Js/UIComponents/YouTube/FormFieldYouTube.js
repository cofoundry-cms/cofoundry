/**
 * Allows a user to search/select a vimeo video. By default this maps to the VimeoVideo c#
 * object and allows editing of the title/description field, but if you set the cms-model-type attribute
 * to be 'id' then this will map to a simple id field and will not allow editing of the title/description fields.
 */
angular.module('cms.shared').directive('cmsFormFieldYoutube', [
    '_',
    'shared.internalModulePath',
    'shared.internalContentPath',
    'shared.modalDialogService',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    contentPath,
    modalDialogService,
    stringUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noImagePath = assetReplacementPath + 'youtube-replacement.png',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/YouTube/FormFieldYouTube.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            modelType: '@cmsModelType'
        }),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.isRemovable = vm.model && !isRequired;

            setButtonText();
        }

        /* EVENTS */

        function remove() {
            setVideo(null);
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/YouTube/YouTubePickerDialog.html',
                controller: 'YoutubePickerDialogController',
                options: {
                    currentVideo: _.clone(vm.model),
                    modelType: vm.modelType,
                    onSelected: setVideo
                }
            });
        }

        /**
         * Initialise the state when the video is changed
         */
        function setVideo(video) {

            if (video) {
                vm.isRemovable = !isRequired;
                vm.model = video;
            } else {
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = null;
                }
            }

            setButtonText();
        }

        /* Helpers */

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Select';
        }
    }

}]);