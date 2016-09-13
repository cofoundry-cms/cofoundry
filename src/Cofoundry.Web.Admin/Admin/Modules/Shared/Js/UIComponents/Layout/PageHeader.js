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