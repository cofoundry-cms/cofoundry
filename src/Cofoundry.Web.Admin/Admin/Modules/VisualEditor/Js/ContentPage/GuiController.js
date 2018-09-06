// Internals
Cofoundry.visualEditor = (function () {

    /*
    !!!IMPORTANT!!!
    THIS FILE WILL BE INJECTED INTO THE SITE TEMPLATES,
    THEREFORE THIS FILE SHOULD CONTAIN NO THIRD PARTY DEPENDENCIES
    TO MINIMIZE ANY RISKS OF CONFLICTS

    ALL ADMIN INPUTS ARE PROXIED VIA AN IFRAME TO ENSURE THE ANGULAR WORLD IS
    CONTAINED AWAY FROM THE SITE TEMPLATES
    
    */

    // Private variables
    var __IFRAME,
        __TOOLBAR,
        __TOOLBAR_BUTTONS,
        __TOOLBAR_REGION,
        __TOOLBAR_BLOCK
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
                model = _internal.model,
                visualEditorMode = model.visualEditorMode,
                route = model.isCustomEntityRoute ? model.pageRoutingInfo.customEntityRoute : model.pageRoutingInfo.pageRoute
                ;

            // Internal refs
            __TOOLBAR = toolbar;

            if (visualEditorMode === 'SpecificVersion' && model.hasEntityUpdatePermission) {
                // Insert copy to draft button
                _toolBar.addButton({
                    icon: 'fa-files-o',
                    title: 'Copy to<br />draft',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.copyToDraft
                });
            }

            if (((visualEditorMode === 'Preview' || visualEditorMode === 'Edit')
                || (route.publishStatus == 'Unpublished' && model.pageVersion.isLatestPublishedVersion && !model.hasDraftVersion))
                && model.hasEntityPublishPermission) {
                // Insert publish button
                _toolBar.addButton({
                    icon: 'fa-cloud-upload',
                    title: 'Publish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.publish
                });
            } else if (visualEditorMode === 'Live' && model.hasEntityPublishPermission) {
                // Insert unpublish button
                _toolBar.addButton({
                    icon: 'fa-cloud-download',
                    title: 'Unpublish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.unpublish
                });
            }
        },

        bindGui: function() {
            var toolbar_add_block = document.getElementById('cofoundry-sv__btn-add-block'),
                toolbar_block = document.getElementById('cofoundry-sv__block-popover-container'),
                wrap_ui_container = document.getElementsByTagName('body')[0];
                wrap_ui_template = document.getElementById('cofoundry-sv__ui-wrap'),
                current_ui_elements = [],
                timer = null,
                scope = {
                    buttons: {},
                    regionY: -1
                }
            ;

            // Internal refs
            __TOOLBAR_BLOCK = toolbar_block;

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
                    onRegionGuiChange(e);
                }
            }

            function addUIAfterResize() {
                if (timer) clearTimeout(timer);
                timer = setTimeout(function () { addUI(document); }, 500);
            }

            function addUI(rootElement, popover) {
                var entityType = _internal.model.isCustomEntityRoute ? 'custom-entity' : 'page';
                addForComponent('region', onRegionMouseEnter, onRegionMouseLeave, onRegionMouseMove);
                addForComponent('region-block', onBlockMouseEnter, onBlockMouseLeave);

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

                        // If name is null then discard as its an empty placeholder block
                        if (!el_data.name) {
                            continue;
                        }

                        // Check to see if the region allows for multiple blocks
                        if (!el_data.isBlock) {
                            // Region
                            multiMode = Boolean(el.getAttribute('data-cms-multi-block'));
                        } else {
                            // Block
                            multiMode = Boolean(el.parentNode.getAttribute('data-cms-multi-block'));
                        }

                        // Store index so we can create unique element ID
                        el_data.index = i;

                        // Create overlay to show bounds of editable area
                        var ui_wrap = createWrapElement(el_data);
                        wrap_ui_container.appendChild(ui_wrap);

                        // Create a plus icon for adding new blocks
                        if (multiMode || (!multiMode && !el_data.hasContent)) {
                            var ui_wrap_add_block = createAddBlockIcon(el_data);
                            wrap_ui_container.appendChild(ui_wrap_add_block);
                        }

                        // Add event handlers to regions/blocks
                        el.addEventListener('mouseenter', onMouseEnterFn);
                        el.addEventListener('mouseleave', onMouseLeaveFn);
                        if (onMouseMoveFn) {
                            el.addEventListener('mousemove', onMouseMoveFn);
                        }

                        current_ui_elements.push({
                            el: el,
                            ui_elements: [ui_wrap, ui_wrap_add_block],
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
                    isBlock = componentName === "region-block",
                    rootEl = isBlock ? el.parentNode : el;
                    ;

                if (!el.offsetWidth) {
                    el.style.width = '100%';
                }

                var os = offset(el);
                data.idName = (data.isBlock ? 'block' : 'region') + '_ui_wrap';
                data.isBlock = isBlock;
                data.y = os.top;
                data.x = os.left;
                data.width = el.offsetWidth;
                data.height = el.offsetHeight;
                data.el = el;
                data.hasContent = isBlock ? el.innerHTML.length > 0 : !el.hasAttribute('data-cms-page-region-empty');
                data.html = el.innerHTML;
                data.name = el.getAttribute(isBlock ? 'data-cms-page-block-title' : 'data-cms-page-region-name');
                data.regionName = rootEl.getAttribute('data-cms-page-region-name');
                data.className = isBlock ? 'cofoundry-sv__ui-wrap--block' : 'cofoundry-sv__ui-wrap--region';

                // Store data from attributes
                parseBlockAttributes(el, data);
                parseRegionAttributes(rootEl, data);

                return data;
            }

            function createWrapElement(data) {
                var ui_wrap = wrap_ui_template.cloneNode(true);
                ui_wrap.id = data.idName + '_' + data.index;
                ui_wrap.className = data.className;
                ui_wrap.style.top = data.y + 'px';
                ui_wrap.style.left = data.x + 'px';
                ui_wrap.style.width = (data.width - 2) + 'px';
                ui_wrap.style.height = (data.height - 2) + 'px';
                ui_wrap.firstChild.nextSibling.innerHTML = data.name;
                return ui_wrap;
            }

            function createAddBlockIcon(data) {
                var ui_wrap_add_block = toolbar_add_block.cloneNode(true);
                if (!data.isBlock && !data.hasContent) {
                    ui_wrap_add_block.className += ' cofoundry-sv__btn-add-block--empty';
                    ui_wrap_add_block.style.top = data.y + (data.height/2) + 'px';
                } else {
                    ui_wrap_add_block.style.top = (data.isBlock ? (data.y + data.height) : data.y) + 'px';
                }
                ui_wrap_add_block.style.left = data.x + (data.width / 2) + 'px';
                ui_wrap_add_block.style.display = 'block';
                ui_wrap_add_block.title = 'Add content block to ' + data.regionName;
                ui_wrap_add_block.addEventListener('click', function () { onAddRegionBlock(data); });
                return ui_wrap_add_block;
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
                var moveupBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-moveup')[0],
                    movedownBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-movedown')[0],
                    editBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-edit')[0],
                    addBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-add')[0],
                    deleteBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-delete')[0],
                    addaboveBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-addabove')[0],
                    addbelowBlockButton = __TOOLBAR_BLOCK.querySelectorAll('#cofoundry-sv__btn-block-addbelow')[0];

                // Bind click events
                bindEventHandler(moveupBlockButton, 'moveBlockUp', onMoveupBlock);
                bindEventHandler(movedownBlockButton, 'moveBlockDown', onMovedownBlock);
                bindEventHandler(editBlockButton, 'editBlock', onEditBlock);
                bindEventHandler(addBlockButton, 'addBlock', onAddBlock);
                bindEventHandler(addaboveBlockButton, 'addBlockAbove', onAddaboveBlock);
                bindEventHandler(addbelowBlockButton, 'addBlockBelow', onAddbelowBlock);
                bindEventHandler(deleteBlockButton, 'deleteBlock', onDeleteBlock);
            }

            // Handlers
            function onAddRegionBlock(data) {
                var insertMode = 'First';
                if (data.isBlock) {
                    insertMode = 'AfterItem';
                }

                buttonHandler('addRegionBlock', {
                    insertMode: insertMode,
                    pageTemplateRegionId: data.pageTemplateRegionId,
                    permittedBlockTypes: data.permittedBlockTypes,
                    versionBlockId: data.versionBlockId,
                    pageBlockTypeId: data.pageBlockTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    regionName: data.regionName,
                    pageId: _internal.model.page.page.pageId
                });
            }

            function onMoveupBlock() {
                buttonHandler('moveBlockUp', {
                    versionBlockId: scope.versionBlockId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    isUp: true
                });
            }

            function onMovedownBlock() {
                buttonHandler('moveBlockDown', {
                    versionBlockId: scope.versionBlockId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    isUp: false
                });
            }

            function onEditBlock() {
                buttonHandler('editBlock', {
                    versionBlockId: scope.versionBlockId,
                    pageBlockTypeId: scope.pageBlockTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute
                });
            }

            function onAddBlock() {
                buttonHandler('addBlock', {
                    insertMode: 'Last',
                    pageTemplateRegionId: scope.pageTemplateRegionId,
                    permittedBlockTypes: scope.permittedBlockTypes,
                    versionBlockId: scope.versionBlockId,
                    pageBlockTypeId: scope.pageBlockTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    pageId: _internal.model.page.page.pageId
                });
            }

            function onAddaboveBlock() {
                buttonHandler('addBlockAbove', {
                    insertMode: 'BeforeItem',
                    pageTemplateRegionId: scope.pageTemplateRegionId,
                    permittedBlockTypes: scope.permittedBlockTypes,
                    versionBlockId: scope.versionBlockId,
                    pageBlockTypeId: scope.pageBlockTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    pageId: _internal.model.page.page.pageId
                });
            }

            function onAddbelowBlock() {
                buttonHandler('addBlockBelow', {
                    insertMode: 'AfterItem',
                    pageTemplateRegionId: scope.pageTemplateRegionId,
                    permittedBlockTypes: scope.permittedBlockTypes,
                    versionBlockId: scope.versionBlockId,
                    pageBlockTypeId: scope.pageBlockTypeId,
                    isCustomEntity: _internal.model.isCustomEntityRoute,
                    pageId: _internal.model.page.page.pageId
                });
            }

            function onDeleteBlock() {
                buttonHandler('deleteBlock', {
                    versionBlockId: scope.versionBlockId,
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

            function onRegionMouseEnter(e) {
                onRegionGuiChange(e, e.target);
            }

            function onRegionMouseLeave() { }
            function onBlockMouseLeave() { }

            function onRegionMouseMove(e) {
                onRegionGuiChange(e);
            }

            function onBlockMouseEnter(e) {
                onBlockGuiChange(e.target);
            }

            function parseRegionAttributes(el, store) {
                store.currentElement = el;
                store.pageTemplateRegionId = el.getAttribute('data-cms-page-template-region-id');
                store.permittedBlockTypes = parseBlockTypes(el.getAttribute('data-cms-page-region-permitted-block-types'));
                store.regionName = el.getAttribute('data-cms-page-region-name');
                store.isMultiBlock = el.getAttribute('data-cms-multi-block');
                store.isCustomEntity = el.hasAttribute('data-cms-custom-entity-region');
            }

            function parseBlockAttributes(el, store) {
                store.currentBlockElement = el;
                store.versionBlockId = el.getAttribute('data-cms-version-block-id');
                store.pageBlockTypeId = el.getAttribute('data-cms-page-block-type-id');
            }

            function parseBlockTypes(blockTypeValue) {
                if (!blockTypeValue) return [];

                return blockTypeValue.split(',');
            }

            function onRegionGuiChange(e, el) {
                if (el) {
                    parseRegionAttributes(el, scope);
                    showHideButton('addRegionBlock', scope.isMultiBlock);
                }
            }

            function onBlockGuiChange(el) {
                var css = {};

                if (el) {
                    parseBlockAttributes(el, scope);

                    showHideButton('addBlock', !scope.versionBlockId);
                    showHideButton('editBlock', scope.versionBlockId);
                    showHideButton('deleteBlock', scope.versionBlockId);
                    showHideButton('moveBlockUp', scope.isMultiBlock);
                    showHideButton('moveBlockDown', scope.isMultiBlock);
                    showHideButton('addBlockAbove', scope.isMultiBlock);
                    showHideButton('addBlockBelow', scope.isMultiBlock);
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

                    __TOOLBAR_BLOCK.style.display = 'block';
                    __TOOLBAR_BLOCK.style.top = css.top;
                    __TOOLBAR_BLOCK.style.left = css.left;
                }
            }

            function onGuiEnd() {
                __TOOLBAR_BLOCK.style.display = 'none';
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
                var scrollLeft = (window.pageXOffset !== undefined) ? window.pageXOffset : (document.documentElement || document.body.parentNode || document.body).scrollLeft;
                var scrollTop = (window.pageYOffset !== undefined) ? window.pageYOffset : (document.documentElement || document.body.parentNode || document.body).scrollTop;

                return {
                    top: rect.top + scrollTop,
                    left: rect.left + scrollLeft
                }
            }

            function onResize(e) {
                scope.isOver = false;
                scope.regionAnchorElement = '';
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
            var dialogOptions;
            if (_internal.model.isCustomEntityRoute) {
                dialogOptions = {
                    entityNameSingular: _internal.model.customEntityDefinition.nameSingular,
                    isCustomEntity: true
                }
            }
            __IFRAME.contentWindow.postMessage({
                action: 'copyToDraft', args: [{
                    entityId: _internal.model.isCustomEntityRoute ? _internal.model.page.customEntity.customEntityId : _internal.model.page.page.pageId,
                    versionId: _internal.model.version.versionId,
                    hasDraftVersion: _internal.model.hasDraftVersion,
                    dialogOptions: dialogOptions
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
        _internal.model = Cofoundry.PageResponseData;

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