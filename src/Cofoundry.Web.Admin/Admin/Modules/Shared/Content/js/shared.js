angular
    .module('cms.shared', [
        'ngRoute',
        'ngSanitize',
        'angularModalService',
        'angularFileUpload',
        'ui.tinymce',
        'ang-drag-drop',
        'ui.select'
    ])
    .constant('shared.internalModulePath', '/Admin/Modules/Shared/Js/')
    .constant('shared.pluginModulePath', '/Plugins/Admin/Modules/Shared/Js/')
    .constant('shared.modulePath', '/Cofoundry/Admin/Modules/Shared/Js/')
    .constant('shared.serviceBase', '/Admin/Api/')
    .constant('shared.pluginServiceBase', '/Admin/Api/Plugins/')
    .constant('shared.urlBaseBase', '/Admin/')
    .constant('shared.contentPath', '/Admin/Modules/Shared/Content/');

angular.module('cms.shared').factory('shared.customEntityService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase
    ) {

    var service = {},
        customEntityServiceBase = serviceBase + 'custom-entities';

    /* QUERIES */

    service.getAll = function (query, customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/custom-entities', {
            params: query
        });
    }

    service.getDefinition = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode));
    }

    service.getDataModelSchema = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/data-model-schema');
    }

    service.getPageRoutes = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/routes');
    }

    service.getDefinitionsByIdRange = function (customEntityDefinitionCodes) {
        return $http.get(getCustomEntityDefinitionServiceBase()).then(filterByIdRange);

        function filterByIdRange(results) {
            return _.filter(results, function (result) {
                return _.contains(customEntityDefinitionCodes, result.customEntityDefinitionCode);
            });
        }
    }

    service.getByIdRange = function (ids) {

        return $http.get(customEntityServiceBase + '/', {
            params: {
                'ids': ids
            }
        });
    }

    service.getById = function (customEntityId) {

        return $http.get(getIdRoute(customEntityId));
    }

    service.getVersionsByCustomEntityId = function (customEntityId) {

        return $http.get(getVerionsRoute(customEntityId));
    }


    /* COMMANDS */

    service.add = function (command, customEntityDefinitionCode) {
        command.customEntityDefinitionCode = customEntityDefinitionCode;
        return $http.post(customEntityServiceBase, command);
    }

    service.updateUrl = function (command) {

        return $http.put(getIdRoute(command.customEntityId) + '/url', command);
    }

    service.updateOrdering = function (command) {

        return $http.put(customEntityServiceBase + '/ordering', command);
    }

    service.updateDraft = function (command, customEntityDefinitionCode) {
        command.customEntityDefinitionCode = customEntityDefinitionCode;

        return $http.put(getVerionsRoute(command.customEntityId) + '/draft', command);
    }

    service.remove = function (customEntityId) {

        return $http.delete(getIdRoute(customEntityId));
    }

    service.removeDraft = function (id) {

        return $http.delete(getVerionsRoute(id) + '/draft');
    }


    /* PRIVATES */

    function getIdRoute(customEntityId) {
        return customEntityServiceBase + '/' + customEntityId;
    }

    function getVerionsRoute(customEntityId) {
        return getIdRoute(customEntityId) + '/versions';
    }

    function getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) {
        var customEntityDefinitionServiceBase = serviceBase + 'custom-entity-definitions/';
        if (!customEntityDefinitionCode) {
            return customEntityDefinitionServiceBase;
        }
        return customEntityDefinitionServiceBase + customEntityDefinitionCode;
    }


    return service;
}]);
angular.module('cms.shared').factory('shared.directoryService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
        directoryServiceBase = serviceBase + 'webdirectories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.documentService', [
    '$http',
    '$upload',
    'shared.serviceBase',
function (
    $http,
    $upload,
    serviceBase) {

    var service = {},
        documentsServiceBase = serviceBase + 'documents';

    /* QUERIES */

    service.getAll = function (query) {

        return $http.get(documentsServiceBase, {
            params: query
        });
    }


    service.getById = function (documentId) {

        return $http.get(service.getIdRoute(documentId));
    }

    service.getAllDocumentFileTypes = function () {

        return $http.get(serviceBase + 'document-file-types');
    }

    /* HELPERS */

    service.getIdRoute = function (documentId) {
        return documentsServiceBase + '/' + documentId;
    }

    service.getBaseRoute = function () {
        return documentsServiceBase;
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.entityVersionService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pageServiceBase = serviceBase + 'pages',
        customEntityServiceBase = serviceBase + 'custom-entities';

    /* QUERIES */


    /* COMMANDS */

    service.publish = function (isCustomEntity, entityId) {

        return $http.patch(getVersionsRoute(isCustomEntity, entityId) + '/draft/publish');
    }

    service.unpublish = function (isCustomEntity, entityId) {

        return $http.patch(getVersionsRoute(isCustomEntity, entityId) + '/published/unpublish');
    }

    service.duplicateDraft = function (isCustomEntity, entityId, entityVersionId) {
        var command;

        if (isCustomEntity) {
            command = {
                customEntityId: entityId,
                copyFromCustomEntityVersionId: entityVersionId
            }
        } else {
            command = {
                pageId: entityId,
                copyFromPageVersionId: entityVersionId
            }
        }

        return $http.post(getVersionsRoute(isCustomEntity, entityId), command);
    }

    service.removeDraft = function (isCustomEntity, entityId) {

        return $http.delete(getVersionsRoute(isCustomEntity, entityId) + '/draft');
    }
    
    /* HELPERS */

    function getVersionsRoute (isCustomEntity, entityId) {
        return getIdRoute(isCustomEntity, entityId) + '/versions';
    }

    function getIdRoute(isCustomEntity, entityId) {
        return getServiceBase(isCustomEntity) + '/' + entityId;
    }

    function getServiceBase(isCustomEntity) {
        return isCustomEntity ? customEntityServiceBase : pageServiceBase;
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.imageService', [
    '$http',
    '$upload',
    'shared.stringUtilities',
    'shared.serviceBase',
function (
    $http,
    $upload,
    stringUtilities,
    serviceBase) {

    var service = {},
        imagesServiceBase = serviceBase + 'images';

    /* QUERIES */

    service.add = function (command) {
        return uploadFile(service.getBaseRoute(), command, 'POST');
    }

    service.update = function (command) {

        return uploadFile(service.getIdRoute(command.imageAssetId), command, 'PUT');
    }

    service.getAll = function (query) {

        return $http.get(imagesServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(service.getIdRoute(imageId));
    }

    service.getByIdRange = function (ids) {

        return $http.get(imagesServiceBase + '/', {
            params: {
                'ids': ids
            }
        });
    }

    /* HELPERS */

    service.getIdRoute = function (imageId) {
        return imagesServiceBase + '/' + imageId;
    }

    service.getBaseRoute = function () {
        return imagesServiceBase;
    }

    /* PRIVATES */

    function uploadFile(path, command, method) {
        var data = _.omit(command, 'file'),
            file;

        // the isCurrentFile flag tells us this is a mock version of a file
        // used as placeholder to enable a preview. We shouldn't try and upload it.
        if (command.file && !command.file.isCurrentFile) {
            file = command.file;
        }

        return $upload.upload({
            url: path,
            data: data,
            file: file,
            method: method
        });
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.localStorage', ['shared.serviceBase', function (serviceBase) {
    var service = {},
        localStorageServiceBase = serviceBase + 'localStorage';

    service.setValue = function (key, value) {
        localStorage.setItem(key, value);
    }

    service.getValue = function (key) {
        var value = localStorage.getItem(key);
        return value;
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.localeService', ['$http', 'shared.serviceBase', function ($http, serviceBase) {
    var service = {},
        localeServiceBase = serviceBase + 'locales';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(localeServiceBase);
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.pageService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pagesServiceBase = serviceBase + 'pages';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pagesServiceBase, {
            params: query
        });
    }

    service.getByIdRange = function (pageIds) {
        return $http.get(pagesServiceBase, {
            params: {
                'pageIds': pageIds
            }
        });
    }
    
    service.getById = function (pageId) {

        return $http.get(service.getIdRoute(pageId));
    }

    service.getVersionsByPageId = function (pageId) {

        return $http.get(service.getPageVerionsRoute(pageId));
    }

    service.getPageTypes = function () {
        return [{
            name: 'Generic',
            value: 'Generic'
        },
        {
            name: 'Custom Entity Details',
            value: 'CustomEntityDetails'
        },
        {
            name: 'Not Found',
            value: 'NotFound'
        }];
    }

    /* COMMANDS */

    service.add = function (command) {

        return $http.post(pagesServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(service.getIdRoute(command.pageId), command);
    }

    service.updateUrl = function (command) {

        return $http.put(service.getIdRoute(command.pageId) + '/url', command);
    }

    service.updateDraft = function (command) {

        return $http.patch(service.getPageVerionsRoute(command.pageId) + '/draft', command);
    }

    service.removeDraft = function (id) {

        return $http.delete(service.getPageVerionsRoute(id) + '/draft');
    }

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    service.duplicate = function (command) {
        return $http.post(service.getIdRoute(command.pageToDuplicateId) + '/duplicate', command);
    }

    /* PRIVATES */

    /* HELPERS */

    service.getIdRoute = function (pageId) {
        return pagesServiceBase + '/' + pageId;
    }

    service.getPageVerionsRoute = function (pageId) {
        return service.getIdRoute(pageId) + '/versions';
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.vimeoService', [
    '$http',
    '$q',
function (
    $http,
    $q
    ) {

    var service = {},
        serviceUrl = '//vimeo.com/api/v2/video/';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id + '.json')
            .then(function (response) {

                if (response && response.data) {
                    return response.data[0];
                }

                return;
            });
    }

    function wrapGetResponse() {
        var def = $q.defer();

        $http.get.apply(this, arguments)
            .then(def.resolve)
            .catch(function (response) {
                if (response.status == 404) {
                    def.resolve();
                }
                def.reject(response);
            });

        return def.promise;
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.youTubeService', [
    '$http',
    '$q',
function (
    $http,
    $q
    ) {

    var service = {},
        serviceKey = 'AIzaSyA1lW3d0K_SxwgQsYXGIXANhMwa013nZXg',
        serviceUrl = 'https://www.googleapis.com/youtube/v3/videos?id=';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id + '&part=contentDetails&key=' + serviceKey)
            .then(function (response) {

                if (response && response.data) {
                    return response.data[0];
                }

                return;
            });
    }

    function wrapGetResponse() {
        var def = $q.defer();

        $http.get.apply(this, arguments)
            .then(def.resolve)
            .catch(function (response) {
                if (response.status == 404) {
                    def.resolve();
                }
                def.reject(response);
            });

        return def.promise;
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.arrayUtilities', function () {
    var service = {};

    /* PUBLIC */

    /**
     * Moves an item in an array from one index to another.
     */
    service.move = function (array, fromIndex, toIndex) {
        array.splice(toIndex, 0, array.splice(fromIndex, 1)[0]);
    };

    /**
     * Moves an object in an array from its index to another. If the object isn't
     * the same as the object in the array, you can use the propertyComparer to look up
     * the object based on a property match.
     */
    service.moveObject = function (array, objectToMove, toIndex, propertyComparer) {
        var fromIndex;

        if (propertyComparer) {
            fromIndex = _.findIndex(array, function (item) {
                return item[propertyComparer] === objectToMove[propertyComparer];
            });
        } else {
            fromIndex = array.indexOf(objectToMove);
        }

        service.move(array, fromIndex, toIndex);
    };

    /**
     * Removes the specified object from an array.
     */
    service.removeObject = function (arr, item) {
        var index = arr.indexOf(item);

        if (index >= 0) {
            return arr.splice(index, 1);
        }
    }

    return service;
});
angular.module('cms.shared').factory('shared.directiveUtilities', function () {
    var service = {};
    
    /* PUBLIC */

    /**
     * Sets the model name property, parsing it from the cms-model attribute property accessor. E.g. parses the property 'title' out
     * of cms-model="vm.command.title" or cms-model="vm.model['title']"
     */
    service.setModelName = function (vm, attrs) {
        if (attrs.cmsModelName) {
            vm.modelName = attrs.cmsModelName;
        } else {
            vm.modelName = service.parseModelName(attrs.cmsModel);
        }
    }

    /**
     * Parses the property name out of a property accessor string. E.g. parses the property 'title' out
     * of vm.command.title or vm.model['title']
     */
    service.parseModelName = function (attr) {
        var modelName, matches;

        if (!attr) return;

        modelName = attr.substring(attr.lastIndexOf('.') + 1, attr.length);

        matches = /['"]([^'"]*)['"]/g.exec(modelName);
        if (matches && matches.length > 1) {
            return matches[1];
        }
        else {
            return modelName;
        }
    }

    return service;
});
(function ($, _) {

    /* Extensions */

    extendIfNotExists('offset', false, function (element, name, value) {
        var documentElem, box = { top: 0, left: 0 },
    	    doc = element && element.ownerDocument;

        if (!doc || !_.isUndefined(name)) {
            return;
        }

        documentElem = doc.documentElement;

        if (!_.isUndefined(element.getBoundingClientRect)) {
            box = element.getBoundingClientRect();
        }

        return {
            top: box.top + (window.pageYOffset || documentElem.scrollTop) - (documentElem.clientTop || 0),
            left: box.left + (window.pageXOffset || documentElem.scrollLeft) - (documentElem.clientLeft || 0)
        };
    });

    /* Helpers */

    /**
     * Taken from http://statelessprime.blogspot.co.uk/2013/09/extending-jqlite-with-new-functions.html 
     */
    function extendIfNotExists(name, wrapResult, fn) {
        if (!$.prototype[name]) {
            $.prototype[name] = function (arg1, arg2) {
                var value;

                for (var i = 0; i < this.length; i++) {
                    if (value == undefined) {
                        value = fn(this[i], arg1, arg2);

                        if (value !== undefined && wrapResult) {
                            // any function which returns a value needs to be wrapped
                            value = $(value);
                        }

                    } else {
                        jqLiteAddNodes(value, fn(this[i], arg1, arg2));
                    }

                }

                return value == undefined ? this : value;
            }
        }
    }

    /**
     * Copied from https://github.com/angular/angular.js/blob/master/src/jqLite.js
     */
    function jqLiteAddNodes(root, elements) {

        if (elements) {

            // if a Node (the most common case)
            if (elements.nodeType) {
                root[root.length++] = elements;
            } else {
                var length = elements.length;

                // if an Array or NodeList and not a Window
                if (typeof length === 'number' && elements.window !== elements) {
                    if (length) {
                        for (var i = 0; i < length; i++) {
                            root[root.length++] = elements[i];
                        }
                    }
                } else {
                    root[root.length++] = elements;
                }
            }
        }
    }

})(angular.element, _);
angular.module('cms.shared').constant('shared.routingUtilities', new RoutingUtilitites());

function RoutingUtilitites() {
    var routingUtilities = {};

    /**
        * Maps a standard set of options for the call to $routeProvider.when() or 
        * $routeProvider.otherwise(), setting the controller, template and controllerAs property.
        */
    routingUtilities.mapOptions = function (modulePath, controllerName) {
        return {
            controller: controllerName + 'Controller',
            controllerAs: 'vm',
            templateUrl: modulePath + 'Routes/' + controllerName + '.html'
        }
    },

    /** 
        * Maps the standard list/new/details routes for the specified entity. Controllers
        * must match the standard naming 'AddEntityController', 'EntityListController', 'EntityDetailsController'
        */
    routingUtilities.registerCrudRoutes = function ($routeProvider, modulePath, entityName) {

        $routeProvider
            .when('/new', routingUtilities.mapOptions(modulePath, 'Add' + entityName))
            .when('/:id', routingUtilities.mapOptions(modulePath, entityName + 'Details'))
            .otherwise(routingUtilities.mapOptions(modulePath, entityName + 'List'));
    }

    return routingUtilities;
};
angular.module('cms.shared').factory('shared.stringUtilities', function () {
    var service = {};

    /* CONSTANTS */

    // from http://semplicewebsites.com/removing-accents-javascript
    var LATIN_MAP ={"Á":"A","Ă":"A","Ắ":"A","Ặ":"A","Ằ":"A","Ẳ":"A","Ẵ":"A","Ǎ":"A","Â":"A","Ấ":"A","Ậ":"A","Ầ":"A","Ẩ":"A","Ẫ":"A","Ä":"A","Ǟ":"A","Ȧ":"A","Ǡ":"A","Ạ":"A","Ȁ":"A","À":"A","Ả":"A","Ȃ":"A","Ā":"A","Ą":"A","Å":"A","Ǻ":"A","Ḁ":"A","Ⱥ":"A","Ã":"A","Ꜳ":"AA","Æ":"AE","Ǽ":"AE","Ǣ":"AE","Ꜵ":"AO","Ꜷ":"AU","Ꜹ":"AV","Ꜻ":"AV","Ꜽ":"AY","Ḃ":"B","Ḅ":"B","Ɓ":"B","Ḇ":"B","Ƀ":"B","Ƃ":"B","Ć":"C","Č":"C","Ç":"C","Ḉ":"C","Ĉ":"C","Ċ":"C","Ƈ":"C","Ȼ":"C","Ď":"D","Ḑ":"D","Ḓ":"D","Ḋ":"D","Ḍ":"D","Ɗ":"D","Ḏ":"D","ǲ":"D","ǅ":"D","Đ":"D","Ƌ":"D","Ǳ":"DZ","Ǆ":"DZ","É":"E","Ĕ":"E","Ě":"E","Ȩ":"E","Ḝ":"E","Ê":"E","Ế":"E","Ệ":"E","Ề":"E","Ể":"E","Ễ":"E","Ḙ":"E","Ë":"E","Ė":"E","Ẹ":"E","Ȅ":"E","È":"E","Ẻ":"E","Ȇ":"E","Ē":"E","Ḗ":"E","Ḕ":"E","Ę":"E","Ɇ":"E","Ẽ":"E","Ḛ":"E","Ꝫ":"ET","Ḟ":"F","Ƒ":"F","Ǵ":"G","Ğ":"G","Ǧ":"G","Ģ":"G","Ĝ":"G","Ġ":"G","Ɠ":"G","Ḡ":"G","Ǥ":"G","Ḫ":"H","Ȟ":"H","Ḩ":"H","Ĥ":"H","Ⱨ":"H","Ḧ":"H","Ḣ":"H","Ḥ":"H","Ħ":"H","Í":"I","Ĭ":"I","Ǐ":"I","Î":"I","Ï":"I","Ḯ":"I","İ":"I","Ị":"I","Ȉ":"I","Ì":"I","Ỉ":"I","Ȋ":"I","Ī":"I","Į":"I","Ɨ":"I","Ĩ":"I","Ḭ":"I","Ꝺ":"D","Ꝼ":"F","Ᵹ":"G","Ꞃ":"R","Ꞅ":"S","Ꞇ":"T","Ꝭ":"IS","Ĵ":"J","Ɉ":"J","Ḱ":"K","Ǩ":"K","Ķ":"K","Ⱪ":"K","Ꝃ":"K","Ḳ":"K","Ƙ":"K","Ḵ":"K","Ꝁ":"K","Ꝅ":"K","Ĺ":"L","Ƚ":"L","Ľ":"L","Ļ":"L","Ḽ":"L","Ḷ":"L","Ḹ":"L","Ⱡ":"L","Ꝉ":"L","Ḻ":"L","Ŀ":"L","Ɫ":"L","ǈ":"L","Ł":"L","Ǉ":"LJ","Ḿ":"M","Ṁ":"M","Ṃ":"M","Ɱ":"M","Ń":"N","Ň":"N","Ņ":"N","Ṋ":"N","Ṅ":"N","Ṇ":"N","Ǹ":"N","Ɲ":"N","Ṉ":"N","Ƞ":"N","ǋ":"N","Ñ":"N","Ǌ":"NJ","Ó":"O","Ŏ":"O","Ǒ":"O","Ô":"O","Ố":"O","Ộ":"O","Ồ":"O","Ổ":"O","Ỗ":"O","Ö":"O","Ȫ":"O","Ȯ":"O","Ȱ":"O","Ọ":"O","Ő":"O","Ȍ":"O","Ò":"O","Ỏ":"O","Ơ":"O","Ớ":"O","Ợ":"O","Ờ":"O","Ở":"O","Ỡ":"O","Ȏ":"O","Ꝋ":"O","Ꝍ":"O","Ō":"O","Ṓ":"O","Ṑ":"O","Ɵ":"O","Ǫ":"O","Ǭ":"O","Ø":"O","Ǿ":"O","Õ":"O","Ṍ":"O","Ṏ":"O","Ȭ":"O","Ƣ":"OI","Ꝏ":"OO","Ɛ":"E","Ɔ":"O","Ȣ":"OU","Ṕ":"P","Ṗ":"P","Ꝓ":"P","Ƥ":"P","Ꝕ":"P","Ᵽ":"P","Ꝑ":"P","Ꝙ":"Q","Ꝗ":"Q","Ŕ":"R","Ř":"R","Ŗ":"R","Ṙ":"R","Ṛ":"R","Ṝ":"R","Ȑ":"R","Ȓ":"R","Ṟ":"R","Ɍ":"R","Ɽ":"R","Ꜿ":"C","Ǝ":"E","Ś":"S","Ṥ":"S","Š":"S","Ṧ":"S","Ş":"S","Ŝ":"S","Ș":"S","Ṡ":"S","Ṣ":"S","Ṩ":"S","ẞ":"SS","Ť":"T","Ţ":"T","Ṱ":"T","Ț":"T","Ⱦ":"T","Ṫ":"T","Ṭ":"T","Ƭ":"T","Ṯ":"T","Ʈ":"T","Ŧ":"T","Ɐ":"A","Ꞁ":"L","Ɯ":"M","Ʌ":"V","Ꜩ":"TZ","Ú":"U","Ŭ":"U","Ǔ":"U","Û":"U","Ṷ":"U","Ü":"U","Ǘ":"U","Ǚ":"U","Ǜ":"U","Ǖ":"U","Ṳ":"U","Ụ":"U","Ű":"U","Ȕ":"U","Ù":"U","Ủ":"U","Ư":"U","Ứ":"U","Ự":"U","Ừ":"U","Ử":"U","Ữ":"U","Ȗ":"U","Ū":"U","Ṻ":"U","Ų":"U","Ů":"U","Ũ":"U","Ṹ":"U","Ṵ":"U","Ꝟ":"V","Ṿ":"V","Ʋ":"V","Ṽ":"V","Ꝡ":"VY","Ẃ":"W","Ŵ":"W","Ẅ":"W","Ẇ":"W","Ẉ":"W","Ẁ":"W","Ⱳ":"W","Ẍ":"X","Ẋ":"X","Ý":"Y","Ŷ":"Y","Ÿ":"Y","Ẏ":"Y","Ỵ":"Y","Ỳ":"Y","Ƴ":"Y","Ỷ":"Y","Ỿ":"Y","Ȳ":"Y","Ɏ":"Y","Ỹ":"Y","Ź":"Z","Ž":"Z","Ẑ":"Z","Ⱬ":"Z","Ż":"Z","Ẓ":"Z","Ȥ":"Z","Ẕ":"Z","Ƶ":"Z","Ĳ":"IJ","Œ":"OE","ᴀ":"A","ᴁ":"AE","ʙ":"B","ᴃ":"B","ᴄ":"C","ᴅ":"D","ᴇ":"E","ꜰ":"F","ɢ":"G","ʛ":"G","ʜ":"H","ɪ":"I","ʁ":"R","ᴊ":"J","ᴋ":"K","ʟ":"L","ᴌ":"L","ᴍ":"M","ɴ":"N","ᴏ":"O","ɶ":"OE","ᴐ":"O","ᴕ":"OU","ᴘ":"P","ʀ":"R","ᴎ":"N","ᴙ":"R","ꜱ":"S","ᴛ":"T","ⱻ":"E","ᴚ":"R","ᴜ":"U","ᴠ":"V","ᴡ":"W","ʏ":"Y","ᴢ":"Z","á":"a","ă":"a","ắ":"a","ặ":"a","ằ":"a","ẳ":"a","ẵ":"a","ǎ":"a","â":"a","ấ":"a","ậ":"a","ầ":"a","ẩ":"a","ẫ":"a","ä":"a","ǟ":"a","ȧ":"a","ǡ":"a","ạ":"a","ȁ":"a","à":"a","ả":"a","ȃ":"a","ā":"a","ą":"a","ᶏ":"a","ẚ":"a","å":"a","ǻ":"a","ḁ":"a","ⱥ":"a","ã":"a","ꜳ":"aa","æ":"ae","ǽ":"ae","ǣ":"ae","ꜵ":"ao","ꜷ":"au","ꜹ":"av","ꜻ":"av","ꜽ":"ay","ḃ":"b","ḅ":"b","ɓ":"b","ḇ":"b","ᵬ":"b","ᶀ":"b","ƀ":"b","ƃ":"b","ɵ":"o","ć":"c","č":"c","ç":"c","ḉ":"c","ĉ":"c","ɕ":"c","ċ":"c","ƈ":"c","ȼ":"c","ď":"d","ḑ":"d","ḓ":"d","ȡ":"d","ḋ":"d","ḍ":"d","ɗ":"d","ᶑ":"d","ḏ":"d","ᵭ":"d","ᶁ":"d","đ":"d","ɖ":"d","ƌ":"d","ı":"i","ȷ":"j","ɟ":"j","ʄ":"j","ǳ":"dz","ǆ":"dz","é":"e","ĕ":"e","ě":"e","ȩ":"e","ḝ":"e","ê":"e","ế":"e","ệ":"e","ề":"e","ể":"e","ễ":"e","ḙ":"e","ë":"e","ė":"e","ẹ":"e","ȅ":"e","è":"e","ẻ":"e","ȇ":"e","ē":"e","ḗ":"e","ḕ":"e","ⱸ":"e","ę":"e","ᶒ":"e","ɇ":"e","ẽ":"e","ḛ":"e","ꝫ":"et","ḟ":"f","ƒ":"f","ᵮ":"f","ᶂ":"f","ǵ":"g","ğ":"g","ǧ":"g","ģ":"g","ĝ":"g","ġ":"g","ɠ":"g","ḡ":"g","ᶃ":"g","ǥ":"g","ḫ":"h","ȟ":"h","ḩ":"h","ĥ":"h","ⱨ":"h","ḧ":"h","ḣ":"h","ḥ":"h","ɦ":"h","ẖ":"h","ħ":"h","ƕ":"hv","í":"i","ĭ":"i","ǐ":"i","î":"i","ï":"i","ḯ":"i","ị":"i","ȉ":"i","ì":"i","ỉ":"i","ȋ":"i","ī":"i","į":"i","ᶖ":"i","ɨ":"i","ĩ":"i","ḭ":"i","ꝺ":"d","ꝼ":"f","ᵹ":"g","ꞃ":"r","ꞅ":"s","ꞇ":"t","ꝭ":"is","ǰ":"j","ĵ":"j","ʝ":"j","ɉ":"j","ḱ":"k","ǩ":"k","ķ":"k","ⱪ":"k","ꝃ":"k","ḳ":"k","ƙ":"k","ḵ":"k","ᶄ":"k","ꝁ":"k","ꝅ":"k","ĺ":"l","ƚ":"l","ɬ":"l","ľ":"l","ļ":"l","ḽ":"l","ȴ":"l","ḷ":"l","ḹ":"l","ⱡ":"l","ꝉ":"l","ḻ":"l","ŀ":"l","ɫ":"l","ᶅ":"l","ɭ":"l","ł":"l","ǉ":"lj","ſ":"s","ẜ":"s","ẛ":"s","ẝ":"s","ḿ":"m","ṁ":"m","ṃ":"m","ɱ":"m","ᵯ":"m","ᶆ":"m","ń":"n","ň":"n","ņ":"n","ṋ":"n","ȵ":"n","ṅ":"n","ṇ":"n","ǹ":"n","ɲ":"n","ṉ":"n","ƞ":"n","ᵰ":"n","ᶇ":"n","ɳ":"n","ñ":"n","ǌ":"nj","ó":"o","ŏ":"o","ǒ":"o","ô":"o","ố":"o","ộ":"o","ồ":"o","ổ":"o","ỗ":"o","ö":"o","ȫ":"o","ȯ":"o","ȱ":"o","ọ":"o","ő":"o","ȍ":"o","ò":"o","ỏ":"o","ơ":"o","ớ":"o","ợ":"o","ờ":"o","ở":"o","ỡ":"o","ȏ":"o","ꝋ":"o","ꝍ":"o","ⱺ":"o","ō":"o","ṓ":"o","ṑ":"o","ǫ":"o","ǭ":"o","ø":"o","ǿ":"o","õ":"o","ṍ":"o","ṏ":"o","ȭ":"o","ƣ":"oi","ꝏ":"oo","ɛ":"e","ᶓ":"e","ɔ":"o","ᶗ":"o","ȣ":"ou","ṕ":"p","ṗ":"p","ꝓ":"p","ƥ":"p","ᵱ":"p","ᶈ":"p","ꝕ":"p","ᵽ":"p","ꝑ":"p","ꝙ":"q","ʠ":"q","ɋ":"q","ꝗ":"q","ŕ":"r","ř":"r","ŗ":"r","ṙ":"r","ṛ":"r","ṝ":"r","ȑ":"r","ɾ":"r","ᵳ":"r","ȓ":"r","ṟ":"r","ɼ":"r","ᵲ":"r","ᶉ":"r","ɍ":"r","ɽ":"r","ↄ":"c","ꜿ":"c","ɘ":"e","ɿ":"r","ś":"s","ṥ":"s","š":"s","ṧ":"s","ş":"s","ŝ":"s","ș":"s","ṡ":"s","ṣ":"s","ṩ":"s","ʂ":"s","ᵴ":"s","ᶊ":"s","ȿ":"s","ɡ":"g","ß":"ss","ᴑ":"o","ᴓ":"o","ᴝ":"u","ť":"t","ţ":"t","ṱ":"t","ț":"t","ȶ":"t","ẗ":"t","ⱦ":"t","ṫ":"t","ṭ":"t","ƭ":"t","ṯ":"t","ᵵ":"t","ƫ":"t","ʈ":"t","ŧ":"t","ᵺ":"th","ɐ":"a","ᴂ":"ae","ǝ":"e","ᵷ":"g","ɥ":"h","ʮ":"h","ʯ":"h","ᴉ":"i","ʞ":"k","ꞁ":"l","ɯ":"m","ɰ":"m","ᴔ":"oe","ɹ":"r","ɻ":"r","ɺ":"r","ⱹ":"r","ʇ":"t","ʌ":"v","ʍ":"w","ʎ":"y","ꜩ":"tz","ú":"u","ŭ":"u","ǔ":"u","û":"u","ṷ":"u","ü":"u","ǘ":"u","ǚ":"u","ǜ":"u","ǖ":"u","ṳ":"u","ụ":"u","ű":"u","ȕ":"u","ù":"u","ủ":"u","ư":"u","ứ":"u","ự":"u","ừ":"u","ử":"u","ữ":"u","ȗ":"u","ū":"u","ṻ":"u","ų":"u","ᶙ":"u","ů":"u","ũ":"u","ṹ":"u","ṵ":"u","ᵫ":"ue","ꝸ":"um","ⱴ":"v","ꝟ":"v","ṿ":"v","ʋ":"v","ᶌ":"v","ⱱ":"v","ṽ":"v","ꝡ":"vy","ẃ":"w","ŵ":"w","ẅ":"w","ẇ":"w","ẉ":"w","ẁ":"w","ⱳ":"w","ẘ":"w","ẍ":"x","ẋ":"x","ᶍ":"x","ý":"y","ŷ":"y","ÿ":"y","ẏ":"y","ỵ":"y","ỳ":"y","ƴ":"y","ỷ":"y","ỿ":"y","ȳ":"y","ẙ":"y","ɏ":"y","ỹ":"y","ź":"z","ž":"z","ẑ":"z","ʑ":"z","ⱬ":"z","ż":"z","ẓ":"z","ȥ":"z","ẕ":"z","ᵶ":"z","ᶎ":"z","ʐ":"z","ƶ":"z","ɀ":"z","ﬀ":"ff","ﬃ":"ffi","ﬄ":"ffl","ﬁ":"fi","ﬂ":"fl","ĳ":"ij","œ":"oe","ﬆ":"st","ₐ":"a","ₑ":"e","ᵢ":"i","ⱼ":"j","ₒ":"o","ᵣ":"r","ᵤ":"u","ᵥ":"v","ₓ":"x"};
    
    /* PUBLIC */

    /**
     * Gets the name of a file in a path without the file extension or directory path.
     */
    service.getFileNameWithoutExtension = function (path) {
        if (!path) return path;
        return path.substring(path.lastIndexOf('/') + 1, path.lastIndexOf('.'));
    }

    /**
     * Capitalises the first letter of a string.
     */
    service.capitaliseFirstLetter = function (s) {
        if (!s) return s;
        return s.charAt(0).toUpperCase() + s.slice(1);
    }

    /**
     * Lowercases the first letter of a string.
     */
    service.lowerCaseFirstLetter = function (s) {
        if (!s) return s;
        return s.charAt(0).toLowerCase() + s.slice(1);
    }

    /**
     * Checks if a string ends with the specified value. Case sensitive.
     */
    service.endsWith = function (s, suffix) {
        return s.indexOf(suffix, s.length - suffix.length) !== -1;
    }

    /**
     * Checks if a string starts with the specified value. Case sensitive.
     */
    service.startsWith = function (s, prefix) {
        return s.lastIndexOf(prefix, 0) === 0;
    }

    /**
     * Converts the text into a valid url slug. Removes accents from Latin characters.
     * Adapted from code at http://stringjs.com/
     */
    service.slugify = function (s) {
        if (!s) return s;
        var result = service.latinise(s)
            .replace(' & ', ' and ')
            .replace(/[^\w\s-]/g, '')
            .toLowerCase()
            .trim()
            .replace(/[_\s]+/g, '-')
            .replace(/-+/g, '-');

        if (result.charAt(0) === '-') result = result.substr(1);

        return result;
    }

    /**
     * Converts from myCamelCaseProperty to my-camel-case-property
     */
    service.toSnakeCase = function(s) {
        if (!s) return '';

        return s.replace(/\B([A-Z])/g, '-$1').toLowerCase();
    }

    /**
     * Converts an object to a url query (without the leading ?)
     */
    service.toQueryString = function(obj) {
        return _.chain(obj)
            .pairs()
            .sortBy(function(kvp) {
                return kvp[0];
            })
            .map(function (kvp) {
                if (!_.isUndefined(kvp[1])) {
                    return kvp[0] + '=' + encodeURIComponent(kvp[1]);
                }
             }).filter(function (s) {
                 return !_.isEmpty(s);
             }).value()
               .join('&');
    }

    /**
     * Removes accents from Latin characters.
     */
    service.latinise = function (s) {
        if (!s) return s;
        var result = s.replace(/[^A-Za-z0-9\[\] ]/g, function (x) { return LATIN_MAP[x] || x; });
        return result;
    }

    service.format = function (s) {
        var args = Array.prototype.slice.call(arguments, 1);
        return s.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
            ;
        });
    }

    service.stripTags = function (s) {
        if (!s) return s;

        return s.replace(/<([^>]+)>/ig, "");
    }

    return service;
});
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

    /* Pages */

    service.pageVisualEditor = function (pageRoute, isEditMode) {
        if (!pageRoute) return '';

        var path = pageRoute.fullPath;

        if (isEditMode) {
            path += '?mode=edit';
        }

        return path;
    }

    /* Custom Entities */

    service.customEntityList = function (customEntityDefinition) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.name))
    }

    service.customEntityDetails = function (customEntityDefinition, id) {
        return service.makePath(stringUtilities.slugify(customEntityDefinition.name), id);
    }

    service.customEntityVisualEditor = function(customEntityDetails, isEditMode) {
        if (!customEntityDetails) return '';

        var path = customEntityDetails.fullPath;

        if (!path) return path;

        if (isEditMode) {
            path += '?mode=edit&edittype=entity';
        }

        return path;
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
/**
 * Taken from https://gist.github.com/thomseddon/3511330
 */
angular.module('cms.shared').filter('bytes', function () {
    return function (bytes, precision) {
        var units = ['bytes', 'kb', 'mb', 'gb', 'tb', 'pb'],
            number;

        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (!bytes) return '0 ' + units[1];
        if (typeof precision === 'undefined') precision = 1;

        number = Math.floor(Math.log(bytes) / Math.log(1024));

        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
});
/**
 * @ngdoc provider
 * @name filterWatcher
 * @kind function
 *
 * @description
 * store specific filters result in $$cache, based on scope life time(avoid memory leak).
 * on scope.$destroy remove it's cache from $$cache container.
 * Adapted from https://github.com/a8m/angular-filter
 */

angular.module('cms.shared').provider('filterWatcher', function () {

    this.$get = ['$window', '$rootScope', function ($window, $rootScope) {

        /**
         * Cache storing
         * @type {Object}
         */
        var $$cache = {};

        /**
         * Scope listeners container
         * scope.$destroy => remove all cache keys
         * bind to current scope.
         * @type {Object}
         */
        var $$listeners = {};

        /**
         * $timeout without triggering the digest cycle
         * @type {function}
         */
        var $$timeout = $window.setTimeout;

        /**
         * @description
         * get `HashKey` string based on the given arguments.
         * @param fName
         * @param args
         * @returns {string}
         */
        function getHashKey(fName, args) {
            return [fName, angular.toJson(args)]
              .join('#')
              .replace(/"/g, '');
        }

        /**
         * @description
         * fir on $scope.$destroy,
         * remove cache based scope from `$$cache`,
         * and remove itself from `$$listeners`
         * @param event
         */
        function removeCache(event) {
            var id = event.targetScope.$id;
            forEach($$listeners[id], function (key) {
                delete $$cache[key];
            });
            delete $$listeners[id];
        }

        /**
         * @description
         * for angular version that greater than v.1.3.0
         * it clear cache when the digest cycle is end.
         */
        function cleanStateless() {
            $$timeout(function () {
                if (!$rootScope.$$phase)
                    $$cache = {};
            });
        }

        /**
         * @description
         * Store hashKeys in $$listeners container
         * on scope.$destroy, remove them all(bind an event).
         * @param scope
         * @param hashKey
         * @returns {*}
         */
        function addListener(scope, hashKey) {
            var id = scope.$id;
            if (isUndefined($$listeners[id])) {
                scope.$on('$destroy', removeCache);
                $$listeners[id] = [];
            }
            return $$listeners[id].push(hashKey);
        }

        /**
         * @description
         * return the `cacheKey` or undefined.
         * @param filterName
         * @param args
         * @returns {*}
         */
        function $$isMemoized(filterName, args) {
            var hashKey = getHashKey(filterName, args);
            return $$cache[hashKey];
        }

        /**
         * @description
         * store `result` in `$$cache` container, based on the hashKey.
         * add $destroy listener and return result
         * @param filterName
         * @param args
         * @param scope
         * @param result
         * @returns {*}
         */
        function $$memoize(filterName, args, scope, result) {
            var hashKey = getHashKey(filterName, args);
            //store result in `$$cache` container
            $$cache[hashKey] = result;
            // for angular versions that less than 1.3
            // add to `$destroy` listener, a cleaner callback
            if (isScope(scope)) {
                addListener(scope, hashKey);
            } else {
                cleanStateless();
            }
            return result;
        }

        return {
            isMemoized: $$isMemoized,
            memoize: $$memoize
        }

    }];

    /**
     * @description
     * Test if given object is a Scope instance
     * @param obj
     * @returns {Boolean}
     */
    function isScope(obj) {
        return obj && obj.$evalAsync && obj.$watch;
    }
});

/**
 * @ngdoc filter
 * @name groupBy
 * @kind function
 *
 * @description
 * Create an object composed of keys generated from the result of running each element of a collection,
 * each key is an array of the elements.
 * Adapted from https://github.com/a8m/angular-filter
 */

angular.module('cms.shared').filter('groupBy', [
    '$parse',
    '_',
    'filterWatcher',
function (
    $parse,
    _,
    filterWatcher) {

    return function (collection, property) {

        if (!_.isObject(collection) || _.isUndefined(property)) {
            return collection;
        }

        var getterFn = $parse(property);

        return filterWatcher.isMemoized('groupBy', arguments) ||
        filterWatcher.memoize('groupBy', arguments, this,
            _groupBy(collection, getterFn));

        /**
        * groupBy function
        * @param collection
        * @param getter
        * @returns {{}}
        */
        function _groupBy(collection, getter) {
            var result = {};
            var prop;

            _.each(collection, function (elm) {
                prop = getter(elm);

                if (!result[prop]) {
                    result[prop] = [];
                }
                result[prop].push(elm);
            });
            return result;
        }
    }
}]);
angular.module('cms.shared').factory('authenticationService', ['$window', function ($window) {
    var service = {};

    /* PUBLIC */

    service.redirectToLogin = function () {
        var loc = $window.location;
        var path = '/admin/auth/login?returnUrl=' + encodeURIComponent(loc.pathname + loc.hash);
        $window.location = path;
    }

    return service;
}]);
angular.module('cms.shared').factory('errorService', function () {
    var service = {};

    /* PUBLIC */

    service.raise = function (errors) {
        //alert('An unexpected error occured');
        //TODO: Log
    }

    return service;
});
//angular.module('cms.shared').factory('$exceptionHandler', [
//    '$injector',
//function (
//    $injector) {

//    var isErrorDisplayed;

//    return function (exception, cause) {
//        // Log out the error
//        console.error('error:', exception, cause);

//        // If we're not already displaying an error dialog, show one.
//        if (!isErrorDisplayed) {
//            isErrorDisplayed = true;
//            var modalDialogService = $injector.get('shared.modalDialogService');

//            modalDialogService
//                .alert({
//                    title: 'Error',
//                    message: 'An unexpected error has occured.'
//                })
//                .then(function () {
//                    isErrorDisplayed = false;
//                });
//        }
//    };
//}]);
angular.module('cms.shared').factory('shared.focusService', ['$document', function ($document) {
    var service = {},
        doc = $document[0];

    /* PUBLIC */

    /**
    * Focus's a dom element with the specified id.
    */
    service.focusById = function (id) {
        var el = doc.getElementById(id);
        if (el) {
            el.focus();
        }
    }

    return service;
}]);
angular.module('cms.shared').factory('httpInterceptor', [
    '$q',
    '$rootScope',
    '_',
    'shared.validationErrorService',
    'authenticationService',
function (
    $q,
    $rootScope,
    _,
    validationErrorService,
    authenticationService) {

    var service = {};

    /* PUBLIC */

    service.response = function (response) {
        if (!_.isUndefined(response.data.data)) {
            response = response.data.data;
        }

        return response;
    };

    service.responseError = function (response) {
        switch (response.status) {
            case 400:
                /* Bad Request */
                if (response.data.isValid === false) {
                    validationErrorService.raise(response.data.errors);
                }
                break;
            case 401:
                /* Unauthorized */
                authenticationService.redirectToLogin();
                break;
            case 403:
                /* Forbidden (authenticated but not permitted to view resource */
                var msg = 'This action is not authorized';
               
                /* Can't use the modal because it would create a circular reference */
                alert(msg);
                break;
            default:
                /* Allow get/404 responses through */
                if (response.status != 404 || response.config.method !== 'GET') {
                    throw new Error('Unexpected response: ' + response.status + ' (' + response.statusText + ')');
                }
                break;
        }

        return $q.reject(response);
    };

    return service;

}]);
/**
* Configured CSRF tokens for $http requests
*/
angular.module('cms.shared').config([
    '$httpProvider',
    'csrfToken',
    'csrfHeaderName',
    function (
        $httpProvider,
        csrfToken,
        csrfHeaderName
    ) {

    var headers = $httpProvider.defaults.headers,
        csrfableVerbs = [
            'put',
            'post',
            'patch',
            'delete'
        ];
    
    $httpProvider.interceptors.push('httpInterceptor');

    csrfableVerbs.forEach(function (verb) {
        headers[verb] = headers[verb] || {};
        headers[verb][csrfHeaderName] = csrfToken;
    });

}]);
/**
* Fix to angular 1.5 > 1.6 upgrade where the default hashPrefix has changed. Here 
* we remove it to be consistent with previous behavior.
*/
angular.module('cms.shared').config(['$locationProvider', function ($locationProvider) {
    $locationProvider.hashPrefix('');
}]);
/**
 * Some helper functions that can be used to determine if a user has permission to access
 * to various entities and actions.
 */
angular.module('cms.shared').factory('shared.permissionValidationService', [
    '_',
    'shared.currentUser',
function (
    _,
    currentUser
    ) {

    var service = {},
        COMMON_PERMISSION_CODE_CMSMODULE = 'COMMOD',
        COMMON_PERMISSION_CODE_READ = 'COMRED',
        COMMON_PERMISSION_CODE_CREATE = 'COMCRT',
        COMMON_PERMISSION_CODE_UPDATE = 'COMUPD',
        COMMON_PERMISSION_CODE_DELETE = 'COMDEL';

    /**
     * Determines if the current user has access to a specific permission. Permission codes
     * are formatted {EntityDefinitionCode}{PermissionTypeCode}, e.g. 'CMSPAGCOMRED' or 'ERRLOG' if there 
     * is no EntityDefinitionCode
     */
    service.hasPermission = function (permissionCode) {
        return _.contains(currentUser.permissionCodes, permissionCode);
    }

    /**
     * Determines if the user is permitted to access any data relating to the entity. Read permissions
     * are required for any other entity permission, so you don't need to check (for example) both Create 
     * and Read permissions, just Create. Pass only the EntityDefinitionCode
     */
    service.canRead = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_READ);
    }

    /**
     * Determines if the user can view the CMS module associated with the entity. Pass only
     * the EntityDefinitionCode
     */
    service.canViewModule = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_CMSMODULE);
    }

    /**
     * Determines if the user can add new entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canCreate = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_CREATE);
    }

    /**
     * Determines if the user can update entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canUpdate = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_UPDATE);
    }

    /**
     * Determines if the user can delete entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canDelete = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_DELETE);
    }

    return service;
}]);
angular.module('cms.shared').factory('shared.validationErrorService', [
    '_',
function (
    _
    ) {

    var service = {},
        handlers = [];

    /* PUBLIC */

    /**
    * Raises validation errors with any registered handlers
    */
    service.raise = function (errors) {
        var unhandledErrors = [];

        errors.forEach(function (error) {
            var errorHandlers = _.filter(handlers, function (handler) {
                return _.find(error.properties, function (prop) {
                    if (!prop || !handler.prop) return false;
                    return handler.prop.toLowerCase() === prop.toLowerCase();
                });
            });

            if (errorHandlers.length) {
                errorHandlers.forEach(function (errorHandler) {
                    errorHandler.fn([error]);
                });
            } else {
                unhandledErrors.push(error);
            }
        });

        if (unhandledErrors.length) {
            var genericHandlers = _.filter(handlers, function (handler) {
                return !handler.prop;
            });

            if (genericHandlers.length) {
                executeHandlers(genericHandlers, unhandledErrors);
            } else {
                unhandledValidationErrorHandler(errors);
            }
        }
    }

    /**
    * Registers a handler for a validation error with a specific property
    */
    service.addHandler = function (prop, fn) {
        var handler = {
            prop: prop,
            fn: fn
        };

        handlers.push(handler);
        return handler;
    }

    /**
    * Unregisters a handler for a validation error, you can either pass
    * the handler function to remove that specific instance or the property
    * name to remove all handlers for that property.
    */
    service.removeHandler = function (fnOrProp) {
        var items; 

        if (_.isFunction(fnOrProp)) {
            items = _.where(handlers, { fn: fnOrProp });
        } else {
            items = _.where(handlers, { prop: fnOrProp });
        }

        handlers = _.difference(handlers, items);
    }

    /* PRIVATE */

    function unhandledValidationErrorHandler(errors) {
        // TODO: Display a friendly validation error popup
        throw new Error('An unhandled validation exception has occured');
    }

    function executeHandlers(handlers, errors) {
        handlers.forEach(function (handler) {
            handler.fn(errors);
        });
    }

    return service;
}]);
/**
  * Placeholder js file to solve issue with Azure and Bundle.IncludeDirectory, because
  * without this file the directory is empty.
*/
angular.module('cms.shared').directive('cmsButton', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/Button.html',
        scope: {
            text: '@cmsText'
        }
    };
}]);
angular.module('cms.shared').directive('cmsButtonIcon', [
    'shared.internalModulePath',
    function (modulePath) {

    return {
        restrict: 'E',
        replace: false,
        templateUrl: modulePath + 'UIComponents/Buttons/ButtonIcon.html',
        scope: {
            title: '@cmsTitle',
            icon: '@cmsIcon',
            href: '@cmsHref',
            external: '@cmsExternal'
        },
        link: function (scope, el) {
            if (scope.icon) {
                scope.iconCls = 'fa-' + scope.icon;
            }
        }
    };
}]);
angular.module('cms.shared').directive('cmsButtonLink', [
    'shared.internalModulePath',
    function (modulePath) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/ButtonLink.html',
        scope: {
            text: '@cmsText',
            href: '@cmsHref'
        }
    };
}]);
angular.module('cms.shared').directive('cmsButtonSubmit', [
    'shared.internalModulePath',
function (
    modulePath
) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/ButtonSubmit.html',
        scope: {
            text: '@cmsText'
        }
    };
}]);
angular.module('cms.shared').controller('AddCustomEntityDialogController', [
    '$scope',
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.customEntityService',
    'options',
    'close',
function (
    $scope,
    $location,
    stringUtilities,
    LoadState,
    customEntityService,
    options,
    close) {

    var vm = $scope;

    init();

    /* INIT */

    function init() {
        angular.extend($scope, options.customEntityDefinition);

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();

        vm.formLoadState = new LoadState(true);
        vm.editMode = false;
        vm.options = options.customEntityDefinition;
        vm.saveButtonText = options.customEntityDefinition.autoPublish ? 'Save' : 'Save & Publish';

        vm.save = save.bind(null, false);
        vm.saveAndPublish = save.bind(null, true);
        vm.cancel = onCancel;
        vm.close = onCancel;
        vm.onNameChanged = onNameChanged;

        initData();
    }

    /* EVENTS */

    function save(publish) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        customEntityService
            .add(vm.command, options.customEntityDefinition.customEntityDefinitionCode)
            .then(complete)
            .finally(setLoadingOff.bind(null, loadState))
        ;
    }

    function onNameChanged() {
        vm.command.urlSlug = stringUtilities.slugify(vm.command.title);
    }

    function onCancel() {
        close();
    }

    function complete(entityId) {
        options.onComplete(entityId);
        close();
    }

    /* PRIVATE FUNCS */

    function initData() {
        customEntityService.getDataModelSchema(options.customEntityDefinition.customEntityDefinitionCode).then(loadModelSchema);
        vm.command = {};

        $scope.$watch('vm.command.localeId', function (localeId) {

            if (localeId) {
                vm.additionalParameters = {
                    localeId: localeId
                };
            } else {
                vm.additionalParameters = {};
            }
        });

        function loadModelSchema(modelMetaData) {
            vm.command.model = {};

            vm.formDataSource = {
                model: vm.command.model,
                modelMetaData: modelMetaData
            }

            vm.formLoadState.off();
        }
    }
    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
        if (loadState && _.isFunction(loadState.on)) loadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
        if (loadState && _.isFunction(loadState.off)) loadState.off();
    }
}]);
angular.module('cms.shared').directive('cmsCustomEntityLink', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: {
            customEntityDefinition: '=cmsCustomEntityDefinition',
            customEntity: '=cmsCustomEntity'
        },
        templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityLink.html',
        controller: controller,
        controllerAs: 'vm',
        bindToController: true
    };

    function controller() {
        var vm = this;

        vm.urlLibrary = urlLibrary;
    }
}]);
angular.module('cms.shared').controller('CustomEntityPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'options',
    'close',
function (
    $scope,
    LoadState,
    customEntityService,
    SearchQuery,
    modalDialogService,
    modulePath,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.onCreate = onCreate;
        vm.selectedEntity = vm.currentEntity; // current entity is null in single mode
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: options.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.isSelected = isSelected;
        vm.customEntityDefinition = options.customEntityDefinition;
        vm.multiMode = vm.selectedIds ? true : false;

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return customEntityService.getAll(vm.query.getParameters(), options.customEntityDefinition.customEntityDefinitionCode).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the entity
            vm.onSelected(vm.currentEntity);
        }
        close();
    }

    function onSelect(entity) {
        if (!vm.multiMode) {
            vm.selectedEntity = entity;
            return;
        }

        addOrRemove(entity);
    }

    function onSelectAndClose(entity) {
        if (!vm.multiMode) {
            vm.selectedEntity = entity;
            onOk();
            return;
        }

        addOrRemove(entity);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedEntity);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    function onCreate() {
        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/CustomEntities/AddCustomEntityDialog.html',
            controller: 'AddCustomEntityDialogController',
            options: {
                customEntityDefinition: options.customEntityDefinition,
                onComplete: onComplete
            }
        });

        function onComplete(customEntityId) {
            if (!vm.multiMode) {
                onSelect({ customEntityId: customEntityId });
                loadGrid();
            } else {
                onSelectAndClose({ customEntityId: customEntityId });
            }
        }
    }

    /* PUBLIC HELPERS */

    function isSelected(entity) {
        if (vm.selectedIds && entity && vm.selectedIds.indexOf(entity.customEntityId) > -1) return true;

        if (!entity || !vm.selectedEntity) return false;
        
        return entity.customEntityId === vm.selectedEntity.customEntityId;
    }

    function addOrRemove(entity) {
        if (!isSelected(entity)) {
            vm.selectedIds.push(entity.customEntityId);
        } else {
            var index = vm.selectedIds.indexOf(entity.customEntityId);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);

angular.module('cms.shared').directive('cmsFormFieldCustomEntityCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    customEntityService,
    modalDialogService,
    arrayUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var CUSTOM_ENTITY_ID_PROP = 'customEntityId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntityCollection.html',
        scope: _.extend(baseConfig.scope, {
            customEntityDefinitionCode: '@cmsCustomEntityDefinitionCode',
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable'
        }),
        require: _.union(baseConfig.require, ['?^^cmsFormDynamicFieldSet']),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            definitionPromise,
            dynamicFormFieldController = _.last(controllers);

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            definitionPromise = customEntityService.getDefinition(vm.customEntityDefinitionCode).then(function (customEntityDefinition) {
                vm.customEntityDefinition = customEntityDefinition;
            });

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(customEntity) {

            arrayUtilities.removeObject(vm.gridData, customEntity);
            arrayUtilities.removeObject(vm.model, customEntity[CUSTOM_ENTITY_ID_PROP]);
        }

        function showPicker() {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityPickerDialog.html',
                controller: 'CustomEntityPickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    customEntityDefinition: vm.customEntityDefinition,
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newEntityArr) {
                vm.model = newEntityArr
                setGridItems(newEntityArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, CUSTOM_ENTITY_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, 'title');
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, CUSTOM_ENTITY_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {},
                localeId;

            if (vm.localeId) {
                localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            if (localeId) {
                filter.localeId = localeId;
            }

            return filter;
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, CUSTOM_ENTITY_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                var promise = customEntityService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    orderGridItemsAndSetModel();
                });

                vm.gridLoadState.offWhen(definitionPromise, promise);
            }
        }
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldCustomEntityMultiTypeCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.customEntityService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    customEntityService,
    modalDialogService,
    arrayUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var CUSTOM_ENTITY_ID_PROP = 'customEntityId',
        CUSTOM_ENTITY_DEFINITION_CODE_PROP = 'customEntityDefinitionCode',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntityMultiTypeCollection.html',
        scope: _.extend(baseConfig.scope, {
            customEntityDefinitionCodes: '@cmsCustomEntityDefinitionCodes',
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable'
        }),
        require: _.union(baseConfig.require, ['?^^cmsFormDynamicFieldSet']),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            definitionPromise,
            dynamicFormFieldController = _.last(controllers);

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            definitionPromise = customEntityService.getDefinitionsByIdRange(getDefinitionCodesAsArray()).then(function (customEntityDefinitions) {
                vm.customEntityDefinitions = _.indexBy(customEntityDefinitions, 'customEntityDefinitionCode');
            });

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(customEntity) {

            removeItemFromArray(vm.gridData, customEntity);
            removeItemFromArray(vm.model, _.find(vm.model, function (value) {
                return value[CUSTOM_ENTITY_ID_PROP] === customEntity[CUSTOM_ENTITY_ID_PROP];
            }));

            function removeItemFromArray(arr, item) {
                var index = arr.indexOf(item);

                if (index >= 0) {
                    return arr.splice(index, 1);
                }
            }
        }

        function showPicker(definition) {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityPickerDialog.html',
                controller: 'CustomEntityPickerDialogController',
                options: {
                    selectedIds: getSelectedIds(),
                    customEntityDefinition: definition,
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function getSelectedIds() {
                return _.chain(vm.model)
                        .where({ customEntityDefinitionCode: definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP] })
                        .map(function (value) {
                            return value[CUSTOM_ENTITY_ID_PROP];
                        })
                        .value();
            }

            function onSelected(newEntityArr) {
                if (!vm.model) vm.model = [];

                // Iterate through existing items - remove items of the type just edited if does not exist in new array
                if (vm.model.length > 0) {
                    for (var i = 0; i < vm.model.length; i++) {
                        if (vm.model[i].customEntityDefinitionCode === definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP]) {

                            // See if this item already exists so we dont add again or lose ordering
                            var index = newEntityArr.indexOf(vm.model[i].customEntityId);
                            if (index > -1) {
                                newEntityArr.splice(index, 1);
                                continue;
                            }

                            vm.model.splice(i, 1);
                        }
                    }
                }

                // Add new items to the end of the list
                for (var i = 0; i < newEntityArr.length; i++) {
                    vm.model.push({
                        customEntityId: newEntityArr[i],
                        customEntityDefinitionCode: definition[CUSTOM_ENTITY_DEFINITION_CODE_PROP]
                    });
                }

                setGridItems(vm.model);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, CUSTOM_ENTITY_ID_PROP);

            // Update model with new ordering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, 'title');
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.map(vm.gridData, function(entity) {
                return _.pick(entity, CUSTOM_ENTITY_ID_PROP, CUSTOM_ENTITY_DEFINITION_CODE_PROP);
            });
        }

        /* HELPERS */

        function getDefinitionCodesAsArray() {
            if (!vm.customEntityDefinitionCodes) return [];
            return vm.customEntityDefinitionCodes.split(',');
        }

        function getFilter() {
            var filter = {},
                localeId;

            if (vm.localeId) {
                localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            if (localeId) {
                filter.localeId = localeId;
            }

            return filter;
        }

        /** 
         * Load the grid data if it is inconsistent with the values collection.
         */
        function setGridItems(values) {
            var ids = values ? _.pluck(values, CUSTOM_ENTITY_ID_PROP) : [];

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, CUSTOM_ENTITY_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                var promise = customEntityService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    orderGridItemsAndSetModel();
                });

                vm.gridLoadState.offWhen(definitionPromise, promise);
            }
        }
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldCustomEntitySelector', [
    '_',
    'shared.internalModulePath',
    'shared.customEntityService',
    'shared.directiveUtilities',
    'shared.modalDialogService',
