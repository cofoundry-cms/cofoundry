﻿/**
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