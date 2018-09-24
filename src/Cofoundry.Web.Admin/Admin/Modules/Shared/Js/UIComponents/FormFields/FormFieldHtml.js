/**
 * Allows editing of html. Note that in order to display html we have included ngSanitize in
 * the module dependencies (https://docs.angularjs.org/api/ng/directive/ngBindHtml)
 */
angular.module('cms.shared').directive('cmsFormFieldHtml', [
    '$sce',
    '$q',
    '$http',
    '_',
    'shared.internalModulePath', 
    'shared.internalContentPath',
    'shared.stringUtilities',
    'shared.modalDialogService',
    'baseFormFieldFactory',
function (
    $sce,
    $q,
    $http,
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
            'disabled',
            'rows'
        ],
        getInputEl: getInputEl,
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            toolbarsConfig: '@cmsToolbars',
            toolbarCustomConfig: '@cmsCustomToolbar',
            options: '=cmsOptions',
            configPath: '@cmsConfigPath',
        }),
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* OVERRIDES */

    function link(scope, el, attributes) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        loadTinyMCEOptions(vm, attributes).then(function (tinymceOptions) {
            vm.tinymceOptions = tinymceOptions;
        });

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

    function loadTinyMCEOptions(vm, attributes) {
        var rows = 20,
            def = $q.defer();

        if (attributes.rows) {
            rows = parseInt(attributes.rows);
        }

        var defaultOptions = {
            toolbar: parseToolbarButtons(vm.toolbarsConfig, vm.toolbarCustomConfig),
            plugins: 'link image media fullscreen imagetools code',
            content_css: contentPath + "css/third-party/tinymce/content.min.css",
            menubar: false,
            min_height: rows * 16,
            setup: function (editor) {
                editor.addButton('cfimage', {
                    icon: 'image',
                    onclick: onEditorImageButtonClick.bind(null, editor)
                });
            },
            browser_spellcheck: true,
            convert_urls: false
        };

        if (vm.configPath) {
            $http.get(vm.configPath).then(bindFileOptions);

        } else {
            bindCodeOptionsAndResolve();
        }

        return def.promise;

        function bindFileOptions(result) {
            if (result && result.data) {
                _.extend(defaultOptions, result.data);
            }
            bindCodeOptionsAndResolve();
        }

        function bindCodeOptionsAndResolve() {
            // Always apply last
            if (vm.options) {
                _.extend(defaultOptions, vm.options);
            }

            def.resolve(defaultOptions);
        }
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
                basicFormatting: 'fullscreen undo redo removeformat | bold italic underline | link unlink',
                advancedFormatting: 'bullist numlist blockquote | alignleft aligncenter alignright alignjustify | strikethrough superscript subscript',
                media: 'cfimage media',
                source: 'code',
            }, toolbars = [];

        toolbarsConfig = toolbarsConfig || DEFAULT_CONFIG;

        toolbarsConfig.split(',').forEach(function (configItem) {
            configItem = stringUtilities.lowerCaseFirstWord(configItem.trim());

            if (configItem === 'custom') {
                toolbars = _.union(toolbars, parseCustomConfig(toolbarCustomConfig));

            } else if (buttonConfig[configItem]) {
                toolbars.push(buttonConfig[configItem]);
            }
        });

        return toolbars.join(' | ');

        function parseCustomConfig(toolbarCustomConfig) {
            var customToolbars;

            if (toolbarCustomConfig) {

                // old formatting allowed array parsing, but this is ambigous when mixed with other toolbars
                // and so will be removed eventually.
                if (!toolbarCustomConfig.startsWith("'") && !toolbarCustomConfig.startsWith("\"")) {
                    // single toolbar
                    return [toolbarCustomConfig];
                }
                try {
                    // parse an array of toolbars, with each one surrounded by quotationmarks
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