/**
 * Displays a YouTube video preview. Model can be an object with an id or the video id itself.
 */
angular.module('cms.shared').directive('cmsYoutubeVideo', [
    '$sce',
    'shared.internalModulePath',
    'shared.contentPath',
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
        templateUrl: modulePath + 'UIComponents/Youtube/YoutubeVideo.html',
        link: function (scope, el, attributes) {

            scope.replacementUrl = contentPath + 'img/assetreplacement/youtube-replacement.png';
            scope.$watch('model', function (model) {
                var id;
                if (model) {
                    id = model.id || model;
                    scope.videoUrl = $sce.trustAsResourceUrl('http://www.youtube.com/embed/' + id);
                } else {
                    scope.videoUrl = null;
                }
            });
        }
    };
}]);