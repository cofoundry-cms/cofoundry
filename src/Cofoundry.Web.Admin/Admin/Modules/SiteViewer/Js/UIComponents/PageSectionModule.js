angular.module('cms.siteViewer').directive('cmsPageSectionModule', [
    '$window',
    '$timeout',
    'siteViewer.pageModuleService',
    'shared.modalDialogService',
    'shared.LoadState',
    'siteViewer.modulePath',
    'siteViewer.options',
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
        templateUrl: modulePath + 'uicomponents/PageSectionModule.html',
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
                templateUrl: modulePath + 'routes/modals/addmodule.html',
                controller: 'AddModuleController',
                options: {
                    anchorElement: scope.anchorElement,
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    adjacentVersionModuleId: scope.versionModuleId,
                    insertMode: insertMode,
                    refreshContent: refreshSection,
                    isCustomEntity: sectionParams.isCustomEntity,
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
                templateUrl: modulePath + 'routes/modals/editmodule.html',
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
                var left = -6 + elementOffset.left - iframeDoc.scrollLeft;

                scope.css = {
                    top: top + 'px',
                    left: (left || 0) + 'px'
                };

                scope.startScroll = siteFrameEl[0].contentWindow.scrollY;
                scope.startY = top;

                // Wait for next digest cycle and check the position of the element
                $timeout(function () {
                    var popoverEl = document.getElementById('cms-module-popover'),
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
            if (anchorEl) anchorEl.toggleClass('cms-hover-module', isOver);
        }

        function updateSectionParams() {
            sectionParams = controllers[0].getSectionParams();
            scope.isMultiModule = sectionParams.isMultiModule;
            scope.pageTemplateSectionId = sectionParams.pageTemplateSectionId;
        }
    }
}]);