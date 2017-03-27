// Internals
Cofoundry.visualEditor = (function () {

    /*
    !!!IMPORTANT!!!
    THIS FILE WILL BE INJECTED INTO THE SITE TEMPLATES,
    THEREFORE THIS FILE SHOULD CONTAIN NO THIRD PARTY DEPENDENCIES
    TO MINIMIZE ANY RISKS OF CONFLICTS

    ALL ADMIN UNPUTS ARE PROXIED VIA AN IFRAME TO ENSURE THE ANGULAR WORLD IS
    CONTAINED AWAY FROM THE SITE TEMPLATES
    
    */

    // Private variables
    var __IFRAME,
        __TOOLBAR,
        __TOOLBAR_BUTTONS,
        __TOOLBAR_SECTION,
        __TOOLBAR_MODULE
        ;

    var _internal = {
        bindIframe: function () {
            // Create dom elements
            var iFrame = document.getElementById('cofoundry-src__iFrame'),
                body = document.getElementsByTagName('body')[0];

            // Store ref to iFrame
            __IFRAME = iFrame;

            // Fire config inside the frame
            __IFRAME.contentWindow.postMessage({
                action: 'config', args: [false, 'EntityNameSingular']
            }, document.location.origin);

            // Create IE + others compatible event handler
            var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent",
                postMessageListener = window[eventMethod],
                messageEvent = eventMethod === "attachEvent" ? "onmessage" : "message";

            // Listen to events from inside the iFrame
            postMessageListener(messageEvent, handleMessage);

            // Handle events received from the iFrame
            function handleMessage (e) {
                switch (e.data.type) {
                    case 'MODAL_CLOSE':
                        __IFRAME.style.display = 'none';
                        break;
                }
            }
        },

        bindToolbar: function () {
            var toolbar = document.getElementById('cofoundry-sv'),
                visualEditorMode = _internal.model.visualEditorMode
                ;

            // Internal refs
            __TOOLBAR = toolbar;

            if (visualEditorMode === 'Draft' || visualEditorMode === 'Edit') {
                // Insert publish button
                _toolBar.addButton({
                    icon: 'fa-cloud-upload',
                    title: 'Publish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.publish
                });
            } else if (visualEditorMode === 'Live') {
                // Insert unpublish button
                _toolBar.addButton({
                    icon: 'fa-cloud-download',
                    title: 'Unpublish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.unpublish
                });
            } else if (visualEditorMode === 'SpecificVersion') {
                // Insert copy to draft button
                _toolBar.addButton({
                    icon: 'fa-files-o',
                    title: 'Copy to<br />draft',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.copyToDraft
                });
            }
        },

        bindGui() {
            var toolbar_add_module = document.getElementById('cofoundry-sv__btn-add-module'),
                toolbar_module = document.getElementById('cofoundry-sv__module-popover-container'),
                wrap_ui_container = document.getElementsByTagName('body')[0];
                wrap_ui_template = document.getElementById('cofoundry-sv__ui-wrap'),
                current_ui_elements = [],
                timer = null,
                scope = {
                    buttons: {},
                    sectionY: -1
                }
            ;

            // Internal refs
            __TOOLBAR_MODULE = toolbar_module;

            // Add class to doc. Used for admin UI 
            document.getElementsByTagName('html')[0]
                .className = (_internal.model.isCustomEntityRoute ? 'cofoundry-editmode__custom-entity' : 'cofoundry-editmode__page');

            if (_internal.model.visualEditorMode === 'Edit') {
                // Bind events
                addUI(document);
                addButtonEvents();
                addDocumentEvents();
            }

            function addDocumentEvents() {
                window.addEventListener('resize', onResize);
                document.addEventListener('scroll', onScroll);

                function onResize(e) {
                    onGuiEnd();
                    removeUI();
                    addUIAfterResize();
                }

                function onScroll(e) {
                    onSectionGuiChange(e);
                }
            }

            function addUIAfterResize() {
                if (timer) clearTimeout(timer);
                timer = setTimeout(function () { addUI(document); }, 500);
            }

            function addUI(rootElement, popover) {
                var entityType = _internal.model.isCustomEntityRoute ? 'custom-entity' : 'page';
                addForComponent('section', onSectionMouseEnter, onSectionMouseLeave, onSectionMouseMove);
                addForComponent('section-module', onModuleMouseEnter, onModuleMouseLeave);

                function addForComponent(componentName, onMouseEnterFn, onMouseLeaveFn, onMouseMoveFn) {
                    var selectorAttribute = 'data-cms-' + entityType + '-' + componentName,
                        elements, multiMode = false, len, i;

                    if (rootElement.hasAttribute && rootElement.hasAttribute(selectorAttribute)) {
                        // we are passing the element directly to bind events
                        elements = [rootElement];
                    } else {
                        // we need to query the children to find elements to bind events to
                        elements = rootElement.querySelectorAll('[' + selectorAttribute + ']');
                    }

                    len = elements.length;
                    for (i = 0; i < len; ++i) {
                        var el = elements[i],
                            el_data = getElementData(el, componentName)
                        ;

                        // If name is null then discard as its an empty placeholder module
                        if (!el_data.name) {
                            continue;
                        }

                        // Check to see if the section allows for multiple modules
                        if (!el_data.isModule) {
                            // Section
                            multiMode = Boolean(el.getAttribute('data-cms-multi-module'));
                        } else {
                            // Module
                            multiMode = Boolean(el.parentNode.getAttribute('data-cms-multi-module'));
                        }

                        // Store index so we can create unique element ID
                        el_data.index = i;

                        // Create overlay to show bounds of editable area
                        var ui_wrap = createWrapElement(el_data);
                        wrap_ui_container.appendChild(ui_wrap);

                        // Create a plus icon for adding new modules
                        if (multiMode || (!multiMode && !el_data.hasContent)) {
                            var ui_wrap_add_module = createAddModuleIcon(el_data);
                            wrap_ui_container.appendChild(ui_wrap_add_module);
                        }

                        // Add event handlers to sections/modules
                        el.addEventListener('mouseenter', onMouseEnterFn);
                        el.addEventListener('mouseleave', onMouseLeaveFn);
                        if (onMouseMoveFn) {
                            el.addEventListener('mousemove', onMouseMoveFn);
                        }

                        current_ui_elements.push({
                            el: el,
                            ui_elements: [ui_wrap, ui_wrap_add_module],
                            remove: removeUIElement,
                            events: {
                                'mouseenter': onMouseEnterFn,
                                'mouseleave': onMouseLeaveFn,
                                'mousemove': onMouseMoveFn
                            }
                        });
                    }
                }
            }

            function getElementData(el, componentName) {
                var data = {},
                    isModule = componentName === "section-module",
                    rootEl = isModule ? el.parentNode : el;
                    ;

                if (!el.offsetWidth) {
                    el.style.width = '100%';
                }

                var os = offset(el);
                data.idName = (data.isModule ? 'module' : 'section') + '_ui_wrap';
                data.isModule = isModule;
                data.y = os.top;
                data.x = os.left;
                data.width = el.offsetWidth;
                data.height = el.offsetHeight;
                data.el = el;
                data.hasContent = el.innerHTML.length > 0;
                data.html = el.innerHTML;
                data.name = el.getAttribute(isModule ? 'data-cms-page-module-title' : 'data-cms-page-section-name');
                data.sectionName = rootEl.getAttribute('data-cms-page-section-name');
                data.className = isModule ? 'cofoundry-sv__ui-wrap--module' : 'cofoundry-sv__ui-wrap--section';

                // Store data from attributes
                parseModuleAttributes(el, data);
                parseSectionAttributes(rootEl, data);

                return data;
            }

            function createWrapElement(data) {
                var ui_wrap = wrap_ui_template.cloneNode(true);
                ui_wrap.id = data.idName + '_' + data.index;
                ui_wrap.className = data.className;
                ui_wrap.style.top = data.y + 'px';
                ui_wrap.style.left = data.x + 'px';
                ui_wrap.style.width = data.width + 'px';
                ui_wrap.style.height = data.height + 'px';
                ui_wrap.firstChild.nextSibling.innerHTML = data.name;
                return ui_wrap;
            }

            function createAddModuleIcon(data) {
                var ui_wrap_add_module = toolbar_add_module.cloneNode(true);
                if (!data.isModule && !data.hasContent) {
                    ui_wrap_add_module.className += ' cofoundry-sv__btn-add-module--empty';
                    ui_wrap_add_module.style.top = data.y + (data.height/2) + 'px';
                } else {
                    ui_wrap_add_module.style.top = (data.isModule ? (data.y + data.height) : data.y) + 'px';
                }
                ui_wrap_add_module.style.left = data.x + (data.width / 2) + 'px';
                ui_wrap_add_module.style.display = 'block';
                ui_wrap_add_module.title = 'Add module to ' + data.sectionName;
                ui_wrap_add_module.addEventListener('click', function () { onAddSectionModule(data); });
                return ui_wrap_add_module;
            }

            function removeUI() {
                current_ui_elements.forEach(function (item, index) {
                    item.remove();
                });
                current_ui_elements = [];
            }

            function removeUIElement() {
                // Remove event listeners
                for (var e in this.events) {
                    if (this.events.hasOwnProperty(e)) {
                        this.el.removeEventListener(e, this.events[e]);
                    }
                }

                // Remove DOM elements
                for (var ui_i = 0; ui_i < this.ui_elements.length; ui_i++) {
                    if (this.ui_elements[ui_i]) {
                        wrap_ui_container.removeChild(this.ui_elements[ui_i]);
                    }
                }
            }

            function addButtonEvents() {
                // Buttons
                var moveupModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-moveup')[0],
                    movedownModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-movedown')[0],
                    editModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-edit')[0],
                    addModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-add')[0],
                    deleteModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-delete')[0],
                    addaboveModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-addabove')[0],
                    addbelowModuleButton = __TOOLBAR_MODULE.querySelectorAll('#cofoundry-sv__btn-module-addbelow')[0];

                // Bind click events
                bindEventHandler(moveupModuleButton, 'moveModuleUp', onMoveupModule);
                bindEventHandler(movedownModuleButton, 'moveModuleDown', onMovedownModule);
                bindEventHandler(editModuleButton, 'editModule', onEditModule);
                bindEventHandler(addModuleButton, 'addModule', onAddModule);
                bindEventHandler(addaboveModuleButton, 'addModuleAbove', onAddaboveModule);
                bindEventHandler(addbelowModuleButton, 'addModuleBelow', onAddbelowModule);
                bindEventHandler(deleteModuleButton, 'deleteModule', onDeleteModule);
            }

            // Handlers
            function onAddSectionModule(data) {
                var insertMode = 'First';
                if (data.isModule) {
                    insertMode = 'AfterItem';
                }

                buttonHandler('addSectionModule', {
                    insertMode: insertMode,
                    pageTemplateSectionId: data.pageTemplateSectionId,
                    permittedModuleTypes: data.permittedModuleTypes,
                    versionModuleId: data.versionModuleId,
                    pageModuleTypeId: data.pageModuleTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onMoveupModule() {
                buttonHandler('moveModuleUp', {
                    versionModuleId: scope.versionModuleId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    isUp: true
                });
            }

            function onMovedownModule() {
                buttonHandler('moveModuleDown', {
                    versionModuleId: scope.versionModuleId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    isUp: false
                });
            }

            function onEditModule() {
                buttonHandler('editModule', {
                    versionModuleId: scope.versionModuleId,
                    pageModuleTypeId: scope.pageModuleTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onAddModule() {
                buttonHandler('addModule', {
                    insertMode: 'Last',
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    permittedModuleTypes: scope.permittedModuleTypes,
                    versionModuleId: scope.versionModuleId,
                    pageModuleTypeId: scope.pageModuleTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onAddaboveModule() {
                buttonHandler('addModuleAbove', {
                    insertMode: 'BeforeItem',
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    permittedModuleTypes: scope.permittedModuleTypes,
                    versionModuleId: scope.versionModuleId,
                    pageModuleTypeId: scope.pageModuleTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onAddbelowModule() {
                buttonHandler('addModuleBelow', {
                    insertMode: 'AfterItem',
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    permittedModuleTypes: scope.permittedModuleTypes,
                    versionModuleId: scope.versionModuleId,
                    pageModuleTypeId: scope.pageModuleTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onDeleteModule() {
                buttonHandler('deleteModule', {
                    versionModuleId: scope.versionModuleId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function bindEventHandler(button, action, handler) {
                if (!button) return;
                scope.buttons[action] = button;
                button.addEventListener('click', handler);
            }

            function buttonHandler(action, args) {
                scope.buttons[action]
                __IFRAME.contentWindow.postMessage({
                    action: action, args: [args]
                }, document.location.origin);
                __IFRAME.style.display = 'block';
            }

            function onSectionMouseEnter(e) {
                onSectionGuiChange(e, e.target);
            }

            function onSectionMouseLeave() {
                //onGuiEnd();
            }

            function onSectionMouseMove(e) {
                onSectionGuiChange(e);
            }

            function onModuleMouseEnter(e) {
                onModuleGuiChange(e.target);
            }

            function onModuleMouseLeave() {
                //onGuiEnd();
            }

            function parseSectionAttributes(el, store) {
                store.currentElement = el;
                store.pageTemplateSectionId = el.getAttribute('data-cms-page-template-section-id');
                store.permittedModuleTypes = parseModuleTypes(el.getAttribute('data-cms-page-section-permitted-module-types'));
                store.sectionName = el.getAttribute('data-cms-page-section-name');
                store.isMultiModule = el.getAttribute('data-cms-multi-module');
                store.isCustomEntity = el.hasAttribute('data-cms-custom-entity-section');
            }

            function parseModuleAttributes(el, store) {
                store.currentModuleElement = el;
                store.versionModuleId = el.getAttribute('data-cms-version-module-id');
                store.pageModuleTypeId = el.getAttribute('data-cms-page-module-type-id');
            }

            function parseModuleTypes(moduleTypeValue) {
                if (!moduleTypeValue) return [];

                return moduleTypeValue.split(',');
            }

            function onSectionGuiChange(e, el) {
                if (el) {
                    parseSectionAttributes(el, scope);
                    showHideButton('addSectionModule', scope.isMultiModule);
                }
            }

            function onModuleGuiChange(el) {
                var css = {};

                if (el) {
                    parseModuleAttributes(el, scope);

                    showHideButton('addModule', !scope.versionModuleId);
                    showHideButton('editModule', scope.versionModuleId);
                    showHideButton('deleteModule', scope.versionModuleId);
                    showHideButton('moveModuleUp', scope.isMultiModule);
                    showHideButton('moveModuleDown', scope.isMultiModule);
                    showHideButton('addModuleAbove', scope.isMultiModule);
                    showHideButton('addModuleBelow', scope.isMultiModule);
                }

                setUIPosition();

                function setUIPosition() {
                    var elementOffset = offset(el),
                        top = elementOffset.top,
                        left = elementOffset.left;

                    css = {
                        top: top + 'px',
                        left: (left || 0) + 'px'
                    };

                    scope.startScroll = scope.currentScrollY;
                    scope.startY = top;

                    __TOOLBAR_MODULE.style.display = 'block';
                    __TOOLBAR_MODULE.style.top = css.top;
                    __TOOLBAR_MODULE.style.left = css.left;
                }
            }

            function onGuiEnd() {
                __TOOLBAR_MODULE.style.display = 'none';
            }

            function showHideButton(action, condition) {
                var btn = scope.buttons[action];
                if (!btn) return;
                btn.style.display = condition ? 'block' : 'none';
            }

            function offset(el) {
                if (!el) return {
                    left: 0,
                    top: 0
                };

                var rect = el.getBoundingClientRect();
                return {
                    top: rect.top + document.body.scrollTop,
                    left: rect.left + document.body.scrollLeft
                }
            }

            function onResize(e) {
                scope.isOver = false;
                scope.sectionAnchorElement = '';
            }

            function onScroll(e) {
                scope.currentScrollY = (e || 0);
                var y = scope.startY + (scope.startScrollY - e);
                if (y < 0) y = 0;
                if (y) {
                    scope.css = {
                        top: y + 'px',
                        left: scope.css.left
                    }
                }
            }
        },

        // Actions
        publish: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'publish', args: [{
                    entityId: _internal.model.isCustomEntityRoute ? _internal.model.page.customEntity.customEntityId : _internal.model.page.page.pageId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    customEntityDefinition: _internal.model.customEntityDefinition
                }]
            }, document.location.origin);
            __IFRAME.style.display = 'block';
        },

        unpublish: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'unpublish', args: [{ 
                    entityId: _internal.model.isCustomEntityRoute ? _internal.model.page.customEntity.customEntityId : _internal.model.page.page.pageId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                }]
            }, document.location.origin);
            __IFRAME.style.display = 'block';
        },

        copyToDraft: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'copyToDraft', args: [{
                    entityId: _internal.model.page.page.pageId,
                    versionId: _internal.model.page.page.versionId,
                    hasDraftVersion: false
                }]
            }, document.location.origin);
            __IFRAME.style.display = 'block';
        }
    }

    var _toolBar = {
        addButton: function (options) {
            var type = options.type || 'secondary',
                btn = document.createElement('a'),
                spn = document.createElement('span');
            btn.appendChild(spn);

            btn.className = (type === 'primary' ? 'cofoundry-sv__options__button' : 'cofoundry-sv__mode__button') + ' ' + options.classNames;
            spn.innerHTML = options.title;

            if (options.click) {
                btn.href = '#';
                btn.addEventListener('click', options.click, false);
            } else {
                btn.href = options.href || '#';
            }

            __TOOLBAR_BUTTONS = __TOOLBAR.getElementsByClassName(type === 'primary' ? 'cofoundry-sv__options' : 'cofoundry-sv__mode')[0];
            if (__TOOLBAR_BUTTONS) {
                __TOOLBAR_BUTTONS.appendChild(btn);
            }
        }
    }

    window.onload = function () {
        // pageResponseData object is a serialized object inserted into the page
        _internal.model = pageResponseData;

        if (_internal.model != null) {
            _internal.bindIframe();
            _internal.bindGui();
            _internal.bindToolbar();
        }
    }

    // Return public API
    return {
        toolBar: _toolBar
    }

})();