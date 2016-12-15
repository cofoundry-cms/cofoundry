angular.module('cms.shared').directive('cmsModalDialogContainer', [
    'shared.internalModulePath',
    '$timeout',
function (
    modulePath,
    $timeout) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogContainer.html',
        transclude: true,
        link: link,
        controller: angular.noop
        };

    function link(scope, el, attributes) {
        var cls = attributes.cmsModalSize === 'large' ? 'modal-lg' : '';
        cls += (scope.isRootModal ? ' is-root-modal' : ' is-child-modal');
        if (attributes.cmsModalSize === 'large') {
            scope.sizeCls = cls;
        }
        $timeout(function () {
            scope.sizeCls = cls + ' modal--show';
        }, 1);
    }
}]);