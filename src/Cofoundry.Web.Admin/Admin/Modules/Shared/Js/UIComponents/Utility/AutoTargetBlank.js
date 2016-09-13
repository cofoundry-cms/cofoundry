/**
 * If this element is in a modal popup or in a form in edit mode, then add a target="_blank" attribute
 * so that links open in a new tab
 */
angular.module('cms.shared').directive('cmsAutoTargetBlank', function () {

    return {
        restrict: 'A',
        require: ['?^^cmsModalDialogContainer', '?^^cmsForm'],
        link: link
    };

    function link(scope, el, attributes, controllers) {
        var modalDialogContainerController = controllers[0],
            formController = controllers[1];

        if (modalDialogContainerController) {
            el.attr('target', '_blank');
        } else if (formController) {
            scope.formScope = formController.getFormScope();

            // watches
            scope.$watch('formScope.editMode', function () {
                if (scope.formScope.editMode) {
                    el.attr('target', '_blank');
                } else {
                    el.removeAttr('target');
                }
            });
        }
    }
});