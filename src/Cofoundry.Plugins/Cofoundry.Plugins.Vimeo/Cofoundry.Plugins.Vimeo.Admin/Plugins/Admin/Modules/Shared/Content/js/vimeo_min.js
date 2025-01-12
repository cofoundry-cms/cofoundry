angular.module("cms.shared").factory("shared.vimeoService",["$http","$q","shared.errorService",function(e,t,i){var s={};return s.getVideoInfo=function(e){return s="https://vimeo.com/api/oembed.json?url=https%3A%2F%2Fvimeo.com%2F"+e,o=t.defer(),(e=new XMLHttpRequest).addEventListener("load",function(){var e=this,s=!1,t="";switch(e.status){case 200:break;case 404:t="You aren't able to access the video because of privacy or permissions issues, or because the video is still transcoding.";break;case 403:t="Embed permissions are disabled for this video, so you can't embed it.";break;default:s=!0,t="Something unexpected happened whilst connecting to the Vimeo API."}{var r;t.length?(r={title:"Vimeo API Error",message:t,response:e},s&&i.raise(r),o.reject(r)):o.resolve(e)}}),e.open("GET",s),e.send(),o.promise.then(function(e){return JSON.parse(e.responseText)});var s,o},s}]);
angular.module("cms.shared").directive("cmsFormFieldVimeo",["_","shared.pluginModulePath","shared.pluginContentPath","shared.modalDialogService","shared.stringUtilities","baseFormFieldFactory",function(s,c,e,u,o,l){var h=l.defaultConfig,e={templateUrl:c+"UIComponents/FormFieldVimeo.html",scope:s.extend(l.defaultConfig.scope,{modelType:"@cmsModelType"}),passThroughAttributes:["required"],link:function(e,o,l,t){var i=e.vm,n=s.has(l,"required");return i.showPicker=m,i.remove=r,i.isRemovable=i.model&&!n,a(),h.link(e,o,l,t);function r(){d(null)}function m(){u.show({templateUrl:c+"UIComponents/VimeoPickerDialog.html",controller:"VimeoPickerDialogController",options:{currentVideo:s.clone(i.model),modelType:i.modelType,onSelected:d}})}function d(e){e?(i.isRemovable=!n,i.model=e):(i.isRemovable=!1,i.model&&(i.model=null)),a()}function a(){i.buttonText=i.model?"Change":"Select"}}};return l.create(e)}]);
angular.module("cms.shared").directive("cmsFormFieldVimeoId",["_","shared.pluginModulePath","shared.LoadState","shared.vimeoService","shared.validationErrorService","baseFormFieldFactory",function(l,e,u,c,s,m){e={templateUrl:e+"UIComponents/FormFieldVimeoId.html",scope:l.extend(m.defaultConfig.scope,{onVideoSelected:"&cmsOnVideoSelected"}),passThroughAttributes:["required"],link:function(e,n,d,i){var o=e.vm;l.has(d,"required"),i[0];return function(){o.setEditing=a.bind(null,!0),o.updateVideoId=t,o.cancelEditing=r,o.updateIdLoadState=new u,e.$watch("vm.model",function(e){a(!e)})}(),m.defaultConfig.link(e,n,d,i);function t(){var e=o.idOrUrlInput,n=function(e){if(!e)return;if(/^\d+$/.test(e))return e;return(e=/^.*(vimeo\.com\/)((channels\/[A-z]+\/)|(groups\/[A-z]+\/videos\/))?([0-9]+)/.exec(e))&&e[5]}(e);function d(e){o.onVideoSelected&&o.onVideoSelected({model:e})}function i(e){s.raise([{properties:[o.modelName],message:e}])}e?e&&!n?i("The url/id is invalid"):n&&n!=o.model?(o.updateIdLoadState.on(),c.getVideoInfo(n).then(function(e){e?(o.model=o.idOrUrlInput=e.video_id,d(e)):i("Video not found")}).catch(function(e){i(e.message)}).finally(o.updateIdLoadState.off)):r():(o.model=null,d(null))}function r(){o.idOrUrlInput=o.model,o.onChange(),a(!1)}function a(e){o.isEditing=e}}};return m.create(e)}]);
angular.module("cms.shared").controller("VimeoPickerDialogController",["$scope","shared.LoadState","shared.stringUtilities","shared.vimeoService","options","close",function(e,i,t,o,d,l){var n=e;function a(e){n.model=e?{id:e.video_id,title:e.title,description:t.stripTags(e.description),width:e.width,height:e.height,uploadDate:e.upload_date,duration:e.duration,thumbnailUrl:e.thumbnail_url,thumbnailWidth:e.thumbnail_width,thumbnailHeight:e.thumbnail_height}:null}function r(){l()}function h(){n.model&&n.isModelId?d.onSelected(n.model.id):d.onSelected(n.model),l()}n.onOk=h,n.onCancel=r,n.close=r,n.onVideoSelected=a,n.isModelId="id"===d.modelType,n.loadState=new i,n.isModelId&&d.currentVideo?(n.loadState.on(),o.getVideoInfo(d.currentVideo).then(a).finally(n.loadState.off)):n.model=d.currentVideo}]);
angular.module("cms.shared").directive("cmsVimeoVideo",["$sce","shared.pluginModulePath","shared.pluginContentPath","shared.urlLibrary",function(r,e,t,l){return{restrict:"E",scope:{model:"=cmsModel"},templateUrl:e+"UIComponents/VimeoVideo.html",link:function(l,e,o){l.replacementUrl=t+"img/AssetReplacement/vimeo-replacement.png",l.$watch("model",function(e){e?(e=e.id||e,l.videoUrl=r.trustAsResourceUrl("//player.vimeo.com/video/"+e)):l.videoUrl=null})}}}]);