angular.module('cms.shared').directive('cmsDocumentAsset', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: {
            document: '=cmsDocument'
        },
        templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentAsset.html',
        link: function (scope, el, attributes) {

            scope.getDocumentUrl = urlLibrary.getDocumentUrl;
        }
    };
}]);