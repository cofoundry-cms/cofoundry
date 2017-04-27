import { ViewContainerRef } from '@angular/core';
import { EntityVersionService, ModalDialogService } from './index';

export class EntityVersionModalDialogService {
    viewContainer: ViewContainerRef;
    pageEntityConfig = {
        entityNameSingular: 'Page'
    };

    constructor(
        private entityVersionService: EntityVersionService,
        private modalDialogService: ModalDialogService) {}

    /* PUBLIC */

    registerViewContainer(viewContainer: ViewContainerRef) {
        this.viewContainer = viewContainer;
    }

    publish(entityId, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || this.pageEntityConfig;

        var options = {
            title: 'Publish ' + config.entityNameSingular,
            message: 'Are you sure you want to publish this ' + config.entityNameSingular.toLowerCase() + '?',
            okButtonTitle: 'Yes, publish it',
            onOk: () => {
                onLoadingStart();
                return this.entityVersionService.publish(config.isCustomEntity, entityId);
            }
        };

        return this.modalDialogService.confirm(options);
    }

    unpublish(entityId, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || this.pageEntityConfig;

        var options = {
            title: 'Unpublish ' + config.entityNameSingular,
            message: 'Unpublishing this ' + config.entityNameSingular.toLowerCase() + ' will remove it from the live site and put it into draft status. Are you sure you want to continue?',
            okButtonTitle: 'Yes, unpublish it',
            onOk: onOk
        };

        return this.modalDialogService.confirm(options);

        function onOk() {
            onLoadingStart();

            return this.entityVersionService.unpublish(config.isCustomEntity, entityId);
        }
    }

    copyToDraft(entityId, entityVersionId, hasDraft, onLoadingStart, customEntityConfig) {
        var config = customEntityConfig || this.pageEntityConfig;

        var options = {
            title: 'Copy ' + config.entityNameSingular + ' Version',
            message: 'A draft version of this ' + config.entityNameSingular.toLowerCase() + ' already exists. Copying this version will delete the current draft. Do you wish to continue?',
            okButtonTitle: 'Yes, replace it',
            onOk: () => {
                onLoadingStart();
                return this.entityVersionService
                    .removeDraft(config.isCustomEntity, entityId)
                    .subscribe(() => {
                        return this.entityVersionService.duplicateDraft(config.isCustomEntity, entityId, entityVersionId);
                    });
            }
        };

        if (hasDraft) {
            // If there's a draft already, warn the user
            return this.modalDialogService
                .confirm(options);
        } else {
            // Run the command directly
            onLoadingStart();
            return runCommand();
        }

        function runCommand() {
            return this.entityVersionService.duplicateDraft(config.isCustomEntity, entityId, entityVersionId);
        }
    }
}