angular.module('cms.shared').factory('shared.directiveUtilities', function () {
    var service = {};
    
    /* PUBLIC */

    /**
     * Sets the model name property, parsing it from the cms-model attribute property accessor. E.g. parses the property 'title' out
     * of cms-model="vm.command.title" or cms-model="vm.model['title']"
     */
    service.setModelName = function (vm, attrs) {
        if (attrs.cmsModelName) {
            vm.modelName = attrs.cmsModelName;
        } else {
            vm.modelName = service.parseModelName(attrs.cmsModel);
        }
    }

    /**
     * Parses the property name out of a property accessor string. E.g. parses the property 'title' out
     * of vm.command.title or vm.model['title']
     */
    service.parseModelName = function (attr) {
        var modelName, matches;

        if (!attr) return;

        modelName = attr.substring(attr.lastIndexOf('.') + 1, attr.length);

        matches = /['"]([^'"]*)['"]/g.exec(modelName);
        if (matches && matches.length > 1) {
            return matches[1];
        }
        else {
            return modelName;
        }
    }

    return service;
});