function (
    _,
    modulePath,
    customEntityService,
    directiveUtilities,
    modalDialogService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntitySelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            localeId: '=cmsLocaleId',
            customEntityDefinitionCode: '@cmsCustomEntityDefinitionCode'
        },
        require: ['?^^cmsFormDynamicFieldSet'],
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs, controllers) {
        var vm = scope.vm,
            dynamicFormFieldController = controllers[0];

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
        }

        directiveUtilities.setModelName(vm, attrs);

        vm.search = function (query) {
            return customEntityService.getAll(query, vm.customEntityDefinitionCode);
        };

        customEntityService
            .getDefinition(vm.customEntityDefinitionCode)
            .then(setCustomEntityDefinition);

        function setCustomEntityDefinition(customEntityDefinition) {
            vm.customEntityDefinition = customEntityDefinition;
        }

        vm.create = function () {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/CustomEntities/AddCustomEntityDialog.html',
                controller: 'AddCustomEntityDialogController',
                options: {
                    customEntityDefinition: vm.customEntityDefinition,
                    onComplete: onCompleted
                }
            });

            function onCompleted(newEntityId) {
                vm.model = newEntityId;
            }
        }

        vm.initialItemFunction = function (id) {
            return customEntityService.getByIdRange([id]).then(function (results) {
                return results[0];
            });
        };
    }

    /* CONTROLLER */

    function Controller() {
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldDirectorySelector', [
    '_',
    'shared.directiveUtilities',
    'shared.internalModulePath',
    'shared.directoryService',
function (
    _,
    directiveUtilities,
    modulePath,
    directoryService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Directories/FormFieldDirectorySelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            onLoaded: '&cmsOnLoaded'
        },
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs) {
        var vm = scope.vm;

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
            vm.defaultItemText = attrs.cmsDefaultItemText || 'None';
        }
        vm.title = attrs.cmsTitle || 'Directory';
        vm.description = attrs.cmsDescription;
        directiveUtilities.setModelName(vm, attrs);
    }

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        directoryService.getAll().then(function (webDirectories) {
            vm.webDirectories = webDirectories;

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);
angular.module('cms.shared').directive('cmsDocumentAsset', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: {
            document: '=cmsDocument'
        },
        templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentAsset.html',
        link: function (scope, el, attributes) {

            scope.getDocumentUrl = urlLibrary.getDocumentUrl;
        }
    };
}]);
angular.module('cms.shared').controller('DocumentAssetPickerDialogController', [
        '$scope',
        'shared.LoadState',
        'shared.documentService',
        'shared.SearchQuery',
        'shared.urlLibrary',
        'options',
        'close',
    function (
        $scope,
        LoadState,
        documentService,
        SearchQuery,
        urlLibrary,
        options,
        close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.selectedAsset = vm.currentAsset;
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: vm.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        vm.isDocumentSelected = isDocumentSelected;
        vm.getDocumentUrl = urlLibrary.getDocumentUrl;

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return documentService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        vm.onSelected(vm.currentAsset);
        close();
    }

    function onSelect(document) {
        if (!isDocumentSelected(document)) {
            vm.selectedAsset = document;
        }
    }

    function onSelectAndClose(document) {
        vm.selectedAsset = document;
        onOk();
    }

    function onOk() {
        vm.onSelected(vm.selectedAsset);
        close();
    }

    /* PUBLIC HELPERS */

    function isDocumentSelected(document) {
        if (!document || !vm.selectedAsset) return false;

        return document.documentAssetId === vm.selectedAsset.documentAssetId;
    }
}]);

