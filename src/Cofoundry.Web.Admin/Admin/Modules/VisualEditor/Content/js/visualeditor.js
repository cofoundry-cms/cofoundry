angular
    .module('cms.visualEditor', ['cms.shared'])
    .constant('_', window._)
    .constant('visualEditor.modulePath', '/Admin/Modules/VisualEditor/Js/');
/**
 * Service for managing page block, which can either be attached to a page or a custom entity.
 * Pass in the isCustomEntityRoute to switch between either route endpoint.
 */
angular.module('cms.visualEditor').factory('visualEditor.pageBlockService', [
    '$http',
    'shared.serviceBase',
    'visualEditor.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        pageBlocksServiceBase = serviceBase + 'page-version-region-blocks',
        customEntityBlocksServiceBase = serviceBase + 'custom-entity-version-page-blocks';

    /* QUERIES */

    service.getAllBlockTypes = function () {
        return $http.get(serviceBase + 'page-block-types/');
    }

    service.getPageVersionBlockById = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.get(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '?datatype=updatecommand');
    }   

    service.getRegion = function (pageRegionId) {
        return $http.get(serviceBase + 'page-templates/0/regions/' + pageRegionId);
    }

    service.getBlockTypeSchema = function (pageBlockTypeId) {
        return $http.get(serviceBase + 'page-block-types/' + pageBlockTypeId);
    }

    /* COMMANDS */

    service.add = function (isCustomEntityRoute, command) {
        var entityName = isCustomEntityRoute ? 'customEntity' : 'page';
        command[entityName + 'VersionId'] = options.versionId;

        return $http.post(getServiceBase(isCustomEntityRoute), command);
    }

    service.update = function (isCustomEntityRoute, pageVersionBlockId, command) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId), command);
    }

    service.remove = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.delete(getIdRoute(isCustomEntityRoute, pageVersionBlockId));
    }

    service.moveUp = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-up');
    }

    service.moveDown = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-down');
    }

    /* HELPERS */

    function getIdRoute(isCustomEntityRoute, pageVersionBlockId) {
        return getServiceBase(isCustomEntityRoute) + '/' + pageVersionBlockId;
    }

    function getServiceBase(isCustomEntityRoute) {
        return isCustomEntityRoute ? customEntityBlocksServiceBase : pageBlocksServiceBase;
    }

    return service;
}]);
angular.module('cms.visualEditor').directive('cmsPageRegion', [
    '$window',
    '$timeout',
    '_',
    'shared.modalDialogService',
    'visualEditor.modulePath',
function (
    $window,
    $timeout,
    _,
    modalDialogService,
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/PageRegion.html',
        controller: ['$scope', Controller],
        link: link,
        replace: true
    };

    /* CONTROLLER */
    function Controller($scope) {

        this.getRegionParams = function () {

            return _.pick($scope, [
                'siteFrameEl',
                'refreshContent',
                'pageTemplateRegionId',
                'isMultiBlock',
                'isCustomEntity',
                'permittedBlockTypes'
            ]);
        }
    };

    /* LINK */
    function link(scope, el, attrs) {
        var overTimer;

        init();

        /* Init */
        function init() {

            scope.isOver = false;

            scope.setIsOver = setIsOver;
            scope.addBlock = addBlock;
            scope.startScrollY = 0;
            scope.currentScrollY = 0;

            scope.$watch('regionAnchorElement', onAnchorChanged);
            scope.$watch('isRegionOver', setIsOver);
            scope.$watch('scrolled', onScroll);
            scope.$watch('resized', onResize);
        }

        /* UI Actions */
        function addBlock() {

            scope.isPopupActive = true;
            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
                controller: 'AddBlockController',
                options: {
                    anchorElement: scope.regionAnchorElement,
                    pageTemplateRegionId: scope.pageTemplateRegionId,
                    onClose: onClose,
                    refreshContent: refreshRegion,
                    isCustomEntity: scope.isCustomEntity,
                    permittedBlockTypes: scope.permittedBlockTypes
                }
            });

            function onClose() {
                scope.isPopupActive = false;
            }
        }

        function setIsOver(isOver) {
            if (isOver) {
                if (overTimer) {
                    $timeout.cancel(overTimer);
                    overTimer = null;
                }
                scope.isOver = true;
                setAnchorOver(scope.regionAnchorElement, true);

            } else if (!overTimer) {

                // a timeout is required to reduce flicker, any timeout value pushes
                // the changes to the next digest cycle, but also i've made it longer
                // than necessary to give a small delay on the hover out.
                overTimer = $timeout(function () {
                    scope.isOver = false;
                    setAnchorOver(scope.regionAnchorElement, false);
                }, 300);
            }
        }

        /* Events */
        function onAnchorChanged(newAnchorElement, oldAnchorElement) {
            if (newAnchorElement) {

                scope.pageTemplateRegionId = newAnchorElement.attr('data-cms-page-template-region-id');
                scope.regionName = newAnchorElement.attr('data-cms-page-region-name');
                scope.isMultiBlock = newAnchorElement.attr('data-cms-multi-block');
                scope.permittedBlockTypes = parseBlockTypes(newAnchorElement.attr('data-cms-page-region-permitted-block-types'));
                scope.isCustomEntity = newAnchorElement[0].hasAttribute('data-cms-custom-entity-region');
                setPosition();
            }

            // Remove over css if the overTimer was cancelled
            setAnchorOver(oldAnchorElement, false)

            function parseBlockTypes(blockTypeValue) {
                if (!blockTypeValue) return [];

                return blockTypeValue.split(',');
            }

            function setPosition() {
                var siteFrameEl = scope.siteFrameEl,
                    elementOffset = newAnchorElement.offset(),
                    siteFrameOffset = siteFrameEl.offset(),
                    iframeDoc = siteFrameEl[0].contentDocument.documentElement;

                var top = elementOffset.top + siteFrameOffset.top - iframeDoc.scrollTop + 2;

                if (top < siteFrameOffset.top) {
                    top = siteFrameOffset.top;
                }

                // popover hovers in the right hand corner of the element
                var left = (($window.innerWidth - siteFrameEl[0].clientWidth) / 2) + (elementOffset.left + newAnchorElement[0].offsetWidth);

                scope.css = {
                    top: top + 'px',
                    left: (left || 0) + 'px'
                };

                scope.startScrollY = scope.currentScrollY;
                scope.startY = top;
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

        /* Private Helpers */
        function refreshRegion() {

            return scope.refreshContent({
                pageTemplateRegionId: scope.pageTemplateRegionId
            });
        }

        function setAnchorOver(anchorEl, isOver) {
            if (anchorEl) anchorEl.toggleClass('cofoundry-sv__hover-region', isOver);
        }
    }

}]);
angular.module('cms.visualEditor').directive('cmsPageRegionBlock', [
    '$window',
    '$timeout',
    'visualEditor.pageBlockService',
    'shared.modalDialogService',
    'shared.LoadState',
    'visualEditor.modulePath',
    'visualEditor.options',
function (
    $window,
    $timeout,
    pageBlockService,
    modalDialogService,
    LoadState,
    modulePath,
    options
    ) {

    return {
        scope: {
            anchorElement: '=',
            isContainerOver: '=',
            refreshContent: '=',
            scrolled: '='
        },
        templateUrl: modulePath + 'UIComponents/PageRegionBlock.html',
        require: ['^cmsPageRegion'],
        link: link
    };

    /* LINK */

    function link(scope, el, attrs, controllers) {
        var overTimer,
            regionParams,
            globalLoadState = new LoadState();

        init();

        /* Init */

        function init() {
            scope.isOver = false;

            updateRegionParams();

            scope.setIsOver = setIsOver;
            scope.addBlock = addBlock.bind(null, 'Last');
            scope.editBlock = editBlock;
            scope.moveBlockUp = moveBlock.bind(null, true);
            scope.moveBlockDown = moveBlock.bind(null, false);
            scope.addBlockAbove = addBlock.bind(null, 'BeforeItem');
            scope.addBlockBelow = addBlock.bind(null, 'AfterItem');
            scope.deleteBlock = deleteBlock;
            scope.globalLoadState = globalLoadState;

            scope.$watch('anchorElement', onAnchorChanged);
            scope.$watch('isContainerOver', setIsOver);
            scope.$watch('scrolled', onScroll);
        }

        /* UI Actions */

        function moveBlock(isUp) {
            var fn = isUp ? pageBlockService.moveUp : pageBlockService.moveDown;

            if (globalLoadState.isLoading) return;

            globalLoadState.on();

            fn(regionParams.isCustomEntity, scope.versionBlockId)
                .then(refreshRegion)
                .finally(globalLoadState.off);
        }

        function deleteBlock() {
            var anchorEl = scope.anchorElement,
                isCustomEntity = regionParams.isCustomEntity,
                options = {
                    title: 'Delete Content Block',
                    message: 'Are you sure you want to delete this content block?',
                    okButtonTitle: 'Yes, delete it',
                    onOk: onOk
            };

            if (globalLoadState.isLoading) return;
            globalLoadState.on();

            modalDialogService.confirm(options);

            function onOk() {
                return pageBlockService
                    .remove(isCustomEntity, scope.versionBlockId)
                    .then(refreshRegion)
                    .finally(globalLoadState.off);
            }
        }

        function addBlock(insertMode) {

            if (globalLoadState.isLoading) return;
            globalLoadState.on();

            scope.isPopupActive = true;

            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
                controller: 'AddBlockController',
                options: {
                    anchorElement: scope.anchorElement,
                    pageTemplateRegionId: scope.pageTemplateRegionId,
                    adjacentVersionBlockId: scope.versionBlockId,
                    insertMode: insertMode,
                    refreshContent: refreshRegion,
                    isCustomEntity: regionParams.isCustomEntity,
                    permittedBlockTypes: regionParams.permittedBlockTypes,
                    onClose: onClose
                }
            });

            function onClose() {
                scope.isPopupActive = false;
                globalLoadState.off();
            }
        }

        function editBlock() {

            if (globalLoadState.isLoading) return;
            globalLoadState.on();
            scope.isPopupActive = true;

            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/EditBlock.html',
                controller: 'EditBlockController',
                options: {
                    anchorElement: scope.anchorElement,
                    versionBlockId: scope.versionBlockId,
                    pageBlockTypeId: scope.pageBlockTypeId,
                    isCustomEntity: regionParams.isCustomEntity,
                    refreshContent: refreshRegion,
                    onClose: onClose
                }
            });

            function onClose() {
                scope.isPopupActive = false;
                globalLoadState.off();
            }
        }

        function setIsOver(isOver) {

            updateRegionParams();

            if (isOver) {
                if (overTimer) {
                    $timeout.cancel(overTimer);
                    overTimer = null;
                }
                scope.isOver = true;
                setAnchorOver(scope.anchorElement, true);

            } else if (!overTimer) {

                // a timeout is required to reduce flicker, any timeout value pushes
                // the changes to the next digest cycle, but also i've made it longer
                // than necessary to give a small delay on the hover out.
                overTimer = $timeout(function () {
                    scope.isOver = false;
                    setAnchorOver(scope.anchorElement, false);
                }, 300);
            }
        }

        /* Events */

        function onAnchorChanged(newAnchorElement, oldAnchorElement) {

            if (newAnchorElement) {

                scope.versionBlockId = newAnchorElement.attr('data-cms-version-block-id');
                scope.pageBlockTypeId = newAnchorElement.attr('data-cms-page-block-type-id');
                setPosition(newAnchorElement, scope);
            }

            // Remove over css if the overTimer was cancelled
            setAnchorOver(oldAnchorElement, false)

            function setPosition(anchorElement, scope) {
                var siteFrameEl = regionParams.siteFrameEl,
                    elementOffset = anchorElement.offset(),
                    siteFrameOffset = siteFrameEl.offset(),
                    iframeDoc = siteFrameEl[0].contentDocument.documentElement;

                var top = elementOffset.top + siteFrameOffset.top - iframeDoc.scrollTop + 2;

                if (top < siteFrameOffset.top) {
                    top = siteFrameOffset.top;
                }

                // popover hovers in the left hand corner of the element
                var left = (elementOffset.left - iframeDoc.scrollLeft) + (($window.innerWidth - siteFrameEl[0].clientWidth) / 2) + 2;

                scope.css = {
                    top: top + 'px',
                    left: (left || 0) + 'px'
                };

                scope.startScroll = siteFrameEl[0].contentWindow.scrollY;
                scope.startY = top;

                // Wait for next digest cycle and check the position of the element
                $timeout(function () {
                    var popoverEl = document.getElementById('cofoundry-sv__block-popover'),
                        popoverElHeight, popoverElHeight;

                    // If moving quickly the element might have been removed
                    if (!popoverEl) return;

                    popoverElHeight = popoverEl.offsetHeight;
                    windowHeight = window.innerHeight;

                    // Move the block hover up a bit if it is off the screen
                    if (popoverEl.offsetTop + popoverElHeight > windowHeight) {
                        scope.css.top = windowHeight - popoverElHeight + 'px';
                    }
                }, 1);
            }
        }

        function onScroll(e) {
            var y = (scope.startY - e) + scope.startScroll;
            if (y) {
                scope.css = {
                    top: y + 'px',
                    left: scope.css.left
                }
            }
        }

        /* Private Helpers */

        function refreshRegion() {
            return scope.refreshContent({
                pageTemplateRegionId: scope.pageTemplateRegionId
            });
        }

        function setAnchorOver(anchorEl, isOver) {
            if (anchorEl) anchorEl.toggleClass('cofoundry-sv__hover-block', isOver);
        }

        function updateRegionParams() {
            regionParams = controllers[0].getRegionParams();
            scope.isMultiBlock = regionParams.isMultiBlock;
            scope.pageTemplateRegionId = regionParams.pageTemplateRegionId;
        }
    }
}]);
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
angular.module('cms.visualEditor').controller('VisualEditorController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'shared.modalDialogService',
    'shared.localStorage',
    'visualEditor.pageBlockService',
    'visualEditor.modulePath',
    'shared.urlLibrary',
    'visualEditor.options',
function (
    $window,
    $scope,
    _,
    LoadState,
    entityVersionModalDialogService,
    modalDialogService,
    localStorageService,
    pageBlockService,
    modulePath,
    urlLibrary,
    options
    ) {

    var vm = this,
        document = $window.document,
        entityDialogServiceConfig,
        globalLoadState = new LoadState();

    init();

    /* INIT */

    function init() {
        // Create IE + others compatible event handler
        var eventMethod = $window.addEventListener ? "addEventListener" : "attachEvent",
            postMessageListener = window[eventMethod],
            messageEvent = eventMethod === "attachEvent" ? "onmessage" : "message";

        postMessageListener(messageEvent, handleMessage);

        vm.globalLoadState = globalLoadState;
        vm.config = config;
        vm.publish = publish;
        vm.unpublish = unpublish;
        vm.copyToDraft = copyToDraft;
        vm.addRegionBlock = addRegionBlock;
        vm.addBlock = addBlock;
        vm.addBlockAbove = addBlock;
        vm.addBlockBelow = addBlock;
        vm.editBlock = editBlock;
        vm.moveBlockUp = moveBlock;
        vm.moveBlockDown = moveBlock;
        vm.deleteBlock = deleteBlock;
    }

    /* UI ACTIONS */

    function handleMessage(e) {
        if (e.data.action && vm[e.data.action]) {
            vm[e.data.action].apply(this, e.data.args);
        }
    }

    function config() {
        entityDialogServiceConfig = {
            entityNameSingular: options.entityNameSingular,
            isCustomEntity: options.isCustomEntityRoute
        };
    }

    function publish(args) {
        entityVersionModalDialogService
            .publish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(preview)
            .catch(setLoadingOff);
    }

    function unpublish(args) {
        entityVersionModalDialogService
            .unpublish(args.entityId, setLoadingOn, entityDialogServiceConfig)
            .then(reload)
            .catch(setLoadingOff);
    }

    function copyToDraft(args) {
        entityVersionModalDialogService
            .copyToDraft(args.entityId, args.versionId, args.hasDraftVersion, setLoadingOn, entityDialogServiceConfig)
            .then(reloadWithoutVersion)
            .catch(setLoadingOff);
    }

    function addRegionBlock(args) {
        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
            controller: 'AddBlockController',
            options: {
                insertMode: args.insertMode,
                pageTemplateRegionId: args.pageTemplateRegionId,
                adjacentVersionBlockId: args.versionBlockId,
                permittedBlockTypes: args.permittedBlockTypes,
                onClose: onClose,
                refreshContent: refreshRegion,
                isCustomEntity: args.isCustomEntity,
                regionName: args.regionName
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function addBlock(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddBlock.html',
            controller: 'AddBlockController',
            options: {
                pageTemplateRegionId: args.pageTemplateRegionId,
                adjacentVersionBlockId: args.versionBlockId,
                permittedBlockTypes: args.permittedBlockTypes,
                insertMode: args.insertMode,
                refreshContent: refreshRegion,
                isCustomEntity: args.isCustomEntity,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function editBlock(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/EditBlock.html',
            controller: 'EditBlockController',
            options: {
                versionBlockId: args.versionBlockId,
                pageBlockTypeId: args.pageBlockTypeId,
                isCustomEntity: args.isCustomEntity,
                refreshContent: refreshRegion,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function moveBlock(args) {
        var fn = args.isUp ? pageBlockService.moveUp : pageBlockService.moveDown;

        if (globalLoadState.isLoading) return;

        globalLoadState.on();

        fn(args.isCustomEntity, args.versionBlockId)
            .then(refreshRegion)
            .finally(globalLoadState.off);
    }

    function deleteBlock(args) {
        var isCustomEntity = args.isCustomEntity,
            options = {
                title: 'Delete Block',
                message: 'Are you sure you want to delete this content block?',
                okButtonTitle: 'Yes, delete it',
                onOk: onOk,
                onCancel: onCancel
            };

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.confirm(options);

        function onOk() {
            return pageBlockService
                .remove(isCustomEntity, args.versionBlockId)
                .then(refreshRegion)
                .finally(globalLoadState.off);
        }

        function onCancel() {
            globalLoadState.off();
        }
    }

    /* PRIVATE FUNCS */

    function refreshRegion() {
        reload();
    }

    function preview() {
        var location = $window.parent.location.href;
        if (location.indexOf('mode=edit') > -1) {
            location = location.replace('mode=edit', 'mode=preview');
        }
        $window.parent.location = location;
    }

    function reload() {
        $window.parent.location = $window.parent.location;
    }

    function reloadWithoutVersion() {
        var location = $window.parent.location.href;
        // remove the version query parameter
        location = location.replace(/(.+\?(?:.+&|))(version=\d+&?)(.*)/i, '$1$3');
        $window.parent.location = location
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);
angular.module('cms.visualEditor').controller('AddBlockController', [
    '$scope',
    '$q',
    '_',
    'shared.LoadState',
    'visualEditor.pageBlockService',
    'visualEditor.options',
    'options',
    'close',
function (
    $scope,
    $q,
    _,
    LoadState,
    pageBlockService,
    visualEditorOptions,
    options,
    close) {

    init();
    
    /* INIT */

    function init() {
        var anchorElement = options.anchorElement;

        $scope.command = { 
            dataModel: {},
            pageTemplateRegionId: options.pageTemplateRegionId,
            pageVersionId: visualEditorOptions.pageVerisonId,
            adjacentVersionBlockId: options.adjacentVersionBlockId,
            insertMode: options.insertMode || 'Last'
        };

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = onClose;
        $scope.selectBlockType = selectBlockType;
        $scope.selectBlockTypeAndNext = selectBlockTypeAndNext;
        $scope.isBlockTypeSelected = isBlockTypeSelected;
        $scope.setStep = setStep;

        initData();
    }

    /* EVENTS */

    function initData() {
        setStep(1);

        pageBlockService
            .getAllBlockTypes()
            .then(onLoaded);

        function onLoaded(allBlockTypes) {;
            $scope.title = options.regionName;

            if (options.permittedBlockTypes.length) {
                // Filter the permitted blocks list to those specified
                $scope.blockTypes = _.filter(allBlockTypes, function (blockType) {
                    return _.contains(options.permittedBlockTypes, blockType.fileName);
                });
            } else {
                // Empty means 'all' block types
                $scope.blockTypes = allBlockTypes;
            }

            if ($scope.blockTypes.length === 1) {
                $scope.command.pageBlockTypeId = $scope.blockTypes[0].pageBlockTypeId;
                setStep(2);
            } else {
                $scope.allowStep1 = true;
            }

            $scope.formLoadState.off();
        }
    }

    function save() {

        $scope.submitLoadState.on();

        pageBlockService
            .add(options.isCustomEntity, $scope.command)
            .then(options.refreshContent)
            .then(onClose)
            .finally($scope.submitLoadState.off);
    }

    function onClose() {
        close();
        options.onClose();
    }

    function setStep(step) {
        $scope.currentStep = step;

        if (step === 2) {
            loadStep2();
        }
    }

    function loadStep2() {
        $scope.formLoadState.on();

        pageBlockService
            .getBlockTypeSchema($scope.command.pageBlockTypeId)
            .then(onLoaded);

        function onLoaded(modelMetaData) {

            $scope.formDataSource = {
                modelMetaData: modelMetaData,
                model: $scope.command.dataModel
            };

            $scope.templates = modelMetaData.templates;

            $scope.formLoadState.off();
        }
    }

    function selectBlockType(blockType) {
        $scope.command.pageBlockTypeId = blockType && blockType.pageBlockTypeId;
    }

    function selectBlockTypeAndNext(blockType) {
        selectBlockType(blockType);
        setStep(2);
    }

    /* PUBLIC HELPERS */

    function isBlockTypeSelected(blockType) {
        return blockType && blockType.pageBlockTypeId === $scope.command.pageBlockTypeId;
    }

}]);
angular.module('cms.visualEditor').controller('EditBlockController', [
    '$scope',
    '$q',
    '_',
    'shared.LoadState',
    'visualEditor.pageBlockService',
    'visualEditor.options',
    'options',
    'close',
function (
    $scope,
    $q,
    _,
    LoadState,
    pageBlockService,
    visualEditorOptions,
    options,
    close) {

    init();
    
    /* INIT */

    function init() {
        var anchorElement = options.anchorElement;

        $scope.command = { 
            dataModel: {}
        };
        
        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = onClose;

        initData();
    }

    /* EVENTS */

    function initData() {
        var formDataSource = {},
            blockTypeSchemaDeferred, blockDataDeferred;

        $scope.formLoadState.on();

        blockTypeSchemaDeferred = pageBlockService
            .getBlockTypeSchema(options.pageBlockTypeId)
            .then(onMetaDataLoaded);

        blockDataDeferred = pageBlockService
            .getPageVersionBlockById(options.isCustomEntity, options.versionBlockId)
            .then(onModelLoaded);

        var q = $q.all([blockTypeSchemaDeferred, blockDataDeferred]).then(onLoadComplete);

        function onMetaDataLoaded(modelMetaData) {
            $scope.templates = modelMetaData.templates;
            formDataSource.modelMetaData = modelMetaData;
        }

        function onModelLoaded(model) {
            $scope.command = model;
            formDataSource.model = model.dataModel;
        }

        function onLoadComplete() {

            $scope.formDataSource = formDataSource;
            $scope.formLoadState.off();
        }
    }

    function save() {

        $scope.submitLoadState.on();

        pageBlockService
            .update(options.isCustomEntity, options.versionBlockId, $scope.command)
            .then(options.refreshContent)
            .then(onClose)
            .finally($scope.submitLoadState.off);
    }

    function onClose() {
        close();
        options.onClose();
    }
    
}]);