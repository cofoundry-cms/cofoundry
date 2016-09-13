angular.module('cms.shared').factory('shared.pageModalDialogService', [
    'shared.pageService',
    'shared.modalDialogService',
function (
    pageService,
    modalDialogService) {

    var service = {};

    /* PUBLIC */

    service.publish = function (pageId, onLoadingStart) {
        var options = {
            title: 'Publish Page',
            message: 'Are you sure you want to publish this page?',
            okButtonTitle: 'Yes, publish it',
            onOk: onOk
        };

        return modalDialogService.confirm(options);

        function onOk() {
            onLoadingStart();
            return pageService.publish(pageId);
        }
    }

    service.unpublish = function (pageId, onLoadingStart) {
        var options = {
            title: 'Unpublish Page',
            message: 'Unpublishing this page will remove it from the live site and put the page into draft status. Are you sure you want to continue?',
            okButtonTitle: 'Yes, unpublish it',
            onOk: onOk
        };

        return modalDialogService.confirm(options);

        function onOk() {
            onLoadingStart();
            return pageService.unpublish(pageId);
        }
    }

    service.copyToDraft = function (pageId, pageVersionId, hasDraft, onLoadingStart) {
        var options = {
            title: 'Copy Version',
            message: 'A draft version of this page already exists. Copying this version will delete the current draft. Do you wish to continue?',
            okButtonTitle: 'Yes, replace it',
            onOk: onOk
        };

        if (hasDraft) {
            // If there's a draft already, warn the user
            return modalDialogService
                .confirm(options);
        } else {
            // Run the command directly
            onLoadingStart();
            return runCommand();
        }

        /* helpers */

        function onOk() {
            onLoadingStart();

            return pageService
                .removeDraft(pageId)
                .then(runCommand);
        }

        function runCommand() {
            return pageService.duplicateDraft(pageId, pageVersionId);
        }
    }

    return service;
}]);