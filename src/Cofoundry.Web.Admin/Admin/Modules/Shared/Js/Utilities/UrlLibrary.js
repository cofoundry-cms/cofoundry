/**
 * Library of commonly used url paths and url path helper functions 
 */
angular.module('cms.shared').factory('shared.urlLibrary', [
    '_',
    'shared.urlBaseBase',
    'shared.stringUtilities',
function (
    _,
    urlBaseBase,
    stringUtilities
    ) {

    var service = {};

    /* General */

    service.makePath = function (module, pathParts, query) {
        var path = urlBaseBase + module + '#/';

        if (_.isArray(pathParts)) {
            path += pathParts.join('/');
        } else if (pathParts != null) {
            path += pathParts;
        } 

        if (query) {
            path += '?' + stringUtilities.toQueryString(query);
        }

        return path;
    }

    /* CRUD Routes */

    addCrudRoutes('page', 'pages');
    addCrudRoutes('pageTemplate', 'page-templates');
    addCrudRoutes('role', 'roles');

    /* Asset File Routes */

    service.getDocumentUrl = function (document) {
        var url;
        if (!document) return;

        url = '/assets/files/' + document.documentAssetId + '_' + document.fileName + '.' + document.fileExtension;

        return url;
    }

    service.getImageUrl = function (img, settings) {
        var url;
        if (!img) return;

        url = '/assets/images/' + img.imageAssetId + '_' + img.fileName + '.' + img.extension;
        setDefaultCrop(img, settings);

        if (settings) {
            url = url + '?' + stringUtilities.toQueryString(settings);
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

    service.login = function () {
        return urlBaseBase + 'auth';
    }

    /* Custom Entities */

    service.customEntityList = function (customEntityDefinition) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.name))
    }

    service.customEntityDetails = function (customEntityDefinition, id) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.name), id);
    }

    /* Users */

    service.userDetails = function (userArea, id) {
        return service.makePath(getGetAreaPath(userArea), id);
    }

    service.userList = function (userArea, query) {
        return service.makePath(getGetAreaPath(userArea), null, query)
    }

    service.userNew = function (userArea, query) {
        return service.makePath(getGetAreaPath(userArea), 'new', query)
    }
    
    /* Private Helpers */

    function getGetAreaPath(userArea) {
        var userAreaName = userArea ? userArea.name : 'cms';
        return stringUtilities.slugify(userAreaName) + '-users';
    }

    function addCrudRoutes(entity, modulePath) {

        service[entity + 'List'] = function (query) {
            return service.makePath(modulePath, null, query)
        };

        service[entity + 'New'] = function (query) {
            return service.makePath(modulePath, 'new', query);
        };

        service[entity + 'Details'] = function (id) {
            return service.makePath(modulePath, id)
        };
    }

    return service;
}]);