/**
 * File upload control for documents/files. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsDocumentUpload', [
            '_',
            'shared.internalModulePath',
            'shared.urlLibrary',
        function (
            _,
            modulePath,
            urlLibrary 
        ) {

    /* CONFIG */
    
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentUpload.html',
        scope: {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            isEditMode: '=cmsIsEditMode',
            modelName: '=cmsModelName',
            ngModel: '=ngModel',
            onChange: '&cmsOnChange'
        },
        require: 'ngModel',
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    /* LINK */

    function link(scope, el, attributes, ngModelController) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();

        /* INIT */

        function init() {
            vm.remove = remove;
            vm.fileChanged = onFileChanged;
            vm.isRemovable = _.isObject(vm.ngModel) && !isRequired;
            scope.$watch("vm.asset", setAsset);
        }

        /* EVENTS */

        function remove() {
            onFileChanged();
        }

        /**
         * Initialise the state when the asset is changed
         */
        function setAsset() {
            var asset = vm.asset;
            if (asset) {
                vm.previewUrl = urlLibrary.getDocumentUrl(asset);
                vm.isRemovable = !isRequired;

                ngModelController.$setViewValue({
                    name: asset.fileName + '.' + asset.fileExtension,
                    size: asset.fileSizeInBytes,
                    isCurrentFile: true
                });

            } else {
                vm.isRemovable = false;

                if (ngModelController.$modelValue) {
                    ngModelController.$setViewValue(undefined);
                }
            }

            setButtonText();
        }

        function onFileChanged($files) {
            if ($files && $files[0]) {
                // set the file is one is selected
                ngModelController.$setViewValue($files[0]);
                vm.isRemovable = !isRequired;

            } else if (!vm.ngModel || _.isUndefined($files)) {
                // if we don't have a file loaded already, remove the file.
                ngModelController.$setViewValue(undefined);
                vm.previewUrl = null;
                vm.isRemovable = false;
                vm.asset = undefined;
            }

            setButtonText();

            // base onChange event
            if (vm.onChange) vm.onChange(vm.ngModel);
        }

        /* Helpers */

        function setButtonText() {
            vm.buttonText = ngModelController.$modelValue ? 'Change' : 'Upload';
        }
    }

}]);
/**
 * A form field control for an image asset that uses a search and pick dialog
 * to allow the user to change the selected file.
 */
