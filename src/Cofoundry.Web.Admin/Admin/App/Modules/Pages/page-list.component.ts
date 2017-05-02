import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { LoadState } from '../shared/components';
import { SearchQuery } from '../shared/components/search/search-query.component';
import { PageService } from './page.service';
import { PageTemplateService } from './page-template.service';
//import { EntityVersionModalDialogService } from '../shared/services/entity-version-modal-dialog.service';

@Component({
  selector: 'cms-pages',
  templateUrl: 'page-list.component.html',
  providers: [PageService, PageTemplateService]
})
export class PageListComponent implements OnInit {
    isFilterVisible: boolean;
    gridLoadState: LoadState;
    globalLoadState: LoadState;
    query: SearchQuery;
    filter;
    workFlowStatus;
    pageTemplates;
    result;

    constructor(private pageService: PageService,
                private pageTemplateService: PageTemplateService) {
        this.gridLoadState = new LoadState(false);
        this.globalLoadState = new LoadState(false);
    }

    ngOnInit() {
        this.loadFilterData();

        this.query = new SearchQuery({
            onChanged: this.onQueryChanged
        });
        this.filter = this.query.getFilters();

        this.toggleFilter(false);
        this.loadGrid();
    }

    /* ACTIONS */

    toggleFilter(show) {
        this.isFilterVisible = _.isUndefined(show) ? !this.isFilterVisible : show;
    }

    publish(pageId) {
        /*
        this.entityVersionModalDialogService
            .publish(pageId, this.globalLoadState.on)
            .subscribe(() => { this.loadGrid })
            .catch(this.globalLoadState.off);
        */
    }

    /* EVENTS */

    onQueryChanged() {
        this.toggleFilter(false);
        this.loadGrid();
    }

    /* PRIVATE FUNCS */

    loadFilterData() {
        this.workFlowStatus = [{
            name: 'Draft'
        }, {
            name: 'Published'
        }];

        this.pageTemplateService
            .getAll()
            .subscribe((pageTemplates) => {
                this.pageTemplates = pageTemplates;
            });
    }

    loadGrid() {
        this.gridLoadState.on();

        return this.pageService
            .getAll(this.query.getParameters())
            .subscribe((result) => {
                this.result = result;
                this.gridLoadState.off();
            });
    }

}