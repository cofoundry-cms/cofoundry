/**
 * Allows editing of html. Note that in order to display html we have included ngSanitize in
 * the module dependencies (https://docs.angularjs.org/api/ng/directive/ngBindHtml)
 */
angular.module('cms.shared').directive('cmsFormFieldHtml', [
    '$sce',
    '_',
    'shared.internalModulePath', 
    'shared.stringUtilities',
    'baseFormFieldFactory', 
function (
    $sce,
    _,
    modulePath, 
    stringUtilities,
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
        vm.tinymceOptions = {
            toolbar: parseToolbarButtons(vm.toolbarsConfig, vm.toolbarCustomConfig),
            plugins: 'link image media fullscreen code',
            content_css: "/admin/modules/shared/content/css/lib/tinymce/content.min.css",
            menubar: false,
            min_height: 250
        };
    }

   
    function link(scope, el, attributes) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);
        
        scope.$watch("vm.model", setEditorModel);
        scope.$watch("vm.editorModel", setCmsModel);

        el.on('$destroy', function () {
            //textAngularManager.unregisterEditor(vm.modelName);
        });

        function setEditorModel(value) {
            if (value !== vm.editorModel) {
                vm.editorModel = value;
                vm.rawHtml = $sce.trustAsHtml(value);
            }
        }

        function setCmsModel(value) {
            if (value !== vm.model) {
                vm.model = value;
                vm.rawHtml = $sce.trustAsHtml(value);
            }
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('textarea');
    }

    /* HELPERS */

    function parseToolbarButtons(toolbarsConfig, toolbarCustomConfig) {
        var DEFAULT_CONFIG = 'headings,basicFormatting',
            buttonConfig = {
                headings: 'formatselect',
                basicFormatting: 'fullscreen undo redo | bold italic underline | link unlink',
                media: 'image media',
                source: 'code',
            }, toolbar = '';

        toolbarsConfig = toolbarsConfig || DEFAULT_CONFIG;

        toolbarsConfig.split(',').forEach(function (configItem) {
            configItem = stringUtilities.lowerCaseFirstLetter(configItem.trim());

            if (configItem === 'custom') {

                toolbar = _.union(toolbar, parseCustomConfig(toolbarCustomConfig));

            } else if (buttonConfig[configItem]) {
                toolbar = toolbar.concat((toolbar.length ? ' | ': '') + buttonConfig[configItem]);
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
}]);