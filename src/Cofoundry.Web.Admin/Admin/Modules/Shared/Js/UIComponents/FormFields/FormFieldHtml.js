/**
 * Allows editing of html. Note that in order to display html we have included ngSanitize in
 * the module dependencies (https://docs.angularjs.org/api/ng/directive/ngBindHtml)
 */
angular.module('cms.shared').directive('cmsFormFieldHtml', [
    '$sce',
    '_',
    'shared.internalModulePath', 
    'shared.stringUtilities',
    'taApplyCustomRenderers',
    'textAngularManager',
    'baseFormFieldFactory', 
function (
    $sce,
    _,
    modulePath, 
    stringUtilities,
    taApplyCustomRenderers,
    textAngularManager,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldHtml.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'disabled'
        ],
        getInputEl: getInputEl,
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            toolbarsConfig: '@cmsToolbars',
            toolbarCustomConfig: '@cmsCustomToolbar'
        }),
        controller: Controller,
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* OVERRIDES */

    function Controller() {
        var vm = this;
        vm.toolbar = parseToolbarButtons(vm.toolbarsConfig, vm.toolbarCustomConfig);
    }

   
    function link(scope, el, attributes) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);
        
        scope.$watch("vm.model", setTextAngularModel);
        scope.$watch("vm.taModel", setCmsModel);

        el.on('$destroy', function () {
            textAngularManager.unregisterEditor(vm.modelName);
        });

        function setTextAngularModel(value) {
            var transformedValue = value ? taReverseCustomRenderers(value) : value;

            if (transformedValue !== vm.taModel) {
                vm.taModel = taReverseCustomRenderers(value);
                vm.rawHtml = $sce.trustAsHtml(value);
            }
        }

        function setCmsModel(value) {
            var transformedValue = formatValueForCMS(value);

            if (transformedValue !== vm.model) {
                vm.model = transformedValue;
                vm.rawHtml = $sce.trustAsHtml(transformedValue);
            }
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('text-angular');
    }

    /* HELPERS */

    function parseToolbarButtons(toolbarsConfig, toolbarCustomConfig) {
        var DEFAULT_CONFIG = 'headings,basicFormatting',
            buttonConfig = {
                headings: ['h1', 'h2', 'h3'],
                basicFormatting: ['p', 'bold', 'italics', 'underline', 'ul', 'ol', 'insertLink'],
                media: ['insertImage', 'insertVideo'],
                source: ['html'],
            }, toolbar = [];

        toolbarsConfig = toolbarsConfig || DEFAULT_CONFIG;

        toolbarsConfig.split(',').forEach(function (configItem) {
            configItem = stringUtilities.lowerCaseFirstLetter(configItem.trim());

            if (configItem === 'custom') {

                toolbar = _.union(toolbar, parseCustomConfig(toolbarCustomConfig));

            } else if (buttonConfig[configItem]) {
                toolbar.push(buttonConfig[configItem]);
            }
        });

        return toolbar;

        function parseCustomConfig(toolbarCustomConfig) {
            var customToolbars;

            if (toolbarCustomConfig) {
                try {
                    customToolbars = JSON.parse('{"j":[' + toolbarCustomConfig.replace(/'/g, '"') + ']}').j;
                }
                catch (e) { }

                if (customToolbars && customToolbars.length) {
                    return customToolbars;
                }
            }

            return [];
        }
    }

    function formatValueForCMS(val) {

        if (!val) return val;

        val = taApplyCustomRenderers(val);

        return val;
    }

    /**
     * Need to parse the text from the server and convert it to a text angular format
     * https://github.com/fraywing/textAngular/issues/769
     */
    function taReverseCustomRenderers(val) {
        var element = angular.element('<div></div>');
        element[0].innerHTML = val;
        elements = element.find('iframe');
        
        angular.forEach(elements, function ($element) {
            var embedSlug = youtubeParser($element.src);
            _element = angular.element($element);

            var embedHtml = '<img class="ta-insert-video" src="https://img.youtube.com/vi/' + embedSlug + '/hqdefault.jpg" ta-insert-video="' + $element.src + '" contenteditable="false" allowfullscreen="true" frameborder="0"';

            if ($element.style.cssText) {
                embedHtml += ' style="' + $element.style.cssText + '"';
            }
            if ($element.width) {
                embedHtml += ' width="' + $element.width + '"';
            }
            if ($element.height) {
                embedHtml += ' height="' + $element.height + '"';
            }
            embedHtml += ' />';
            _element.replaceWith(embedHtml);
        });

        return element[0].innerHTML;
    }

    function youtubeParser(url) {
        var match = url && url.match((/^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/));
        return match && match[2].length === 11 ? match[2] : false;
    }
}]);