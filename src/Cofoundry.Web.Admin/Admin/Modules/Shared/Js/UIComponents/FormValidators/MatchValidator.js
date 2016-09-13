/**
 * Validates that a field matches the value of another field. Set the came of the field 
 * in the attribute definition e.g. cms-match="vm.command.password"
 * Adapted from http://ericpanorel.net/2013/10/05/angularjs-password-match-form-validation/
 */
angular.module('cms.shared').directive('cmsMatch', [
    '$parse',
    '$timeout',
    'shared.internalModulePath',
    'shared.directiveUtilities',
function (
    $parse,
    $timeout,
    modulePath,
    directiveUtilities
    ) {

    var DIRECTIVE_ID = 'cmsMatch';
    var DIRECTIVE_ATTRIBUTE = 'cms-match';

    return {
        link: link,
        restrict: 'A',
        require: ['^^cmsForm', '?ngModel'],
    };
    
    function link(scope, el, attrs, controllers) {
        // NB: ngModel may be null on an outer form control before it has been copied to the inner input.
        if (!attrs[DIRECTIVE_ID] || !controllers[1]) return;

        var formController = controllers[0],
            ngModelController = controllers[1],
            form = formController.getFormScope().getForm(),
            sourceField = directiveUtilities.parseModelName(attrs[DIRECTIVE_ID]);

        var validator = function (value, otherVal) {
            var formField = form[sourceField];
            if (!formField) return false;

            var sourceFieldValue = formField.$viewValue;

            return value === sourceFieldValue;
        }

        ngModelController.$validators[DIRECTIVE_ID] = validator;
    }
}]);