angular.module('cms.shared').directive('cmsFormFieldDocumentAsset', [
    '_',
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.modalDialogService',
    'shared.stringUtilities',
    'shared.documentService',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    contentPath,
    modalDialogService,
    stringUtilities,
    documentService,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentAsset.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            updateAsset: '@cmsUpdateAsset' // update the asset property if it changes
        }),
        passThroughAttributes: ['required'],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            isAssetInitialized;

        init();
        return baseFormFieldFactory.defaultConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.isRemovable = _.isObject(vm.model) && !isRequired;

            vm.filter = parseFilters(attributes);

            scope.$watch("vm.asset", setAsset);
            scope.$watch("vm.model", setAssetById);
        }

        /* EVENTS */

        function remove() {
            setAsset(null);
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentAssetPickerDialog.html',
                controller: 'DocumentAssetPickerDialogController',
                options: {
                    currentAsset: vm.previewAsset,
                    filter: vm.filter,
                    onSelected: onSelected
                }
            });

            function onSelected(newAsset) {
                if (!newAsset && vm.asset) {
                    setAsset(null);
                } else if (!vm.asset || (newAsset && vm.asset.documentAssetId !== newAsset.documentAssetId)) {
                    setAsset(newAsset);
                }
            }
        }

        /** 
         * When the model is set without a preview asset, we need to go get the full 
         * asset details. This query can be bypassed by setting the cms-asset attribute
         */
        function setAssetById(assetId) {

            if (assetId && (!vm.previewAsset || vm.previewAsset.documentAssetId != assetId)) {
                documentService.getById(assetId).then(function (asset) {
                    setAsset(asset);
                });
            }
        }

        /**
         * Initialise the state when the asset is changed
         */
        function setAsset(asset) {

            if (asset) {
                vm.previewAsset = asset;
                vm.isRemovable = !isRequired;
                vm.model = asset.documentAssetId;

                if (vm.updateAsset) {
                    vm.asset = asset;
                }

            } else if (isAssetInitialized) {
                // Ignore if we are running this first time to avoid overwriting the model with a null vlaue
                vm.previewAsset = null;
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = null;
                }
                if (vm.updateAsset) {
                    vm.asset = null;
                }
            }

            setButtonText();

            isAssetInitialized = true;
        }

        /* Helpers */

        function parseFilters(attributes) {
            var filter = {},
                attributePrefix = 'cms';

            setAttribute('Tags');
            setAttribute('FileExtension');
            setAttribute('FileExtensions');

            return filter;

            function setAttribute(attributeName) {
                var filterName = stringUtilities.lowerCaseFirstLetter(attributeName);
                filter[filterName] = attributes[attributePrefix + attributeName];
            }
        }

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Select';
        }
    }

}]);
angular.module('cms.shared').directive('cmsFormFieldDocumentTypeSelector', [
    '_',
    'shared.internalModulePath',
    'shared.documentService',
function (
    _,
    modulePath,
    documentService) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentTypeSelector.html',
        scope: {
            model: '=cmsModel',
            onLoaded: '&cmsOnLoaded'
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        documentService.getAllDocumentFileTypes().then(function (fileTypes) {

            vm.fileTypes = fileTypes;

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);
/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsFormFieldDocumentUpload', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentUpload.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState'
        }),
        passThroughAttributes: ['required', 'ngRequired'],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('cms-document-upload');
    }
}]);
angular.module('cms.shared').controller('ImageAssetEditorDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
    SearchQuery,
    urlLibrary,
    options,
    close) {
    
    var vm = $scope,
        isAssetInitialized;

    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.formLoadState = new LoadState();
        vm.saveLoadState = new LoadState();

        vm.onInsert = onInsert;
        vm.onCancel = onCancel;
        vm.command = {};

        setCurrentImage();
    }

    /* ACTIONS */

    function setCurrentImage() {
        // If we have an existing image, we need to find the asset id to set the command image
        if (vm.imageAssetHtml && vm.imageAssetHtml.length) {
            vm.command.imageAssetId = vm.imageAssetHtml.attr('data-image-asset-id');
            vm.command.altTag = vm.imageAssetHtml.attr('alt');
            vm.command.style = vm.imageAssetHtml.attr('style');

            // If the image had any styles (mainly dimensions), pass them to the command so they are retained
            if (vm.command.style) {
                var styles = parseStyles(vm.command.style);
                vm.command.width = styles['width'];
                vm.command.height = styles['height'];

            // Else, look to see if the dimensions are stored as attibutes of the image
            } else {
                vm.command.width = vm.imageAssetHtml.attr('width');
                vm.command.height = vm.imageAssetHtml.attr('height');
            }

            // If we cannot find the asset id (could have removed the data attribute that this relies on),
            // we try to work this out based on the image path (this might change in future versions of cofoundry so less reliable)
            if (!vm.command.imageAssetId) {
                var src = vm.imageAssetHtml.attr('src');
                var lastIndex = src.lastIndexOf('/');
                var extractId = src.substr(lastIndex + 1, ((src.indexOf('_') - lastIndex) - 1));
                vm.command.imageAssetId = extractId;
            }
        }
    }
    
    /* EVENTS */

    function onCancel() {
        close();
    }

    function onInsert() {

        // Parse and hold dimensions
        var dimensions = {
            width: parseUnits(vm.command.width),
            height: parseUnits(vm.command.height)
        };

        // If we have no sizes set, default to percentage respecting ratio
        if (!dimensions.width && !dimensions.height) {
            dimensions.width = '100%';
            dimensions.height = 'auto';
        }

        // Get the image path, including specific size options if nessessary
        var path = urlLibrary.getImageUrl(vm.command.imageAsset, parseImageRequestSize(dimensions));

        // Default the alt tag to an empty string if not specified
        var alt = vm.command.altTag || '';

        // Define an object thay holds formatted outputs, plus the model itself
        var output = {
            markdown: "![Alt " + alt + "](" + path + ")",
            html: "<img src='" + path + "' alt='" + alt + "' data-image-asset-id='" + vm.command.imageAssetId + "' />",
            model: vm.command
        };

        // Add css styles to output html
        output.html = insertCssStyles(output.html, dimensions);

        // Call callback with output
        vm.onSelected(output);

        // Close dialog
        close();
    }

    /* PUBLIC HELPERS */

    function insertCssStyles(html, styles) {
        return angular.element(html).css(styles)[0].outerHTML;
    }

    function parseImageRequestSize(dimensions) {
        // If unit type is percent, use original image size
        if ((dimensions.width || '').indexOf('%') > -1 || (dimensions.height || '').indexOf('%') > -1) return {};

        // Else, return raw pixel sizes
        return {
            width: dimensions.width.replace('px', ''),
            height: dimensions.height.replace('px', '')
        };
    }

    function parseUnits(value) {
        if (!value) return '';

        // Default to pixels if not unit type specified
        if (value.indexOf('px') == -1 && value.indexOf('%') == -1 && value.indexOf('auto') == -1) return value + 'px';

        // Return original value if we get here
        return value;
    }

    function parseStyles(cssText) {
        var regex = /([\w-]*)\s*:\s*([^;]*)/g;
        var match, properties = {};
        while (match = regex.exec(cssText)) properties[match[1]] = match[2];
        return properties;
    }

}]);

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
angular.module('cms.shared').directive('cmsForm', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/Form.html',
        replace: true,
        transclude: true,
        scope: {
            editMode: '=cmsEditMode',
            name: '@cmsName'
        },
        compile: compile,
        controller: ['$scope', FormController]
    };

    /* CONTROLLER/COMPILE */

    function FormController($scope) {
        $scope.getForm = function () {
            return $scope[$scope.name];
        }

        this.getFormScope = function () {

            return $scope;
        }
    };

    function compile(element, attrs) {
        // Default edit mode to true if not specified
        if (!angular.isDefined(attrs.cmsEditMode)) {
            attrs.cmsEditMode = 'true';
        }

        return link;
    }

    function link (scope, el, attrs, controllers) {
        // Do somethng similar to the behavior of NgForm and bind the form property a 
        // parent scope except in our case the root scope.
        var parentScope = findRootScopeModel(scope);
        parentScope[scope.name] = scope.getForm();
    }

    /* HELPERS */

    function findRootScopeModel(scope, vmScope) {
        var parent = scope.$parent;

        // We've reached the root, return a vm scope or the root scope
        if (!parent) return vmScope || scope;

        if (angular.isDefined(parent.vm)) {
            // we've found a parent with a controller as 'vm' scope
            vmScope = parent.vm;
        }

        // Keep searching up the tree recursively and return the last one found
        return findRootScopeModel(parent, vmScope);
    }
}]);
angular.module('cms.shared').directive('cmsFormSection', ['shared.internalModulePath', '$timeout', function (modulePath, $timeout) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSection.html',
        scope: {
            title: '@cmsTitle'
        },
        replace: true,
        transclude: true,
        link: link
    };

    function link(scope, elem, attrs) {
        // Wait a moment until child components are rendered before searching the dom
        $timeout(function () {
            var helpers = angular.element(elem[0].querySelector('.help-inline'));
            var btn = angular.element(elem[0].querySelector('.toggle-helpers'));

            if (helpers.length) {
                btn
                    .addClass('show')
                    .on('click', function () {
                        btn.toggleClass('active');
                        elem.toggleClass('show-helpers');
                    });
            }
        }, 100);
    }
}]);
angular.module('cms.shared').directive('cmsFormSectionActions', ['shared.internalModulePath', '$timeout', function (modulePath, $timeout) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSectionActions.html',
        scope: {
        },
        replace: true,
        transclude: true,
        link: link
    };

    function link(scope, elem, attrs) {
    }
}]);
angular.module('cms.shared').directive('cmsFormSectionAuditData', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSectionAuditData.html',
        scope: {
            auditData: '=cmsAuditData'
        }
    };
}]);
/**
 * A status message that can appear in a form to notify the user of any errors or other messages. A scope is
 * attached to the parent form and can be accessed via [myFormName].formStatus
 */
angular.module('cms.shared').directive('cmsFormStatus', [
    '_',
    'shared.validationErrorService',
    'shared.internalModulePath',
function (
    _,
    validationErrorService,
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormStatus.html',
        require: ['^^cmsForm'],
        replace: true,
        scope: true,
        link: { post: link }
    };

    function link(scope, el, attr, controllers) {

        initScope(scope, controllers[0]);
        bindValidationHandler(scope, el);
    }

    function initScope(scope, formController) {
        var formScope = formController.getFormScope(),
            form = formScope.getForm();

        scope.success = success.bind(scope);
        scope.error = error.bind(scope);
        scope.errors = errors.bind(scope);
        scope.clear = clear.bind(scope);
        form.formStatus = scope;
    }

    function bindValidationHandler(scope, el) {

        validationErrorService.addHandler('', scope.errors);
        scope.$on('$destroy', function () {
            validationErrorService.removeHandler(scope.errors);
        });
    }

    function errors(errors, message) {

        var processedErrors = _.uniq(errors, function (error) {
            return error.message;
        });

        setScope(this, message, 'error', processedErrors);
    }

    function error(message) {
        setScope(this, message, 'error');
    }

    function success(message) {
        setScope(this, message, 'success');
    }

    function clear() {
        setScope(this);
    }

    function setScope(scope, message, cls, errors) {
        scope.message = message;
        scope.errors = errors;
        scope.cls = cls;
    }
}]);

