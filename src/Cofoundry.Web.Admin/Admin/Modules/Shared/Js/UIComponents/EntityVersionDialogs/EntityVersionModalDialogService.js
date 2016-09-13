angular.module('cms.shared').factory('shared.entityVersionModalDialogService', [
    'shared.entityVersionService',
    'shared.modalDialogService',
function (
    entityVersionService,
    modalDialogService) {

    var service = {},
        pageEntityConfig = {
            entityNameSingular: 'Page'
        };

    /* PUBLIC */

    service.publish = function (entityId, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || pageEntityConfig;

        var options = {
            title: 'Publish ' + config.entityNameSingular,
            message: 'Are you sure you want to publish this ' + config.entityNameSingular.toLowerCase() + '?',
            okButtonTitle: 'Yes, publish it',
            onOk: onOk
        };

        return modalDialogService.confirm(options);

        function onOk() {
            onLoadingStart();
            return entityVersionService.publish(config.isCustomEntity, entityId);
        }
    }

    service.unpublish = function (entityId, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || pageEntityConfig;

        var options = {
            title: 'Unpublish ' + config.entityNameSingular,
            message: 'Unpublishing this ' + config.entityNameSingular.toLowerCase() + ' will remove it from the live site and put it into draft status. Are you sure you want to continue?',
            okButtonTitle: 'Yes, unpublish it',
            onOk: onOk
        };

        return modalDialogService.confirm(options);

        function onOk() {
            onLoadingStart();

            return entityVersionService.unpublish(config.isCustomEntity, entityId);
        }
    }

    service.copyToDraft = function (entityId, entityVersionId, hasDraft, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || pageEntityConfig;

        var options = {
            title: 'Copy ' + config.entityNameSingular + ' Version',
            message: 'A draft version of this ' + config.entityNameSingular.toLowerCase() + ' already exists. Copying this version will delete the current draft. Do you wish to continue?',
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

            return entityVersionService
                .removeDraft(config.isCustomEntity, entityId)
                .then(runCommand);
        }

        function runCommand() {
            return entityVersionService.duplicateDraft(config.isCustomEntity, entityId, entityVersionId);
        }
    }
    
    return service;
}]);