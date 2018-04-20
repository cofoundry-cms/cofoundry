/**
 * Allows editing of html. Note that in order to display html we have included ngSanitize in
 * the module dependencies (https://docs.angularjs.org/api/ng/directive/ngBindHtml)
 */
angular.module('cms.shared').directive('cmsFormFieldHtml', [
    '$sce',
    '_',
    'shared.internalModulePath', 
    'shared.internalContentPath',
    'shared.stringUtilities',
    'shared.modalDialogService',
    'baseFormFieldFactory',
function (
    $sce,
    _,
    modulePath, 
    contentPath,
    stringUtilities,
    modalDialogService,
    baseFormFieldFactory
) {

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
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* OVERRIDES */

    function link(scope, el, attributes) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        vm.tinymceOptions = getTinyMceOptions(vm);

        scope.$watch("vm.model", setEditorModel);
        scope.$watch("vm.editorModel", setCmsModel);

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

    function getTinyMceOptions(vm) {
        return {
            toolbar: parseToolbarButtons(vm.toolbarsConfig, vm.toolbarCustomConfig),
            plugins: 'link image media fullscreen imagetools code',
            content_css: contentPath + "css/third-party/tinymce/content.min.css",
            menubar: false,
            min_height: 300,
            setup: function (editor) {
                editor.addButton('cfimage', {
                    icon: 'image',
                    onclick: onEditorImageButtonClick.bind(null, editor)
                });
            },
            browser_spellcheck: true
        };
    }

    function onEditorImageButtonClick(editor) {
        var currentElement = editor.selection.getContent({ format: 'image' }),
            currentImage = currentElement.length ? angular.element(currentElement) : null;

        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/EditorDialogs/ImageAssetEditorDialog.html',
            controller: 'ImageAssetEditorDialogController',
            options: {
                imageAssetHtml: currentImage,
                onSelected: function (output) {
                    editor.insertContent(output.html);
                }
            }
        });
    }

    function parseToolbarButtons(toolbarsConfig, toolbarCustomConfig) {
        var DEFAULT_CONFIG = 'headings,basicFormatting',
            buttonConfig = {
                headings: 'formatselect',
                basicFormatting: 'fullscreen undo redo | bold italic underline | link unlink',
                advancedFormatting: 'bullist numlist blockquote | alignleft aligncenter alignright alignjustify',
                media: 'cfimage media',
                source: 'code removeformat',
            }, toolbar = '';

        toolbarsConfig = toolbarsConfig || DEFAULT_CONFIG;

        toolbarsConfig.split(',').forEach(function (configItem) {
            configItem = stringUtilities.lowerCaseFirstWord(configItem.trim());

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