angular.module('cms.shared').directive('cmsTagList', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { tags: '=cmsTags' },
        require: '?^^cmsModalDialogContainer',
        templateUrl: modulePath + 'UIComponents/Tags/taglist.html',
        link: link
    };

    function link(scope, el, attributes, modalDialogContainerController) {
        if (modalDialogContainerController) {
            scope.isInModal = true;
        }
    }
}]);