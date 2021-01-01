var numericTypes = [
    { name: 'Byte', min: 0, max: 255, step: 1 },
    { name: 'SByte', min: -128, max: 127, step: 1 },
    { name: 'Int16', min: -32768, max: 32767, step: 1 },
    { name: 'UInt16', min: 0, max: 65535, step: 1 },
    { name: 'Int32', min: -2147483648, max: 2147483647, step: 1 },
    { name: 'UInt32', min: 0, max: 4294967295, step: 1 },
    { name: 'Int64', step: 1 },
    { name: 'UInt64', min: 0, step: 1 }
];

numericTypes.forEach(function (numericType) {

    angular.module('cms.shared').directive('cmsFormField' + numericType.name, [
        'shared.internalModulePath',
        'baseFormFieldFactory',
        function (
            modulePath,
            baseFormFieldFactory
        ) {

            var config = {
                templateUrl: modulePath + 'UIComponents/FormFields/FormFieldNumber.html',
                passThroughAttributes: [
                    'required',
                    'maxlength',
                    { name: 'min', default: numericType.min },
                    { name: 'max', default: numericType.max },
                    { name: 'step', default: numericType.step },
                    'disabled',
                    'placeholder',
                    'cmsMatch'
                ]
            };

            return baseFormFieldFactory.create(config);
        }]);
});