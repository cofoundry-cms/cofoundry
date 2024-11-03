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