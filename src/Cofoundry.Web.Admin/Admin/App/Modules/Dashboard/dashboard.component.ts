import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { LoadState } from '../shared/components';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
	selector: 'cms-dashboard-component',
	inputs: [
		'title',
		'entityname',
		'entitynameplural',
		'listurl',
		'createurl',
		'loader',
		'numitems'
	],
	templateUrl: 'dashboard.component.html',
})
export class DashboardComponent {
	title: string;
	entityname: string;
	entitynameplural: string;
	listurl: string;
	createurl: string;
	loader: LoadState;
	numitems: number;

	constructor(private _ngbActiveModal: NgbActiveModal) { }

	close() {
		this._ngbActiveModal.dismiss();
	}
}
