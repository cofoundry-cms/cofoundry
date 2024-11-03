angular.module('cms.shared').factory('shared.youTubeService', [
    '$http',
    '$q',
    'shared.pluginServiceBase',
function (
    $http,
    $q,
    serviceBase
    ) {

    var service = {},
        serviceUrl = 'https://www.googleapis.com/youtube/v3/videos?id=',
        apiKey;

    /* INIT */

    $http.get(serviceBase + 'youtube-settings').then(loadSettings);

    function loadSettings(data) {
        apiKey = data.apiKey;
    }

    /* QUERIES */

    service.getVideoInfo = function (id) {

        if (apiKey) {

            return wrapGetResponse(serviceUrl + id + '&part=snippet&key=' + apiKey)
                .then(function (response) {
                    if (response && response.data && response.data.items.length) {
                        var data = response.data.items[0],
                            snippet = data.snippet,
                            thumbail = snippet.thumbnails.high;

                        var result = {
                            id: id,
                            title: snippet.title,
                            description: snippet.description,
                            publishDate: snippet.publishedAt
                        };

                        if (thumbail) {
                            result.thumbnailUrl = thumbail.url;
                            result.thumbnailWidth = thumbail.width;
                            result.thumbnailHeight = thumbail.height;
                        }

                        return result;
                    }

                    return;
                });
        } else {
            // No API key provided, so just return the id part of the data
            var def = $q.defer();
            def.resolve({ id: id });
            return def.promise;
        }
    }

    function wrapGetResponse() {
        var def = $q.defer();

        $http.get.apply(this, arguments)
            .then(def.resolve)
            .catch(function (response) {
                if (response.status == 404) {
                    def.resolve();
                }
                def.reject(response);
            });

        return def.promise;
    }

    return service;
}]);
/**
 * Allows a user to search/select a youtube video. By default this maps to the YouTubeVideo c#
 * object and allows editing of the title/description field, but if you set the cms-model-type attribute
 * to be 'id' then this will map to a simple id field and will not allow editing of the title/description fields.
 */
angular.module('cms.shared').directive('cmsFormFieldYoutube', [
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
        noImagePath = assetReplacementPath + 'youtube-replacement.png',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFieldYouTube.html',
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
                templateUrl: modulePath + 'UIComponents/YouTubePickerDialog.html',
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
/**
 * Directive that allows a user to enter a YouTube Url or Id which will be
 * verified and then used to get the video information from YouTube, which
 * is then passed to the optional cmsOnVideoSelected scope function.
 * Does not support non-edit mode since so far it's only used in the 
 * YouTubePickerDialog.
 */
angular.module('cms.shared').directive('cmsFormFieldYoutubeId', [
    '_',
    'shared.pluginModulePath',
    'shared.LoadState',
    'shared.youTubeService',
    'shared.validationErrorService',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    youTubeService,
    validationErrorService,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFieldYouTubeId.html',
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
                youTubeService
                    .getVideoInfo(videoId)
                    .then(onInfoLoaded) 
                    .catch(onFail)
                    .finally(vm.updateIdLoadState.off);
            }

            function onFail(response) {
                addError('There was a problem accessing YouTube');
            }

            function onInfoLoaded(info) {
                if (info) {
                    vm.model = vm.idOrUrlInput = videoId;
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
            var urlRegex = /(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i,
                matches;

            if (!urlOrId) return;

            if (/^[^"&?\/ ]{11}$/.test(urlOrId)) {
                return urlOrId;
            }

            matches = urlRegex.exec(urlOrId);

            return matches && matches[1];
        }
    }
}]);
angular.module('cms.shared').controller('YoutubePickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.youTubeService',
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
            vm.model = model;
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
 * Displays a YouTube video preview. Model can be an object with an id or the video id itself.
 */
angular.module('cms.shared').directive('cmsYoutubeVideo', [
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
        templateUrl: modulePath + 'UIComponents/YouTubeVideo.html',
        link: function (scope, el, attributes) {

            scope.replacementUrl = contentPath + 'img/AssetReplacement/youtube-replacement.png';
            scope.$watch('model', function (model) {
                var id;
                if (model) {
                    id = model.id || model;
                    scope.videoUrl = $sce.trustAsResourceUrl('https://www.youtube-nocookie.com/embed/' + id);
                } else {
                    scope.videoUrl = null;
                }
            });
        }
    };
}]);