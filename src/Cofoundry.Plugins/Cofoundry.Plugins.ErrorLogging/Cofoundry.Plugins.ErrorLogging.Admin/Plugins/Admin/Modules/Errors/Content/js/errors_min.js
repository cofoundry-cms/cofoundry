angular.module("cms.errors",["ngRoute","cms.shared"]).constant("_",window._).constant("errors.modulePath","/Plugins/Admin/Modules/Errors/Js/");
angular.module("cms.errors").config(["$routeProvider","shared.routingUtilities","errors.modulePath",function(r,o,i){r.when("/:id",o.mapOptions(i,"ErrorDetails")).otherwise(o.mapOptions(i,"ErrorList"))}]);
angular.module("cms.errors").factory("errors.errorService",["$http","shared.pluginServiceBase",function(e,r){var t={},n=r+"errors";return t.getAll=function(r){return e.get(n,{params:r})},t.getById=function(r){return e.get(n+"/"+r)},t}]);
angular.module("cms.errors").controller("ErrorDetailsController",["$routeParams","shared.LoadState","errors.errorService",function(e,r,o){var t=this;t.editMode=!1,t.formLoadState=new r(!0),function(){var r=e.id;return o.getById(r).then(function(r){t.error=r})}().then(t.formLoadState.off)}]);
angular.module("cms.errors").controller("ErrorListController",["_","shared.LoadState","shared.SearchQuery","errors.errorService",function(r,e,t,i){var n=this;function o(e){n.isFilterVisible=r.isUndefined(e)?!n.isFilterVisible:e}function a(){o(!1),l()}function l(){return n.gridLoadState.on(),i.getAll(n.query.getParameters()).then(function(e){n.result=e,n.gridLoadState.off()})}n.gridLoadState=new e,n.query=new t({onChanged:a}),n.filter=n.query.getFilters(),n.toggleFilter=o,o(!1),l()}]);