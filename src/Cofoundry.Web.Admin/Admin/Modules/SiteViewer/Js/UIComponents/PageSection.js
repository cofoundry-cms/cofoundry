angular.module('cms.siteViewer').directive('cmsPageSection', [
    '$window',
    '$timeout',
    '_',
    'shared.modalDialogService',
    'siteViewer.modulePath',
function (
    $window,
    $timeout,
    _,
    modalDialogService,
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'uicomponents/PageSection.html',
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
                'isCustomEntity'
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
                templateUrl: modulePath + 'routes/modals/addmodule.html',
                controller: 'AddModuleController',
                options: {
                    anchorElement: scope.sectionAnchorElement,
                    pageTemplateSectionId: scope.pageTemplateSectionId,
                    onClose: onClose,
                    refreshContent: refreshSection,
                    isCustomEntity: scope.isCustomEntity,
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
                scope.isCustomEntity = newAnchorElement[0].hasAttribute('data-cms-custom-entity-section');
                setPosition();
            }

            // Remove over css if the overTimer was cancelled
            setAnchorOver(oldAnchorElement, false)

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