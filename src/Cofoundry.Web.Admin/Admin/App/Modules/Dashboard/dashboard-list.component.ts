import { Component, OnInit } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { LoadState } from '../shared/components';
import { DashboardService } from './dashboard.service';
import { UrlLibrary } from '../shared/utilities/url-library.utility';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

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
	closeResult: string;

	constructor(
		private dashboardService: DashboardService,
		private urlLibrary: UrlLibrary,
		private modalService: NgbModal) {
		this.urlLibrary = urlLibrary;
	}

	ngOnInit() {
		this.loadGrid('getPages', 'pages');
		this.loadGrid('getDraftPages', 'draftPages');
		this.loadGrid('getPageTemplates', 'pageTemplates');
		// this.loadGrid('getUsers', 'users');
	}

	loadGrid(queryExecutor, resultProperty) {
		let loadState = new LoadState(true);
		this[resultProperty + 'LoadState'] = loadState;

		return this.dashboardService[queryExecutor]()
			.subscribe((result) => {
				this[resultProperty] = (result.json().data.items).slice(0, 5);
				loadState.off();
			});
	}

	open(content) {
		let options: NgbModalOptions = { };
		this.modalService.open(DashboardComponent, options).result.then((result) => {
			this.closeResult = `Closed with: ${result}`;
		}, (reason) => {
			this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
		});
	}

	private getDismissReason(reason: any): string {
		if (reason === ModalDismissReasons.ESC) {
			return 'by pressing ESC';
		} else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
			return 'by clicking on a backdrop';
		} else {
			return  `with: ${reason}`;
		}
	}

	onSubmit() {

	}
}