/**
 * A dynamically generated set of form elements based on model meta data and bound to 
 * a dynamic model.
 */
angular.module('cms.shared').directive('cmsFormDynamicFieldSet', [
    '$compile',
    '_',
    'shared.stringUtilities',
    'shared.internalModulePath',
    'shared.LoadState',
function (
    $compile,
    _,
    stringUtilities,
    modulePath,
    LoadState
    ) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            /** 
             * An object comprising of a modelMetaData object containing property data for properties
             * of the model, and a model field containing the actual model to bind to.
             */
            dataSource: '=cmsDataSource',
            /** 
             * An object containing additional fields that can be picked up by
             * form controls. The principle example of this is localeId.
             */
            additionalParameters: '=cmsAdditionalParameters'
        },
        link: link,
        controller: ['$scope', FormDynamicFieldSetController],
        bindToController: true,
        controllerAs: 'vm'
    };

    /* CONTROLLER/COMPILE */

    function FormDynamicFieldSetController($scope) {
        var vm = this;
    };

    function link(scope, el, attrs, controllers) {
        var vm = scope.vm,
            attributeMapper = new AttributeMapper();

        scope.$watch('vm.dataSource', function (dataSource) {
            generateForm(dataSource);
        });

        function generateForm(dataSource) {
            var html = '';

            el.empty();
            if (dataSource) {
                dataSource.modelMetaData.dataModelProperties.forEach(function (modelProperty) {
                    var fieldName = mapDirectiveName(modelProperty);

                    html += '<' + fieldName;

                    html += attributeMapper.map('model', stringUtilities.lowerCaseFirstLetter(modelProperty.name));
                    html += attributeMapper.map('title', modelProperty.displayName);
                    html += attributeMapper.map('required', modelProperty.isRequired);
                    html += attributeMapper.map('description', modelProperty.description);

                    if (modelProperty.additionalAttributes) {

                        _.each(modelProperty.additionalAttributes, function (value, key) {
                            html += attributeMapper.map(key, value);
                        });
                    }

                    html += '></' + fieldName + '>';
                });

                el.append($compile(html)(scope));
            }
        }
    }

    /* HELPERS */

    function mapDirectiveName(modelProperty) {
        var fieldPrefix = 'cms-form-field-'

        // default fields for simple properties
        switch (modelProperty.dataTemplateName) {
            case 'Int32':
                return fieldPrefix + 'number';
            case 'String':
                return fieldPrefix + 'text';
            case 'Boolean':
                return fieldPrefix + 'checkbox';
            case 'MultilineText':
                return fieldPrefix + 'text-area';
        }

        return fieldPrefix + stringUtilities.toSnakeCase(modelProperty.dataTemplateName);
    }

    /* CLASSES */

    /**
     * Encapsulates the impedence mismatch between the attributes
     * returned from the api and the formatting of attributes output in html
     */
    function AttributeMapper() {

        var ATTR_PREFIX = 'cms-',
            attributeMap = {
                'maxlength': mapHtmlAttributeWithValue,
                'minlength': mapHtmlAttributeWithValue,
                'min': mapHtmlAttributeWithValue,
                'max': mapHtmlAttributeWithValue,
                'pattern': mapHtmlAttributeWithValue,
                'step': mapHtmlAttributeWithValue,
                'placeholder': mapHtmlAttributeWithValue,
                'match': mapDataSourceAttribute,
                'model': mapDataSourceAttribute,
                'required': mapHtmlAttributeWithoutValue
            };

        /* public */

        this.map = function (key, value) {
            var postfix = 'ValMsg',
                attrMapFn = attributeMap[key],
                attrToVal;

            // allow validation messages for html attributes to pass-through
            if (!attrMapFn && stringUtilities.endsWith(key, postfix)) {
                attrToVal = key.substring(0, key.length - postfix.length);

                if (value && attributeMap[attrToVal] === mapHtmlAttributeWithValue) {
                    attrMapFn = mapHtmlAttributeWithValue;
                }
            } else if (!attrMapFn) {
                // default mapping function
                attrMapFn = mapCmsAttribute;
            }

            return attrMapFn ? attrMapFn(key, value) : '';
        }

        /* private */

        function mapDataSourceAttribute(key, value) {
            value = 'vm.dataSource.model[\'' + value + '\']';
            return mapCmsAttribute(key, value);
        }

        function mapHtmlAttributeWithValue(key, value) {
            return formatAttributeText(stringUtilities.toSnakeCase(key), value);
        }

        function mapHtmlAttributeWithoutValue(key, condition) {
            if (condition) {
                return formatAttributeText(key.toLowerCase());
            }

            return '';
        }

        function mapCmsAttribute(key, value) {
            key = ATTR_PREFIX + stringUtilities.toSnakeCase(key);
            return formatAttributeText(key, value);
        }

        function formatAttributeText(key, value) {
            if (!value) return ' ' + key;

            return ' ' + key + '="' + value + '"'
        }
    }

}]);
/**
 * Base class for form fields that uses default conventions and includes integration with 
 * server validation.
 */
angular.module('cms.shared').factory('baseFormFieldFactory', [
    '$timeout',
    'shared.stringUtilities',
    'shared.directiveUtilities',
    'shared.validationErrorService',
function (
    $timeout,
    stringUtilities,
    directiveUtilities,
    validationErrorService
    ) {

    var service = {},
        /* Here we can validation messages that can apply to all FormField controls */
        globalDefaultValidationMessages = [
            {
                attr: 'required',
                msg: 'This field is required'
            },
            {
                attr: 'maxlength',
                msg: 'This field cannot be longer than {0} characters'
            },
            {
                attr: 'minlength',
                msg: 'This must be at least {0} characters long'
            }
        ];

    /* PUBLIC */

    service.create = function (config) {
        return angular.extend({}, service.defaultConfig, config);
    }

    /* CONFIG */

    /**
     * Configuration defaults
     */
    service.defaultConfig = {
        restrict: 'E',
        replace: true,
        require: ['^^cmsForm'],
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription',
            change: '&cmsChange',
            model: '=cmsModel',
            disabled: '=cmsDisabled'
        },
        compile: compile,
        link: link,
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true,

        /* Custom Properties */

        /** 
         * Should return the main input element that is displayed in edit mode.
         * By default this returns the first child input element.
         */
        getInputEl: getInputEl,

        /**
         * a list of attributes that when defined on the directive are passed through to the element
         * returned from getInputEl.
         */
        passThroughAttributes: [],

        /**
         * Default validation messages to use when none are provided on the element. Saves specifying common messages
         * like 'This field is required' on every field. Each array element should be an object in the form of:
         * { attr: 'name', msg: 'string message or formatter function' }
         */
        defaultValidationMessages: []
    };

    /* COMPILE */

    function compile(el, attrs) {

        initPassThroughAttributes.call(this, el, attrs);

        return this.link.bind(this);
    }

    function link(scope, el, attrs, controllers) {
        var vm = scope.vm,
            formController = controllers[0];

        // Model Properties
        vm.formScope = formController.getFormScope();
        vm.form = vm.formScope.getForm();
        
        directiveUtilities.setModelName(vm, attrs);
        parseErrorMessages(vm, attrs);

        // Model Funcs
        vm.onChange = onChange.bind(vm);

        vm.resetCustomErrors = resetCustomErrors.bind(vm);

        // Init Errors
        vm.resetCustomErrors();

        // Bind Validation Events
        bindValidationEvents(scope, el);

        // watches
        scope.$watch('vm.model', function () {
            vm.resetCustomErrors();
        });
    }

    /* HELPERS */

    /**
     * Loop through attributes specified in config.passThroughAttributes and copy
     * them onto the input control requrned by config.getInputEl
     */
    function initPassThroughAttributes(rootEl, attrs) {
        var config = this,
            el = config.getInputEl(rootEl);

        (config.passThroughAttributes || []).forEach(function (passThroughAttribute) {
            if (angular.isDefined(attrs[passThroughAttribute])) {
                el[0].setAttribute(attrs.$attr[passThroughAttribute], attrs[passThroughAttribute]);
            }
        });
    }

    function getInputEl(rootEl) {
        return rootEl.find('input');
    }

    function bindValidationEvents(scope, el) {
        var fn = _.partial(addErrors, scope.vm, el);

        validationErrorService.addHandler(scope.vm.modelName, fn);

        scope.$on('$destroy', function () {
            validationErrorService.removeHandler(fn);
        });
    }
    
    function parseErrorMessages(vm, attrs) {
        var config = this,
            postfix = 'ValMsg',
            attrPostfix = '-val-msg';

        vm.validators = [];

        _.each(attrs.$attr, function (value, key) {
            var attributeToValidate,
                msg;

            if (stringUtilities.endsWith(key, postfix)) {

                // if the property is postfix '-val-msg' then pull in the message from the attribute
                attributeToValidate = value.substring(0, value.length - attrPostfix.length);
                msg = attrs[key];
            } else {

                attributeToValidate = value;

                // check to see if we have a default message for the property
                msg = getDefaultErrorMessage(config.defaultValidationMessages, key) || getDefaultErrorMessage(globalDefaultValidationMessages, key);
            }

            if (msg) {
                vm.validators.push({
                    name: attrs.$normalize(attributeToValidate),
                    message: stringUtilities.format(msg, attrs[key])
                });

            }
        });

        function getDefaultErrorMessage(defaultsToCheck, attr) {
            var validator = _.find(defaultsToCheck, function (v) {
                return v.attr === attr;
            });

            if (validator) {
                if (_.isFunction(validator.msg)) {
                    return validator.msg(vm.modelName, attrs);
                }
                return validator.msg;
            }
        }
    }

    function addErrors(vm, el, errors) {
        var form = vm.formScope.getForm();
        var model = form[vm.modelName];
        vm.resetCustomErrors();

        model.$setValidity('server', false);

        // make dirty to ensure css classes are applied
        getInputEl(el).removeClass('ng-pristine').addClass('ng-dirty');

        errors.forEach(function (error) {
            vm.customErrors.push(error);
        });
    }

    function onChange() {
        var vm = this;

        vm.resetCustomErrors();
        if (vm.change) {
            // run after digest cycle completes so the parent ngModel is updated
            $timeout(vm.change, 0);
        }
    }

    function resetCustomErrors() {
        var model = this.form[this.modelName];

        if (model) {
            model.$setValidity('server', true);
        }
        this.customErrors = [];
    }

    /* DEFINITION */

    return service;
}]);
angular.module('cms.shared').directive('cmsFormFieldCheckbox', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldCheckbox.html',
        passThroughAttributes: [
            'disabled'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsFormFieldColor', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldColor.html',
        passThroughAttributes: [
            'required',
            'disabled'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        // add custom error for email since its not attribute based like other validation messages
        vm.validators.push({
            name: 'pattern',
            message: vm.title + " must be a hexadecimal colour value e.g. '#EFEFEF' or '#fff'"
        });
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldContainer', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldContainer.html',
        require: ['^^cmsForm'],
        replace: true,
        transclude: true,
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription'
        }
    };
}]);
angular.module('cms.shared').directive('cmsFormFieldDate', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDate.html',
        passThroughAttributes: [
            'required',
            'min',
            'max',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsFormFieldDropdown', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldDropdown.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            options: '=cmsOptions',
            optionValue: '@cmsOptionValue',
            optionName: '@cmsOptionName',
            defaultItemText: '@cmsDefaultItemText',
            required: '=cmsRequired',
            disabled: '=cmsDisabled'
        }),
        passThroughAttributes: [
            'placeholder',
            'disabled',
            'cmsMatch'
        ],
        getInputEl: getInputEl,
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* PRIVATE */

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm;
        init();

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        /* Init */

        function init() {
            vm.isRequiredAttributeDefined = angular.isDefined(attrs.required);

            var optionsWatch = scope.$watch('vm.options', function () {
                bindDisplayValue();

                if (vm.options) {
                    // remove watch
                    optionsWatch();
                }
            });

            scope.$watch('vm.model', bindDisplayValue);
        }

        /* Helpers */

        function bindDisplayValue() {
             var selectedOption = _.find(vm.options, function (option) {
                return option[vm.optionValue] == vm.model;
             });

             vm.displayValue = selectedOption ? selectedOption[vm.optionName] : vm.defaultItemText;
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('select');
    }

}]);
angular.module('cms.shared').directive('cmsFormFieldEmailAddress', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldEmailAddress.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'placeholder',
            'disabled',
            'cmsMatch'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    function link(scope) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        // add custom error for email since its not attribute based like other validation messages
        vm.validators.push({
            name: 'email',
            message: 'Please enter a valid email address'
        });
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldFilteredDropdown', [
    '$q',
    '_',
    'shared.internalModulePath',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    $q,
    _,
    modulePath,
    stringUtilities,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldFilteredDropdown.html',
        passThroughAttributes: [
            'required',
            'disabled'
        ],
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            defaultItemText: '@cmsDefaultItemText',
            searchFunction: '&cmsSearchFunction',
            initialItemFunction: '&cmsInitialItemFunction',
            optionName: '@cmsOptionName',
            optionValue: '@cmsOptionValue',
            required: '=cmsRequired'
        }),
        require: _.union(baseFormFieldFactory.defaultConfig.require, ['?^^cmsFormDynamicFieldSet']),
        link: link,
        transclude: true
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, element, attrs, controllers) {
        var vm = scope.vm,
            dynamicFormFieldController = _.last(controllers);

        init();

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        /* Init */

        function init() {
            vm.refreshDataSource = refreshDataSource;
            vm.dataSource = [];
            vm.hasRequiredAttribute = _.has(attrs, 'required');
            vm.placeholder = attrs['placeholder'];
            vm.clearSelected = clearSelected;

            scope.$watch("vm.model", setSelectedText);
        }

        function setSelectedText(id) {
            vm.selectedText = '';

            if (id && vm.dataSource && vm.dataSource.length) {
                var item = _.find(vm.dataSource, function (item) {
                    return id == item[vm.optionValue];
                });

                if (item) vm.selectedText = item[vm.optionName];
            }

            if (!vm.selectedText && id && vm.initialItemFunction) {
                $q.when(vm.initialItemFunction({ id: id })).then(setSelectedItem);
            }

            function setSelectedItem(item) {
                if (item) {
                    vm.selectedText = item[vm.optionName];
                    refreshDataSource(vm.selectedText);
                }
            }
        }

        function clearSelected() {
            vm.selectedText = '';

            if (vm.model) {
                vm.model = null;
            }
        }

        function refreshDataSource(search) {
            var query = {
                text: search,
                pageSize: 20
            }

            if (vm.localeId) {
                query.localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                query.localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            return vm.searchFunction({ $query: query }).then(loadResults);

            function loadResults(results) {
                vm.dataSource = results.items;
            }
        }
    }

}]);
/**
 * Allows editing of html. Note that in order to display html we have included ngSanitize in
 * the module dependencies (https://docs.angularjs.org/api/ng/directive/ngBindHtml)
 */
angular.module('cms.shared').directive('cmsFormFieldHtml', [
    '$sce',
    '_',
    'shared.internalModulePath', 
    'shared.contentPath',
    'shared.stringUtilities',
    'shared.modalDialogService',
    'baseFormFieldFactory',
function (
    $sce,
    _,
    modulePath, 
    contentPath,
    stringUtilities,
    modalDialogService,
    baseFormFieldFactory
) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldHtml.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'disabled'
        ],
        getInputEl: getInputEl,
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            toolbarsConfig: '@cmsToolbars',
            toolbarCustomConfig: '@cmsCustomToolbar'
        }),
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* OVERRIDES */

    function link(scope, el, attributes) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);

        vm.tinymceOptions = getTinyMceOptions(vm);

        scope.$watch("vm.model", setEditorModel);
        scope.$watch("vm.editorModel", setCmsModel);

        function setEditorModel(value) {
            if (value !== vm.editorModel) {
                vm.editorModel = value;
                vm.rawHtml = $sce.trustAsHtml(value);
            }
        }

        function setCmsModel(value) {
            if (value !== vm.model) {
                vm.model = value;
                vm.rawHtml = $sce.trustAsHtml(value);
            }
        }
    }

    function getInputEl(rootEl) {
        return rootEl.find('textarea');
    }

    /* HELPERS */

    function getTinyMceOptions(vm) {
        return {
            toolbar: parseToolbarButtons(vm.toolbarsConfig, vm.toolbarCustomConfig),
            plugins: 'link image media fullscreen imagetools code',
            content_css: contentPath + "css/third-party/tinymce/content.min.css",
            menubar: false,
            min_height: 300,
            setup: function (editor) {
                editor.addButton('cfimage', {
                    icon: 'image',
                    onclick: onEditorImageButtonClick.bind(null, editor)
                });
            }
        };
    }

    function onEditorImageButtonClick(editor) {
        var currentElement = editor.selection.getContent({ format: 'image' }),
            currentImage = currentElement.length ? angular.element(currentElement) : null;

        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/EditorDialogs/ImageAssetEditorDialog.html',
            controller: 'ImageAssetEditorDialogController',
            options: {
                imageAssetHtml: currentImage,
                onSelected: function (output) {
                    editor.insertContent(output.html);
                }
            }
        });
    }

    function parseToolbarButtons(toolbarsConfig, toolbarCustomConfig) {
        var DEFAULT_CONFIG = 'headings,basicFormatting',
            buttonConfig = {
                headings: 'formatselect',
                basicFormatting: 'fullscreen undo redo | bold italic underline | link unlink',
                advancedFormatting: 'bullist numlist blockquote | alignleft aligncenter alignright alignjustify',
                media: 'cfimage media',
                source: 'code removeformat',
            }, toolbar = '';

        toolbarsConfig = toolbarsConfig || DEFAULT_CONFIG;

        toolbarsConfig.split(',').forEach(function (configItem) {
            configItem = stringUtilities.lowerCaseFirstLetter(configItem.trim());

            if (configItem === 'custom') {

                toolbar = _.union(toolbar, parseCustomConfig(toolbarCustomConfig));

            } else if (buttonConfig[configItem]) {
                toolbar = toolbar.concat((toolbar.length ? ' | ': '') + buttonConfig[configItem]);
            }
        });

        return toolbar;

        function parseCustomConfig(toolbarCustomConfig) {
            var customToolbars;

            if (toolbarCustomConfig) {
                try {
                    customToolbars = JSON.parse('{"j":[' + toolbarCustomConfig.replace(/'/g, '"') + ']}').j;
                }
                catch (e) { }

                if (customToolbars && customToolbars.length) {
                    return customToolbars;
                }
            }

            return [];
        }
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldNumber', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory
    ) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldNumber.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'min',
            'max',
            'step',
            'disabled',
            'placeholder',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsFormFieldPassword', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldPassword.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsFormFieldReadonly', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldReadonly.html',
        replace: true,
        require: '^^cmsForm',
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription',
            model: '=cmsModel'
        },
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true
    };
}]);
angular.module('cms.shared').directive('cmsFormFieldText', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldText.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'placeholder',
            'pattern',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsFormFieldTextArea', [
    'shared.internalModulePath', 
    'shared.stringUtilities',
    'baseFormFieldFactory', 
function (
    modulePath, 
    stringUtilities,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldTextArea.html',
        passThroughAttributes: [
            'required',
            'maxlength',
            'placeholder',
            'ngMinlength',
            'ngMaxlength',
            'ngPattern',
            'disabled',
            'rows',
            'cols',
            'wrap'
        ],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('textarea');
    }

}]);
angular.module('cms.shared').directive('cmsFormFieldUrl', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldUrl.html',
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'placeholder',
            'pattern',
            'disabled',
            'cmsMatch'
        ]
    };

    return baseFormFieldFactory.create(config);

}]);
angular.module('cms.shared').directive('cmsFormFieldValidationSummary', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldValidationSummary.html',
        replace: true
    };
}]);
angular.module('cms.shared').directive('cmsHttpPrefix', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, controller) {
            function ensureHttpPrefix(value) {
                var httpPrefix = 'http://';

                if (value
                    && !/^(https?):\/\//i.test(value)
                    && httpPrefix.indexOf(value) === -1
                    && 'https://'.indexOf(value) === -1) {

                    controller.$setViewValue(httpPrefix + value);
                    controller.$render();

                    return httpPrefix + value;
                }
                else {
                    return value;
                }
            }
            controller.$formatters.push(ensureHttpPrefix);
            controller.$parsers.splice(0, 0, ensureHttpPrefix);
        }
    };
});
/**
 * Validates that a field matches the value of another field. Set the came of the field 
 * in the attribute definition e.g. cms-match="vm.command.password"
 * Adapted from http://ericpanorel.net/2013/10/05/angularjs-password-match-form-validation/
 */
