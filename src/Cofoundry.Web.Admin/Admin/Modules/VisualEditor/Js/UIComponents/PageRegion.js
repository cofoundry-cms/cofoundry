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