angular.module('cms.pageTemplates').factory('pageTemplates.pageTemplateService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pageTemplateServiceBase = serviceBase + 'page-templates',
        pageTemplateFilesServiceBase = serviceBase + 'page-template-files';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pageTemplateServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(getIdRoute(imageId));
    }

    service.getFiles = function (query) {
        query.excluderegistered = true;

        return $http.get(pageTemplateFilesServiceBase, {
            params: query
        });
    }

    service.parseFile = function (filePath) {

        return $http.get(pageTemplateFilesServiceBase + '/parse?path=' + encodeURIComponent(filePath));
    }

    /* COMMANDS */

    service.add = function (command) {
        return $http.post(pageTemplateServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.pageTemplateId), command);
    }

    service.remove = function (id) {

        return $http.delete(getIdRoute(id));
    }

    service.addSection = function (command) {
        return $http.post(getSectionRoute(command.pageTemplateId), command);
    }

    service.updateSection = function (command) {

        return $http.patch(getSectionIdRoute(command.pageTemplateId, command.pageTemplateSectionId), command);
    }

    service.removeSection = function (pageTemplateId, pageTemplateSectionId) {

        return $http.delete(getSectionIdRoute(pageTemplateId, pageTemplateSectionId));
    }

    service.updateSectionModuleTypes = function (command) {
        var route = getSectionIdRoute(command.pageTemplateId, command.pageTemplateSectionId) + '/UpdateModuleTypes/';
        return $http.put(route, command);
    }

    /* PRIVATES */

    function getIdRoute(id) {
        return pageTemplateServiceBase + '/' + id;
    }

    function getSectionRoute(pageTemplateId, pageTemplateSectionId) {
        return getIdRoute(pageTemplateId) + '/sections';
    }

    function getSectionIdRoute(pageTemplateId, pageTemplateSectionId) {
        return getSectionRoute(pageTemplateId) + '/' + pageTemplateSectionId;
    }

    return service;
}]);