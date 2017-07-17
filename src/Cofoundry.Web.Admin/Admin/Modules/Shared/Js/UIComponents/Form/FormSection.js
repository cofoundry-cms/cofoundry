angular.module('cms.shared').directive('cmsFormSection', ['shared.internalModulePath', '$timeout', function (modulePath, $timeout) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSection.html',
        scope: {
            title: '@cmsTitle'
        },
        replace: true,
        transclude: true,
        link: link
    };

    function link(scope, elem, attrs) {
        // Wait a moment until child components are rendered before searching the dom
        $timeout(function () {
            var helpers = angular.element(elem[0].querySelector('.help-inline'));
            var btn = angular.element(elem[0].querySelector('.toggle-helpers'));

            if (helpers.length) {
                btn
                    .addClass('show')
                    .on('click', function () {
                        btn.toggleClass('active');
                        elem.toggleClass('show-helpers');
                    });
            }
        }, 100);
    }
}]);