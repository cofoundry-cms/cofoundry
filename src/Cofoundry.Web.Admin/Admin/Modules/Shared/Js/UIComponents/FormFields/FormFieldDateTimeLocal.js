angular.module('cms.shared').directive('cmsFormFieldDateTimeLocal', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDateTimeLocal.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            minUtc: '@cmsMinUtc',
            maxUtc: '@cmsMaxUtc'
        }),
        passThroughAttributes: [
            'required',
            'min',
            'max',
            'disabled',
            'cmsMatch'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        init();

        /* Init */

        function init() {
            scope.$watch("vm.minUtc", function (value) {
                var date = parseDateAsUtc(value);
                vm.minLocal = formatDate(date);
                if (vm.minLocal) {
                    vm.addOrUpdateValidator({
                        name: 'min',
                        message: "This date cannot be before " + date.toLocaleString()
                    });
                }
            });

            scope.$watch("vm.maxUtc", function (value) {
                var date = parseDateAsUtc(value);
                vm.maxLocal = formatDate(date);
                if (vm.maxLocal) {
                    vm.addOrUpdateValidator({
                        name: 'max',
                        message: "This date cannot be after " + date.toLocaleString()
                    });
                }
            });
        }

        function parseDateAsUtc(value) {
            if (!value) return;

            // Format is expected to be 'yyyy-MM-ddT00:00'
            return new Date(value + ':00.000Z');
        }

        function formatDate(date) {
            if (!date) return;

            return date.getFullYear()
                + '-' + padTwoDigitNumber(date.getMonth() + 1)
                + '-' + padTwoDigitNumber(date.getDate())
                + 'T' + padTwoDigitNumber(date.getHours())
                + ':' + padTwoDigitNumber(date.getMinutes());
        }

        function padTwoDigitNumber(number) {
            return ("00" + number).slice(-2);
        }
    }
}]);