import { Injectable }    from '@angular/core';
import { URL_BASE_BASE } from '../constants/path.constants';
import { StringUtility } from '../utilities/string.utility';
import * as _ from 'lodash';

@Injectable()
export class UrlLibrary {

    constructor(private stringUtility : StringUtility) {
        /* CRUD Routes */
        this.addCrudRoutes('page', 'pages');
        this.addCrudRoutes('pageTemplate', 'page-templates');
        this.addCrudRoutes('role', 'roles');
    };

    /* General */

    makePath (module, pathParts, query) {
        var path = URL_BASE_BASE + module + '#/';

        if (_.isArray(pathParts)) {
            path += pathParts.join('/');
        } else if (pathParts != null) {
            path += pathParts;
        } 

        if (query) {
            path += '?' + this.stringUtility.toQueryString(query);
        }

        return path;
    }

    /* Asset File Routes */

    getDocumentUrl(document) {
        var url;
        if (!document) return;

        url = '/assets/files/' + document.documentAssetId + '_' + document.fileName + '.' + document.fileExtension;

        return url;
    }

    getImageUrl(img, settings) {
        var url;
        if (!img) return;

        url = '/assets/images/' + img.imageAssetId + '_' + img.fileName + '.' + img.extension;
        setDefaultCrop(img, settings);

        if (settings) {
            url = url + '?' + this.stringUtility.toQueryString(settings);
        }

        return url;

        /* Helpers */

        function setDefaultCrop(asset, settings) {
            
            if (!settings) return;

            if (isDefinedAndChanged(settings.width, asset.width) || isDefinedAndChanged(settings.height, asset.height))
            {
                if (!settings.mode)
                {
                    settings.mode = 'Crop';
                }

                if (asset.defaultAnchorLocation)
                {
                    settings.anchor = asset.defaultAnchorLocation;
                }
            }
        }

        function isDefinedAndChanged(settingValue, imageValue) {
            return settingValue > 0 && settingValue != imageValue;
        }
    }

    /* Login */

    login() {
        return URL_BASE_BASE + 'auth';
    }

    /* Custom Entities */

    customEntityList(customEntityDefinition) {
        return this.makePath(this.stringUtility.slugify(customEntityDefinition.name), null, null)
    }

    customEntityDetails(customEntityDefinition, id) {
        return this.makePath(this.stringUtility.slugify(customEntityDefinition.name), id, null);
    }

    /* Users */

    userDetails(userArea, id) {
        return this.makePath(this.getGetAreaPath(userArea), id, null);
    }

    userList(userArea, query) {
        return this.makePath(this.getGetAreaPath(userArea), null, query)
    }

    userNew(userArea, query) {
        return this.makePath(this.getGetAreaPath(userArea), 'new', query)
    }
    
    /* Private Helpers */

    getGetAreaPath(userArea) {
        var userAreaName = userArea ? userArea.name : 'cms';
        return this.stringUtility.slugify(userAreaName) + '-users';
    }

    addCrudRoutes(entity, modulePath) {

        this[entity + 'List'] = (query) => {
            return this.makePath(modulePath, null, query)
        };

        this[entity + 'New'] = function (query) {
            return this.makePath(modulePath, 'new', query);
        };

        this[entity + 'Details'] = function (id) {
            return this.makePath(modulePath, id)
        };
    }
};