/**
 * A dynamically generated set of form elements based on model meta data and bound to 
 * a dynamic model.
 */
angular.module('cms.shared').directive('cmsFormDynamicFieldSet', [
    '$compile',
    '_',
    'shared.stringUtilities',
    'shared.internalModulePath',
    'shared.LoadState',
function (
    $compile,
    _,
    stringUtilities,
    modulePath,
    LoadState
    ) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            /** 
             * An object comprising of a modelMetaData object containing property data for properties
             * of the model, and a model field containing the actual model to bind to.
             */
            dataSource: '=cmsDataSource',
            /** 
             * An object containing additional fields that can be picked up by
             * form controls. The principle example of this is localeId.
             */
            additionalParameters: '=cmsAdditionalParameters'
        },
        link: link,
        controller: ['$scope', FormDynamicFieldSetController],
        bindToController: true,
        controllerAs: 'vm'
    };

    /* CONTROLLER/COMPILE */

    function FormDynamicFieldSetController($scope) {
        var vm = this;
    };

    function link(scope, el, attrs, controllers) {
        var vm = scope.vm,
            attributeMapper = new AttributeMapper();

        scope.$watch('vm.dataSource', function (dataSource) {
            generateForm(dataSource);
        });

        function generateForm(dataSource) {
            var html = '';

            el.empty();

            if (dataSource && dataSource.modelMetaData.dataModelProperties.length) {
                dataSource.modelMetaData.dataModelProperties.forEach(function (modelProperty, i) {
                    var fieldName = mapDirectiveName(modelProperty);

                    html += '<' + fieldName;

                    html += attributeMapper.map('model', stringUtilities.lowerCaseFirstWord(modelProperty.name));
                    html += attributeMapper.map('title', modelProperty.displayName);
                    html += attributeMapper.map('required', modelProperty.isRequired && !modelProperty.additionalAttributes.readonly);
                    html += attributeMapper.map('description', modelProperty.description);

                    if (modelProperty.additionalAttributes) {

                        _.each(modelProperty.additionalAttributes, function (value, key) {
                            html += attributeMapper.map(key, value, i);
                        });
                    }

                    html += '></' + fieldName + '>';
                });

                el.append($compile(html)(scope));
            }
        }
    }

    /* HELPERS */

    function mapDirectiveName(modelProperty) {
        var fieldPrefix = 'cms-form-field-'

        // default fields for simple properties
        switch (modelProperty.dataTemplateName) {
            case 'Single':
            case 'Double':
            case 'Decimal':
                return fieldPrefix + 'number';
            case 'String':
                return fieldPrefix + 'text';
            case 'Boolean':
                return fieldPrefix + 'checkbox';
            case 'MultilineText':
                return fieldPrefix + 'text-area';
        }

        return fieldPrefix + stringUtilities.toSnakeCase(modelProperty.dataTemplateName);
    }

    /* CLASSES */

    /**
     * Encapsulates the impedence mismatch between the attributes
     * returned from the api and the formatting of attributes output in html
     */
    function AttributeMapper() {

        // certain attributes require special treatment, such as native html attributes
        var ATTR_PREFIX = 'cms-',
            attributeMap = {
                'maxlength': mapHtmlAttributeWithValue,
                'minlength': mapHtmlAttributeWithValue,
                'min': mapHtmlAttributeWithValue,
                'max': mapHtmlAttributeWithValue,
                'pattern': mapHtmlAttributeWithValue,
                'step': mapHtmlAttributeWithValue,
                'placeholder': mapHtmlAttributeWithValue,
                'match': mapDataSourceAttribute,
                'model': mapDataSourceAttribute,
                'options': mapAttributeWithObjectValue,
                'required': mapHtmlAttributeWithoutValue,
                'rows': mapHtmlAttributeWithValue,
                'cols': mapHtmlAttributeWithValue
            };

        /* public */

        this.map = function (key, value, i) {
            var postfix = 'ValMsg',
                attrMapFn = attributeMap[key],
                attrToVal;

            // allow validation messages for html attributes to pass-through
            if (!attrMapFn && stringUtilities.endsWith(key, postfix)) {
                attrToVal = key.substring(0, key.length - postfix.length);

                if (value && attributeMap[attrToVal] === mapHtmlAttributeWithValue) {
                    attrMapFn = mapHtmlAttributeWithValue;
                }
            } else if (!attrMapFn) {
                // default mapping function
                attrMapFn = mapCmsAttribute;
            }

            return attrMapFn ? attrMapFn(key, value, i) : '';
        }

        /* private */

        /**
         * Maps the attribute, binding the value to a property on the data model
         */
        function mapDataSourceAttribute(key, value) {
            value = 'vm.dataSource.model[\'' + value + '\']';
            return mapCmsAttribute(key, value);
        }

        /**
         * Maps the attribute, binding the value to an object in the additional
         * attributes collection on the model meta data.
         */
        function mapAttributeWithObjectValue(key, value, i) {

            value = 'vm.dataSource.modelMetaData.dataModelProperties[' + i + '].additionalAttributes[\'' + key + '\']';
            return mapCmsAttribute(key, value);
        }

        /**
         * Maps a native html attribute (without the cms prefix)
         */
        function mapHtmlAttributeWithValue(key, value) {
            return formatAttributeText(stringUtilities.toSnakeCase(key), value);
        }

        /**
         * Maps a native html attribute (without the cms prefix) which doesn't
         * require a value.
         */
        function mapHtmlAttributeWithoutValue(key, condition) {
            if (condition) {
                return formatAttributeText(key.toLowerCase());
            }

            return '';
        }

        function mapCmsAttribute(key, value) {
            key = ATTR_PREFIX + stringUtilities.toSnakeCase(key);
            return formatAttributeText(key, value);
        }

        function formatAttributeText(key, value) {
            if (!value) return ' ' + key;

            return ' ' + key + '="' + value + '"'
        }
    }

}]);