angular.module('cms.shared').directive('cmsMatch', [
    '$parse',
    '$timeout',
    'shared.internalModulePath',
    'shared.directiveUtilities',
function (
    $parse,
    $timeout,
    modulePath,
    directiveUtilities
    ) {

    var DIRECTIVE_ID = 'cmsMatch';
    var DIRECTIVE_ATTRIBUTE = 'cms-match';

    return {
        link: link,
        restrict: 'A',
        require: ['^^cmsForm', '?ngModel'],
    };
    
    function link(scope, el, attrs, controllers) {
        // NB: ngModel may be null on an outer form control before it has been copied to the inner input.
        if (!attrs[DIRECTIVE_ID] || !controllers[1]) return;

        var formController = controllers[0],
            ngModelController = controllers[1],
            form = formController.getFormScope().getForm(),
            sourceField = directiveUtilities.parseModelName(attrs[DIRECTIVE_ID]);

        var validator = function (value, otherVal) {
            var formField = form[sourceField];
            if (!formField) return false;

            var sourceFieldValue = formField.$viewValue;

            return value === sourceFieldValue;
        }

        ngModelController.$validators[DIRECTIVE_ID] = validator;
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldImageAnchorLocationSelector', [
        '_',
        'shared.internalModulePath',
    function (
        _,
        modulePath) {
        return {
            restrict: 'E',
            templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageAnchorLocationSelector.html',
            scope: {
                model: '=cmsModel'
            },
            controller: Controller,
            controllerAs: 'vm',
            bindToController: true
        };

    /* CONTROLLER */

    function Controller() {
        var vm = this;
        
        vm.options = [
            { name: 'Top Left', id: 'TopLeft' },
            { name: 'Top Center', id: 'TopCenter' },
            { name: 'Top Right', id: 'TopRight' },
            { name: 'Middle Left', id: 'MiddleLeft' },
            { name: 'Middle Center', id: 'MiddleCenter' },
            { name: 'Middle Right', id: 'MiddleRight' },
            { name: 'Bottom Left', id: 'BottomLeft' },
            { name: 'Bottom Center', id: 'BottomCenter' },
            { name: 'Bottom Right', id: 'BottomRight' }
        ];
    }
}]);
/**
 * A form field control for an image asset that uses a search and pick dialog
 * to allow the user to change the selected file.
 */
angular.module('cms.shared').directive('cmsFormFieldImageAsset', [
            '_',
            'shared.internalModulePath',
            'shared.contentPath',
            'shared.modalDialogService',
            'shared.stringUtilities',
            'shared.imageService',
            'baseFormFieldFactory',
        function (
            _,
            modulePath,
            contentPath,
            modalDialogService,
            stringUtilities,
            imageService,
            baseFormFieldFactory) {

            /* VARS */

            var assetReplacementPath = contentPath + 'img/AssetReplacement/',
                noImagePath = assetReplacementPath + 'image-replacement.png',
                baseConfig = baseFormFieldFactory.defaultConfig;

            /* CONFIG */

            var config = {
                templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageAsset.html',
                scope: _.extend(baseConfig.scope, {
                    asset: '=cmsAsset', // if we already have the full asset data we can set it here to save an api call
                    loadState: '=cmsLoadState',
                    updateAsset: '@cmsUpdateAsset' // update the asset property if it changes
                }),
                passThroughAttributes: [
                    'required'
                ],
                link: link
            };

            return baseFormFieldFactory.create(config);

            /* LINK */

            function link(scope, el, attributes, controllers) {
                var vm = scope.vm,
                    isRequired = _.has(attributes, 'required'),
                    isAssetInitialized;

                init();
                return baseConfig.link(scope, el, attributes, controllers);

                /* INIT */

                function init() {
                    vm.showPicker = showPicker;
                    vm.remove = remove;
                    vm.isRemovable = _.isObject(vm.model) && !isRequired;

                    vm.filter = parseFilters(attributes);
                    vm.previewWidth = attributes['cmsPreviewWidth'] || 220;
                    vm.previewHeight = attributes['cmsPreviewHeight'];


                    scope.$watch("vm.asset", setAsset);
                    scope.$watch("vm.model", setAssetById);
                }

                /* EVENTS */

                function remove() {
                    setAsset(null);
                }

                function showPicker() {

                    modalDialogService.show({
                        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAssetPickerDialog.html',
                        controller: 'ImageAssetPickerDialogController',
                        options: {
                            currentAsset: vm.previewAsset,
                            filter: vm.filter,
                            onSelected: onSelected
                        }
                    });

                    function onSelected(newAsset) {

                        if (!newAsset && vm.previewAsset) {
                            setAsset(null);
                        } else if (!vm.previewAsset || (newAsset && vm.previewAsset.imageAssetId !== newAsset.imageAssetId)) {
                            setAsset(newAsset);
                        }
                    }
                }

                /** 
                 * When the model is set without a preview asset, we need to go get the full 
                 * asset details. This query can be bypassed by setting the cms-asset attribute
                 */
                function setAssetById(assetId) {

                    if (assetId && (!vm.previewAsset || vm.previewAsset.imageAssetId != assetId)) {
                        imageService.getById(assetId).then(function (asset) {
                            if (asset) {
                                setAsset(asset);
                            }
                        });
                    }
                }

                /**
                 * Initialise the state when the asset is changed
                 */
                function setAsset(asset) {

                    if (asset) {
                        vm.previewAsset = asset;
                        vm.isRemovable = !isRequired;
                        vm.model = asset.imageAssetId;

                        if (vm.updateAsset) {
                            vm.asset = asset;
                        }
                    } else if (isAssetInitialized) {
                        // Ignore if we are running this first time to avoid overwriting the model with a null vlaue
                        vm.previewAsset = null;
                        vm.isRemovable = false;

                        if (vm.model) {
                            vm.model = null;
                        }
                        if (vm.updateAsset) {
                            vm.asset = null;
                        }
                    }

                    setButtonText();

                    isAssetInitialized = true;
                }

                /* Helpers */

                function parseFilters(attributes) {
                    var filter = {},
                        attributePrefix = 'cms';

                    setAttribute('Tags');
                    setAttribute('Width', true);
                    setAttribute('Height', true);
                    setAttribute('MinWidth', true);
                    setAttribute('MinHeight', true);

                    return filter;

                    function setAttribute(filterName, isInt) {
                        var value = attributes[attributePrefix + filterName];

                        if (value) {
                            filterName = stringUtilities.lowerCaseFirstLetter(filterName);
                            filter[filterName] = isInt ? parseInt(value) : value;
                        }
                    }
                }

                function setButtonText() {
                    vm.buttonText = vm.model ? 'Change' : 'Select';
                }
            }

        }]);
angular.module('cms.shared').directive('cmsFormFieldImageAssetCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.imageService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    imageService,
    modalDialogService,
    arrayUtilities,
    stringUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var IMAGE_ASSET_ID_PROP = 'imageAssetId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageAssetCollection.html',
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(image) {

            removeItemFromArray(vm.gridData, image);
            removeItemFromArray(vm.model, image.imageAssetId);

            function removeItemFromArray(arr, item) {
                var index = arr.indexOf(item);

                if (index >= 0) {
                    return arr.splice(index, 1);
                }
            }
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAssetPickerDialog.html',
                controller: 'ImageAssetPickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    customEntityDefinition: vm.customEntityDefinition,
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newImageArr) {
                vm.model = newImageArr;
                setGridItems(newImageArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, IMAGE_ASSET_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, IMAGE_ASSET_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {};

            setAttribute('Width');
            setAttribute('Height');
            setAttribute('MinWidth');
            setAttribute('MinHeight');

            return filter;

            function setAttribute(filterName) {
                var value = attributes['cms' + filterName];

                if (value) {
                    filterName = stringUtilities.lowerCaseFirstLetter(filterName);
                    filter[filterName] = parseInt(value);
                }
            }
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, IMAGE_ASSET_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                imageService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    vm.gridLoadState.off();
                });
            }
        }
    }
}]);
/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsFormFieldImageUpload', [
            '_',
            'shared.internalModulePath',
            'baseFormFieldFactory',
        function (
            _,
            modulePath,
            baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageUpload.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState'
        }),
        passThroughAttributes: ['required', 'ngRequired'],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('cms-image-upload');
    }
}]);
angular.module('cms.shared').directive('cmsImageAsset', [
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.urlLibrary',
function (
    modulePath,
    contentPath,
    urlLibrary) {

    return {
        restrict: 'E',
        scope: {
            image: '=cmsImage',
            width: '@cmsWidth',
            height: '@cmsHeight',
            cropMode: '@cmsCropMode'
        },
        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAsset.html',
        link: function (scope, el, attributes) {

            scope.$watch('image', function (newValue, oldValue) {
                if (newValue && newValue.imageAssetId) {
                    scope.src = urlLibrary.getImageUrl(newValue, {
                        width: scope.width,
                        height: scope.height,
                        mode: scope.cropMode
                    });
                } else {
                    scope.src = contentPath + 'img/assetreplacement/image-replacement.png';
                }
            });


        },
        replace: true
    };
}]);
angular.module('cms.shared').controller('ImageAssetPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
    SearchQuery,
    modalDialogService,
    modulePath,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.onUpload = onUpload;
        vm.selectedAsset = vm.currentAsset; // currentAsset is null in single mode
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: options.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.isSelected = isSelected;
        vm.multiMode = vm.selectedIds ? true : false;
        vm.okText = vm.multiMode ? 'Ok' : 'Select';

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return imageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the currentAsset
            vm.onSelected(vm.currentAsset);
        }
        close();
    }

    function onSelect(image) {
        if (!vm.multiMode) {
            vm.selectedAsset = image;
            return;
        }

        addOrRemove(image);
    }

    function onSelectAndClose(image) {
        if (!vm.multiMode) {
            vm.selectedAsset = image;
            onOk();
            return;
        }

        addOrRemove(image);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedAsset);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    function onUpload() {
        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/ImageAssets/UploadImageAssetDialog.html',
            controller: 'UploadImageAssetDialogController',
            options: {
                filter: options.filter,
                onUploadComplete: onUploadComplete
            }
        });

        function onUploadComplete(imageAssetId) {
            onSelectAndClose({ imageAssetId: imageAssetId });
        }
    }

    /* PUBLIC HELPERS */

    function isSelected(image) {
        if (vm.selectedIds && image && vm.selectedIds.indexOf(image.imageAssetId) > -1) return true;

        if (!image || !vm.selectedAsset) return false;

        return image.imageAssetId === vm.selectedAsset.imageAssetId;
    }

    function addOrRemove(image) {
        if (!isSelected(image)) {
            vm.selectedIds.push(image.imageAssetId);
        } else {
            var index = vm.selectedIds.indexOf(image.imageAssetId);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);

/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsImageUpload', [
            '_',
            'shared.internalModulePath',
            'shared.contentPath',
            'shared.stringUtilities',
            'shared.urlLibrary',
        function (
            _,
            modulePath,
            contentPath,
            stringUtilities,
            urlLibrary) {

    /* VARS */

    var assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noPreviewImagePath = assetReplacementPath + 'preview-not-supported.png',
        noImagePath = assetReplacementPath + 'image-replacement.png';

    /* CONFIG */

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageUpload.html',
        scope: {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            isEditMode: '=cmsIsEditMode',
            modelName: '=cmsModelName',
            ngModel: '=ngModel',
            onChange: '&cmsOnChange'
        },
        require: 'ngModel',
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    
    /* LINK */

    function link(scope, el, attributes, ngModelController) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();

        /* INIT */

        function init() {
            vm.remove = remove;
            vm.fileChanged = onFileChanged;
            vm.isRemovable = _.isObject(vm.ngModel) && !isRequired;
            scope.$watch("vm.asset", setAsset);
        }

        /* EVENTS */

        function remove() {
            onFileChanged();
        }

        /**
         * Initialise the state when the asset is changed
         */
        function setAsset() {
            var asset = vm.asset;

            if (asset) {
                vm.previewUrl = urlLibrary.getImageUrl(asset, {
                    width: 220
                });
                vm.isRemovable = !isRequired;

                ngModelController.$setViewValue({
                    name: asset.fileName + '.' + asset.extension,
                    size: asset.fileSizeInBytes,
                    isCurrentFile: true
                });
            } else {
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;

                if (ngModelController.$modelValue) {
                    ngModelController.$setViewValue(undefined);
                }
            }

            setButtonText();
        }

        function onFileChanged($files) {
            if ($files && $files[0]) {
                // set the file is one is selected
                ngModelController.$setViewValue($files[0]);
                setPreviewImage($files[0]);
                vm.isRemovable = !isRequired;

            } else if (!vm.ngModel || _.isUndefined($files)) {
                // if we don't have an image loaded already, remove the file.
                ngModelController.$setViewValue(undefined);
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;
                //vm.asset = undefined;
            }

            setButtonText();

            // base onChange event
            if (vm.onChange) vm.onChange(vm.ngModel);
        }

        /* Helpers */

        function setPreviewImage(file) {
            if (!isPreviewSupported(file))
            {
                vm.previewUrl = noPreviewImagePath;
                return;
            }

            try {
                vm.previewUrl = URL.createObjectURL(file);
            }
            catch (err) {
                vm.previewUrl = noPreviewImagePath;
            }
        }

        function isPreviewSupported(file) {
            var unsupportedPreviewFormats = ['.tiff', '.tif'];

            return !_.find(unsupportedPreviewFormats, function(format) {
                return stringUtilities.endsWith(file.name, format);
            });
        }

        function setButtonText() {
            vm.buttonText = ngModelController.$modelValue ? 'Change' : 'Upload';
        }
    }

}]);
angular.module('cms.shared').controller('UploadImageAssetDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.focusService',
    'shared.stringUtilities',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
    SearchQuery,
    focusService,
    stringUtilities,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    function init() {
        angular.extend($scope, options);

        initData();

        vm.onUpload = onUpload;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.filter = options.filter;
        vm.onFileChanged = onFileChanged;
        vm.hasFilterRestrictions = hasFilterRestrictions;

        vm.saveLoadState = new LoadState();
    }

    /* EVENTS */
    function onUpload() {
        vm.saveLoadState.on();

        imageService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(uploadComplete);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            focusService.focusById('title');
        }
    }

    function onCancel() {
        close();
    }

    /* PUBLIC HELPERS */
    function initData() {
        vm.command = {};
    }

    function hasFilterRestrictions() {
        return options.filter.minWidth ||
            options.filter.minHeight ||
            options.filter.width ||
            options.filter.height;
    }

    function cancel() {
        close();
    }

    function uploadComplete(imageAssetId) {
        options.onUploadComplete(imageAssetId);
        close();
    }

}]);

