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