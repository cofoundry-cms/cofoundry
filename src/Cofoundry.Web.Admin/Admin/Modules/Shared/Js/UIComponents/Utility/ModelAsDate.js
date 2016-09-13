/**
 * Use this to turn a string date into a date object for ng model binding.
 */
angular.module('cms.shared').directive('cmsModelAsDate', function () {

    return {
        require: 'ngModel',
        link: link
    };

    function link(scope, elem, attr, ngModelController) {
        ngModelController.$formatters.push(function (modelValue) {
            return modelValue ? new Date(modelValue) : null;
        });
    }
});