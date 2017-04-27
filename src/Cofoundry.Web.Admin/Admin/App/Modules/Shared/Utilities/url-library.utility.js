"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var path_constants_1 = require("../constants/path.constants");
var string_utility_1 = require("../utilities/string.utility");
var _ = require("lodash");
var UrlLibrary = (function () {
    function UrlLibrary(stringUtility) {
        this.stringUtility = stringUtility;
        /* CRUD Routes */
        this.addCrudRoutes('page', 'pages');
        this.addCrudRoutes('pageTemplate', 'page-templates');
        this.addCrudRoutes('role', 'roles');
    }
    ;
    /* General */
    UrlLibrary.prototype.makePath = function (module, pathParts, query) {
        var path = path_constants_1.URL_BASE_BASE + module + '#/';
        if (_.isArray(pathParts)) {
            path += pathParts.join('/');
        }
        else if (pathParts != null) {
            path += pathParts;
        }
        if (query) {
            path += '?' + this.stringUtility.toQueryString(query);
        }
        return path;
    };
    /* Asset File Routes */
    UrlLibrary.prototype.getDocumentUrl = function (document) {
        var url;
        if (!document)
            return;
        url = '/assets/files/' + document.documentAssetId + '_' + document.fileName + '.' + document.fileExtension;
        return url;
    };
    UrlLibrary.prototype.getImageUrl = function (img, settings) {
        var url;
        if (!img)
            return;
        url = '/assets/images/' + img.imageAssetId + '_' + img.fileName + '.' + img.extension;
        setDefaultCrop(img, settings);
        if (settings) {
            url = url + '?' + this.stringUtility.toQueryString(settings);
        }
        return url;
        /* Helpers */
        function setDefaultCrop(asset, settings) {
            if (!settings)
                return;
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
    UrlLibrary.prototype.login = function () {
        return path_constants_1.URL_BASE_BASE + 'auth';
    };
    /* Custom Entities */
    UrlLibrary.prototype.customEntityList = function (customEntityDefinition) {
        return this.makePath(this.stringUtility.slugify(customEntityDefinition.name), null, null);
    };
    UrlLibrary.prototype.customEntityDetails = function (customEntityDefinition, id) {
        return this.makePath(this.stringUtility.slugify(customEntityDefinition.name), id, null);
    };
    /* Users */
    UrlLibrary.prototype.userDetails = function (userArea, id) {
        return this.makePath(this.getGetAreaPath(userArea), id, null);
    };
    UrlLibrary.prototype.userList = function (userArea, query) {
        return this.makePath(this.getGetAreaPath(userArea), null, query);
    };
    UrlLibrary.prototype.userNew = function (userArea, query) {
        return this.makePath(this.getGetAreaPath(userArea), 'new', query);
    };
    /* Private Helpers */
    UrlLibrary.prototype.getGetAreaPath = function (userArea) {
        var userAreaName = userArea ? userArea.name : 'cms';
        return this.stringUtility.slugify(userAreaName) + '-users';
    };
    UrlLibrary.prototype.addCrudRoutes = function (entity, modulePath) {
        var _this = this;
        this[entity + 'List'] = function (query) {
            return _this.makePath(modulePath, null, query);
        };
        this[entity + 'New'] = function (query) {
            return this.makePath(modulePath, 'new', query);
        };
        this[entity + 'Details'] = function (id) {
            return this.makePath(modulePath, id);
        };
    };
    return UrlLibrary;
}());
UrlLibrary = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [string_utility_1.StringUtility])
], UrlLibrary);
exports.UrlLibrary = UrlLibrary;
;
//# sourceMappingURL=url-library.utility.js.map