angular.module('cms.shared').directive('cmsField', [
    '$timeout',
    'shared.internalModulePath',
    function (
        $timeout,
        modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Layout/Field.html',
        transclude: true,
        replace: true,
        require: '^^cmsForm'
    }
}]);
(function ($) {

    // Context
    var $el = $(document.getElementsByClassName('main-nav'));

    // Temp vars
    var categories, currentCategory;

    function init() {

        categories = $(document.getElementsByClassName('category'));
        currentCategory = $(document.getElementsByClassName('category selected'));

        //Events
        categories.on('mouseenter', function (e) {
            var $src = $(e.srcElement);
            
            currentCategory.removeClass('selected');
            //$src.addClass('selected');
        });

        categories.on('mouseleave', function (e) {
            var $src = $(e.srcElement);

            currentCategory.addClass('selected');
            //$src.removeClass('selected');
        });
    }

    init();

})(angular.element);
angular.module('cms.shared').directive('cmsPageActions', function () {
    return {
        restrict: 'E',
        template: '<div class="page-actions" ng-transclude></div>',
        replace: true,
        transclude: true,
    }
});
angular.module('cms.shared').directive('cmsPageBody', function () {
    return {
        restrict: 'E',
        template: '<div class="page-body {{ contentType }} {{ subHeader }}"><div class="form-wrap" ng-transclude></div></div>',
        scope: {
            contentType: '@cmsContentType',
            subHeader: '@cmsSubHeader'
        },
        replace: true,
        transclude: true
    }
});
angular.module('cms.shared').directive('cmsPageFilter', function () {
    return {
        restrict: 'E',
        template: '<div class="page-filter" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});
angular.module('cms.shared').directive('cmsPageHeader', function () {
    return {
        restrict: 'E',
        template: '<h1 class="page-header"><a ng-href="{{parentHref ? parentHref : \'#/\'}}" ng-if="parentTitle">{{parentTitle}}</a><span ng-if="parentTitle && title"> &gt; </span>{{title}}</h1>',
        replace: true,
        scope: {
            title: '@cmsTitle',
            parentTitle: '@cmsParentTitle',
            parentHref: '@cmsParentHref'
        },
    }
});
angular.module('cms.shared').directive('cmsPageHeaderButtons', function () {
    return {
        restrict: 'E',
        template: '<div class="btn-group page-header-buttons" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});
angular.module('cms.shared').directive('cmsPageSubHeader', function () {
    return {
        restrict: 'E',
        template: '<div class="page-sub-header" ng-transclude></div>',
        replace: true,
        transclude: true
    }
});
angular.module('cms.shared').factory('shared.LoadState', ['$q', '$rootScope', '_', function($q, $rootScope, _) {
    return LoaderState;

    function LoaderState(onByDefault) {
        var me = this;

        /* Properties */

        me.isLoading = onByDefault === true;
        me.progress = me.isLoading ? 0 : 100;

        /* Funcs */

        me.on = function () {
            me.isLoading = true;
            if (me.progress === 100) {
                me.progress = 0;
            }
        }

        me.off = function() {
            me.isLoading = false;
            me.progress = 100;
        }

        me.offWhen = function () {
            var promises = [],
                promise,
                args = Array.prototype.slice.call(arguments);

            _.each(args, function(arg) {
                promises.push(arg.promise ? arg.promise : arg);
            });

            return $q.all(promises).then(function () {
                me.off();
            });
        }

        /**
         * Update the progress of the loader. Loaded can be a a numerical value
         * or the eventArgs to a file upload progress event. Total is optional 
         * in which case total is assumed to be 100.
         */
        me.setProgress = function (loaded, total) {
            var progress;

            // If we pass in the eventArgs of a progress event
            if (_.isObject(loaded)) {
                total = loaded.total;
                loaded = loaded.loaded;
            }

            // If no total is provided assume 100
            if (_.isUndefined(total)) total = 100;
            progress = parseInt(100.0 * loaded / total);

            if (progress <= 0) progress = 0;

            if (progress >= 100) {
                progress = 100;
            } else {
                me.on();
            }

            me.progress = progress;
        }
    }
}]);
angular.module('cms.shared').directive('cmsLoading', function () {
    return {
        restrict: 'A',
        link: function (scope, el, attributes) {

            scope.$watch(attributes.cmsLoading, function (isLoading) {
                el.toggleClass('loading', isLoading);
            });
        }
    };
});
angular.module('cms.shared').directive('cmsProgressBar', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { loadState: '=' },
        templateUrl: modulePath + 'UIComponents/Loader/ProgressBar.html'
    };
}]);
angular.module('cms.shared').directive('cmsFormFieldLocaleSelector', [
    '_',
    'shared.internalModulePath',
    'shared.localeService',
    'shared.directiveUtilities',
function (
    _,
    modulePath,
    localeService,
    directiveUtilities
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Locales/FormFieldLocaleSelector.html',
        scope: {
            model: '=cmsModel',
            onLoaded: '&cmsOnLoaded'
        },
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs) {
        var vm = scope.vm;

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
            vm.defaultItemText = attrs.cmsDefaultItemText || 'None';
        }

        directiveUtilities.setModelName(vm, attrs);
    }

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        localeService.getAll().then(function (locales) {

            vm.locales = _.map(locales, function (locale) {
                return {
                    name: locale.name + ' (' + locale.ietfLanguageTag + ')',
                    id: locale.localeId
                }
            });

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);
angular.module('cms.shared').directive('cmsMenu', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        replace: true,
        transclude: true,
        templateUrl: modulePath + 'UIComponents/Menus/Menu.html',
        scope: {
            text: '@cmsIcon'
        }
    };
}]);
angular.module('cms.shared').controller('AlertController', [
    '$scope',
    'options',
    'close', function (
        $scope,
        options,
        close) {

    angular.extend($scope, options);
    $scope.close = close;
}]);
angular.module('cms.shared').controller('ConfirmDialogController', ['$scope', 'options', 'close', function ($scope, options, close) {
    angular.extend($scope, options);
    $scope.close = resolve;

    /* helpers */

    function resolve(result) {
        var resolver = result ? options.ok : options.cancel;

        if (resolver) {
            resolver()
                .then(closeIfRequired)
                .finally(options.onCancel);
        }
    }

    function closeIfRequired() {
        if (options.autoClose) {
            close();
        }
    }

}]);
angular.module('cms.shared').directive('cmsModalDialogActions', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogActions.html',
        transclude: true
    };
}]);
angular.module('cms.shared').directive('cmsModalDialogBody', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogBody.html',
        transclude: true,
    };
}]);
angular.module('cms.shared').directive('cmsModalDialogContainer', [
    'shared.internalModulePath',
    '$timeout',
function (
    modulePath,
    $timeout) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogContainer.html',
        transclude: true,
        link: link,
        controller: angular.noop
        };

    function link(scope, el, attributes) {
        var cls = attributes.cmsModalSize === 'large' ? 'modal-lg' : '';
        cls += (scope.isRootModal ? ' is-root-modal' : ' is-child-modal');
        if (attributes.cmsModalSize === 'large') {
            scope.sizeCls = cls;
        }
        $timeout(function () {
            scope.sizeCls = cls + ' modal--show';
        }, 1);
    }
}]);
angular.module('cms.shared').directive('cmsModalDialogHeader', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogHeader.html',
        transclude: true,
    };
}]);
angular.module('cms.shared').factory('shared.modalDialogService', [
    '$q',
    '_',
    'ModalService',
    'shared.internalModulePath',
    'shared.LoadState',
function (
    $q,
    _,
    ModalService,
    modulePath,
    LoadState) {

    var service = {};

    /* PUBLIC */

    /**
    * Displays a modal message with a button to dismiss the message.
    */
    service.alert = function (optionsOrMessage) {
        var deferred = $q.defer(),
            options = optionsOrMessage || {};

        if (_.isString(optionsOrMessage)) {
            options = {
                message: optionsOrMessage
            }
        }

        ModalService.showModal({
            templateUrl: modulePath + "UIComponents/Modals/Alert.html",
            controller: "AlertController",
            inputs: {
                options: options
            }
        }).then(function (modal) {

            // Apres-creation stuff
            modal.close.then(deferred.resolve);
        });

        return deferred.promise;
    }

    /**
    * Displays a custom modal popup using a template at the specified url.
    */
    service.show = function (modalOptions) {
        return ModalService.showModal({
            templateUrl: modalOptions.templateUrl,
            controller: modalOptions.controller,
            inputs: {
                options: modalOptions.options
            }
        });
    }

    /**
    * Displays a modal message with a button options to ok/cancel an action.
    */
    service.confirm = function (optionsOrMessage) {
        var returnDeferred = $q.defer(),
            onOkLoadState = new LoadState(),
            options = initOptions(optionsOrMessage);

        ModalService.showModal({
            templateUrl: modulePath + "UIComponents/Modals/ConfirmDialog.html",
            controller: "ConfirmDialogController",
            inputs: {
                options: options
            }
        });

        return returnDeferred.promise;

        /* helpers */

        function initOptions(optionsOrMessage) {
            var options = optionsOrMessage || {},
                defaults = {
                    okButtonTitle: 'OK',
                    cancelButtonTitle: 'Cancel',
                    autoClose: true,
                    // onCancel: fn or promise
                    // onOk: fn or promise
                },
                internalScope = {
                    ok: resolve.bind(null, true),
                    cancel: resolve.bind(null, false),
                    onOkLoadState: onOkLoadState
                };

            if (_.isString(optionsOrMessage)) {
                options = {
                    message: optionsOrMessage
                }
            }

            return _.defaults(internalScope, options, defaults);
        }

        function resolve(isSuccess) {
            var optionToExec = isSuccess ? options.onOk : options.onOk.onCancel,
                deferredAction = isSuccess ? returnDeferred.resolve : returnDeferred.reject,
                optionResult;

            // run the action
            if (_.isFunction(optionToExec)) {
                onOkLoadState.on();
                optionResult = optionToExec();
            }

            // Wait for the result to resolve if its a promise
            // Then resolve/reject promise we returned to the callee
            return $q.when(optionResult)
                     .then(deferredAction);
        }
    }

    return service;
}]);
angular.module('cms.shared').directive('cmsFormFieldPageCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.pageService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    pageService,
    modalDialogService,
    arrayUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var PAGE_ID_PROP = 'pageId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/Pages/FormFieldPageCollection.html',
        scope: _.extend(baseConfig.scope, {
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable'
        }),
        require: _.union(baseConfig.require, ['?^^cmsFormDynamicFieldSet']),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            definitionPromise,
            dynamicFormFieldController = _.last(controllers);

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(page) {

            arrayUtilities.removeObject(vm.gridData, page);
            arrayUtilities.removeObject(vm.model, page[PAGE_ID_PROP]);
        }

        function showPicker() {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/Pages/PagePickerDialog.html',
                controller: 'PagePickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newEntityArr) {
                vm.model = newEntityArr
                setGridItems(newEntityArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, PAGE_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, function (page) {
                    return page.auditData.createDate;
                }).reverse();
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, PAGE_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {},
                localeId;

            if (vm.localeId) {
                localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            if (localeId) {
                filter.localeId = localeId;
            }

            return filter;
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, PAGE_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                pageService
                    .getByIdRange(ids)
                    .then(loadPages)
                    .then(vm.gridLoadState.off);
            }

            function loadPages(items) {
                vm.gridData = items;
                orderGridItemsAndSetModel();
            }
        }
    }
}]);
angular.module('cms.shared').directive('cmsFormFieldPageSelector', [
    '_',
    'shared.internalModulePath',
    'shared.pageService',
    'shared.directiveUtilities',
    'shared.modalDialogService',
function (
    _,
    modulePath,
    pageService,
    directiveUtilities,
    modalDialogService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Pages/FormFieldPageSelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            localeId: '=cmsLocaleId'
        },
        require: ['?^^cmsFormDynamicFieldSet'],
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs, controllers) {
        var vm = scope.vm,
            dynamicFormFieldController = controllers[0];

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
        }

        directiveUtilities.setModelName(vm, attrs);

        vm.search = function (query) {
            return pageService.getAll(query);
        };
        
        vm.initialItemFunction = function (id) {
            return pageService.getByIdRange([id]).then(function (results) {
                return results[0];
            });
        };
    }

    /* CONTROLLER */

    function Controller() {
    }
}]);
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
angular.module('cms.shared').controller('PagePickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.pageService',
    'shared.SearchQuery',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'options',
    'close',
function (
    $scope,
    LoadState,
    pageService,
    SearchQuery,
    modalDialogService,
    modulePath,
    options,
    close) {

    /* VARS */

    var vm = $scope,
        PAGE_ID_PROP = 'pageId';

    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.onSelect = onSelect;
        vm.selectedPage = vm.currentPage; // current page is null in single mode
        vm.onSelectAndClose = onSelectAndClose;
        vm.close = onCancel;

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            useHistory: false,
            defaultParams: options.filter
        });
        vm.presetFilter = options.filter;

        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.isSelected = isSelected;
        vm.multiMode = vm.selectedIds ? true : false;

        toggleFilter(false);
        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    function loadGrid() {
        vm.gridLoadState.on();

        return pageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
    
    /* EVENTS */

    function onCancel() {
        if (!vm.multiMode) {
            // in single-mode reset the page
            vm.onSelected(vm.currentPage);
        }
        close();
    }

    function onSelect(page) {
        if (!vm.multiMode) {
            vm.selectedPage = page;
            return;
        }

        addOrRemove(page);
    }

    function onSelectAndClose(page) {
        if (!vm.multiMode) {
            vm.selectedPage = page;
            onOk();
            return;
        }

        addOrRemove(page);
        onOk();
    }

    function onOk() {
        if (!vm.multiMode) {
            vm.onSelected(vm.selectedPage);
        } else {
            vm.onSelected(vm.selectedIds);
        }

        close();
    }

    /* PUBLIC HELPERS */

    function isSelected(page) {
        if (vm.selectedIds && page && vm.selectedIds.indexOf(page.pageId) > -1) return true;

        if (!page || !vm.selectedPage) return false;
        
        return page[PAGE_ID_PROP] === vm.selectedPage[PAGE_ID_PROP];
    }

    function addOrRemove(page) {
        if (!isSelected(page)) {
            vm.selectedIds.push(page[PAGE_ID_PROP]);
        } else {
            var index = vm.selectedIds.indexOf(page[PAGE_ID_PROP]);
            vm.selectedIds.splice(index, 1);
        }
    }
}]);

angular.module('cms.shared').directive('cmsPager', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { 
            result: '=cmsResult',
            query: '=cmsQuery'
        },
        templateUrl: modulePath + 'UIComponents/Search/Pager.html',
        controller: angular.noop,
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    /* Link */

    function link(scope, el, attributes) {
        var vm = scope.vm;
        init();

        /* Init */

        function init() {

            /* Actions */
            vm.setPage = setPage;
        }

        /* Actions */

        function setPage(pageNumber) {
            if (!vm.query) return;

            vm.query.update({
                pageNumber: pageNumber
            });
        }

        /* Watches*/
        scope.$watch('vm.result', function (newResult) {
            if (!newResult) {
                vm.isFirstPage = true;
                vm.isLastPage = true;
            }
            else {
                vm.isFirstPage = newResult.pageNumber <= 1
                vm.isLastPage = newResult.pageNumber === newResult.pageCount;
            }
        });
    }

}]);
angular.module('cms.shared').directive('cmsSearchFilter', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        scope: { 
            query: '=cmsQuery',
            filter: '=cmsFilter',
            ngShow: '='
        },
        templateUrl: modulePath + 'UIComponents/Search/SearchFilter.html',
        transclude: true,

        controller: angular.noop,
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    /* Link */

    function link(scope, el, attributes) {
        var vm = scope.vm;
        init();

        /* Init */

        function init() {

            /* Properties */
            if (_.isUndefined(vm.ngShow)) vm.ngShow = true;

            /* Actions */
            vm.setFilter = setFilter;
            vm.cancel = cancel;
        }

        /* Actions */

        function setFilter() {
            vm.query.update(vm.filter);

            vm.ngShow = true;
        }

        function cancel() {
            vm.ngShow = false;
            vm.query.clear();
        }
    }
}]);
angular.module('cms.shared').factory('shared.SearchQuery', ['$location', '_', function ($location, _) {
    var pagingDefaultConfig = {
        pageSize: 20,
        pageNumber: 1
    };

    return SearchQuery;

    /**
     * Represents a query for searching entities and returning a paged result, handling
     * the persistance of the query parameters in the query string.
     */
    function SearchQuery(options) {
        var me = this,
            useHistory = _.isUndefined(options.useHistory) || options.useHistory,
            defaultParams = _.defaults({}, options.defaultParams, pagingDefaultConfig),
            filters = options.filters || [],
            searchParams = _.defaults({}, parseSearchParams(), defaultParams);

        /* Public Funcs */

        /**
         * Gets an object containing all the query params including filters and 
         * paging parameters.
         */
        me.getParameters = function () {
            return searchParams;
        }

        /**
         * Gets an object containing the filter portion of the query params.
         */
        me.getFilters = function () {
            return _.omit(searchParams, Object.keys(pagingDefaultConfig));
        }

        /**
         * Updates the query parameters.
         */
        me.update = function (query) {
            var newParams = _.defaults({}, query, pagingDefaultConfig, searchParams);
            setParams(newParams);
        }

        /**
         * Resets the query to the defaul parameters
         */
        me.clear = function () {
            setParams(defaultParams);
        }

        /* Private Funcs */

        function parseSearchParams() {
            if (!useHistory) return {};

            var params = $location.search();

            _.each(params, function (value, key) {
                // parse numbers
                if (!isNaN(value)) {
                    params[key] = parseInt(value);
                }
            });

            return params;
        }

        function setParams(params) {
            searchParams = params;

            if (useHistory) {
                // filter out default params so they dont appear in the query string
                qsParams = _.omit(params, function (value, key) {
                    return defaultParams[key] === value || !value;
                });

                $location.search(qsParams);
            }

            if (options.onChanged) options.onChanged(me);
        }
    }
}]);
/**
 * A success status message, dislpayed in a green coloured box
 */
angular.module('cms.shared').directive('cmsSuccessMessage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/StatusMessages/SuccessMessage.html',
        replace: true,
        transclude: true
    };
}]);

/**
 * A warning status message, dislpayed in a yellow coloured box
 */
angular.module('cms.shared').directive('cmsWarningMessage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/StatusMessages/WarningMessage.html',
        replace: true,
        transclude: true
    };
}]);

angular.module('cms.shared').directive('cmsTableActions', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Table/TableActions.html',
        replace: true,
        transclude: true,
        link: function (scope, el, attrs, controllers, transclude) {

        }
    };
}]);
angular.module('cms.shared').directive('cmsTableCellCreatedAuditData', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { auditData: '=cmsAuditData' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellCreatedAuditData.html'
    };
}]);
angular.module('cms.shared').directive('cmsTableCellImage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { image: '=cmsImage' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellImage.html'
    };
}]);
angular.module('cms.shared').directive('cmsTableCellUpdatedAuditData', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { auditData: '=cmsAuditData' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellUpdatedAuditData.html'
    };
}]);
angular.module('cms.shared').directive('cmsTableColumnActions', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs, ctrls) {
            element.addClass("actions");
        }
    }
});
angular.module('cms.shared').directive('cmsTableContainer', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Table/TableContainer.html',
        replace: true,
        transclude: true,
        link: function (scope, el, attrs, controllers, transclude) {

            el.find('table').addClass('table');

            //transclude(scope, function (clone) {
            //    clone.find('table').addClass('table');
            //    el.append(clone);
            //});
        }
    };
}]);
angular.module('cms.shared').directive('cmsTableGroupHeading', function () {
    return {
        restrict: 'E',
        template: '<h5 class="table-group-heading" ng-transclude></h5>',
        replace: true,
        transclude: true
    }
});
angular.module('cms.shared').directive('cmsTableRowInactive', function () {
    return {
        restrict: 'A',
        scope: {
            tableRowInactive: '=cmsTableRowInactive',
        },
        link: function (scope, element, attrs, ctrls) {

            scope.$watch('tableRowInactive', function (isTableRowInactive) {
                element.toggleClass("inactive", !!isTableRowInactive);
            });
        }
    }
});
/**
 * Formfield wrapper around a TagInput
 */
angular.module('cms.shared').directive('cmsFormFieldTags', [
    '_',
    'shared.internalModulePath',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    stringUtilities,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/Tags/FormFieldTags.html',
        passThroughAttributes: [
            'required'
        ]
    };

    return baseFormFieldFactory.create(config);
}]);
angular.module('cms.shared').directive('cmsTagList', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { tags: '=cmsTags' },
        require: '?^^cmsModalDialogContainer',
        templateUrl: modulePath + 'UIComponents/Tags/TagList.html',
        link: link
    };

    function link(scope, el, attributes, modalDialogContainerController) {
        if (modalDialogContainerController) {
            scope.isInModal = true;
        }
    }
}]);
/**
 * Allows inputting of tags in a comma delimited format. Only
 * allows alpha-numeric and '()&-_ characters.
 */
