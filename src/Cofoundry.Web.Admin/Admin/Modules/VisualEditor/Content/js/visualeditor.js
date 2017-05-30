angular
    .module('cms.visualEditor', ['cms.shared'])
    .constant('_', window._)
    .constant('visualEditor.modulePath', '/Admin/Modules/VisualEditor/Js/');
/**
 * Service for managing page modules, which can either be attached to a page or a custom entity. 
 * Pass in the isCustomEntityRoute to switch between either route endpoint.
 */
angular.module('cms.visualEditor').factory('visualEditor.pageModuleService', [
    '$http',
    'shared.serviceBase',
    'visualEditor.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        pageModulesServiceBase = serviceBase + 'page-version-section-modules',
        customEntityModulesServiceBase = serviceBase + 'custom-entity-version-page-modules';

    /* QUERIES */

    service.getAllModuleTypes = function () {
        return $http.get(serviceBase + 'page-module-types/');
    }

    service.getPageVersionModuleById = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.get(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '?datatype=updatecommand');
    }   

    service.getSection = function (pageSectionId) {
        return $http.get(serviceBase + 'page-templates/0/sections/' + pageSectionId);
    }

    service.getModuleTypeSchema = function (pageModuleTypeId) {
        return $http.get(serviceBase + 'page-module-types/' + pageModuleTypeId);
    }

    /* COMMANDS */

    service.add = function (isCustomEntityRoute, command) {
        var entityName = isCustomEntityRoute ? 'customEntity' : 'page';
        command[entityName + 'VersionId'] = options.versionId;

        return $http.post(getServiceBase(isCustomEntityRoute), command);
    }

    service.update = function (isCustomEntityRoute, pageVersionModuleId, command) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId), command);
    }

    service.remove = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.delete(getIdRoute(isCustomEntityRoute, pageVersionModuleId));
    }

    service.moveUp = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '/move-up');
    }

    service.moveDown = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '/move-down');
    }

    /* HELPERS */

    function getIdRoute(isCustomEntityRoute, pageVersionModuleId) {
        return getServiceBase(isCustomEntityRoute) + '/' + pageVersionModuleId;
    }

    function getServiceBase(isCustomEntityRoute) {
        return isCustomEntityRoute ? customEntityModulesServiceBase : pageModulesServiceBase;
    }

    return service;
}]);
angular.module('cms.visualEditor').directive('cmsPageSection', [
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
        templateUrl: modulePath + 'UIComponents/PageSection.html',
        controller: ['$scope', Controller],
        link: link,
        replace: true
    };

    /* CONTROLLER */
    function Controller($scope) {

        this.getSectionParams = function () {

            return _.pick($scope, [
                'siteFrameEl',
                'refreshContent',
                'pageTemplateSectionId',
                'isMultiModule',
                'isCustomEntity',
                'permittedModuleTypes'
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
            scope.addModule = addModule;
            scope.startScrollY = 0;
            scope.currentScrollY = 0;

            scope.$watch('sectionAnchorElement', onAnchorChanged);
            scope.$watch('isSectionOver', setIsOver);
            scope.$watch('scrolled', onScroll);
            scope.$watch('resized', onResize);
        }

        /* UI Actions */
        function addModule() {

            scope.isPopupActive = true;
            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/AddModule.html',
                controller: 'AddModuleController',
                options: {
                    anchorElement: scope.sectionAnchorElement,
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    onClose: onClose,
                    refreshContent: refreshSection,
                    isCustomEntity: scope.isCustomEntity,
                    permittedModuleTypes: scope.permittedModuleTypes
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
                setAnchorOver(scope.sectionAnchorElement, true);

            } else if (!overTimer) {

                // a timeout is required to reduce flicker, any timeout value pushes
                // the changes to the next digest cycle, but also i've made it longer
                // than necessary to give a small delay on the hover out.
                overTimer = $timeout(function () {
                    scope.isOver = false;
                    setAnchorOver(scope.sectionAnchorElement, false);
                }, 300);
            }
        }

        /* Events */
        function onAnchorChanged(newAnchorElement, oldAnchorElement) {
            if (newAnchorElement) {

                scope.pageTemplateSectionId = newAnchorElement.attr('data-cms-page-template-section-id');
                scope.sectionName = newAnchorElement.attr('data-cms-page-section-name');
                scope.isMultiModule = newAnchorElement.attr('data-cms-multi-module');
                scope.permittedModuleTypes = parseModuleTypes(newAnchorElement.attr('data-cms-page-section-permitted-module-types'));
                scope.isCustomEntity = newAnchorElement[0].hasAttribute('data-cms-custom-entity-section');
                setPosition();
            }

            // Remove over css if the overTimer was cancelled
            setAnchorOver(oldAnchorElement, false)

            function parseModuleTypes(moduleTypeValue) {
                if (!moduleTypeValue) return [];

                return moduleTypeValue.split(',');
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

        /* Private Helpers */
        function refreshSection() {

            return scope.refreshContent({
                pageTemplateSectionId: scope.pageTemplateSectionId
            });
        }

        function setAnchorOver(anchorEl, isOver) {
            if (anchorEl) anchorEl.toggleClass('cofoundry-sv__hover-section', isOver);
        }
    }

}]);
angular.module('cms.visualEditor').directive('cmsPageSectionModule', [
    '$window',
    '$timeout',
    'visualEditor.pageModuleService',
    'shared.modalDialogService',
    'shared.LoadState',
    'visualEditor.modulePath',
    'visualEditor.options',
function (
    $window,
    $timeout,
    pageModuleService,
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
        templateUrl: modulePath + 'UIComponents/PageSectionModule.html',
        require: ['^cmsPageSection'],
        link: link
    };

    /* LINK */

    function link(scope, el, attrs, controllers) {
        var overTimer,
            sectionParams,
            globalLoadState = new LoadState();

        init();

        /* Init */

        function init() {
            scope.isOver = false;

            updateSectionParams();

            scope.setIsOver = setIsOver;
            scope.addModule = addModule.bind(null, 'Last');
            scope.editModule = editModule;
            scope.moveModuleUp = moveModule.bind(null, true);
            scope.moveModuleDown = moveModule.bind(null, false);
            scope.addModuleAbove = addModule.bind(null, 'BeforeItem');
            scope.addModuleBelow = addModule.bind(null, 'AfterItem');
            scope.deleteModule = deleteModule;
            scope.globalLoadState = globalLoadState;

            scope.$watch('anchorElement', onAnchorChanged);
            scope.$watch('isContainerOver', setIsOver);
            scope.$watch('scrolled', onScroll);
        }

        /* UI Actions */

        function moveModule(isUp) {
            var fn = isUp ? pageModuleService.moveUp : pageModuleService.moveDown;

            if (globalLoadState.isLoading) return;

            globalLoadState.on();

            fn(sectionParams.isCustomEntity, scope.versionModuleId)
                .then(refreshSection)
                .finally(globalLoadState.off);
        }

        function deleteModule() {
            var anchorEl = scope.anchorElement,
                isCustomEntity = sectionParams.isCustomEntity,
                options = {
                    title: 'Delete Module',
                    message: 'Are you sure you want to delete this module?',
                    okButtonTitle: 'Yes, delete it',
                    onOk: onOk
            };

            if (globalLoadState.isLoading) return;
            globalLoadState.on();

            modalDialogService.confirm(options);

            function onOk() {
                return pageModuleService
                    .remove(isCustomEntity, scope.versionModuleId)
                    .then(refreshSection)
                    .finally(globalLoadState.off);
            }
        }

        function addModule(insertMode) {

            if (globalLoadState.isLoading) return;
            globalLoadState.on();

            scope.isPopupActive = true;

            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/AddModule.html',
                controller: 'AddModuleController',
                options: {
                    anchorElement: scope.anchorElement,
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    adjacentVersionModuleId: scope.versionModuleId,
                    insertMode: insertMode,
                    refreshContent: refreshSection,
                    isCustomEntity: sectionParams.isCustomEntity,
                    permittedModuleTypes: sectionParams.permittedModuleTypes,
                    onClose: onClose
                }
            });

            function onClose() {
                scope.isPopupActive = false;
                globalLoadState.off();
            }
        }

        function editModule() {

            if (globalLoadState.isLoading) return;
            globalLoadState.on();
            scope.isPopupActive = true;

            modalDialogService.show({
                templateUrl: modulePath + 'Routes/Modals/EditModule.html',
                controller: 'EditModuleController',
                options: {
                    anchorElement: scope.anchorElement,
                    versionModuleId: scope.versionModuleId,
                    pageModuleTypeId: scope.pageModuleTypeId,
                    isCustomEntity: sectionParams.isCustomEntity,
                    refreshContent: refreshSection,
                    onClose: onClose
                }
            });

            function onClose() {
                scope.isPopupActive = false;
                globalLoadState.off();
            }
        }

        function setIsOver(isOver) {

            updateSectionParams();

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

                scope.versionModuleId = newAnchorElement.attr('data-cms-version-module-id');
                scope.pageModuleTypeId = newAnchorElement.attr('data-cms-page-module-type-id');
                setPosition(newAnchorElement, scope);
            }

            // Remove over css if the overTimer was cancelled
            setAnchorOver(oldAnchorElement, false)

            function setPosition(anchorElement, scope) {
                var siteFrameEl = sectionParams.siteFrameEl,
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
                    var popoverEl = document.getElementById('cofoundry-sv__module-popover'),
                        popoverElHeight, popoverElHeight;

                    // If moving quickly the element might have been removed
                    if (!popoverEl) return;

                    popoverElHeight = popoverEl.offsetHeight;
                    windowHeight = window.innerHeight;

                    // Move the module hover up a bit if it is off the screen
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

        function refreshSection() {
            return scope.refreshContent({
                pageTemplateSectionId: scope.pageTemplateSectionId
            });
        }

        function setAnchorOver(anchorEl, isOver) {
            if (anchorEl) anchorEl.toggleClass('cofoundry-sv__hover-module', isOver);
        }

        function updateSectionParams() {
            sectionParams = controllers[0].getSectionParams();
            scope.isMultiModule = sectionParams.isMultiModule;
            scope.pageTemplateSectionId = sectionParams.pageTemplateSectionId;
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
angular.module('cms.visualEditor').controller('VisualEditorController', [
    '$window',
    '$scope',
    '_',
    'shared.LoadState',
    'shared.entityVersionModalDialogService',
    'shared.modalDialogService',
    'shared.localStorage',
    'visualEditor.pageModuleService',
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
    pageModuleService,
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
        vm.addSectionModule = addSectionModule;
        vm.addModule = addModule;
        vm.addModuleAbove = addModule;
        vm.addModuleBelow = addModule;
        vm.editModule = editModule;
        vm.moveModuleUp = moveModule;
        vm.moveModuleDown = moveModule;
        vm.deleteModule = deleteModule;
    }

    /* UI ACTIONS */

    function handleMessage(e) {
        vm[e.data.action].apply(this, e.data.args);
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
            .then(reload)
            .catch(setLoadingOff);
    }

    function addSectionModule(args) {
        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddModule.html',
            controller: 'AddModuleController',
            options: {
                insertMode: args.insertMode,
                pageTemplateSectionId: args.pageTemplateSectionId,
                adjacentVersionModuleId: args.versionModuleId,
                permittedModuleTypes: args.permittedModuleTypes,
                onClose: onClose,
                refreshContent: refreshSection,
                isCustomEntity: args.isCustomEntity,
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function addModule(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddModule.html',
            controller: 'AddModuleController',
            options: {
                pageTemplateSectionId: args.pageTemplateSectionId,
                adjacentVersionModuleId: args.versionModuleId,
                permittedModuleTypes: args.permittedModuleTypes,
                insertMode: args.insertMode,
                refreshContent: refreshSection,
                isCustomEntity: args.isCustomEntity,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function editModule(args) {

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/EditModule.html',
            controller: 'EditModuleController',
            options: {
                versionModuleId: args.versionModuleId,
                pageModuleTypeId: args.pageModuleTypeId,
                isCustomEntity: args.isCustomEntity,
                refreshContent: refreshSection,
                onClose: onClose
            }
        });

        function onClose() {
            globalLoadState.off();
        }
    }

    function moveModule(args) {
        var fn = args.isUp ? pageModuleService.moveUp : pageModuleService.moveDown;

        if (globalLoadState.isLoading) return;

        globalLoadState.on();

        fn(args.isCustomEntity, args.versionModuleId)
            .then(refreshSection)
            .finally(globalLoadState.off);
    }

    function deleteModule(args) {
        var isCustomEntity = args.isCustomEntity,
            options = {
                title: 'Delete Module',
                message: 'Are you sure you want to delete this module?',
                okButtonTitle: 'Yes, delete it',
                onOk: onOk,
                onCancel: onCancel
            };

        if (globalLoadState.isLoading) return;
        globalLoadState.on();

        modalDialogService.confirm(options);

        function onOk() {
            return pageModuleService
                .remove(isCustomEntity, args.versionModuleId)
                .then(refreshSection)
                .finally(globalLoadState.off);
        }

        function onCancel() {
            globalLoadState.off();
        }
    }

    /* PRIVATE FUNCS */

    function refreshSection() {
        reload();
    }

    function preview() {
        var location = $window.parent.location.href;
        if (location.indexOf('mode=edit') > -1) {
            location = location.replace('mode=edit', 'mode=preview')
        }
        $window.parent.location = location;
    }

    function reload() {
        $window.parent.location = $window.parent.location;
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
    }
}]);