import { Component, OnInit, Input, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { LoadState, Form, FormFieldTextboxComponent } from '../shared/components';
import { DashboardService } from './dashboard.service';
import { UrlLibrary } from '../shared/utilities/url-library.utility';
import { ModalService, ModalOptionsArgs } from '../shared/services';
import { TempComponent } from '../shared/components/temp.component';

@Component({
  selector: 'cms-dashboard',
  templateUrl: 'dashboard-list.component.html',
  providers: [DashboardService, UrlLibrary]
})
export class DashboardListComponent implements OnInit {
    pages = [];
    draftPages = [];
    pageTemplates = [];
    users = [];
    
    constructor(
        private dashboardService: DashboardService,
        private urlLibrary: UrlLibrary,
        private modalService: ModalService) {
        this.urlLibrary = urlLibrary;
    }
    
    ngOnInit() {
        this.loadGrid('getPages', 'pages');
        this.loadGrid('getDraftPages', 'draftPages');
        this.loadGrid('getPageTemplates', 'pageTemplates');
        //this.loadGrid('getUsers', 'users');
    }

    loadGrid(queryExecutor, resultProperty) {
        var loadState = new LoadState(true);
        this[resultProperty + 'LoadState'] = loadState;

        return this.dashboardService[queryExecutor]()
            .subscribe((result) => {
                this[resultProperty] = (result.json().data.items).slice(0, 5);
                loadState.off();
            });
    }

    onModalOpen(event) {
        //let args = new ModalOptionsArgs();
        //this.modalService.open(TempComponent, args);
        alert('Click');
    }

    onSubmit() {
        alert('Hi');
    }
}