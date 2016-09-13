angular.module('cms.shared').directive('cmsHttpPrefix', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, controller) {
            function ensureHttpPrefix(value) {
                var httpPrefix = 'http://';

                if (value
                    && !/^(https?):\/\//i.test(value)
                    && httpPrefix.indexOf(value) === -1
                    && 'https://'.indexOf(value) === -1) {

                    controller.$setViewValue(httpPrefix + value);
                    controller.$render();

                    return httpPrefix + value;
                }
                else {
                    return value;
                }
            }
            controller.$formatters.push(ensureHttpPrefix);
            controller.$parsers.splice(0, 0, ensureHttpPrefix);
        }
    };
});