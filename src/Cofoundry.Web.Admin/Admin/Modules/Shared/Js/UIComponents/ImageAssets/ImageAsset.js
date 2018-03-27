angular.module('cms.shared').directive('cmsImageAsset', [
    'shared.internalModulePath',
    'shared.internalContentPath',
    'shared.urlLibrary',
function (
    modulePath,
    contentPath,
    urlLibrary) {

    return {
        restrict: 'E',
        scope: {
            image: '=cmsImage',
            width: '@cmsWidth',
            height: '@cmsHeight',
            cropMode: '@cmsCropMode'
        },
        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAsset.html',
        link: function (scope, el, attributes) {

            scope.$watch('image', function (newValue, oldValue) {
                if (newValue && newValue.imageAssetId) {
                    scope.src = urlLibrary.getImageUrl(newValue, {
                        width: scope.width,
                        height: scope.height,
                        mode: scope.cropMode
                    });
                } else {
                    scope.src = contentPath + 'img/AssetReplacement/image-replacement.png';
                }
            });


        },
        replace: true
    };
}]);