angular.module('cms.shared').directive('cmsFormFieldTime', [
    'shared.internalModulePath',
    'shared.timeUtilities',
    'baseFormFieldFactory',
function (
    modulePath,
    timeUtilities,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldTime.html',
        passThroughAttributes: [
            'required',
            'min',
            'step',
            'max',
            'disabled',
            'readonly',
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

            // Use watch instead of formatters/parsers here due to issues 
            // with utc dates
            scope.$watch("vm.model", setEditorModel);
            scope.$watch("vm.editorModel", setCmsModel);

            function setEditorModel(value) {
                if (value !== vm.editorModel) {
                    vm.editorModel = timeUtilities.toDate(value);
                }
            }

            function setCmsModel(value) {
                if (value !== vm.model) {
                    vm.model = timeUtilities.format(value);
                }
            }
        }
    }
}]);