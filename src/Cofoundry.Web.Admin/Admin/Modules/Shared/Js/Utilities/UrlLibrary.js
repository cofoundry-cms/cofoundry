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
    };

    /* CRUD Routes */

    addCrudRoutes('document', 'documents');
    addCrudRoutes('image', 'images');
    addCrudRoutes('page', 'pages');
    addCrudRoutes('pageTemplate', 'page-templates');
    addCrudRoutes('role', 'roles');

    /* Asset File Routes */

    service.getDocumentUrl = function (document) {
        if (!document) return;

        return document.downloadUrl;
    };

    service.getImageUrl = function (img, settings) {
        var url;
        if (!img) return;

        setDefaultCrop(img, settings);

        if (settings) {
            url = img.url + '?' + stringUtilities.toQueryString(settings);
        } else {
            url = img.url;
        }

        return url;

        /* Helpers */

        function setDefaultCrop(asset, settings) {

            if (!settings) return;

            if (isDefinedAndChanged(settings.width, asset.width) || isDefinedAndChanged(settings.height, asset.height)) {
                if (!settings.mode) {
                    settings.mode = 'Crop';
                }

                if (asset.defaultAnchorLocation) {
                    settings.anchor = asset.defaultAnchorLocation;
                }
            }
        }

        function isDefinedAndChanged(settingValue, imageValue) {
            return settingValue > 0 && settingValue != imageValue;
        }
    };

    /* Login */

    service.login = function (returnUrl) {
        var url = urlBaseBase + 'auth/login';

        if (returnUrl) {
            url += '?returnUrl=' + encodeURIComponent(returnUrl);
        }

        return url;
    };

    /* Pages */

    service.visualEditorForPage = function (pageRoute, isEditMode) {
        if (!pageRoute) return '';

        var path = pageRoute.fullPath;

        if (isEditMode) {
            path += '?mode=edit';
        }

        return path;
    };

    service.visualEditorForVersion = function (
        pageRoute,
        versionRoute,
        isCustomEntityVersion,
        isPublished
    ) {
        if (!pageRoute) return '';

        var url = pageRoute.fullPath + "?";

        // Some of the latest version states will have a default view e.g. preview 
        // or live so check for these first before we defer to showing by version number
        if (versionRoute.workFlowStatus == 'Draft') {
            url += "mode=preview";
        }
        else if (versionRoute.isLatestPublishedVersion && isPublished) {
            // Published, so show live view
            url += "mode=live";
        } else {
            var versionIdProperty = (isCustomEntityVersion ? 'customEntity' : 'page') + 'VersionId';
            url += "version=" + versionRoute[versionIdProperty];
        }

        if (isCustomEntityVersion) {
            url += "&edittype=entity";
        }

        return url;
    };

    /* Custom Entities */

    service.customEntityList = function (customEntityDefinition) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.name));
    };

    service.customEntityDetails = function (customEntityDefinition, id) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.namePlural), id);
    };

    service.customEntityVisualEditor = function (customEntityDetails, isEditMode) {
        if (!customEntityDetails) return '';

        var path = customEntityDetails.fullPath;

        if (!path) return path;

        if (isEditMode) {
            path += '?mode=edit&edittype=entity';
        }

        return path;
    };

    /* Users */

    service.userDetails = function (userArea, id) {
        return service.makePath(getGetAreaPath(userArea), id);
    };

    service.userList = function (userArea, query) {
        return service.makePath(getGetAreaPath(userArea), null, query)
    };

    service.userNew = function (userArea, query) {
        return service.makePath(getGetAreaPath(userArea), 'new', query)
    };
    
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