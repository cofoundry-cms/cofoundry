/**
 * Directive that allows a user to enter a vimeo Url or Id which will be
 * verified and then used to get the video information from vimeo, which
 * is then passed to the optional cmsOnVideoSelected scope function.
 * Does not support non-edit mode since so far it's only used in the 
 * VimeoPickerDialog.
 */
angular.module('cms.shared').directive('cmsFormFieldVimeoId', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.vimeoService',
    'shared.validationErrorService',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    vimeoService,
    validationErrorService,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/Vimeo/FormFieldVimeoId.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            onVideoSelected: '&cmsOnVideoSelected'
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
            isRequired = _.has(attributes, 'required'),
            formController = controllers[0];

        init();
        return baseFormFieldFactory.defaultConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.setEditing = toggleEditing.bind(null, true);
            vm.updateVideoId = updateVideoId;
            vm.cancelEditing = cancelEditing;

            vm.updateIdLoadState = new LoadState();

            scope.$watch('vm.model', function (newValue) {
                toggleEditing(!newValue);
            });
        }

        /* ACTIONS */

        function updateVideoId() {
            var inputId = vm.idOrUrlInput,
                videoId = parseVideoId(inputId);

            if (!inputId) {
                vm.model = null;
                triggerOnVideoSelected(null);

            } else if (inputId && !videoId) {
                addError('The url/id is invalid');
            }
            else if (!videoId || videoId == vm.model) {
                cancelEditing();
            }  else {

                vm.updateIdLoadState.on();
                vimeoService
                    .getVideoInfo(videoId)
                    .then(onInfoLoaded)
                    .catch(onFail)
                    .finally(vm.updateIdLoadState.off);
            }

            function onFail(response) {
                addError('There was a problem accessing Vimeo');
            }

            function onInfoLoaded(info) {
                if (info) {
                    vm.model = vm.idOrUrlInput = info.id;

                    triggerOnVideoSelected(info);
                } else {
                    addError('Video not found');
                }
            }

            function triggerOnVideoSelected(info) {
                if (vm.onVideoSelected) vm.onVideoSelected({ model: info })
            }

            function addError(message) {
                validationErrorService.raise([{
                    properties: [vm.modelName],
                    message: message
                }]);
            }
        }

        function cancelEditing() {
            vm.idOrUrlInput = vm.model;
            vm.onChange();
            toggleEditing(false);
        }

        /* Helpers */

        function toggleEditing(isEditing) {
            vm.isEditing = isEditing;
        }

        function parseVideoId(urlOrId) {
            var urlRegex = /^.*(vimeo\.com\/)((channels\/[A-z]+\/)|(groups\/[A-z]+\/videos\/))?([0-9]+)/,
                matches;

            if (!urlOrId) return;

            if (/^\d+$/.test(urlOrId)) {
                return urlOrId;
            }

            matches = urlRegex.exec(urlOrId);
            return matches && matches[5];
        }
    }
}]);