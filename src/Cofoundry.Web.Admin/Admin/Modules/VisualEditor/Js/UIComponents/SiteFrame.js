angular.module('cms.visualEditor').directive('cmsSitePageFrame', [
    '$window',
    '$templateCache',
    '$compile',
    '$document',
    '$q',
    '$http',
    'visualEditor.options',
function (
    $window,
    $templateCache,
    $compile,
    $document,
    $q,
    $http,
    options) {

    return {
        restrict: 'A',
        link: link
    };

    /* GLOBALS */
    var pageLoadDeferred;


    /* CONTROLLER */

    function link(scope, el) {

        el.ready(onFrameLoaded);

        function onFrameLoaded() {
            scope.$apply(function () {
                var win = el[0].contentWindow;
                var doc = el[0].contentDocument;

                initEditableContent(scope, doc, el);

                win.addEventListener('scroll', function (e) {
                    scope.scrolled = angular.element(this)[0].scrollY;
                    scope.$apply();
                });

                win.addEventListener('resize', function (e) {
                    scope.resized = angular.element(this)[0].innerWidth;
                });

                if (pageLoadDeferred) {
                    pageLoadDeferred.resolve();
                }
            });
        }
    }

    /* HELPERS */

    function initEditableContent(scope, doc, el) {
        var popover = new PageSectionPopOver(scope, el);
        addMouseEvents(doc, popover);

        // Add class to iFrame doc. Used for admin UI 
        angular.element(doc)
            .find('html')
            .addClass(options.isCustomEntityRoute ? 'cofoundry-editmode__custom-entity' : 'cofoundry-editmode__page');
    }

    /**
     * This gets called when initializing the page mouse events, but is also used
     * to rebind the mouse events when a section element is updated (since jqLite does
     * not support event delegation)
     */
    function addMouseEvents(rootElement, popover) {
        var entityType = options.isCustomEntityRoute ? 'custom-entity' : 'page';
        addEventsForComponent('section', popover.showSection, popover.hideSection);
        addEventsForComponent('section-module', popover.showModule, popover.hideModule);

        function addEventsForComponent(componentName, showFn, hideFn) {
            var selectorAttribute = 'data-cms-' + entityType + '-' + componentName,
                elements, len, i;

            if (rootElement.hasAttribute && rootElement.hasAttribute(selectorAttribute)) {
                // we are passing the element directly to bind events
                elements = [rootElement];
            } else {
                // we need to query the children to find elements to bind events to
                elements = rootElement.querySelectorAll('[' + selectorAttribute + ']');
            }

            len = elements.length;
            for (i = 0; i < len; ++i) {

                elements[i].addEventListener('mouseenter', showFn.bind(null, angular.element(elements[i])));
                elements[i].addEventListener('mouseleave', hideFn);
            }
        }
    }

    function PageSectionPopOver(scope, siteFrameEl) {
        var me = this,
            childScope = scope.$new(),
            template = $compile('<cms-page-section></cms-page-section>'),
            popover;

        /* Create element */
        childScope.siteFrameEl = siteFrameEl;
        childScope.refreshContent = refreshContent;
        popover = template(childScope);
        $document.find('body').eq(0).append(popover);

        /* Add show/hide functions */

        me.showSection = wrapInApply(function (el) {
            if (!childScope.isSectionOver || el != childScope.sectionAnchorElement) {
                childScope.isSectionOver = true;
                childScope.sectionAnchorElement = el;
            }
        });

        me.hideSection = wrapInApply(function () {
            childScope.isSectionOver = false;
        });

        me.showModule = wrapInApply(function (el) {
            if (!childScope.isModuleOver || el != childScope.moduleAnchorElement) {
                childScope.isModuleOver = true;
                childScope.moduleAnchorElement = el;
            }
        });

        me.hideModule = wrapInApply(function () {
            childScope.isModuleOver = false;
        });

        function wrapInApply(fn) {
            return function (el) {
                scope.$apply(fn.bind(null, el));
            };
        }

        /**
         * Invoked when a section or module has been modified, we give the option to re-load various parts of
         * the dom, all of which involve re-loading the entire document from the server.
         */
        function refreshContent(options) {
            if (options.pageTemplateSectionId) {
                return reloadElementBySelector('[data-cms-page-template-section-id="' + options.pageTemplateSectionId + '"]');
            } else if (options.versionModuleId) {
                return reloadElementBySelector('[data-cms-version-module-id="' + options.versionModuleId + '"]');
            } else {
                return reloadPage();
            }
        }

        function getPageContent() {

            return $http.get(siteFrameEl[0].contentWindow.location.href);
        }

        function reloadElementBySelector(selector) {
            var oldSectionElement = siteFrameEl[0].contentDocument.querySelector(selector),
                loadingNode = oldSectionElement.cloneNode(true),
                hoveredModule = loadingNode.querySelector('.cofoundry-sv__hover-module');

            // set loading, clone the node to remove event handlers
            loadingNode.className += ' cofoundry-sv__section-loading';
            if (hoveredModule) hoveredModule.className = hoveredModule.className.replace('cofoundry-sv__hover-module', '');
            oldSectionElement.parentNode.replaceChild(loadingNode, oldSectionElement);

            return getPageContent().then(function (content) {
                var html = loadHtmlStringToElement(content.data);

                // clear all hover states before we remove the element events
                childScope.isModuleOver = false;
                childScope.isSectionOver = false;

                var newSectionElement = html.querySelector(selector);
                if (newSectionElement) {

                    loadingNode.parentNode.replaceChild(newSectionElement, loadingNode);
                    addMouseEvents(newSectionElement, me);
                }

                triggerClientEvent(siteFrameEl, 'pageContentReloaded', {

                });
            });
        }

        function reloadPage() {
            siteFrameEl[0].contentWindow.location.reload();

            pageLoadDeferred = $q.defer();
            return pageLoadDeferred.promise;
        }

        function loadHtmlStringToElement(htmlString) {
            var d = document.createElement('div');
            d.innerHTML = htmlString;

            return d;
        }

        function triggerClientEvent(siteFrameEl, event, data) {
            var win = siteFrameEl[0].contentWindow;
            if (win.CMS) {
                win.CMS.events.trigger(event, data);
            }
        }
    }
}]);