angular.module('cms.shared').factory('shared.vimeoService', [
    '$http',
    '$q',
    'shared.errorService',
function (
    $http,
    $q,
    errorService
    ) {

    var service = {},
        serviceUrl = 'https://vimeo.com/api/oembed.json?url=https%3A%2F%2Fvimeo.com%2F';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id)
            .then(function (response) {
                return JSON.parse(response.responseText);
            });
    }

    function wrapGetResponse(url) {
        var def = $q.defer();

        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", onComplete);
        xhr.open("GET", url);
        xhr.send();

        function onComplete() {
            var response = this;
            var isUnexpectedError = false;
            var errorMsg = "";

            switch (response.status) {
                case 200:
                    break;
                case 404:
                    errorMsg = "You aren't able to access the video because of privacy or permissions issues, or because the video is still transcoding.";
                    break;
                case 403:
                    errorMsg = "Embed permissions are disabled for this video, so you can't embed it.";
                    break;
                default:
                    isUnexpectedError = true;
                    errorMsg = "Something unexpected happened whilst connecting to the Vimeo API.";
            }

            if (!errorMsg.length) {
                def.resolve(response);
            } else {
                var error = {
                    title: 'Vimeo API Error',
                    message: errorMsg,
                    response: response
                }

                if (isUnexpectedError) {
                    errorService.raise(error);
                }

                def.reject(error);
            }
        }

        return def.promise;
    }

    return service;
}]);
/**
 * Allows a user to search/select a vimeo video. By default this maps to the VimeoVideo c#
 * object and allows editing of the title/description field, but if you set the cms-model-type attribute
 * to be 'id' then this will map to a simple id field and will not allow editing of the title/description fields.
 */
angular.module('cms.shared').directive('cmsFormFieldVimeo', [
    '_',
    'shared.pluginModulePath',
    'shared.pluginContentPath',
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
        noImagePath = assetReplacementPath + 'vimeo-replacement.png',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFieldVimeo.html',
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
                templateUrl: modulePath + 'UIComponents/VimeoPickerDialog.html',
                controller: 'VimeoPickerDialogController',
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
/**
 * Directive that allows a user to enter a vimeo Url or Id which will be
 * verified and then used to get the video information from vimeo, which
 * is then passed to the optional cmsOnVideoSelected scope function.
 * Does not support non-edit mode since so far it's only used in the 
 * VimeoPickerDialog.
 */
angular.module('cms.shared').directive('cmsFormFieldVimeoId', [
    '_',
    'shared.pluginModulePath',
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
        templateUrl: modulePath + 'UIComponents/FormFieldVimeoId.html',
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
                addError(response.message);
            }

            function onInfoLoaded(info) {
                if (info) {
                    vm.model = vm.idOrUrlInput = info.video_id;

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

/**
 * Displays a vimeo video preview. Model can be an object with an id or the video id itself.
 */
angular.module('cms.shared').directive('cmsVimeoVideo', [
    '$sce',
    'shared.pluginModulePath',
    'shared.pluginContentPath',
    'shared.urlLibrary',
function (
    $sce,
    modulePath,
    contentPath,
    urlLibrary) {

    return {
        restrict: 'E',
        scope: {
            model: '=cmsModel'
        },
        templateUrl: modulePath + 'UIComponents/VimeoVideo.html',
        link: function (scope, el, attributes) {

            scope.replacementUrl = contentPath + 'img/AssetReplacement/vimeo-replacement.png';
            scope.$watch('model', function (model) {
                var id;
                if (model) {
                    id = model.id || model;
                    scope.videoUrl = $sce.trustAsResourceUrl('//player.vimeo.com/video/' + id)
                } else {
                    scope.videoUrl = null;
                }
            });
        }
    };
}]);