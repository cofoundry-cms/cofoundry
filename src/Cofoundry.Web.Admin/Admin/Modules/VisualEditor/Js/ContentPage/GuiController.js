// Internals
Cofoundry.siteViewer = (function () {
    var __IFRAME;
    var __TOOLBAR;
    var __TOOLBAR_BUTTONS;
    var _internal = {
        createIframe: function () {
            // Create dom elements
            var iFrame = document.createElement('iframe'),
                body = document.getElementsByTagName('body')[0];

            // Point iFrame at razor view that bootstraps angular admin components
            iFrame.src = '/admin/visual-editor/frame';

            // Add styles to the iFrame elements
            iFrame.className = 'cofoundry-sv__iFrame';

            // Insert iFrame at the end of the body element
            body.insertBefore(iFrame, body.childNodes[body.childNodes.length]);

            // Store ref to iFrame
            __IFRAME = iFrame;

            // Fire config inside the frame
            __IFRAME.contentWindow.postMessage({
                action: 'config', args: [false, 'EntityNameSingular']
            }, document.location.origin);

            // Create IE + others compatible event handler
            var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent",
                postMessageListener = window[eventMethod],
                messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

            // Listen to events from inside the iFrame
            postMessageListener(messageEvent, _internal.handleMessage);
        },

        handleMessage: function (e) {
            switch (e.data.type) {
                case 'MODAL_CLOSE':
                    __IFRAME.style.display = 'none';
                    break;
            }
        },

        bindToolbar: function () {
            var toolbar = document.getElementById('cofoundry-sv'),
                siteViewerMode = _internal.model.siteViewerMode;

            // Internal refs
            __TOOLBAR = toolbar;

            if (siteViewerMode == 'Draft' || siteViewerMode == 'Edit') {
                // Insert publish button
                _toolBar.addButton({
                    icon: 'fa-cloud-upload',
                    title: 'Publish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.publish
                });
            } else if (siteViewerMode == 'Live') {
                // Insert unpublish button
                _toolBar.addButton({
                    icon: 'fa-cloud-download',
                    title: 'Unpublish',
                    type: 'primary',
                    classNames: 'publish popup',
                    click: _internal.unpublish
                });
            } else if (siteViewerMode == 'SpecificVersion') {
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

        // Actions
        publish: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'publish', args: [{ entityId: 1 }]
            }, document.location.origin);
            __IFRAME.style.display = 'block';
        },

        unpublish: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'unpublish', args: [{ entityId: 1 }]
            }, document.location.origin);
        },

        copyToDraft: function (e) {
            e.preventDefault();
            __IFRAME.contentWindow.postMessage({
                action: 'copyToDraft', args: [{
                    entityId: 1,
                    versionId: 1,
                    hasDraftVersion: false
                }]
            }, document.location.origin);
        }
    }

    var _toolBar = {
        addButton: function (options) {
            var type = options.type || 'secondary',
                btn = document.createElement('a');

            btn.className = (type == 'primary' ? 'cofoundry-sv__options__button' : 'cofoundry-sv__mode__button') + ' ' + options.classNames;
            btn.innerHTML = options.title;

            if (options.click) {
                btn.href = '#';
                btn.addEventListener('click', options.click, false);
            } else {
                btn.href = options.href || '#';
            }

            if (options.icon) {
                var icon = document.createElement('span');
                icon.className = 'fa ' + options.icon;
                btn.insertBefore(icon, btn.firstChild);
            }

            __TOOLBAR_BUTTONS = __TOOLBAR.getElementsByClassName(type == 'primary' ? 'cofoundry-sv__options' : 'cofoundry-sv__mode')[0];
            if (__TOOLBAR_BUTTONS) {
                __TOOLBAR_BUTTONS.appendChild(btn);
            }
        }
    }

    window.onload = function () {
        // pageResponseData object is a serialized object inserted into the page
        _internal.model = pageResponseData;
        _internal.createIframe();
        _internal.bindToolbar();
    }

    // Return public API
    return {
        toolBar: _toolBar
    }

})();