angular.module('cms.shared').directive('cmsTagsInput', [
    '_',
    'shared.stringUtilities',
function (
    _,
    stringUtilities
    ) {

    /* CONSTANTS */

    var CHAR_BLACKLIST = /[^,&\w\s'()-]+/g,
        TAG_DELIMITER = ', ';

    /* CONFIG */

    return {
        restrict: 'A',
        require: 'ngModel',
        link: link
    };

    /* LINK */
    
    function link(scope, el, attributes, ngModelController) {
        var controller = this,
            vm = scope.vm;

        init();

        /* Init */

        function init() {
            ngModelController.$formatters.push(formatTags);
            ngModelController.$parsers.push(parseTags);
            ngModelController.$render = function () {
                el.val(ngModelController.$viewValue || '');
            };

            el.on('keypress', onKeyPress);
            el.on('blur change keyup', onValueChanged);
        }

        /* Formatter/Parser */

        function formatTags(tagsArray) {
            if (!tagsArray || !tagsArray.length) return '';

            return _.map(tagsArray, function (tag) {
                return stringUtilities.capitaliseFirstLetter(tag.replace(getBadTagRegex(), '').trim());
            }).join(TAG_DELIMITER);
        }

        function parseTags(tagString) {
            var nonEmptyTags, allTags;

            allTags = tagString
                .replace(getBadTagRegex(), '')
                .split(',')
                .map(function (s) { return s.trim() });

            nonEmptyTags = _.filter(allTags, function (tag) {
                return tag && tag !== ',';
            });

            if (!nonEmptyTags.length) {
                return null;
            }

            return nonEmptyTags;
        }

        /* Events */

        function onValueChanged(e) {
            cleanInputValue();
            scope.$evalAsync(setModelValue);
        }

        function onKeyPress(e) {
            var charToTest = String.fromCharCode(e.which);

            // if the key entered isn't on the whitelist, ignore it 
            if (getBadTagRegex().test(charToTest)) {
                e.preventDefault();
            }
        }

        /* Helpers */

        function cleanInputValue() {
            var value = el.val(),
                cleanedValue = value.replace(getBadTagRegex(), '');

            if (value != cleanedValue) {
                el.val(cleanedValue);
            }
        }

        function setModelValue() {
            var value = el.val();
            ngModelController.$setViewValue(value);
        }

        function getBadTagRegex() {
            CHAR_BLACKLIST.lastIndex = 0;
            return CHAR_BLACKLIST;
        }
    }
}]);
angular.module('cms.shared').directive('cmsTimeAgo', ['shared.internalModulePath', function (modulePath) {

    return {
        restrict: 'E',
        scope: { time: '=cmsTime' },
        templateUrl: modulePath + 'UIComponents/Time/TimeAgo.html',
        controller: ['$scope', TimeAgoController],
        controllerAs: 'vm'
    };

    function TimeAgoController($scope) {
        var vm = this;

        $scope.$watch('time', function () {
            if ($scope.time) {
                vm.date = new Date($scope.time);
                vm.timeAgo = timeSince(vm.date);
            } else {
                vm.date = null;
                vm.timeAgo = null;
            }
        });
    }

    function timeSince(time) {

        switch (typeof time) {
            case 'number': break;
            case 'string': time = +new Date(time); break;
            case 'object': if (time.constructor === Date) time = time.getTime(); break;
            default: time = +new Date();
        }

        var time_formats = [
            [60, 'seconds', 1], // 60
            [120, '1 minute ago', '1 minute from now'], // 60*2
            [3600, 'minutes', 60], // 60*60, 60
            [7200, '1 hour ago', '1 hour from now'], // 60*60*2
            [86400, 'hours', 3600], // 60*60*24, 60*60
            [172800, 'Yesterday', 'Tomorrow'], // 60*60*24*2
            [604800, 'days', 86400], // 60*60*24*7, 60*60*24
            [1209600, 'Last week', 'Next week'], // 60*60*24*7*4*2
            [2419200, 'weeks', 604800], // 60*60*24*7*4, 60*60*24*7
            [4838400, 'Last month', 'Next month'], // 60*60*24*7*4*2
            [29030400, 'months', 2419200], // 60*60*24*7*4*12, 60*60*24*7*4
            [58060800, 'Last year', 'Next year'], // 60*60*24*7*4*12*2
            [2903040000, 'years', 29030400], // 60*60*24*7*4*12*100, 60*60*24*7*4*12
            [5806080000, 'Last century', 'Next century'], // 60*60*24*7*4*12*100*2
            [58060800000, 'centuries', 2903040000] // 60*60*24*7*4*12*100*20, 60*60*24*7*4*12*100
        ];
        var seconds = (+new Date() - time) / 1000,
            token = 'ago', list_choice = 1;

        // browser time may not be in sync with server time so treat
        // negative minutes as 'just now' to avoid outputting future times.
        if ((seconds <= 0 && seconds > -3600) || Math.abs(seconds) == 0) {
            return 'Just now'
        }
        if (seconds < 0) {
            seconds = Math.abs(seconds);
            token = 'from now';
            list_choice = 2;
        }
        var i = 0, format;
        while (format = time_formats[i++])
            if (seconds < format[0]) {
                if (typeof format[2] == 'string')
                    return format[list_choice];
                else
                    return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
            }
        return time;
    }
}]);
angular.module('cms.shared').directive('cmsUserLink', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: { user: '=cmsUser' },
        templateUrl: modulePath + 'UIComponents/User/UserLink.html',
        controller: controller,
        controllerAs: 'vm',
        bindToController: true
    };

    function controller() {
        var vm = this;

        vm.urlLibrary = urlLibrary;
    }
}]);
/**
 * If this element is in a modal popup or in a form in edit mode, then add a target="_blank" attribute
 * so that links open in a new tab
 */
angular.module('cms.shared').directive('cmsAutoTargetBlank', function () {

    return {
        restrict: 'A',
        require: ['?^^cmsModalDialogContainer', '?^^cmsForm'],
        link: link
    };

    function link(scope, el, attributes, controllers) {
        var modalDialogContainerController = controllers[0],
            formController = controllers[1];

        if (modalDialogContainerController) {
            el.attr('target', '_blank');
        } else if (formController) {
            scope.formScope = formController.getFormScope();

            // watches
            scope.$watch('formScope.editMode', function () {
                if (scope.formScope.editMode) {
                    el.attr('target', '_blank');
                } else {
                    el.removeAttr('target');
                }
            });
        }
    }
});
/**
 * Use this to turn a string date into a date object for ng model binding.
 */
angular.module('cms.shared').directive('cmsModelAsDate', function () {

    return {
        require: 'ngModel',
        link: link
    };

    function link(scope, elem, attr, ngModelController) {
        ngModelController.$formatters.push(function (modelValue) {
            return modelValue ? new Date(modelValue) : null;
        });
    }
});
/**
 * Allows a user to search/select a vimeo video. By default this maps to the VimeoVideo c#
 * object and allows editing of the title/description field, but if you set the cms-model-type attribute
 * to be 'id' then this will map to a simple id field and will not allow editing of the title/description fields.
 */
angular.module('cms.shared').directive('cmsFormFieldVimeo', [
    '_',
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.modalDialogService',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    contentPath,
    modalDialogService,
    stringUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noImagePath = assetReplacementPath + 'vimeo-replacement.png',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/Vimeo/FormFieldVimeo.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            modelType: '@cmsModelType'
        }),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.isRemovable = vm.model && !isRequired;

            setButtonText();
        }

        /* EVENTS */

        function remove() {
            setVideo(null);
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/Vimeo/VimeoPickerDialog.html',
                controller: 'VimeoPickerDialogController',
                options: {
                    currentVideo: _.clone(vm.model),
                    modelType: vm.modelType,
                    onSelected: setVideo
                }
            });
        }

        /**
         * Initialise the state when the video is changed
         */
        function setVideo(video) {

            if (video) {
                vm.isRemovable = !isRequired;
                vm.model = video;
            } else {
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = null;
                }
            }

            setButtonText();
        }

        /* Helpers */

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Select';
        }
    }

}]);
/**
 * Directive that allows a user to enter a vimeo Url or Id which will be
 * verified and then used to get the video information from vimeo, which
 * is then passed to the optional cmsOnVideoSelected scope function.
 * Does not support non-edit mode since so far it's only used in the 
 * VimeoPickerDialog.
 */
angular.module('cms.shared').directive('cmsFormFieldVimeoId', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.vimeoService',
    'shared.validationErrorService',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    vimeoService,
    validationErrorService,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/Vimeo/FormFieldVimeoId.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            onVideoSelected: '&cmsOnVideoSelected'
        }),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            formController = controllers[0];

        init();
        return baseFormFieldFactory.defaultConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.setEditing = toggleEditing.bind(null, true);
            vm.updateVideoId = updateVideoId;
            vm.cancelEditing = cancelEditing;

            vm.updateIdLoadState = new LoadState();

            scope.$watch('vm.model', function (newValue) {
                toggleEditing(!newValue);
            });
        }

        /* ACTIONS */

        function updateVideoId() {
            var inputId = vm.idOrUrlInput,
                videoId = parseVideoId(inputId);

            if (!inputId) {
                vm.model = null;
                triggerOnVideoSelected(null);

            } else if (inputId && !videoId) {
                addError('The url/id is invalid');
            }
            else if (!videoId || videoId == vm.model) {
                cancelEditing();
            }  else {

                vm.updateIdLoadState.on();
                vimeoService
                    .getVideoInfo(videoId)
                    .then(onInfoLoaded)
                    .catch(onFail)
                    .finally(vm.updateIdLoadState.off);
            }

            function onFail(response) {
                addError('There was a problem accessing Vimeo');
            }

            function onInfoLoaded(info) {
                if (info) {
                    vm.model = vm.idOrUrlInput = info.id;

                    triggerOnVideoSelected(info);
                } else {
                    addError('Video not found');
                }
            }

            function triggerOnVideoSelected(info) {
                if (vm.onVideoSelected) vm.onVideoSelected({ model: info })
            }

            function addError(message) {
                validationErrorService.raise([{
                    properties: [vm.modelName],
                    message: message
                }]);
            }
        }

        function cancelEditing() {
            vm.idOrUrlInput = vm.model;
            vm.onChange();
            toggleEditing(false);
        }

        /* Helpers */

        function toggleEditing(isEditing) {
            vm.isEditing = isEditing;
        }

        function parseVideoId(urlOrId) {
            var urlRegex = /^.*(vimeo\.com\/)((channels\/[A-z]+\/)|(groups\/[A-z]+\/videos\/))?([0-9]+)/,
                matches;

            if (!urlOrId) return;

            if (/^\d+$/.test(urlOrId)) {
                return urlOrId;
            }

            matches = urlRegex.exec(urlOrId);
            return matches && matches[5];
        }
    }
}]);
angular.module('cms.shared').controller('VimeoPickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.vimeoService',
    'options',
    'close',
function (
    $scope,
    LoadState,
    stringUtilities,
    vimeoService,
    options,
    close) {

    var vm = $scope;
    init();

    /* INIT */

    function init() {

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.onVideoSelected = onVideoSelected;
        vm.isModelId = options.modelType === 'id';
        vm.loadState = new LoadState();

        if (vm.isModelId && options.currentVideo) {
            vm.loadState.on();
            vimeoService
                .getVideoInfo(options.currentVideo)
                .then(onVideoSelected)
                .finally(vm.loadState.off);
        } else {
            vm.model = options.currentVideo;
        }
    }

    /* ACTIONS */

    function onVideoSelected(model) {

        if (model) {
            vm.model = {
                id: model.id,
                title: model.title,
                description: stringUtilities.stripTags(model.description),
                width: model.width,
                height: model.height,
                uploadDate: model.upload_date,
                duration: model.duration,
            };
        } else {
            vm.model = null;
        }
    }

    function onCancel() {
        close();
    }

    function onOk() {
        if (vm.model && vm.isModelId) {
            options.onSelected(vm.model.id);
        } else {
            options.onSelected(vm.model);
        }
        close();
    }
}]);

/**
 * Displays a vimeo video preview. Model can be an object with an id or the video id itself.
 */
angular.module('cms.shared').directive('cmsVimeoVideo', [
    '$sce',
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.urlLibrary',
function (
    $sce,
    modulePath,
    contentPath,
    urlLibrary) {

    return {
        restrict: 'E',
        scope: {
            model: '=cmsModel'
        },
        templateUrl: modulePath + 'UIComponents/Vimeo/VimeoVideo.html',
        link: function (scope, el, attributes) {

            scope.replacementUrl = contentPath + 'img/assetreplacement/vimeo-replacement.png';
            scope.$watch('model', function (model) {
                var id;
                if (model) {
                    id = model.id || model;
                    scope.videoUrl = $sce.trustAsResourceUrl('//player.vimeo.com/video/' + id)
                } else {
                    scope.videoUrl = null;
                }
            });
        }
    };
}]);
/**
 * Allows a user to search/select a vimeo video. By default this maps to the VimeoVideo c#
 * object and allows editing of the title/description field, but if you set the cms-model-type attribute
 * to be 'id' then this will map to a simple id field and will not allow editing of the title/description fields.
 */
angular.module('cms.shared').directive('cmsFormFieldYoutube', [
    '_',
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.modalDialogService',
    'shared.stringUtilities',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    contentPath,
    modalDialogService,
    stringUtilities,
    baseFormFieldFactory) {

    /* VARS */

    var assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noImagePath = assetReplacementPath + 'youtube-replacement.png',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/YouTube/FormFieldYouTube.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            modelType: '@cmsModelType'
        }),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.isRemovable = vm.model && !isRequired;

            setButtonText();
        }

        /* EVENTS */

        function remove() {
            setVideo(null);
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/YouTube/YouTubePickerDialog.html',
                controller: 'YoutubePickerDialogController',
                options: {
                    currentVideo: _.clone(vm.model),
                    modelType: vm.modelType,
                    onSelected: setVideo
                }
            });
        }

        /**
         * Initialise the state when the video is changed
         */
        function setVideo(video) {

            if (video) {
                vm.isRemovable = !isRequired;
                vm.model = video;
            } else {
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = null;
                }
            }

            setButtonText();
        }

        /* Helpers */

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Select';
        }
    }

}]);
/**
 * Directive that allows a user to enter a YouTube Url or Id which will be
 * verified and then used to get the video information from YouTube, which
 * is then passed to the optional cmsOnVideoSelected scope function.
 * Does not support non-edit mode since so far it's only used in the 
 * YouTubePickerDialog.
 */
angular.module('cms.shared').directive('cmsFormFieldYoutubeId', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.youtubeService',
    'shared.validationErrorService',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    YouTubeService,
    validationErrorService,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/YouTube/FormFieldYouTubeId.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            onVideoSelected: '&cmsOnVideoSelected'
        }),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            formController = controllers[0];

        init();
        return baseFormFieldFactory.defaultConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {
            vm.setEditing = toggleEditing.bind(null, true);
            vm.updateVideoId = updateVideoId;
            vm.cancelEditing = cancelEditing;

            vm.updateIdLoadState = new LoadState();

            scope.$watch('vm.model', function (newValue) {
                toggleEditing(!newValue);
            });
        }

        /* ACTIONS */

        function updateVideoId() {
            var inputId = vm.idOrUrlInput,
                videoId = parseVideoId(inputId);

            if (!inputId) {
                vm.model = null;
                triggerOnVideoSelected(null);

            } else if (inputId && !videoId) {
                addError('The url/id is invalid');
            }
            else if (!videoId || videoId == vm.model) {
                cancelEditing();
            }  else {

                vm.updateIdLoadState.on();
                youTubeService
                    .getVideoInfo(videoId)
                    .then(onInfoLoaded)
                    .catch(onFail)
                    .finally(vm.updateIdLoadState.off);
            }

            function onFail(response) {
                addError('There was a problem accessing YouTube');
            }

            function onInfoLoaded(info) {
                if (info) {
                    vm.model = vm.idOrUrlInput = info.id;

                    triggerOnVideoSelected(info);
                } else {
                    addError('Video not found');
                }
            }

            function triggerOnVideoSelected(info) {
                if (vm.onVideoSelected) vm.onVideoSelected({ model: info })
            }

            function addError(message) {
                validationErrorService.raise([{
                    properties: [vm.modelName],
                    message: message
                }]);
            }
        }

        function cancelEditing() {
            vm.idOrUrlInput = vm.model;
            vm.onChange();
            toggleEditing(false);
        }

        /* Helpers */

        function toggleEditing(isEditing) {
            vm.isEditing = isEditing;
        }

        function parseVideoId(urlOrId) {
            var urlRegex = /(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i,
                matches;

            if (!urlOrId) return;

            if (/^\d+$/.test(urlOrId)) {
                return urlOrId;
            }

            matches = urlRegex.exec(urlOrId);
            return matches && matches[5];
        }
    }
}]);
angular.module('cms.shared').controller('YoutubePickerDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.youtubeService',
    'options',
    'close',
function (
    $scope,
    LoadState,
    stringUtilities,
    YouTubeService,
    options,
    close) {

    var vm = $scope;
    init();

    /* INIT */

    function init() {

        vm.onOk = onOk;
        vm.onCancel = onCancel;
        vm.close = onCancel;
        vm.onVideoSelected = onVideoSelected;
        vm.isModelId = options.modelType === 'id';
        vm.loadState = new LoadState();

        if (vm.isModelId && options.currentVideo) {
            vm.loadState.on();
            youTubeService
                .getVideoInfo(options.currentVideo)
                .then(onVideoSelected)
                .finally(vm.loadState.off);
        } else {
            vm.model = options.currentVideo;
        }
    }

    /* ACTIONS */

    function onVideoSelected(model) {

        if (model) {
            vm.model = {
                id: model.id,
                title: model.title,
                description: stringUtilities.stripTags(model.description),
                width: model.width,
                height: model.height,
                uploadDate: model.upload_date,
                duration: model.duration,
            };
        } else {
            vm.model = null;
        }
    }

    function onCancel() {
        close();
    }

    function onOk() {
        if (vm.model && vm.isModelId) {
            options.onSelected(vm.model.id);
        } else {
            options.onSelected(vm.model);
        }
        close();
    }
}]);

/**
 * Displays a YouTube video preview. Model can be an object with an id or the video id itself.
 */
angular.module('cms.shared').directive('cmsYoutubeVideo', [
    '$sce',
    'shared.internalModulePath',
    'shared.contentPath',
    'shared.urlLibrary',
function (
    $sce,
    modulePath,
    contentPath,
    urlLibrary) {

    return {
        restrict: 'E',
        scope: {
            model: '=cmsModel'
        },
        templateUrl: modulePath + 'UIComponents/YouTube/YouTubeVideo.html',
        link: function (scope, el, attributes) {

            scope.replacementUrl = contentPath + 'img/assetreplacement/youtube-replacement.png';
            scope.$watch('model', function (model) {
                var id;
                if (model) {
                    id = model.id || model;
                    scope.videoUrl = $sce.trustAsResourceUrl('http://www.youtube.com/embed/' + id);
                } else {
                    scope.videoUrl = null;
                }
            });
        }
    };
}]);