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
        var popover = new PageRegionPopOver(scope, el);
        addMouseEvents(doc, popover);

        // Add class to iFrame doc. Used for admin UI 
        angular.element(doc)
            .find('html')
            .addClass(options.isCustomEntityRoute ? 'cofoundry-editmode__custom-entity' : 'cofoundry-editmode__page');
    }

    /**
     * This gets called when initializing the page mouse events, but is also used
     * to rebind the mouse events when a region element is updated (since jqLite does
     * not support event delegation)
     */
    function addMouseEvents(rootElement, popover) {
        var entityType = options.isCustomEntityRoute ? 'custom-entity' : 'page';
        addEventsForComponent('region', popover.showRegion, popover.hideRegion);
        addEventsForComponent('region-block', popover.showBlock, popover.hideBlock);

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

    function PageRegionPopOver(scope, siteFrameEl) {
        var me = this,
            childScope = scope.$new(),
            template = $compile('<cms-page-region></cms-page-region>'),
            popover;

        /* Create element */
        childScope.siteFrameEl = siteFrameEl;
        childScope.refreshContent = refreshContent;
        popover = template(childScope);
        $document.find('body').eq(0).append(popover);

        /* Add show/hide functions */

        me.showRegion = wrapInApply(function (el) {
            if (!childScope.isRegionOver || el != childScope.regionAnchorElement) {
                childScope.isRegionOver = true;
                childScope.regionAnchorElement = el;
            }
        });

        me.hideRegion = wrapInApply(function () {
            childScope.isRegionOver = false;
        });

        me.showBlock = wrapInApply(function (el) {
            if (!childScope.isBlockOver || el != childScope.blockAnchorElement) {
                childScope.isBlockOver = true;
                childScope.blockAnchorElement = el;
            }
        });

        me.hideBlock = wrapInApply(function () {
            childScope.isBlockOver = false;
        });

        function wrapInApply(fn) {
            return function (el) {
                scope.$apply(fn.bind(null, el));
            };
        }

        /**
         * Invoked when a region or block has been modified, we give the option to re-load various parts of
         * the dom, all of which involve re-loading the entire document from the server.
         */
        function refreshContent(options) {
            if (options.pageTemplateRegionId) {
                return reloadElementBySelector('[data-cms-page-template-region-id="' + options.pageTemplateRegionId + '"]');
            } else if (options.versionBlockId) {
                return reloadElementBySelector('[data-cms-version-block-id="' + options.versionBlockId + '"]');
            } else {
                return reloadPage();
            }
        }

        function getPageContent() {

            return $http.get(siteFrameEl[0].contentWindow.location.href);
        }

        function reloadElementBySelector(selector) {
            var oldRegionElement = siteFrameEl[0].contentDocument.querySelector(selector),
                loadingNode = oldRegionElement.cloneNode(true),
                hoveredBlock = loadingNode.querySelector('.cofoundry-sv__hover-block');

            // set loading, clone the node to remove event handlers
            loadingNode.className += ' cofoundry-sv__region-loading';
            if (hoveredBlock) hoveredBlock.className = hoveredBlock.className.replace('cofoundry-sv__hover-block', '');
            oldRegionElement.parentNode.replaceChild(loadingNode, oldRegionElement);

            return getPageContent().then(function (content) {
                var html = loadHtmlStringToElement(content.data);

                // clear all hover states before we remove the element events
                childScope.isBlockOver = false;
                childScope.isRegionOver = false;

                var newRegionElement = html.querySelector(selector);
                if (newRegionElement) {

                    loadingNode.parentNode.replaceChild(newRegionElement, loadingNode);
                    addMouseEvents(newRegionElement, me);
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